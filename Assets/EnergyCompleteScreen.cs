using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GGMatch3;
using TMPro;
using UnityEngine;

public class EnergyCompleteScreen : MonoBehaviour
{
	private sealed class _003CDoPlainAnimation_003Ed__21 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public EnergyCompleteScreen _003C_003E4__this;

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
			EnergyCompleteScreen winScreen = _003C_003E4__this;
			WinScreen.Settings settings;
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
				_003CenumList_003E5__5 = new EnumeratorsList();
				_003CenumList_003E5__5.Clear();
				GGUtil.Hide(winScreen.thingsToHideAtEnd);
				settings = winScreen.settings;
				_003CenumList_003E5__5.Add(winScreen.Fade(winScreen.thingsTofadeAtEnd, 1f, 0f, settings.backgroundFadeOutDuration));
				_003CenumList_003E5__5.Add(winScreen.star.DoMoveTo(winScreen.energyRect));
				break;
			}
			if (_003CenumList_003E5__5.Update())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
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

	private sealed class _003CFade_003Ed__24 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public float duration;

		public float from;

		public float to;

		public EnergyCompleteScreen _003C_003E4__this;

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
			EnergyCompleteScreen winScreen = _003C_003E4__this;
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
	private TextMeshProUGUI coins;

	[SerializeField]
	private Transform normalAnimationContainer;

	[SerializeField]
	private Animator normalAnimator;

	[SerializeField]
	private float minTimeBeforeCanTap = 1f;

	private RectTransform energyRect;

	private IEnumerator animationEnum;

	private WinScreen.Settings settings => Match3Settings.instance.winScreenSettings;

	public void Show(RectTransform energyRect, int energy, Action onComplete = null)
	{
		GGUtil.Show(this);
		Init(energyRect, energy, onComplete);
	}

	private Action onAnimationComplete;
	
	private void Init(RectTransform energyRect, int energy, Action onComplete)
	{
		this.energyRect = energyRect;
		GGUtil.Hide(tapContainer);
		onAnimationComplete = onComplete;
		// Match3Game game = initArguments.game;
		// game.StartWinScreenBoardAnimation();
		// game.gameScreen.HideVisibleObjects();
		// game.SuspendGameSounds();
		SetAlpha(thingsTofadeAtEnd, 1f);
		star.Show();
		coins.SetText(energy.ToString());
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

	public void Hide()
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
}
