using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ArcadeCar : MonoBehaviour
{
    public enum Axel { Front, Rear }

    [Serializable]
    public struct Wheel
    {
        public Transform wheelModel;           // визуальная модель
        public WheelCollider wheelCollider;    // физическое колесо
        public Axel axel;                      // ось колеса
        [Header("Optional Effects")]
        public GameObject wheelEffectObj;      // TrailRenderer
        public ParticleSystem smokeParticle;   // дым
    }

    [Header("Arcade Settings")]
    public float maxForwardSpeed = 20f;
    public float maxReverseSpeed = 10f;
    public float acceleration = 600f;        // моторная сила
    public float brakeForce = 300f;          // сила тормоза
    public float turnSpeed = 30f;            // максимальный угол поворота
    public float driftBoost = 1.4f;          // усиление поворота при дрифте
    public float driftSide = 2.5f;           // боковое смещение при дрифте
    public float driftLerp = 6f;             // плавность бокового смещения
    public float steeringAssist = 5f;        // авто-подруливание
    public float minTurnScale = 0.5f;        // минимальный коэффициент поворота

    [Header("Wheels")]
    public List<Wheel> wheels = new List<Wheel>();

    private float motorInput;
    private float steerInput;
    private bool brakeInput;

    private Vector3 lateralOffset;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.centerOfMass = new Vector3(0, -0.5f, 0); // немного ниже для устойчивости
    }

    void Update()
    {
        motorInput = Keyboard.current.wKey.isPressed ? 1f :
                     Keyboard.current.sKey.isPressed ? -1f : 0f;

        steerInput = Keyboard.current.aKey.isPressed ? -1f :
                     Keyboard.current.dKey.isPressed ? 1f : 0f;

        brakeInput = Keyboard.current.spaceKey.isPressed;

        AnimateWheels();
        WheelEffects();
    }

    void FixedUpdate()
    {
        Move();
        Steer();
        ApplyDrift();
        AutoAlign();
    }

    private void Move()
    {
        float speed = rb.linearVelocity.magnitude;
        float maxSpeed = motorInput >= 0 ? maxForwardSpeed : maxReverseSpeed;

        foreach (var wheel in wheels)
        {
            if (wheel.wheelCollider == null) continue;

            // Мотор только для задних колес
            if (wheel.axel == Axel.Rear)
                wheel.wheelCollider.motorTorque = motorInput * acceleration * Time.fixedDeltaTime;

            // Тормоз
            float brake = (brakeInput || motorInput == 0f) ? brakeForce * Time.fixedDeltaTime : 0f;
            wheel.wheelCollider.brakeTorque = brake;
        }
    }

    private void Steer()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxForwardSpeed;
        float turnScale = Mathf.Lerp(1.2f, minTurnScale, Mathf.Clamp01(speedFactor));

        // Задний ход — немного меньше поворот
        if (motorInput < 0)
            turnScale *= 0.7f;

        float steerAngle = steerInput * turnSpeed * turnScale;

        // Усиление при дрифте
        if (brakeInput)
            steerAngle *= driftBoost;

        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front && wheel.wheelCollider != null)
            {
                wheel.wheelCollider.steerAngle = Mathf.Lerp(
                    wheel.wheelCollider.steerAngle, steerAngle, 0.6f
                );
            }
        }
    }

    private void ApplyDrift()
{
    if (!brakeInput || Mathf.Abs(steerInput) < 0.01f || rb.linearVelocity.magnitude < 0.1f)
    {
        lateralOffset = Vector3.zero;
        return;
    }

    // Сила бокового смещения зависит от скорости
    float driftStrength = driftSide * (rb.linearVelocity.magnitude / maxForwardSpeed);
    Vector3 targetSide = transform.right * steerInput * driftStrength;

    lateralOffset = Vector3.Lerp(lateralOffset, targetSide, driftLerp * Time.fixedDeltaTime);

    // Применяем только пропорционально скорости
    rb.linearVelocity += lateralOffset;
}

    private void AutoAlign()
    {
        if (!brakeInput && rb.linearVelocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, steeringAssist * Time.fixedDeltaTime);
        }
    }

    private void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.wheelCollider == null || wheel.wheelModel == null) continue;
            wheel.wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            wheel.wheelModel.position = pos;
            wheel.wheelModel.rotation = rot;
        }
    }

    private void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel != Axel.Rear) continue;
            if (wheel.wheelCollider == null) continue;

            bool shouldEmit = brakeInput && wheel.wheelCollider.isGrounded && rb.linearVelocity.magnitude >= 10f;

            if (wheel.wheelEffectObj != null)
            {
                var trail = wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>();
                if (trail != null) trail.emitting = shouldEmit;
            }

            if (shouldEmit && wheel.smokeParticle != null)
                wheel.smokeParticle.Emit(1);
        }
    }
}
