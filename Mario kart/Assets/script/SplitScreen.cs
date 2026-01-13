using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Camera cam1;
    public Camera cam2;

    void Update()
    {
            ChangeScreen();

    }

    public void ChangeScreen()
    {
        
            cam1.rect = new Rect(0, 0, 0.5f, 1);
            cam2.rect = new Rect(0.5f, 0, 0.5f, 1);
    }
}
