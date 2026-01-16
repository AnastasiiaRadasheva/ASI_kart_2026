using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LapCounter : MonoBehaviour
{
    public int laps = 0;
    public int lapsToWin = 3;

    public TextMeshProUGUI lapText;

    private bool[] checkpointsPassed = new bool[3];
    private int checkpointsCount = 0;

    void Start()
    {
        ResetCheckpoints();
        UpdateText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            CheckpointID cp = other.GetComponent<CheckpointID>();
            if (cp == null) return;

            if (!checkpointsPassed[cp.id])
            {
                checkpointsPassed[cp.id] = true;
                checkpointsCount++;

                if (checkpointsCount >= 3)
                {
                    laps++;
                    ResetCheckpoints();
                    UpdateText();
                }
            }
        }
    }

    void ResetCheckpoints()
    {
        for (int i = 0; i < checkpointsPassed.Length; i++)
        {
            checkpointsPassed[i] = false;
        }

        checkpointsCount = 0;
    }

    void UpdateText()
    {
        lapText.text = "Lap: " + laps + " / " + lapsToWin;
    }
}
