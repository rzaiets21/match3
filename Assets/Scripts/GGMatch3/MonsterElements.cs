using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class MonsterElements
	{
		public class MonsterElementPieces
		{
			public LevelDefinition.MonsterElement element;

			public List<MonsterElementSlotComponent> slotComponents = new List<MonsterElementSlotComponent>();

			public MonsterElementBehaviour elementBehaviour;

			public int collectedCount;
			public IntVector2 position;
			public IntVector2 size;

			public int collectedAnimationsStarted;

			public bool isCollected => collectedCount >= element.numberToCollect;

			public bool isMoreChipsRequiredToCollect => Mathf.Max(collectedCount, collectedAnimationsStarted) < element.numberToCollect;

			public Vector3 LocalPositionOfCenter(Match3Game game)
			{
				if (elementBehaviour != null)
				{
					return elementBehaviour.GetComponent<TransformBehaviour>().localPosition;
				}
				return Vector3.Lerp(game.LocalPositionOfCenter(element.position), game.LocalPositionOfCenter(element.oppositeCornerPosition), 0.5f);
			}

			public void OnStartCollectAnimation()
			{
				collectedAnimationsStarted++;
			}

			public void OnCollected(Match3Game game)
			{
				if (isCollected)
				{
					return;
				}
				collectedCount++;
				if (elementBehaviour != null)
				{
					elementBehaviour.SetCount(element.numberToCollect - collectedCount);
					elementBehaviour.DoEatAnimation();
					game.particles.CreateParticles(elementBehaviour.transform.localPosition, Match3Particles.PositionType.OnDestroyChip, ChipType.Chip, element.itemColor);
				}
				if (!isCollected)
				{
					return;
				}
				List<Slot> list = new List<Slot>();
				for (int i = 0; i < slotComponents.Count; i++)
				{
					MonsterElementSlotComponent monsterElementSlotComponent = slotComponents[i];
					if (monsterElementSlotComponent.lastConnectedSlot != null)
					{
						list.Add(monsterElementSlotComponent.lastConnectedSlot);
					}
					monsterElementSlotComponent.RemoveFromGame();
				}
				Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
				chipTypeDef.chipType = ChipType.MonsterBlocker;
				chipTypeDef.itemColor = element.itemColor;
				Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
				CollectGoalAction collectGoalAction = new CollectGoalAction();
				CollectGoalAction.CollectGoalParams collectParams = default(CollectGoalAction.CollectGoalParams);
				collectParams.goal = activeGoal;
				collectParams.game = game;
				collectParams.otherAffectedSlots = list;
				if (elementBehaviour != null)
				{
					collectParams.moveTransform = elementBehaviour.GetComponent<TransformBehaviour>();
					elementBehaviour.SetCount(0);
				}
				collectGoalAction.Init(collectParams);
				game.board.actionManager.AddAction(collectGoalAction);
			}
		}

		public List<MonsterElementPieces> pieces = new List<MonsterElementPieces>();

		public MonsterElementPieces GetPieceThatNeedsFeeding(Match3Goals.ChipTypeDef chipTypeDef)
		{
			if (chipTypeDef.chipType != 0)
			{
				return null;
			}
			for (int i = 0; i < pieces.Count; i++)
			{
				MonsterElementPieces monsterElementPieces = pieces[i];
				if (monsterElementPieces.isMoreChipsRequiredToCollect && monsterElementPieces.element.itemColor == chipTypeDef.itemColor)
				{
					return monsterElementPieces;
				}
			}
			return null;
		}

		public void Init(Match3Game game, LevelDefinition level)
		{
			LevelDefinition.MonsterElements monsterElements = level.monsterElements;
			for (int i = 0; i < monsterElements.elements.Count; i++)
			{
				LevelDefinition.MonsterElement monsterElement = monsterElements.elements[i];
				MonsterElementPieces monsterElementPieces = new MonsterElementPieces();
				monsterElementPieces.element = monsterElement;
				pieces.Add(monsterElementPieces);
				MonsterElementBehaviour monsterElementBehaviour = monsterElementPieces.elementBehaviour = game.CreateMonsterElementBehaviour();
				if (monsterElementBehaviour != null)
				{
					monsterElementBehaviour.Init(monsterElement);
					monsterElementBehaviour.GetComponent<TransformBehaviour>().localPosition = Vector3.Lerp(game.LocalPositionOfCenter(monsterElement.position), game.LocalPositionOfCenter(monsterElement.oppositeCornerPosition), 0.5f);
					monsterElementBehaviour.SetCount(monsterElement.numberToCollect);
				}
				for (int j = 0; j < monsterElement.size.x; j++)
				{
					for (int k = 0; k < monsterElement.size.y; k++)
					{
						Slot slot = game.GetSlot(new IntVector2(j, -k) + monsterElement.position);
						if (slot != null)
						{
							MonsterElementSlotComponent monsterElementSlotComponent = new MonsterElementSlotComponent();
							slot.AddComponent(monsterElementSlotComponent);
							monsterElementPieces.slotComponents.Add(monsterElementSlotComponent);
						}
					}
				}
			}
		}

		public void AddToGoalsAtStart(Match3Goals goals)
		{
			for (int i = 0; i < pieces.Count; i++)
			{
				MonsterElementPieces monsterElementPieces = pieces[i];
				Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
				chipTypeDef.chipType = ChipType.MonsterBlocker;
				chipTypeDef.itemColor = monsterElementPieces.element.itemColor;
				goals.GetChipTypeCounter(chipTypeDef).countAtStart++;
			}
		}

		public bool IsCompatibleWithPickupGoal(Slot slot, Match3Goals.ChipTypeDef chipTypeDef)
		{
			if (chipTypeDef.chipType != ChipType.MonsterBlocker)
			{
				return false;
			}
			Match3Goals.ChipTypeDef chipTypeDef2 = default(Match3Goals.ChipTypeDef);
			chipTypeDef2.chipType = ChipType.Chip;
			chipTypeDef2.itemColor = chipTypeDef.itemColor;
			for (int i = 0; i < pieces.Count; i++)
			{
				MonsterElementPieces monsterElementPieces = pieces[i];
				if (monsterElementPieces.isMoreChipsRequiredToCollect && monsterElementPieces.element.itemColor == chipTypeDef.itemColor)
				{
					Chip slotComponent = slot.GetSlotComponent<Chip>();
					if (slotComponent != null && slotComponent.IsCompatibleWithPickupGoal(chipTypeDef2))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
