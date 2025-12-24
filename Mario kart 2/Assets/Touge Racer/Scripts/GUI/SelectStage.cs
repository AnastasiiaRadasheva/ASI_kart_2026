using Sain.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Sain.TougeRacer
{
    public class SelectStage : MonoBehaviour
    {
		[SceneRef] public string scene;
        public GameManager gameManager;

        void Awake()
        {
			// GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.raceDetails = raceDetails);
        }
    }
}
