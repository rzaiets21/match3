using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GiftsDefinitionDB : ScriptableObjectSingleton<GiftsDefinitionDB>
{
	[Serializable]
	public class BuildupBooster
	{
		[Serializable]
		public class BoosterGift
		{
			public int totalGamesWonInARow;

			[NonSerialized]
			public int level;

			public List<BoosterConfig> boosterConfig = new List<BoosterConfig>();
		}

		[SerializeField]
		private bool isEnabled;

		[SerializeField]
		private int minStageBeforeEnabled;

		[SerializeField]
		private List<BoosterGift> boosters = new List<BoosterGift>();

		public int currentBoosterLevel => GetBoosterGift()?.level ?? 0;

		public BoosterGift GetBoosterGiftForLevel(int level)
		{
			if (level == 0)
			{
				return null;
			}
			if (boosters.Count == 0)
			{
				return null;
			}
			return boosters[Mathf.Clamp(level - 1, 0, boosters.Count - 1)];
		}

		public BoosterGift GetBoosterGift()
		{
			if (!isEnabled)
			{
				return null;
			}
			int num = Match3StagesDB.instance.PassedStagesInRow(minStageBeforeEnabled + 1);
			if (Match3StagesDB.instance.passedStages < minStageBeforeEnabled)
			{
				return null;
			}
			for (int num2 = boosters.Count - 1; num2 >= 0; num2--)
			{
				BoosterGift boosterGift = boosters[num2];
				if (num >= boosterGift.totalGamesWonInARow)
				{
					boosterGift.level = num2 + 1;
					return boosterGift;
				}
			}
			return null;
		}
	}

	[Serializable]
	public class GiftDefinition
	{
		public struct StagesPassedDescriptor
		{
			public int currentStagesPassed;

			public int stagesNeededToPass;
		}

		[SerializeField]
		private int stagesPassedToGive;

		public GiftBoxScreen.GiftsDefinition gifts = new GiftBoxScreen.GiftsDefinition();

		[NonSerialized]
		private int previousStagePassedToGive;

		[NonSerialized]
		public int totalStagesPassedToGive;

		[NonSerialized]
		private GiftsDefinitionDB db;

		public StagesPassedDescriptor stagesPassedDescriptor
		{
			get
			{
				StagesPassedDescriptor result = default(StagesPassedDescriptor);
				result.currentStagesPassed = Mathf.Max(0, Match3StagesDB.instance.passedStages - previousStagePassedToGive);
				result.stagesNeededToPass = stagesPassedToGive;
				return result;
			}
		}

		public float progress => Mathf.InverseLerp(previousStagePassedToGive, totalStagesPassedToGive, Match3StagesDB.instance.passedStages);

		public bool isAvailableToCollect => progress >= 1f;

		public void ClaimGifts()
		{
			db.ClaimGift(totalStagesPassedToGive);
		}

		public void Init(GiftsDefinitionDB db, GiftDefinition previousGift)
		{
			this.db = db;
			if (previousGift != null)
			{
				previousStagePassedToGive = previousGift.totalStagesPassedToGive;
			}
			else
			{
				previousStagePassedToGive = 0;
			}
			totalStagesPassedToGive = previousStagePassedToGive + stagesPassedToGive;
		}
	}

	public struct CombinedGifts
	{
		public int bombCount;

		public int discoCount;

		public int rocketCount;

		public int hammerCount;

		public int powerHammerCount;

		public int coinsCount;
	}

	[Serializable]
	public class DailyGifts
	{
		[Serializable]
		public class DailyGift
		{
			[NonSerialized]
			public int index;

			[SerializeField]
			public GiftBoxScreen.GiftsDefinition gifts = new GiftBoxScreen.GiftsDefinition();
		}

		[SerializeField]
		private int hoursTillDailyCoinsAvailable = 18;

		[SerializeField]
		private List<DailyGift> gifts = new List<DailyGift>();

		private int currentGiftIndex
		{
			get
			{
				return GGPlayerSettings.instance.Model.timesCollectedFreeCoins;
			}
			set
			{
				GGPlayerSettings instance = GGPlayerSettings.instance;
				instance.Model.timesCollectedFreeCoins = value;
				instance.Save();
			}
		}

		public DailyGift currentDailyGift
		{
			get
			{
				if (gifts.Count == 0)
				{
					return null;
				}
				return gifts[Mathf.Clamp(currentGiftIndex, 0, gifts.Count - 1)];
			}
		}

		public bool IsSelected(int index)
		{
			int num = Mathf.Clamp(currentGiftIndex, 0, gifts.Count - 1);
			return index == num;
		}

		public void Init()
		{
			GGPlayerSettings instance = GGPlayerSettings.instance;
			if (instance.Model.timeWhenLastCollectedDailyCoins == 0L)
			{
				instance.Model.timeWhenLastCollectedDailyCoins = DateTime.Now.Ticks;
				instance.Save();
			}
			for (int i = 0; i < gifts.Count; i++)
			{
				gifts[i].index = i;
			}
		}

		public void OnClaimedDailyCoins()
		{
			GGPlayerSettings instance = GGPlayerSettings.instance;
			instance.Model.timeWhenLastCollectedDailyCoins = DateTime.Now.Ticks;
			currentGiftIndex++;
			instance.Save();
		}
	}

	[SerializeField]
	private List<GiftDefinition> gifts = new List<GiftDefinition>();

	[SerializeField]
	public DailyGifts dailyGifts = new DailyGifts();

	private List<GiftDefinition> giftsToStage = new List<GiftDefinition>();

	[SerializeField]
	public BuildupBooster buildupBooster = new BuildupBooster();

	public int lastStageWhenGivenGift
	{
		get
		{
			return GGPlayerSettings.instance.givenGifts.lastStageWhenGivenGift;
		}
		set
		{
			GGPlayerSettings.instance.givenGifts.lastStageWhenGivenGift = value;
			GGPlayerSettings.instance.Save();
		}
	}

	public GiftDefinition currentGift
	{
		get
		{
			int lastStageWhenGivenGift = this.lastStageWhenGivenGift;
			for (int i = 0; i < gifts.Count; i++)
			{
				GiftDefinition giftDefinition = gifts[i];
				if (giftDefinition.totalStagesPassedToGive > lastStageWhenGivenGift)
				{
					return giftDefinition;
				}
			}
			return null;
		}
	}

	public CombinedGifts GetCombinedGiftsTillStage(int stageIndex)
	{
		return GetCombinedGifts(GiftsTillStage(stageIndex));
	}

	public List<GiftDefinition> GiftsTillStage(int stageIndex)
	{
		giftsToStage.Clear();
		for (int i = 0; i < gifts.Count; i++)
		{
			GiftDefinition giftDefinition = gifts[i];
			if (giftDefinition.totalStagesPassedToGive <= stageIndex)
			{
				giftsToStage.Add(giftDefinition);
			}
		}
		return giftsToStage;
	}

	public CombinedGifts GetCombinedGifts(List<GiftDefinition> gifts)
	{
		CombinedGifts result = default(CombinedGifts);
		for (int i = 0; i < gifts.Count; i++)
		{
			GiftDefinition giftDefinition = gifts[i];
			for (int j = 0; j < giftDefinition.gifts.gifts.Count; j++)
			{
				GiftBoxScreen.Gift gift = giftDefinition.gifts.gifts[j];
				if (gift.giftType == GiftBoxScreen.GiftType.Booster && gift.boosterType == BoosterType.VerticalRocketBooster)
				{
					result.rocketCount += gift.amount;
				}
				if (gift.giftType == GiftBoxScreen.GiftType.Booster && gift.boosterType == BoosterType.BombBooster)
				{
					result.bombCount += gift.amount;
				}
				if (gift.giftType == GiftBoxScreen.GiftType.Booster && gift.boosterType == BoosterType.DiscoBooster)
				{
					result.discoCount += gift.amount;
				}
				if (gift.giftType == GiftBoxScreen.GiftType.Powerup && gift.powerupType == PowerupType.Hammer)
				{
					result.hammerCount += gift.amount;
				}
				if (gift.giftType == GiftBoxScreen.GiftType.Powerup && gift.powerupType == PowerupType.PowerHammer)
				{
					result.powerHammerCount += gift.amount;
				}
				if (gift.giftType == GiftBoxScreen.GiftType.Coins)
				{
					result.coinsCount += gift.amount;
				}
			}
		}
		return result;
	}

	protected void ClaimGift(int stagePassedToGive)
	{
		lastStageWhenGivenGift = Mathf.Max(lastStageWhenGivenGift, stagePassedToGive);
	}

	protected override void UpdateData()
	{
		base.UpdateData();
		for (int i = 0; i < gifts.Count; i++)
		{
			GiftDefinition giftDefinition = gifts[i];
			GiftDefinition previousGift = null;
			if (i > 0)
			{
				previousGift = gifts[i - 1];
			}
			giftDefinition.Init(this, previousGift);
		}
		dailyGifts.Init();
	}
}
