using UnityEngine;
using TMPro;
using System.Collections;
public class StartCountdown : MonoBehaviour
{
    [Header("UI References")]
    public GameObject countdownPanel;   
    public TextMeshProUGUI countdownText; 

    [Header("Settings")]
    public int countdownTime = 3;

    void Start()
    {
        Time.timeScale = 0f;
        
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        countdownPanel.SetActive(true);

        int remainingTime = countdownTime;
        while (remainingTime > 0)
        {
            countdownText.text = remainingTime.ToString();
            
            yield return new WaitForSecondsRealtime(1f);
            
            remainingTime--;
        }
        countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
    
        countdownPanel.SetActive(false);
        
        RaceTime raceTimer = FindObjectOfType<RaceTime>();
        if (raceTimer != null)
        {
            raceTimer.StartTimer();
        }
    }
}
