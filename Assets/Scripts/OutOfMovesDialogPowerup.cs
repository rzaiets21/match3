using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutOfMovesDialogPowerup : MonoBehaviour
{
	[Serializable]
	public class PowerupContainer
	{
		public ChipType powerupType;

		public RectTransform container;

		public Image image;
	}

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private List<PowerupContainer> powerups = new List<PowerupContainer>();

	[SerializeField]
	private TextMeshProUGUI countLabel;

	public void Init(ChipType powerupType, int powerupCount)
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		for (int i = 0; i < powerups.Count; i++)
		{
			PowerupContainer powerupContainer = powerups[i];
			if (powerupContainer.powerupType != powerupType)
			{
				continue;
			}
			GGUtil.SetActive(powerupContainer.container, active: true);
			if (!(powerupContainer.image == null))
			{
				ChipDisplaySettings chipDisplaySettings = Match3Settings.instance.GetChipDisplaySettings(powerupContainer.powerupType, ItemColor.Uncolored);
				if (chipDisplaySettings != null)
				{
					GGUtil.SetSprite(powerupContainer.image, chipDisplaySettings.displaySprite);
				}
			}
		}
		GGUtil.ChangeText(countLabel, $"+{powerupCount}");
	}
}
