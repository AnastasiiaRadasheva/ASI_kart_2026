using System;
using System.Collections;
using Sain.Utils;
using UnityEngine;
using UnityEngine.Splines;

namespace Sain.TougeRacer
{
    public class RaceManager : MonoBehaviour
    {
        public enum RaceState
        {
            Preview,
            Countdown,
            Race,
            Finish,
            End
        }

        public static RaceManager Instance { get; private set; }

        [SerializeField] private int countdown = 3;
        [SerializeField] private SplineContainer racingLine;
        [SerializeField] private Transform[] startingPoints;
        [SerializeField] private FinishLine FinishLine;
        [Header("Minimap")]
        [SerializeField] private MinimapManager_FollowTarget minimapManager;
        [SerializeField] private MinimapWorldElement playerIcon;
        [SerializeField] private MinimapWorldElement aiIcon;

        private GameManager gameManager;
        private CarController[] cars = new CarController[0];
        private CarController focusCar;
        private int focusCarIndex;
        private AIData[] carAI;
        private RaceState raceState;
        private Coroutine countdownCoroutine;
        private float[] raceProgress;
        private float countdownTimer;
        private float raceTimer;
        private float splineLength;
        private WaitForSeconds oneSeconds = new WaitForSeconds(1);
        private int lap;
        private int[] currentLap;

        public CarController FocusCar => focusCar;
        public string CountdownTimer { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            gameManager = GameManager.Instance;

            RaceEvents.SpawnRacersEvent.AddListener(OnRacersSpawn);

            splineLength = racingLine.CalculateLength();

            InitRace();
        }

        void Update()
        {
            switch (raceState)
            {
                case RaceState.Preview:
                    raceState = RaceState.Countdown;
                    break;
                case RaceState.Countdown:
                    if (countdownCoroutine == null)
                        countdownCoroutine = StartCoroutine(CountdownToStart());
                    break;
                case RaceState.Race:
                    raceTimer += Time.deltaTime;
                    RaceEvents.RaceTimerEvent.Invoke(raceTimer);
                    break;
                case RaceState.Finish:
                    break;
            }

            RaceEvents.UpdatePlayerPositionEvent.Invoke(GetCarPosition(focusCarIndex));

            UpdateRaceProgress();
        }

        void OnEnable()
        {
            RaceEvents.SpawnRacersEvent.AddListener(OnRacersSpawn);
            RaceEvents.ThroughFinishLineEvent.AddListener(OnThroughFinishLine);
        }

        void OnDisable()
        {
            RaceEvents.SpawnRacersEvent.RemoveListener(OnRacersSpawn);
            RaceEvents.ThroughFinishLineEvent.RemoveListener(OnThroughFinishLine);
        }

        private void UpdateRaceProgress()
        {
            var startPoint = startingPoints[0].position - racingLine.transform.position;
            var finishPoint = FinishLine.transform.position - racingLine.transform.position;
            SplineUtility.GetNearestPoint(racingLine.Spline, startPoint, out _, out float startPos);
            SplineUtility.GetNearestPoint(racingLine.Spline, finishPoint, out _, out float finishPos);
            var raceLength = finishPos - startPos;

            for (int i = 0; i < raceProgress.Length; i++)
            {
                var point = cars[i].transform.position - racingLine.transform.position;
                SplineUtility.GetNearestPoint(racingLine.Spline, point, out _, out float pos);
                raceProgress[i] = pos;
            }

            if (racingLine.Spline.Closed)
            {
                RaceEvents.UpdateRaceProgressEvent.Invoke($"{currentLap[focusCarIndex]}/{lap}");
            }
            else
            {
                var progress = (raceProgress[focusCarIndex] - startPos) / (finishPos - startPos);
                RaceEvents.UpdateRaceProgressEvent.Invoke((Mathf.Clamp01(progress) * 100).ToString("0") + "%");
            }
        }

        public int ComparePosition(CarController x, CarController y)
        {
            Vector3 pointX = x.transform.position - racingLine.transform.position;
            SplineUtility.GetNearestPoint(racingLine.Spline, pointX, out _, out float posX);

            Vector3 pointY = y.transform.position - racingLine.transform.position;
            SplineUtility.GetNearestPoint(racingLine.Spline, pointY, out _, out float posY);

            return posY.CompareTo(posX);
        }

