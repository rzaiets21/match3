using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UIWaitForTap : MonoBehaviour
{
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

	private sealed class _003CDoWaitForTap_003Ed__5 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public UIWaitForTap _003C_003E4__this;

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
		public _003CDoWaitForTap_003Ed__5(int _003C_003E1__state)
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
			UIWaitForTap uIWaitForTap = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				uIWaitForTap.tapState = default(TapState);
				uIWaitForTap.tapState.isWaitingForTap = true;
				GGUtil.Show(uIWaitForTap.tapContainer);
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (uIWaitForTap.tapState.isWaitingForTap && !uIWaitForTap.tapState.isTapped)
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
	private RectTransform tapContainer;

	private TapState tapState;

	private bool isTapped = false;
	
	public void Hide()
	{
		tapState = default(TapState);
		GGUtil.Hide(tapContainer);
	}

	public void OnTap(Action onTap)
	{
		tapState = default(TapState);
		tapState.onTap = onTap;
		tapState.isWaitingForTap = true;
		GGUtil.Show(tapContainer);
	}

	public IEnumerator DoWaitForTap()
	{
		return new _003CDoWaitForTap_003Ed__5(0)
		{
			_003C_003E4__this = this
		};
	}

	public void ButtonCallback_OnTap()
	{

			if (tapState.isWaitingForTap && !tapState.isTapped)
			{
				GGUtil.Hide(this);
				tapState.isTapped = true;
				tapState.CallOnTap();
			}
		// if(PlayerPrefs.GetInt("FirstGift", 0) == 0)
		// {
		// 	if (tapState.isWaitingForTap && !tapState.isTapped)
		// 	{
		// 		GGUtil.Hide(this);
		// 		tapState.isTapped = true;
		// 		tapState.CallOnTap();
		// 		PlayerPrefs.SetInt("FirstGift", 1);
		// 	}
		// 	return;
		// }
		
	}
}
