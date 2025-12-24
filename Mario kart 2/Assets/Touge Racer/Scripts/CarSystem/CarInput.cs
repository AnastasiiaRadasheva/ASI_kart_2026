using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

namespace Sain.TougeRacer
{
    [AddComponentMenu("Arcade Car/Car Input")]
    public class CarInput : MonoBehaviour
    {
        public enum InputType
        {
            User,
            AI
        }

        public bool active = true;

        public InputType inputType;

        [Header("Spline References")]
        [SerializeField] private SplineContainer racingSpline;
        [Range(0, 1)] public float splinePosition = 0f;
        private float splineLength;

        [Header("User Settings")]
        public bool autoAccel = false;

        private CarInputActions input;

        private InputAction accelAction;
        private InputAction brakeAction;
        private InputAction steerAction;
        private InputAction handbrakeAction;
        private InputAction headlightAction;
        private InputAction rightSignAction;
        private InputAction leftSignAction;
        private InputAction hazardAction;
        private InputAction resetAction;
        private InputAction pauseAction;

        [Header("AI Settings")]

        [Header("Steering Parameters")]
        public float steeringResponsiveness = 2f;

        [Header("Speed Control")]
        public float maxSpeed = 50f;
        public float minCorneringSpeed = 20f;

        [Header("Braking System")]
        public float brakeIntensity = 1f;
        public float brakeAngle = 70;
        public AnimationCurve brakeCurve;

        [Header("Look Ahead")]
        public float lookAheadDistance = 10f;
        public float lookAheadForBraking = 20f;

        [Header("Overtaking")]
        public LayerMask obstacleMask;
        public LayerMask wallMask;
        public float sensorHeight = 1f;
        public float frontSensorPos = 1.5f;
        public float sideSensorPos = 1;
        public float sideSensorAngle = 45;
        public float overtakingDistance = 5f;
        public float overtakingWidth = 2f;
        public float overtakingDuration = 3f;
        private float overtakingTimer = 0f;
        private bool isOvertaking = false;
        [SerializeField] private float lateralOffset = 0f;

        private float accelInput;
        private float brakeInput;
        private float steerInput;
        private bool handbrakeInput;
        private bool headlightInput;
        private bool rightSignInput;
        private bool leftSignInput;
        private bool hazardInput;
        private bool lookbackInput;
        private float currentSpeed;
        private float targetSpeed;
        private Vector3 currentLocalVelocity;
        private CarController controller;
        private CarLightController lightController;
        private Transform tr;
        private Rigidbody rb;
        private float3 targetPos;
        private float3 targetTangent;
        private float3 targetUp;
        private float3 brakeTangent;
        private Vector3 dirToTarget;
        private float steerDot;
        private float targetSplinePos;
        private float brakeLookAheadPos;
        private float cornerAngle;
        private float normalizedAngle;
        private float rightObstacle = 0;
        private float leftObstacle = 0;
        private Vector3 frontSensor;
        private Vector3 rightSensor;
        private Vector3 leftSensor;
        private Vector3 rightAngle;
        private Vector3 leftAngle;
        private Vector3 halfExtend => Vector3.one * sideSensorPos;
        private RaycastHit hit;
        private RaycastHit rightHit;
        private RaycastHit leftHit;
        private Vector3 point;
        
        float3 resetPos = Vector3.zero;
        float3 resetFwd = Vector3.forward;
        private float stoppedTime;
        private float resetTime = 2f;

        public SplineContainer RacingSpline
        {
            get => racingSpline;
            set
            {
                racingSpline = value;
                splineLength = racingSpline.CalculateLength();
                point = transform.position - racingSpline.transform.position;
                SplineUtility.GetNearestPoint(racingSpline.Spline, point, out _, out splinePosition);
            }
        }

        private void Awake()
        {
            controller = GetComponent<CarController>();
            lightController = GetComponent<CarLightController>();
            tr = transform;
            rb = GetComponent<Rigidbody>();
            input = new CarInputActions();

            accelAction = input.Car.Accel;
            brakeAction = input.Car.Brake;
            steerAction = input.Car.Steer;
            handbrakeAction = input.Car.Handbrake;
            headlightAction = input.Car.Headlight;
            rightSignAction = input.Car.RightSign;
            leftSignAction = input.Car.LeftSign;
            hazardAction = input.Car.Hazard;
            resetAction = input.Car.Reset;
            pauseAction = input.Pause.Pause;

            if (brakeCurve == null || brakeCurve.length == 0)
            {
                brakeCurve = new AnimationCurve(
                    new Keyframe(0, 0),
                    new Keyframe(0.5f, 0.8f),
                    new Keyframe(1, 1)
                );
            }

            if (racingSpline != null)
            {
                splineLength = racingSpline.CalculateLength();
                point = tr.position - racingSpline.transform.position;
                SplineUtility.GetNearestPoint(racingSpline.Spline, point, out _, out splinePosition);
            }
        }

