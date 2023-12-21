using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class ShowSwapNotPossibleAction : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public float duration;

			public float moveDistance;

			public AnimationCurve moveCurve;

			public AnimationCurve moveOutCurve;

			public float brightness = 2f;
		}

		public struct SwapChipsParams
		{
			public Match3Game game;

			public Slot slot1;

			public IntVector2 positionToMoveSlot1;

			public bool wobble;
		}

		private sealed class _003CMoveSingleChipAnimation_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ShowSwapNotPossibleAction _003C_003E4__this;

			public Slot slotToMove;

			public AnimationCurve moveCurve;

			public Vector3 startPosition;

			public Vector3 endPosition;

			private float _003Ctime_003E5__2;

			private float _003Cduration_003E5__3;

			private TransformBehaviour _003CchipBehaviour_003E5__4;

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
			public _003CMoveSingleChipAnimation_003Ed__9(int _003C_003E1__state)
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
				ShowSwapNotPossibleAction showSwapNotPossibleAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__3 = showSwapNotPossibleAction.settings.duration;
					Chip slotComponent = slotToMove.GetSlotComponent<Chip>();
					if (slotComponent == null)
					{
						return false;
					}
					_003CchipBehaviour_003E5__4 = slotComponent.GetComponentBehaviour<TransformBehaviour>();
					if (_003CchipBehaviour_003E5__4 == null)
					{
						return false;
					}
					break;
				}
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__2 <= _003Cduration_003E5__3)
				{
					_003Ctime_003E5__2 += showSwapNotPossibleAction.deltaTime;
					float time = Mathf.InverseLerp(0f, _003Cduration_003E5__3, _003Ctime_003E5__2);
					time = moveCurve.Evaluate(time);
					Vector3 localPosition = Vector3.LerpUnclamped(startPosition, endPosition, time);
					_003CchipBehaviour_003E5__4.localPosition = localPosition;
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

		private sealed class _003CMoveAnimation_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ShowSwapNotPossibleAction _003C_003E4__this;

			private Slot _003Cslot1_003E5__2;

			private Vector3 _003CstartPos_003E5__3;

			private Vector3 _003CendPos_003E5__4;

			private TransformBehaviour _003CchipBehaviour_003E5__5;

			private IEnumerator _003Canimation_003E5__6;

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
			public _003CMoveAnimation_003Ed__10(int _003C_003E1__state)
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
				ShowSwapNotPossibleAction showSwapNotPossibleAction = _003C_003E4__this;
				Slot slot;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003Cslot1_003E5__2 = showSwapNotPossibleAction.chipParams.slot1;
					_003CstartPos_003E5__3 = _003Cslot1_003E5__2.localPositionOfCenter;
					Vector3 normalized = (showSwapNotPossibleAction.chipParams.positionToMoveSlot1 - _003Cslot1_003E5__2.position).ToVector3().normalized;
					_003CendPos_003E5__4 = _003CstartPos_003E5__3 + normalized * showSwapNotPossibleAction.settings.moveDistance;
					Chip slotComponent = _003Cslot1_003E5__2.GetSlotComponent<Chip>();
					_003CchipBehaviour_003E5__5 = null;
					if (slotComponent != null)
					{
						_003CchipBehaviour_003E5__5 = slotComponent.GetComponentBehaviour<TransformBehaviour>();
					}
					if (_003CchipBehaviour_003E5__5 != null)
					{
						_003CchipBehaviour_003E5__5.SetBrightness(showSwapNotPossibleAction.settings.brightness);
					}
					_003Canimation_003E5__6 = showSwapNotPossibleAction.MoveSingleChipAnimation(_003Cslot1_003E5__2, _003CstartPos_003E5__3, _003CendPos_003E5__4, showSwapNotPossibleAction.settings.moveCurve);
					goto IL_0123;
				}
				case 1:
					_003C_003E1__state = -1;
					goto IL_0123;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_0123:
					if (_003Canimation_003E5__6.MoveNext())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					slot = showSwapNotPossibleAction.chipParams.game.GetSlot(showSwapNotPossibleAction.chipParams.positionToMoveSlot1);
					if (slot != null && showSwapNotPossibleAction.chipParams.wobble)
					{
						slot.Wobble(Match3Settings.instance.chipWobbleSettings);
					}
					_003Canimation_003E5__6 = showSwapNotPossibleAction.MoveSingleChipAnimation(_003Cslot1_003E5__2, _003CendPos_003E5__4, _003CstartPos_003E5__3, showSwapNotPossibleAction.settings.moveOutCurve);
					break;
				}
				if (_003Canimation_003E5__6.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				if (_003CchipBehaviour_003E5__5 != null)
				{
					_003CchipBehaviour_003E5__5.SetBrightness(1f);
				}
				showSwapNotPossibleAction.slotLock.UnlockAll();
				showSwapNotPossibleAction.isAlive = false;
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

		private SwapChipsParams chipParams;

		private float deltaTime;

		private Lock slotLock;

		private IEnumerator moveAnimation;

		private Settings settings => Match3Settings.instance.showSwapNotPossibleActionSettings;

		public void Init(SwapChipsParams chipParams)
		{
			this.chipParams = chipParams;
			Slot slot = chipParams.slot1;
			slotLock = lockContainer.NewLock();
			slotLock.isSlotGravitySuspended = true;
			slotLock.isSlotMatchingSuspended = true;
			slotLock.LockSlot(slot);
		}

		public IEnumerator MoveSingleChipAnimation(Slot slotToMove, Vector3 startPosition, Vector3 endPosition, AnimationCurve moveCurve)
		{
			return new _003CMoveSingleChipAnimation_003Ed__9(0)
			{
				_003C_003E4__this = this,
				slotToMove = slotToMove,
				startPosition = startPosition,
				endPosition = endPosition,
				moveCurve = moveCurve
			};
		}

		public IEnumerator MoveAnimation()
		{
			return new _003CMoveAnimation_003Ed__10(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.deltaTime = deltaTime;
			if (moveAnimation == null)
			{
				moveAnimation = MoveAnimation();
			}
			moveAnimation.MoveNext();
		}
	}
}
