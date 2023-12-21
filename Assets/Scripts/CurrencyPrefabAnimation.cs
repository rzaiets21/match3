using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CurrencyPrefabAnimation : MonoBehaviour
{
	[Serializable]
	public class Scaler
	{
		[SerializeField]
		private bool scaleX = true;

		[SerializeField]
		private bool scaleY = true;

		[SerializeField]
		private bool scaleZ = true;

		public void LocalScale(Transform trans, float scale)
		{
			Vector3 localScale = trans.localScale;
			if (scaleX)
			{
				localScale.x = scale;
			}
			if (scaleY)
			{
				localScale.y = scale;
			}
			if (scaleZ)
			{
				localScale.z = scale;
			}
			trans.localScale = localScale;
		}
	}

	private sealed class _003CDoPlay_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public CurrencyPrefabAnimation _003C_003E4__this;

		public float delay;

		public Action onEnd;

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
		public _003CDoPlay_003Ed__10(int _003C_003E1__state)
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
			CurrencyPrefabAnimation currencyPrefabAnimation = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				currencyPrefabAnimation.canvasGroup.alpha = currencyPrefabAnimation.alphaCurve.Evaluate(0f);
				float scale = currencyPrefabAnimation.scaleCurve.Evaluate(0f);
				currencyPrefabAnimation.scaler.LocalScale(currencyPrefabAnimation.targetTransform, scale);
				_003Ctime_003E5__2 = 0f;
				goto IL_009d;
			}
			case 1:
				_003C_003E1__state = -1;
				goto IL_009d;
			case 2:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_009d:
				if (_003Ctime_003E5__2 < delay)
				{
					_003Ctime_003E5__2 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Ctime_003E5__2 = 0f;
				break;
			}
			if (_003Ctime_003E5__2 < currencyPrefabAnimation.duration)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float time = _003Ctime_003E5__2 / currencyPrefabAnimation.duration;
				float alpha = currencyPrefabAnimation.alphaCurve.Evaluate(time);
				float scale2 = currencyPrefabAnimation.scaleCurve.Evaluate(time);
				currencyPrefabAnimation.scaler.LocalScale(currencyPrefabAnimation.targetTransform, scale2);
				currencyPrefabAnimation.canvasGroup.alpha = alpha;
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			}
			currencyPrefabAnimation.animateInEnum = null;
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

	[SerializeField]
	private AnimationCurve scaleCurve;

	[SerializeField]
	private AnimationCurve alphaCurve;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Transform targetTransform;

	[SerializeField]
	private float duration;

	[SerializeField]
	private Scaler scaler = new Scaler();

	public IEnumerator animateInEnum;

	public void Init()
	{
		canvasGroup.alpha = alphaCurve.Evaluate(0f);
		scaler.LocalScale(targetTransform, scaleCurve.Evaluate(0f));
	}

	public void Play(float delay, Action onEnd = null)
	{
		animateInEnum = DoPlay(delay, onEnd);
	}

	public void Stop()
	{
		animateInEnum = null;
	}

	public IEnumerator DoPlay(float delay, Action onEnd = null)
	{
		return new _003CDoPlay_003Ed__10(0)
		{
			_003C_003E4__this = this,
			delay = delay,
			onEnd = onEnd
		};
	}

	public void Update()
	{
		if (animateInEnum != null)
		{
			animateInEnum.MoveNext();
		}
	}
}
