using UnityEngine;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "StagesDatabase", menuName = "Touge Racer/Stages Database")]
    public class StagesDatabase : ScriptableObject
    {
        public int selectedStage;
        public StageData[] stageData;

        public void RefreshStage()
        {
            StageData[] stages = Resources.LoadAll<StageData>("Stage Data");
            stageData = stages;
        }
    }
}
