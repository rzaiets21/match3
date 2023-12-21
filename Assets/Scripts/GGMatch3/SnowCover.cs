namespace GGMatch3
{
	public class SnowCover : SlotComponent
	{
		public struct InitProperties
		{
			public WobbleAnimation.Settings wobbleSettings;

			public int sortingOrder;
		}

		private InitProperties initProperties;

		public override bool isBlockingBurriedElement => true;

		public override bool isBlockingCarpetSpread => true;

		public override bool isAttachGrowingElementSuspended => true;

		public override bool isPlaceBubbleSuspended => true;

		public override int sortingOrder => initProperties.sortingOrder;

		public override bool isBlockingChip => true;

		public override int blockerLevel => 1;

		public override bool isSlotMatchingSuspended => true;

		public override bool isMoveIntoSlotSuspended => true;

		public override bool isSlotGravitySuspended => true;

		public override bool isSlotSwapSuspended => true;

		public override bool isPreventingGravity => true;

		public override bool isPowerupReplacementSuspended => true;

		public override bool isCreatePowerupWithThisSlotSuspended => false;

		public override bool isMovingWithConveyor => false;

		public override bool isDestroyedByMatchingNextTo => true;

		public override void AddToGoalsAtStart(Match3Goals goals)
		{
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			return false;
		}

		public void Init(InitProperties initProperties)
		{
			this.initProperties = initProperties;
		}

		public override bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			return true;
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
			game.Play(GGSoundSystem.SFXType.SnowDestroy);
			if (slot != null)
			{
				game.particles.CreateParticles(slot.localPositionOfCenter, Match3Particles.PositionType.BubblesDestroy);
			}
			if (slot != null)
			{
				slot.GetSlotComponent<Chip>();
			}
			if (slot != null && initProperties.wobbleSettings != null)
			{
				slot.Wobble(initProperties.wobbleSettings);
			}
			RemoveFromGame();
			return result;
		}
	}
}