        public float CarDistance(Transform x, Transform y)
        {
            Vector3 pointX = x.position - racingLine.transform.position;
            SplineUtility.GetNearestPoint(racingLine.Spline, pointX, out _, out float posX);

            Vector3 pointY = y.position - racingLine.transform.position;
            SplineUtility.GetNearestPoint(racingLine.Spline, pointY, out _, out float posY);

            return (posX - posY) * splineLength;
        }

        public int GetCarPosition(int car)
        {
            var maxPos = 1;

            for (int i = 0; i < raceProgress.Length; i++)
            {
                if (raceProgress[car] < raceProgress[i])
                    maxPos += 1;
            }

            return maxPos;
        }

        public void InitRace()
        {
            if (cars.Length > 0)
            {
                foreach (var car in cars)
                {
                    Destroy(car.gameObject);
                }
                Array.Clear(cars, 0, cars.Length);
            }

            RaceEvents.SpawnRacersEvent.Invoke();

            raceState = RaceState.Preview;

            lap = gameManager.stageData.lap;

            countdownTimer = countdown;
            cars = FindObjectsByType<CarController>(FindObjectsSortMode.None);

            raceProgress = new float[cars.Length];
            currentLap = new int[cars.Length];

            foreach (var car in cars)
            {
                if (car.TryGetComponent(out CarInput input))
                {
                    input.RacingSpline = racingLine;
                    input.active = false;
                }

                if (car.CompareTag("Player")) Instantiate(playerIcon, car.transform);
                if (car.CompareTag("AI")) Instantiate(aiIcon, car.transform);
            }

            if (focusCar == null) focusCar = cars[0];
            minimapManager.SetTargetToFollow(focusCar.transform);

            focusCarIndex = Array.IndexOf(cars, focusCar);
        }

        void OnRacersSpawn()
        {
            carAI = gameManager.AICar;

            for (int i = 0; i < gameManager.maxCar - 1; i++)
            {
                CarController aicar = Instantiate(carAI[i].carPrefab, startingPoints[i].position, startingPoints[i].rotation);
                aicar.tag = "AI";
                if (aicar.TryGetComponent(out CarInput input)) input.inputType = CarInput.InputType.AI;
                if (aicar.TryGetComponent(out CarVisuals visuals)) visuals.SetMaterial(carAI[i].bodyPaint);
            }

            focusCar = Instantiate(gameManager.playerCar, startingPoints[gameManager.maxCar - 1].position, startingPoints[gameManager.maxCar - 1].rotation);
        }

        void OnThroughFinishLine(FinishLine finishLine, CarController controller)
        {
            int carIndex = Array.IndexOf(cars, controller);
            if (gameManager.stageData.raceType == RaceType.Circuit && currentLap[carIndex] < lap)
            {
                currentLap[carIndex] ++;
                return;
            }

            if (ReferenceEquals(controller, focusCar))
            {
                RaceEvents.RaceFinishEvent.Invoke();
                RaceEvents.RaceResultEvent.Invoke(GetCarPosition(focusCarIndex), raceTimer);
            }

            if (controller.TryGetComponent(out CarInput input))
            {
                input.active = false;
            }
        }

        IEnumerator CountdownToStart()
        {
            while (countdownTimer > 0)
            {
                CountdownTimer = countdownTimer.ToString();
                RaceEvents.CountdownEvent.Invoke((int)countdownTimer);
                yield return oneSeconds;
                countdownTimer--;
            }

            CountdownTimer = "GO!";
            RaceEvents.CountdownEvent.Invoke(0);

            raceState = RaceState.Race;
            foreach (var car in cars)
            {
                car.EnableAverageSpeed = true;
                if (car.TryGetComponent(out CarInput input))
                {
                    input.active = true;
                }
            }

            RaceEvents.RaceStartEvent.Invoke();

            yield return oneSeconds;
            
            CountdownTimer = string.Empty;
            RaceEvents.CountdownEvent.Invoke(-1);
        }
    }
}
