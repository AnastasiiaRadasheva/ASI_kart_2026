
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainSC : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayMenu(){
        SceneManager.LoadScene("cart1");


    }
    public void ExitGame()
    {
        Debug.Log("Game is over");
        Application.Quit();
    }
}
