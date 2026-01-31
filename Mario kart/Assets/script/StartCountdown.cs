using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartCountdown : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;

    public int countdownTime = 3;

    private static string lastSceneWhereStartWasSeen = "";
    private bool hasCountdownPlayed = false;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        bool shouldShowStart = (lastSceneWhereStartWasSeen != sceneName);

        if (shouldShowStart)
        {
            startPanel.SetActive(true);
            countdownPanel.SetActive(false);
            Time.timeScale = 0f;
        }
        else
        {
            startPanel.SetActive(false);
            StartCoroutine(LevelCountdown());
        }
    }

    public void OnPlayButtonPressed()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        lastSceneWhereStartWasSeen = sceneName;

        startPanel.SetActive(false);
        StartCoroutine(LevelCountdown());
    }

    IEnumerator LevelCountdown()
    {
        if (hasCountdownPlayed) yield break;

        hasCountdownPlayed = true;

        countdownPanel.SetActive(true);
        Time.timeScale = 0f;

        int remainingTime = countdownTime;

        while (remainingTime > 0)
        {
            countdownText.text = remainingTime.ToString();
            yield return new WaitForSecondsRealtime(1f);
            remainingTime--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);

        countdownPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
