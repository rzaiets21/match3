using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoScriptPrefabTutorial : MonoBehaviour
	{
		public LightningBoltPrefabScript LightningScript;

		private void Update()
		{
			if (UnityEngine.Input.GetKey(KeyCode.Space))
			{
				LightningScript.Trigger();
			}
		}
	}
}
