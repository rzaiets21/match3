using GGMatch3;
using ProtoModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InAppBackend : BehaviourSingletonInit<InAppBackend>
{
	public class PurchasesList
	{
		protected Dictionary<string, OffersDB.ProductDefinition> objectsThatCanBePurchased = new Dictionary<string, OffersDB.ProductDefinition>();

		public List<string> consumableProductIds
		{
			get
			{
				List<string> list = new List<string>();
				foreach (string key in objectsThatCanBePurchased.Keys)
				{
					OffersDB.ProductDefinition productDefinition = objectsThatCanBePurchased[key];
					if (!string.IsNullOrEmpty(key) && productDefinition.isConsumable)
					{
						list.Add(key);
					}
				}
				return list;
			}
		}

		public List<string> nonConsumableProductIds
		{
			get
			{
				List<string> list = new List<string>();
				foreach (string key in objectsThatCanBePurchased.Keys)
				{
					OffersDB.ProductDefinition productDefinition = objectsThatCanBePurchased[key];
					if (!string.IsNullOrEmpty(key) && !productDefinition.isConsumable)
					{
						list.Add(key);
					}
				}
				return list;
			}
		}

		public void Add(List<OffersDB.ProductDefinition> objects)
		{
			if (objects != null)
			{
				for (int i = 0; i < objects.Count; i++)
				{
					OffersDB.ProductDefinition inAppObject = objects[i];
					Add(inAppObject);
				}
			}
		}

		public void Add(OffersDB.ProductDefinition inAppObject)
		{
			if (inAppObject != null && !string.IsNullOrEmpty(inAppObject.productID))
			{
				objectsThatCanBePurchased.Add(inAppObject.productID, inAppObject);
			}
		}
	}

	public struct InitializeArguments
	{
		public bool isSuccess;
	}

	public struct PurchaseEventArguments
	{
		public bool isSuccess;

		public string productId;
	}

	public interface Listener
	{
		void OnInitialized(InitializeArguments initializeArguments);

		void OnPurchase(PurchaseEventArguments purchaseParams);
	}

	private GGInAppPurchase inApp;

	private List<Listener> listeners = new List<Listener>();

	public void AddListener(Listener listener)
	{
		if (!listeners.Contains(listener))
		{
			listeners.Add(listener);
		}
	}

	public void RemoveListener(Listener listener)
	{
		listeners.Remove(listener);
	}

	public void PurchaseItem(string productId)
	{
		PurchaseEventArguments purchaseEventArguments = default(PurchaseEventArguments);
		purchaseEventArguments.productId = productId;
		purchaseEventArguments.isSuccess = false;
		
		if (FindInAppForId(productId) == null)
		{
			CallListenersOnPurchase(purchaseEventArguments);
		}
		else
		{
			IAPManager.Instance.BuyProduct(productId, OnProductPurchased);
			// inApp.buy(productId);
		}
	}

	private void CallListenersOnInitialized(InitializeArguments initializeArguments)
	{
		for (int i = 0; i < listeners.Count; i++)
		{
			listeners[i].OnInitialized(initializeArguments);
		}
	}

	private void CallListenersOnPurchase(PurchaseEventArguments purchaseEventArguments)
	{
		for (int i = 0; i < listeners.Count; i++)
		{
			listeners[i].OnPurchase(purchaseEventArguments);
		}
	}

	public override void Init()
	{
		InitializePurchasing();
		IAPManager.Instance.InitializeIAPManager(null);
	}

	private bool IsInitialized()
	{
		return inApp != null;
	}

	private void InitializePurchasing()
	{
		if (!IsInitialized())
		{
			// inApp = GGInAppPurchase.instance;
			// inApp.onSetupComplete += OnSetupComplete;
			// inApp.onPurchaseComplete += OnProductPurchased;
			
			// IAPManager.PurchaseCompleted += x =>
			// {
			// 	Debug.LogError(x == null);
			// 	OnProductPurchased(new GGInAppPurchase.PurchaseResponse(x.Id, GGInAppPurchase.PurchaseResponseCode.Success));
			// };
			
			// PurchasesList purchasesList = new PurchasesList();
			// purchasesList.Add(ScriptableObjectSingleton<OffersDB>.instance.products);
			// List<string> consumableProductIds = purchasesList.consumableProductIds;
			// List<string> nonConsumableProductIds = purchasesList.nonConsumableProductIds;
			// inApp.start(consumableProductIds.ToArray(), nonConsumableProductIds.ToArray(), ScriptableObjectSingleton<OffersDB>.instance.base64EncodedPublicKey);
		}
	}

	public string LocalisedPriceString(string productId)
	{
		var price = IAPManager.Instance.GetPrice(productId);
		return price;
		if (inApp == null)
		{
			return "Buy";
		}
		return inApp.GetFormatedPrice(productId);
	}

	protected void OnSetupComplete(bool success)
	{
		if (success)
		{
			inApp.restorePurchases();
		}
		InitializeArguments initializeArguments = default(InitializeArguments);
		initializeArguments.isSuccess = success;
		CallListenersOnInitialized(initializeArguments);
	}

	public void OnProductPurchased(IAPOperationStatus status, string msg, StoreProduct storeProduct)
	{
		string productId = storeProduct.GetStoreID();
		bool success = status == IAPOperationStatus.Success;
		PurchaseEventArguments purchaseEventArguments = new PurchaseEventArguments
		{
			productId = productId
		};
		if (!success)
		{
			CallListenersOnPurchase(purchaseEventArguments);
			return;
		}
		OffersDB.ProductDefinition product = ScriptableObjectSingleton<OffersDB>.instance.GetProduct(productId);
		if (product == null)
		{
			CallListenersOnPurchase(purchaseEventArguments);
			return;
		}
		if (GGPlayerSettings.instance.IsPurchaseConsumed(storeProduct.receipt))
		{
			UnityEngine.Debug.Log("PURCHASE ALREADY CONSUMED");
			// inApp.consumePurchase(response.purchaseToken);
			CallListenersOnPurchase(purchaseEventArguments);
			return;
		}
		product.ConsumeProduct();
		// inApp.consumePurchase(response.purchaseToken);
		purchaseEventArguments.isSuccess = true;
		InAppPurchaseDAO inAppPurchaseDAO = new InAppPurchaseDAO();
		inAppPurchaseDAO.productId = productId;
		inAppPurchaseDAO.purchasedSomething = true;
		inAppPurchaseDAO.receipt = storeProduct.receipt;
		inAppPurchaseDAO.timeUtc = DateTime.UtcNow.Ticks;
		GGPlayerSettings.instance.AddPurchase(inAppPurchaseDAO);
		Analytics.IAPEvent iAPEvent = new Analytics.IAPEvent();
		iAPEvent.purchaseArguments = purchaseEventArguments;
		iAPEvent.inAppObject = product;
		iAPEvent.purchaseToken = storeProduct.receipt;
		iAPEvent.Send();
		CallListenersOnPurchase(purchaseEventArguments);
		NavigationManager instance = NavigationManager.instance;
		InAppPurchaseConfirmScreen @object = instance.GetObject<InAppPurchaseConfirmScreen>();
		if (instance.CurrentScreen.gameObject != @object.gameObject)
		{
			InAppPurchaseConfirmScreen.PurchaseArguments purchaseArguments = default(InAppPurchaseConfirmScreen.PurchaseArguments);
			purchaseArguments.isProductBought = true;
			purchaseArguments.productToBuy = product;
			@object.Show(purchaseArguments);
		}
		else
		{
			@object.OnPurchase(purchaseEventArguments);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			if(inApp != null)
				inApp.restorePurchases();
		}
	}

	public OffersDB.ProductDefinition FindInAppForId(string productId)
	{
		return ScriptableObjectSingleton<OffersDB>.instance.GetProduct(productId);
	}
}
