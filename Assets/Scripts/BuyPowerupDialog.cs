using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuyPowerupDialog : MonoBehaviour
{
	[Serializable]
	private class PowerupDefinition
	{
		public PowerupType powerupType;

		public RectTransform container;

		public void Show()
		{
			GGUtil.Show(container);
		}
	}

	public struct InitArguments
	{
		public PowerupsDB.PowerupDefinition powerup;

		public Action<bool> onSuccess;

		public PowerupType powerupType => powerup.type;
	}

	[SerializeField]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private TextMeshProUGUI descriptionLabel;

	[SerializeField]
	private TextMeshProUGUI priceLabel;

	[SerializeField]
	private TextMeshProUGUI quantityLabel;

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private List<PowerupDefinition> powerups = new List<PowerupDefinition>();

	private InitArguments initArguments;

	public static BuyPowerupDialog instance => NavigationManager.instance.GetObject<BuyPowerupDialog>();

	public static void Show(InitArguments initArguments)
	{
		BuyPowerupDialog instance = BuyPowerupDialog.instance;
		instance.Init(initArguments);
		GGUtil.SetActive(instance, active: true);
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}

	public void Hide()
	{
		GGUtil.Hide(this);
	}

	public void Init(InitArguments initArguments)
	{
		this.initArguments = initArguments;
		GGUtil.SetActive(widgetsToHide, active: false);
		PowerupDefinition definition = GetDefinition(initArguments.powerupType);
		PowerupsDB.PowerupDefinition powerup = initArguments.powerup;
		definition.Show();
		GGUtil.ChangeText(titleLabel, powerup.name);
		GGUtil.ChangeText(descriptionLabel, powerup.description);
		GGUtil.ChangeText(quantityLabel, $"x{powerup.buyQuanitty}");
		GGUtil.ChangeText(priceLabel, powerup.buyPrice.cost);
	}

	private PowerupDefinition GetDefinition(PowerupType powerupType)
	{
		for (int i = 0; i < powerups.Count; i++)
		{
			PowerupDefinition powerupDefinition = powerups[i];
			if (powerupDefinition.powerupType == powerupType)
			{
				return powerupDefinition;
			}
		}
		return null;
	}

	private void OnComplete(bool success)
	{
		if (initArguments.onSuccess != null)
		{
			initArguments.onSuccess(success);
		}
	}

	public void ButtonCallback_OnPressed()
	{
		WalletManager walletManager = GGPlayerSettings.instance.walletManager;
		NavigationManager instance = NavigationManager.instance;
		PowerupsDB.PowerupDefinition powerup = initArguments.powerup;
		if (!walletManager.CanBuyItemWithPrice(powerup.buyPrice))
		{
			Hide();
			OnComplete(success: false);
			instance.GetObject<CurrencyPurchaseDialog>().Show(ScriptableObjectSingleton<OffersDB>.instance);
		}
		else
		{
			powerup.ownedCount += powerup.buyQuanitty;
			walletManager.BuyItem(powerup.buyPrice);
			OnComplete(success: true);
			Hide();
			GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		}
	}

	public void ButtonCallback_OnHide()
	{
		OnComplete(success: false);
		Hide();
		GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
	}
}
