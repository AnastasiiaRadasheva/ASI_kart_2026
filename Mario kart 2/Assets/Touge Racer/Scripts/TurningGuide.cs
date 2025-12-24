using UnityEngine;

namespace Sain.TougeRacer
{
    public class TurningGuide : MonoBehaviour
    {
        [SerializeField] private Sprite guideSprite;
        [SerializeField] private float guideTime = 5;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                RaceEvents.ShowGuideEvent.Invoke(guideSprite, guideTime);
            }
        }
    }
}
