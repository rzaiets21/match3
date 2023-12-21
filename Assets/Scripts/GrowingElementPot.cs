using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GrowingElementPot : MonoBehaviour
{
	[Serializable]
	public class Settings
	{
		public Vector3 startScale;

		public Vector3 endScale = Vector3.one;

		public AnimationCurve scaleCurve;

		public float scaleDuration;
	}

	private sealed class _003CDoAnimateIn_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GrowingElementPot _003C_003E4__this;

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
		public _003CDoAnimateIn_003Ed__12(int _003C_003E1__state)
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
			GrowingElementPot growingElementPot = _003C_003E4__this;
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
			if (_003Ctime_003E5__2 <= growingElementPot.settings.scaleDuration)
			{
				float deltaTime = Time.deltaTime;
				_003Ctime_003E5__2 += deltaTime;
				float time = Mathf.InverseLerp(0f, growingElementPot.settings.scaleDuration, _003Ctime_003E5__2);
				float t = growingElementPot.settings.scaleCurve.Evaluate(time);
				Vector3 scale = Vector3.Lerp(growingElementPot.settings.startScale, growingElementPot.settings.endScale, t);
				GGUtil.SetScale(growingElementPot.scaleTransform, scale);
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
	private Transform scaleTransform;

	[SerializeField]
	private Transform flowerTransform;

	[SerializeField]
	private Transform stemTransform;

	private IEnumerator animationEnum;

	private Settings settings => Match3Settings.instance.growingElementPotSettings;

	public Vector3 WorldPositionForFlower => flowerTransform.position;

	public void SetActve(bool active)
	{
		GGUtil.SetActive(flowerTransform, active);
	}

	public void AnimateIn()
	{
		animationEnum = DoAnimateIn();
	}

	public void StopAnimation()
	{
		animationEnum = null;
		GGUtil.SetScale(scaleTransform, settings.endScale);
	}

	public IEnumerator DoAnimateIn()
	{
		return new _003CDoAnimateIn_003Ed__12(0)
		{
			_003C_003E4__this = this
		};
	}

	public void Update()
	{
		if (animationEnum != null && !animationEnum.MoveNext())
		{
			animationEnum = null;
		}
	}
}
