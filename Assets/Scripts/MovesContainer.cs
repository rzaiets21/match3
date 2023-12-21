using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class MovesContainer : MonoBehaviour
{
	private sealed class _003CShowAndHide_003Ed__7 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public MovesContainer _003C_003E4__this;

		private IEnumerator _003Cenumerator_003E5__2;

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
		public _003CShowAndHide_003Ed__7(int _003C_003E1__state)
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
			MovesContainer movesContainer = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Cenumerator_003E5__2 = movesContainer.DoShow();
				goto IL_0052;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0052;
			case 2:
				_003C_003E1__state = -1;
				goto IL_0095;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0052:
				if (_003Cenumerator_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Ctime_003E5__3 = 0f;
				goto IL_0095;
				IL_0095:
				if (_003Ctime_003E5__3 < movesContainer.showTime)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003Cenumerator_003E5__2 = movesContainer.DoHide();
				break;
			}
			if (_003Cenumerator_003E5__2.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
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

	private sealed class _003CDoShow_003Ed__8 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public MovesContainer _003C_003E4__this;

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
			MovesContainer movesContainer = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				AnimatorStateInfo currentAnimatorStateInfo = movesContainer.animator.GetCurrentAnimatorStateInfo(0);
				bool flag = currentAnimatorStateInfo.IsName("InAnimation");
				if (!flag || (flag && currentAnimatorStateInfo.normalizedTime >= 1f))
				{
					return false;
				}
			}
			else
			{
				_003C_003E1__state = -1;
				GGUtil.SetActive(movesContainer, active: true);
				movesContainer.animator.Play("InAnimation");
			}
			_003C_003E2__current = null;
			_003C_003E1__state = 1;
			return true;
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

	private sealed class _003CDoHide_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public MovesContainer _003C_003E4__this;

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
		public _003CDoHide_003Ed__9(int _003C_003E1__state)
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
			MovesContainer movesContainer = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				AnimatorStateInfo currentAnimatorStateInfo = movesContainer.animator.GetCurrentAnimatorStateInfo(0);
				movesContainer.animator.GetAnimatorTransitionInfo(0);
				bool flag = currentAnimatorStateInfo.IsName("OutAnimation");
				if (!flag || (flag && currentAnimatorStateInfo.normalizedTime >= 1f))
				{
					GGUtil.SetActive(movesContainer, active: false);
					return false;
				}
			}
			else
			{
				_003C_003E1__state = -1;
				movesContainer.animator.Play("OutAnimation");
			}
			_003C_003E2__current = null;
			_003C_003E1__state = 1;
			return true;
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
	private Animator animator;

	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private float showTime;

	private IEnumerator animation;

	public void Show(string text)
	{
		GGUtil.ChangeText(label, text);
		GGUtil.SetActive(this, active: true);
		animation = ShowAndHide();
		animation.MoveNext();
	}

	private void Update()
	{
		if (animation != null)
		{
			animation.MoveNext();
		}
	}

	public void Reset()
	{
		animator.Play("InitialState");
	}

	private IEnumerator ShowAndHide()
	{
		return new _003CShowAndHide_003Ed__7(0)
		{
			_003C_003E4__this = this
		};
	}

	public IEnumerator DoShow()
	{
		return new _003CDoShow_003Ed__8(0)
		{
			_003C_003E4__this = this
		};
	}

	public IEnumerator DoHide()
	{
		return new _003CDoHide_003Ed__9(0)
		{
			_003C_003E4__this = this
		};
	}
}
