using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankedBoostersContainer : MonoBehaviour
{
	[Serializable]
	public class Booster
	{
		[SerializeField]
		private Transform container;

		[SerializeField]
		private List<Image> images = new List<Image>();

		public void SetImages(GiftsDefinitionDB.BuildupBooster.BoosterGift booster)
		{
			if (booster == null)
			{
				return;
			}
			List<BoosterConfig> boosterConfig = booster.boosterConfig;
			for (int i = 0; i < images.Count && i < boosterConfig.Count; i++)
			{
				Image image = images[i];
				BoosterConfig boosterConfig2 = boosterConfig[i];
				ChipDisplaySettings chipDisplaySettings = Match3Settings.instance.GetChipDisplaySettings(boosterConfig2.chipType, ItemColor.Uncolored);
				if (chipDisplaySettings != null)
				{
					GGUtil.SetSprite(image, chipDisplaySettings.displaySprite);
				}
			}
		}

		public void SetActive(bool active)
		{
			GGUtil.SetActive(container, active);
		}
	}

	[SerializeField]
	private List<Booster> boosters = new List<Booster>();

	[SerializeField]
	private Transform mainContainer;

	public void Init(int rankLevel)
	{
		GGUtil.SetActive(mainContainer, rankLevel > 0);
		GiftsDefinitionDB.BuildupBooster.BoosterGift boosterGiftForLevel = ScriptableObjectSingleton<GiftsDefinitionDB>.instance.buildupBooster.GetBoosterGiftForLevel(rankLevel);
		for (int i = 0; i < boosters.Count; i++)
		{
			Booster booster = boosters[i];
			bool flag = i + 1 == rankLevel;
			booster.SetActive(flag);
			if (flag)
			{
				booster.SetImages(boosterGiftForLevel);
			}
		}
	}
}
