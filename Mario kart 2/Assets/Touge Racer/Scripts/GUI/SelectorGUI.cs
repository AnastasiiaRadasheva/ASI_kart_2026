using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sain.TougeRacer
{
    public class SelectorGUI : MonoBehaviour
    {
        public Button buttonDown, buttonUp;
        // public TMP_Text quantityTMP;
        public int defaultQuantity = 2;

        protected int min;
        protected int quantity;
        protected int max;

        private void Start()
        {
            buttonDown.onClick.AddListener(OnClickDown);
            buttonUp.onClick.AddListener(OnClickUp);

            if (defaultQuantity < min)
                Debug.LogError("default quantity lower than minimum");
            if (defaultQuantity > max)
                Debug.LogError("default quantity higer than maximum");

            quantity = defaultQuantity;
            UpdateQuantity(0);
        }

        private void OnClickDown()
        {
            UpdateQuantity(-1);
        }

        private void OnClickUp()
        {
            UpdateQuantity(1);
        }

        protected virtual void UpdateQuantity(int change)
        {
            quantity = Mathf.Clamp(quantity + change, min, max);
            buttonUp.interactable = quantity < max;
            buttonDown.interactable = quantity > min;
            QuantityUpdated();
        }

        protected virtual void QuantityUpdated() {}
    }
}