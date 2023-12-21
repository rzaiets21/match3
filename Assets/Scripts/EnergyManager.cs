using ProtoModels;
using System;
using GGMatch3;
using UnityEngine;

public class EnergyManager : BehaviourSingleton<EnergyManager>
{
	private string freeEnergyString_;

	private string limitedFreeEnergyString_;

	public bool isFullLives => ownedPlayCoins >= EnergyControlConfig.instance.totalCoin;

	public int ownedPlayCoins
	{
		get
		{
			return Mathf.FloorToInt(GetCurrentEnergyValue() / (float)EnergyControlConfig.instance.energyPointPerCoin);
		}
		set
		{
			float energy = EnergyControlConfig.instance.CoinsToEnergy(value);
			SetEnergy(energy);
		}
	}

	public float secPerCoin
	{
		get
		{
			EnergyControlConfig instance = EnergyControlConfig.instance;
			return instance.maxEnergy * (float)instance.secondsToRefreshPoint / (float)instance.totalCoin;
		}
	}

	public float secToNextCoin
	{
		get
		{
			int totalCoin = EnergyControlConfig.instance.totalCoin;
			if (ownedPlayCoins < totalCoin)
			{
				return (float)(ownedPlayCoins + 1) * secPerCoin - GetCurrentEnergyPercent() * secPerCoin * (float)totalCoin;
			}
			return 0f;
		}
	}

	private string freeEnergyString
	{
		get
		{
			if (freeEnergyString_ == null)
			{
				freeEnergyString_ = PlayerInventory.Item.FreeEnergy.ToString();
			}
			return freeEnergyString_;
		}
	}

	private string limitedFreeEnergyString
	{
		get
		{
			if (limitedFreeEnergyString_ == null)
			{
				limitedFreeEnergyString_ = PlayerInventory.Item.FreeEnergyLimited.ToString();
			}
			return limitedFreeEnergyString_;
		}
	}

	public bool isUnlimitedInfiniteEnergy => PlayerInventory.instance.IsOwned(freeEnergyString);

	public bool isFreeEnergy
	{
		get
		{
			#if UNITY_EDITOR
			return false;
			#endif
			if (!isUnlimitedInfiniteEnergy)
			{
				return isLimitedFreeEnergyActive;
			}
			return true;
		}
	}

	public bool isLimitedFreeEnergyActive
	{
		get
		{
			OwnedItemDAO itemWithName = PlayerInventory.instance.owned.GetItemWithName(limitedFreeEnergyString);
			if (itemWithName == null)
			{
				return false;
			}
			return IsActive(itemWithName);
		}
	}

	public TimeSpan limitedEnergyTimespanLeft
	{
		get
		{
			OwnedItemDAO itemWithName = PlayerInventory.instance.owned.GetItemWithName(limitedFreeEnergyString);
			return ActiveTimespanLeft(itemWithName);
		}
	}

	public float MaxEnergy
	{
		get
		{
			if (EnergyControlConfig.instance == null)
			{
				return 100f;
			}
			return EnergyControlConfig.instance.maxEnergy;
		}
	}

	public float GetCurrentEnergyPercent()
	{
		return GetCurrentEnergyValue() / EnergyControlConfig.instance.maxEnergy;
	}

	public void SpendLifeIfNotFreeEnergy()
	{
		if (!isFreeEnergy)
		{
			ownedPlayCoins--;
		}
	}

	public void AddLifeIfNotFreeEnergy()
	{
		if (!isFreeEnergy)
		{
			ownedPlayCoins++;
		}
	}

	public bool HasEnergyForOneLife(bool notify = true)
	{
		if (isFreeEnergy)
		{
			return true;
		}

		var hasEnergy = ownedPlayCoins > 0;
		if(!hasEnergy && notify)
		{
			ShowOutOfEnergyDialog();
			Debug.LogError("Has no energy!");
		}
		return hasEnergy;
	}

	private void ShowOutOfEnergyDialog()
	{
		OutOfLivesDialog @object = NavigationManager.instance.GetObject<OutOfLivesDialog>();
		LivesPriceConfig.PriceConfig priceForLevelOrDefault = ScriptableObjectSingleton<LivesPriceConfig>.instance.GetPriceForLevelOrDefault();
		@object.Show(priceForLevelOrDefault, null, null, null);
	}
	
	public void ConsumeCoin(int coinAmount)
	{
		if (!isFreeEnergy && coinAmount > 0)
		{
			GGPlayerSettings.instance.IncreaseSessionCoins(coinAmount);
			AddCoins(-coinAmount);
		}
	}

	public void AddCoins(int coinAmount)
	{
		int energyPointPerCoin = EnergyControlConfig.instance.energyPointPerCoin;
		float num = MathEx.Max(0f, GetCurrentEnergyValue() + EnergyControlConfig.instance.CoinsToEnergy(coinAmount));
		if (num > EnergyControlConfig.instance.maxEnergy)
		{
			SetEnergy(Mathf.FloorToInt(num / (float)energyPointPerCoin) * energyPointPerCoin);
		}
		else
		{
			SetEnergy(num);
		}
	}

