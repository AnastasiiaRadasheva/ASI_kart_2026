using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CarProgress : MonoBehaviour
{
    [Header("Status")]
    public int passedRankingCheckpoints = 0; 
    
    private Dictionary<int, int> checkpointPassCounts = new Dictionary<int, int>();
    
    private static List<int> allCheckpointIDs = null;
    
    private int currentCycle = 0;
    private bool isFinished = false;

    void Awake()
    {
        if (allCheckpointIDs == null)
        {
            allCheckpointIDs = GameObject.FindGameObjectsWithTag("RankingCheckpoint")
                .Select(obj => obj.GetInstanceID())
                .ToList();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFinished) return;

        if (other.CompareTag("RankingCheckpoint"))
        {
            int id = other.gameObject.GetInstanceID();

            if (!checkpointPassCounts.ContainsKey(id))
            {
                checkpointPassCounts.Add(id, 0);
            }

            if (checkpointPassCounts[id] == currentCycle)
            {
                passedRankingCheckpoints++;
                checkpointPassCounts[id]++;

                if (CheckCycleCompletion())
                {
                    currentCycle++;
                }
            }
        }
    }

    private bool CheckCycleCompletion()
    {
        int passedInCurrentCycle = 0;
        foreach (var pair in checkpointPassCounts)
        {
            if (pair.Value > currentCycle) passedInCurrentCycle++;
        }
        
        return passedInCurrentCycle >= allCheckpointIDs.Count * 0.9f;
    }

    public void StopProgress()
    {
        isFinished = true;
    }

    public int GetProgress()
    {
        return passedRankingCheckpoints;
    }
}