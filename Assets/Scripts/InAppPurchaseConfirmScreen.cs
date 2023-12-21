using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class InAppPurchaseConfirmScreen : UILayer, InAppBackend.Listener
{
	public struct ConfirmState
	{
		public bool isWaitingForConfirm;

		public bool isConfirmed;
	}

	public struct PurchaseArguments
	{
		public OffersDB.ProductDefinition productToBuy;

		public bool isProductBought;
	}

	private sealed class _003CDoShowPurchasedItem_003Ed__26 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public InAppPurchaseConfirmScreen _003C_003E4__this;

		private IEnumerator _003CwaitingEnum_003E5__2;

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
		public _003CDoShowPurchasedItem_003Ed__26(int _003C_003E1__state)
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
			InAppPurchaseConfirmScreen inAppPurchaseConfirmScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				inAppPurchaseConfirmScreen.purchaseSuccessStyle.Apply();
				inAppPurchaseConfirmScreen.ShowConfettiParticle();
				GGUtil.Show(inAppPurchaseConfirmScreen.successAnimationContainer);
				GGSoundSystem.Play(GGSoundSystem.SFXType.PurchaseSuccess);
				_003CwaitingEnum_003E5__2 = inAppPurchaseConfirmScreen.DoWaitForConfirm(3.5f);
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003CwaitingEnum_003E5__2.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			inAppPurchaseConfirmScreen.Hide();
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

	private sealed class _003CDoWaitForConfirm_003Ed__28 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public InAppPurchaseConfirmScreen _003C_003E4__this;

		public float maxSeconds;

		private float _003Ctime_003E5__2;

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
		public _003CDoWaitForConfirm_003Ed__28(int _003C_003E1__state)
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
			InAppPurchaseConfirmScreen inAppPurchaseConfirmScreen = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
			}
			else
			{
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				inAppPurchaseConfirmScreen.confirmState = default(ConfirmState);
				inAppPurchaseConfirmScreen.confirmState.isWaitingForConfirm = true;
			}
			_003Ctime_003E5__2 += Time.deltaTime;
			if ((!(maxSeconds > 0f) || !(_003Ctime_003E5__2 > maxSeconds)) && !inAppPurchaseConfirmScreen.confirmState.isConfirmed)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			inAppPurchaseConfirmScreen.confirmState = default(ConfirmState);
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

	[SerializeField]
	private GameObject confettiParticle;

	[SerializeField]
	private RectTransform successAnimationContainer;

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private CurrencyPurchaseDialogBigPrefab namedPrefab;

	[SerializeField]
	private CurrencyPurchaseDialogSmallPrefab notNamedPrefab;

	[SerializeField]
	private VisualStyleSet loadingStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet purchaseSuccessStyle = new VisualStyleSet();

	private PurchaseArguments _003CpurchaseArguments_003Ek__BackingField;

	private IEnumerator updateAnimation;

	private ConfirmState confirmState;

	private bool isShowSuspended;

	private List<PurchaseArguments> suspendedArguments = new List<PurchaseArguments>();

	public PurchaseArguments purchaseArguments
	{
		get
		{
			return _003CpurchaseArguments_003Ek__BackingField;
		}
		private set
		{
			_003CpurchaseArguments_003Ek__BackingField = value;
		}
	}

	public void SuspendShow()
	{
		isShowSuspended = true;
	}

	public void ResumeShow()
	{
		isShowSuspended = false;
		if (suspendedArguments.Count > 0)
		{
			PurchaseArguments purchaseArguments = suspendedArguments[suspendedArguments.Count - 1];
			suspendedArguments.Clear();
			Show(purchaseArguments);
		}
	}

	public void Show(PurchaseArguments purchaseArguments)
	{
		if (isShowSuspended && purchaseArguments.isProductBought)
		{
			suspendedArguments.Add(purchaseArguments);
			return;
		}
		suspendedArguments.Clear();
		isShowSuspended = false;
		this.purchaseArguments = purchaseArguments;
		NavigationManager.instance.Push(base.gameObject);
	}

	private void Init()
	{
		updateAnimation = null;
		GGUtil.SetActive(widgetsToHide, active: false);
		OffersDB.ProductDefinition productToBuy = purchaseArguments.productToBuy;
		if (productToBuy.offer.isNamedOffer)
		{
			namedPrefab.Init(productToBuy);
			GGUtil.SetActive(namedPrefab, active: true);
		}
		else
		{
			notNamedPrefab.Init(productToBuy);
			GGUtil.SetActive(notNamedPrefab, active: true);
		}
		confirmState = default(ConfirmState);
		loadingStyle.Apply();
		if (purchaseArguments.isProductBought)
		{
			updateAnimation = DoShowPurchasedItem();
		}
		else
		{
			BehaviourSingletonInit<InAppBackend>.instance.PurchaseItem(purchaseArguments.productToBuy.productID);
		}
	}

	private void OnEnable()
	{
		BehaviourSingletonInit<InAppBackend>.instance.AddListener(this);
		// IAPManager.PurchaseCompleted += OnPurchaseSuccess;
		// IAPManager.PurchaseFailed += OnPurchaseFailed;
		Init();
	}

	private void OnDisable()
	{
		BehaviourSingletonInit<InAppBackend>.instance.RemoveListener(this);
		// IAPManager.PurchaseCompleted -= OnPurchaseSuccess;
		// IAPManager.PurchaseFailed -= OnPurchaseFailed;
	}

	public void OnInitialized(InAppBackend.InitializeArguments initializeArguments)
	{
	}

	// private void OnPurchaseSuccess(IAPProduct product)
	// {
	// 	// OnPurchase(new InAppBackend.PurchaseEventArguments
	// 	// {
	// 	// 	isSuccess = true,
	// 	// 	productId = product.Id,
	// 	// });
	// }
	//
	// private void OnPurchaseFailed(IAPProduct product)
	// {
	// 	// OnPurchase(new InAppBackend.PurchaseEventArguments
	// 	// {
	// 	// 	isSuccess = false,
	// 	// 	productId = product.Id,
	// 	// });
	// }
	
	public void OnPurchase(InAppBackend.PurchaseEventArguments purchaseParams)
	{
		if (updateAnimation != null)
		{
			return;
		}
		if (purchaseParams.isSuccess)
		{
			if (purchaseParams.productId != purchaseArguments.productToBuy.productID)
			{
				GGUtil.Hide(namedPrefab);
				GGUtil.Hide(notNamedPrefab);
			}
			updateAnimation = DoShowPurchasedItem();
		}
		else
		{
			Hide();
		}
	}

	private void Hide()
	{
		NavigationManager.instance.Pop();
	}

	private IEnumerator DoShowPurchasedItem()
	{
		return new _003CDoShowPurchasedItem_003Ed__26(0)
		{
			_003C_003E4__this = this
		};
	}

	private void ShowConfettiParticle()
	{
		GGUtil.SetActive(UnityEngine.Object.Instantiate(confettiParticle, base.transform), active: true);
	}

	private IEnumerator DoWaitForConfirm(float maxSeconds)
	{
		return new _003CDoWaitForConfirm_003Ed__28(0)
		{
			_003C_003E4__this = this,
			maxSeconds = maxSeconds
		};
	}

	private void Update()
	{
		if (updateAnimation != null)
		{
			updateAnimation.MoveNext();
		}
	}

	public void ButtonCallback_OnConfirm()
	{
		if (confirmState.isWaitingForConfirm)
		{
			confirmState.isConfirmed = true;
		}
	}

	public override void OnGoBack(NavigationManager nav)
	{
		if (updateAnimation == null)
		{
			Hide();
			GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
		}
	}
}
