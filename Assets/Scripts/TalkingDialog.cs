using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class TalkingDialog : MonoBehaviour
{
	public class ShowArguments
	{
		public List<string> thingsToSay = new List<string>();

		public Action onComplete;

		public InputHandler inputHandler;
	}

	private struct ClickParams
	{
		public bool isWaitingForClick;

		public bool isClicked;
	}

	private sealed class _003CDoShow_003Ed__8 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TalkingDialog _003C_003E4__this;

		public ShowArguments showArguments;

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
		public _003CDoShow_003Ed__8(int _003C_003E1__state)
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
			TalkingDialog talkingDialog = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				talkingDialog.Show(showArguments);
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (talkingDialog.isActive)
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

	private sealed class _003CDoShowText_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TalkingDialog _003C_003E4__this;

		private List<string> _003CthingsToSay_003E5__2;

		private int _003Ci_003E5__3;

		private IEnumerator _003Ce_003E5__4;

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
		public _003CDoShowText_003Ed__12(int _003C_003E1__state)
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
			TalkingDialog talkingDialog = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				goto IL_007b;
			}
			_003C_003E1__state = -1;
			_003CthingsToSay_003E5__2 = talkingDialog.showArguments.thingsToSay;
			_003Ci_003E5__3 = 0;
			goto IL_009f;
			IL_007b:
			if (_003Ce_003E5__4.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			_003Ce_003E5__4 = null;
			_003Ci_003E5__3++;
			goto IL_009f;
			IL_009f:
			if (_003Ci_003E5__3 < _003CthingsToSay_003E5__2.Count)
			{
				string text = _003CthingsToSay_003E5__2[_003Ci_003E5__3];
				GGUtil.ChangeText(talkingDialog.talkLabel, text);
				_003Ce_003E5__4 = talkingDialog.DoWaitForClick();
				goto IL_007b;
			}
			talkingDialog.Hide();
			talkingDialog.showArguments.onComplete?.Invoke();
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

	private sealed class _003CDoWaitForClick_003Ed__14 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TalkingDialog _003C_003E4__this;

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
		public _003CDoWaitForClick_003Ed__14(int _003C_003E1__state)
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
			TalkingDialog talkingDialog = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				talkingDialog.clickParams = default(ClickParams);
				talkingDialog.clickParams.isWaitingForClick = true;
				if (talkingDialog.showArguments.inputHandler != null)
				{
					talkingDialog.showArguments.inputHandler.Clear();
					talkingDialog.showArguments.inputHandler.onClick -= talkingDialog.InputHandler_OnClick;
					talkingDialog.showArguments.inputHandler.onClick += talkingDialog.InputHandler_OnClick;
				}
				else
				{
					GGUtil.Show(talkingDialog.clickContainer);
				}
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (!talkingDialog.clickParams.isClicked)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if (talkingDialog.showArguments.inputHandler != null)
			{
				talkingDialog.showArguments.inputHandler.onClick -= talkingDialog.InputHandler_OnClick;
			}
			GGUtil.Hide(talkingDialog.clickContainer);
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
	private TextMeshProUGUI talkLabel;

	[SerializeField]
	private Transform clickContainer;

	[NonSerialized]
	private ShowArguments showArguments;

	[NonSerialized]
	private IEnumerator animEnum;

	[NonSerialized]
	private ClickParams clickParams;

	private bool isActive;

	public IEnumerator DoShow(ShowArguments showArguments)
	{
		return new _003CDoShow_003Ed__8(0)
		{
			_003C_003E4__this = this,
			showArguments = showArguments
		};
	}

	public void ShowSingleLine(string toSay)
	{
		GGUtil.Hide(clickContainer);
		GGUtil.Show(this);
		GGUtil.ChangeText(talkLabel, toSay);
	}

	public void Show(ShowArguments showArguments)
	{
		isActive = true;
		this.showArguments = showArguments;
		animEnum = DoShowText();
		animEnum.MoveNext();
		GGUtil.Show(this);
		GGUtil.Hide(clickContainer);
	}

	public void Hide()
	{
		isActive = false;
		GGUtil.Hide(this);
	}

	private IEnumerator DoShowText()
	{
		return new _003CDoShowText_003Ed__12(0)
		{
			_003C_003E4__this = this
		};
	}

	private void InputHandler_OnClick(Vector2 screenPosition)
	{
		clickParams.isClicked = true;
	}

	private IEnumerator DoWaitForClick()
	{
		return new _003CDoWaitForClick_003Ed__14(0)
		{
			_003C_003E4__this = this
		};
	}

	public void ButtonCallback_OnClick()
	{
		if (clickParams.isWaitingForClick)
		{
			clickParams.isClicked = true;
		}
	}

	private void Update()
	{
		if (animEnum != null)
		{
			animEnum.MoveNext();
		}
	}
}
