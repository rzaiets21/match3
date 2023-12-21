using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RankedBoostersStartAnimation : MonoBehaviour
{
	private sealed class _003CDoAnimation_003Ed__5 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public RankedBoostersStartAnimation _003C_003E4__this;

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
		public _003CDoAnimation_003Ed__5(int _003C_003E1__state)
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
			RankedBoostersStartAnimation rankedBoostersStartAnimation = _003C_003E4__this;
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
			if (_003Ctime_003E5__2 <= rankedBoostersStartAnimation.timeToLive)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			GGUtil.Hide(rankedBoostersStartAnimation);
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
	private RankedBoostersContainer boosters;

	[SerializeField]
	public float boosterDelay = 1f;

	[SerializeField]
	private float timeToLive;

	private IEnumerator animation;

	public void Show(int rankLevel)
	{
		boosters.Init(rankLevel);
		GGUtil.Show(this);
		animation = DoAnimation();
		animation.MoveNext();
	}

	private IEnumerator DoAnimation()
	{
		return new _003CDoAnimation_003Ed__5(0)
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
