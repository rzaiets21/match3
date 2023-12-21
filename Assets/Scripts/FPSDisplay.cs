using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	private sealed class _003CDoFPSDisplay_003Ed__5 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public FPSDisplay _003C_003E4__this;

		private float _003CtotalFps_003E5__2;

		private int _003Ci_003E5__3;

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
		public _003CDoFPSDisplay_003Ed__5(int _003C_003E1__state)
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
			FPSDisplay fPSDisplay = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				_003Ci_003E5__3++;
				goto IL_0090;
			}
			_003C_003E1__state = -1;
			goto IL_001e;
			IL_0090:
			if (_003Ci_003E5__3 < fPSDisplay.iterationsBeforeDisplay)
			{
				float unscaledDeltaTime = Time.unscaledDeltaTime;
				_003CtotalFps_003E5__2 += 1f / unscaledDeltaTime;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			float num2 = _003CtotalFps_003E5__2 / (float)fPSDisplay.iterationsBeforeDisplay;
			GGUtil.ChangeText(fPSDisplay.label, $"{num2:0.}");
			goto IL_001e;
			IL_001e:
			_003CtotalFps_003E5__2 = 0f;
			if (fPSDisplay.iterationsBeforeDisplay <= 0)
			{
				fPSDisplay.iterationsBeforeDisplay = 1;
			}
			Application.targetFrameRate = fPSDisplay.targetFrameRate;
			_003Ci_003E5__3 = 0;
			goto IL_0090;
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
	private TextMeshProUGUI label;

	[SerializeField]
	private int iterationsBeforeDisplay = 1;

	[SerializeField]
	private int targetFrameRate = 60;

	private IEnumerator enumerator;

	private void Update()
	{
		if (enumerator == null)
		{
			enumerator = DoFPSDisplay();
		}
		enumerator.MoveNext();
	}

	private IEnumerator DoFPSDisplay()
	{
		return new _003CDoFPSDisplay_003Ed__5(0)
		{
			_003C_003E4__this = this
		};
	}
}
