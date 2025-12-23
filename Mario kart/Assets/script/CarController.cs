using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public enum ControlMode { Keyboard, Buttons }
    public enum Axel { Front, Rear }

    [Serializable]
    public struct Wheel
    {
        public Transform wheelModel;           // Mesh_1
        public WheelCollider wheelCollider;    // WC_RP / WC_LP / WC_RZ / WC_LZ

        [Header("Optional Effects (can be empty)")]
        public GameObject wheelEffectObj;      // объект с TrailRenderer (можно null)
        public ParticleSystem smokeParticle;   // можно null

        public Axel axel;                      // Front/Rear
    }

    [Header("Control")]
    public ControlMode control = ControlMode.Keyboard;

    [Header("Engine / Brake")]
    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    [Header("Steering")]
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    [Header("Rigidbody")]
    public Vector3 centerOfMass;

    [Header("Wheels")]
    public List<Wheel> wheels = new List<Wheel>();

    private float moveInput;
    private float steerInput;

    private Rigidbody carRb;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        if (carRb != null) carRb.centerOfMass = centerOfMass;
    }

    void Update()
    {
        GetInputs();
        AnimateWheels();
        WheelEffects();
    }

    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    // Для UI-кнопок (если control = Buttons)
    public void MoveInput(float input) => moveInput = Mathf.Clamp(input, -1f, 1f);
    public void SteerInput(float input) => steerInput = Mathf.Clamp(input, -1f, 1f);

    void GetInputs()
    {
        if (control != ControlMode.Keyboard) return;

        var kb = Keyboard.current;
        if (kb == null)
        {
            moveInput = 0f;
            steerInput = 0f;
            return;
        }

        // Газ/тормоз: W/S или стрелки
        moveInput = 0f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) moveInput += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) moveInput -= 1f;

        // Руль: A/D или стрелки
        steerInput = 0f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) steerInput += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) steerInput -= 1f;
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.wheelCollider == null) continue;

            wheel.wheelCollider.motorTorque =
                moveInput * 600f * maxAcceleration * Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.wheelCollider == null) continue;

            if (wheel.axel == Axel.Front)
            {
                float steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle =
                    Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {
        bool spacePressed = false;

        var kb = Keyboard.current;
        if (kb != null) spacePressed = kb.spaceKey.isPressed;

        bool braking = spacePressed || moveInput == 0f;

        float brakeTorque = braking ? (300f * brakeAcceleration * Time.deltaTime) : 0f;

        foreach (var wheel in wheels)
        {
            if (wheel.wheelCollider == null) continue;
            wheel.wheelCollider.brakeTorque = brakeTorque;
        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.wheelModel == null || wheel.wheelCollider == null) continue;

            wheel.wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
            wheel.wheelModel.position = pos;
            wheel.wheelModel.rotation = rot;
        }
    }

    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel != Axel.Rear) continue;
            if (wheel.wheelCollider == null) continue;

            // Space на New Input System
            bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

            bool shouldEmit =
                spacePressed &&
                wheel.wheelCollider.isGrounded &&
                carRb != null &&
                carRb.linearVelocity.magnitude >= 10f;

            // Trail (если есть)
            if (wheel.wheelEffectObj != null)
            {
                var trail = wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>();
                if (trail != null) trail.emitting = shouldEmit;
            }

            // Дым (если есть)
            if (shouldEmit && wheel.smokeParticle != null)
            {
                wheel.smokeParticle.Emit(1);
            }
        }
    }
}
