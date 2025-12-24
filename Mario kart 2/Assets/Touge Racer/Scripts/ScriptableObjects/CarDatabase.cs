using UnityEngine;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "CarDatabase", menuName = "Touge Racer/Car Database")]
    public class CarDatabase : ScriptableObject
    {
        public CarData[] cars;
    }

    [System.Serializable]
    public struct CarData
    {
        public string name;
        [TextArea] public string description;
        public CarController carPrefab;
        public GameObject modelPreview;
    }
}
