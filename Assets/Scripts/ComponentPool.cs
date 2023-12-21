using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComponentPool
{
	public Transform parent;

	public GameObject prefab;

	[SerializeField]
	private bool resetScale;

	[SerializeField]
	private bool resetRotation;

	[NonSerialized]
	public List<GameObject> usedObjects = new List<GameObject>();

	private List<GameObject> notUsedObjects = new List<GameObject>();

	public Vector2 prefabSizeDelta
	{
		get
		{
			RectTransform component = prefab.GetComponent<RectTransform>();
			if (component == null)
			{
				return Vector2.zero;
			}
			if (resetScale)
			{
				component.localScale = Vector3.one;
			}
			return component.sizeDelta;
		}
	}

	public void Clear()
	{
		if (GGUtil.isPartOfHierarchy(prefab))
		{
			GGUtil.Hide(prefab);
		}
		for (int num = usedObjects.Count - 1; num >= 0; num--)
		{
			GameObject item = usedObjects[num];
			notUsedObjects.Add(item);
		}
		usedObjects.Clear();
	}

	public void HideNotUsed()
	{
		for (int i = 0; i < notUsedObjects.Count; i++)
		{
			GGUtil.SetActive(notUsedObjects[i], active: false);
		}
	}

	public GameObject Instantiate(bool activate = false)
	{
		GameObject gameObject = null;
		if (prefab == null)
		{
			return null;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(prefab);
		gameObject2.transform.parent = parent.transform;
		gameObject = gameObject2;
		if (resetScale)
		{
			gameObject.transform.localScale = Vector3.one;
		}
		if (resetRotation)
		{
			gameObject.transform.localRotation = Quaternion.identity;
		}
		if (activate)
		{
			GGUtil.SetActive(gameObject, active: true);
		}
		return gameObject;
	}

	public GameObject Next(bool activate = false)
	{
		GameObject gameObject = null;
		if (prefab == null)
		{
			return null;
		}
		if (notUsedObjects.Count > 0)
		{
			gameObject = notUsedObjects[notUsedObjects.Count - 1];
			notUsedObjects.RemoveAt(notUsedObjects.Count - 1);
		}
		else
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(prefab);
			gameObject2.transform.parent = parent.transform;
			gameObject = gameObject2;
			if (resetScale)
			{
				gameObject.transform.localScale = Vector3.one;
			}
			if (resetRotation)
			{
				gameObject.transform.localRotation = Quaternion.identity;
			}
		}
		if (activate)
		{
			GGUtil.SetActive(gameObject, active: true);
		}
		usedObjects.Add(gameObject);
		return gameObject;
	}

	public T Next<T>(bool activate = false) where T : MonoBehaviour
	{
		return Next(activate).GetComponent<T>();
	}
}
