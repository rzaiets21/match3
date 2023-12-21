using UnityEngine;

public class CurrencyPurchaseCurrencyPrefab : MonoBehaviour
{
	[SerializeField]
	private CurrencyPurchaseCurrencyVisualConfig visualConfig;

	public void Init(OffersDB.OfferConfig offer)
	{
		SingleCurrencyPrice price = offer.price;
		visualConfig.SetLabel(GGFormat.FormatPrice(price.cost));
	}
}
