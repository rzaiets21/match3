using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoScript2D : MonoBehaviour
	{
		public GameObject SpriteToRotate;

		public LightningBoltPrefabScriptBase LightningScript;

		private void Update()
		{
			SpriteToRotate.transform.Rotate(0f, 0f, LightningBoltScript.DeltaTime * 10f);
		}
	}
}
