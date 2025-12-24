using Sain.Utils;
using UnityEngine;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "StageData", menuName = "Touge Racer/Stage Data")]
    public class StageData : ScriptableObject
    {
        public string stageName;
        public Sprite layoutMap;
        [SceneRef] public string[] SceneStage;
        public AIPool aIPool;
        public int maxCar;
        public float trackLength;
        public RaceType raceType;
        public int lap = 1;
    }
}
