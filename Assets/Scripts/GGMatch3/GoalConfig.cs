using System;

namespace GGMatch3
{
	[Serializable]
	public class GoalConfig
	{
		public GoalType goalType;

		public ChipType chipType;

		public ItemColor itemColor;

		public int collectCount;

		public bool isCollectAllPresentAtStart => collectCount <= 0;

		public GoalConfig Clone()
		{
			return new GoalConfig
			{
				goalType = goalType,
				chipType = chipType,
				itemColor = itemColor,
				collectCount = collectCount
			};
		}

		public bool IsCompatible(GoalConfig goalConfig)
		{
			if (goalType != goalConfig.goalType)
			{
				return false;
			}
			if (chipType != goalConfig.chipType)
			{
				return false;
			}
			if (chipType != 0)
			{
				return true;
			}
			if (itemColor != goalConfig.itemColor)
			{
				return false;
			}
			return true;
		}
	}
}
