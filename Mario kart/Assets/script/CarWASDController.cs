using UnityEngine;
using UnityEngine.InputSystem;

public class CarWASDController_NewInput : MonoBehaviour
{
    [Header("WheelColliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Visual wheel meshes (optional)")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Speed / Power (HARD)")]
    public float motorTorque = 5200f;     // ОЧЕНЬ бодро
    public float maxSpeedKmh = 260f;
    public float lowSpeedBoost = 1.2f;    // +120% тяги на старте

    [Header("Steering (HARD)")]
    public float maxSteerAngleLow = 48f;  // на малой скорости
    public float maxSteerAngleHigh = 22f; // на высокой скорости
    public float steerResponse = 18f;     // резче

    [Header("Brakes")]
    public float brakeTorque = 4200f;
    public float autoBrakeTorque = 2200f; // когда отпустил W/S

    [Header("Handling / Stability")]
    public float antiRoll = 9000f;
    public float extraDownforce = 60f;    // прижим к дороге
    public float sideGripMultiplier = 1.25f; // “липкость” вбок (1.1-1.4)

    [Header("Rigidbody")]
    public Rigidbody rb;
    public Vector3 centerOfMass = new Vector3(0, -0.65f, 0);

    float steerCurrent;

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (rb) rb.centerOfMass = centerOfMass;

        // усиливаем боковое сцепление (поворотливость и меньше занос)
        ApplySideGrip(frontLeft, sideGripMultiplier);
        ApplySideGrip(frontRight, sideGripMultiplier);
        ApplySideGrip(rearLeft, sideGripMultiplier);
        ApplySideGrip(rearRight, sideGripMultiplier);
    }

    void FixedUpdate()
    {
        var kb = Keyboard.current;
        if (kb == null || rb == null) return;

        float v = 0f;
        if (kb.wKey.isPressed) v += 1f;
        if (kb.sKey.isPressed) v -= 1f;

        float h = 0f;
        if (kb.dKey.isPressed) h += 1f;
        if (kb.aKey.isPressed) h -= 1f;

        bool braking = kb.spaceKey.isPressed;
        bool autoBrake = Mathf.Approximately(v, 0f);

        float speedKmh = rb.linearVelocity.magnitude * 3.6f;
        float speed01 = Mathf.Clamp01(speedKmh / maxSpeedKmh);

        // ТЯГА: очень сильная на старте, падает к максимуму скорости
        float boost = 1f + lowSpeedBoost * (1f - speed01);
        float torque = (speedKmh < maxSpeedKmh) ? v * motorTorque * boost : 0f;

        // РУЛЬ: на малой скорости сильно, на высокой меньше
        float steerMax = Mathf.Lerp(maxSteerAngleLow, maxSteerAngleHigh, speed01);
        float steerTarget = h * steerMax;
        steerCurrent = Mathf.Lerp(steerCurrent, steerTarget, steerResponse * Time.fixedDeltaTime);

        frontLeft.steerAngle = steerCurrent;
        frontRight.steerAngle = steerCurrent;

        // Задний привод
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;

        // Тормоза
        float bt = 0f;
        if (braking) bt = brakeTorque;
        else if (autoBrake) bt = autoBrakeTorque;

        frontLeft.brakeTorque = bt;
        frontRight.brakeTorque = bt;
        rearLeft.brakeTorque = bt;
        rearRight.brakeTorque = bt;

        // Прижим к дороге (стабильность на скорости)
        rb.AddForce(-transform.up * extraDownforce * rb.linearVelocity.magnitude);

        // Анти-ролл (чтобы не кувыркалась в резких поворотах)
        ApplyAntiRoll(frontLeft, frontRight);
        ApplyAntiRoll(rearLeft, rearRight);

        UpdateWheel(frontLeft, frontLeftMesh);
        UpdateWheel(frontRight, frontRightMesh);
        UpdateWheel(rearLeft, rearLeftMesh);
        UpdateWheel(rearRight, rearRightMesh);
    }

    void ApplySideGrip(WheelCollider wc, float mul)
    {
        if (!wc) return;
        var s = wc.sidewaysFriction;
        s.stiffness *= mul;
        wc.sidewaysFriction = s;
    }

    void ApplyAntiRoll(WheelCollider left, WheelCollider right)
    {
        if (!left || !right || !rb) return;

        float travelL = 1f, travelR = 1f;

        bool groundedL = left.GetGroundHit(out WheelHit hitL);
        if (groundedL)
            travelL = (-left.transform.InverseTransformPoint(hitL.point).y - left.radius) / left.suspensionDistance;

        bool groundedR = right.GetGroundHit(out WheelHit hitR);
        if (groundedR)
            travelR = (-right.transform.InverseTransformPoint(hitR.point).y - right.radius) / right.suspensionDistance;

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedL) rb.AddForceAtPosition(left.transform.up * -antiRollForce, left.transform.position);
        if (groundedR) rb.AddForceAtPosition(right.transform.up * antiRollForce, right.transform.position);
    }

    void UpdateWheel(WheelCollider col, Transform mesh)
    {
        if (!col || !mesh) return;
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
