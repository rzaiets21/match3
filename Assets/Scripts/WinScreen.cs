using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Playables;
using Debug = UnityEngine.Debug;

public class WinScreen : MonoBehaviour
{
	public class InitArguments
	{
		public long baseStageWonCoins;

		public long additionalCoins;

		public long previousCoins;

		public long previousStars;

		public long currentStars;

		public Action onComplete;

		public Action onMiddle;

		public Match3Game game;

		public DecorateRoomScreen decorateRoomScreen;

		public CurrencyPanel currencyPanel;

		public RectTransform starRect;

		public RectTransform coinRect;

		public long currentCoins => previousCoins + coinsWon;

		public long coinsWon => baseStageWonCoins + additionalCoins;

		public void CallOnComplete()
		{
			GGUtil.Call(onComplete);
		}

		public void CallOnMiddle()
		{
			GGUtil.Call(onMiddle);
		}
	}

	[Serializable]
	public class Settings
	{
		public float starTravelDuration = 1f;

		public float starRotationAngle = 760f;

		public float starEndScale;

		public float backgroundFadeOutDuration = 0.5f;

		public float coinTravelDuration = 0.75f;

		public float coinEndScale;

		public int maxCoinsToAnimate = 100;
	}

	private struct TapState
	{
		public bool isTapped;

		public bool isWaitingForTap;

		public Action onTap;

		public void CallOnTap()
		{
			if (onTap != null)
			{
				onTap();
			}
		}
	}

	private sealed class _003CDoPlainAnimation_003Ed__21 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WinScreen _003C_003E4__this;

		private float _003Ctime_003E5__2;

		private IEnumerator _003CwaitForTapEnum_003E5__3;

		private CurrencyDisplay _003CstarsDisplay_003E5__4;

		private EnumeratorsList _003CenumList_003E5__5;

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
		public _003CDoPlainAnimation_003Ed__21(int _003C_003E1__state)
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
			WinScreen winScreen = _003C_003E4__this;
			CurrencyPanel currencyPanel;
			CurrencyDisplay currencyDisplay;
			Settings settings;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				GGSoundSystem.Play(GGSoundSystem.SFXType.WinScreenStart);
				GGUtil.SetActive(winScreen.thingsToHideAtEnd, active: true);
				GGUtil.Show(winScreen.normalAnimationContainer);
				_003Ctime_003E5__2 = 0f;
				goto IL_0081;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0081;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00b5;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0081:
				if (_003Ctime_003E5__2 < 1.5f)
				{
					_003Ctime_003E5__2 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				// _003CwaitForTapEnum_003E5__3 = winScreen.DoWaitForTap();
				goto IL_00b5;
				IL_00b5:
				// if (_003CwaitForTapEnum_003E5__3.MoveNext())
				// {
				// 	_003C_003E2__current = null;
				// 	_003C_003E1__state = 2;
				// 	return true;
				// }
				winScreen.normalAnimator.StopPlayback();
				// winScreen.initArguments.CallOnMiddle();
				currencyPanel = winScreen.initArguments.currencyPanel;
				_003CstarsDisplay_003E5__4 = currencyPanel.DisplayForCurrency(CurrencyType.diamonds);
				currencyDisplay = currencyPanel.DisplayForCurrency(CurrencyType.coins);
				_003CstarsDisplay_003E5__4.DisplayCount(winScreen.initArguments.previousStars);
				currencyDisplay.DisplayCount(winScreen.initArguments.previousCoins);
				_003CenumList_003E5__5 = new EnumeratorsList();
				_003CenumList_003E5__5.Clear();
				GGUtil.Hide(winScreen.thingsToHideAtEnd);
				settings = winScreen.settings;
				_003CenumList_003E5__5.Add(winScreen.Fade(winScreen.thingsTofadeAtEnd, 1f, 0f, settings.backgroundFadeOutDuration));
				_003CenumList_003E5__5.Add(winScreen.star.DoMoveTo(winScreen.initArguments.starRect));
				_003CenumList_003E5__5.Add(winScreen.coins.DoMoveCoins(Mathf.Min((int)winScreen.initArguments.coinsWon, settings.maxCoinsToAnimate), winScreen.initArguments.coinRect, winScreen.initArguments.previousCoins, winScreen.initArguments.currentCoins));
				break;
			}
			if (_003CenumList_003E5__5.Update())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
			_003CstarsDisplay_003E5__4.DisplayCount(winScreen.initArguments.currentStars);
			winScreen.Hide();
			// winScreen.initArguments.CallOnComplete();
			winScreen.OnCompleteAnimation();
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

