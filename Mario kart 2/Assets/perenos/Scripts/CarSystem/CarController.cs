using System.Collections;
using System.Linq;
using Sain.Utils;
using UnityEngine;

namespace Sain.TougeRacer
{
    [AddComponentMenu("Arcade Car/Car Controller")]
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {
        // References
        // [Header("References")]
        [SerializeField] private Transform[] rayPoints;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform accelPoint;

        // Suspension Settings
        // [Header("Suspension Settings")]
        [SerializeField] private float springStiffness = 30000f;
        [SerializeField] private float damperStiffness = 3000f;
        [SerializeField] private float restLength = 0.5f;
        [SerializeField] private float springTravel = 0.2f;
        [SerializeField] private float wheelRadius = 0.33f;
        [Range(0, 2)][SerializeField] private float frontWheelPosZ;
        [Range(0, 2)][SerializeField] private float frontWheelPosY;
        [Range(0, 2)][SerializeField] private float frontWheelPosX;
        [Range(0, 2)][SerializeField] private float rearWheelPosZ;
        [Range(0, 2)][SerializeField] private float rearWheelPosY;
        [Range(0, 2)][SerializeField] private float rearWheelPosX;

        // Car Dynamics Settings
        // [Header("Car Settings")]
        [SerializeField] private float acceleration = 25f;
        [SerializeField] private float maxSpeed = 100f;
        [SerializeField] private float reverseSpeed = 15f;
        [SerializeField] private float deceleration = 10f;
        [SerializeField] private float steerStrength = 35f;
        [SerializeField] private AnimationCurve turningCurve;
        [SerializeField] private float dragCoefficient = 2f;
        [SerializeField] private float brakingDeceleration = 100f;
        [SerializeField] private float brakingDragCoefficient = 0.5f;

        // Gear System Settings
        // [Header("Gear Settings")]
        [SerializeField] private int gearNum = 5;
        [SerializeField] private float revRangeBoundary = 1f;
        [SerializeField] private float engineBraking = 0.2f;

        // Visuals
        // [Header("Car Visuals")]
        [SerializeField] private GameObject[] tires = new GameObject[4];
        [SerializeField] private GameObject[] frontTireParent = new GameObject[2];
        [SerializeField] private float maxSteerAngle = 30f;
        [SerializeField] private Transform steeringWheel;
        [SerializeField] private float maxSteerWheelAngle = 180f;
        [SerializeField] private TrailRenderer[] skidmarks = new TrailRenderer[4];
        [SerializeField] private GameObject[] smokes = new GameObject[4];
        [SerializeField] private float minSideSkidVel = 10f;
        private Vector3 steeringWheelEulerAngle;

        // [Header("Ghost Mode Settings")]
        [Layer] [SerializeField] private string normalLayer = "Car";
        [Layer] [SerializeField] private string ghostLayer = "GhostCar";
        [SerializeField] private bool isGhost = false;
        private Coroutine ghostModeCoroutine;

        // Average Speed Settings
        // [Header("Average Speed Settings")]
        [SerializeField] private bool enableAverageSpeed = false;
        private float totalDistanceTraveled = 0f;
        private float totalTime = 0f;
        private float averageSpeed = 0f;
        private Vector3 lastPosition;

        // Private variables
        private Rigidbody rb;
        private Transform tr;
        private Vector3 currentCarLocalVelocity;
        private Vector3[] skidPos = new Vector3[4];
        private int[] wheelsIsGrounded = new int[4];
        private float maxSuspensionDistance;
        private float currentSteerAngle;
        private float accelInput;
        private float brakeInput;
        private float steerInput;
        private bool handbrake;
        private int currentGear;
        private float gearFactor;
        private float carVelocityRatio;
        private float wheelCircumference;
        private bool isGrounded;
        private bool isDrifting;
        private bool isBraking;
        private bool isReverse;
        private float revs;

        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Handling => steerStrength;
        public Transform[] RayPoints { get => rayPoints; set => rayPoints = value; }
        public bool IsGrounded => isGrounded;
        public bool IsDrifting => isDrifting;
        public bool IsBraking => isBraking;
        public bool IsReverse => isReverse;
        public GameObject[] Tires { get => tires; set => tires = value; }
        public float Radius => wheelRadius;
        public float CurrentSpeed => currentCarLocalVelocity.magnitude;
        public float VelocityRatio => carVelocityRatio;
        public int CurrentGear => currentGear;
        public float Revs => revs;
        public bool IsGhost => isGhost;
        public bool EnableAverageSpeed { get => enableAverageSpeed; set => enableAverageSpeed = value; }
        public float AverageSpeed => averageSpeed;

        private void Awake()
        {
            tr = transform;
            rb = GetComponent<Rigidbody>();
            wheelCircumference = 2 * Mathf.PI * wheelRadius;
            maxSuspensionDistance = restLength + springTravel;

            lastPosition = tr.position;

            if (steeringWheel)
            {
                steeringWheelEulerAngle = steeringWheel.localEulerAngles;
            }
        }

        private void Update()
        {
            GroundCheck();
            DriftCheck();
            UpdateVisuals();

            if (enableAverageSpeed) UpdateAverageSpeed();
        }

        private void FixedUpdate()
        {
            ApplySuspension();
            UpdateCarVelocity();
            HandleMovement();
        }

        private void UpdateAverageSpeed()
        {
            float distance = Vector3.Distance(tr.position, lastPosition);
            totalDistanceTraveled += distance;
            totalTime += Time.deltaTime;

            if (totalTime > 0.01f)
                averageSpeed = totalDistanceTraveled / totalTime; // m/s

            lastPosition = tr.position;
        }

        public void ResetAverageSpeed()
        {
            totalDistanceTraveled = 0f;
            totalTime = 0f;
            averageSpeed = 0f;
            lastPosition = tr.position;
        }

        public void SetInput(float accel, float brake, float steer, bool handbrake = false)
        {
            accelInput = Mathf.Clamp(accel, 0, 1);
            brakeInput = Mathf.Clamp(brake, 0, 1);
            steerInput = Mathf.Clamp(steer, -1, 1);
            this.handbrake = handbrake;
        }

        private void HandleMovement()
        {
            isBraking = brakeInput > 0 && currentCarLocalVelocity.z > 0;
            isReverse = currentCarLocalVelocity.z < -0.2f;

            if (!isGrounded) return;
            ApplyAcceleration();
            ApplyDeceleration();
            HandleSteering();
            ApplySidewayDrag();
            CapSpeed();
            UpdateRevs();
            AutoTransmission();
        }

        private void ApplyAcceleration()
        {
            float force = handbrake ? 0 : acceleration * (accelInput - brakeInput);
            rb.AddForceAtPosition(force * tr.forward, accelPoint.position, ForceMode.Acceleration);

            if (currentCarLocalVelocity.magnitude < .2f)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }
            if (accelInput > 0 || brakeInput > 0)
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }

