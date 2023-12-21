using System;
using System.Collections.Generic;
using UnityEngine;

public class GGMessageConfig : ScriptableObject
{
	[Serializable]
	public enum FbObjectType
	{
		EnergyGift,
		CoinGift,
		DollarGift,
		CinemaTrip,
		None
	}

	[Serializable]
	public class FacebookGiftObject
	{
		public FbObjectType objectType;

		public string objectId;

		public string message;

		public float popularityBoost;

		public float moodBoost;

		public int coinCost;
	}

	private static GGMessageConfig instance_;

	public List<FacebookGiftObject> giftDefinitions = new List<FacebookGiftObject>();

	public static GGMessageConfig instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = (Resources.Load("GGServerAssets/GGMessageConfig", typeof(GGMessageConfig)) as GGMessageConfig);
			}
			return instance_;
		}
	}

	public FacebookGiftObject GetGiftForType(FbObjectType type)
	{
		foreach (FacebookGiftObject giftDefinition in giftDefinitions)
		{
			if (giftDefinition.objectType == type)
			{
				return giftDefinition;
			}
		}
		return null;
	}
}