        private void Update()
        {
            currentLocalVelocity = tr.InverseTransformDirection(rb.linearVelocity);
            currentSpeed = currentLocalVelocity.z;

            

            if (racingSpline != null)
            {
                // splinePosition += currentSpeed * Time.deltaTime / splineLength;
                point = tr.position - racingSpline.transform.position;
                SplineUtility.GetNearestPoint(racingSpline.Spline, point, out _, out splinePosition);
            }

            switch (inputType)
            {
                case InputType.User:
                    UserInput();
                    break;
                case InputType.AI:
                    AIInput();
                    CheckForOvertaking();
                    break;
            }

            if (!active)
            {
                accelInput = steerInput = 0;
                brakeInput = currentSpeed > .2f ? 1 : 0;
            }

            controller.SetInput(accelInput, brakeInput, steerInput, handbrakeInput);
        }

        private void FixedUpdate()
        {
            if (racingSpline != null)
                racingSpline.Evaluate(splinePosition, out resetPos, out resetFwd, out _);
            else
            {
                resetPos = tr.position + tr.up;
                resetFwd = tr.forward;
            }

            switch (inputType)
            {
                case InputType.User:
                    if (resetAction.WasPressedThisFrame()) controller.ResetCar(resetPos, resetFwd);
                    break;
                case InputType.AI:
                    if (stoppedTime > resetTime)
                    {
                        controller.ResetCar(resetPos, resetFwd);
                        stoppedTime = 0;
                    }
                    break;
            }
        }

        void OnEnable()
        {
            input.Enable();
            RaceEvents.PauseEvent.AddListener(() => input.Car.Disable());
            RaceEvents.ResumeEvent.AddListener(() => input.Car.Enable());
        }

        void OnDisable()
        {
            input.Disable();
            RaceEvents.PauseEvent.RemoveListener(() => input.Car.Disable());
            RaceEvents.ResumeEvent.RemoveListener(() => input.Car.Enable());
        }

        private void UserInput()
        {
            if (pauseAction.WasPressedThisFrame())
            {
                if (!PauseManager.Instance.isPaused)
                {
                    PauseManager.Instance.Pause();
                    input.Car.Disable();
                }
                else
                {
                    PauseManager.Instance.Resume();
                    input.Car.Enable();
                }
            }
            
            if (!active) return;
            accelInput = autoAccel ? 1 - brakeInput : accelAction.ReadValue<float>();
            brakeInput = brakeAction.ReadValue<float>();
            steerInput = steerAction.ReadValue<float>();
            handbrakeInput = handbrakeAction.IsPressed();

            UserLightControl();
        }

        private void UserLightControl()
        {
            if (headlightAction.WasPressedThisFrame())
            {
                lightController.headlightOn = !headlightInput;
                headlightInput = lightController.headlightOn;
            }

            if (rightSignAction.WasPressedThisFrame())
            {
                lightController.rightBlinkersOn = !rightSignInput;
                if (leftSignInput || hazardInput)
                {
                    lightController.leftBlinkersOn = rightSignInput;
                }
                rightSignInput = lightController.rightBlinkersOn;
                leftSignInput = hazardInput = false;
            }

            if (leftSignAction.WasPressedThisFrame())
            {
                lightController.leftBlinkersOn = !leftSignInput;
                if (rightSignInput || hazardInput)
                {
                    lightController.rightBlinkersOn = leftSignInput;
                }
                leftSignInput = lightController.leftBlinkersOn;
                rightSignInput = hazardInput = false;
            }

            if (hazardAction.WasPressedThisFrame())
            {
                lightController.rightBlinkersOn = lightController.leftBlinkersOn = !hazardInput;
                hazardInput = lightController.rightBlinkersOn && lightController.leftBlinkersOn;
                rightSignInput = leftSignInput = false;
            }
        }

