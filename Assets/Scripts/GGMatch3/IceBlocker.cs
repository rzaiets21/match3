namespace GGMatch3
{
	public class IceBlocker : SlotComponent
	{
		public struct InitProperties
		{
			public int level;

			public int sortingOrder;

			public Chip chip;
		}

		private int level;

		private InitProperties initProperties;

		private WobbleAnimation wobbleAnimation = new WobbleAnimation();

		private MultiLayerItemBehaviour itemBehaviour => GetComponentBehaviour<MultiLayerItemBehaviour>();

		private TransformBehaviour transformBehaviour => GetComponentBehaviour<TransformBehaviour>();

		private IceBehaviour iceBehaviour
		{
			get
			{
				TransformBehaviour transformBehaviour = this.transformBehaviour;
				if (transformBehaviour == null)
				{
					return null;
				}
				return transformBehaviour.GetComponent<IceBehaviour>();
			}
		}

		public override int sortingOrder => initProperties.sortingOrder;

		public override bool isBlockingChip => true;

		public override int blockerLevel => level;

		public override bool isSlotMatchingSuspended => false;

		public override bool isMoveIntoSlotSuspended => false;

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
			IceBehaviour iceBehaviour = this.iceBehaviour;
			if (iceBehaviour != null)
			{
				iceBehaviour.Init(initProperties.chip, level);
			}
		}

		public override bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			if (replaceWithBombs)
			{
				return true;
			}
			return false;
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
			wobbleAnimation.Update(deltaTime);
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			IceBehaviour iceBehaviour = this.iceBehaviour;
			if (iceBehaviour != null)
			{
				iceBehaviour.TryInitIfDifferent(slotComponent, level);
			}
			if (slotComponent == null)
			{
				return;
			}
			TransformBehaviour componentBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
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

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.stopPropagation = true;
			return result;
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
			level--;
			MultiLayerItemBehaviour itemBehaviour = this.itemBehaviour;
			if (itemBehaviour != null)
			{
				itemBehaviour.SetLayerIndex(level - 1);
			}
			game.Play(GGSoundSystem.SFXType.BreakIce);
			if (slot != null)
			{
				game.particles.CreateParticles(slot.localPositionOfCenter, Match3Particles.PositionType.BoxDestroy, ChipType.IceOnChip, ItemColor.Unknown);
				slot.Wobble(Match3Settings.instance.chipWobbleSettings);
			}
			IceBehaviour iceBehaviour = this.iceBehaviour;
			if (level > 0)
			{
				if (iceBehaviour != null)
				{
					iceBehaviour.Init(slot.GetSlotComponent<Chip>(), level);
				}
				return result;
			}
			if (iceBehaviour != null)
			{
				iceBehaviour.DoOnDestroy(lastConnectedSlot.GetSlotComponent<Chip>());
			}
			RemoveFromGame();
			return result;
		}

		public override void Wobble(WobbleAnimation.Settings settings)
		{
			if (settings != null)
			{
				wobbleAnimation.Init(settings, transformBehaviour);
			}
		}
	}
}
