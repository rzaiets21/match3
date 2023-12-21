using UnityEngine;
using UnityEngine.UI;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoScriptManualAutomatic : MonoBehaviour
	{
		public GameObject LightningPrefab;

		public Toggle AutomaticToggle;

		public Transform a;

		public Transform b;

		private void Update()
		{
			if (Input.GetMouseButton(0))
			{
				Vector3 vector = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
				vector.z = 0f;
				LightningPrefab.GetComponent<LightningBoltPrefabScriptBase>().Trigger(a.position, b.position);
			}
		}

		public void AutomaticToggled()
		{
			LightningPrefab.GetComponent<LightningBoltPrefabScriptBase>().ManualMode = !AutomaticToggle.isOn;
		}

		public void ManualTriggerClicked()
		{
			LightningPrefab.GetComponent<LightningBoltPrefabScriptBase>().Trigger();
		}
	}
}
