using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class TilesSlotsProvider
	{
		public struct Slot
		{
			public bool isOccupied;

			public IntVector2 position;

			public bool isEmpty => !isOccupied;

			public Slot(IntVector2 position, bool isOccupied)
			{
				this.position = position;
				this.isOccupied = isOccupied;
			}
		}

		public virtual int MaxSlots => 100;

		public virtual Vector2 StartPosition(float slotSize)
		{
			return Vector2.zero;
		}

		public virtual Slot GetSlot(IntVector2 position)
		{
			return default(Slot);
		}

		public virtual List<Slot> GetAllSlots()
		{
			return new List<Slot>();
		}
	}
}
