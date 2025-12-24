using UnityEngine;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "AIPool", menuName = "Touge Racer/AI Pool")]
    public class AIPool : ScriptableObject
    {
        public AIData[] ai;

        public CarController GetRandomAI()
        {
            int maxAI = ai.Length - 1;
            int rand = Random.Range(0, maxAI);

            return ai[rand].carPrefab;
        }
    }
}
