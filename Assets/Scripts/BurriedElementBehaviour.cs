using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BurriedElementBehaviour : SlotComponentBehavoiour
{
	[Serializable]
	public class ElementDescriptor
	{
		public IntVector2 size;

		public Transform image;
	}

	[SerializeField]
	private List<ElementDescriptor> elements = new List<ElementDescriptor>();

	[SerializeField]
	public Transform rotationTransform;

	public void Init(LevelDefinition.BurriedElement element)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			ElementDescriptor elementDescriptor = elements[i];
			GGUtil.SetActive(active: elementDescriptor.size == element.size, transform: elementDescriptor.image);
		}
		Quaternion localRotation = Quaternion.identity;
		if (element.orientation == LevelDefinition.BurriedElement.Orientation.Horizontal)
		{
			localRotation = Quaternion.Euler(0f, 0f, -90f);
		}
		rotationTransform.localRotation = localRotation;
	}

	public override void RemoveFromGame()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
