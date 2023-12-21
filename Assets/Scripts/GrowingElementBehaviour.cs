using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class GrowingElementBehaviour : SlotComponentBehavoiour
{
	[SerializeField]
	private List<GrowingElementPot> growingElements = new List<GrowingElementPot>();

	public void Init()
	{
		SetLevel(0);
		GGUtil.SetActive(this, active: true);
	}

	public void StopAllAnimations()
	{
		for (int i = 0; i < growingElements.Count; i++)
		{
			growingElements[i].StopAnimation();
		}
	}

	private GrowingElementPot GetElementPot(int elementIndex)
	{
		return growingElements[Mathf.Clamp(elementIndex, 0, growingElements.Count - 1)];
	}

	public void StartAnimationFor(int elementIndex)
	{
		GetElementPot(elementIndex).AnimateIn();
	}

	public void SetLevel(int level)
	{
		for (int i = 0; i < growingElements.Count; i++)
		{
			growingElements[i].SetActve(level > i);
		}
	}

	public Vector3 WorldPositionForElement(int elementIndex)
	{
		return GetElementPot(elementIndex).WorldPositionForFlower;
	}
}
