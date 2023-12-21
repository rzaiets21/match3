using UnityEngine;

namespace GGMatch3
{
	public class NetMatchOrDestroyNextToLock : SlotComponent
	{
		public struct InitProperties
		{
			public bool isMoveIntoSlotSuspended;

			public bool isSlotMatchingSuspended;

			public bool isAvailableForDiscoBombSuspended;

			public bool isBlockingBurriedElement;

			public bool isAvailableForDiscoBombReplaceBombs;

			public bool isAttachGrowingElementSuspended;

			public WobbleAnimation.Settings wobbleSettings;

			public bool isDestroyByMatchingNeighborSuspended;

			public bool canFallthroughPickup;

			public int level;

			public ChipType chipType;

			public int sortingOrder;

			private bool overrideDisplayChipType;

			private ChipType displayChipType_;

			public bool useSound;

			public GGSoundSystem.SFXType soundType;

			public ChipType displayChipType
			{
				get
				{
					if (overrideDisplayChipType)
					{
						return displayChipType_;
					}
					return chipType;
				}
			}

			public void SetDisplayChipType(ChipType type)
			{
				overrideDisplayChipType = true;
				displayChipType_ = type;
			}
		}

		private WobbleAnimation wobbleAnimation = new WobbleAnimation();

		private int level;

		private InitProperties initProperties;

		public ChipType ChipType => initProperties.chipType;
		
		private MultiLayerItemBehaviour itemBehaviour => GetComponentBehaviour<MultiLayerItemBehaviour>();

		public override bool isBlockingBurriedElement => initProperties.isBlockingBurriedElement;

		public override bool isBlockingCarpetSpread => true;

		public override bool isAttachGrowingElementSuspended => initProperties.isAttachGrowingElementSuspended;

		public override bool isPlaceBubbleSuspended => true;

		public override int sortingOrder => initProperties.sortingOrder;

		public override bool isBlockingChip => true;

		public override int blockerLevel => level;

		public override bool isSlotMatchingSuspended => initProperties.isSlotMatchingSuspended;

		public override bool isMoveIntoSlotSuspended => initProperties.isMoveIntoSlotSuspended;

		public override bool isSlotGravitySuspended => true;

		public override bool isSlotSwapSuspended => true;

		public override bool isPreventingGravity => true;

		public override bool isPowerupReplacementSuspended => true;

		public override bool isCreatePowerupWithThisSlotSuspended => false;

		public override bool isMovingWithConveyor => true;

		public override bool isDestroyedByMatchingNextTo => !initProperties.isDestroyByMatchingNeighborSuspended;

		public override void AddToGoalsAtStart(Match3Goals goals)
		{
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = initProperties.chipType;
			chipTypeDef.itemColor = ItemColor.Unknown;
			goals.GetChipTypeCounter(chipTypeDef).countAtStart += level;
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			return chipTypeDef.chipType == initProperties.chipType;
		}

		public void Init(InitProperties initProperties)
		{
			this.initProperties = initProperties;
			level = initProperties.level;
		}

		public override bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			if (replaceWithBombs)
			{
				return !initProperties.isAvailableForDiscoBombReplaceBombs;
			}
			return initProperties.isAvailableForDiscoBombSuspended;
		}

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (initProperties.isDestroyByMatchingNeighborSuspended)
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
			Match3Game game = base.lastConnectedSlot.game;
			Slot lastConnectedSlot = base.lastConnectedSlot;
			CollectPointsAction.OnBlockerDestroy(base.lastConnectedSlot, destroyParams);
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = initProperties.chipType;
			chipTypeDef.itemColor = ItemColor.Unknown;
			Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
			if (activeGoal != null)
			{
				destroyParams.goalsCollected++;
				game.OnPickupGoal(new GoalCollectParams(activeGoal, destroyParams));
			}
			if (initProperties.useSound)
			{
				GGSoundSystem.PlayParameters sound = default(GGSoundSystem.PlayParameters);
				sound.soundType = initProperties.soundType;
				sound.variationIndex = level;
				game.Play(sound);
			}
			level--;
			MultiLayerItemBehaviour itemBehaviour = this.itemBehaviour;
			if (itemBehaviour != null)
			{
				itemBehaviour.SetLayerIndex(Mathf.Max(0, level - 1));
			}
			Chip chip = null;
			if (lastConnectedSlot != null)
			{
				chip = lastConnectedSlot.GetSlotComponent<Chip>();
			}
			if (lastConnectedSlot != null && initProperties.wobbleSettings != null)
			{
				lastConnectedSlot.Wobble(initProperties.wobbleSettings);
			}
			if (level > 0)
			{
				if (slot != null)
				{
					game.particles.CreateParticles(lastConnectedSlot.localPositionOfCenter, Match3Particles.PositionType.BoxDestroy, initProperties.chipType, level);
				}
				return result;
			}
			if (initProperties.canFallthroughPickup && chip != null && chip.isPickupElement)
			{
				result.stopPropagation = false;
			}
			RemoveFromSlot();
			if (initProperties.chipType == ChipType.Box)
			{
				DestroyBoxAction.InitArguments initArguments = default(DestroyBoxAction.InitArguments);
				initArguments.slot = base.lastConnectedSlot;
				initArguments.chip = this;
				DestroyBoxAction destroyBoxAction = new DestroyBoxAction();
				destroyBoxAction.Init(initArguments);
				game.board.actionManager.AddAction(destroyBoxAction);
				return result;
			}
			if (lastConnectedSlot != null)
			{
				game.particles.CreateParticles(lastConnectedSlot.localPositionOfCenter, Match3Particles.PositionType.BoxDestroy, initProperties.chipType, level);
			}
			RemoveFromGame();
			return result;
		}

		public override void Wobble(WobbleAnimation.Settings settings)
		{
			if (settings != null)
			{
				wobbleAnimation.Init(initProperties.wobbleSettings, GetComponentBehaviour<TransformBehaviour>());
			}
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			wobbleAnimation.Update(deltaTime);
		}
	}
}
