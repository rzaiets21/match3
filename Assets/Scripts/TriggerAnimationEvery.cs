using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TriggerAnimationEvery : MonoBehaviour
{
	private sealed class _003CDoAnimation_003Ed__7 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TriggerAnimationEvery _003C_003E4__this;

		private float _003Cdelay_003E5__2;

		private float _003Ctime_003E5__3;

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
		public _003CDoAnimation_003Ed__7(int _003C_003E1__state)
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
			TriggerAnimationEvery triggerAnimationEvery = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				goto IL_0029;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0076;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_00e6;
				}
				IL_0029:
				_003Cdelay_003E5__2 = UnityEngine.Random.Range(triggerAnimationEvery.fromTime, triggerAnimationEvery.toTime);
				_003Ctime_003E5__3 = 0f;
				goto IL_0076;
				IL_0076:
				if (_003Ctime_003E5__3 <= _003Cdelay_003E5__2)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (triggerAnimationEvery.animator != null)
				{
					triggerAnimationEvery.animator.Play(triggerAnimationEvery.stateToPlay, 0);
				}
				_003Cdelay_003E5__2 = triggerAnimationEvery.animationWait;
				_003Ctime_003E5__3 = 0f;
				goto IL_00e6;
				IL_00e6:
				if (_003Ctime_003E5__3 <= _003Cdelay_003E5__2)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_0029;
			}
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
	private float fromTime;

	[SerializeField]
	private float toTime;

	[SerializeField]
	private float animationWait;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string stateToPlay;

	private IEnumerator animation;

	private void OnEnable()
	{
		animation = DoAnimation();
	}

	private IEnumerator DoAnimation()
	{
		return new _003CDoAnimation_003Ed__7(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		if (animation != null)
		{
			animation.MoveNext();
		}
	}
}
