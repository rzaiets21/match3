using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyPurchaseDialogBigPrefab : MonoBehaviour, InAppBackend.Listener
{
	[Serializable]
	public class NamedVisualConfig
	{
		public Image image;

		public string name;

		public bool IsMatching(OffersDB.ProductDefinition product)
		{
			return product.offer.name == name;
		}

		public void SetActive(bool flag)
		{
			GGUtil.SetActive(image.gameObject, flag);
		}
	}

	[SerializeField]
	private bool onlyForShow;

	[SerializeField]
	private List<NamedVisualConfig> visualConfigs = new List<NamedVisualConfig>();

	[SerializeField]
	private NamedVisualConfig defaulVisualConfig;

	[SerializeField]
	private List<TextMeshProUGUI> ribbonLabels;

	[SerializeField]
	private RectTransform infoContainer;

	[SerializeField]
	private RectTransform economyPrefabContainer;

	[SerializeField]
	private RectTransform powerupGroupPrefabContainer;

	[SerializeField]
	private ComponentPool economyPrefabsPool = new ComponentPool();

	[SerializeField]
	private ComponentPool powerupPrefabsPool = new ComponentPool();

	[SerializeField]
	private ComponentPool powerupGroupPrefabPool = new ComponentPool();

	[SerializeField]
	private RectTransform powerupContainer;

	[SerializeField]
	private RectTransform economyContainer;

	[SerializeField]
	private Vector2 offset;

	[SerializeField]
	private int powerupsPerRow = 2;

	public OffersDB.ProductDefinition product;

	[SerializeField]
	private CurrencyPurchaseDialog screen;

	[SerializeField]
	public TextMeshProUGUI priceLabel;

	[SerializeField]
	private CurrencyPrefabAnimation animationIn;

	[SerializeField]
	private CurrencyPrefabAnimation animationOut;

	[SerializeField]
	private CurrencyPrefabAnimation clickAnimation;

	private void OnEnable()
	{
		if (!onlyForShow)
		{
			BehaviourSingletonInit<InAppBackend>.instance.AddListener(this);
		}
	}

	private void OnDisable()
	{
		if (!onlyForShow)
		{
			BehaviourSingletonInit<InAppBackend>.instance.RemoveListener(this);
		}
	}

	public void OnInitialized(InAppBackend.InitializeArguments initializeArguments)
	{
		if (!onlyForShow && initializeArguments.isSuccess)
		{
			UpdatePrice();
		}
	}

	public void OnPurchase(InAppBackend.PurchaseEventArguments purchaseParams)
	{
	}

	private void UpdatePrice()
	{
		string productId = null;
		if (product != null)
		{
			productId = product.productID;
		}
		string text = null;
		text = BehaviourSingletonInit<InAppBackend>.instance.LocalisedPriceString(productId);
		if (Application.isEditor && !string.IsNullOrEmpty(product.mocupPrice))
		{
			text = product.mocupPrice;
		}
		GGUtil.ChangeText(priceLabel, text);
	}

	public void Init(OffersDB.ProductDefinition product)
	{
		this.product = product;
		base.transform.localScale = Vector3.one;
		for (int i = 0; i < ribbonLabels.Count; i++)
		{
			ribbonLabels[i].text = product.offer.name;
		}
		bool flag = false;
		defaulVisualConfig.SetActive(flag: false);
		for (int j = 0; j < visualConfigs.Count; j++)
		{
			NamedVisualConfig namedVisualConfig = visualConfigs[j];
			if (namedVisualConfig.IsMatching(product))
			{
				namedVisualConfig.SetActive(flag: true);
				flag = true;
			}
			else
			{
				namedVisualConfig.SetActive(flag: false);
			}
		}
		if (!flag)
		{
			defaulVisualConfig.SetActive(flag: true);
		}
		UpdatePrice();
		List<OffersDB.OfferConfig> config = product.offer.config;
		List<OffersDB.OfferConfig> list = new List<OffersDB.OfferConfig>();
		List<OffersDB.OfferConfig> list2 = new List<OffersDB.OfferConfig>();
		for (int k = 0; k < config.Count; k++)
		{
			OffersDB.OfferConfig offerConfig = config[k];
			if (offerConfig.usePrice)
			{
				list2.Add(offerConfig);
			}
			else
			{
				list.Add(offerConfig);
			}
		}
		CurrencyPurchaseDialog.PageItemInfo pageItemInfo = new CurrencyPurchaseDialog.PageItemInfo();
		CurrencyPurchaseDialog.PageItemInfo pageItemInfo2 = new CurrencyPurchaseDialog.PageItemInfo();
		pageItemInfo2.rank = 1;
		pageItemInfo.rank = 0;
		pageItemInfo2.count = Mathf.CeilToInt((float)list.Count / (float)powerupsPerRow);
		pageItemInfo.count = list2.Count;
		pageItemInfo2.space = powerupGroupPrefabContainer.rect.size;
		pageItemInfo.space = economyPrefabContainer.rect.size;
		CurrencyPurchaseDialog.PageSpace.SpacingInfo spacingInfo = new CurrencyPurchaseDialog.PageSpace.SpacingInfo();
		spacingInfo.direction = Vector2.down;
		spacingInfo.offset = offset;
		spacingInfo.size = infoContainer.rect.size;
		spacingInfo.size.y = float.PositiveInfinity;
		pageItemInfo2.spacingInfo = spacingInfo;
		pageItemInfo.spacingInfo = spacingInfo;
		List<CurrencyPurchaseDialog.PageItemInfo> list3 = new List<CurrencyPurchaseDialog.PageItemInfo>();
		list3.Add(pageItemInfo2);
		list3.Add(pageItemInfo);
		new CurrencyPurchaseDialog.PageConfig().Pack(list3);
		economyPrefabsPool.Clear();
		economyPrefabsPool.HideNotUsed();
		powerupGroupPrefabPool.Clear();
		powerupGroupPrefabPool.HideNotUsed();
		powerupPrefabsPool.Clear();
		powerupPrefabsPool.HideNotUsed();
		List<RectTransform> list4 = new List<RectTransform>();
		List<RectTransform> list5 = new List<RectTransform>();
		for (int l = 0; l < pageItemInfo2.results.Count - 1; l++)
		{
			OffersDB.OfferConfig offerConfig2 = list[l];
			CurrencyPurchaseDialogMultyPrefab currencyPurchaseDialogMultyPrefab = powerupGroupPrefabPool.Next<CurrencyPurchaseDialogMultyPrefab>(activate: true);
			List<RectTransform> buttons = CreatePowerupPrefabs(list, l * powerupsPerRow, powerupsPerRow);
			currencyPurchaseDialogMultyPrefab.Init(buttons);
			CurrencyPurchaseDialog.PageSpace.FittingResult fittingResult = pageItemInfo2.results[l];
			GGUtil.uiUtil.PositionRectInsideRect(infoContainer, currencyPurchaseDialogMultyPrefab.transform as RectTransform, fittingResult.position);
			list4.Add(currencyPurchaseDialogMultyPrefab.transform as RectTransform);
		}
		int num = Mathf.Min(pageItemInfo2.results.Count * powerupsPerRow, list.Count);
		if (pageItemInfo2.results.Count > 0)
		{
			OffersDB.OfferConfig offerConfig3 = list[pageItemInfo2.results.Count - 1];
			CurrencyPurchaseDialogMultyPrefab currencyPurchaseDialogMultyPrefab2 = powerupGroupPrefabPool.Next<CurrencyPurchaseDialogMultyPrefab>(activate: true);
			// int length = num % powerupsPerRow;
			int length = num;
			// Debug.LogError(num);
			List<RectTransform> buttons2 = CreatePowerupPrefabs(list, (pageItemInfo2.results.Count - 1) * powerupsPerRow, length);
			currencyPurchaseDialogMultyPrefab2.Init(buttons2);
			CurrencyPurchaseDialog.PageSpace.FittingResult fittingResult2 = pageItemInfo2.results[pageItemInfo2.results.Count - 1];
			GGUtil.uiUtil.PositionRectInsideRect(infoContainer, currencyPurchaseDialogMultyPrefab2.transform as RectTransform, fittingResult2.position);
			list4.Add(currencyPurchaseDialogMultyPrefab2.transform as RectTransform);
		}
		for (int m = 0; m < pageItemInfo.results.Count; m++)
		{
			OffersDB.OfferConfig config2 = list2[m];
			CurrencyPurchaseDialogEconomyPrefab currencyPurchaseDialogEconomyPrefab = economyPrefabsPool.Next<CurrencyPurchaseDialogEconomyPrefab>(activate: true);
			currencyPurchaseDialogEconomyPrefab.Init(config2);
			CurrencyPurchaseDialog.PageSpace.FittingResult fittingResult3 = pageItemInfo.results[m];
			GGUtil.uiUtil.PositionRectInsideRect(infoContainer, currencyPurchaseDialogEconomyPrefab.transform as RectTransform, fittingResult3.position);
			list5.Add(currencyPurchaseDialogEconomyPrefab.transform as RectTransform);
		}
		Pair<Vector2, Vector2> aABB = GGUtil.uiUtil.GetAABB(list5);
		RectTransform trans = economyContainer.parent as RectTransform;
		Vector2 worldDimensions = GGUtil.uiUtil.GetWorldDimensions(trans);
		float num2 = aABB.second.y - aABB.first.y;
		economyContainer.anchorMin = new Vector2(0f, 1f - num2 / worldDimensions.y);
		economyContainer.anchorMax = Vector2.one;
		economyContainer.anchoredPosition = Vector2.zero;
		economyContainer.offsetMax = Vector2.zero;
		economyContainer.offsetMin = Vector2.zero;
		powerupContainer.anchorMin = new Vector2(0f, 0f);
		powerupContainer.anchorMax = new Vector2(1f, (worldDimensions.y - num2) / worldDimensions.y);
		powerupContainer.anchoredPosition = Vector2.zero;
		powerupContainer.offsetMax = Vector2.zero;
		powerupContainer.offsetMin = Vector2.zero;
		for (int n = 0; n < list4.Count; n++)
		{
			RectTransform rectTransform = list4[n];
			Vector3 localPosition = rectTransform.transform.localPosition;
			localPosition.z = 0f;
			rectTransform.transform.localPosition = localPosition;
		}
		for (int num3 = 0; num3 < list5.Count; num3++)
		{
			RectTransform rectTransform2 = list5[num3];
			Vector3 localPosition2 = rectTransform2.transform.localPosition;
			localPosition2.z = 0f;
			rectTransform2.transform.localPosition = localPosition2;
		}
		if (onlyForShow)
		{
			GGUtil.ChangeText(priceLabel, "Purchased");
			CanvasGroup component = GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.interactable = false;
				component.blocksRaycasts = false;
			}
		}
	}

	public List<RectTransform> CreatePowerupPrefabs(List<OffersDB.OfferConfig> configs, int startIndex, int length)
	{
		List<RectTransform> list = new List<RectTransform>();
		for (int i = startIndex; i < startIndex + length; i++)
		{
			OffersDB.OfferConfig config = configs[i];
			CurrencyPurchaseDialogPowerupPrefab currencyPurchaseDialogPowerupPrefab = powerupPrefabsPool.Next<CurrencyPurchaseDialogPowerupPrefab>(activate: true);
			currencyPurchaseDialogPowerupPrefab.Init(config);
			list.Add(currencyPurchaseDialogPowerupPrefab.transform as RectTransform);
		}
		return list;
	}

	public void ButtonCallback_OnBuyButtonClicked()
	{
		clickAnimation.Play(0f, NotifyScreenForClick);
	}

	private void NotifyScreenForClick()
	{
		if (screen != null)
		{
			screen.OnButtonPressed(this);
		}
	}

	public void AnimateIn(float delay)
	{
		animationIn.Init();
		animationIn.Play(delay);
	}

	public IEnumerator DoAnimateIn(float delay)
	{
		return animationIn.DoPlay(delay);
	}

	public IEnumerator DoAnimateOut(float delay)
	{
		return animationOut.DoPlay(delay);
	}
}
