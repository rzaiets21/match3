using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class CarpetSpread
	{
		public struct SlotData
		{
			public IntVector2 position;

			public bool hasCarpet;

			public bool isCarpetPossible;

			public bool isCarpetSuspended => !isCarpetPossible;
		}

		private int slotsWithCarpet;

		public SlotData[] slots;

		private CarpetSpreadBehaviour carpetBehaviour;

		private Match3Game game;

		public bool isCarpetPossible => slotsWithCarpet > 0;

		public bool HasCarpet(IntVector2 position)
		{
			if (position.x >= game.board.size.x || position.x < 0 || position.y >= game.board.size.y || position.y < 0)
			{
				return false;
			}
			return slots[game.board.Index(position)].hasCarpet;
		}

		public void AddToGoalsAtStart(Match3Goals goals)
		{
			if (slotsWithCarpet == 0)
			{
				return;
			}
			Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
			chipTypeDef.chipType = ChipType.Carpet;
			chipTypeDef.itemColor = ItemColor.Unknown;
			Match3Goals.ChipTypeCounter chipTypeCounter = goals.GetChipTypeCounter(chipTypeDef);
			for (int i = 0; i < slots.Length; i++)
			{
				SlotData slotData = slots[i];
				if (!slotData.isCarpetSuspended && !slotData.hasCarpet)
				{
					chipTypeCounter.countAtStart++;
				}
			}
		}

		public void Init(Match3Game game, LevelDefinition level)
		{
			this.game = game;
			slots = new SlotData[game.board.slots.Length];
			List<LevelDefinition.SlotDefinition> list = level.slots;
			for (int i = 0; i < slots.Length; i++)
			{
				ref SlotData reference = ref slots[i];
				slots[i].position = game.board.PositionFromIndex(i);
			}
			for (int j = 0; j < list.Count; j++)
			{
				LevelDefinition.SlotDefinition slotDefinition = list[j];
				if (!slotDefinition.isPartOfConveyor)
				{
					if (slotDefinition.hasCarpet)
					{
						AddCarpetToSlot(slotDefinition.position);
					}
					else if (slotDefinition.slotType == SlotType.PlayingSpace)
					{
						slots[game.board.Index(slotDefinition.position)].isCarpetPossible = true;
					}
				}
			}
			carpetBehaviour = game.CreateCarpetSpread();
			if (carpetBehaviour != null)
			{
				carpetBehaviour.transform.localPosition = Vector3.zero;
				carpetBehaviour.transform.localScale = Vector3.one;
				GGUtil.SetActive(carpetBehaviour, active: true);
				carpetBehaviour.Init(game, this);
				RefreshVisually();
			}
		}

		private void RefreshVisually()
		{
			if (!(carpetBehaviour == null))
			{
				carpetBehaviour.RefreshCarpet();
			}
		}

		public bool IsPossibleToAddCarpet(IntVector2 slotPosition)
		{
			if (!isCarpetPossible)
			{
				return false;
			}
			int num = game.board.Index(slotPosition);
			SlotData slotData = slots[num];
			if (slotData.hasCarpet || slotData.isCarpetSuspended)
			{
				return false;
			}
			return true;
		}

		public void AddCarpetFromGame(IntVector2 slotPosition)
		{
			int num = game.board.Index(slotPosition);
			SlotData slotData = slots[num];
			if (!slotData.hasCarpet && !slotData.isCarpetSuspended)
			{
				slots[num].hasCarpet = true;
				Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
				chipTypeDef.chipType = ChipType.Carpet;
				chipTypeDef.itemColor = ItemColor.Unknown;
				Match3Goals.GoalBase activeGoal = game.goals.GetActiveGoal(chipTypeDef);
				if (activeGoal != null)
				{
					game.OnPickupGoal(new GoalCollectParams(activeGoal, null));
				}
				RefreshVisually();
			}
		}

		private void AddCarpetToSlot(IntVector2 slotPosition)
		{
			int num = game.board.Index(slotPosition);
			SlotData slotData = slots[num];
			if (!slotData.hasCarpet)
			{
				slotData.hasCarpet = true;
				slotsWithCarpet++;
				slots[num] = slotData;
			}
		}
	}
}
