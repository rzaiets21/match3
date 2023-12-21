using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShuffleContainer : MonoBehaviour
{
	private sealed class _003CDoShow_003Ed__2 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ShuffleContainer _003C_003E4__this;

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
		public _003CDoShow_003Ed__2(int _003C_003E1__state)
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
			ShuffleContainer shuffleContainer = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				AnimatorStateInfo currentAnimatorStateInfo = shuffleContainer.animator.GetCurrentAnimatorStateInfo(0);
				bool flag = currentAnimatorStateInfo.IsName("InAnimation");
				if (!flag || (flag && currentAnimatorStateInfo.normalizedTime >= 1f))
				{
					return false;
				}
			}
			else
			{
				_003C_003E1__state = -1;
				GGUtil.SetActive(shuffleContainer, active: true);
				shuffleContainer.animator.Play("InAnimation");
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

	private sealed class _003CDoHide_003Ed__3 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ShuffleContainer _003C_003E4__this;

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
		public _003CDoHide_003Ed__3(int _003C_003E1__state)
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
			ShuffleContainer shuffleContainer = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				AnimatorStateInfo currentAnimatorStateInfo = shuffleContainer.animator.GetCurrentAnimatorStateInfo(0);
				shuffleContainer.animator.GetAnimatorTransitionInfo(0);
				bool flag = currentAnimatorStateInfo.IsName("OutAnimation");
				if (!flag || (flag && currentAnimatorStateInfo.normalizedTime >= 1f))
				{
					GGUtil.SetActive(shuffleContainer, active: false);
					return false;
				}
			}
			else
			{
				_003C_003E1__state = -1;
				shuffleContainer.animator.Play("OutAnimation");
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

	public void Reset()
	{
		animator.Play("InitialState");
	}

	public IEnumerator DoShow()
	{
		return new _003CDoShow_003Ed__2(0)
		{
			_003C_003E4__this = this
		};
	}

	public IEnumerator DoHide()
	{
		return new _003CDoHide_003Ed__3(0)
		{
			_003C_003E4__this = this
		};
	}
}
