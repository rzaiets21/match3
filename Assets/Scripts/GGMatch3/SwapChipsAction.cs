using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class SwapChipsAction : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public float duration;

			public AnimationCurve moveCurve;

			public AnimationCurve scaleCurve;

			public float brightness = 2f;
		}

		public struct SwapChipsParams
		{
			public Match3Game game;

			public Slot slot1;

			public Slot slot2;

			public bool moveToSpecificPos;

			public IntVector2 positionToMoveSlot1;

			public IntVector2 positionToMoveSlot2;

			public Action onComplete;

			public bool switchSlots;

			public bool scaleDownChip2;

			public void CallOnComplete()
			{
				if (onComplete != null)
				{
					onComplete();
				}
			}
		}

		private sealed class _003CMoveSingleChipAnimation_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public SwapChipsAction _003C_003E4__this;

			public Slot slotToMove;

			public IntVector2 positionToMoveTo;

			private float _003Ctime_003E5__2;

			private float _003Cduration_003E5__3;

			private TransformBehaviour _003CchipBehaviour_003E5__4;

			private Vector3 _003CstartPosition_003E5__5;

			private Vector3 _003CendPosition_003E5__6;

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
				SwapChipsAction swapChipsAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__3 = swapChipsAction.settings.duration;
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
					_003CstartPosition_003E5__5 = _003CchipBehaviour_003E5__4.localPosition;
					_003CendPosition_003E5__6 = swapChipsAction.chipParams.game.LocalPositionOfCenter(positionToMoveTo);
					break;
				}
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__2 <= _003Cduration_003E5__3)
				{
					_003Ctime_003E5__2 += swapChipsAction.deltaTime;
					float time = Mathf.InverseLerp(0f, _003Cduration_003E5__3, _003Ctime_003E5__2);
					time = swapChipsAction.settings.moveCurve.Evaluate(time);
					Vector3 localPosition = Vector3.LerpUnclamped(_003CstartPosition_003E5__5, _003CendPosition_003E5__6, time);
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

		private sealed class _003CScaleDownChipAnimation_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public SwapChipsAction _003C_003E4__this;

			public Slot slotToMove;

			public IntVector2 positionToMoveTo;

			private float _003Ctime_003E5__2;

			private float _003Cduration_003E5__3;

			private TransformBehaviour _003CchipBehaviour_003E5__4;

			private Vector3 _003CstartPosition_003E5__5;

			private Vector3 _003CendPosition_003E5__6;

			private Vector3 _003Cdirection_003E5__7;

			private float _003ChalphDuration_003E5__8;

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
			public _003CScaleDownChipAnimation_003Ed__10(int _003C_003E1__state)
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
				SwapChipsAction swapChipsAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__3 = swapChipsAction.settings.duration;
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
					_003CstartPosition_003E5__5 = _003CchipBehaviour_003E5__4.localPosition;
					_003CendPosition_003E5__6 = swapChipsAction.chipParams.game.LocalPositionOfCenter(positionToMoveTo);
					_003CchipBehaviour_003E5__4.localPosition = _003CstartPosition_003E5__5;
					_003Cdirection_003E5__7 = (_003CendPosition_003E5__6 - _003CstartPosition_003E5__5) * 0.5f;
					_003ChalphDuration_003E5__8 = _003Cduration_003E5__3 * 0.5f;
					break;
				}
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__2 <= _003Cduration_003E5__3)
				{
					_003Ctime_003E5__2 += swapChipsAction.deltaTime;
					float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__3, _003Ctime_003E5__2);
					float num3 = swapChipsAction.settings.moveCurve.Evaluate(num2);
					float t = swapChipsAction.settings.scaleCurve.Evaluate(Mathf.PingPong(num2, 0.5f));
					Vector3 localScale = Vector3.LerpUnclamped(Vector3.one, new Vector3(0f, 0f, 1f), t);
					_003CchipBehaviour_003E5__4.localScale = localScale;
					_003CchipBehaviour_003E5__4.localScale = Vector3.zero;
					if (num3 < _003ChalphDuration_003E5__8)
					{
						Vector3 localPosition = Vector3.LerpUnclamped(_003CstartPosition_003E5__5, _003CstartPosition_003E5__5 - _003Cdirection_003E5__7, Mathf.InverseLerp(0f, 0.5f, num3));
						_003CchipBehaviour_003E5__4.localPosition = localPosition;
					}
					else
					{
						Vector3 localPosition2 = Vector3.Lerp(_003CendPosition_003E5__6 + _003Cdirection_003E5__7, _003CendPosition_003E5__6, Mathf.InverseLerp(0.5f, 1f, num3));
						_003CchipBehaviour_003E5__4.localPosition = localPosition2;
					}
					_003CchipBehaviour_003E5__4.localPosition = _003CstartPosition_003E5__5;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003CchipBehaviour_003E5__4.localPosition = _003CendPosition_003E5__6;
				_003CchipBehaviour_003E5__4.localScale = Vector3.one;
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

		private sealed class _003CMoveAnimation_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public SwapChipsAction _003C_003E4__this;

			private EnumeratorsList _003CenumList_003E5__2;

			private Slot _003Cslot1_003E5__3;

			private Slot _003Cslot2_003E5__4;

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
			public _003CMoveAnimation_003Ed__12(int _003C_003E1__state)
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
				SwapChipsAction swapChipsAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003CenumList_003E5__2 = new EnumeratorsList();
					_003Cslot1_003E5__3 = swapChipsAction.chipParams.slot1;
					_003Cslot2_003E5__4 = swapChipsAction.chipParams.slot2;
					swapChipsAction.SetBrightness(_003Cslot1_003E5__3, swapChipsAction.settings.brightness);
					swapChipsAction.SetBrightness(_003Cslot2_003E5__4, swapChipsAction.settings.brightness);
					if (swapChipsAction.chipParams.moveToSpecificPos)
					{
						_003CenumList_003E5__2.Add(swapChipsAction.MoveSingleChipAnimation(_003Cslot1_003E5__3, swapChipsAction.chipParams.positionToMoveSlot1));
						if (swapChipsAction.chipParams.scaleDownChip2)
						{
							_003CenumList_003E5__2.Add(swapChipsAction.ScaleDownChipAnimation(_003Cslot2_003E5__4, swapChipsAction.chipParams.positionToMoveSlot2));
						}
						else
						{
							_003CenumList_003E5__2.Add(swapChipsAction.MoveSingleChipAnimation(_003Cslot2_003E5__4, swapChipsAction.chipParams.positionToMoveSlot2));
						}
					}
					else if (swapChipsAction.chipParams.switchSlots)
					{
						_003CenumList_003E5__2.Add(swapChipsAction.MoveSingleChipAnimation(_003Cslot1_003E5__3, _003Cslot2_003E5__4.position));
						if (swapChipsAction.chipParams.scaleDownChip2)
						{
							_003CenumList_003E5__2.Add(swapChipsAction.ScaleDownChipAnimation(_003Cslot2_003E5__4, _003Cslot1_003E5__3.position));
						}
						else
						{
							_003CenumList_003E5__2.Add(swapChipsAction.MoveSingleChipAnimation(_003Cslot2_003E5__4, _003Cslot1_003E5__3.position));
						}
					}
					else
					{
						_003CenumList_003E5__2.Add(swapChipsAction.MoveSingleChipAnimation(_003Cslot1_003E5__3, _003Cslot1_003E5__3.position));
						if (swapChipsAction.chipParams.scaleDownChip2)
						{
							_003CenumList_003E5__2.Add(swapChipsAction.ScaleDownChipAnimation(_003Cslot2_003E5__4, _003Cslot2_003E5__4.position));
						}
						else
						{
							_003CenumList_003E5__2.Add(swapChipsAction.MoveSingleChipAnimation(_003Cslot2_003E5__4, _003Cslot2_003E5__4.position));
						}
					}
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003CenumList_003E5__2.Update())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				swapChipsAction.slotLock.UnlockAll();
				swapChipsAction.SetBrightness(_003Cslot1_003E5__3, 1f);
				swapChipsAction.SetBrightness(_003Cslot2_003E5__4, 1f);
				swapChipsAction.chipParams.CallOnComplete();
				swapChipsAction.isAlive = false;
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

		private Settings settings => Match3Settings.instance.swapChipsActionSettings;

		public void Init(SwapChipsParams chipParams)
		{
			this.chipParams = chipParams;
			Slot slot = chipParams.slot1;
			Slot slot2 = chipParams.slot2;
			slotLock = lockContainer.NewLock();
			slotLock.isSlotGravitySuspended = true;
			slotLock.isSlotMatchingSuspended = true;
			slotLock.LockSlot(slot);
			slotLock.LockSlot(slot2);
		}

		public IEnumerator MoveSingleChipAnimation(Slot slotToMove, IntVector2 positionToMoveTo)
		{
			return new _003CMoveSingleChipAnimation_003Ed__9(0)
			{
				_003C_003E4__this = this,
				slotToMove = slotToMove,
				positionToMoveTo = positionToMoveTo
			};
		}

		public IEnumerator ScaleDownChipAnimation(Slot slotToMove, IntVector2 positionToMoveTo)
		{
			return new _003CScaleDownChipAnimation_003Ed__10(0)
			{
				_003C_003E4__this = this,
				slotToMove = slotToMove,
				positionToMoveTo = positionToMoveTo
			};
		}

		private void SetBrightness(Slot slot, float brightness)
		{
			if (slot == null)
			{
				return;
			}
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			if (slotComponent != null)
			{
				TransformBehaviour componentBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
				if (!(componentBehaviour == null))
				{
					componentBehaviour.SetBrightness(brightness);
				}
			}
		}

		public IEnumerator MoveAnimation()
		{
			return new _003CMoveAnimation_003Ed__12(0)
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
