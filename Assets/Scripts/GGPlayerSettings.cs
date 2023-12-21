using ProtoModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GGPlayerSettings
{
	private PlayerModel model;

	public float overPocketNominationScale = 1.5f;

	private static GGPlayerSettings instance_;

	private WalletManager _003CwalletManager_003Ek__BackingField;

	private bool isSavingSuspended;

	public PlayerModel Model => model;

	public PlayerModel.GivenGiftsData givenGifts
	{
		get
		{
			if (Model.givenGiftsData == null)
			{
				Model.givenGiftsData = new PlayerModel.GivenGiftsData();
			}
			return Model.givenGiftsData;
		}
	}

	public bool isMusicOff
	{
		get
		{
			return Model.musicOff;
		}
		set
		{
			Model.musicOff = value;
			Save();
			ConfigBase.instance.SetAudioMixerValues(this);
		}
	}

	public bool isSoundFXOff
	{
		get
		{
			return Model.sfxOff;
		}
		set
		{
			Model.sfxOff = value;
			Save();
			ConfigBase.instance.SetAudioMixerValues(this);
		}
	}

	public static GGPlayerSettings instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = new GGPlayerSettings();
				instance_.walletManager = new WalletManager(instance_.model, instance_.Save);
				instance_.Init();
			}
			return instance_;
		}
	}

	public bool canCloudSync
	{
		get
		{
			if (!Model.canCloudSync)
			{
				return ConfigBase.instance.isFakePlayerIdOn;
			}
			return true;
		}
		set
		{
			Model.canCloudSync = value;
			Save();
		}
	}

	public WalletManager walletManager
	{
		get
		{
			return _003CwalletManager_003Ek__BackingField;
		}
		private set
		{
			_003CwalletManager_003Ek__BackingField = value;
		}
	}

	public List<PlayerModel.UsageData> usageDataList
	{
		get
		{
			if (model.usageDataList == null)
			{
				model.usageDataList = new List<PlayerModel.UsageData>();
			}
			return model.usageDataList;
		}
	}

	public bool shouldGiveExperience
	{
		get
		{
			if (model.rankExperienceLong == 0L && model.rankLevelDouble == 0.0)
			{
				return MultiplayerGamesPlayed() > 0;
			}
			return false;
		}
	}

	public void ResetEverything()
	{
		model = new PlayerModel();
		model.creationTime = DateTime.UtcNow.Ticks;
		ConfigBase instance = ConfigBase.instance;
		model.mood = 0.5f;
		model.lastTimeMoodBoost = DateTime.Now.Ticks;
		model.version = ConfigBase.instance.initialPlayerVersion;
		model.musicVolume = ConfigBase.instance.initialVolumeLevel;
		model.sfxVolume = ConfigBase.instance.initialVolumeLevel;
		model.ambientVolume = ConfigBase.instance.initialVolumeLevel;
		new DateTime(model.creationTime);
		Save();
		ReloadModel();
		walletManager.SetCurrency(CurrencyType.coins, instance.initialCoins);
		walletManager.SetCurrency(CurrencyType.diamonds, instance.initialStars);
	}

	public void IncreaseSessionCoins(int coinsAmount)
	{
		if (usageDataList.Count != 0)
		{
			usageDataList[usageDataList.Count - 1].coinsUsed += coinsAmount;
			Save();
		}
	}

	private GGPlayerSettings()
	{
	}

	private GGPlayerSettings(PlayerModel model)
	{
		isSavingSuspended = true;
		this.model = model;
		walletManager = new WalletManager(model, Save);
	}

	public GGPlayerSettings CreateFromData(CloudSyncData fileSystemData)
	{
		PlayerModel playerModel = ProtoIO.Clone(model);
		if (fileSystemData == null)
		{
			return new GGPlayerSettings(playerModel);
		}
		CloudSyncData.CloudSyncFile file = ProtoModelExtensions.GetFile(fileSystemData, "player.bytes");
		if (file == null)
		{
			return new GGPlayerSettings(playerModel);
		}
		PlayerModel playerModel2 = null;
		if (!ProtoIO.LoadFromBase64String(file.data, out playerModel2))
		{
			return new GGPlayerSettings(playerModel);
		}
		if (playerModel2 == null)
		{
			return new GGPlayerSettings(playerModel);
		}
		return new GGPlayerSettings(playerModel2);
	}

	public int MultiplayerGamesPlayed()
	{
		return model.multiplayerWins + model.multiplayerLoses;
	}

	private void Init()
	{
		ReloadModel();
		bool isPlaying = Application.isPlaying;
		if (!isSavingSuspended)
		{
			SingletonInit<FileIOChanges>.instance.OnChange(ReloadModel);
		}
	}

	public void ReloadModel()
	{
		bool flag = false;
		if (!ProtoIO.LoadFromFileLocal<ProtoSerializer, PlayerModel>("player.bytes", out model))
		{
			model = new PlayerModel();
			model.creationTime = DateTime.UtcNow.Ticks;
			flag = true;
			model.mood = 0.5f;
			model.lastTimeMoodBoost = DateTime.Now.Ticks;
			model.version = ConfigBase.instance.initialPlayerVersion;
			model.musicVolume = ConfigBase.instance.initialVolumeLevel;
			model.sfxVolume = ConfigBase.instance.initialVolumeLevel;
			model.ambientVolume = ConfigBase.instance.initialVolumeLevel;
			new DateTime(model.creationTime);
			ProtoIO.SaveToFile<ProtoSerializer, PlayerModel>("player.bytes", GGFileIO.instance, model);
		}
		if (model.version <= 7)
		{
			model.version = ConfigBase.instance.initialPlayerVersion;
			model.ambientVolume = model.sfxVolume;
			ProtoIO.SaveToFile<ProtoSerializer, PlayerModel>("player.bytes", GGFileIO.instance, model);
		}
		if (walletManager != null)
		{
			walletManager.ReloadModel(model);
		}
		if (flag)
		{
			ConfigBase instance = ConfigBase.instance;
			walletManager.SetCurrency(CurrencyType.coins, instance.initialCoins);
			walletManager.SetCurrency(CurrencyType.diamonds, instance.initialStars);
		}
		CheckShouldGiveExperience(save: false);
	}

	public void AddPurchase(InAppPurchaseDAO inAppPurchase)
	{
		if (inAppPurchase != null)
		{
			if (model.purchases == null)
			{
				model.purchases = new List<InAppPurchaseDAO>();
			}
			model.purchases.Add(inAppPurchase);
			Save();
		}
	}

	public bool IsPurchaseConsumed(string token)
	{
		if (model.purchases == null)
		{
			return false;
		}
		List<InAppPurchaseDAO> purchases = model.purchases;
		for (int i = 0; i < purchases.Count; i++)
		{
			InAppPurchaseDAO inAppPurchaseDAO = purchases[i];
			if (!string.IsNullOrEmpty(inAppPurchaseDAO.receipt) && inAppPurchaseDAO.receipt == token)
			{
				return true;
			}
		}
		return false;
	}

	public List<InAppPurchaseDAO> GetPurchases()
	{
		if (model.purchases == null)
		{
			model.purchases = new List<InAppPurchaseDAO>();
		}
		return model.purchases;
	}

	public string GetName()
	{
		if (string.IsNullOrEmpty(Model.name))
		{
			return "You";
		}
		return Model.name;
	}

	public static long GetExperienceToGive(int multiplayerWins, int multiplayerLoses)
	{
		long num = 0L;
		for (int i = 0; i < multiplayerWins; i++)
		{
			num = ((i >= 5) ? ((i >= 15) ? ((i >= 25) ? ((i >= 50) ? ((i >= 150) ? (num + 35) : (num + 30)) : (num + 25)) : (num + 20)) : (num + 15)) : (num + 10));
		}
		for (int j = 0; j < multiplayerLoses; j++)
		{
			num = ((j >= 15) ? ((j >= 25) ? ((j >= 50) ? ((j >= 150) ? (num + 5) : (num + 4)) : (num + 3)) : (num + 2)) : (num + 1));
		}
		num = MathEx.Max(num, 0L);
		UnityEngine.Debug.Log("GIVING EXPERINENCE " + num);
		return num;
	}

	public void CheckShouldGiveExperience(bool save)
	{
		if (shouldGiveExperience)
		{
			long experienceToGive = GetExperienceToGive(model.multiplayerWins, model.multiplayerLoses);
			UnityEngine.Debug.Log("GIVING EXPERINENCE " + experienceToGive);
			double rankLevelDouble = model.rankLevelDouble;
			model.rankExperienceLong = experienceToGive;
			if (save)
			{
				Save();
			}
		}
	}

	public void SetName(string name)
	{
		model.name = name;
		Save();
	}

	public void Save()
	{
		if (!isSavingSuspended)
		{
			ProtoIO.SaveToFileCS("player.bytes", model);
		}
	}

	public void SetEnergy(float energy, DateTime lastTimeTookEnergy)
	{
		SetEnergy(energy, lastTimeTookEnergy);
	}
	
	public void SetEnergy(float energy, DateTime lastTimeTookEnergy, bool save)
	{
		Model.energy = energy;
		Model.lastTimeTookEnergy = lastTimeTookEnergy.Ticks;
		if(save)
			Save();
	}
}
