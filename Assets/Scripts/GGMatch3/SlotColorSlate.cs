namespace GGMatch3
{
	public class SlotColorSlate : SlotComponent
	{
		private int level = 1;

		private MultiLayerItemBehaviour itemBehaviour => GetComponentBehaviour<MultiLayerItemBehaviour>();

		public override bool isBlockingChip => false;

		public override bool isDestroyedByMatchingNextTo => false;

		public override bool isBlockingBurriedElement => true;

		public override bool isBlockingCarpetSpread => true;

		public override int blockerLevel => level;

		public int _sortingOrder = 5;
		public override int sortingOrder => _sortingOrder;

		public override bool isMovingWithConveyor => false;

		public void Init(int level)
		{
			this.level = level;
		}

		private void DestroyLayer(SlotDestroyParams destroyParams)
		{
			Match3Game game = lastConnectedSlot.game;
			level--;
			if (!game.board.burriedElements.ContainsPosition(slot.position))
			{
				level = 0;
			}
			MultiLayerItemBehaviour itemBehaviour = this.itemBehaviour;
			if (itemBehaviour != null)
			{
				itemBehaviour.SetLayerIndex(level - 1);
			}
			if (slot != null && level > 0)
			{
				game.particles.CreateParticles(slot.localPositionOfCenter, Match3Particles.PositionType.BoxDestroy, ChipType.PickupGrass, ItemColor.Unknown);
			}
			GGSoundSystem.PlayParameters sound = default(GGSoundSystem.PlayParameters);
			sound.soundType = GGSoundSystem.SFXType.BreakColorSlate;
			sound.variationIndex = level;
			game.Play(sound);
			if (level <= 0)
			{
				Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
				chipTypeDef.chipType = ChipType.PickupGrass;
				chipTypeDef.itemColor = ItemColor.Unknown;
				Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
				if (activeGoal != null)
				{
					destroyParams.goalsCollected++;
					game.OnPickupGoal(new GoalCollectParams(activeGoal, destroyParams));
				}
				game.board.burriedElements.OnSlateDestroyed(slot, destroyParams);
				RemoveFromGame();
			}
		}

		public override void AddToGoalsAtStart(Match3Goals goals)
		{
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = ChipType.PickupGrass;
			chipTypeDef.itemColor = ItemColor.Unknown;
			goals.GetChipTypeCounter(chipTypeDef).countAtStart++;
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			if (level <= 0)
			{
				return false;
			}
			return chipTypeDef.chipType == ChipType.PickupGrass;
		}

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (level <= 0)
			{
				return result;
			}
			if (!isDestroyedByMatchingNextTo)
			{
				return result;
			}
			result.isDestroyed = true;
			result.stopPropagation = true;
			DestroyLayer(destroyParams);
			return result;
		}

		public override SlotDestroyResolution OnDestroySlotComponent(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (level <= 0)
			{
				return result;
			}
			result.isDestroyed = true;
			result.stopPropagation = true;
			DestroyLayer(destroyParams);
			return result;
		}
	}
}
