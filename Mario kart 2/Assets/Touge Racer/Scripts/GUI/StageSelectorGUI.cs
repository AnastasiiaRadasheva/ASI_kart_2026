using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sain.TougeRacer
{
    public class StageSelectorGUI : SelectorGUI
    {
        public Image layoutMap;
        public TMP_Text stageName;
        public TMP_Text stageDesc;
        public Slider aiCountSlider;
        
        private GameManager gameManager;
        private StagesDatabase stages;

        private void Awake()
        {
            gameManager = GameManager.Instance;
            stages = gameManager.stagesDatabase;

            min = 0;
            max = stages.stageData.Length - 1;

            defaultQuantity = GameManager.GetSelectedStageIndex();
            gameManager.maxCar = (int)aiCountSlider.value + 1;
        }

        public void UpdateAICount(float count)
        {
            gameManager.maxCar = (int)count + 1;
        }

        protected override void QuantityUpdated()
        {
            GameManager.SetSelectedStageIndex(quantity);
            aiCountSlider.maxValue = stages.stageData[GameManager.GetSelectedStageIndex()].maxCar - 1;
            layoutMap.sprite = stages.stageData[quantity].layoutMap;
            stageName.text = stages.stageData[quantity].stageName;
            stageDesc.text = $"Maximum Car	: {stages.stageData[quantity].maxCar}\nTrack Length	:  {stages.stageData[quantity].trackLength}";

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameManager);
            UnityEditor.EditorUtility.SetDirty(stages);
#endif
        }
    }
}
