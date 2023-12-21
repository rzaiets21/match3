using System;
using System.Collections.Generic;
using UnityEngine;

public class EnergyControlConfig : ScriptableObject
{
	public enum TimeLimitation
	{
		NotLimited,
		InstallTime,
		ExactTime
	}

	public enum CardPrefabType
	{
		EnergyDrinkPrefab,
		OfferPrefab,
		InAppPurchasePrefab
	}

	[Serializable]
	public class EnergyPackConfig
	{
		public string packID;

		public SingleCurrencyPrice price;

		public int drinkCount;

		public string nameSuffix = "Drink";

		public string packStyle;

		public string packBckStyle;

		public string labelStyle;

		public int packWidth;

		public CardPrefabType cardPrefabType;

		public string cueName;

		public string cueStyle;

		public TimeLimitation timeLimitation;

		public float durationInDays;

		public string datetimeWhenFirstAvailable;

		public bool canBuyOneTime;

		public bool showDealSticker;

		public string dealStickerText;

		public string inAppName;

		public string centerText;
	}

	[Serializable]
	public class EnergyPackBundle
	{
		public string packName;

		public List<EnergyPackConfig> packs = new List<EnergyPackConfig>();
	}

	public string energyNotificationsName = "Energy";

	private static EnergyControlConfig instance_;

	public float maxEnergy = 50f;

	public int secondsToRefreshPoint = 30;

	public int totalCoin = 5;

	public SingleCurrencyPrice price;

	public SingleCurrencyPrice freePlay24hPrice;

	public List<EnergyPackBundle> energyPackBundles = new List<EnergyPackBundle>();

	public static EnergyControlConfig instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = (Resources.Load("EnergyControlConfig", typeof(EnergyControlConfig)) as EnergyControlConfig);
			}
			return instance_;
		}
	}

	public int energyPointPerCoin
	{
		get
		{
			float num = maxEnergy / (float)totalCoin;
			if ((float)(int)(maxEnergy / (float)totalCoin) != num)
			{
				UnityEngine.Debug.LogWarning("maxEnergy can not be devided by energyCoin exactly!");
			}
			return (int)(maxEnergy / (float)totalCoin);
		}
	}

	public float CoinsToEnergy(int coins)
	{
		return coins * energyPointPerCoin;
	}

	public float GetEnergyForTimespan(TimeSpan timeSpan)
	{
		return (float)timeSpan.TotalSeconds / (float)secondsToRefreshPoint;
	}

	public TimeSpan TimeToGainEnergy(float energyGain)
	{
		return new TimeSpan(0, 0, (int)energyGain * secondsToRefreshPoint);
	}
}
