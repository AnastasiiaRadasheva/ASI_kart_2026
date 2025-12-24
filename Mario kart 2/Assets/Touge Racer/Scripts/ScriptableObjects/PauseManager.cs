using UnityEditor;
using UnityEngine;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "PauseManager", menuName = "Touge Racer/Pause Manager")]
    public class PauseManager : ScriptableObject
    {
        private static PauseManager instance;
        public static PauseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<PauseManager>("PauseManager");
#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = CreateInstance<PauseManager>();
                        // Optionally save the new instance as an asset in the editor
                        AssetDatabase.CreateAsset(instance, "Assets/Resources/PauseManager.asset");
                    }
#endif
                }

                return instance;
            }
        }

        public bool isPaused;
        public GameObject pausePrefab;

        private GameObject pauseObject;

        public void Pause()
        {
            CheckPauseObject();
            isPaused = true;
            Time.timeScale = 0;
            AudioListener.pause = true;
            pauseObject.SetActive(true);
        }

        public void Resume()
        {
            CheckPauseObject();
            isPaused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
            pauseObject.SetActive(false);
        }

        public void CheckPauseObject()
        {
            if (pauseObject == null)
                pauseObject = Instantiate(pausePrefab);
        }
    }
}
