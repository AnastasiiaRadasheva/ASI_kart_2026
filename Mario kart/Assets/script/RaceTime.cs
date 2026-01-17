using UnityEngine;
using TMPro;

public class RaceTime : MonoBehaviour
{
   public TextMeshProUGUI timerText; 
    private float elapsedTime = 0f;
    private bool isRunning = true; 
    
public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }
    
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerUI();
        isRunning = true;
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime; 
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);
        timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public void StopTimer()
    {
        isRunning = false; 
    }
}
