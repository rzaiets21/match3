using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class ExtraFallingChips
	{
		public class FallingChipPickup
		{
			public int chipTag;
		}

		public class FallingChipCreation
		{
			public Slot slot;

			public int userMovesCount;

			public int slotCreatedChipsNum;

			public int chipTag;
		}

		public ExtraFallingElements setup;

		private List<FallingChipCreation> createdList = new List<FallingChipCreation>();

		private List<FallingChipPickup> pickedUpList = new List<FallingChipPickup>();

		private int generatedFallingChips => createdList.Count;

		public int LastSlotCreatedChipsNum(Slot slot)
		{
			int num = -1;
			for (int i = 0; i < createdList.Count; i++)
			{
				FallingChipCreation fallingChipCreation = createdList[i];
				if (fallingChipCreation.slot == slot)
				{
					num = Mathf.Max(fallingChipCreation.slotCreatedChipsNum, num);
				}
			}
			return num;
		}

		public void Init(ExtraFallingElements extraFallingElements)
		{
			setup = extraFallingElements;
		}

		private int GeneratedFallingChipsForTag(int chipTag)
		{
			int num = 0;
			for (int i = 0; i < createdList.Count; i++)
			{
				if (createdList[i].chipTag == chipTag)
				{
					num++;
				}
			}
			return num;
		}

		private int PickedUpChipsForTag(int chipTag)
		{
			int num = 0;
			for (int i = 0; i < pickedUpList.Count; i++)
			{
				if (pickedUpList[i].chipTag == chipTag)
				{
					num++;
				}
			}
			return num;
		}

		public bool ShouldGenerateExtraFallingChip(Slot slot)
		{
			if (setup == null)
			{
				return false;
			}
			Match3Game game = slot.game;
			int chipTag = slot.generatorSettings.chipTag;
			if (PickedUpChipsForTag(chipTag) <= GeneratedFallingChipsForTag(chipTag))
			{
				return false;
			}
			if (generatedFallingChips >= setup.fallingElementsList.Count)
			{
				return false;
			}
			ExtraFallingElements.ExtraFallingElement extraFallingElement = setup.fallingElementsList[generatedFallingChips];
			if (extraFallingElement.minMovesBeforeActive > game.board.userMovesCount)
			{
				return false;
			}
			int num = LastSlotCreatedChipsNum(slot);
			if (slot.createdChips - num < extraFallingElement.minCreatedChipsBeforeReactivate)
			{
				return false;
			}
			return true;
		}

		public void OnExtraFallingChipGenerated(Slot slot)
		{
			Match3Game game = slot.game;
			FallingChipCreation fallingChipCreation = new FallingChipCreation();
			fallingChipCreation.userMovesCount = game.board.userMovesCount;
			fallingChipCreation.slot = slot;
			fallingChipCreation.slotCreatedChipsNum = slot.createdChips;
			fallingChipCreation.chipTag = slot.generatorSettings.chipTag;
			createdList.Add(fallingChipCreation);
		}

		public void OnFallingElementPickup(Chip chip)
		{
			FallingChipPickup fallingChipPickup = new FallingChipPickup();
			fallingChipPickup.chipTag = chip.chipTag;
			pickedUpList.Add(fallingChipPickup);
		}
	}
}
