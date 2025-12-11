using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Camera cam1;
    public Camera cam2;

    public bool Horizontal = false;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ChangeScreen();
        }
    }

    public void ChangeScreen()
    {
        Horizontal = !Horizontal;

        if(Horizontal)
        {
            cam1.rect = new Rect(0, 0, 1, 0.5f);
            cam2.rect = new Rect(0, 0.5f, 1, 0.5f);

        }else{
            cam1.rect = new Rect(0, 0, 0.5f, 1);
            cam2.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
    }
}
