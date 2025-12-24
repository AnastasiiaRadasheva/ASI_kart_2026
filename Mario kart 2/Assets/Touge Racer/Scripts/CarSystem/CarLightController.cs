using UnityEngine;

namespace Sain.TougeRacer
{
    [AddComponentMenu("Arcade Car/Car Light Controller")]
    public class CarLightController : MonoBehaviour
    {
        private CarController controller;

        public bool headlightOn;
        public bool brakelightOn;
        public bool reverselightOn;
        public bool rightBlinkersOn;
        public bool leftBlinkersOn;
        public float blinkerInterval = 0.3f;

        [SerializeField] private CarLight[] headlight;
        [SerializeField] private CarLight[] brakelight;
        [SerializeField] private CarLight[] reverselight;
        [SerializeField] private CarLight[] rightBlinkerlight;
        [SerializeField] private CarLight[] leftBlinkerlight;

        private float blinkerSwitchTime;
        private bool blinkerIntervalOn;

        private void Awake()
        {
            controller = GetComponent<CarController>();
        }

        private void Update()
        {
            // Если CarController отсутствует — просто обновляем ручные флаги (headlightOn и т.п.)
            if (controller != null)
            {
                brakelightOn = controller.IsBraking;
                reverselightOn = controller.IsReverse;
            }

            // Мигалки
            if (leftBlinkersOn || rightBlinkersOn)
            {
                if (blinkerSwitchTime <= 0f)
                {
                    blinkerIntervalOn = !blinkerIntervalOn;
                    blinkerSwitchTime = blinkerInterval;
                }
                else
                {
                    blinkerSwitchTime = Mathf.Max(0f, blinkerSwitchTime - Time.deltaTime);
                }
            }
            else
            {
                blinkerIntervalOn = false;
                blinkerSwitchTime = 0f;
            }

            UpdateLight();
        }

        private void UpdateLight()
        {
            SetLights(headlight, headlightOn);
            SetLights(brakelight, brakelightOn);
            SetLights(reverselight, reverselightOn);
            SetLights(rightBlinkerlight, rightBlinkersOn && blinkerIntervalOn);
            SetLights(leftBlinkerlight, leftBlinkersOn && blinkerIntervalOn);
        }

        private static void SetLights(CarLight[] lights, bool condition)
        {
            if (lights == null || lights.Length == 0) return;

            for (int i = 0; i < lights.Length; i++)
            {
                var l = lights[i];
                if (l == null) continue; // если в массиве есть None — пропускаем
                l.on = condition;
            }
        }
    }
}
