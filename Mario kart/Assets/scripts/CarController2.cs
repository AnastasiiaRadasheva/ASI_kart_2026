using UnityEngine;

public class CarController2 : MonoBehaviour
{
    [Header("Wheel Transforms")]
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
    [SerializeField] private float _maxSteerAngle = 30f;
    [SerializeField] private float enginePower = 1200f; // уменьшили силу двигателя
    [SerializeField] private float maxSpeed = 15f; // максимальная скорость (м/с)

    [Header("Brakes")]
    [SerializeField] private float brakeForce = 6000f;

    private Rigidbody rb;
    private float _vertical = 0f;
    private float _horizontal = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.6f, 0); // низкий центр тяжести
    }

    private void FixedUpdate()
    {
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        // ---------- INPUT ----------
        float targetVertical = 0f;
        float targetHorizontal = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
            targetVertical = 1f;
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (forwardSpeed > 0.5f)
            {
                // тормозим перед движением назад
                ApplyBrakes(brakeForce);
                targetVertical = 0f;
            }
            else
            {
                targetVertical = -1f;
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
            targetHorizontal = 1f;
        else if (Input.GetKey(KeyCode.LeftArrow))
            targetHorizontal = -1f;

        _vertical = Mathf.MoveTowards(_vertical, targetVertical, Time.fixedDeltaTime * 3f);
        _horizontal = Mathf.MoveTowards(_horizontal, targetHorizontal, Time.fixedDeltaTime * 4f);

        // ---------- MOTOR ----------
        float speed = rb.linearVelocity.magnitude;

        float targetMotor = _vertical * enginePower;
        if (_vertical < 0f)
            targetMotor *= 0.4f; 

        // Плавный разгон
        float motor = Mathf.MoveTowards(_colliderFL.motorTorque, targetMotor, Time.fixedDeltaTime * 500f);

        // Ограничение скорости
        float maxAllowedSpeed = (_vertical < 0f) ? maxSpeed * 0.4f : maxSpeed;

        if (speed < maxAllowedSpeed)
            SetMotorTorque(motor);
        else
            SetMotorTorque(0f);

        // ---------- STEERING ----------
        float steerLimit = (_vertical < 0f) ? 15f : _maxSteerAngle;
        float steerBySpeed = Mathf.Lerp(steerLimit, 10f, speed / maxSpeed);
        float steerAngle = steerBySpeed * _horizontal;

        _colliderFL.steerAngle = steerAngle;
        _colliderFR.steerAngle = steerAngle;

        // ---------- BRAKE ----------
        if (Input.GetKey(KeyCode.Escape))
            ApplyBrakes(brakeForce);
        else if (_vertical >= 0)
            ApplyBrakes(0f);

        // ---------- VISUAL ----------
        RotateWheel(_colliderFL, _transformFL);
        RotateWheel(_colliderFR, _transformFR);
        RotateWheel(_colliderBL, _transformBL);
        RotateWheel(_colliderBR, _transformBR);
    }

    private void SetMotorTorque(float torque)
    {
        _colliderFL.motorTorque = torque;
        _colliderFR.motorTorque = torque;
        _colliderBL.motorTorque = torque;
        _colliderBR.motorTorque = torque;
    }

    private void ApplyBrakes(float force)
    {
        _colliderFL.brakeTorque = force;
        _colliderFR.brakeTorque = force;
        _colliderBL.brakeTorque = force;
        _colliderBR.brakeTorque = force;
    }

    private void RotateWheel(WheelCollider collider, Transform wheel)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheel.position = pos;
        wheel.rotation = rot;
    }
}
