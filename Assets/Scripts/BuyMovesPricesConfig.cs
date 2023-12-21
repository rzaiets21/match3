using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuyMovesPricesConfig : ScriptableObjectSingleton<BuyMovesPricesConfig>
{
	[Serializable]
	public class OfferConfig
	{
		[Serializable]
		public class PowerupDefinition
		{
			public ChipType powerupType;

			public int count = 1;
		}

		public int movesCount;

		public SingleCurrencyPrice price;

		public List<PowerupDefinition> powerups = new List<PowerupDefinition>();
	}

	[SerializeField]
	private List<OfferConfig> offers = new List<OfferConfig>();

	public OfferConfig GetOffer(int index)
	{
		index = Mathf.Clamp(index, 0, offers.Count - 1);
		return offers[index];
	}

	public OfferConfig GetOffer(GameScreen.StageState state)
	{
		List<GameScreen.GameProgress> gameProgressList = state.gameProgressList;
		int num = 0;
		for (int i = 0; i < gameProgressList.Count; i++)
		{
			Match3Game game = gameProgressList[i].game;
			num += game.timesBoughtMoves;
		}
		return GetOffer(num);
	}
}
