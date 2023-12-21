using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ITSoft;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyPurchaseDialog : UILayer
{
	public enum PageType
	{
		FirstPage,
		AllPage
	}
	
	public void ShowRewardedVideo()
	{
		AdsManager.ShowRewarded();
	}

	[Serializable]
	public class LimitTypePageConfig
	{
		[SerializeField]
		private int maxBigElements;

		[SerializeField]
		private int maxSmallElements;

		private PageSpace pageSpace = new PageSpace();

		public void Pack(List<PageItemInfo> infos)
		{
			pageSpace.Clear();
			infos.Sort(PageConfig.Sort_Rank);
			PageItemInfo pageItemInfo = infos[0];
			PageItemInfo pageItemInfo2 = infos[1];
			pageSpace.Init(pageItemInfo.spacingInfo);
			for (int i = 0; i < pageItemInfo.count; i++)
			{
				if (pageItemInfo.results.Count >= maxBigElements)
				{
					break;
				}
				PageSpace.FittingResult fittingResult = pageSpace.TryToFit(pageItemInfo.space);
				if (!fittingResult.succeeded)
				{
					break;
				}
				pageItemInfo.results.Add(fittingResult);
			}
			pageSpace.occupiedSpace += Vector2.Scale(pageItemInfo.spacingInfo.groupOffset, pageItemInfo.spacingInfo.direction).magnitude;
			pageSpace.Init(pageItemInfo2.spacingInfo);
			for (int j = 0; j < pageItemInfo2.count; j++)
			{
				if (pageItemInfo2.results.Count >= maxSmallElements)
				{
					break;
				}
				PageSpace.FittingResult fittingResult2 = pageSpace.TryToFit(pageItemInfo2.space);
				if (fittingResult2.succeeded)
				{
					pageItemInfo2.results.Add(fittingResult2);
					continue;
				}
				break;
			}
		}
	}

	[Serializable]
	public class PageConfig
	{
		private PageSpace pageSpace = new PageSpace();

		public virtual void Pack(List<PageItemInfo> infos)
		{
			pageSpace.Clear();
			infos.Sort(Sort_Rank);
			for (int i = 0; i < infos.Count; i++)
			{
				PageItemInfo pageItemInfo = infos[i];
				PageSpace.SpacingInfo spacingInfo = pageItemInfo.spacingInfo;
				pageSpace.Init(spacingInfo);
				for (int j = 0; j < pageItemInfo.count; j++)
				{
					PageSpace.FittingResult fittingResult = pageSpace.TryToFit(pageItemInfo.space);
					if (!fittingResult.succeeded)
					{
						break;
					}
					pageItemInfo.results.Add(fittingResult);
				}
				pageSpace.occupiedSpace += Vector2.Scale(spacingInfo.groupOffset, spacingInfo.direction).magnitude;
			}
		}

		public static int Sort_Rank(PageItemInfo a, PageItemInfo b)
		{
			return a.rank.CompareTo(b.rank);
		}
	}

	public class PageItemInfo
	{
		public Vector2 space;

		public int count;

		public List<PageSpace.FittingResult> results = new List<PageSpace.FittingResult>();

		public int rank;

		public PageSpace.SpacingInfo spacingInfo;
	}

	public class PageSpace
	{
		public class FittingResult
		{
			public bool succeeded;

			public Vector2 position;
		}

		public class SpacingInfo
		{
			public Vector2 size;

			public Vector2 offset;

			public Vector2 groupOffset;

			public Vector2 direction;

			public float totalSize => Vector2.Scale(size, direction).magnitude;
		}

		public float occupiedSpace;

		private SpacingInfo spaceInfo;

		public float freeSpace => spaceInfo.totalSize - occupiedSpace;

		public void Clear()
		{
			occupiedSpace = 0f;
		}

		public void Init(SpacingInfo info)
		{
			spaceInfo = info;
		}

		public FittingResult TryToFit(Vector2 space)
		{
			FittingResult fittingResult = new FittingResult();
			float num = Vector2.Scale(spaceInfo.direction, space).magnitude + Vector2.Scale(spaceInfo.direction, spaceInfo.offset).magnitude;
			if (num > freeSpace)
			{
				return fittingResult;
			}
			fittingResult.succeeded = true;
			fittingResult.position = spaceInfo.direction * occupiedSpace + spaceInfo.direction * num * 0.5f;
			occupiedSpace += num;
			return fittingResult;
		}
	}

	private sealed class _003CDoAnimateOut_003Ed__35 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public CurrencyPurchaseDialog _003C_003E4__this;

		public Action onEnd;

		private EnumeratorsList _003CenumList_003E5__2;

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
		public _003CDoAnimateOut_003Ed__35(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			CurrencyPurchaseDialog currencyPurchaseDialog = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				float num2 = 0f;
				_003CenumList_003E5__2 = new EnumeratorsList();
				_003CenumList_003E5__2.Add(currencyPurchaseDialog.rewardButton.DoAnimateOut(num2));
				_003CenumList_003E5__2.Add(currencyPurchaseDialog.nextButton.DoAnimateOut(num2));
				for (int i = 0; i < currencyPurchaseDialog.smallItems.Count; i++)
				{
					if ((i + 1) % currencyPurchaseDialog.smallItemsPerGroup == 0)
					{
						num2 += currencyPurchaseDialog.animationDelayPerColumn;
					}
					CurrencyPurchaseDialogSmallPrefab currencyPurchaseDialogSmallPrefab = currencyPurchaseDialog.smallItems[currencyPurchaseDialog.smallItems.Count - 1 - i];
					_003CenumList_003E5__2.Add(currencyPurchaseDialogSmallPrefab.DoAnimateOut(num2));
				}
				for (int j = 0; j < currencyPurchaseDialog.bigItems.Count; j++)
				{
					num2 += currencyPurchaseDialog.animationDelayPerColumn;
					CurrencyPurchaseDialogBigPrefab currencyPurchaseDialogBigPrefab = currencyPurchaseDialog.bigItems[currencyPurchaseDialog.bigItems.Count - 1 - j];
					_003CenumList_003E5__2.Add(currencyPurchaseDialogBigPrefab.DoAnimateOut(num2));
				}
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003CenumList_003E5__2.Update())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			currencyPurchaseDialog.animateOutEnum = null;
			if (onEnd != null)
			{
				onEnd();
			}
			return false;
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

	private Action onHide;

	[SerializeField]
	private LimitTypePageConfig firstPageConfig = new LimitTypePageConfig();

	private PageConfig defaultPageConfig = new PageConfig();

	[SerializeField]
	private ComponentPool namedOffersPool = new ComponentPool();

	[SerializeField]
	private ComponentPool unamedOffersPool = new ComponentPool();

	[SerializeField]
	private ComponentPool unamedGroupContainerPool = new ComponentPool();

	[SerializeField]
	private RectTransform offersContainer;

	[SerializeField]
	private RectTransform namedOffersContainer;

	[SerializeField]
	private RectTransform unamedOffersContainer;

	[SerializeField]
	private RectTransform nextButtonContainer;

	[SerializeField]
	private RectTransform rewardButtonContainer;

	[SerializeField]
	private Vector2 spacingBigPrefabs;

	[SerializeField]
	private Vector2 spacingSmallPrefabs;

	[SerializeField]
	private Vector2 groupsSpacing;

	[SerializeField]
	private int smallItemsPerGroup = 2;

	[SerializeField]
	private TextMeshProUGUI coinsCurrencyLabel;

	[SerializeField]
	private float totalScrollingPaddingHorizontalWorldSpace;

	[SerializeField]
	private ScrollRect scrollRect;

	private OffersDB offers;

	[SerializeField]
	private CurrencyPurchaseDialogNextButton nextButton;
	
	[SerializeField]
	private CurrencyPurchaseDialogSmallPrefab rewardButton;

	[SerializeField]
	private float animationDelayPerColumn = 0.1f;

	private List<CurrencyPurchaseDialogMultyPrefab> groupSmallItems = new List<CurrencyPurchaseDialogMultyPrefab>();

	private List<CurrencyPurchaseDialogSmallPrefab> smallItems = new List<CurrencyPurchaseDialogSmallPrefab>();

	private List<CurrencyPurchaseDialogBigPrefab> bigItems = new List<CurrencyPurchaseDialogBigPrefab>();

	private PageType type;

	private IEnumerator animateOutEnum;

	public void Show(OffersDB offers, Action onHide = null)
	{
		this.onHide = onHide;
		this.offers = offers;
		type = PageType.FirstPage;
		NavigationManager.instance.Push(base.gameObject);
	}

	public void Init()
	{
		Init(offers, onHide, type);
	}

	public void Init(OffersDB offers, Action onHide, PageType type)
	{
		this.onHide = onHide;
		this.offers = offers;
		this.type = type;
		offersContainer.anchorMin = Vector2.zero;
		offersContainer.anchorMax = Vector2.one;
		offersContainer.anchoredPosition = Vector2.zero;
		offersContainer.offsetMin = Vector2.zero;
		offersContainer.offsetMax = Vector2.zero;
		unamedOffersPool.parent.localScale = Vector3.one;
		coinsCurrencyLabel.text = GGFormat.FormatPrice(GGPlayerSettings.instance.walletManager.CurrencyCount(CurrencyType.coins));
		List<OffersDB.ProductDefinition> products = offers.products;
		List<OffersDB.ProductDefinition> list = new List<OffersDB.ProductDefinition>();
		List<OffersDB.ProductDefinition> list2 = new List<OffersDB.ProductDefinition>();
		for (int i = 0; i < products.Count; i++)
		{
			OffersDB.ProductDefinition productDefinition = products[i];
			if (productDefinition.active)
			{
				if (productDefinition.offer.isNamedOffer)
				{
					list.Add(productDefinition);
				}
				else
				{
					list2.Add(productDefinition);
				}
			}
		}
		Vector2 size = offersContainer.rect.size;
		PageItemInfo pageItemInfo = new PageItemInfo();
		PageItemInfo pageItemInfo2 = new PageItemInfo();
		pageItemInfo.count = list.Count;
		pageItemInfo.space = namedOffersContainer.rect.size;
		pageItemInfo.rank = 0;
		PageSpace.SpacingInfo spacingInfo = new PageSpace.SpacingInfo();
		spacingInfo.offset = spacingBigPrefabs;
		spacingInfo.size = size;
		spacingInfo.direction = Vector2.right;
		spacingInfo.size.x = float.PositiveInfinity;
		spacingInfo.groupOffset = groupsSpacing;
		pageItemInfo.spacingInfo = spacingInfo;
		if (type == PageType.FirstPage)
		{
			pageItemInfo2.count = list2.Count / smallItemsPerGroup + 1;
		}
		else
		{
			pageItemInfo2.count = Mathf.CeilToInt((float)list2.Count / (float)smallItemsPerGroup);
		}
		pageItemInfo2.space = unamedOffersContainer.rect.size;
		pageItemInfo2.rank = 1;
		PageSpace.SpacingInfo spacingInfo2 = new PageSpace.SpacingInfo();
		spacingInfo2.offset = spacingSmallPrefabs;
		spacingInfo2.size = size;
		spacingInfo2.direction = Vector2.right;
		spacingInfo2.size.x = float.PositiveInfinity;
		spacingInfo2.groupOffset = groupsSpacing;
		pageItemInfo2.spacingInfo = spacingInfo2;
		List<PageItemInfo> list3 = new List<PageItemInfo>();
		list3.Add(pageItemInfo);
		list3.Add(pageItemInfo2);
		if (type == PageType.FirstPage)
		{
			firstPageConfig.Pack(list3);
		}
		else
		{
			defaultPageConfig.Pack(list3);
		}
		namedOffersPool.Clear();
		namedOffersPool.HideNotUsed();
		bigItems.Clear();
		for (int j = 0; j < pageItemInfo.results.Count; j++)
		{
			Vector2 position = pageItemInfo.results[j].position;
			CurrencyPurchaseDialogBigPrefab currencyPurchaseDialogBigPrefab = namedOffersPool.Next<CurrencyPurchaseDialogBigPrefab>(activate: true);
			OffersDB.ProductDefinition product = list[j];
			currencyPurchaseDialogBigPrefab.Init(product);
			GGUtil.uiUtil.GetWorldDimensions(offersContainer);
			GGUtil.uiUtil.PositionRectInsideRect(offersContainer, currencyPurchaseDialogBigPrefab.transform as RectTransform, position);
			bigItems.Add(currencyPurchaseDialogBigPrefab);
		}
		unamedOffersPool.Clear();
		unamedOffersPool.HideNotUsed();
		unamedGroupContainerPool.Clear();
		unamedGroupContainerPool.HideNotUsed();
		int num = Mathf.Min(smallItemsPerGroup * pageItemInfo2.results.Count, list2.Count + 1);
		groupSmallItems.Clear();
		smallItems.Clear();
		for (int k = 0; k < pageItemInfo2.results.Count - 1; k++)
		{
			List<RectTransform> buttons = CreateSmallPrefabs(list2, k * smallItemsPerGroup, smallItemsPerGroup);
			CurrencyPurchaseDialogMultyPrefab currencyPurchaseDialogMultyPrefab = unamedGroupContainerPool.Next<CurrencyPurchaseDialogMultyPrefab>(activate: true);
			currencyPurchaseDialogMultyPrefab.Init(buttons);
			PageSpace.FittingResult fittingResult = pageItemInfo2.results[k];
			GGUtil.uiUtil.PositionRectInsideRect(offersContainer, currencyPurchaseDialogMultyPrefab.transform as RectTransform, fittingResult.position);
			groupSmallItems.Add(currencyPurchaseDialogMultyPrefab);
		}
		if (pageItemInfo2.results.Count > 0)
		{
			CurrencyPurchaseDialogMultyPrefab currencyPurchaseDialogMultyPrefab2 = unamedGroupContainerPool.Next<CurrencyPurchaseDialogMultyPrefab>(activate: true);
			if (type == PageType.FirstPage)
			{
				int num2 = smallItemsPerGroup - num % smallItemsPerGroup;
				List<RectTransform> list4 = CreateSmallPrefabs(list2, (pageItemInfo2.results.Count - 1) * smallItemsPerGroup, num2 - 2);
				list4.Add(rewardButtonContainer);
				list4.Add(nextButtonContainer);
				currencyPurchaseDialogMultyPrefab2.Init(list4);
			}
			else
			{
				int length = smallItemsPerGroup - num % smallItemsPerGroup;
				List<RectTransform> buttons2 = CreateSmallPrefabs(list2, (pageItemInfo2.results.Count - 1) * smallItemsPerGroup, length);
				currencyPurchaseDialogMultyPrefab2.Init(buttons2);
			}
			PageSpace.FittingResult fittingResult2 = pageItemInfo2.results[pageItemInfo2.results.Count - 1];
			GGUtil.uiUtil.PositionRectInsideRect(offersContainer, currencyPurchaseDialogMultyPrefab2.transform as RectTransform, fittingResult2.position);
			groupSmallItems.Add(currencyPurchaseDialogMultyPrefab2);
		}
		if (type == PageType.FirstPage)
		{
			GGUtil.SetActive(nextButtonContainer, active: true);
			GGUtil.SetActive(rewardButtonContainer, active: true);
		}
		else
		{
			GGUtil.SetActive(nextButtonContainer, active: false);
			GGUtil.SetActive(rewardButtonContainer, active: false);
		}
		List<RectTransform> list5 = new List<RectTransform>();
		for (int l = 0; l < bigItems.Count; l++)
		{
			RectTransform item = bigItems[l].transform as RectTransform;
			list5.Add(item);
		}
		for (int m = 0; m < smallItems.Count; m++)
		{
			RectTransform item2 = smallItems[m].transform as RectTransform;
			list5.Add(item2);
		}
		for (int n = 0; n < groupSmallItems.Count; n++)
		{
			RectTransform item3 = groupSmallItems[n].transform as RectTransform;
			list5.Add(item3);
		}
		Pair<Vector2, Vector2> aABB = GGUtil.uiUtil.GetAABB(list5);
		Vector3 b = aABB.first * 0.5f + aABB.second * 0.5f;
		Vector3 vector = offersContainer.transform.position - b;
		for (int num3 = 0; num3 < bigItems.Count; num3++)
		{
			bigItems[num3].transform.position += vector;
		}
		for (int num4 = 0; num4 < groupSmallItems.Count; num4++)
		{
			groupSmallItems[num4].transform.position += vector;
		}
		aABB.first.x += vector.x;
		aABB.first.y += vector.y;
		aABB.second.x += vector.x;
		aABB.second.y += vector.y;
		RectTransform trans = offersContainer.parent as RectTransform;
		Vector2 worldDimensions = GGUtil.uiUtil.GetWorldDimensions(trans);
		float num5 = (aABB.second.x - aABB.first.x + totalScrollingPaddingHorizontalWorldSpace) / worldDimensions.x;
		offersContainer.anchorMin = new Vector2((0f - num5) * 0.5f, offersContainer.anchorMin.y);
		offersContainer.anchorMax = new Vector2(num5 * 0.5f, offersContainer.anchorMax.y);
		offersContainer.anchoredPosition = new Vector2(0f, offersContainer.anchoredPosition.y);
		offersContainer.offsetMin = new Vector2(0f, offersContainer.offsetMin.y);
		offersContainer.offsetMax = new Vector2(0f, offersContainer.offsetMax.y);
		scrollRect.horizontalNormalizedPosition = 0f;
		if (type == PageType.FirstPage)
		{
			Camera camera = NavigationManager.instance.GetCamera();
			Vector2 worldDimensions2 = GGUtil.uiUtil.GetWorldDimensions(offersContainer);
			GGUtil.uiUtil.AnchorRectInsideScreen(offersContainer, camera);
			Vector2 worldDimensions3 = GGUtil.uiUtil.GetWorldDimensions(offersContainer);
			float d = Mathf.Min(worldDimensions3.x / worldDimensions2.x, worldDimensions3.y / worldDimensions2.y);
			namedOffersPool.parent.localScale = (Vector3.right + Vector3.up) * d + Vector3.forward;
		}
		float num6 = 0f;
		for (int num7 = 0; num7 < bigItems.Count; num7++)
		{
			bigItems[num7].AnimateIn(num6);
			num6 += animationDelayPerColumn;
		}
		for (int num8 = 0; num8 < smallItems.Count; num8++)
		{
			smallItems[num8].AnimateIn(num6);
			if ((num8 + 1) % smallItemsPerGroup == 0)
			{
				num6 += animationDelayPerColumn;
			}
		}

		rewardButton.AnimateIn(num6);
		nextButton.AnimateIn(num6);
	}

	private List<RectTransform> CreateSmallPrefabs(List<OffersDB.ProductDefinition> products, int startIndex, int length)
	{
		List<RectTransform> list = new List<RectTransform>();
		for (int i = startIndex; i < startIndex + length; i++)
		{
			OffersDB.ProductDefinition product = products[i];
			CurrencyPurchaseDialogSmallPrefab currencyPurchaseDialogSmallPrefab = unamedOffersPool.Next<CurrencyPurchaseDialogSmallPrefab>(activate: true);
			currencyPurchaseDialogSmallPrefab.Init(product);
			smallItems.Add(currencyPurchaseDialogSmallPrefab);
			list.Add(currencyPurchaseDialogSmallPrefab.transform as RectTransform);
		}
		return list;
	}

	public void ButtonCallback_Next()
	{
		type = PageType.AllPage;
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		animateOutEnum = DoAnimateOut(Init);
	}

	public void ButtonCallback_Close()
	{
		Hide();
		GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
	}

	public override void OnGoBack(NavigationManager nav)
	{
		Hide();
	}

	private void Hide()
	{
		animateOutEnum = DoAnimateOut(OnHideEnd);
	}

	private void OnHideEnd()
	{
		if (onHide != null)
		{
			onHide();
		}
		NavigationManager.instance.Pop();
	}

	private IEnumerator DoAnimateOut(Action onEnd = null)
	{
		return new _003CDoAnimateOut_003Ed__35(0)
		{
			_003C_003E4__this = this,
			onEnd = onEnd
		};
	}

	public void Update()
	{
		if (animateOutEnum != null)
		{
			animateOutEnum.MoveNext();
		}
	}

	public void OnButtonPressed(CurrencyPurchaseDialogSmallPrefab button)
	{
		OffersDB.ProductDefinition product = button.product;
		BuyOffer(product);
	}

	public void OnButtonPressed(CurrencyPurchaseDialogBigPrefab button)
	{
		OffersDB.ProductDefinition product = button.product;
		BuyOffer(product);
	}

	private void BuyOffer(OffersDB.ProductDefinition product)
	{
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		NavigationManager instance = NavigationManager.instance;
		instance.Pop(activateNextScreen: false);
		instance.GetObject<InAppPurchaseConfirmScreen>().Show(new InAppPurchaseConfirmScreen.PurchaseArguments
		{
			productToBuy = product
		});
	}

	private void OnEnable()
	{
		Init();
		GGInAppPurchase instance = GGInAppPurchase.instance;
		if (!instance.IsInventoryAvailable())
		{
			instance.QueryInventory();
		}

		AdsManager.OnCompleteRewardVideo += AddMoney;
	}

	private void OnDisable()
	{
		AdsManager.OnCompleteRewardVideo -= AddMoney;
	}

	private void AddMoney()
	{
		GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.coins, 100);
		coinsCurrencyLabel.text = GGFormat.FormatPrice(GGPlayerSettings.instance.walletManager.CurrencyCount(CurrencyType.coins));
	}
}
