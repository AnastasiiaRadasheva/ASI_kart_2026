using UnityEngine;
using UnityEngine.UI;

namespace Sain.TougeRacer
{
    public class TurningGuideGUI : MonoBehaviour
    {
        private Image guideImage;
        private float guideTimer;

        void Awake()
        {
            guideImage = GetComponent<Image>();

            ResetGuide();
        }

        void Update()
        {
            guideTimer -= Time.deltaTime;

            if (guideTimer < 0)
            {
                ResetGuide();
            }
        }

        void OnEnable()
        {
            RaceEvents.ShowGuideEvent.AddListener(ShowGuide);
        }

        void OnDisable()
        {
            RaceEvents.ShowGuideEvent.RemoveListener(ShowGuide);
        }

        private void ResetGuide()
        {
            guideImage.sprite = null;
            guideImage.enabled = false;
        }

        private void ShowGuide(Sprite sprite, float guideTime)
        {
            guideTimer = guideTime;

            guideImage.sprite = sprite;
            guideImage.enabled = true;
        }
    }
}
