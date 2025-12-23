using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarAIWaypoint : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float reachDistance = 4f;
    public bool loop = true;

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
    [Tooltip("Если точка позади, даём полный руль для разворота")]
    public bool fullLockWhenBehind = true;
    [Tooltip("С какой 'глубины' в локальных координатах считать точку позади")]
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
    int index;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (setCenterOfMass)
            rb.centerOfMass = centerOfMass;

        if (rb.mass < 800f) rb.mass = 1000f;
        if (rb.angularDamping < 2f) rb.angularDamping = 3f; // Unity 6
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (index < 0) index = 0;
        if (index >= waypoints.Length) index = loop ? 0 : waypoints.Length - 1;

        // скорость
        float speedKmh = rb.linearVelocity.magnitude * 3.6f;
        bool overspeed = speedKmh > maxSpeedKmh;

        // текущая цель
        Vector3 target = waypoints[index].position;

        // расстояние по XZ (игнор Y) — важный фикс!
        float distXZ = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(target.x, target.z)
        );

        // если дошли — переключаемся (и можем пропустить сразу несколько близких)
        if (distXZ <= reachDistance)
        {
            AdvanceWaypoint();
            // пропуск близких точек, чтобы не "залипать"
            int safety = 0;
            while (safety++ < waypoints.Length)
            {
                target = waypoints[index].position;
                distXZ = Vector2.Distance(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(target.x, target.z)
                );
                if (distXZ > reachDistance) break;
                AdvanceWaypoint();
            }
        }

        // локальная цель
        Vector3 localTarget = transform.InverseTransformPoint(waypoints[index].position);

        // ---- STEER ----
        float steer;

        // если точка позади — разворачиваемся резче
        if (fullLockWhenBehind && localTarget.z < behindThreshold)
        {
            steer = Mathf.Sign(localTarget.x);
            if (steer == 0f) steer = 1f;
        }
        else
        {
            steer = localTarget.x / Mathf.Max(0.1f, localTarget.magnitude);
            steer = Mathf.Clamp(steer * steerSharpness, -1f, 1f);

            float speedFactor = Mathf.Clamp01(rb.linearVelocity.magnitude / Mathf.Max(0.01f, steerBoostFadeSpeed));
            float boost = Mathf.Lerp(lowSpeedSteerBoost, 1f, speedFactor);
            steer = Mathf.Clamp(steer * boost, -1f, 1f);
        }

        float steerAngle = steer * maxSteerAngle;

        // избегание препятствий
        if (avoidObstacles)
        {
            float avoid = ObstacleAvoidanceSteer();
            steerAngle = Mathf.Clamp(steerAngle + avoid * maxSteerAngle, -maxSteerAngle, maxSteerAngle);
        }

        // ---- THROTTLE ----
        float throttle = 1f - Mathf.Clamp01(Mathf.Abs(steer) * throttleSteerReduction);
        if (overspeed) throttle = 0f;

        // если точка позади — чуть притормозить, чтобы легче развернулась
        if (fullLockWhenBehind && localTarget.z < behindThreshold)
            throttle *= 0.6f;

        ApplySteer(steerAngle);
        ApplyMotor(throttle);
        ApplyBrakes(overspeed);

        // anti-roll
        if (useAntiRoll)
        {
            AntiRoll(frontLeft, frontRight, antiRollForce);
            AntiRoll(rearLeft, rearRight, antiRollForce);
        }

        UpdateWheelVisuals();
    }

    void AdvanceWaypoint()
    {
        index++;
        if (index >= waypoints.Length)
            index = loop ? 0 : waypoints.Length - 1;
    }

    float ObstacleAvoidanceSteer()
    {
        Vector3 origin = transform.position + transform.up * 0.5f;
        Vector3 fwd = transform.forward;

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
}
