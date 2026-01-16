
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainSC : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayMenu(){
        SceneManager.LoadScene("scenes/1player/cart1");


    }
    public void Play1es(){
        SceneManager.LoadScene("scenes/1player/cart1");


    }
    public void Play1tei(){
        SceneManager.LoadScene("scenes/1player/cart2");


    }
    public void Play1kolm(){
        SceneManager.LoadScene("scenes/1player/cart3");


    }
    public void Play2es(){
        SceneManager.LoadScene("scenes/2players/cart1");


    }
    public void Play2tei(){
        SceneManager.LoadScene("scenes/2players/cart2");


    }
    public void Play2kolm(){
        SceneManager.LoadScene("scenes/2players/cart3");


    }
    public void ExitGame()
    {
        Debug.Log("Game is over");
        Application.Quit();
    }
}
