using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CarController1 : MonoBehaviour
{
    [Header("Wheel meshes (visual)")]
    [SerializeField] private Transform _transformFL;
    [SerializeField] private Transform _transformFR;
    [SerializeField] private Transform _transformBL;
    [SerializeField] private Transform _transformBR;

    [Header("Wheel colliders (physics)")]
    [SerializeField] private WheelCollider _colliderFL;
    [SerializeField] private WheelCollider _colliderFR;
    [SerializeField] private WheelCollider _colliderBL;
    [SerializeField] private WheelCollider _colliderBR;

    [Header("Driving")]
    [SerializeField] private float enginePower = 3000f;
    [SerializeField] private float maxSpeed = 7f; // м/с
    [SerializeField] private float _maxAngle = 25f;
    [SerializeField] private float steeringSmooth = 5f;

    [Header("Brakes")]
    [SerializeField] private float brakePower = 3000f;

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
        bool braking = false;

#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed) targetVertical = 1f;
            if (kb.sKey.isPressed) targetVertical = -1f;

            if (kb.dKey.isPressed) targetHorizontal = 1f;
            if (kb.aKey.isPressed) targetHorizontal = -1f;

            braking = kb.zKey.isPressed;
        }
#else
        if (Input.GetKey(KeyCode.W)) targetVertical = 1f;
        if (Input.GetKey(KeyCode.S)) targetVertical = -1f;

        if (Input.GetKey(KeyCode.D)) targetHorizontal = 1f;
        if (Input.GetKey(KeyCode.A)) targetHorizontal = -1f;

        braking = Input.GetKey(KeyCode.Z);
#endif

        // РУЛЬ со сглаживанием
        _horizontal = Mathf.MoveTowards(_horizontal, targetHorizontal, Time.fixedDeltaTime * steeringSmooth);

        // ГАЗ без "инерции": отпустила W/S -> сразу 0
        _vertical = targetVertical;

        // Мотор: если газ не нажат — не едем
        float motor = 0f;
        if (Mathf.Abs(_vertical) > 0.01f)
        {
            float speed = (rb != null) ? rb.linearVelocity.magnitude : 0f;
            if (speed < maxSpeed)
                motor = _vertical * enginePower;
        }

        // Мотор на все 4 колеса
        _colliderFL.motorTorque = motor;
        _colliderFR.motorTorque = motor;
        _colliderBL.motorTorque = motor;
        _colliderBR.motorTorque = motor;

        // Тормоз
        float brake = braking ? brakePower : 0f;
        _colliderFL.brakeTorque = brake;
        _colliderFR.brakeTorque = brake;
        _colliderBL.brakeTorque = brake;
        _colliderBR.brakeTorque = brake;

        // Поворот (передние колёса)
        _colliderFL.steerAngle = _maxAngle * _horizontal;
        _colliderFR.steerAngle = _maxAngle * _horizontal;

        // Обновление визуала
        RotateWheel(_colliderFL, _transformFL);
        RotateWheel(_colliderFR, _transformFR);
        RotateWheel(_colliderBL, _transformBL);
        RotateWheel(_colliderBR, _transformBR);
    }

    private void RotateWheel(WheelCollider collider, Transform transform)
    {
        if (collider == null || transform == null) return;

        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        transform.SetPositionAndRotation(position, rotation);
    }
}