	public void DebugChangeEnergy(float modifyEnergtPoints)
	{
		GGPlayerSettings.instance.SetEnergy(GetCurrentEnergyValue() + modifyEnergtPoints, DateTime.Now);
	}

	public void FillEnergy()
	{
		SetEnergy(EnergyControlConfig.instance.maxEnergy);
	}

	public void AddEnergy()
	{
		if (ownedPlayCoins > 3)
		{
			SetEnergy(EnergyControlConfig.instance.maxEnergy, true);
			return;
		}
		SetEnergy(GetCurrentEnergyValue() + 10f);
	}

	public void UpdateLimitedEnergy(float passedTimeSec)
	{
		if (isLimitedFreeEnergyActive)
		{
			OwnedItemDAO itemWithName = PlayerInventory.instance.owned.GetItemWithName(limitedFreeEnergyString);
			if (itemWithName != null)
			{
				long ticks = DateTime.Now.Ticks;
				itemWithName.lastCheckTime = MathEx.Max(itemWithName.purchaseTime, MathEx.Max(itemWithName.lastCheckTime, ticks));
				itemWithName.timeLeft -= TimeSpan.FromSeconds(passedTimeSec).Ticks;
				PlayerInventory.instance.owned.Save();
			}
		}
	}

	public bool IsActive(OwnedItemDAO owned)
	{
		if (owned.totalDuration == 0L)
		{
			return true;
		}
		if (owned.timeLeft <= 0)
		{
			return false;
		}
		return ActiveTimespanLeft(owned).TotalSeconds >= 0.0;
	}

	public TimeSpan ActiveTimespanLeft(OwnedItemDAO owned)
	{
		if (owned == null)
		{
			return TimeSpan.FromSeconds(-1.0);
		}
		if (owned.totalDuration <= 0)
		{
			return TimeSpan.FromDays(365.0);
		}
		if (owned.timeLeft <= 0)
		{
			return TimeSpan.FromSeconds(-1.0);
		}
		TimeSpan value = TimeSpan.FromTicks(owned.totalDuration);
		DateTime d = new DateTime(owned.purchaseTime).Add(value);
		DateTime dateTime = DateTime.Now;
		DateTime dateTime2 = new DateTime(owned.lastCheckTime);
		if (dateTime2 > dateTime)
		{
			dateTime = dateTime2;
		}
		long num = (d - dateTime).Ticks;
		if (owned.timeLeft < num)
		{
			num = owned.timeLeft;
		}
		return TimeSpan.FromTicks(num);
	}

	public TimeSpan TimeSpanTillEnergyFull()
	{
		TimeSpan timeSpan = DateTime.Now.Subtract(new DateTime(GGPlayerSettings.instance.Model.lastTimeTookEnergy));
		EnergyControlConfig instance = EnergyControlConfig.instance;
		float num = instance.maxEnergy * (float)instance.secondsToRefreshPoint;
		double num2 = timeSpan.TotalSeconds + (double)(GetCurrentEnergyValue() * (float)instance.secondsToRefreshPoint);
		return TimeSpan.FromSeconds(MathEx.Max(0.0, (double)num - num2));
	}

	public float GetCurrentEnergyValue()
	{
		if (GGPlayerSettings.instance.Model.energy >= EnergyControlConfig.instance.maxEnergy)
		{
			return GGPlayerSettings.instance.Model.energy;
		}
		TimeSpan timeSpan = DateTime.Now.Subtract(new DateTime(GGPlayerSettings.instance.Model.lastTimeTookEnergy));
		float a = Mathf.Min(EnergyControlConfig.instance.GetEnergyForTimespan(timeSpan), EnergyControlConfig.instance.maxEnergy - GGPlayerSettings.instance.Model.energy);
		a = Mathf.Max(a, 0f);
		return GGPlayerSettings.instance.Model.energy + a;
	}

	public bool HasEnoughEnergy(float energyNeededToHave)
	{
		if (isFreeEnergy)
		{
			return true;
		}
		return GetCurrentEnergyValue() >= energyNeededToHave;
	}

	public void SpendEnergy(float energyToSpend)
	{
		if (!isFreeEnergy)
		{
			float energy = MathEx.Max(0f, GetCurrentEnergyValue() - energyToSpend);
			SetEnergy(energy);
		}
	}

	public void SetEnergy(float energy)
	{
		SetEnergy(energy, true);
	}
	
	public void SetEnergy(float energy, bool save)
	{
		GGPlayerSettings.instance.SetEnergy(energy, DateTime.Now, save);
	}

	public void GainEnergy(float energyToGain)
	{
		SetEnergy(GetCurrentEnergyValue() + energyToGain);
	}
}
