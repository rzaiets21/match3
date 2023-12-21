using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GGInAppPurchaseAndroid : GGInAppPurchase
{
	public class PurchaseData
	{
		public string originalJson;

		public string signature;

		public string productId;

		public string purchaseToken;

		public bool isValid;
	}

	private sealed class _003CDoVerifyInAppData_003Ed__20 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PurchaseData data;

		public OnValidateDelegate onComplete;

		private WWW _003Cw_003E5__2;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDoVerifyInAppData_003Ed__20(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				string appName = ConfigBase.instance.appName;
				string originalJson = data.originalJson;
				string signature = data.signature;
				if (string.IsNullOrEmpty(originalJson) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(appName))
				{
					if (onComplete != null)
					{
						onComplete(data.productId, data.isValid, data);
					}
					return false;
				}
				WWWForm wWWForm = new WWWForm();
				wWWForm.AddField("app", appName);
				wWWForm.AddField("data", originalJson);
				wWWForm.AddField("signature", signature);
				_003Cw_003E5__2 = new WWW(GGServerConstants.instance.verifyInAppPurchasesUrl, wWWForm);
				_003C_003E2__current = _003Cw_003E5__2;
				_003C_003E1__state = 1;
				return true;
			}
			case 1:
				_003C_003E1__state = -1;
				if (!string.IsNullOrEmpty(_003Cw_003E5__2.error))
				{
					UnityEngine.Debug.Log("Error with request " + _003Cw_003E5__2.error);
				}
				else if (_003Cw_003E5__2.text == "true")
				{
					data.isValid = true;
				}
				if (onComplete != null)
				{
					onComplete(data.productId, data.isValid, data);
				}
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private AndroidJavaObject javaInstance;

	private RuntimePlatform platform = RuntimePlatform.Android;

	protected override void Init()
	{
		return;
		if (Application.platform == platform)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.giraffegames.unityutil.GGInAppPurchase"))
			{
				javaInstance = androidJavaClass.CallStatic<AndroidJavaObject>("instance", Array.Empty<object>());
			}
		}
	}

	public void startSetup(string base64EncodedPublicKey, string csvConsumableSkuList, string csvNonConsumableSkuList, bool enableDebugLogging)
	{
		return;
		if (Application.platform == platform)
		{
			string text = "";
			string text2 = "";
			if (ConfigBase.instance.verifyPlayInApp)
			{
				GGDebug.Log("verify");
				text = GGServerConstants.instance.appName;
				text2 = GGServerConstants.instance.verifyInAppPurchasesUrl;
			}
			javaInstance.Call("startSetup2", base64EncodedPublicKey, text, text2, csvConsumableSkuList, csvNonConsumableSkuList, enableDebugLogging);
		}
	}

	public override void updateProductList(string csvConsumableSkuList, string csvNonConsumableSkuList)
	{
		return;
		if (Application.platform != platform)
		{
			UnityEngine.Debug.Log("CONSUMABLE: " + csvConsumableSkuList);
			UnityEngine.Debug.Log("NON CONSUMABLE: " + csvNonConsumableSkuList);
		}
		else
		{
			UnityEngine.Debug.Log("CONSUMABLE: " + csvConsumableSkuList);
			UnityEngine.Debug.Log("NON CONSUMABLE: " + csvNonConsumableSkuList);
			javaInstance.Call("updateProductList", csvConsumableSkuList, csvNonConsumableSkuList);
		}
	}

	public bool isSetupFinished()
	{
		return false;
		if (Application.platform != platform)
		{
			return false;
		}
		return javaInstance.Call<bool>("isSetupFinished", Array.Empty<object>());
	}

	public bool isSetupStarted()
	{
		return false; 
		if (Application.platform != platform)
		{
			return false;
		}
		return javaInstance.Call<bool>("isSetupStarted", Array.Empty<object>());
	}

	public void queryInventory()
	{
		return;
		if (Application.platform == platform)
		{
			javaInstance.Call("queryInventory");
		}
	}

	public void startPurchaseFlow(string sku)
	{return;
	
		if (Application.platform == platform)
		{
			javaInstance.Call("startPurchaseFlow", sku);
		}
	}

	public override void start(string[] productIds, string[] nonConsumableProductIds, string publicKey)
	{
		if (!isSetupStarted())
		{
			startSetup(publicKey, GGFormat.Implode(productIds, ","), GGFormat.Implode(nonConsumableProductIds, ","), enableDebugLogging: true);
		}
	}

	public override void consumePurchase(string purchaseToken)
	{
		return;
		if (Application.platform != platform)
		{
			UnityEngine.Debug.Log("CONSUME PURCHASE " + purchaseToken);
		}
		else
		{
			javaInstance.Call("consumePurchaseWithToken", purchaseToken);
		}
	}

	public override void buy(string productId)
	{
		if (!isSetupFinished() && !Application.isEditor)
		{
			purchaseCanceled(productId);
		}
		else
		{
			startPurchaseFlow(productId);
		}
	}

	public override void restorePurchases()
	{
		return;
		if (isSetupFinished())
		{
			queryInventory();
		}
	}

	public override string GetFormatedPrice(string productId)
	{
		return "0.2";
		if (Application.platform != platform)
		{
			return base.GetFormatedPrice(productId);
		}
		return javaInstance.Call<string>("getFormatedPrice", new object[1]
		{
			productId
		});
	}

	public override string GetPriceCurrencyCode(string productId)
	{
		return "vnd";
		if (Application.platform != platform)
		{
			return base.GetPriceCurrencyCode(productId);
		}
		return javaInstance.Call<string>("getPriceCurrencyCode", new object[1]
		{
			productId
		});
	}

	public override string GetPriceAmountMicros(string productId)
	{
		return "000";
		if (Application.platform != platform)
		{
			return base.GetPriceAmountMicros(productId);
		}
		return javaInstance.Call<string>("getPriceAmountMicros", new object[1]
		{
			productId
		});
	}

	public string getPurchaseOriginalJSON(string productId)
	{
		return "{}";
		if (Application.platform != platform)
		{
			return "";
		}
		return javaInstance.Call<string>("getPurchaseOriginalJSON", new object[1]
		{
			productId
		});
	}

	public string getPurchaseSignature(string productId)
	{
		return "";
		if (Application.platform != platform)
		{
			return "";
		}
		return javaInstance.Call<string>("getPurchaseSignature", new object[1]
		{
			productId
		});
	}

	public override void ValidatePurchase(string productId, OnValidateDelegate onComplete)
	{
		return;
		PurchaseData purchaseData = new PurchaseData();
		purchaseData.originalJson = getPurchaseOriginalJSON(productId);
		purchaseData.signature = getPurchaseSignature(productId);
		purchaseData.productId = productId;
		try
		{
			Hashtable hashtable = NGUIJson.jsonDecode(purchaseData.originalJson) as Hashtable;
			if (hashtable != null && hashtable.ContainsKey("purchaseToken"))
			{
				purchaseData.purchaseToken = (hashtable["purchaseToken"] as string);
			}
		}
		catch
		{
		}
		StartCoroutine(DoVerifyInAppData(purchaseData, onComplete));
	}

	protected IEnumerator DoVerifyInAppData(PurchaseData data, OnValidateDelegate onComplete)
	{
		return new _003CDoVerifyInAppData_003Ed__20(0)
		{
			data = data,
			onComplete = onComplete
		};
	}

	public override void QueryInventory()
	{
		if (Application.platform != platform)
		{
			base.QueryInventory();
		}
		else if (isSetupFinished())
		{
			queryInventory();
		}
	}

	public override bool IsInventoryAvailable()
	{
		return false;
		if (Application.platform != platform)
		{
			return base.IsInventoryAvailable();
		}
		return javaInstance.Call<bool>("isInventoryAvailable", Array.Empty<object>());
	}
}
