using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGMatch3
{
	public class MultiLevelGoals
	{
		public class Goal
		{
			public GoalConfig config;

			public List<Match3Goals.GoalBase> goals = new List<Match3Goals.GoalBase>();

			public bool isComplete => goals.TrueForAll(x => x.IsComplete());

			public int Collected
			{
				get
				{
					int num = 0;
					for (int i = 0; i < goals.Count; i++)
					{
						if (goals[i] is Match3Goals.CollectItemsGoal collectItemsGoal)
						{
							num += collectItemsGoal.collected;
						}
					}
					return num;
				}
			}

			public int RemainingCount
			{
				get
				{
					int num = 0;
					for (int i = 0; i < goals.Count; i++)
					{
						Match3Goals.GoalBase goalBase = goals[i];
						num += goalBase.RemainingCount;
					}
					return num;
				}
			}

			public bool IsCompatible(Match3Goals.GoalBase goal)
			{
				return goal?.config.IsCompatible(config) ?? false;
			}
		}

		// public List<GoalInfo> goalsList = new List<GoalInfo>();
		private List<Match3Goals> goalsList = new List<Match3Goals>();
		public List<Goal> allGoals = new List<Goal>();

		private List<Goal> activeGoals = new List<Goal>();

		public List<Goal> GetActiveGoals()
		{
			activeGoals.Clear();
			for (int i = 0; i < allGoals.Count; i++)
			{
				Goal goal = allGoals[i];
				if (!goal.isComplete)
				{
					activeGoals.Add(goal);
				}
			}
			return activeGoals;
		}

		private Goal GetOrCreateGoal(GoalConfig goalConfig)
		{
			for (int i = 0; i < allGoals.Count; i++)
			{
				Goal goal = allGoals[i];
				if (goal.config.IsCompatible(goalConfig))
				{
					return goal;
				}
			}
			Goal goal2 = new Goal();
			goal2.config = goalConfig;
			allGoals.Add(goal2);
			return goal2;
		}

		public void Clear()
		{
			allGoals.Clear();
		}
		
		public void Add(Match3Goals goals)
		{
			goalsList.Add(goals);
			List<Match3Goals.GoalBase> goals2 = goals.goals;
			for (int i = 0; i < goals2.Count; i++)
			{
				Match3Goals.GoalBase goalBase = goals2[i];
				GetOrCreateGoal(goalBase.config).goals.Add(goalBase);
			}
			// List<Match3Goals.GoalBase> goals2 = goals.goals;
			// for (int i = 0; i < goals2.Count; i++)
			// {
			// 	Match3Goals.GoalBase goalBase = goals2[i];
			// 	goalBase.OnComplete += OnGoalComplete;
			// 	GetOrCreateGoal(goalBase.config).goals.Add(goalBase);
			// }
		}

		// public void Add(GoalInfo goalInfo)
		// {
		// 	goalsList.Add(goalInfo);
		// }

		public void OnGoalComplete(Match3Goals.GoalBase goal)
		{
			// var goalInfo = goalsList.FirstOrDefault(x => x.goals.Any(g => g.IsCompatible(goal.config)));
			// if(goalInfo == null)
			// 	return;
			//
			// if(goalInfo.goals.Any(x => !GetOrCreateGoal(x).isComplete))
			// 	return;
			//
			// var rewards = goalInfo.rewards;
			// foreach (var reward in rewards)
			// {
			// 	switch (reward.RewardType)
			// 	{
			// 		case RewardType.Coins:
			// 			GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.coins, reward.rewardCount);
			// 			break;
			// 		case RewardType.Diamonds:
			// 			GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.diamonds, reward.rewardCount);
			// 			break;
			// 		case RewardType.Energy:
			// 			EnergyManager.instance.GainEnergy(reward.rewardCount * EnergyControlConfig.instance.energyPointPerCoin);
			// 			break;
			// 	}
			// }
		}
	}
}
