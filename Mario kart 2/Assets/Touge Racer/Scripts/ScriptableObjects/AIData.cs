using UnityEngine;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "AIData", menuName = "Touge Racer/AI Data")]
    public class AIData : ScriptableObject
    {
        public string aiName;
        public CarController carPrefab;
        public Material bodyPaint;
    }
}
