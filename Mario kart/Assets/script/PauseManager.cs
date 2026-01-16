using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour
{
    public int sceneIndex; 
    

    public GameObject pausePanel;

    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }
    public void RestartLevel()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene(sceneIndex);
}



public void GoToMainMenu()
{
    Time.timeScale = 1f; SceneManager.LoadScene("menyTEST");
}

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }
}
