using System.Collections.Generic;

namespace GGMatch3
{
	public class SetLock
	{
		public List<Slot> slots = new List<Slot>();

		public bool isSwapingSuspended;

		public bool GetIsSwappingSuspended(Slot slot)
		{
			if (!isSwapingSuspended)
			{
				return false;
			}
			return !slots.Contains(slot);
		}
	}
}
