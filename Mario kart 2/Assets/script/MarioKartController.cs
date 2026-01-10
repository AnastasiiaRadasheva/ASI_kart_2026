using UnityEngine;
using UnityEngine.InputSystem;

public class MarioKartController : MonoBehaviour
{
    [Header("Visual Wheels")]
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    [SerializeField] private Transform backLeftWheel;
    [SerializeField] private Transform backRightWheel;

    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float accelerationForce = 60f;
    [SerializeField] private float reverseSpeed = 15f;
    [SerializeField] private float naturalSlowdown = 0.97f;

    [Header("Turning Settings")]
    [SerializeField] private float turnStrength = 120f;
    [SerializeField] private float maxSteerAngle = 35f;
    [SerializeField] private float driftTurnBonus = 1.6f;

    [Header("Drift Settings")]
    [SerializeField] private bool enableDrift = true;
    [SerializeField] private float driftForce = 15f;
    [SerializeField] private float driftControl = 0.92f;
    [SerializeField] private float normalGrip = 0.75f;
    
    [Header("Physics")]
    [SerializeField] private float downForce = 50f;
    [SerializeField] private float carMass = 1200f;

    private Rigidbody rb;
    private float currentSpeed;
    private bool isDrifting;
    
    // Input values
    private float moveInput;
    private float turnInput;
    private bool driftInput;

    void Start()
    {
        SetupRigidbody();
    }

    void SetupRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Настройки Rigidbody для аркадной физики
        rb.mass = carMass;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.8f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.centerOfMass = new Vector3(0, -0.4f, 0);
        
        // Снимаем все ограничения
        rb.constraints = RigidbodyConstraints.None;
        
        Debug.Log("Mario Kart Controller Ready! Press W/S to move, A/D to turn, Space to drift");
    }

    void Update()
    {
        GetInput();
        AnimateWheels();
    }

    void GetInput()
    {
        // Движение вперед/назад
        moveInput = 0f;
        if (Keyboard.current.wKey.isPressed) moveInput = 1f;
        else if (Keyboard.current.sKey.isPressed) moveInput = -1f;

        // Поворот
        turnInput = 0f;
        if (Keyboard.current.aKey.isPressed) turnInput = -1f;
        else if (Keyboard.current.dKey.isPressed) turnInput = 1f;

        // Дрифт
        driftInput = enableDrift && Keyboard.current.spaceKey.isPressed;
        
        // Определяем активный дрифт
        isDrifting = driftInput && Mathf.Abs(turnInput) > 0.1f && currentSpeed > 3f;
    }

    void FixedUpdate()
    {
        currentSpeed = rb.linearVelocity.magnitude;

        ApplyAcceleration();
        ApplyTurning();
        ApplyDrift();
        ApplyGrip();
        ApplyDownforce();
        LimitSpeed();
    }

    void ApplyAcceleration()
    {
        if (moveInput > 0f)
        {
            // Ускорение вперёд
            Vector3 force = transform.forward * accelerationForce * moveInput;
            rb.AddForce(force, ForceMode.Acceleration);
        }
        else if (moveInput < 0f)
        {
            // Торможение или задний ход
            if (currentSpeed > 3f)
            {
                // Торможение
                rb.AddForce(-rb.linearVelocity.normalized * accelerationForce * 0.7f, ForceMode.Acceleration);
            }
            else
            {
                // Задний ход
                Vector3 reverseForce = transform.forward * accelerationForce * moveInput * 0.5f;
                rb.AddForce(reverseForce, ForceMode.Acceleration);
            }
        }
        else
        {
            // Естественное замедление
            rb.linearVelocity *= naturalSlowdown;
        }
    }

    void ApplyTurning()
    {
        if (Mathf.Abs(turnInput) > 0.01f && currentSpeed > 0.3f)
        {
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
            float turnPower = turnStrength;

            // Бонус к повороту при дрифте
            if (isDrifting)
            {
                turnPower *= driftTurnBonus;
            }
            else
            {
                // Нормальный поворот - отзывчивый на любой скорости
                turnPower *= Mathf.Lerp(0.6f, 1.2f, speedFactor);
            }

            // Применяем поворот через rotation
            float turnAmount = turnInput * turnPower * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    void ApplyDrift()
    {
        if (isDrifting)
        {
            // Боковая сила при дрифте (как в Mario Kart)
            Vector3 driftDirection = transform.right * turnInput;
            rb.AddForce(driftDirection * driftForce, ForceMode.Acceleration);
        }
    }

    void ApplyGrip()
    {
        // Управление боковым скольжением (grip)
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        
        if (isDrifting)
        {
            // Во время дрифта - больше скольжение
            localVelocity.x *= driftControl;
        }
        else
        {
            // Нормальная езда - хорошее сцепление
            localVelocity.x *= normalGrip;
        }
        
        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }

    void ApplyDownforce()
    {
        // Прижимная сила для стабильности
        if (currentSpeed > 1f)
        {
            rb.AddForce(-transform.up * downForce * currentSpeed, ForceMode.Acceleration);
        }
    }

    void LimitSpeed()
    {
        // Ограничение максимальной скорости
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        float speedLimit = moveInput < 0f ? reverseSpeed : maxSpeed;
        
        if (flatVelocity.magnitude > speedLimit)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * speedLimit;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    void AnimateWheels()
    {
        if (frontLeftWheel == null) return;

        // Вращение колёс от скорости
        float wheelRotationSpeed = currentSpeed * 50f * Mathf.Sign(Vector3.Dot(rb.linearVelocity, transform.forward));
        
        RotateWheel(frontLeftWheel, wheelRotationSpeed, true);
        RotateWheel(frontRightWheel, wheelRotationSpeed, true);
        RotateWheel(backLeftWheel, wheelRotationSpeed, false);
        RotateWheel(backRightWheel, wheelRotationSpeed, false);
    }

    void RotateWheel(Transform wheel, float speed, bool isFrontWheel)
    {
        if (wheel == null) return;

        // Вращение колеса вокруг своей оси
        wheel.Rotate(Vector3.right, speed * Time.deltaTime, Space.Self);

        // Поворот передних колёс
        if (isFrontWheel)
        {
            float steerAngle = turnInput * maxSteerAngle;
            
            // Увеличенный угол при дрифте
            if (isDrifting)
            {
                steerAngle *= 1.3f;
            }
            
            Vector3 currentRotation = wheel.localEulerAngles;
            wheel.localRotation = Quaternion.Euler(currentRotation.x, steerAngle, currentRotation.z);
        }
    }

    // Визуальная отладка
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Направление движения
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 3f);

        // Скорость
        if (rb != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, rb.linearVelocity);
        }

        // Статус дрифта
        if (isDrifting)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }

    // Публичные методы для расширения функционала
    public void ApplyBoost(float boostMultiplier, float duration)
    {
        StartCoroutine(BoostCoroutine(boostMultiplier, duration));
    }

    private System.Collections.IEnumerator BoostCoroutine(float multiplier, float duration)
    {
        float originalSpeed = maxSpeed;
        float originalAcceleration = accelerationForce;
        
        maxSpeed *= multiplier;
        accelerationForce *= multiplier;
        
        yield return new WaitForSeconds(duration);
        
        maxSpeed = originalSpeed;
        accelerationForce = originalAcceleration;
    }

    public float GetCurrentSpeed() => currentSpeed;
    public bool IsDrifting() => isDrifting;
    public float GetSpeedPercent() => Mathf.Clamp01(currentSpeed / maxSpeed);
}