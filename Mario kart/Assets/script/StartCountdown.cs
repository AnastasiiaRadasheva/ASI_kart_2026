using UnityEngine;
using TMPro;
using System.Collections;

public class StartCountdown : MonoBehaviour
{
    public GameObject startPanel;      
    public GameObject countdownPanel;  
    public TextMeshProUGUI countdownText;

    public int countdownTime = 3;

    private static bool hasSeenStartScreen = false; 
    private bool hasCountdownPlayed = false;       

    void Start()
    {
        if (hasSeenStartScreen)
        {
            startPanel.SetActive(false);
            StartCoroutine(LevelCountdown());
        }
        else
        {
            startPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void OnPlayButtonPressed()
    {
        if (hasSeenStartScreen) return;

        hasSeenStartScreen = true; 
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
