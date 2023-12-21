using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSaucerBehaviour : MonoBehaviour
{
	[Serializable]
	public class VisualStyle
	{
		[SerializeField]
		private ChipType chipType;

		[SerializeField]
		private ItemColor itemColor;

		[SerializeField]
		private List<Transform> visualItems = new List<Transform>();

		public bool IsApplicable(ChipType chipType, ItemColor itemColor)
		{
			bool flag = chipType == ChipType.Chip;
			if (this.chipType == chipType)
			{
				if (flag)
				{
					return this.itemColor == itemColor;
				}
				return true;
			}
			return false;
		}

		public void SetActive(bool active)
		{
			GGUtil.SetActive(visualItems, active);
		}
	}

	[SerializeField]
	private SpriteRenderer iconSprite;

	[SerializeField]
	private List<VisualStyle> visualStyles = new List<VisualStyle>();

	public void Init(ChipType chipType, ItemColor itemColor)
	{
		for (int i = 0; i < visualStyles.Count; i++)
		{
			VisualStyle visualStyle = visualStyles[i];
			visualStyle.SetActive(visualStyle.IsApplicable(chipType, itemColor));
		}
		ChipDisplaySettings chipDisplaySettings = Match3Settings.instance.GetChipDisplaySettings(chipType, itemColor);
		if (iconSprite != null && chipDisplaySettings != null)
		{
			iconSprite.sprite = chipDisplaySettings.displaySprite;
		}
	}

	public void RemoveFromGame()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
