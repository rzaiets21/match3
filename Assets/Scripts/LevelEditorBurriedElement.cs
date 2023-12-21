using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorBurriedElement : MonoBehaviour
{
	[Serializable]
	public class BurriedElementDesc
	{
		public IntVector2 size;

		public Image image;
	}

	[SerializeField]
	private List<BurriedElementDesc> elements = new List<BurriedElementDesc>();

	[SerializeField]
	private RectTransform rotationRect;

	public void Init(LevelEditorVisualizer viz, LevelDefinition level, LevelDefinition.BurriedElement burriedElement)
	{
		for (int i = 0; i < elements.Count; i++)
		{
			BurriedElementDesc burriedElementDesc = elements[i];
			GGUtil.SetActive(burriedElementDesc.image, burriedElementDesc.size == burriedElement.size);
		}
		Quaternion localRotation = Quaternion.identity;
		if (burriedElement.orientation == LevelDefinition.BurriedElement.Orientation.Horizontal)
		{
			localRotation = Quaternion.Euler(0f, 0f, 90f);
		}
		Vector3 localPosition = viz.GetLocalPosition(level, burriedElement.position);
		Vector3 localPosition2 = viz.GetLocalPosition(level, burriedElement.oppositeCornerPosition);
		Vector3 localPosition3 = Vector3.Lerp(localPosition, localPosition2, 0.5f);
		base.transform.localPosition = localPosition3;
		base.transform.localRotation = localRotation;
	}
}
