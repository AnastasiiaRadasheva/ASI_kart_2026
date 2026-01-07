using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index;
    
    private void OnTriggerEnter(Collider other)
{
    if (other.attachedRigidbody == null) return;

    RaceManager raceManager = FindObjectOfType<RaceManager>();
    if (raceManager == null) return;

    raceManager.OnCarPassedCheckpoint(other.attachedRigidbody, index);
}

}
