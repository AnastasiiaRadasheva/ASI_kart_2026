using UnityEngine;

public class MinimapPlayerIconAuto : MonoBehaviour
{
    [Header("Игрок и иконка")]
    public Transform player;        
    public RectTransform icon;       

    [Header("Миникарта")]
    public RectTransform mapRect;   
    public Camera minimapCamera;    

    private Vector3 bottomLeft;      
    private Vector3 topRight;     

    void LateUpdate()
    {
        if (!player || !icon || !mapRect || !minimapCamera) return;
        Vector3 bl = minimapCamera.ViewportToWorldPoint(new Vector3(0, 0, minimapCamera.transform.position.y));
        Vector3 tr = minimapCamera.ViewportToWorldPoint(new Vector3(1, 1, minimapCamera.transform.position.y));

        bottomLeft = new Vector3(bl.x, 0, bl.z);
        topRight = new Vector3(tr.x, 0, tr.z);

        float normalizedX = Mathf.InverseLerp(bottomLeft.x, topRight.x, player.position.x);
        float normalizedY = Mathf.InverseLerp(bottomLeft.z, topRight.z, player.position.z);
        float posX = (normalizedX - 0.5f) * mapRect.rect.width;
        float posY = (normalizedY - 0.5f) * mapRect.rect.height;

        icon.anchoredPosition = new Vector2(posX, posY);
    }
}
