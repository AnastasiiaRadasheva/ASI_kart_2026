using UnityEngine;

[RequireComponent(typeof(Collider))]
public class lastchek : MonoBehaviour
{
    [Header("Respawn direction (optional)")]
    public Transform direction; 

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
