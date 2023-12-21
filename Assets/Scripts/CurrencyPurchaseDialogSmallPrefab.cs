using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyPurchaseDialogSmallPrefab : MonoBehaviour, InAppBackend.Listener
{
	[SerializeField]
	private bool onlyForShow;

	[SerializeField]
	private List<CurrencyPurchaseDialogEconomyPrefab.NamedVisualConfig> visualConfigs = new List<CurrencyPurchaseDialogEconomyPrefab.NamedVisualConfig>();

	[SerializeField]
	private CurrencyPurchaseDialog screen;

	public OffersDB.ProductDefinition product;

	[SerializeField]
	public TextMeshProUGUI label;

	[SerializeField]
	private CurrencyPrefabAnimation animationIn;

	[SerializeField]
	private CurrencyPrefabAnimation animationOut;

	[SerializeField]
	private CurrencyPrefabAnimation animationClick;

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
		GGUtil.ChangeText(label, text);
	}

	public void Init(OffersDB.ProductDefinition product)
	{
		this.product = product;
		base.transform.localScale = Vector3.one;
		OffersDB.OfferConfig offerConfig = product.offer.config[0];
		for (int i = 0; i < visualConfigs.Count; i++)
		{
			CurrencyPurchaseDialogEconomyPrefab.NamedVisualConfig namedVisualConfig = visualConfigs[i];
			if (namedVisualConfig.IsMatching(offerConfig))
			{
				namedVisualConfig.SetLabel(GGFormat.FormatPrice(offerConfig.price.cost));
				namedVisualConfig.SetActive(flag: true);
			}
			else
			{
				namedVisualConfig.SetActive(flag: false);
			}
		}
		UpdatePrice();
		if (onlyForShow)
		{
			GGUtil.ChangeText(label, "Purchased");
			CanvasGroup component = GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.interactable = false;
				component.blocksRaycasts = false;
			}
		}
	}

	public void ButtonCallback_OnBuyButtonPressed()
	{
		animationClick.Play(0f, NotifyScreenButtonPress);
	}

	public void NotifyScreenButtonPress()
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

	public void AnimateOut(float delay)
	{
		animationOut.Play(delay);
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
