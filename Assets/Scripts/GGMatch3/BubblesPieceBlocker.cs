namespace GGMatch3
{
	public class BubblesPieceBlocker : SlotComponent
	{
		public override bool isBlockingBurriedElement => true;

		public override bool isBlockingCarpetSpread => true;

		public override bool isAttachGrowingElementSuspended => true;

		public override int sortingOrder => 150;

		public override bool isBlockingChip => true;

		public override int blockerLevel => 1;

		public override bool isSlotMatchingSuspended => true;

		public override bool isMoveIntoSlotSuspended => true;

		public override bool isSlotGravitySuspended => true;

		public override bool isSlotSwapSuspended => true;

		public override bool isPlaceBubbleSuspended => true;

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
			SlotDestroyResolution slotDestroyResolution = default(SlotDestroyResolution);
			slotDestroyResolution.isDestroyed = true;
			return DestroyLayer(destroyParams);
		}

		private SlotDestroyResolution DestroyLayer(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.isDestroyed = true;
			result.isNeigbourDestroySuspendedForThisChipOnly = true;
			result.stopPropagation = true;
			Match3Game game = lastConnectedSlot.game;
			game.board.bubblesBoardComponent.OnBubbleBurst(this);
			if (slot != null)
			{
				game.particles.CreateParticles(slot.localPositionOfCenter, Match3Particles.PositionType.BubblesDestroy);
			}
			game.Play(GGSoundSystem.SFXType.SnowDestroy);
			RemoveFromGame();
			return result;
		}
	}
}