        private void ApplyDeceleration()
        {
            float decelForce = (handbrake ? brakingDeceleration : deceleration) * carVelocityRatio;
            if (accelInput == 0 && currentCarLocalVelocity.z > 1f)
            {
                decelForce += engineBraking;
            }
            rb.AddForceAtPosition(decelForce * -tr.forward, accelPoint.position, ForceMode.Acceleration);
        }

        private void HandleSteering()
        {
            float torque = steerStrength * steerInput * turningCurve.Evaluate(Mathf.Abs(carVelocityRatio)) * Mathf.Sign(carVelocityRatio);
            rb.AddTorque(torque * tr.up, ForceMode.Acceleration);
        }

        private void ApplySidewayDrag()
        {
            float sidewaySpeed = currentCarLocalVelocity.x;
            float drag = -sidewaySpeed * (handbrake && currentCarLocalVelocity.magnitude < 0.2f ? brakingDragCoefficient : dragCoefficient);
            rb.AddForceAtPosition(tr.right * drag, rb.worldCenterOfMass, ForceMode.Acceleration);
        }

        private void CapSpeed()
        {
            float speed = currentCarLocalVelocity.z;
            float max = 1f / gearNum * (currentGear + 1) * maxSpeed;
            if (speed > max)
            {
                rb.linearVelocity = max * rb.linearVelocity.normalized;
            }
            else if (speed < -reverseSpeed)
            {
                rb.linearVelocity = reverseSpeed * rb.linearVelocity.normalized;
            }
        }

        private void UpdateCarVelocity()
        {
            currentCarLocalVelocity = tr.InverseTransformDirection(rb.linearVelocity);
            carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
        }

        private void UpdateRevs()
        {
            CalculateGearFactor();
            float gearNumFactor = currentGear / (float)gearNum;
            float revsRangeMin = ULerp(0f, revRangeBoundary, CurveFactor(gearNumFactor));
            float revsRangeMax = ULerp(revRangeBoundary, 1f, gearNumFactor);
            revs = ULerp(revsRangeMin, revsRangeMax, gearFactor); // Fixed Revs assignment
        }

        private void AutoTransmission()
        {
            float upGearLimit = 1f / gearNum * (currentGear + 1);
            float downGearLimit = 1f / gearNum * currentGear;

            if (currentGear > 0 && Mathf.Abs(carVelocityRatio) < downGearLimit)
            {
                currentGear--;
            }
            else if (Mathf.Abs(carVelocityRatio) > upGearLimit && currentGear < gearNum - 1)
            {
                currentGear++;
            }
        }

        private void CalculateGearFactor()
        {
            float gearRange = 1f / gearNum;
            float targetGearFactor = Mathf.InverseLerp(gearRange * currentGear, gearRange * (currentGear + 1), Mathf.Abs(rb.linearVelocity.magnitude / maxSpeed));
            gearFactor = Mathf.Lerp(gearFactor, targetGearFactor, Time.deltaTime * 5f);
        }

        private void GroundCheck()
        {
            isGrounded = wheelsIsGrounded.Sum() > 1; // Fixed Sum() usage
        }

        private void DriftCheck()
        {
            isDrifting = isGrounded && (Mathf.Abs(currentCarLocalVelocity.x) > minSideSkidVel || handbrake) && Mathf.Abs(currentCarLocalVelocity.z) > 2;
        }

