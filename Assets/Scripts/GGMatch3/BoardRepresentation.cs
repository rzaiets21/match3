using System.Collections.Generic;

namespace GGMatch3
{
	public class BoardRepresentation
	{
		public struct RepresentationSlot
		{
			public bool canFormColorMatches;

			public IntVector2 position;

			public ItemColor itemColor;

			public bool isOutsideBoard;

			public bool canMove;

			public bool wallLeft;

			public bool wallRight;

			public bool wallUp;

			public bool wallDown;

			public bool isOutOfPlayArea;

			private bool IsBlocked(IntVector2 direction)
			{
				if (direction.x < 0 && wallLeft)
				{
					return true;
				}
				if (direction.x > 0 && wallRight)
				{
					return true;
				}
				if (direction.y < 0 && wallDown)
				{
					return true;
				}
				if (direction.y > 0 && wallUp)
				{
					return true;
				}
				return false;
			}

			public bool IsBlockedTo(RepresentationSlot slot)
			{
				IntVector2 intVector = slot.position - position;
				if (IsBlocked(intVector) || slot.IsBlocked(-intVector))
				{
					return true;
				}
				return false;
			}
		}

		public IntVector2 size;

		public List<RepresentationSlot> slots = new List<RepresentationSlot>();

		public RepresentationSlot GetSlot(IntVector2 pos)
		{
			if (pos.x < 0 || pos.y < 0 || pos.x >= size.x || pos.y >= size.y)
			{
				RepresentationSlot result = default(RepresentationSlot);
				result.position = pos;
				result.isOutsideBoard = true;
				return result;
			}
			int index = pos.x + pos.y * size.x;
			return slots[index];
		}

		public void Init(Match3Game match3Game)
		{
			slots.Clear();
			size = match3Game.board.size;
			for (int i = 0; i < size.y; i++)
			{
				for (int j = 0; j < size.x; j++)
				{
					IntVector2 intVector = new IntVector2(j, i);
					RepresentationSlot item = default(RepresentationSlot);
					item.position = intVector;
					Slot slot = match3Game.GetSlot(intVector);
					Chip chip = null;
					if (slot == null)
					{
						item.isOutOfPlayArea = true;
					}
					if (slot != null)
					{
						chip = slot.GetSlotComponent<Chip>();
						item.canMove = !slot.isSlotSwapSuspended;
						item.wallUp = Slot.IsPathBlockedBetween(slot, match3Game.GetSlot(intVector + IntVector2.up));
						item.wallDown = Slot.IsPathBlockedBetween(slot, match3Game.GetSlot(intVector + IntVector2.down));
						item.wallLeft = Slot.IsPathBlockedBetween(slot, match3Game.GetSlot(intVector + IntVector2.left));
						item.wallRight = Slot.IsPathBlockedBetween(slot, match3Game.GetSlot(intVector + IntVector2.right));
					}
					if (chip == null || !chip.canFormColorMatches)
					{
						slots.Add(item);
						continue;
					}
					item.itemColor = chip.itemColor;
					item.canFormColorMatches = !slot.isSlotMatchingSuspended;
					slots.Add(item);
				}
			}
		}
	}
}
