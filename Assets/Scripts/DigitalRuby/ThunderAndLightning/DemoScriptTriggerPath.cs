using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DigitalRuby.ThunderAndLightning
{
	public class DemoScriptTriggerPath : MonoBehaviour
	{
		public LightningSplineScript Script;

		public Toggle SplineToggle;

		private readonly List<Vector3> points = new List<Vector3>();

		private void Start()
		{
			Script.ManualMode = true;
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				DemoScript.ReloadCurrentScene();
			}
			else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				Vector3 vector = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
				if (Camera.main.orthographic)
				{
					vector.z = 0f;
				}
				if (points.Count == 0 || (points[points.Count - 1] - vector).magnitude > 8f)
				{
					points.Add(vector);
					Script.Trigger(points, SplineToggle.isOn);
				}
			}
		}
	}
}
