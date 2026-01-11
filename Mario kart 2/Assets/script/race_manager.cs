using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    [Header("Checkpoints")]
    public Checkpoint[] checkpoints;

    [Header("Car")]
    public Car1 car;

    [Header("UI")]
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI lapTimeText;
    public TextMeshProUGUI bestLapText;

    [Header("Race Settings")]
    public int maxLaps = 3;

    private CarRaceData carData;

    private void Start()
    {
        carData = new CarRaceData();
        UpdateUI();
    }

    private void Update()
    {
        if (!carData.finished)
            carData.currentLapTime += Time.deltaTime;

        UpdateUI();
    }

    // вызывается при проезде чекпоинта
    private void HandleCheckpoint(int checkpointIndex)
    {
        if (carData.finished)
            return;

        // ❗ строгий порядок чекпоинтов
        if (checkpointIndex != carData.currentCheckpoint)
            return;

        carData.currentCheckpoint++;

        // если прошли все чекпоинты — круг завершён
        if (carData.currentCheckpoint >= checkpoints.Length)
        {
            CompleteLap();
        }
    }

    public void OnCarPassedCheckpoint(Rigidbody carRb, int checkpointIndex)
    {
        if (carRb.GetComponent<CarController1>())
        {
            HandleCheckpoint(checkpointIndex);
        }
    }

    private void CompleteLap()
    {
        // проверяем лучший круг
        if (carData.bestLapTime <= 0f || carData.currentLapTime < carData.bestLapTime)
        {
            carData.bestLapTime = carData.currentLapTime;
        }

        carData.lap++;
        carData.currentCheckpoint = 0;
        carData.currentLapTime = 0f;

        if (carData.lap > maxLaps)
        {
            FinishRace();
        }
    }

    private void FinishRace()
    {
        carData.finished = true;

        if (car != null)
            car.enabled = false; // 🚗 стоп машина
    }

    private void UpdateUI()
    {
        lapText.text = carData.finished
            ? "FINISH!"
            : $"Lap: {carData.lap}/{maxLaps}";

        lapTimeText.text = $"Lap Time: {FormatTime(carData.currentLapTime)}";

        bestLapText.text = carData.bestLapTime > 0
            ? $"Best Lap: {FormatTime(carData.bestLapTime)}"
            : "Best Lap: --:--.---";
    }

    private string FormatTime(float time)
    {
        int min = Mathf.FloorToInt(time / 60f);
        float sec = time % 60f;
        return $"{min:00}:{sec:00.000}";
    }
}

[System.Serializable]
public class CarRaceData
{
    public int lap = 1;
    public int currentCheckpoint = 0;

    public float currentLapTime = 0f;
    public float bestLapTime = 0f;

    public bool finished = false;
}
