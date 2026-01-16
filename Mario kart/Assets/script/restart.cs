using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class restart : MonoBehaviour
{
    
    public string sceneToRestart;

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(sceneToRestart);
        }
    }

}
