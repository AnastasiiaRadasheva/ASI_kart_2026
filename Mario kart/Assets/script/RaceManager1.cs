using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    [Header("Checkpoints")]
    public Checkpoint[] checkpoints;

    [Header("Cars")]
    public CarController1 car1;
    public CarController2 car2;

    [Header("UI")]
    public TextMeshProUGUI car1LapText;
    public TextMeshProUGUI car1TimeText;
    public TextMeshProUGUI car2LapText;
    public TextMeshProUGUI car2TimeText;

    [Header("Race Settings")]
    public int maxLaps = 3;

    private CarRaceData car1Data;
    private CarRaceData car2Data;

    private void Start()
    {
        car1Data = new CarRaceData();
        car2Data = new CarRaceData();

        UpdateUI();
    }

    private void Update()
    {
        if (!car1Data.finished)
            car1Data.timer += Time.deltaTime;

        if (!car2Data.finished)
            car2Data.timer += Time.deltaTime;

        UpdateUI();
    }

private void HandleCheckpoint(CarRaceData data, MonoBehaviour car, int checkpointIndex)
{
    if (data.finished) return;

    // ❗ проверяем порядок
    if (checkpointIndex != data.currentCheckpoint)
        return;

    data.currentCheckpoint++;

    if (data.currentCheckpoint >= checkpoints.Length)
    {
        data.currentCheckpoint = 0;
        data.lap++;
        data.timer = 0f;

        if (data.lap > maxLaps)
        {
            FinishCar(data, car);
        }
    }
}

public void OnCarPassedCheckpoint(Rigidbody carRb, int checkpointIndex)
{
    if (carRb.GetComponent<CarController1>())
    {
        HandleCheckpoint(car1Data, car1, checkpointIndex);
    }
    else if (carRb.GetComponent<CarController2>())
    {
        HandleCheckpoint(car2Data, car2, checkpointIndex);
    }
}


    private void FinishCar(CarRaceData data, MonoBehaviour car)
    {
        data.finished = true;
        car.enabled = false; // 🚗 машина останавливается
    }

    private void UpdateUI()
    {
        car1LapText.text = car1Data.finished
            ? "FINISH!"
            : $"Car 1 Lap: {car1Data.lap}/{maxLaps}";

        car2LapText.text = car2Data.finished
            ? "FINISH!"
            : $"Car 2 Lap: {car2Data.lap}/{maxLaps}";

        car1TimeText.text = FormatTime(car1Data.timer);
        car2TimeText.text = FormatTime(car2Data.timer);
    }

    private string FormatTime(float time)
    {
        int min = Mathf.FloorToInt(time / 60);
        float sec = time % 60f;
        return $"{min:00}:{sec:00.000}";
    }
}

[System.Serializable]
public class CarRaceData
{
    public int lap = 1;
    public int currentCheckpoint = 0;
    public int expectedCheckpoint = 0;
    public float timer = 0f;
    public bool finished = false;
}
