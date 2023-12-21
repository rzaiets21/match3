using System;

namespace GGMatch3
{
	public class BasketBlocker : SlotComponent
	{
		public struct InitProperties
		{
			public int level;

			public int sortingOrder;

			public bool canFallthroughPickup;
		}

		[Serializable]
		public class Settings
		{
			public float lightDuration = 0.5f;
		}

		private int level;

		private InitProperties initProperties;

		private Settings settings => Match3Settings.instance.basketBlockerSettings;

		private MultiLayerItemBehaviour itemBehaviour => GetComponentBehaviour<MultiLayerItemBehaviour>();

		private TransformBehaviour transformBehaviour => GetComponentBehaviour<TransformBehaviour>();

		public override bool isMovingElementRequired => true;

		public override int sortingOrder => initProperties.sortingOrder;

		public override bool isBlockingChip => true;

		public override int blockerLevel => level;

		public override bool isDestroyedByMatchingNextTo => true;

		public override bool isSlotMatchingSuspended => true;

		public override bool isMoveIntoSlotSuspended => true;

		public override bool isSlotGravitySuspended => false;

		public override bool isMovedByGravity => true;

		public override bool isSlotSwapSuspended => true;

		public override bool isPreventingGravity => false;

		public override bool isAttachGrowingElementSuspended => true;

		public override bool isPowerupReplacementSuspended => true;

		public override bool isCreatePowerupWithThisSlotSuspended => false;

		public override bool isMovingWithConveyor => true;

		public void Init(InitProperties initProperties)
		{
			this.initProperties = initProperties;
			level = initProperties.level;
		}

		public override bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			return true;
		}

		private TransformBehaviour GetTransformBehaviour()
		{
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			if (slotComponent != null)
			{
				return slotComponent.GetComponentBehaviour<TransformBehaviour>();
			}
			return slot.GetSlotComponent<MovingElement>()?.GetComponentBehaviour<TransformBehaviour>();
		}

		public override void OnSlotComponentMadeAStartMove(SlotStartMoveParams moveParams)
		{
			LightSlotComponent backLight = moveParams.fromSlot.backLight;
			backLight.AddLightWithDuration(backLight.maxIntensity, settings.lightDuration);
		}

		public override void OnSlotComponentMadeATransformChange(SlotComponent component)
		{
			if (slot == null)
			{
				return;
			}
			TransformBehaviour componentBehaviour = component.GetComponentBehaviour<TransformBehaviour>();
			if (!(componentBehaviour == null))
			{
				TransformBehaviour transformBehaviour = this.transformBehaviour;
				if (!(transformBehaviour == null))
				{
					transformBehaviour.localPosition = componentBehaviour.localPosition;
					transformBehaviour.slotOffsetPosition = componentBehaviour.slotOffsetPosition;
				}
			}
		}

		public override void OnUpdate(float deltaTime)
		{
			if (slot == null)
			{
				return;
			}
			TransformBehaviour transformBehaviour = GetTransformBehaviour();
			if (!(transformBehaviour == null))
			{
				TransformBehaviour transformBehaviour2 = this.transformBehaviour;
				if (!(transformBehaviour2 == null))
				{
					transformBehaviour2.localPosition = transformBehaviour.localPosition;
					transformBehaviour2.slotOffsetPosition = transformBehaviour.slotOffsetPosition;
				}
			}
		}

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (destroyParams.isHitByBomb && !destroyParams.isBombAllowingNeighbourDestroy)
			{
				return result;
			}
			result.isDestroyed = true;
			result.stopPropagation = true;
			return DestroyLayer(destroyParams);
		}

		public override SlotDestroyResolution OnDestroySlotComponent(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.isDestroyed = true;
			if (destroyParams.isFromTap)
			{
				result.stopPropagation = true;
				return result;
			}
			result.stopPropagation = true;
			result.isNeigbourDestroySuspendedForThisChipOnly = true;
			return DestroyLayer(destroyParams);
		}

		private SlotDestroyResolution DestroyLayer(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.isDestroyed = true;
			result.isNeigbourDestroySuspendedForThisChipOnly = true;
			result.stopPropagation = true;
			Match3Game game = lastConnectedSlot.game;
			GGSoundSystem.PlayParameters sound = default(GGSoundSystem.PlayParameters);
			sound.soundType = GGSoundSystem.SFXType.ChocolateBreak;
			sound.variationIndex = level;
			game.Play(sound);
			level--;
			MultiLayerItemBehaviour itemBehaviour = this.itemBehaviour;
			if (itemBehaviour != null)
			{
				itemBehaviour.SetLayerIndex(level - 1);
			}
			if (slot != null)
			{
				game.particles.CreateParticles(slot.localPositionOfCenter, Match3Particles.PositionType.BoxDestroy, ChipType.BasketBlocker, ItemColor.Unknown);
			}
			if (level > 0)
			{
				return result;
			}
			Chip chip = null;
			if (slot != null)
			{
				chip = slot.GetSlotComponent<Chip>();
			}
			if (initProperties.canFallthroughPickup && chip != null && chip.isPickupElement)
			{
				result.stopPropagation = false;
			}
			RemoveFromGame();
			return result;
		}
	}
}