	private sealed class _003CDoDirectorAnimation_003Ed__22 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WinScreen _003C_003E4__this;

		private float _003Ctime_003E5__2;

		private IEnumerator _003CwaitForTapEnum_003E5__3;

		private CurrencyDisplay _003CstarsDisplay_003E5__4;

		private EnumeratorsList _003CenumList_003E5__5;

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
		public _003CDoDirectorAnimation_003Ed__22(int _003C_003E1__state)
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
			WinScreen winScreen = _003C_003E4__this;
			CurrencyPanel currencyPanel;
			CurrencyDisplay currencyDisplay;
			Settings settings;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				GGSoundSystem.Play(GGSoundSystem.SFXType.WinScreenStart);
				GGUtil.SetActive(winScreen.thingsToHideAtEnd, active: true);
				winScreen.playableDirector.Play();
				_003Ctime_003E5__2 = 0f;
				goto IL_0081;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0081;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00b5;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0081:
				if (_003Ctime_003E5__2 < winScreen.minTimeBeforeCanTap)
				{
					_003Ctime_003E5__2 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003CwaitForTapEnum_003E5__3 = winScreen.DoWaitForTap();
				goto IL_00b5;
				IL_00b5:
				if (_003CwaitForTapEnum_003E5__3.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				winScreen.playableDirector.Stop();
				winScreen.initArguments.CallOnMiddle();
				currencyPanel = winScreen.initArguments.currencyPanel;
				_003CstarsDisplay_003E5__4 = currencyPanel.DisplayForCurrency(CurrencyType.diamonds);
				currencyDisplay = currencyPanel.DisplayForCurrency(CurrencyType.coins);
				_003CstarsDisplay_003E5__4.DisplayCount(winScreen.initArguments.previousStars);
				currencyDisplay.DisplayCount(winScreen.initArguments.previousCoins);
				_003CenumList_003E5__5 = new EnumeratorsList();
				_003CenumList_003E5__5.Clear();
				GGUtil.Hide(winScreen.thingsToHideAtEnd);
				settings = winScreen.settings;
				_003CenumList_003E5__5.Add(winScreen.Fade(winScreen.thingsTofadeAtEnd, 1f, 0f, settings.backgroundFadeOutDuration));
				_003CenumList_003E5__5.Add(winScreen.star.DoMoveTo(winScreen.initArguments.starRect));
				_003CenumList_003E5__5.Add(winScreen.coins.DoMoveCoins(Mathf.Min((int)winScreen.initArguments.coinsWon, settings.maxCoinsToAnimate), winScreen.initArguments.coinRect, winScreen.initArguments.previousCoins, winScreen.initArguments.currentCoins));
				break;
			}
			if (_003CenumList_003E5__5.Update())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
			_003CstarsDisplay_003E5__4.DisplayCount(winScreen.initArguments.currentStars);
			winScreen.Hide();
			winScreen.initArguments.CallOnComplete();
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

	private sealed class _003CDoAnimation_003Ed__23 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WinScreen _003C_003E4__this;

		private IEnumerator _003CwaitingTillAnimationFinish_003E5__2;

		private IEnumerator _003CwaitForTapEnum_003E5__3;

		private CurrencyDisplay _003CstarsDisplay_003E5__4;

		private EnumeratorsList _003CenumList_003E5__5;

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
		public _003CDoAnimation_003Ed__23(int _003C_003E1__state)
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
			WinScreen winScreen = _003C_003E4__this;
			CurrencyPanel currencyPanel;
			CurrencyDisplay currencyDisplay;
			Settings settings;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				winScreen.particles.CreateAndRunParticles("StarParticle", winScreen.star.transform);
				GGSoundSystem.Play(GGSoundSystem.SFXType.WinScreenStart);
				winScreen.starAnimator.Play(winScreen.inStarAnimatorState, 0);
				_003CwaitingTillAnimationFinish_003E5__2 = winScreen.WaitTillStateFinishes(winScreen.starAnimator, winScreen.inStarAnimatorState);
				goto IL_0092;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0092;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00c5;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0092:
				if (_003CwaitingTillAnimationFinish_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003CwaitForTapEnum_003E5__3 = winScreen.DoWaitForTap();
				goto IL_00c5;
				IL_00c5:
				if (_003CwaitForTapEnum_003E5__3.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				GGSoundSystem.Play(GGSoundSystem.SFXType.WinScreenReceieveAnimationStart);
				winScreen.initArguments.CallOnMiddle();
				currencyPanel = winScreen.initArguments.currencyPanel;
				_003CstarsDisplay_003E5__4 = currencyPanel.DisplayForCurrency(CurrencyType.diamonds);
				currencyDisplay = currencyPanel.DisplayForCurrency(CurrencyType.coins);
				_003CstarsDisplay_003E5__4.DisplayCount(winScreen.initArguments.previousStars);
				currencyDisplay.DisplayCount(winScreen.initArguments.previousCoins);
				_003CenumList_003E5__5 = new EnumeratorsList();
				_003CenumList_003E5__5.Clear();
				settings = winScreen.settings;
				_003CenumList_003E5__5.Add(winScreen.Fade(winScreen.thingsTofadeAtEnd, 1f, 0f, settings.backgroundFadeOutDuration));
				_003CenumList_003E5__5.Add(winScreen.star.DoMoveTo(winScreen.initArguments.starRect));
				_003CenumList_003E5__5.Add(winScreen.coins.DoMoveCoins(Mathf.Min((int)winScreen.initArguments.coinsWon, settings.maxCoinsToAnimate), winScreen.initArguments.coinRect, winScreen.initArguments.previousCoins, winScreen.initArguments.currentCoins));
				break;
			}
			if (_003CenumList_003E5__5.Update())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
			_003CstarsDisplay_003E5__4.DisplayCount(winScreen.initArguments.currentStars);
			winScreen.Hide();
			winScreen.initArguments.CallOnComplete();
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

	private sealed class _003CFade_003Ed__24 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public float duration;

		public float from;

		public float to;

		public WinScreen _003C_003E4__this;

		public List<CanvasGroup> fadeItems;

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
		public _003CFade_003Ed__24(int _003C_003E1__state)
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
			WinScreen winScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__2 <= duration)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float t = Mathf.InverseLerp(0f, duration, _003Ctime_003E5__2);
				float alpha = Mathf.Lerp(from, to, t);
				winScreen.SetAlpha(fadeItems, alpha);
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
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

	private sealed class _003CFade_003Ed__25 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public float duration;

		public float from;

		public float to;

		public CanvasGroup visualItem;

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
		public _003CFade_003Ed__25(int _003C_003E1__state)
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
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__2 <= duration)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float t = Mathf.InverseLerp(0f, duration, _003Ctime_003E5__2);
				float alpha = Mathf.Lerp(from, to, t);
				visualItem.alpha = alpha;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
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

