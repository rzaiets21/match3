using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoScriptReloadSceneEsc : MonoBehaviour
	{
		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene(0);
			}
		}
	}
}
