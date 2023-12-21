using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyPurchaseDialogEconomyPrefab : MonoBehaviour
{
	[Serializable]
	public class NamedVisualConfig
	{
		public CurrencyPurchaseDialogEconomyVisualConfig visualConfig;

		public CurrencyType currency;

		public bool IsMatching(OffersDB.OfferConfig config)
		{
			return config.price.currency == currency;
		}

		public void SetLabel(string text)
		{
			visualConfig.SetLabel(text);
		}

		public void SetActive(bool flag)
		{
			GGUtil.SetActive(visualConfig, flag);
		}
	}

	[SerializeField]
	private List<NamedVisualConfig> visualConfigs = new List<NamedVisualConfig>();

	public void Init(OffersDB.OfferConfig config)
	{
		for (int i = 0; i < visualConfigs.Count; i++)
		{
			NamedVisualConfig namedVisualConfig = visualConfigs[i];
			if (namedVisualConfig.IsMatching(config))
			{
				namedVisualConfig.SetLabel(GGFormat.FormatPrice(config.price.cost));
				namedVisualConfig.SetActive(flag: true);
			}
			else
			{
				namedVisualConfig.SetActive(flag: false);
			}
		}
	}
}
