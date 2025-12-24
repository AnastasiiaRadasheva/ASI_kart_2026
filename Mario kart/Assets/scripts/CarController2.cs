using UnityEngine;
using UnityEngine.InputSystem;

public class CarController2 : MonoBehaviour
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
    [SerializeField] private float _maxAngle;
    [SerializeField] private float enginePower = 3000f;
    [SerializeField] private float maxSpeed = 7f;

    private Rigidbody rb;
    private float _vertical = 0f;
    private float _horizontal = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float targetVertical = 0f;
        float targetHorizontal = 0f;

        var kb = Keyboard.current;
        if(kb == null) return;

        if (kb.upArrowKey.isPressed) targetVertical = 0.5f;
        if (kb.downArrowKey.isPressed) targetVertical = -0.5f;

        if (kb.rightArrowKey.isPressed) targetHorizontal = 0.5f;
        if (kb.leftArrowKey.isPressed) targetHorizontal = -0.5f;

        _vertical = Mathf.MoveTowards(_vertical, targetVertical, Time.fixedDeltaTime * 5f);
        _horizontal = Mathf.MoveTowards(_horizontal, targetHorizontal, Time.fixedDeltaTime * 5f);


        _colliderFL.motorTorque = _vertical * (enginePower * 0.5f);
        _colliderFR.motorTorque = _vertical * (enginePower * 0.5f);
        _colliderBL.motorTorque = _vertical * (enginePower * 0.5f);
        _colliderBR.motorTorque = _vertical * (enginePower * 0.5f);

        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            _colliderFL.motorTorque = _vertical * enginePower;
            _colliderFR.motorTorque = _vertical * enginePower;
        }
        else
        {
            _colliderFL.motorTorque = 0f;
            _colliderFR.motorTorque = 0f;
        }

        if (kb.escapeKey.isPressed)
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

        _colliderFL.steerAngle = _maxAngle * _horizontal;
        _colliderFR.steerAngle = _maxAngle * _horizontal;

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
