using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject panelSettings;
    
    public void Play()
    {
        SceneManager.LoadScene("mainscane");
    }
    public void Exit()
    {
        Application.Quit();

    }
}
