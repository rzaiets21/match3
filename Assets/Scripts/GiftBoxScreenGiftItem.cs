using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxScreenGiftItem : MonoBehaviour
{
	[Serializable]
	public class GiftTypeStyle
	{
		[SerializeField]
		private GiftBoxScreen.GiftType giftType;

		[SerializeField]
		private PowerupType powerupType;

		[SerializeField]
		private BoosterType boosterType;

		[SerializeField]
		private ChipType chipTypeUsedForRepresentation;

		[SerializeField]
		private Image imageRepresentation;

		[SerializeField]
		private VisualStyleSet style = new VisualStyleSet();

		public TextMeshProUGUI label;

		public bool isApplicable(GiftBoxScreen.Gift gift)
		{
			if (giftType != gift.giftType)
			{
				return false;
			}
			if (giftType == GiftBoxScreen.GiftType.Booster)
			{
				return boosterType == gift.boosterType;
			}
			if (giftType == GiftBoxScreen.GiftType.Powerup)
			{
				return powerupType == gift.powerupType;
			}
			return true;
		}

		public void Apply()
		{
			style.Apply();
			if (!(imageRepresentation == null))
			{
				ChipDisplaySettings chipDisplaySettings = Match3Settings.instance.GetChipDisplaySettings(chipTypeUsedForRepresentation, ItemColor.Uncolored);
				if (chipDisplaySettings != null)
				{
					imageRepresentation.sprite = chipDisplaySettings.displaySprite;
				}
			}
		}
	}

	[SerializeField]
	private List<Transform> widgetsToHide = new List<Transform>();

	[SerializeField]
	private List<GiftTypeStyle> giftStyles = new List<GiftTypeStyle>();

	public void Init(GiftBoxScreen.Gift gift)
	{
		GGUtil.Hide(widgetsToHide);
		GGUtil.Show(this);
		GiftTypeStyle applicableStyle = GetApplicableStyle(gift);
		if (applicableStyle == null)
		{
			UnityEngine.Debug.LogError("NO APPLICABLE STYLE!!!");
			return;
		}
		applicableStyle.Apply();
		if (gift.giftType == GiftBoxScreen.GiftType.Energy)
		{
			GGUtil.ChangeText(applicableStyle.label, $"{gift.hours}hr");
		}
		else
		{
			GGUtil.ChangeText(applicableStyle.label, gift.amount.ToString());
		}
	}

	private GiftTypeStyle GetApplicableStyle(GiftBoxScreen.Gift gift)
	{
		for (int i = 0; i < giftStyles.Count; i++)
		{
			GiftTypeStyle giftTypeStyle = giftStyles[i];
			if (giftTypeStyle.isApplicable(gift))
			{
				return giftTypeStyle;
			}
		}
		return null;
	}
}
