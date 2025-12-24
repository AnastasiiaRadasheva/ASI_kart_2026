using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sain.TougeRacer
{
    public class CarSelectorGUI : SelectorGUI
    {
        public GameObject previewParent;
        public TMP_Text carName;
        public Slider accelSlider;
        public Slider speedSlider;
        public Slider handlingSlider;

        private GameManager gameManager;
        private List<GameObject> carModels = new();

        void Awake()
        {
            gameManager = GameManager.Instance;
            foreach (var car in gameManager.carsDatabase.cars)
            {
                carModels.Add(Instantiate(car.modelPreview, previewParent.transform));
            }

            min = 0;
            max = gameManager.carsDatabase.cars.Length - 1;

            defaultQuantity = GameManager.GetSelectedCarIndex();
        }

        protected override void QuantityUpdated()
        {
            GameManager.SetSelectedCarIndex(quantity);
            carName.text = gameManager.carsDatabase.cars[quantity].name;
            accelSlider.value = gameManager.carsDatabase.cars[quantity].carPrefab.Acceleration;
            speedSlider.value = gameManager.carsDatabase.cars[quantity].carPrefab.MaxSpeed;
            handlingSlider.value = gameManager.carsDatabase.cars[quantity].carPrefab.Handling;
            UpdateCar();
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameManager.carsDatabase);
#endif
        }

        public void DeactiveAllCar()
        {
            foreach (var car in carModels)
            {
                car.SetActive(false);
            }
        }

        public void UpdateCar()
        {
            for (int i = 0; i < gameManager.carsDatabase.cars.Length; i++)
            {
                if (i == quantity) carModels[i].SetActive(true);
                else carModels[i].SetActive(false);
            }
        }
    }
}
