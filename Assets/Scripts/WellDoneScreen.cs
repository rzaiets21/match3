using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class WellDoneScreen : MonoBehaviour
{
	public struct InitArguments
	{
		public string mainText;

		public Action onComplete;
	}

	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public bool complete;

		internal void _003CDoAnimation_003Eb__0()
		{
			complete = true;
		}
	}

	private sealed class _003CDoAnimation_003Ed__7 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WellDoneScreen _003C_003E4__this;

		private _003C_003Ec__DisplayClass7_0 _003C_003E8__1;

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
			WellDoneScreen wellDoneScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass7_0();
				WellDoneContainer.InitArguments initArguments = default(WellDoneContainer.InitArguments);
				_003C_003E8__1.complete = false;
				initArguments.onComplete = _003C_003E8__1._003CDoAnimation_003Eb__0;
				wellDoneScreen.container.Show(initArguments);
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (!_003C_003E8__1.complete)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			wellDoneScreen.container.Hide();
			if (wellDoneScreen.initArguments.onComplete != null)
			{
				wellDoneScreen.initArguments.onComplete();
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
	private WellDoneContainer container;

	[SerializeField]
	private TextMeshProUGUI mainTextLabel;

	private IEnumerator animation;

	private InitArguments initArguments;

	public void Show(InitArguments initArguments)
	{
		this.initArguments = initArguments;
		NavigationManager.instance.Push(this, isModal: true);
	}

	private void Init()
	{
		GGUtil.ChangeText(mainTextLabel, initArguments.mainText);
		animation = DoAnimation();
		animation.MoveNext();
	}

	private IEnumerator DoAnimation()
	{
		return new _003CDoAnimation_003Ed__7(0)
		{
			_003C_003E4__this = this
		};
	}

	private void OnEnable()
	{
		Init();
	}

	private void Update()
	{
		if (animation != null)
		{
			animation.MoveNext();
		}
	}
}
