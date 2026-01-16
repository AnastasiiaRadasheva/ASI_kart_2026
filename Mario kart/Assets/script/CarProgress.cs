using UnityEngine;
using System.Collections.Generic;

public class CarProgress : MonoBehaviour
{
    public int passedRankingCheckpoints = 0; 
    
    private HashSet<int> visitedOnCurrentLap = new HashSet<int>();
    private static int totalCheckpointsInScene = -1;
    private int lastCheckpointID = -1;
    private float lastTimeChecked = 0f;

    void Start()
    {
        if (totalCheckpointsInScene == -1)
        {
            totalCheckpointsInScene = GameObject.FindGameObjectsWithTag("RankingCheckpoint").Length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RankingCheckpoint"))
        {
            int id = other.gameObject.GetInstanceID();

            if (id == lastCheckpointID && Time.time - lastTimeChecked < 0.1f)
            {
                return;
            }

            lastCheckpointID = id;
            lastTimeChecked = Time.time;

            if (!visitedOnCurrentLap.Contains(id))
            {
                passedRankingCheckpoints++;
                visitedOnCurrentLap.Add(id);
                if (visitedOnCurrentLap.Count >= totalCheckpointsInScene * 0.9f)
                {
                    visitedOnCurrentLap.Clear();
                    visitedOnCurrentLap.Add(id);
                }
            }
        }
    }

    public int GetProgress()
    {
        return passedRankingCheckpoints;
    }
}