using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Playables;

public class WellDoneContainer : MonoBehaviour
{
	public enum AnimationType
	{
		InAndOout,
		SingleAnimation,
		Director
	}

	public struct InitArguments
	{
		public Action onComplete;

		public InitArguments(Action onComplete)
		{
			this.onComplete = onComplete;
		}

		public void CallOnComplete()
		{
			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	private sealed class _003CDoShowAnimatioDirector_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WellDoneContainer _003C_003E4__this;

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
		public _003CDoShowAnimatioDirector_003Ed__12(int _003C_003E1__state)
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
			WellDoneContainer wellDoneContainer = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				wellDoneContainer.director.Play();
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (wellDoneContainer.director.state == PlayState.Playing)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			wellDoneContainer.Hide();
			wellDoneContainer.initArguments.CallOnComplete();
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

	private sealed class _003CDoShowSingleAnimation_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WellDoneContainer _003C_003E4__this;

		private IEnumerator _003CwaitingEnum_003E5__2;

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
		public _003CDoShowSingleAnimation_003Ed__13(int _003C_003E1__state)
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
			WellDoneContainer wellDoneContainer = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				wellDoneContainer.animator.Play(wellDoneContainer.inStateName, 0);
				_003CwaitingEnum_003E5__2 = wellDoneContainer.WaitTillStateFinishes(wellDoneContainer.animator, wellDoneContainer.inStateName);
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003CwaitingEnum_003E5__2.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			wellDoneContainer.Hide();
			wellDoneContainer.initArguments.CallOnComplete();
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

	private sealed class _003CDoShowAnimation_003Ed__14 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public WellDoneContainer _003C_003E4__this;

		private IEnumerator _003CwaitingEnum_003E5__2;

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
		public _003CDoShowAnimation_003Ed__14(int _003C_003E1__state)
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
			WellDoneContainer wellDoneContainer = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				wellDoneContainer.animator.Play(wellDoneContainer.inStateName, 0);
				_003CwaitingEnum_003E5__2 = wellDoneContainer.WaitTillStateFinishes(wellDoneContainer.animator, wellDoneContainer.inStateName);
				goto IL_0070;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0070;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00b3;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0070:
				if (_003CwaitingEnum_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Ctime_003E5__3 = 0f;
				goto IL_00b3;
				IL_00b3:
				if (_003Ctime_003E5__3 < wellDoneContainer.centerDuration)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				wellDoneContainer.animator.Play(wellDoneContainer.outStateName, 0);
				_003CwaitingEnum_003E5__2 = wellDoneContainer.WaitTillStateFinishes(wellDoneContainer.animator, wellDoneContainer.outStateName);
				break;
			}
			if (_003CwaitingEnum_003E5__2.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
			wellDoneContainer.Hide();
			wellDoneContainer.initArguments.CallOnComplete();
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

	private sealed class _003CWaitTillStateFinishes_003Ed__15 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public Animator animator;

		public string stateName;

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
		public _003CWaitTillStateFinishes_003Ed__15(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			case 2:
				_003C_003E1__state = -1;
				break;
			case 3:
				_003C_003E1__state = -1;
				break;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			if (!currentAnimatorStateInfo.IsName(stateName))
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if (animator.IsInTransition(0))
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			}
			if (!(currentAnimatorStateInfo.normalizedTime >= 1f))
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

	[SerializeField]
	private string inStateName;

	[SerializeField]
	private string outStateName;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float centerDuration;

	[SerializeField]
	private PlayableDirector director;

	[SerializeField]
	private AnimationType animationType;

	private InitArguments initArguments;

	private IEnumerator runningAnimation;

	public void Show(InitArguments initArguments)
	{
		this.initArguments = initArguments;
		GGUtil.SetActive(this, active: true);
		if (animationType == AnimationType.Director)
		{
			runningAnimation = DoShowAnimatioDirector();
		}
		else if (animationType == AnimationType.InAndOout)
		{
			runningAnimation = DoShowAnimation();
		}
		else
		{
			runningAnimation = DoShowSingleAnimation();
		}
		runningAnimation.MoveNext();
		GGSoundSystem.Play(GGSoundSystem.SFXType.YouWinAnimation);
	}

	public void Hide()
	{
		GGUtil.SetActive(this, active: false);
		runningAnimation = null;
	}

	private IEnumerator DoShowAnimatioDirector()
	{
		return new _003CDoShowAnimatioDirector_003Ed__12(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator DoShowSingleAnimation()
	{
		return new _003CDoShowSingleAnimation_003Ed__13(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator DoShowAnimation()
	{
		return new _003CDoShowAnimation_003Ed__14(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator WaitTillStateFinishes(Animator animator, string stateName)
	{
		return new _003CWaitTillStateFinishes_003Ed__15(0)
		{
			animator = animator,
			stateName = stateName
		};
	}

	private void Update()
	{
		if (runningAnimation != null && !runningAnimation.MoveNext())
		{
			runningAnimation = null;
		}
	}
}
