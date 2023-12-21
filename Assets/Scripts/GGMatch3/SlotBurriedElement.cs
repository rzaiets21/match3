namespace GGMatch3
{
	public class SlotBurriedElement : SlotComponent
	{
		public LevelDefinition.BurriedElement elementDefinition;

		public override bool isBlockingChip => false;

		public override bool isDestroyedByMatchingNextTo => false;

		public override int blockerLevel => 1;

		public override int sortingOrder => 2;

		public BurriedElementBehaviour burriedElementBehaviour
		{
			get
			{
				TransformBehaviour componentBehaviour = GetComponentBehaviour<TransformBehaviour>();
				if (componentBehaviour == null)
				{
					return null;
				}
				return componentBehaviour.GetComponent<BurriedElementBehaviour>();
			}
		}

		public override bool isMovingWithConveyor => true;

		public void Init(LevelDefinition.BurriedElement elementDefinition)
		{
			this.elementDefinition = elementDefinition;
		}

		public override void AddToGoalsAtStart(Match3Goals goals)
		{
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = ChipType.BurriedElement;
			chipTypeDef.itemColor = ItemColor.Unknown;
			goals.GetChipTypeCounter(chipTypeDef).countAtStart++;
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			return chipTypeDef.chipType == ChipType.BurriedElement;
		}

		public override SlotDestroyResolution OnDestroySlotComponent(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (slot.isBlockingBurriedElement)
			{
				return result;
			}
			result.isDestroyed = true;
			result.stopPropagation = false;
			Match3Game game = lastConnectedSlot.game;
			slot.RemoveComponent(this);
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = ChipType.BurriedElement;
			chipTypeDef.itemColor = ItemColor.Unknown;
			Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
			GoalCollectParams goalCollectParams = default(GoalCollectParams);
			goalCollectParams.goal = activeGoal;
			CollectBurriedElementAction.CollectGoalParams collectParams = default(CollectBurriedElementAction.CollectGoalParams);
			collectParams.slotBurriedElement = this;
			collectParams.destroyParams = destroyParams;
			collectParams.game = game;
			collectParams.goal = activeGoal;
			collectParams.slotToLock = lastConnectedSlot;
			if (destroyParams != null)
			{
				collectParams.explosionCentre = destroyParams.explosionCentre;
			}
			CollectBurriedElementAction collectBurriedElementAction = new CollectBurriedElementAction();
			collectBurriedElementAction.Init(collectParams);
			game.board.actionManager.AddAction(collectBurriedElementAction);
			return result;
		}
	}
}
