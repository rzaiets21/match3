using System;
using UnityEngine;

public class GGInAppPurchaseAmazon : GGInAppPurchase
{
	private AndroidJavaObject javaInstance;

	private RuntimePlatform platform = RuntimePlatform.Android;

	protected override void Init()
	{
		return;
		if (Application.platform == platform)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.giraffegames.unityutil.GGAmazonInAppPurchase"))
			{
				javaInstance = androidJavaClass.CallStatic<AndroidJavaObject>("instance", Array.Empty<object>());
			}
		}
	}

	public void startSetup(string base64EncodedPublicKey, string csvConsumableSkuList, bool enableDebugLogging)
	{
		return;
		if (Application.platform == platform)
		{
			javaInstance.Call("startSetup", base64EncodedPublicKey, csvConsumableSkuList, enableDebugLogging);
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
	{
		return;
		if (Application.platform == platform)
		{
			javaInstance.Call("startPurchaseFlow", sku);
		}
	}

	public override void start(string[] productIds, string[] nonConsumableProductIds, string publicKey)
	{
		if (!isSetupStarted())
		{
			startSetup(publicKey, GGFormat.Implode(productIds, ","), enableDebugLogging: true);
		}
	}

	public override void buy(string productId)
	{
		startPurchaseFlow(productId);
	}

	public override void restorePurchases()
	{
		if (isSetupFinished())
		{
			queryInventory();
		}
	}

	public override string GetFormatedPrice(string productId)
	{
		return "0.0";
		if (Application.platform != platform)
		{
			return base.GetFormatedPrice(productId);
		}
		return javaInstance.Call<string>("getFormatedPrice", new object[1]
		{
			productId
		});
	}

	public override void QueryInventory()
	{
		
		base.QueryInventory();
		return;
		if (Application.platform != platform)
		{
			base.QueryInventory();
		}
		else
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
