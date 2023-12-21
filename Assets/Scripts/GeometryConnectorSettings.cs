using System;
using System.Collections.Generic;
using UnityEngine;

public class GeometryConnectorSettings : MonoBehaviour
{
	[Serializable]
	public class Link
	{
		public string name;

		public GameObject gameObject;

		public bool positionRelativeToRoot;

		public Vector3 offset;
	}

	private struct InParams
	{
		public Transform root;

		public string namePrefix;

		public string searchPath;

		public bool stopSearchOnFirst;
	}

	private class Output
	{
		public class FoundItem
		{
			public Transform transform;

			public string name;
		}

		public List<FoundItem> found = new List<FoundItem>();

		public bool HasFound => found.Count > 0;
	}

	[SerializeField]
	public List<Link> links = new List<Link>();

	private Link GetLink(string name)
	{
		for (int i = 0; i < links.Count; i++)
		{
			Link link = links[i];
			if (link.name == name)
			{
				return link;
			}
		}
		return null;
	}

	public void HideAllLinks()
	{
		SetAllLinksActive(active: false);
	}

	public void SetAllLinksActive(bool active)
	{
		for (int i = 0; i < links.Count; i++)
		{
			GGUtil.SetActive(links[i].gameObject, active);
		}
	}

	public GameObject GetGameObject(string path)
	{
		string[] array = path.Split(']');
		if (array.Length != 2)
		{
			return null;
		}
		string text = array[0].Substring(1);
		Link link = GetLink(text);
		if (link == null)
		{
			UnityEngine.Debug.Log("Missing Link " + text);
			return null;
		}
		string searchPath = array[1];
		Output output = new Output();
		InParams inParams = default(InParams);
		inParams.root = link.gameObject.transform;
		inParams.searchPath = searchPath;
		inParams.stopSearchOnFirst = true;
		FindPath(inParams, output);
		if (!output.HasFound)
		{
			UnityEngine.Debug.Log("Missing Path " + path);
			return null;
		}
		return output.found[0].transform.gameObject;
	}

	public Vector3 GetWorldPositionUnderLink(Link link, Transform transform)
	{
		if (link.positionRelativeToRoot)
		{
			return link.gameObject.transform.InverseTransformPoint(transform.position) + link.offset;
		}
		return transform.position;
	}

	public GameObject InstantiateGameObject(string path, Transform parent)
	{
		string[] array = path.Split(']');
		if (array.Length != 2)
		{
			return null;
		}
		string text = array[0].Substring(1);
		Link link = GetLink(text);
		if (link == null)
		{
			UnityEngine.Debug.Log("Missing Link " + text);
			return null;
		}
		string searchPath = array[1];
		Output output = new Output();
		InParams inParams = default(InParams);
		inParams.root = link.gameObject.transform;
		inParams.searchPath = searchPath;
		inParams.stopSearchOnFirst = true;
		FindPath(inParams, output);
		if (!output.HasFound)
		{
			UnityEngine.Debug.Log("Missing Path " + path);
			return null;
		}
		GameObject gameObject = output.found[0].transform.gameObject;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, parent);
		gameObject2.name = gameObject.name;
		GGUtil.SetActive(gameObject2, active: true);
		if (link.positionRelativeToRoot)
		{
			GGUtil.CopyWorldTransform(gameObject.transform, gameObject2.transform);
			gameObject2.transform.position = link.gameObject.transform.InverseTransformPoint(gameObject2.transform.position) + link.offset;
		}
		else
		{
			GGUtil.CopyWorldTransform(gameObject.transform, gameObject2.transform);
		}
		return gameObject2;
	}

	private void FindPath(InParams inParams, Output output)
	{
		if (inParams.searchPath == "" || inParams.searchPath == "/")
		{
			Output.FoundItem foundItem = new Output.FoundItem();
			foundItem.transform = inParams.root;
			foundItem.name = inParams.searchPath;
			output.found.Add(foundItem);
		}
		else if (!output.HasFound || !inParams.stopSearchOnFirst)
		{
			foreach (Transform item in inParams.root)
			{
				string text = inParams.namePrefix + "/" + item.name;
				if (inParams.searchPath == text || inParams.searchPath == item.name)
				{
					Output.FoundItem foundItem2 = new Output.FoundItem();
					foundItem2.transform = item;
					foundItem2.name = text;
					output.found.Add(foundItem2);
				}
				else
				{
					InParams inParams2 = inParams;
					inParams2.namePrefix = text;
					inParams2.root = item;
					FindPath(inParams2, output);
				}
				if (output.HasFound && inParams.stopSearchOnFirst)
				{
					break;
				}
			}
		}
	}
}
