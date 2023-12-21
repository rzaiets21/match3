using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class LevelDefinitionTilesSlotsProvider : TilesSlotsProvider
	{
		public LevelDefinition level;

		private List<Slot> allSlots = new List<Slot>();

		public override int MaxSlots => level.size.width * level.size.height;

		public LevelDefinitionTilesSlotsProvider(LevelDefinition level)
		{
			this.level = level;
		}

		public override Vector2 StartPosition(float slotSize)
		{
			return new Vector2((float)(-level.size.width) * slotSize * 0.5f, (float)(-level.size.height) * slotSize * 0.5f);
		}

		public override Slot GetSlot(IntVector2 position)
		{
			Slot result = default(Slot);
			LevelDefinition.SlotDefinition slot = level.GetSlot(position);
			result.position = position;
			if (slot != null)
			{
				result.isOccupied = IsOccupied(slot);
			}
			return result;
		}

		private bool IsOccupied(LevelDefinition.SlotDefinition levelSlot)
		{
			return levelSlot.slotType == SlotType.PlayingSpace;
		}

		public override List<Slot> GetAllSlots()
		{
			allSlots.Clear();
			for (int i = 0; i < level.slots.Count; i++)
			{
				LevelDefinition.SlotDefinition slotDefinition = level.slots[i];
				Slot item = default(Slot);
				item.position = slotDefinition.position;
				item.isOccupied = IsOccupied(slotDefinition);
				allSlots.Add(item);
			}
			return allSlots;
		}
	}
}
