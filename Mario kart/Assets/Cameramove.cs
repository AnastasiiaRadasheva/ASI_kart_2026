using UnityEngine;

public class Cameramove : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 2f;
    private float rotX = 0f;
    private float rotY = 0f;
    void Update()
    {
        rotX +=Input.GetAxis("Mouse X") * mouseSensitivity;
        rotY -=Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotY =Mathf.Clamp(rotY, -80f, 80f);
        transform.rotation = Quaternion.Euler(rotY, rotX, 0);
        float x= Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float z= Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(new Vector3(x, 0, z));
    }
}
