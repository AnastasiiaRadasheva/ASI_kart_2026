using UnityEngine;
using UnityEngine.InputSystem;

public class CarController1 : MonoBehaviour
{
[SerializeField] private Transform _transformFL;
    [SerializeField] private Transform _transformFR;
    [SerializeField] private Transform _transformBL;
    [SerializeField] private Transform _transformBR;

    [SerializeField] private WheelCollider _colliderFL;
    [SerializeField] private WheelCollider _colliderFR;
    [SerializeField] private WheelCollider _colliderBL;
    [SerializeField] private WheelCollider _colliderBR;

    [SerializeField] private float _force;
    [SerializeField] private float _maxAngle = 4f;
    [SerializeField] private float enginePower = 600f;
    [SerializeField] private float maxSpeed = 6f;

    private Rigidbody rb;
    private float _vertical = 0f;
    private float _horizontal = 0f;
    public float downForce = 6f;

    private float baseEnginePower;
    private float baseMaxAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.4f, 0);

        baseEnginePower = enginePower;
        baseMaxAngle = _maxAngle;
    }

    private void FixedUpdate()
    {
        rb.AddForce(-transform.up * downForce * (rb.linearVelocity.magnitude / 20f));

        float targetVertical = 0f;
        float targetHorizontal = 0f;

        var kb = Keyboard.current;
        if(kb == null) return;

        if (kb.wKey.isPressed) targetVertical = 0.5f;
        if (kb.sKey.isPressed) targetVertical = -0.5f;

        if (kb.dKey.isPressed) targetHorizontal = 0.5f;
        if (kb.aKey.isPressed) targetHorizontal = -0.5f;

        _vertical = Mathf.MoveTowards(_vertical, targetVertical, Time.fixedDeltaTime * 5f);
        _horizontal = Mathf.MoveTowards(_horizontal, targetHorizontal, Time.fixedDeltaTime * 5f);

        float power = baseEnginePower;
        float steer = baseMaxAngle;

        if (_vertical < 0)
            power *= 0.35f;

        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            _colliderFL.motorTorque = _vertical * power;
            _colliderFR.motorTorque = _vertical * power;
        }
        else
        {
            _colliderFL.motorTorque = 0f;
            _colliderFR.motorTorque = 0f;
        }

        if (_vertical < 0)
            steer *= 0.25f;

        _colliderFL.steerAngle = steer * _horizontal;
        _colliderFR.steerAngle = steer * _horizontal;

        if (kb.zKey.isPressed)
        {
            _colliderFL.brakeTorque = 3000f;
            _colliderFR.brakeTorque = 3000f;
            _colliderBL.brakeTorque = 3000f;
            _colliderBR.brakeTorque = 3000f;
        }
        else
        {
            _colliderFL.brakeTorque = 0f;
            _colliderFR.brakeTorque = 0f;
            _colliderBL.brakeTorque = 0f;
            _colliderBR.brakeTorque = 0f;
        }

        RotateWheel(_colliderFL, _transformFL);
        RotateWheel(_colliderFR, _transformFR);
        RotateWheel(_colliderBL, _transformBL);
        RotateWheel(_colliderBR, _transformBR);
    }

        private void RotateWheel(WheelCollider collider, Transform transform)
    {
        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        transform.rotation = rotation;
        transform.position = position;
    }
    
}