        private void AIInput()
        {
            if (!active || racingSpline == null || controller == null) return;

            stoppedTime = Mathf.Abs(currentLocalVelocity.z) < 1 ? stoppedTime + Time.deltaTime : 0;

            // Evaluate Spline Positions
            targetSplinePos = math.fmod(splinePosition + (lookAheadDistance / splineLength), 1f);
            brakeLookAheadPos = math.fmod(splinePosition + (lookAheadForBraking / splineLength), 1f);

            racingSpline.Evaluate(targetSplinePos, out targetPos, out targetTangent, out targetUp);
            racingSpline.Evaluate(brakeLookAheadPos, out _, out brakeTangent, out _);

            Vector3 rightOffset = Vector3.Cross(targetUp, math.normalize(targetTangent)) * lateralOffset;
            Vector3 finalTargetPos = (Vector3)targetPos + rightOffset;

            cornerAngle = Vector3.Angle(tr.forward, brakeTangent);
            normalizedAngle = Mathf.Clamp01(cornerAngle / brakeAngle);

            dirToTarget = (finalTargetPos - tr.position).normalized;

            steerDot = Vector3.Dot(tr.right, dirToTarget);

            steerInput = Mathf.Abs(Mathf.Pow(steerDot, (tr.position - finalTargetPos).sqrMagnitude > 20 ? 1 : 2)) * Mathf.Sign(steerDot) * steeringResponsiveness * 10;

            targetSpeed = Mathf.Clamp(maxSpeed * (1 - normalizedAngle), minCorneringSpeed, maxSpeed);

            accelInput = Mathf.Clamp01(targetSpeed - currentSpeed);
            brakeInput = (currentSpeed > targetSpeed && currentSpeed > minCorneringSpeed)
                        ? brakeCurve.Evaluate(normalizedAngle) * brakeIntensity : 0f;

            if (racingSpline.Spline.Closed)
                splinePosition = Mathf.Repeat(splinePosition, 1f);
            else if (splinePosition >= 1 || !active)
            {
                accelInput = steerInput = brakeInput = 0;
            }
        }
        
        void CheckForOvertaking()
        {
            frontSensor = tr.position + tr.up * sensorHeight + tr.forward * frontSensorPos;
            rightSensor = frontSensor + tr.right * sideSensorPos;
            leftSensor = frontSensor - tr.right * sideSensorPos;
            rightAngle = Quaternion.AngleAxis(sideSensorAngle, tr.up) * tr.forward;
            leftAngle = Quaternion.AngleAxis(-sideSensorAngle, tr.up) * tr.forward;

            if (Physics.Raycast(rightSensor, rightAngle, out rightHit, overtakingDistance, wallMask))
            {
                rightObstacle = Vector3.Distance(rightHit.point, rightSensor);
            }

            if (Physics.Raycast(leftSensor, leftAngle, out leftHit, overtakingDistance, wallMask))
            {
                leftObstacle = Vector3.Distance(leftHit.point, leftSensor);
            }

            if (isOvertaking)
            {
                overtakingTimer -= Time.deltaTime;
                if (overtakingTimer <= 0f || rightObstacle < .01f || leftObstacle < .01f)
                {
                    isOvertaking = false;
                    lateralOffset = 0f;
                }
                return;
            }

            if (Physics.BoxCast(frontSensor, halfExtend, tr.forward, out hit, Quaternion.identity, overtakingDistance, obstacleMask))
            {
                if ((hit.collider.CompareTag("Player") || hit.collider.CompareTag("AI")) &&
                    hit.collider.TryGetComponent(out Rigidbody colrb) &&
                    currentSpeed >= colrb.linearVelocity.magnitude)
                {
                    float opponentDot = Vector3.Dot(tr.right, hit.point - tr.position);
                    if (opponentDot > 0 && leftObstacle >= rightObstacle)
                    {
                        lateralOffset = -overtakingWidth;
                        StartOvertaking();
                    }
                    else if (opponentDot < 0 && rightObstacle >= leftObstacle)
                    {
                        lateralOffset = overtakingWidth;
                        StartOvertaking();
                    }
                    else
                    {
                        accelInput = 0;
                        brakeInput = 1;
                    }
                }
            }
        }

        void StartOvertaking()
        {
            isOvertaking = true;
            overtakingTimer = overtakingDuration;
        }
    }
}
