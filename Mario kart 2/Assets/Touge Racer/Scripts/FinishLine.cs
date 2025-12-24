using UnityEngine;

namespace Sain.TougeRacer
{
    public class FinishLine : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarController controller))
            {
                RaceEvents.ThroughFinishLineEvent.Invoke(this, controller);
            }
        }
    }
}
