using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(Rigidbody))]
public class CarAISpline : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer splineContainer;  // перетащи сюда объект со SplineContainer
    public int splineIndex = 0;              // если в контейнере несколько сплайнов
    public bool loop = true;

    [Tooltip("Насколько далеко вперед по сплайну целиться (в метрах).")]
    public float lookAheadDistance = 8f;

    [Tooltip("Как часто пересчитывать nearest point (сек).")]
    public float recomputeNearestEvery = 0.2f;

    [Header("Line Following")]
    public float lineFollowGain = 0.6f;   // возврат на линию (больше = сильнее тянет к линии)
    public float headingGain = 1.2f;      // держать направление линии
    public float maxLineOffset = 3f;      // ограничение ошибки (метры)
    public bool drawDebug = true;

    [Header("Model forward fix (if car goes sideways)")]
    public Vector3 localForwardAxis = Vector3.forward; // если едет боком: попробуй Vector3.right или Vector3.left

    [Header("Driving")]
    public float maxMotorTorque = 1500f;
    public float maxSteerAngle = 25f;
    public float maxSpeedKmh = 60f;

    [Header("Braking")]
    public float brakeTorque = 2500f;

    [Header("Steer Tuning (sharp)")]
    public float steerSharpness = 1.6f;
    public float lowSpeedSteerBoost = 1.8f;
    public float steerBoostFadeSpeed = 10f; // m/s
    [Range(0f, 1f)] public float throttleSteerReduction = 0.4f;

    [Header("Behind target behavior")]
    public bool fullLockWhenBehind = true;
    public float behindThreshold = -1.0f;

    [Header("Stability")]
    public bool useAntiRoll = true;
    public float antiRollForce = 8000f;
    public bool setCenterOfMass = true;
    public Vector3 centerOfMass = new Vector3(0f, -0.6f, 0f);

    [Header("Wheel Colliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Optional: visuals")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Obstacle Avoidance (simple)")]
    public bool avoidObstacles = true;
    public float sensorLength = 8f;
    public float sensorSideOffset = 0.8f;
    public float avoidSteerStrength = 1.2f;
    public LayerMask obstacleMask = ~0;

    Rigidbody rb;

    float tNearest = 0f;
    float splineLength = 1f;
    float nextNearestTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (setCenterOfMass)
            rb.centerOfMass = centerOfMass;

        if (rb.mass < 800f) rb.mass = 1000f;
        if (rb.angularDamping < 2f) rb.angularDamping = 3f;

        CacheSplineLength();
        RecomputeNearest();
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
            CacheSplineLength();
    }

    void CacheSplineLength()
    {
        if (splineContainer == null) return;
        if (splineIndex < 0) splineIndex = 0;
        if (splineIndex >= splineContainer.Splines.Count) splineIndex = 0;

        var spline = splineContainer.Splines[splineIndex];
        splineLength = Mathf.Max(0.01f, spline.GetLength());
    }

    void FixedUpdate()
    {
        if (splineContainer == null) return;

        if (Time.time >= nextNearestTime)
        {
            RecomputeNearest();
            nextNearestTime = Time.time + Mathf.Max(0.02f, recomputeNearestEvery);
        }

        float speedKmh = rb.linearVelocity.magnitude * 3.6f;
        bool overspeed = speedKmh > maxSpeedKmh;

        float dt = lookAheadDistance / Mathf.Max(0.01f, splineLength);
        float tTarget = tNearest + dt;
        if (loop) tTarget = Mathf.Repeat(tTarget, 1f);
        else tTarget = Mathf.Clamp01(tTarget);

        // точки на линии
        Vector3 pNearest = EvaluatePosition(tNearest);
        Vector3 pAhead = EvaluatePosition(tTarget);

        // направление линии (касательная)
        Vector3 lineDir = (pAhead - pNearest);
        lineDir.y = 0f;
        if (lineDir.sqrMagnitude < 0.001f) lineDir = ForwardWorld();
        else lineDir.Normalize();

        // "forward" машины с учётом localForwardAxis (чинит "едет боком")
        Vector3 carFwd = ForwardWorld();
        Vector3 carRight = transform.right;

        // ошибка по направлению (куда смотрим vs куда идёт линия)
        float headingErrDeg = Vector3.SignedAngle(carFwd, lineDir, Vector3.up);
        float headingErr = Mathf.Clamp(headingErrDeg / 45f, -1f, 1f);

        // ошибка от линии (слева/справа)
        Vector3 toCar = transform.position - pNearest;
        toCar.y = 0f;
        float crossTrack = Vector3.Dot(toCar, carRight);
        crossTrack = Mathf.Clamp(crossTrack, -maxLineOffset, maxLineOffset);

        // итоговое руление (line following)
        float steer01 = headingErr * headingGain + (crossTrack / Mathf.Max(0.01f, maxLineOffset)) * lineFollowGain;

        // чуть "заострим" как в твоём скрипте
        steer01 = Mathf.Clamp(steer01 * steerSharpness, -1f, 1f);

        // буст руля на низкой скорости
        float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / Mathf.Max(0.01f, steerBoostFadeSpeed));
        float boost = Mathf.Lerp(lowSpeedSteerBoost, 1f, speedFactor);
        steer01 = Mathf.Clamp(steer01 * boost, -1f, 1f);

        // если точка "впереди" оказалась сзади (редко, но бывает на старте/телепорте)
        // делаем полный лок в сторону линии
        Vector3 localLineAhead = transform.InverseTransformPoint(pAhead);
        if (fullLockWhenBehind && localLineAhead.z < behindThreshold)
        {
            steer01 = Mathf.Sign(localLineAhead.x);
            if (steer01 == 0f) steer01 = 1f;
        }

        float steerAngle = steer01 * maxSteerAngle;

        if (avoidObstacles)
        {
            float avoid = ObstacleAvoidanceSteer();
            steerAngle = Mathf.Clamp(steerAngle + avoid * maxSteerAngle, -maxSteerAngle, maxSteerAngle);
        }

        float throttle = 1f - Mathf.Clamp01(Mathf.Abs(steer01) * throttleSteerReduction);
        if (overspeed) throttle = 0f;
        if (fullLockWhenBehind && localLineAhead.z < behindThreshold) throttle *= 0.6f;

        ApplySteer(steerAngle);
        ApplyMotor(throttle);
        ApplyBrakes(overspeed);

        if (useAntiRoll)
        {
            AntiRoll(frontLeft, frontRight, antiRollForce);
            AntiRoll(rearLeft, rearRight, antiRollForce);
        }

        UpdateWheelVisuals();
    }

    void RecomputeNearest()
    {
        if (splineContainer == null) return;

        if (splineIndex < 0) splineIndex = 0;
        if (splineIndex >= splineContainer.Splines.Count) splineIndex = 0;

        var spline = splineContainer.Splines[splineIndex];

        float3 pos = transform.position;
        SplineUtility.GetNearestPoint(spline, pos, out _, out float t);
        tNearest = t;
    }

    Vector3 EvaluatePosition(float t)
    {
        var spline = splineContainer.Splines[splineIndex];
        float3 p = spline.EvaluatePosition(t);
        return (Vector3)p;
    }

    Vector3 ForwardWorld()
    {
        // "вперёд" с учётом того, что у модели ось может быть X вместо Z
        Vector3 fwd = transform.TransformDirection(localForwardAxis);
        fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.001f) return transform.forward;
        return fwd.normalized;
    }

    float ObstacleAvoidanceSteer()
    {
        Vector3 origin = transform.position + transform.up * 0.5f;
        Vector3 fwd = ForwardWorld(); // важно: используем исправленный forward

        float steerBias = 0f;
        int hits = 0;

        Vector3 leftOrigin = origin - transform.right * sensorSideOffset;
        Vector3 rightOrigin = origin + transform.right * sensorSideOffset;

        if (Physics.Raycast(origin, fwd, out _, sensorLength, obstacleMask))
        {
            bool leftFree = !Physics.Raycast(leftOrigin, fwd, sensorLength, obstacleMask);
            bool rightFree = !Physics.Raycast(rightOrigin, fwd, sensorLength, obstacleMask);

            if (leftFree && !rightFree) { steerBias -= 1f; hits++; }
            else if (rightFree && !leftFree) { steerBias += 1f; hits++; }
            else { steerBias += 1f; hits++; }
        }

        if (Physics.Raycast(leftOrigin, fwd, out _, sensorLength, obstacleMask))
        {
            steerBias += 0.8f; hits++;
        }
        if (Physics.Raycast(rightOrigin, fwd, out _, sensorLength, obstacleMask))
        {
            steerBias -= 0.8f; hits++;
        }

        if (hits == 0) return 0f;
        return Mathf.Clamp(steerBias / hits, -1f, 1f) * avoidSteerStrength;
    }

    void ApplySteer(float angle)
    {
        if (frontLeft) frontLeft.steerAngle = angle;
        if (frontRight) frontRight.steerAngle = angle;
    }

    void ApplyMotor(float throttle01)
    {
        float torque = throttle01 * maxMotorTorque;

        if (rearLeft) rearLeft.motorTorque = torque;
        if (rearRight) rearRight.motorTorque = torque;

        if (frontLeft) frontLeft.motorTorque = 0f;
        if (frontRight) frontRight.motorTorque = 0f;
    }

    void ApplyBrakes(bool braking)
    {
        float bt = braking ? brakeTorque : 0f;

        if (frontLeft) frontLeft.brakeTorque = bt;
        if (frontRight) frontRight.brakeTorque = bt;
        if (rearLeft) rearLeft.brakeTorque = bt;
        if (rearRight) rearRight.brakeTorque = bt;
    }

    void AntiRoll(WheelCollider left, WheelCollider right, float force)
    {
        if (!left || !right) return;

        WheelHit hit;
        float travelL = 1f;
        float travelR = 1f;

        if (left.GetGroundHit(out hit))
            travelL = (-left.transform.InverseTransformPoint(hit.point).y - left.radius) / left.suspensionDistance;

        if (right.GetGroundHit(out hit))
            travelR = (-right.transform.InverseTransformPoint(hit.point).y - right.radius) / right.suspensionDistance;

        float antiRoll = (travelL - travelR) * force;

        if (left.isGrounded)
            rb.AddForceAtPosition(left.transform.up * -antiRoll, left.transform.position);

        if (right.isGrounded)
            rb.AddForceAtPosition(right.transform.up * antiRoll, right.transform.position);
    }

    void UpdateWheelVisuals()
    {
        UpdateOneWheel(frontLeft, frontLeftMesh);
        UpdateOneWheel(frontRight, frontRightMesh);
        UpdateOneWheel(rearLeft, rearLeftMesh);
        UpdateOneWheel(rearRight, rearRightMesh);
    }

    void UpdateOneWheel(WheelCollider col, Transform mesh)
    {
        if (col == null || mesh == null) return;
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }

    void OnDrawGizmos()
    {
        if (!drawDebug || splineContainer == null) return;
        if (splineIndex < 0 || splineIndex >= splineContainer.Splines.Count) return;

        // если в редакторе ещё не посчитали длину — прикинем
        if (splineLength <= 0.01f)
        {
            var spline = splineContainer.Splines[splineIndex];
            splineLength = Mathf.Max(0.01f, spline.GetLength());
        }

        float dt = lookAheadDistance / Mathf.Max(0.01f, splineLength);
        float tTarget = loop ? Mathf.Repeat(tNearest + dt, 1f) : Mathf.Clamp01(tNearest + dt);

        Vector3 pN = EvaluatePosition(tNearest);
        Vector3 pT = EvaluatePosition(tTarget);

        Gizmos.color = Color.green;  Gizmos.DrawSphere(pN, 0.35f);
        Gizmos.color = Color.yellow; Gizmos.DrawSphere(pT, 0.35f);
        Gizmos.color = Color.white;  Gizmos.DrawLine(transform.position, pT);

        // линия касательной
        Vector3 tangent = (pT - pN); tangent.y = 0f;
        if (tangent.sqrMagnitude > 0.001f)
        {
            tangent.Normalize();
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pN, pN + tangent * 3f);
        }
    }
}
