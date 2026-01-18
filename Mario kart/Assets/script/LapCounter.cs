using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LapCounter : MonoBehaviour
{
    public int lapsToWin = 3;
    public int gameMode = 1;
    public int currentLaps = 0;

    public TextMeshProUGUI lapText;
    public GameObject winPanel;
    public TextMeshProUGUI resultText;

    [SerializeField] private int numberOfLapCheckpoints = 3;
    private bool[] checkpointsPassed;
    private int checkpointsCount = 0;

    private bool isFinished = false;
    private string personalFinishTime;
    private static Dictionary<GameObject, string> finishTimes = new Dictionary<GameObject, string>();

    private RaceTime timerScript;
    private static int playersFinished = 0;

    private int lastRankCheckpointID = -1;
    private Dictionary<CheckpointID, float> lastHitTime = new Dictionary<CheckpointID, float>();
    private float minTimeBetweenHits = 0.05f;

    void Start()
    {
        timerScript = FindObjectOfType<RaceTime>();
        if (winPanel != null) winPanel.SetActive(false);
        playersFinished = 0;
        checkpointsPassed = new bool[numberOfLapCheckpoints];
        UpdateText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFinished) return;

        if (other.CompareTag("Checkpoint"))
        {
            CheckpointID cp = other.GetComponent<CheckpointID>();
            if (cp == null) return;
            if (cp.id < 0 || cp.id >= checkpointsPassed.Length) return;

            if (lastHitTime.ContainsKey(cp) && Time.time - lastHitTime[cp] < minTimeBetweenHits) return;
            lastHitTime[cp] = Time.time;

            if (!checkpointsPassed[cp.id])
            {
                checkpointsPassed[cp.id] = true;
                checkpointsCount++;

                if (checkpointsCount >= checkpointsPassed.Length)
                {
                    currentLaps++;
                    ResetCheckpoints();
                    UpdateText();

                    if (currentLaps >= lapsToWin)
                        FinishRace();
                }
            }
        }

        if (other.CompareTag("RankCheckpoint"))
        {
            CheckpointID cp = other.GetComponent<CheckpointID>();
            if (cp == null) return;
            if (lastHitTime.ContainsKey(cp) && Time.time - lastHitTime[cp] < minTimeBetweenHits) return;
            lastHitTime[cp] = Time.time;

            if (cp.id == lastRankCheckpointID) return;
            lastRankCheckpointID = cp.id;
        }
    }

    void FinishRace()
    {
        isFinished = true;
        if (timerScript != null) personalFinishTime = timerScript.timerText.text;
        if (!finishTimes.ContainsKey(gameObject)) finishTimes.Add(gameObject, personalFinishTime);
        playersFinished++;

        if (gameMode == 1)
        {
            timerScript.StopTimer();
            ShowResultIfQualified();
        }
        else if (gameMode == 2)
        {
            if (playersFinished >= 2)
            {
                timerScript.StopTimer();
                LapCounter[] players = FindObjectsOfType<LapCounter>();
                foreach (var p in players) p.ShowResultIfQualified();
            }
        }
    }

    void ShowResultIfQualified()
    {
        int myRank = GetMyRank();
        bool isWinner = false;
        if (gameMode == 1 && myRank == 1) isWinner = true;
        if (gameMode == 2 && myRank <= 3) isWinner = true;

        if (winPanel != null)
        {
            winPanel.SetActive(true);
            string myTime = finishTimes.ContainsKey(gameObject) ? finishTimes[gameObject] : personalFinishTime;
            string status = isWinner ? "VICTORY!" : "RACE OVER";
            resultText.text = status + "\n" + "Rank: " + myRank + "\n" + "Your Time: " + myTime;
        }
    }

    int GetMyRank()
    {
        var cars = FindObjectsOfType<CarProgress>().OrderByDescending(c => c.GetProgress()).ToList();
        for (int i = 0; i < cars.Count; i++)
        {
            if (cars[i].gameObject == this.gameObject) return i + 1;
        }
        return cars.Count;
    }

    void ResetCheckpoints()
    {
        for (int i = 0; i < checkpointsPassed.Length; i++) checkpointsPassed[i] = false;
        checkpointsCount = 0;
    }

    void UpdateText()
    {
        if (lapText != null) lapText.text = "Lap: " + currentLaps + " / " + lapsToWin;
    }
}
