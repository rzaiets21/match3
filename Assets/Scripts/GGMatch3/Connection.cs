using System.Collections.Generic;

namespace GGMatch3
{
	public class Connection
	{
		public enum ConnectionType
		{
			Vertical,
			Horizontal,
			Square
		}

		public List<Slot> slotsList = new List<Slot>();

		public ConnectionType type;

		public ItemColor itemColor
		{
			get
			{
				if (slotsList.Count == 0)
				{
					return ItemColor.Unknown;
				}
				return slotsList[0].GetSlotComponent<Chip>().itemColor;
			}
		}

		public bool isUsable
		{
			get
			{
				if (type == ConnectionType.Square)
				{
					return slotsList.Count >= 4;
				}
				return slotsList.Count >= 3;
			}
		}

		public bool ContainsPosition(IntVector2 position)
		{
			for (int i = 0; i < slotsList.Count; i++)
			{
				if (slotsList[i].position == position)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsIntersecting(Connection c)
		{
			for (int i = 0; i < slotsList.Count; i++)
			{
				Slot slot = slotsList[i];
				for (int j = 0; j < c.slotsList.Count; j++)
				{
					if (c.slotsList[j] == slot)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsChipAcceptable(Chip chip)
		{
			if (chip == null)
			{
				return false;
			}
			if (!chip.canFormColorMatches)
			{
				return false;
			}
			if (itemColor != ItemColor.Unknown && chip.itemColor != itemColor)
			{
				return false;
			}
			return true;
		}

		public void Clear()
		{
			slotsList.Clear();
		}

		public void CopyFrom(Connection c)
		{
			slotsList.Clear();
			slotsList.AddRange(c.slotsList);
			type = c.type;
		}
	}
}
