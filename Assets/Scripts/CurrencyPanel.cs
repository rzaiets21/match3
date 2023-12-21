using System.Collections.Generic;
using UnityEngine;

public class CurrencyPanel : MonoBehaviour, InAppBackend.Listener
{
	[SerializeField]
	private List<CurrencyDisplay> currencyDisplays = new List<CurrencyDisplay>();
	
	public CurrencyDisplay DisplayForCurrency(CurrencyType currencyType)
	{
		for (int i = 0; i < currencyDisplays.Count; i++)
		{
			CurrencyDisplay currencyDisplay = currencyDisplays[i];
			if (currencyDisplay.currencyType == currencyType)
			{
				return currencyDisplay;
			}
		}
		return null;
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
	}

	public void OnInitialized(InAppBackend.InitializeArguments initializeArguments)
	{
	}

	public void OnPurchase(InAppBackend.PurchaseEventArguments purchaseParams)
	{
		SetLabels();
	}

	public void OnEnable()
	{
		SetLabels();
		BehaviourSingletonInit<InAppBackend>.instance.AddListener(this);
	}

	public void SetLabels()
	{
		Debug.LogError("Set labels");
		WalletManager walletManager = GGPlayerSettings.instance.walletManager;
		for (int i = 0; i < currencyDisplays.Count; i++)
		{
			CurrencyDisplay currencyDisplay = currencyDisplays[i];
			currencyDisplay.Init(walletManager.CurrencyCount(currencyDisplay.currencyType));
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
}
