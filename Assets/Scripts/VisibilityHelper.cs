using System.Collections.Generic;
using UnityEngine;

public class VisibilityHelper
{
	private struct VisibilitySetting
	{
		public GameObject go;

		public bool isVisible;
	}

	private List<VisibilitySetting> visibilities = new List<VisibilitySetting>();

	public void Clear()
	{
		visibilities.Clear();
	}

	public void SaveIsVisible(List<RectTransform> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			RectTransform rectTransform = list[i];
			if (!(rectTransform == null))
			{
				SaveIsVisible(rectTransform.gameObject);
			}
		}
	}

	public void SaveIsVisible(GameObject go)
	{
		if (!(go == null))
		{
			SetActive(go, go.activeSelf);
		}
	}

	public void SetActive(List<Transform> list, bool isVisible)
	{
		for (int i = 0; i < list.Count; i++)
		{
			Transform transform = list[i];
			if (!(transform == null))
			{
				SetActive(transform.gameObject, isVisible);
			}
		}
	}

	public void SetActive(Transform transform, bool isVisible)
	{
		if (!(transform == null))
		{
			SetActive(transform.gameObject, isVisible);
		}
	}

	public void SetActive(GameObject go, bool isVisible)
	{
		for (int i = 0; i < visibilities.Count; i++)
		{
			VisibilitySetting visibilitySetting = visibilities[i];
			if (visibilitySetting.go == go)
			{
				visibilitySetting.isVisible = isVisible;
				visibilities[i] = visibilitySetting;
				return;
			}
		}
		VisibilitySetting item = default(VisibilitySetting);
		item.go = go;
		item.isVisible = isVisible;
		visibilities.Add(item);
	}

	public void Complete()
	{
		for (int i = 0; i < visibilities.Count; i++)
		{
			VisibilitySetting visibilitySetting = visibilities[i];
			GGUtil.SetActive(visibilitySetting.go, visibilitySetting.isVisible);
		}
	}
}
