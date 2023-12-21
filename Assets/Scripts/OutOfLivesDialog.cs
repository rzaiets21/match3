using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ITSoft;
using TMPro;
using UnityEngine;

public class OutOfLivesDialog : MonoBehaviour
{
	public class State
	{
		public int lives;

		public float secsToNextLife;
	}

	private sealed class _003CUpdateLives_003Ed__24 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public OutOfLivesDialog _003C_003E4__this;

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
		public _003CUpdateLives_003Ed__24(int _003C_003E1__state)
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
			OutOfLivesDialog outOfLivesDialog = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				goto IL_0029;
			case 1:
				_003C_003E1__state = -1;
				break;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_0029;
				}
				IL_0029:
				_003Ctime_003E5__2 = 0f;
				break;
			}
			if (_003Ctime_003E5__2 < 1f)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			outOfLivesDialog.UpdateState();
			outOfLivesDialog.UpdateVisuals();
			_003C_003E2__current = null;
			_003C_003E1__state = 2;
			return true;
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
	private TextMeshProUGUI livesCountLabel;

	[SerializeField]
	private TextMeshProUGUI timeCountLabel;

	[SerializeField]
	private TextMeshProUGUI priceLabel;

	[SerializeField]
	private TextMeshProUGUI coinsLabel;

	[SerializeField]
	private string priceFormat;

	private Action onAllLivesRefilled;

	private Action onMinLivesAvailable;

	private Action onHide;

	private State initState = new State();

	private State currentState = new State();

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private VisualStyleSet noLivesStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet someLivesStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet fullLivesStyle = new VisualStyleSet();

	private LivesPriceConfig.PriceConfig priceConfig;

	private IEnumerator updateLivesEnum;

	private int maxLives => EnergyControlConfig.instance.totalCoin;
	
	
	public void Show(LivesPriceConfig.PriceConfig priceConfig, Action onAllLivesRefilled, Action onMinLivesAvailable, Action onHide)
	{
		this.onAllLivesRefilled = onAllLivesRefilled;
		this.onMinLivesAvailable = onMinLivesAvailable;
		this.onHide = onHide;
		this.priceConfig = priceConfig;
		NavigationManager.instance.Push(base.gameObject, isModal: true);
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}

	public void Init()
	{
		initState.lives = BehaviourSingleton<EnergyManager>.instance.ownedPlayCoins;
		initState.secsToNextLife = BehaviourSingleton<EnergyManager>.instance.secToNextCoin;
		UpdateState();
		UpdateVisuals();
	}

	public void Hide()
	{
		if (onHide != null)
		{
			onHide();
		}
		NavigationManager.instance.Pop();
		GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
	}

	public void OnEnable()
	{
		Init();
		AdsManager.OnCompleteRewardVideo += RewardedCallBack;
	}

	public void OnDisable()
	{
		updateLivesEnum = null;
		AdsManager.OnCompleteRewardVideo -= RewardedCallBack;
	}

	public void UpdateVisuals()
	{
		long price = GGPlayerSettings.instance.walletManager.CurrencyCount(CurrencyType.coins);
		coinsLabel.text = GGFormat.FormatPrice(price);
		SingleCurrencyPrice priceForLives = priceConfig.GetPriceForLives(maxLives - currentState.lives);
		priceLabel.text = string.Format(priceFormat, priceForLives.cost);
		livesCountLabel.text = currentState.lives.ToString();
		timeCountLabel.text = GGFormat.FormatTimeSpan(TimeSpan.FromSeconds(currentState.secsToNextLife));
		GGUtil.SetActive(widgetsToHide, active: false);
		if (currentState.lives == 0)
		{
			noLivesStyle.Apply();
		}
		else if (currentState.lives == maxLives)
		{
			fullLivesStyle.Apply();
		}
		else
		{
			someLivesStyle.Apply();
		}
	}

	public void UpdateState()
	{
		currentState.lives = BehaviourSingleton<EnergyManager>.instance.ownedPlayCoins;
		currentState.secsToNextLife = BehaviourSingleton<EnergyManager>.instance.secToNextCoin;
		if (onMinLivesAvailable != null && initState.lives == 0 && currentState.lives > 0)
		{
			onMinLivesAvailable();
		}
		if (onAllLivesRefilled != null && currentState.lives == maxLives && currentState.lives > initState.lives)
		{
			onAllLivesRefilled();
		}
	}

	public void Update()
	{
		if (updateLivesEnum == null)
		{
			updateLivesEnum = UpdateLives();
		}
		updateLivesEnum.MoveNext();
	}

	public IEnumerator UpdateLives()
	{
		return new _003CUpdateLives_003Ed__24(0)
		{
			_003C_003E4__this = this
		};
	}

	public void ButtonCallback_RefillLives()
	{
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		if (currentState.lives == maxLives)
		{
			Hide();
			return;
		}
		SingleCurrencyPrice priceForLives = priceConfig.GetPriceForLives(maxLives - currentState.lives);
		WalletManager walletManager = GGPlayerSettings.instance.walletManager;
		if (!walletManager.CanBuyItemWithPrice(priceForLives))
		{
			NavigationManager.instance.GetObject<CurrencyPurchaseDialog>().Show(ScriptableObjectSingleton<OffersDB>.instance);
			return;
		}
		Analytics.LivesRefillBoughtEvent livesRefillBoughtEvent = new Analytics.LivesRefillBoughtEvent();
		livesRefillBoughtEvent.config = priceConfig;
		livesRefillBoughtEvent.livesBeforeRefill = currentState.lives;
		livesRefillBoughtEvent.livesAfterRefill = maxLives;
		livesRefillBoughtEvent.Send();
		walletManager.BuyItem(priceForLives);
		BehaviourSingleton<EnergyManager>.instance.FillEnergy();
		UpdateState();
		UpdateVisuals();
	}

	private void RewardedCallBack()
	{
		
		if (currentState.lives == maxLives)
		{
			Hide();
			return;
		}
		
		BehaviourSingleton<EnergyManager>.instance.AddEnergy();
		if (currentState.lives == 4)
		{
			Hide();
			return;
		}

		UpdateState();
		UpdateVisuals();
	}
	
	public void ButtonCallback_RefillOneLive()
	{
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		if (currentState.lives == maxLives)
		{
			Hide();
			return;
		}
		
		AdsManager.ShowRewarded();
	}
}
