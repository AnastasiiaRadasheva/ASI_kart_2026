using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "GameManager", menuName = "Touge Racer/Game Manager")]
    public class GameManager : ScriptableObject
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<GameManager>("GameManager");
#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = CreateInstance<GameManager>();
                        // Optionally save the new instance as an asset in the editor
                        AssetDatabase.CreateAsset(instance, "Assets/Resources/GameManager.asset");
                    }
#endif
                }

                return instance;
            }
        }
        
        public string playerName;

        public CarDatabase carsDatabase;
        public StagesDatabase stagesDatabase;
        public int maxCar;

        public StageData stageData => stagesDatabase.stageData[GetSelectedStageIndex()];
        public CarController playerCar => carsDatabase.cars[GetSelectedCarIndex()].carPrefab;

        public AIData[] AICar => GetAICar();

        private List<AIData> availableAICar;
        private List<AIData> chosenAICar;

        private const string SELECTED_CAR = "SELECTED_CAR";
        private const string SELECTED_STAGE = "SELECTED_STAGE";
        private const string SELECTED_CAMERA = "SELECTED_CAMERA";

        public AIData[] GetAICar()
        {
            availableAICar = new List<AIData>(stageData.aIPool.ai);
            chosenAICar = new List<AIData>();

            int aiCount = Mathf.Min(availableAICar.Count, maxCar);

            for (int i = 0; i < aiCount; i++)
            {
                int randomIndex = Random.Range(0, availableAICar.Count);

                chosenAICar.Add(availableAICar[randomIndex]);

                availableAICar.RemoveAt(randomIndex);
            }

            return chosenAICar.ToArray();
        }

        public static int GetSelectedCarIndex()
        {
            return PlayerPrefs.GetInt(SELECTED_CAR);
        }

        public static void SetSelectedCarIndex(int index)
        {
            PlayerPrefs.SetInt(SELECTED_CAR, index);
        }

        public static int GetSelectedStageIndex()
        {
            return PlayerPrefs.GetInt(SELECTED_STAGE);
        }

        public static void SetSelectedStageIndex(int index)
        {
            PlayerPrefs.SetInt(SELECTED_STAGE, index);
        }

        public static int GetSelectedCameraIndex()
        {
            return PlayerPrefs.GetInt(SELECTED_CAMERA);
        }

        public static void SetSelectedCameraIndex(int index)
        {
            PlayerPrefs.SetInt(SELECTED_CAMERA, index);
        }

        public void LoadScene()
        {
            _ = LoadSceneAwaitable();
        }

        async Awaitable LoadSceneAwaitable()
        {
            await SceneManager.LoadSceneAsync(stageData.SceneStage[0]);

            if (stageData.SceneStage.Length <= 1) return;

            for (int i = 1; i < stageData.SceneStage.Length; i++)
            {
                await SceneManager.LoadSceneAsync(stageData.SceneStage[i], LoadSceneMode.Additive);
            }
        }
    }
    
    public enum RaceType
    {
        Sprint,
        Circuit,
        // TimeAttack,
        // TimeTrial,
        // FreeRoam
    }
}
