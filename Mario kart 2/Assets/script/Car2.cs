using UnityEngine;

public class Car2 : MonoBehaviour
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
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float enginePower = 1500f;
    [SerializeField] private float maxSpeed = 25f;

    [Header("Brakes")]
    [SerializeField] private float brakeForce = 8000f;

    [Header("Physics")]
    [SerializeField] private float downforce = 50f;
    [SerializeField] private float drag = 0.02f;

    private Rigidbody rb;

    private float throttle;
    private float steer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.6f, 0);
    }

    private void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;
        float forwardSpeed = Vector3.Dot(rb.linearVelocity, transform.forward);

        float targetThrottle = 0f;
        float targetSteer = 0f;

            if (Input.GetKey(KeyCode.UpArrow)) targetThrottle = 0.5f;
            if (Input.GetKey(KeyCode.DownArrow)) targetThrottle = -0.5f;

            if (Input.GetKey(KeyCode.RightArrow)) targetSteer = 0.5f;
            if (Input.GetKey(KeyCode.LeftArrow)) targetSteer = -0.5f;

        throttle = Mathf.Lerp(throttle, targetThrottle, Time.fixedDeltaTime * 2f);
        steer = Mathf.Lerp(steer, targetSteer, Time.fixedDeltaTime * 4f);


        float motorTorque = throttle * enginePower;

        if (speed < maxSpeed)
        {
            _colliderBL.motorTorque = motorTorque;
            _colliderBR.motorTorque = motorTorque;
        }
        else
        {
            _colliderBL.motorTorque = 0f;
            _colliderBR.motorTorque = 0f;
        }

        // -------- STEERING --------
        float t = speed / maxSpeed;
        t = Mathf.Clamp01(t);
        float speedSteerLimit = Mathf.Lerp(maxSteerAngle, 6f, t);
        float steerAngle = speedSteerLimit * steer;
        
        _colliderFL.steerAngle = steerAngle;
        _colliderFR.steerAngle = steerAngle;

        // -------- BRAKES --------
        if (Input.GetKey(KeyCode.Space) || (throttle < 0 && forwardSpeed > 1f))
        {
            _colliderFL.brakeTorque = brakeForce * 0.6f;
            _colliderFR.brakeTorque = brakeForce * 0.6f;
            _colliderBL.brakeTorque = brakeForce * 0.4f;
            _colliderBR.brakeTorque = brakeForce * 0.4f;
        }
        else
        {
            _colliderFL.brakeTorque = 0f;
            _colliderFR.brakeTorque = 0f;
            _colliderBL.brakeTorque = 0f;
            _colliderBR.brakeTorque = 0f;
        }

        // -------- DOWNFORCE --------
        rb.AddForce(-transform.up * speed * downforce);

        // -------- DRAG --------
        rb.linearVelocity *= (1f - drag * Time.fixedDeltaTime);

        // -------- VISUAL --------
        UpdateWheel(_colliderFL, _transformFL);
        UpdateWheel(_colliderFR, _transformFR);
        UpdateWheel(_colliderBL, _transformBL);
        UpdateWheel(_colliderBR, _transformBR);
    }

    private void UpdateWheel(WheelCollider collider, Transform wheel)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheel.position = pos;
        wheel.rotation = rot;
    }
}
