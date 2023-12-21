using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MagicHatBehaviour : SlotComponentBehavoiour
{
	[Serializable]
	public class ChipTypeLook
	{
		public ChipType chipType;

		public List<Transform> widgetsToShow = new List<Transform>();
	}

	[SerializeField]
	private Transform bunnyTransform;

	[SerializeField]
	private TextMeshPro text;

	[SerializeField]
	private Transform nonCountObject;

	[SerializeField]
	private List<ChipTypeLook> chipLooks = new List<ChipTypeLook>();

	public Vector3 bunnyScale
	{
		get
		{
			return bunnyTransform.localScale;
		}
		set
		{
			bunnyTransform.localScale = value;
		}
	}

	public Vector3 bunnyOffset
	{
		get
		{
			return bunnyTransform.localPosition;
		}
		set
		{
			bunnyTransform.localPosition = value;
		}
	}

	public void Init(ChipType chipType)
	{
		for (int i = 0; i < chipLooks.Count; i++)
		{
			ChipTypeLook chipTypeLook = chipLooks[i];
			GGUtil.SetActive(chipTypeLook.widgetsToShow, chipTypeLook.chipType == chipType);
		}
	}

	public void SetCountActive(bool active)
	{
		GGUtil.SetActive(text, active);
		GGUtil.SetActive(nonCountObject, !active);
	}

	public void SetCount(int count)
	{
		if (!(text == null))
		{
			text.text = count.ToString();
		}
	}
}
