using System.Collections.Generic;

namespace GGMatch3
{
	public class BoltCollection
	{
		public List<LightingBolt> bolts = new List<LightingBolt>();

		public void AddUsedBolt(LightingBolt bolt)
		{
			if (bolts != null && !(bolt == null))
			{
				bolts.Remove(bolt);
			}
		}

		public LightingBolt GetBoltEndingOnSlot(Slot slot)
		{
			if (bolts == null)
			{
				return null;
			}
			for (int i = 0; i < bolts.Count; i++)
			{
				LightingBolt lightingBolt = bolts[i];
				if (lightingBolt.endSlot == slot)
				{
					return lightingBolt;
				}
			}
			return null;
		}

		public void Clear()
		{
			if (bolts != null)
			{
				for (int i = 0; i < bolts.Count; i++)
				{
					bolts[i].RemoveFromGame();
				}
			}
		}
	}
}
