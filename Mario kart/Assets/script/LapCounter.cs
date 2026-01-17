using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LapCounter : MonoBehaviour
{
    [Header("Race Settings")]
    public int lapsToWin = 3;
    public int gameMode = 1; // 1 = Single, 2 = Duo
    public int currentLaps = 0;

    [Header("UI References")]
    public TextMeshProUGUI lapText;
    public GameObject winPanel;
    public TextMeshProUGUI resultText;

    private bool[] checkpointsPassed = new bool[3];
    private int checkpointsCount = 0;
    private bool isFinished = false;
        private string personalFinishTime; 

    private RaceTime timerScript;
    private static int playersFinished = 0;

    void Start()
    {
        timerScript = FindObjectOfType<RaceTime>();
        if (winPanel != null) winPanel.SetActive(false);
        playersFinished = 0; 
        UpdateText();
    }

    private float lastCheckpointTime = 0f;
private float cooldownDuration = 1.0f; 

private void OnTriggerEnter(Collider other)
{
    if (isFinished) return;

    if (Time.time - lastCheckpointTime < cooldownDuration) return;

    if (other.CompareTag("Checkpoint"))
    {
        CheckpointID cp = other.GetComponent<CheckpointID>();
        if (cp == null) return;

        if (!checkpointsPassed[cp.id])
        {
            checkpointsPassed[cp.id] = true;
            checkpointsCount++;
            
            lastCheckpointTime = Time.time; 

            if (checkpointsCount >= 3)
            {
                currentLaps++;
                ResetCheckpoints();
                UpdateText();

                if (currentLaps >= lapsToWin)
                {
                    FinishRace();
                }
            }
        }
    }
}
    void FinishRace()
    {
        isFinished = true;
        
        if (timerScript != null)
        {
            personalFinishTime = timerScript.timerText.text;
        }
        playersFinished++;

        if (gameMode == 1)
        {
            timerScript.StopTimer();
            ShowResultIfQualified();
        }
        else if (gameMode == 2 && playersFinished >= 2)
        {
            timerScript.StopTimer();
            
            LapCounter[] players = FindObjectsOfType<LapCounter>();
            foreach(var p in players) p.ShowResultIfQualified();
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
            
            string status = isWinner ? "VICTORY!" : "RACE OVER";
            resultText.text = status + "\n" +
                              "Rank: " + myRank + "\n" +
                              "Your Time: " + personalFinishTime;
        }
    }

    int GetMyRank()
    {
        var cars = FindObjectsOfType<CarProgress>()
            .OrderByDescending(c => c.GetProgress())
            .ToList();

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
        if (lapText != null)
            lapText.text = "Lap: " + currentLaps + " / " + lapsToWin;
    }
}