using GGMatch3;
using ProtoModels;
using System;

public class PlayerInventory
{
	public enum Item
	{
		EasyModeItem,
		MediumModeItem,
		HardModeItem,
		NoAds,
		Trainer,
		FreeEnergy,
		FreeEnergyLimited
	}

	private static PlayerInventory instance_;

	private OwnedItems _003Cowned_003Ek__BackingField;

	public static PlayerInventory instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = new PlayerInventory();
			}
			return instance_;
		}
	}

	public OwnedItems owned
	{
		get
		{
			return _003Cowned_003Ek__BackingField;
		}
		protected set
		{
			_003Cowned_003Ek__BackingField = value;
		}
	}

	public PlayerInventory()
	{
		owned = new OwnedItems("playerInventory.bytes");
	}

	public void BuyItem(Item item, bool canStockpile)
	{
		owned.AddToOwned(item.ToString(), canStockpile);
	}

	private string PowerupId(PowerupType powerupType)
	{
		return "pwup_" + powerupType.ToString();
	}

	private string BoosterId(GGMatch3.BoosterType boosterType)
	{
		return "boost_" + boosterType.ToString();
	}

	public long UsedCount(PowerupType powerupType)
	{
		return owned.GetOrCreateUsedItemWithName(PowerupId(powerupType)).count;
	}

	public long SetUsedCount(PowerupType powerupType, long count)
	{
		UsedItemDAO orCreateUsedItemWithName = owned.GetOrCreateUsedItemWithName(PowerupId(powerupType));
		orCreateUsedItemWithName.count = count;
		owned.Save();
		return orCreateUsedItemWithName.count;
	}

	public long OwnedCount(PowerupType powerupType)
	{
		return owned.GetOrCreateItemWithName(PowerupId(powerupType)).count;
	}

	public void Add(PowerupType powerupType, int amount)
	{
		owned.GetOrCreateItemWithName(PowerupId(powerupType)).count += amount;
		owned.Save();
	}

	public void BuyTimedItem(Item item, TimeSpan duration)
	{
		string name = item.ToString();
		OwnedItemDAO itemWithName = owned.GetItemWithName(name);
		if (itemWithName == null)
		{
			BuyItem(item, canStockpile: false);
		}
		itemWithName = owned.GetItemWithName(name);
		if (itemWithName != null)
		{
			long num2 = itemWithName.lastCheckTime = (itemWithName.purchaseTime = DateTime.Now.Ticks);
			itemWithName.totalDuration = duration.Ticks;
			itemWithName.timeLeft = itemWithName.totalDuration;
		}
		owned.Save();
	}

	public void SetOwned(PowerupType powerupType, long ownedNumber)
	{
		owned.GetOrCreateItemWithName(PowerupId(powerupType)).count = MathEx.Max(ownedNumber, 0L);
		owned.Save();
	}

	public long OwnedCount(GGMatch3.BoosterType boosterType)
	{
		return owned.GetOrCreateItemWithName(BoosterId(boosterType)).count;
	}

	public long UsedCount(GGMatch3.BoosterType boosterType)
	{
		return owned.GetOrCreateUsedItemWithName(BoosterId(boosterType)).count;
	}

	public void SetUsedCount(GGMatch3.BoosterType boosterType, long count)
	{
		owned.GetOrCreateUsedItemWithName(BoosterId(boosterType)).count = count;
		owned.Save();
	}

	public void Add(GGMatch3.BoosterType boosterType, int amount)
	{
		owned.GetOrCreateItemWithName(BoosterId(boosterType)).count += amount;
		owned.Save();
	}

	public void SetOwned(GGMatch3.BoosterType boosterType, long ownedNumber)
	{
		owned.GetOrCreateItemWithName(BoosterId(boosterType)).count = MathEx.Max(ownedNumber, 0L);
		owned.Save();
	}

	public bool IsOwned(string name)
	{
		return owned.isOwned(name);
	}
}
