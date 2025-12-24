using UnityEngine;
using UnityEngine.UI;

namespace Sain.Utils
{
    [RequireComponent(typeof(Button))]
	public class LoadSceneButton : MonoBehaviour
	{
		[SceneRef] public string scene;

		void Awake()
		{
			GetComponent<Button>().onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(scene));
		}
	}
}
