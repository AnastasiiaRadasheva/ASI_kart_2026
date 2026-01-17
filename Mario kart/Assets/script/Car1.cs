using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Car1 : MonoBehaviour
{
    [Header("Wheel Transforms (Visual)")]
    [SerializeField] private Transform _transformFL;
    [SerializeField] private Transform _transformFR;
    [SerializeField] private Transform _transformBL;
    [SerializeField] private Transform _transformBR;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider _colliderFL;
    [SerializeField] private WheelCollider _colliderFR;
    [SerializeField] private WheelCollider _colliderBL;
    [SerializeField] private WheelCollider _colliderBR;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private float driftMultiplier = 0.8f;
    [SerializeField] private float sideGrip = 0.85f;

    private Rigidbody rb;

    private float motorInput;
    private float steerInput;
    private bool brakeInput;
    private bool handbrakeInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.6f, 0);
        DisableWheelColliderMotors();
    }

    private void DisableWheelColliderMotors()
    {
        _colliderFL.motorTorque = 0f; _colliderFL.brakeTorque = 0f;
        _colliderFR.motorTorque = 0f; _colliderFR.brakeTorque = 0f;
        _colliderBL.motorTorque = 0f; _colliderBL.brakeTorque = 0f;
        _colliderBR.motorTorque = 0f; _colliderBR.brakeTorque = 0f;
    }

    private void Update()
    {
        motorInput = Keyboard.current.wKey.isPressed ? 1f : (Keyboard.current.sKey.isPressed ? -1f : 0f);
        steerInput = Keyboard.current.aKey.isPressed ? -1f : (Keyboard.current.dKey.isPressed ? 1f : 0f);
        brakeInput = Keyboard.current.spaceKey.isPressed;
        handbrakeInput = Keyboard.current.yKey.isPressed;
    }

    private void FixedUpdate()
    {
        if (!IsOnGround()) return;

        if (motorInput != 0f)
            rb.AddForce(transform.forward * motorInput * acceleration, ForceMode.Acceleration);

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
            rb.linearVelocity = flatVel.normalized * maxSpeed + Vector3.up * rb.linearVelocity.y;

        if (Mathf.Abs(steerInput) > 0.01f && flatVel.magnitude > 0.5f)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, steerInput * turnSpeed * Time.fixedDeltaTime, 0f));

        if (brakeInput && Mathf.Abs(steerInput) > 0.01f)
            rb.AddForce(transform.right * steerInput * acceleration * driftMultiplier, ForceMode.Acceleration);

        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        float gripAmount = brakeInput ? 0.3f : sideGrip;
        localVel.x *= gripAmount;
        rb.linearVelocity = transform.TransformDirection(localVel);

        ApplyHandbrake();

        float wheelSpeed = rb.linearVelocity.magnitude * 360f;
        UpdateWheel(_transformFL, wheelSpeed);
        UpdateWheel(_transformFR, wheelSpeed);
        UpdateWheel(_transformBL, wheelSpeed);
        UpdateWheel(_transformBR, wheelSpeed);
    }

    private void ApplyHandbrake()
    {
        if (!handbrakeInput) return;

        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        localVel.z = Mathf.Lerp(localVel.z, 0f, 0.05f);
        localVel.x = Mathf.Lerp(localVel.x, 0f, 0.02f);
        rb.linearVelocity = transform.TransformDirection(localVel);
    }

    private bool IsOnGround()
    {
        float distanceToGround = 0.6f;
        return Physics.Raycast(transform.position, Vector3.down, distanceToGround + 0.1f);
    }

    private void UpdateWheel(Transform wheel, float speed)
    {
        if (wheel == null) return;

        wheel.Rotate(Vector3.right, speed * Time.fixedDeltaTime);

        if (wheel == _transformFL || wheel == _transformFR)
        {
            wheel.localRotation = Quaternion.Euler(
                wheel.localRotation.eulerAngles.x,
                steerInput * 30f,
                wheel.localRotation.eulerAngles.z
            );
        }
    }
}
