using System.Collections.Generic;
using UnityEngine;

public class DestroyUtil
{
	public static List<GameObject> allObjects = new List<GameObject>();

	public static void DontDestroyOnLoad(GameObject gameObject)
	{
		if (!(gameObject == null))
		{
			Object.DontDestroyOnLoad(gameObject);
			allObjects.Add(gameObject);
		}
	}
}
