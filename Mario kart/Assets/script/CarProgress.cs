using UnityEngine;

public class CarProgress : MonoBehaviour
{
    public int passedRankingCheckpoints = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RankingCheckpoint"))
        {
            passedRankingCheckpoints++;
        }
    }
    public int GetProgress()
    {
        return passedRankingCheckpoints;
    }
}
