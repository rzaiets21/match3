using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class ListSlotsProvider : TilesSlotsProvider
	{
		public Match3Game game;

		public List<Slot> allSlots = new List<Slot>();

		public override int MaxSlots => allSlots.Count;

		public bool ContainsPosition(IntVector2 position)
		{
			for (int i = 0; i < allSlots.Count; i++)
			{
				if (allSlots[i].position == position)
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			allSlots.Clear();
		}

		public void AddSlot(Slot slot)
		{
			allSlots.Add(slot);
		}

		public void Init(Match3Game game)
		{
			this.game = game;
		}

		public override Vector2 StartPosition(float slotSize)
		{
			return new Vector2((float)(-game.board.size.x) * slotSize * 0.5f, (float)(-game.board.size.y) * slotSize * 0.5f);
		}

		public override Slot GetSlot(IntVector2 position)
		{
			for (int i = 0; i < allSlots.Count; i++)
			{
				Slot slot = allSlots[i];
				if (slot.position == position)
				{
					return slot;
				}
			}
			Slot result = default(Slot);
			result.position = position;
			return result;
		}

		public override List<Slot> GetAllSlots()
		{
			return allSlots;
		}
	}
}
