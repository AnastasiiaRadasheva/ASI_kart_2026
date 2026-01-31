using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LavaRestart : MonoBehaviour
{
    [Header("UI")]
    public GameObject losePanel;
    public float restartDelay = 2f;

    private restart restartScript;
    private bool triggered;

    private void Start()
    {
        restartScript = FindObjectOfType<restart>();

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("PlayerMain")) return;

        triggered = true;

        if (losePanel != null)
            losePanel.SetActive(true);

        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        if (restartScript != null && !string.IsNullOrEmpty(restartScript.sceneToRestart))
            SceneManager.LoadScene(restartScript.sceneToRestart);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
