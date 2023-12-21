using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VisualStyleSet
{
	[Serializable]
	public class ObjectStyle
	{
		public enum Action
		{
			Show,
			Hide
		}

		public Action action;

		public Transform visual;

		public void Apply()
		{
			bool active = action == Action.Show;
			GGUtil.SetActive(visual, active);
		}

		public void Apply(VisibilityHelper visibility)
		{
			bool isVisible = action == Action.Show;
			visibility.SetActive(visual, isVisible);
		}
	}

	[SerializeField]
	private List<ObjectStyle> objects = new List<ObjectStyle>();

	public void Apply()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			objects[i].Apply();
		}
	}

	public void Apply(VisibilityHelper visibility)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			objects[i].Apply(visibility);
		}
	}
}
