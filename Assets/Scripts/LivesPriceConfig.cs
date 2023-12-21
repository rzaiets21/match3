using System;
using System.Collections.Generic;
using UnityEngine;

public class LivesPriceConfig : ScriptableObjectSingleton<LivesPriceConfig>
{
	[Serializable]
	public class PriceConfig
	{
		public SingleCurrencyPrice pricePerLife;

		public int levelIndex;

		public SingleCurrencyPrice GetPriceForLives(int lives)
		{
			return new SingleCurrencyPrice
			{
				cost = lives * pricePerLife.cost,
				currency = pricePerLife.currency
			};
		}
	}

	[SerializeField]
	private List<PriceConfig> priceConfigs = new List<PriceConfig>();

	[SerializeField]
	private PriceConfig defaultConfig = new PriceConfig();

	public PriceConfig GetPriceForLevelOrDefault(int levelIndex)
	{
		for (int i = 0; i < priceConfigs.Count; i++)
		{
			PriceConfig priceConfig = priceConfigs[i];
			if (priceConfig.levelIndex == levelIndex)
			{
				return priceConfig;
			}
		}
		return defaultConfig;
	}
	
	public PriceConfig GetPriceForLevelOrDefault()
	{
		return defaultConfig;
	}
}
