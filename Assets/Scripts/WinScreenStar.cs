using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenStar : MonoBehaviour
{
	private sealed class _003CDoMoveTo_003Ed__3 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WinScreenStar _003C_003E4__this;

		public RectTransform moveToTransform;

		private Transform _003CmyTransform_003E5__2;

		private Vector3 _003CstartPositionLocal_003E5__3;

		private Vector3 _003CendPositionLocal_003E5__4;

		private int _003CstartAngle_003E5__5;

		private float _003CendAngle_003E5__6;

		private int _003CstartScale_003E5__7;

		private float _003CendScale_003E5__8;

		private float _003Ctime_003E5__9;

		private float _003Cduration_003E5__10;

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
		public _003CDoMoveTo_003Ed__3(int _003C_003E1__state)
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
			WinScreenStar winScreenStar = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003CmyTransform_003E5__2 = winScreenStar.image.transform;
				Vector3 vector = winScreenStar.image.transform.parent.InverseTransformPoint(moveToTransform.position);
				vector.z = 0f;
				WinScreen.Settings settings = winScreenStar.settings;
				_003CstartPositionLocal_003E5__3 = _003CmyTransform_003E5__2.localPosition;
				_003CendPositionLocal_003E5__4 = vector;
				_003CstartAngle_003E5__5 = 0;
				_003CendAngle_003E5__6 = settings.starRotationAngle;
				_003CstartScale_003E5__7 = 1;
				_003CendScale_003E5__8 = settings.starEndScale;
				_003Ctime_003E5__9 = 0f;
				_003Cduration_003E5__10 = settings.starTravelDuration;
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__9 <= _003Cduration_003E5__10)
			{
				_003Ctime_003E5__9 += Time.deltaTime;
				float t = Mathf.InverseLerp(0f, _003Cduration_003E5__10, _003Ctime_003E5__9);
				float angle = Mathf.Lerp(_003CstartAngle_003E5__5, _003CendAngle_003E5__6, t);
				float d = Mathf.Lerp(_003CstartScale_003E5__7, _003CendScale_003E5__8, t);
				Vector3 localPosition = Vector3.LerpUnclamped(_003CstartPositionLocal_003E5__3, _003CendPositionLocal_003E5__4, t);
				_003CmyTransform_003E5__2.localPosition = localPosition;
				_003CmyTransform_003E5__2.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
				_003CmyTransform_003E5__2.localScale = Vector3.one * d;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if(winScreenStar.winScreen != null)
				winScreenStar.winScreen.currencyPanel.DisplayForCurrency(CurrencyType.diamonds).ShowShineParticle();
			GGSoundSystem.Play(GGSoundSystem.SFXType.RecieveStar);
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
	private Image image;

	private WinScreen winScreen;

	private WinScreen.Settings settings => Match3Settings.instance.winScreenSettings;

	public void Show(WinScreen winScreen)
	{
		this.winScreen = winScreen;
		GGUtil.SetActive(this, active: true);
		GGUtil.SetAlpha(image, 1f);
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = Vector3.one;
		base.transform.localRotation = Quaternion.identity;
	}
	public void Show()
	{
		this.winScreen = null;
		GGUtil.SetActive(this, active: true);
		GGUtil.SetAlpha(image, 1f);
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = Vector3.one;
		base.transform.localRotation = Quaternion.identity;
	}

	public IEnumerator DoMoveTo(RectTransform moveToTransform)
	{
		return new _003CDoMoveTo_003Ed__3(0)
		{
			_003C_003E4__this = this,
			moveToTransform = moveToTransform
		};
	}
}