	private sealed class _003CWaitTillStateFinishes_003Ed__26 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public Animator animator;

		public string stateName;

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
		public _003CWaitTillStateFinishes_003Ed__26(int _003C_003E1__state)
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
				_003C_003E1__state = -1;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			case 2:
				_003C_003E1__state = -1;
				break;
			case 3:
				_003C_003E1__state = -1;
				break;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (!currentAnimatorStateInfo.IsName(stateName))
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if (animator.IsInTransition(0))
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			}
			if (!(currentAnimatorStateInfo.normalizedTime >= 1f))
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
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

	private sealed class _003CDoWaitForTap_003Ed__35 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WinScreen _003C_003E4__this;

		public bool showTapContainer;

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
		public _003CDoWaitForTap_003Ed__35(int _003C_003E1__state)
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
			WinScreen winScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				winScreen.tapState = default(TapState);
				winScreen.tapState.isWaitingForTap = true;
				GGUtil.SetActive(winScreen.tapContainer, showTapContainer);
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (winScreen.tapState.isWaitingForTap && !winScreen.tapState.isTapped)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
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

	[SerializeField]
	private Animator starAnimator;

	[SerializeField]
	private UIGGParticleCreator particles;

	[SerializeField]
	private string inStarAnimatorState;

	[SerializeField]
	private RectTransform tapContainer;

	[SerializeField]
	private CanvasGroup background;

	[SerializeField]
	private List<CanvasGroup> thingsTofadeAtEnd = new List<CanvasGroup>();

	[SerializeField]
	private List<Transform> thingsToHideAtEnd = new List<Transform>();

	[SerializeField]
	private WinScreenStar star;

	[SerializeField]
	private WinScreenCoins coins;

	[SerializeField]
	private PlayableDirector playableDirector;

	[SerializeField]
	private Transform normalAnimationContainer;

	[SerializeField]
	private Animator normalAnimator;

	[SerializeField]
	private float minTimeBeforeCanTap = 1f;

	private InitArguments initArguments;

	private IEnumerator animationEnum;

	private TapState tapState;

	public CurrencyPanel currencyPanel => initArguments.currencyPanel;

	private Settings settings => Match3Settings.instance.winScreenSettings;

	public void Show(InitArguments init, Action onComplete = null)
	{
		GGUtil.Show(this);
		Init(init, onComplete);
	}

	private Action onAnimationComplete;
	
	private void Init(InitArguments init, Action onComplete)
	{
		initArguments = init;
		tapState = default(TapState);
		GGUtil.Hide(tapContainer);
		onAnimationComplete = onComplete;
		// Match3Game game = initArguments.game;
		// game.StartWinScreenBoardAnimation();
		// game.gameScreen.HideVisibleObjects();
		// game.SuspendGameSounds();
		SetAlpha(thingsTofadeAtEnd, 1f);
		star.Show(this);
		coins.Init(init.coinsWon, this);
		particles.DestroyCreatedObjects();
		animationEnum = DoPlainAnimation();
		animationEnum.MoveNext();
		// AdsManager.ShowInterstitial();
	}

	public void OnCompleteAnimation()
	{
		onAnimationComplete?.Invoke();
		Hide();
	}

	private void SetAlpha(List<CanvasGroup> list, float alpha)
	{
		for (int i = 0; i < list.Count; i++)
		{
			CanvasGroup canvasGroup = list[i];
			if (!(canvasGroup == null))
			{
				canvasGroup.alpha = alpha;
			}
		}
	}

	private IEnumerator DoPlainAnimation()
	{
		return new _003CDoPlainAnimation_003Ed__21(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator DoDirectorAnimation()
	{
		return new _003CDoDirectorAnimation_003Ed__22(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator DoAnimation()
	{
		return new _003CDoAnimation_003Ed__23(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator Fade(List<CanvasGroup> fadeItems, float from, float to, float duration)
	{
		return new _003CFade_003Ed__24(0)
		{
			_003C_003E4__this = this,
			fadeItems = fadeItems,
			from = from,
			to = to,
			duration = duration
		};
	}

	private IEnumerator Fade(CanvasGroup visualItem, float from, float to, float duration)
	{
		return new _003CFade_003Ed__25(0)
		{
			visualItem = visualItem,
			from = from,
			to = to,
			duration = duration
		};
	}

	private IEnumerator WaitTillStateFinishes(Animator animator, string stateName)
	{
		return new _003CWaitTillStateFinishes_003Ed__26(0)
		{
			animator = animator,
			stateName = stateName
		};
	}

	private void Hide()
	{
		GGUtil.Hide(this);
	}

	private void Update()
	{
		if (animationEnum != null && !animationEnum.MoveNext())
		{
			animationEnum = null;
		}
	}

	private void OnTap(Action onTap)
	{
		tapState = default(TapState);
		tapState.onTap = onTap;
		tapState.isWaitingForTap = true;
		GGUtil.Show(tapContainer);
	}

	private IEnumerator DoWaitForTap(bool showTapContainer = true)
	{
		return new _003CDoWaitForTap_003Ed__35(0)
		{
			_003C_003E4__this = this,
			showTapContainer = showTapContainer
		};
	}

	public void ButtonCallback_OnTap()
	{
		if (tapState.isWaitingForTap && !tapState.isTapped)
		{
			tapState.isTapped = true;
			tapState.CallOnTap();
		}
	}
}
