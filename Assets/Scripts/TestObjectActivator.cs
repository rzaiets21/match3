using System.Collections.Generic;
using UnityEngine;

public class TestObjectActivator : MonoBehaviour
{
	public string propertyName;

	public bool defaultValue;

	public List<Transform> objectsToEnable = new List<Transform>();

	public void OnEnable()
	{
		bool @bool = GGAB.GetBool(propertyName, defaultValue);
		for (int i = 0; i < objectsToEnable.Count; i++)
		{
			GGUtil.SetActive(objectsToEnable[i], @bool);
		}
	}
}
