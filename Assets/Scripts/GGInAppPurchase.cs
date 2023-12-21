using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GGInAppPurchase : MonoBehaviour
{
	public enum PurchaseResponseCode
	{
		AlreadyOwned,
		CantVerifySignature,
		SignatureNotAccepted,
		ConsumeFailed,
		UnknownError,
		Failed,
		Canceled,
		Success
	}

	public class PurchaseResponse
	{
		public string productId;

		public string purchaseToken;

		public PurchaseResponseCode responseCode;

		public bool success => responseCode == PurchaseResponseCode.Success;

		public PurchaseResponse(string productId, PurchaseResponseCode responseCode)
		{
			this.productId = productId;
			this.responseCode = responseCode;
		}
	}

	public delegate void PurchaseCompleteDelegate(PurchaseResponse response);

	public delegate void SetupCompleteDelegate(bool success);

	public delegate void OnValidateDelegate(string productId, bool isValid, object data);

	[Serializable]
	public class PurchaseJSON
	{
		public string sku;

		public string purchaseToken;
	}

	private static GGInAppPurchase instance_;

	private static GameObject instanceGameObject_;

	public static GGInAppPurchase instance
	{
		get
		{
			if (instance_ == null)
			{
				instanceGameObject_ = GameObject.Find("GGInAppPurchase");
				if (instanceGameObject_ == null)
				{
					instanceGameObject_ = new GameObject("GGInAppPurchase");
				}
				instance_ = instanceGameObject_.GetComponent<GGInAppPurchase>();
				if (instance_ == null)
				{
					if (ConfigBase.instance.inAppProvider == ConfigBase.InAppProvider.AmazonInApp)
					{
						instance_ = instanceGameObject_.AddComponent<GGInAppPurchaseAmazon>();
					}
					else
					{
						instance_ = instanceGameObject_.AddComponent<GGInAppPurchaseAndroid>();
					}
					instance_.Init();
				}
				UnityEngine.Object.DontDestroyOnLoad(instanceGameObject_);
			}
			return instance_;
		}
	}

	public event PurchaseCompleteDelegate onPurchaseComplete;

	public event SetupCompleteDelegate onSetupComplete;

	public event SetupCompleteDelegate onQueryInventoryComplete;

	public virtual void ValidatePurchase(string productId, OnValidateDelegate onComplete)
	{
		onComplete?.Invoke(productId, isValid: true, null);
	}

	public virtual void updateProductList(List<string> consumableSkuList, List<string> nonConsumableSkuList)
	{
		string csvConsumableSkuList = GGFormat.Implode(consumableSkuList, ",");
		string csvNonConsumableSkuList = GGFormat.Implode(nonConsumableSkuList, ",");
		updateProductList(csvConsumableSkuList, csvNonConsumableSkuList);
	}

	public virtual void updateProductList(string csvConsumableSkuList, string csvNonConsumableSkuList)
	{
	}

	public void setupFinished(string success)
	{
		bool success2 = success.ToLower().Equals("success");
		if (this.onSetupComplete != null)
		{
			this.onSetupComplete(success2);
		}
	}

	public void purchaseCompleteJSON(string json)
	{
		PurchaseJSON purchaseJSON = JsonUtility.FromJson<PurchaseJSON>(json);
		if (purchaseJSON == null)
		{
			UnityEngine.Debug.Log("NOT KNOWN PURCHASE");
			return;
		}
		PurchaseResponse purchaseResponse = new PurchaseResponse(purchaseJSON.sku, PurchaseResponseCode.Success);
		purchaseResponse.purchaseToken = purchaseJSON.purchaseToken;
		if (this.onPurchaseComplete != null)
		{
			this.onPurchaseComplete(purchaseResponse);
		}
	}

	public void purchaseComplete(string productId)
	{
		if (this.onPurchaseComplete != null)
		{
			this.onPurchaseComplete(new PurchaseResponse(productId, PurchaseResponseCode.Success));
		}
	}

	public void queryInventoryFinished(string success)
	{
		bool success2 = success.ToLower().Equals("success");
		UnityEngine.Debug.Log("Query inventory " + success2.ToString());
		if (this.onQueryInventoryComplete != null)
		{
			this.onQueryInventoryComplete(success2);
		}
	}

	public void purchaseAlreadyOwned(string productId)
	{
		UnityEngine.Debug.Log("purchaseAlreadyOwned");
		if (this.onPurchaseComplete != null)
		{
			this.onPurchaseComplete(new PurchaseResponse(productId, PurchaseResponseCode.AlreadyOwned));
		}
	}

	public void purchaseCantVerifySignature(string productId)
	{
		UnityEngine.Debug.Log("purchaseCantVerifySignature");
		if (this.onPurchaseComplete != null)
		{
			this.onPurchaseComplete(new PurchaseResponse(productId, PurchaseResponseCode.CantVerifySignature));
		}
	}

	public void purchaseSignatureNotAccepted(string productId)
	{
		UnityEngine.Debug.Log("purchaseSignatureNotAccepted");
		if (this.onPurchaseComplete != null)
		{
			this.onPurchaseComplete(new PurchaseResponse(productId, PurchaseResponseCode.SignatureNotAccepted));
		}
	}

	public void purchaseConsumeFailed(string token)
	{
		UnityEngine.Debug.Log("purchaseConsumeFailed " + token);
	}

	public void purchaseConsumeSuccess(string token)
	{
		UnityEngine.Debug.Log("purchaseConsumeSuccess " + token);
	}

	public void purchaseUnknownError(string productId)
	{
		UnityEngine.Debug.Log("purchaseUnknownError");
		if (this.onPurchaseComplete != null)
		{
			this.onPurchaseComplete(new PurchaseResponse(productId, PurchaseResponseCode.UnknownError));
		}
	}

	public void purchaseFailed(string productId)
	{
		UnityEngine.Debug.Log("purchaseFailed");
		if (this.onPurchaseComplete != null)
		{
			this.onPurchaseComplete(new PurchaseResponse(productId, PurchaseResponseCode.Failed));
		}
	}

	public void purchaseCanceled(string productId)
	{
		UnityEngine.Debug.Log("purchaseCanceled");
		if (this.onPurchaseComplete != null)
		{
			this.onPurchaseComplete(new PurchaseResponse(productId, PurchaseResponseCode.Canceled));
		}
	}

	protected virtual void Init()
	{
	}

	public virtual void start(string[] productIds, string[] nonConsumableProductIds, string publicKey)
	{
	}

	public virtual void buy(string productId)
	{
	}

	public virtual void consumePurchase(string purchaseToken)
	{
	}

	public virtual void restorePurchases()
	{
	}

	public virtual string GetFormatedPrice(string productId)
	{
		return "- Buy -";
	}

	public virtual void QueryInventory()
	{
	}

	public virtual bool IsInventoryAvailable()
	{
		return false;
	}

	public virtual string GetPriceCurrencyCode(string productId)
	{
		return "";
	}

	public virtual string GetPriceAmountMicros(string productId)
	{
		return "";
	}
}
