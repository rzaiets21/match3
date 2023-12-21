using System;
using System.Collections.Generic;
using UnityEngine;

public class GeometryConnector : MonoBehaviour
{
	[Serializable]
	public class Connection
	{
		public string path;
	}

	[Serializable]
	public class LoadedTransform
	{
		public string path;

		public Transform transform;
	}

	[SerializeField]
	public List<Connection> connections = new List<Connection>();

	[SerializeField]
	public bool replaceWithMeshColliders;

	[SerializeField]
	public List<LoadedTransform> loaded = new List<LoadedTransform>();

	public GeometryConnectorSettings FindSettings()
	{
		return base.transform.GetComponentInParent<GeometryConnectorSettings>();
	}

	public void AddLoaded(string path, Transform loadedTransform)
	{
		LoadedTransform loadedTransform2 = new LoadedTransform();
		loadedTransform2.path = path;
		loadedTransform2.transform = loadedTransform;
		loaded.Add(loadedTransform2);
	}

	public void ClearLoaded()
	{
		for (int i = 0; i < loaded.Count; i++)
		{
			LoadedTransform loadedTransform = loaded[i];
			if (!(loadedTransform.transform == null))
			{
				UnityEngine.Object.DestroyImmediate(loadedTransform.transform.gameObject);
			}
		}
		loaded.Clear();
	}
}
