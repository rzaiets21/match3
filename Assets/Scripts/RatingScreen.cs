using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class RatingScreen : MonoBehaviour
{
	public struct ButtonState
	{
		public bool isActive;

		public bool isAccepted;

		public bool isDone;

		public float time;
	}

	private sealed class _003CDoAnimation_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public RatingScreen _003C_003E4__this;

		private IEnumerator _003CwaitEnum_003E5__2;

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
		public _003CDoAnimation_003Ed__16(int _003C_003E1__state)
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
			RatingScreen ratingScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
				GGUtil.ChangeText(ratingScreen.mainlabel, "Are you enjoying Home Design?");
				GGUtil.Hide(ratingScreen.widgetsToHide);
				ratingScreen.ShowYesNo("Yes", "No");
				_003CwaitEnum_003E5__2 = ratingScreen.WaitForButtonPress();
				goto IL_0083;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0083;
			case 2:
				_003C_003E1__state = -1;
				goto IL_010e;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0083:
				if (_003CwaitEnum_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (!ratingScreen.buttonState.isAccepted)
				{
					ratingScreen.End(isLike: false, isGoingToRate: false);
					ScriptableObjectSingleton<RateCallerSettings>.instance.OnUserNotLike();
					return false;
				}
				ratingScreen.PlayInState();
				GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
				GGUtil.ChangeText(ratingScreen.mainlabel, "Wohoo!!!");
				GGUtil.Hide(ratingScreen.widgetsToHide);
				ratingScreen.clickToContinueStyle.Apply();
				ratingScreen.PlayInState();
				_003CwaitEnum_003E5__2 = ratingScreen.WaitForButtonPress();
				goto IL_010e;
				IL_010e:
				if (_003CwaitEnum_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
				GGUtil.ChangeText(ratingScreen.mainlabel, "Please leave us a Rating");
				GGUtil.Hide(ratingScreen.widgetsToHide);
				ratingScreen.ShowYesNo("Yes", "Later");
				ratingScreen.PlayInState();
				_003CwaitEnum_003E5__2 = ratingScreen.WaitForButtonPress();
				break;
			}
			if (_003CwaitEnum_003E5__2.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
			if (!ratingScreen.buttonState.isAccepted)
			{
				ratingScreen.End(isLike: true, isGoingToRate: false);
				return false;
			}
			ScriptableObjectSingleton<RateCallerSettings>.instance.OnUserRated();
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.extreme.makeover.design");
			// GGSupportMenu.instance.showRateApp(ConfigBase.instance.platformRateProvider);
			ratingScreen.End(isLike: true, isGoingToRate: true);
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

	private sealed class _003CWaitForButtonPress_003Ed__18 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public RatingScreen _003C_003E4__this;

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
		public _003CWaitForButtonPress_003Ed__18(int _003C_003E1__state)
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
			RatingScreen ratingScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				ratingScreen.buttonState = default(ButtonState);
				ratingScreen.buttonState.isActive = true;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (!ratingScreen.buttonState.isDone)
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
	private List<Transform> widgetsToHide = new List<Transform>();

	[SerializeField]
	private VisualStyleSet buttonStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet clickToContinueStyle = new VisualStyleSet();

	[SerializeField]
	private Animator mainAnimation;

	[SerializeField]
	private TextMeshProUGUI mainlabel;

	[SerializeField]
	private string inState;

	[SerializeField]
	private float minTimeBeforeClick;

	private IEnumerator animation;

	[SerializeField]
	private TextMeshProUGUI yesLabel;

	[SerializeField]
	private TextMeshProUGUI noLabel;

	private ButtonState buttonState;

	private void OnEnable()
	{
		Init();
	}

	private void Init()
	{
		GGUtil.Hide(widgetsToHide);
		animation = DoAnimation();
	}

	private void PlayInState()
	{
		mainAnimation.gameObject.SetActive(value: false);
		mainAnimation.gameObject.SetActive(value: true);
		mainAnimation.Play(inState, 0);
	}

	private void ShowYesNo(string yesText, string noText)
	{
		GGUtil.ChangeText(yesLabel, yesText);
		GGUtil.ChangeText(noLabel, noText);
		GGUtil.Hide(widgetsToHide);
		buttonStyle.Apply();
	}

	private IEnumerator DoAnimation()
	{
		return new _003CDoAnimation_003Ed__16(0)
		{
			_003C_003E4__this = this
		};
	}

	private void End(bool isLike, bool isGoingToRate)
	{
		NavigationManager.instance.Pop();
		Analytics.RateDialog rateDialog = new Analytics.RateDialog();
		rateDialog.timesShown = ScriptableObjectSingleton<RateCallerSettings>.instance.timesShown;
		rateDialog.isLike = isLike;
		rateDialog.isGoingToRate = isGoingToRate;
		rateDialog.Send();
	}

	private IEnumerator WaitForButtonPress()
	{
		return new _003CWaitForButtonPress_003Ed__18(0)
		{
			_003C_003E4__this = this
		};
	}

	private void ButtonPress(bool success)
	{
		UnityEngine.Debug.Log("BUTTON PRESS");
		if (!(buttonState.time < minTimeBeforeClick))
		{
			buttonState.isAccepted = success;
			buttonState.isDone = true;
			UnityEngine.Debug.Log("Is Done");
		}
	}

	public void ButtonCallback_OnYes()
	{
		UnityEngine.Debug.Log("On Yes");
		ButtonPress(success: true);
	}

	public void ButtonCallback_OnNo()
	{
		UnityEngine.Debug.Log("On No");
		ButtonPress(success: false);
	}

	private void Update()
	{
		buttonState.time += Time.unscaledDeltaTime;
		if (animation != null)
		{
			animation.MoveNext();
		}
	}
}