        private void ApplySuspension()
        {
            for (int i = 0; i < rayPoints.Length; i++)
            {
                if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out RaycastHit hit, maxSuspensionDistance + wheelRadius, groundLayer))
                {
                    wheelsIsGrounded[i] = 1;
                    float springForce = springStiffness * (restLength - (hit.distance - wheelRadius)) / springTravel;
                    float dampingForce = damperStiffness * Vector3.Dot(rb.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                    rb.AddForceAtPosition((springForce - dampingForce) * rayPoints[i].up, rayPoints[i].position);


                    if (tires[i])
                    {
                        tires[i].transform.position = hit.point + rayPoints[i].up * wheelRadius;
                    }

                    skidPos[i] = hit.point + Vector3.up * 0.01f;
                }
                else
                {
                    wheelsIsGrounded[i] = 0;
                    if (tires[i])
                    {
                        tires[i].transform.position = rayPoints[i].position - rayPoints[i].up * (restLength + springTravel);
                    }
                }
            }
        }

        private void UpdateVisuals()
        {
            UpdateTireVisuals();
            UpdateVFX();
        }

        private void UpdateTireVisuals()
        {
            float steeringAngle = maxSteerAngle * steerInput;
            if (isDrifting)
            {
                steeringAngle = Mathf.Clamp(Vector3.SignedAngle(tr.forward, rb.linearVelocity.normalized, tr.up), -maxSteerAngle, maxSteerAngle);
            }
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, steeringAngle, 4 * Time.deltaTime);

            for (int i = 0; i < tires.Length; i++)
            {
                if (i < 2 && frontTireParent[i])
                {
                    frontTireParent[i].transform.localEulerAngles = new Vector3(frontTireParent[i].transform.localEulerAngles.x, currentSteerAngle, frontTireParent[i].transform.localEulerAngles.z);
                }
                tires[i].transform.Rotate(Vector3.right, currentCarLocalVelocity.z / wheelCircumference, Space.Self);
            }

            float steeringWheelAngle = maxSteerWheelAngle * (currentSteerAngle / maxSteerAngle);

            if (steeringWheel)
            {
                steeringWheel.localEulerAngles = new Vector3(steeringWheelEulerAngle.x, steeringWheelEulerAngle.y, steeringWheelEulerAngle.z + steeringWheelAngle);
            }
        }

        private void UpdateVFX()
        {
            ToggleSkidmarks(isDrifting);
            for (int i = 0; i < smokes.Length; i++)
            {
                if (smokes[i] == null) continue;
                smokes[i].SetActive(isDrifting && wheelsIsGrounded[i] == 1);
                smokes[i].transform.position = new Vector3(tires[i].transform.position.x, skidPos[i].y + .1f, tires[i].transform.position.z);
                smokes[i].transform.localRotation = Quaternion.Euler(0, Vector3.SignedAngle(tr.forward, rb.linearVelocity.normalized, Vector3.up), 0);
            }
        }

        private void ToggleSkidmarks(bool toggle)
        {
            for (int i = 0; i < skidmarks.Length; i++)
            {
                if (skidmarks[i] == null) continue;
                skidmarks[i].emitting = toggle && wheelsIsGrounded[i] == 1;
                skidmarks[i].transform.position = new Vector3(tires[i].transform.position.x, skidPos[i].y + .1f, tires[i].transform.position.z);
            }
        }

        public void ResetCar(Vector3 position, Vector3 forward)
        {
            tr.position = position;
            tr.forward = forward;
            rb.linearVelocity = Vector3.zero * .5f;

            if (ghostModeCoroutine != null)
            {
                StopCoroutine(ghostModeCoroutine);
                SetGhostMode(false);
            }

            ghostModeCoroutine = StartCoroutine(GhostModeCoroutine(3));
        }

        IEnumerator GhostModeCoroutine(float time)
        {
            SetGhostMode(true);
            yield return new WaitForSeconds(time);
            SetGhostMode(false);
        }

        public void SetGhostMode(bool enable)
        {
            if (isGhost == enable) return;
            isGhost = enable;
            UpdateGhostLayer(enable);

            SetGhostVisual(enable);
        }

        private void UpdateGhostLayer(bool ghost)
        {
            int targetLayer = LayerMask.NameToLayer(ghost ? ghostLayer : normalLayer);
            gameObject.layer = targetLayer;

            foreach (Transform child in transform)
                child.gameObject.layer = targetLayer;

        }

        private void SetGhostVisual(bool ghost)
        {
            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach (var r in rends)
            {
                foreach (var mat in r.materials)
                {
                    if (!mat.HasFloat("_DitherValue")) continue;
                    if (ghost)
                    {
                        mat.SetFloat("_DitherValue", .5f);
                    }
                    else
                    {
                        mat.SetFloat("_DitherValue", 1);
                    }
                }
            }
        }

        public static float CurveFactor(float factor) => 1 - (1 - factor) * (1 - factor);

        public static float ULerp(float from, float to, float value) => (1f - value) * from + value * to;
    }
}
