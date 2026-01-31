using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

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
    private static bool endSequenceStarted = false;

    private RaceTime timerScript;

    private int lastRankCheckpointID = -1;
    private Dictionary<CheckpointID, float> lastHitTime = new Dictionary<CheckpointID, float>();
    private float minTimeBetweenHits = 0.05f;

    public float returnToMenuDelay = 5f;
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        timerScript = FindObjectOfType<RaceTime>();
        if (winPanel != null) winPanel.SetActive(false);
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

        if (other.CompareTag("RankingCheckpoint"))
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
        if (isFinished) return;

        isFinished = true;

        if (timerScript != null)
            personalFinishTime = timerScript.timerText.text;

        if (!finishTimes.ContainsKey(gameObject))
            finishTimes.Add(gameObject, personalFinishTime);

        if (gameMode == 1)
        {
            EndRaceForEveryone();
            return;
        }

        var players = FindObjectsOfType<LapCounter>().Where(p => p.gameMode == 2).ToArray();

        if (players.Length < 2) return;

        bool allFinished = true;
        foreach (var p in players)
        {
            if (!p.isFinished)
            {
                allFinished = false;
                break;
            }
        }

        if (allFinished)
        {
            EndRaceForEveryone();
        }
    }

    void EndRaceForEveryone()
    {
        if (timerScript != null)
            timerScript.StopTimer();

        LapCounter[] targets;

        if (gameMode == 2)
            targets = FindObjectsOfType<LapCounter>().Where(p => p.gameMode == 2).ToArray();
        else
            targets = FindObjectsOfType<LapCounter>().Where(p => p.gameMode == 1).ToArray();

        foreach (var p in targets)
            p.ShowResult();

        if (!endSequenceStarted)
        {
            endSequenceStarted = true;
            StartCoroutine(ReturnToMenuAfterDelay());
        }
    }

    System.Collections.IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSeconds(returnToMenuDelay);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void ShowResult()
    {
        int myRank = GetMyRank();

        if (winPanel != null)
        {
            winPanel.SetActive(true);

            string myTime = finishTimes.ContainsKey(gameObject)
                ? finishTimes[gameObject]
                : personalFinishTime;

            if (resultText != null)
            {
                resultText.text =
                    "RACE OVER\n" +
                    "Rank: " + myRank + "\n" +
                    "Your Time: " + myTime;
            }
        }
    }

    int GetMyRank()
    {
        var cars = FindObjectsOfType<CarProgress>()
            .OrderByDescending(c => c.GetProgress())
            .ToList();

        for (int i = 0; i < cars.Count; i++)
        {
            if (cars[i].gameObject == this.gameObject)
                return i + 1;
        }

        return cars.Count;
    }

    void ResetCheckpoints()
    {
        for (int i = 0; i < checkpointsPassed.Length; i++)
            checkpointsPassed[i] = false;

        checkpointsCount = 0;
    }

    void UpdateText()
    {
        if (lapText != null)
            lapText.text = "Lap: " + currentLaps + " / " + lapsToWin;
    }
}
