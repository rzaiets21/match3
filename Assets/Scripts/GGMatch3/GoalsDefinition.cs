using System;
using System.Collections.Generic;

namespace GGMatch3
{
	[Serializable]
	public class GoalsDefinition
	{
		public int movesCount;

		public List<GoalConfig> goals = new List<GoalConfig>();

		public GoalsDefinition Clone()
		{
			GoalsDefinition goalsDefinition = new GoalsDefinition();
			goalsDefinition.movesCount = movesCount;
			for (int i = 0; i < goals.Count; i++)
			{
				GoalConfig goalConfig = goals[i];
				goalsDefinition.goals.Add(goalConfig.Clone());
			}
			return goalsDefinition;
		}
	}
}
