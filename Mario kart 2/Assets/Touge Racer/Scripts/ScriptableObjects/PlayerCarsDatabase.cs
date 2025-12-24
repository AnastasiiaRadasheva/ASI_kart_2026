using UnityEngine;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "PlayerCarsDatabase", menuName = "Touge Racer/Player Cars Database")]
    public class PlayerCarsDatabase : ScriptableObject
    {
        public int selectedCar;
        public PlayerCars[] playerCars;
    }

    [System.Serializable]
    public struct PlayerCars
    {
        public string name;
        [TextArea] public string description;
        public CarController carPrefab;
        public GameObject modelPreview;
        public bool isAvailable;
    }
}
