using GGMatch3;
using System;
using System.Collections.Generic;

public class OffersDB : ScriptableObjectSingleton<OffersDB>
{
	[Serializable]
	public class OfferConfig
	{
		public bool useBoosterType;

		public PowerupType powerupType;

		public int count;

		public bool usePrice;

		public SingleCurrencyPrice price = new SingleCurrencyPrice();
	}

	[Serializable]
	public class OfferDefinition
	{
		public string name;

		public List<OfferConfig> config = new List<OfferConfig>();

		public bool isNamedOffer => !string.IsNullOrEmpty(name);
	}

	[Serializable]
	public class ProductDefinition
	{
		public enum ProductType
		{
			Consumable,
			Permanent
		}

		public string editorName;

		public OfferDefinition offer = new OfferDefinition();

		public string productID;

		public ProductType productType;

		public string mocupPrice;

		public bool active;

		public bool isConsumable => productType == ProductType.Consumable;

		public void ConsumeProduct()
		{
			List<OfferConfig> config = offer.config;
			for (int i = 0; i < config.Count; i++)
			{
				OfferConfig offerConfig = config[i];
				if (offerConfig.usePrice)
				{
					GGPlayerSettings.instance.walletManager.AddCurrency(offerConfig.price.currency, offerConfig.price.cost);
				}
				else if (offerConfig.useBoosterType)
				{
					PlayerInventory.instance.Add(offerConfig.powerupType, offerConfig.count);
				}
			}
		}
	}

	public string base64EncodedPublicKey;

	public List<ProductDefinition> products = new List<ProductDefinition>();

	public ProductDefinition GetProduct(string productId)
	{
		for (int i = 0; i < products.Count; i++)
		{
			ProductDefinition productDefinition = products[i];
			if (productDefinition.productID == productId)
			{
				return productDefinition;
			}
		}
		return null;
	}
}
