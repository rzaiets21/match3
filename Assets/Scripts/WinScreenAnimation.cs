using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WinScreenAnimation : MonoBehaviour
{
	private sealed class _003CDoPlay_003Ed__8 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WinScreenAnimation _003C_003E4__this;

		private bool _003CcalledOnMiddle_003E5__2;

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
		public _003CDoPlay_003Ed__8(int _003C_003E1__state)
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
			WinScreenAnimation winScreenAnimation = _003C_003E4__this;
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
				_003CcalledOnMiddle_003E5__2 = false;
			}
			AnimatorStateInfo currentAnimatorStateInfo = winScreenAnimation.animationAnimator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName(winScreenAnimation.stateNameToPlay))
			{
				if (!_003CcalledOnMiddle_003E5__2 && currentAnimatorStateInfo.normalizedTime >= winScreenAnimation.normalizedTimeOfMiddle)
				{
					_003CcalledOnMiddle_003E5__2 = true;
					if (winScreenAnimation.onMiddle != null)
					{
						winScreenAnimation.onMiddle();
					}
				}
				if (!(currentAnimatorStateInfo.normalizedTime >= 1f))
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
			}
			if (!_003CcalledOnMiddle_003E5__2 && winScreenAnimation.onMiddle != null)
			{
				winScreenAnimation.onMiddle();
			}
			if (winScreenAnimation.onEnd != null)
			{
				winScreenAnimation.onEnd();
			}
			GGUtil.SetActive(winScreenAnimation.animationTransform, active: false);
			GGUtil.SetActive(winScreenAnimation, active: false);
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
	private Animator animationAnimator;

	[SerializeField]
	private Transform animationTransform;

	[SerializeField]
	private string stateNameToPlay;

	[SerializeField]
	private float normalizedTimeOfMiddle = 0.5f;

	private Action onMiddle;

	private Action onEnd;

	private IEnumerator animationEnumerator;

	public void ShowAnimation(Action onMiddle, Action onEnd)
	{
		this.onMiddle = onMiddle;
		this.onEnd = onEnd;
		GGUtil.SetActive(this, active: true);
		GGUtil.SetActive(animationTransform, active: true);
		animationAnimator.Play(stateNameToPlay);
		animationEnumerator = DoPlay();
	}

	private IEnumerator DoPlay()
	{
		return new _003CDoPlay_003Ed__8(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		if (animationEnumerator != null && !animationEnumerator.MoveNext())
		{
			animationEnumerator = null;
		}
	}
}
