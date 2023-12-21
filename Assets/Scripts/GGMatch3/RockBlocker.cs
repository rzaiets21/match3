namespace GGMatch3
{
	public class RockBlocker : SlotComponent
	{
		public struct InitArguments
		{
			public int level;

			public bool cancelsSnow;

			public int sortingOrder;
		}

		private InitArguments initArguments;

		private int level;

		private WobbleAnimation wobbleAnimation = new WobbleAnimation();

		private int sortingOrder_;

		private MultiLayerItemBehaviour itemBehaviour => GetComponentBehaviour<MultiLayerItemBehaviour>();

		public override bool isBlockingBurriedElement => true;

		public override bool isBlockingCarpetSpread => true;

		public override bool isAttachGrowingElementSuspended => true;

		public override bool isPlaceBubbleSuspended => true;

		private ChipType chipType => ChipType.RockBlocker;

		public override int sortingOrder => sortingOrder_;

		public override bool isBlockingChip => true;

		public override int blockerLevel => level + 1;

		public override bool isSlotMatchingSuspended => true;

		public override bool isMoveIntoSlotSuspended => true;

		public override bool isSlotGravitySuspended => true;

		public override bool isSlotSwapSuspended => true;

		public override bool isPreventingOtherChipsToFallIntoSlot => true;

		public override bool isPreventingGravity => true;

		public override bool isPowerupReplacementSuspended => true;

		public override bool isCreatePowerupWithThisSlotSuspended => false;

		public override bool isMovingWithConveyor => true;

		public override void AddToGoalsAtStart(Match3Goals goals)
		{
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = chipType;
			chipTypeDef.itemColor = ItemColor.Unknown;
			goals.GetChipTypeCounter(chipTypeDef).countAtStart += level;
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			return chipTypeDef.chipType == chipType;
		}

		public void Init(InitArguments initArguments)
		{
			this.initArguments = initArguments;
			level = initArguments.level;
			sortingOrder_ = initArguments.sortingOrder;
		}

		public override bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			return true;
		}

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (level > 0)
			{
				result.stopPropagation = true;
				return result;
			}
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
			Match3Game game = lastConnectedSlot.game;
			if (level > 0 && !destroyParams.isHitByBomb)
			{
				if (initArguments.cancelsSnow && game != null)
				{
					game.board.bubblesBoardComponent.CancelSpread();
				}
				return result;
			}
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
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = chipType;
			chipTypeDef.itemColor = ItemColor.Unknown;
			Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
			if (activeGoal != null)
			{
				destroyParams.goalsCollected++;
				game.OnPickupGoal(new GoalCollectParams(activeGoal, destroyParams));
			}
			GGSoundSystem.PlayParameters sound = default(GGSoundSystem.PlayParameters);
			sound.soundType = GGSoundSystem.SFXType.RockBreak;
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
				game.particles.CreateParticles(slot.localPositionOfCenter, Match3Particles.PositionType.BoxDestroy, chipType, ItemColor.Unknown);
				slot.Wobble(Match3Settings.instance.chipWobbleSettings);
			}
			CollectPointsAction.OnBlockerDestroy(lastConnectedSlot, destroyParams);
			if (level >= 0)
			{
				return result;
			}
			if (slot != null)
			{
				slot.GetSlotComponent<Chip>();
			}
			if (initArguments.cancelsSnow)
			{
				game.board.bubblesBoardComponent.CancelSpread();
			}
			RemoveFromGame();
			return result;
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			wobbleAnimation.Update(deltaTime);
		}

		public override void Wobble(WobbleAnimation.Settings settings)
		{
			if (settings != null)
			{
				wobbleAnimation.Init(settings, GetComponentBehaviour<TransformBehaviour>());
			}
		}
	}
}
