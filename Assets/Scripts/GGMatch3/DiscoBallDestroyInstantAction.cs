using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DiscoBallDestroyInstantAction : BoardAction
	{
		private sealed class _003CDestroyAnimation_003Ed__8 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DiscoBallDestroyInstantAction _003C_003E4__this;

			private List<Slot> _003CaffectedSlots_003E5__2;

			private DiscoChipAffector _003CchipAffector_003E5__3;

			private float _003Ctime_003E5__4;

			private float _003Cduration_003E5__5;

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
			public _003CDestroyAnimation_003Ed__8(int _003C_003E1__state)
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
				DiscoBallDestroyInstantAction discoBallDestroyInstantAction = _003C_003E4__this;
				bool isHavingCarpet;
				SlotDestroyParams slotDestroyParams;
				bool flag;
				List<Slot> list;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003CaffectedSlots_003E5__2 = discoBallDestroyInstantAction.discoParams.affectedSlotsList;
					_003CchipAffector_003E5__3 = new DiscoChipAffector();
					_003CchipAffector_003E5__3.Init(discoBallDestroyInstantAction.discoParams.originSlot, discoBallDestroyInstantAction.discoParams.game, discoBallDestroyInstantAction.discoParams.affectedSlotsList, discoBallDestroyInstantAction.discoParams.itemColor);
					_003Ctime_003E5__4 = 0f;
					_003Cduration_003E5__5 = 1f;
					goto IL_00c9;
				case 1:
					_003C_003E1__state = -1;
					_003CchipAffector_003E5__3.Update();
					goto IL_00c9;
				case 2:
					{
						_003C_003E1__state = -1;
						discoBallDestroyInstantAction.isAlive = false;
						return false;
					}
					IL_00c9:
					if (_003Ctime_003E5__4 < _003Cduration_003E5__5)
					{
						_003Ctime_003E5__4 += discoBallDestroyInstantAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					_003CchipAffector_003E5__3.Clear();
					isHavingCarpet = discoBallDestroyInstantAction.discoParams.isHavingCarpet;
					discoBallDestroyInstantAction.slotLock.UnlockAll();
					discoBallDestroyInstantAction.stopGeneratorsLock.UnlockAll();
					slotDestroyParams = new SlotDestroyParams();
					slotDestroyParams.isHitByBomb = true;
					slotDestroyParams.isHavingCarpet = isHavingCarpet;
					slotDestroyParams.isBombAllowingNeighbourDestroy = true;
					slotDestroyParams.bombType = ChipType.DiscoBall;
					flag = !discoBallDestroyInstantAction.discoParams.replaceWithBombs;
					list = new List<Slot>();
					list.AddRange(_003CaffectedSlots_003E5__2);
					list.Add(discoBallDestroyInstantAction.discoParams.originSlot);
					for (int i = 0; i < list.Count; i++)
					{
						Slot slot = list[i];
						if (slot == null)
						{
							continue;
						}
						slot.OnDestroySlot(slotDestroyParams);
						discoBallDestroyInstantAction.AffectOuterCircleWithExplosion(slot.position, 1, Match3Settings.instance.discoBallDestroyActionSettings.shockWaveOffset);
						if (flag)
						{
							List<Slot> neigbourSlots = slot.neigbourSlots;
							for (int j = 0; j < neigbourSlots.Count; j++)
							{
								neigbourSlots[j].OnDestroyNeighbourSlot(slot, slotDestroyParams);
							}
						}
					}
					if (discoBallDestroyInstantAction.originChip != null)
					{
						CollectPointsAction.OnChipDestroy(discoBallDestroyInstantAction.originChip, slotDestroyParams);
						DestroyChipActionGrow destroyChipActionGrow = new DestroyChipActionGrow();
						destroyChipActionGrow.Init(discoBallDestroyInstantAction.originChip, discoBallDestroyInstantAction.originChip.lastConnectedSlot);
						discoBallDestroyInstantAction.discoParams.game.board.actionManager.AddAction(destroyChipActionGrow);
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
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

		private DiscoBallDestroyAction.DiscoParams discoParams;

		private Lock slotLock;

		private Lock stopGeneratorsLock;

		private IEnumerator destroyAnimation;

		private Chip originChip;

		private Slot otherBombSlot;

		private float deltaTime;

		public void Init(DiscoBallDestroyAction.DiscoParams discoParams)
		{
			this.discoParams = discoParams;
			slotLock = lockContainer.NewLock();
			slotLock.SuspendAll();
			slotLock.LockSlot(discoParams.originSlot);
			stopGeneratorsLock = lockContainer.NewLock();
			stopGeneratorsLock.isChipGeneratorSuspended = true;
			discoParams.game.AddLockToAllSlots(stopGeneratorsLock);
			originChip = discoParams.originBomb;
			if (originChip != null)
			{
				originChip.RemoveFromSlot();
			}
			if (discoParams.otherBomb != null)
			{
				discoParams.otherBomb.RemoveFromGame();
				otherBombSlot = discoParams.otherBomb.lastConnectedSlot;
				slotLock.LockSlot(otherBombSlot);
			}
			Match3Game game = discoParams.game;
			slotLock.LockSlots(discoParams.affectedSlotsList);
		}

		private IEnumerator DestroyAnimation()
		{
			return new _003CDestroyAnimation_003Ed__8(0)
			{
				_003C_003E4__this = this
			};
		}

		private void AffectOuterCircleWithExplosion(IntVector2 center, int radius, float shockWaveOffset)
		{
			Match3Game game = discoParams.game;
			Vector3 b = game.LocalPositionOfCenter(center);
			for (int i = center.x - radius; i <= center.x + radius; i++)
			{
				for (int j = center.y - radius; j <= center.y + radius; j++)
				{
					int a = Mathf.Abs(center.x - i);
					int b2 = Mathf.Abs(center.y - j);
					if (Mathf.Max(a, b2) == radius)
					{
						Slot slot = game.board.GetSlot(new IntVector2(i, j));
						if (slot != null)
						{
							slot.offsetPosition = (slot.localPositionOfCenter - b).normalized * shockWaveOffset;
							slot.positionIntegrator.currentPosition = slot.offsetPosition;
						}
					}
				}
			}
		}

		public override void OnUpdate(float deltaTime)
		{
			this.deltaTime = deltaTime;
			base.OnUpdate(deltaTime);
			if (destroyAnimation == null)
			{
				destroyAnimation = DestroyAnimation();
			}
			destroyAnimation.MoveNext();
		}
	}
}
