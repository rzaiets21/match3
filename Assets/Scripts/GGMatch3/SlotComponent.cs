using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class SlotComponent
	{
		public struct SlotStartMoveParams
		{
			public Slot fromSlot;

			public Slot toSlot;

			public SlotComponent slotComponent;
		}

		public Slot slot;

		public Slot lastConnectedSlot;

		public bool isRemovedFromGame;

		private readonly float _003ClastMoveTime_003Ek__BackingField;

		protected List<SlotComponentBehavoiour> monoBehaviours = new List<SlotComponentBehavoiour>();

		public List<SlotComponentLock> slotComponentLocks = new List<SlotComponentLock>();

		private readonly bool _003CisSlotSwapSuspended_003Ek__BackingField;

		private readonly bool _003CisSlotTapSuspended_003Ek__BackingField;

		private readonly bool _003CisSlotMatchingSuspended_003Ek__BackingField;

		private readonly bool _003CisSlotGravitySuspended_003Ek__BackingField;

		private readonly bool _003CisPlaceBubbleSuspended_003Ek__BackingField;

		private readonly bool _003CisMoving_003Ek__BackingField;

		private readonly bool _003CisMoveIntoSlotSuspended_003Ek__BackingField;

		private readonly int _003CblockerLevel_003Ek__BackingField;

		private readonly bool _003CisBlockingBurriedElement_003Ek__BackingField;

		private readonly bool _003CisBlockingCarpetSpread_003Ek__BackingField;

		private readonly bool _003CisBlockingChip_003Ek__BackingField;

		private readonly bool _003CisMoveByConveyorSuspended_003Ek__BackingField;

		private readonly bool _003CcanReactWithBomb_003Ek__BackingField;

		private readonly bool _003CisCreatePowerupWithThisSlotSuspended_003Ek__BackingField;

		private readonly bool _003CisMovingWithConveyor_003Ek__BackingField;

		private readonly bool _003CisMovedByGravity_003Ek__BackingField;

		private readonly bool _003CisPreventingGravity_003Ek__BackingField;

		private readonly bool _003CisPreventingOtherChipsToFallIntoSlot_003Ek__BackingField;

		private readonly bool _003CisPreventingReplaceByOtherChips_003Ek__BackingField;

		public virtual int sortingOrder => 0;

		public virtual bool isMovingElementRequired => false;

		public virtual long lastMoveFrameIndex => 0L;

		public virtual float lastMoveTime => _003ClastMoveTime_003Ek__BackingField;

		public bool isRemoveFromGameDestroySuspended
		{
			get
			{
				for (int i = 0; i < slotComponentLocks.Count; i++)
				{
					if (slotComponentLocks[i].isRemoveFromGameDestroySuspended)
					{
						return true;
					}
				}
				return false;
			}
		}

		public virtual bool isSlotSwapSuspended => _003CisSlotSwapSuspended_003Ek__BackingField;

		public virtual bool isSlotTapSuspended => _003CisSlotTapSuspended_003Ek__BackingField;

		public virtual bool isSlotMatchingSuspended => _003CisSlotMatchingSuspended_003Ek__BackingField;

		public virtual bool isSlotGravitySuspended => _003CisSlotGravitySuspended_003Ek__BackingField;

		public virtual bool isPlaceBubbleSuspended => _003CisPlaceBubbleSuspended_003Ek__BackingField;

		public virtual bool isMoving => _003CisMoving_003Ek__BackingField;

		public virtual bool isMoveIntoSlotSuspended => _003CisMoveIntoSlotSuspended_003Ek__BackingField;

		public virtual int blockerLevel => _003CblockerLevel_003Ek__BackingField;

		public virtual bool isBlockingBurriedElement => _003CisBlockingBurriedElement_003Ek__BackingField;

		public virtual bool isBlockingCarpetSpread => _003CisBlockingCarpetSpread_003Ek__BackingField;

		public virtual bool isBlockingChip => _003CisBlockingChip_003Ek__BackingField;

		public virtual bool isMoveByConveyorSuspended => _003CisMoveByConveyorSuspended_003Ek__BackingField;

		public virtual bool canReactWithBomb => _003CcanReactWithBomb_003Ek__BackingField;

		public virtual bool isAttachGrowingElementSuspended => false;

		public virtual bool isPowerupReplacementSuspended => false;

		public virtual bool isCreatePowerupWithThisSlotSuspended => _003CisCreatePowerupWithThisSlotSuspended_003Ek__BackingField;

		public virtual bool isMovingWithConveyor => _003CisMovingWithConveyor_003Ek__BackingField;

		public virtual bool isMovedByGravity => _003CisMovedByGravity_003Ek__BackingField;

		public virtual bool isDestroyedByMatchingNextTo => false;

		public virtual bool isPreventingGravity => _003CisPreventingGravity_003Ek__BackingField;

		public virtual bool isPreventingOtherChipsToFallIntoSlot => _003CisPreventingOtherChipsToFallIntoSlot_003Ek__BackingField;

		public virtual bool isPreventingReplaceByOtherChips => _003CisPreventingReplaceByOtherChips_003Ek__BackingField;

		public virtual void OnSlotComponentMadeAStartMove(SlotStartMoveParams moveParams)
		{
		}

		public virtual void OnSlotComponentMadeATransformChange(SlotComponent component)
		{
		}

		public virtual void AddToGoalsAtStart(Match3Goals goals)
		{
		}

		public virtual SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.isDestroyed = false;
			return result;
		}

		public virtual SlotDestroyResolution OnDestroySlotComponent(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.isDestroyed = false;
			return result;
		}

		public void RemoveFromSlot()
		{
			if (slot != null)
			{
				slot.RemoveComponent(this);
			}
		}

		public void RemoveFromGame()
		{
			isRemovedFromGame = true;
			RemoveFromSlot();
			if (isRemoveFromGameDestroySuspended)
			{
				UnityEngine.Debug.LogError("REMOVING FROM GAME SOMETHING THAT IS LOCKED!");
				return;
			}
			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				monoBehaviours[i].RemoveFromGame();
			}
			monoBehaviours.Clear();
		}

		public void Add(SlotComponentBehavoiour beh)
		{
			if (!(beh == null))
			{
				beh.OnAddedToSlotComponent(this);
				monoBehaviours.Add(beh);
			}
		}

		public T GetComponentBehaviour<T>() where T : SlotComponentBehavoiour
		{
			for (int i = 0; i < monoBehaviours.Count; i++)
			{
				T val = monoBehaviours[i] as T;
				if ((Object)val != (Object)null)
				{
					return val;
				}
			}
			return null;
		}

		public void AddLock(SlotComponentLock slotComponentLock)
		{
			if (slotComponentLock != null && !slotComponentLocks.Contains(slotComponentLock))
			{
				slotComponentLocks.Add(slotComponentLock);
			}
		}

		public void RemoveLock(SlotComponentLock slotComponentLock)
		{
			if (slotComponentLock != null)
			{
				slotComponentLocks.Remove(slotComponentLock);
			}
		}

		public virtual bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			return false;
		}

		public virtual bool isBlockingDirection(IntVector2 direction)
		{
			return false;
		}

		public virtual bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			return false;
		}

		public virtual void Wobble(WobbleAnimation.Settings settings)
		{
		}

		public virtual void OnUpdate(float deltaTime)
		{
		}

		public virtual void OnMovedBySlotGravity(Slot fromSlot, Slot toSlot, MoveContentsToSlotParams moveParams)
		{
		}

		public virtual void OnCreatedBySlot(Slot toSlot)
		{
		}

		public virtual void OnAddedToSlot(Slot slot)
		{
		}

		public virtual void OnRemovedFromSlot(Slot slot)
		{
		}
	}
}
