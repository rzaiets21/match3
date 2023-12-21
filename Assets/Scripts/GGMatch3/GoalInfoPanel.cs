using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGMatch3
{
    public class GoalInfoPanel : MonoBehaviour
    {
        [SerializeField]
        private ComponentPool goalsPool = new ComponentPool();
        
        [SerializeField]
        private ComponentPool rewardsPool = new ComponentPool();
        
        [SerializeField]
        private ComponentPool particlesPool = new ComponentPool();
        
        private List<GoalsPanelGoal> goalsList = new List<GoalsPanelGoal>();
        private List<RewardGoalPanel> rewardsList = new List<RewardGoalPanel>();

        public event Action onCompleteGoal;

        public void Init(GameScreen.StageState stageState, GoalInfo goals)
        {
            var allGoals = goals.goals;
            var allRewards = goals.rewards;
            for (int i = 0; i < allGoals.Count; i++)
            {
                MultiLevelGoals.Goal goal = stageState.goals.allGoals.FirstOrDefault(x => x.config == allGoals[i]);
                if(goal == null)
                    continue;
                
                GoalsPanelGoal goalsPanelGoal = goalsPool.Next<GoalsPanelGoal>(activate: true);
                goalsPanelGoal.Init(goal);
                goalsPanelGoal.OnCompleteGoal += OnCompleteGoal;
                goalsList.Add(goalsPanelGoal);
                goalsPanelGoal.UpdateCollectedCount();
            }

            for (int i = 0; i < allRewards.Count; i++)
            {
                var reward = allRewards[i];
                RewardGoalPanel rewardGoalPanel = rewardsPool.Next<RewardGoalPanel>(activate: true);
                rewardGoalPanel.Init(reward);
                rewardsList.Add(rewardGoalPanel);
            }
        }

        private void OnDisable()
        {
            onCompleteGoal?.Invoke();
            onCompleteGoal = null;
        }

        public GoalsPanelGoal GetGoal(Match3Goals.GoalBase goal)
        {
            if (goal == null)
            {
                return null;
            }
            for (int i = 0; i < goalsList.Count; i++)
            {
                GoalsPanelGoal goalsPanelGoal = goalsList[i];
                if (goalsPanelGoal.goal.IsCompatible(goal))
                {
                    return goalsPanelGoal;
                }
            }
            return null;
        }
        
        public bool IsCompatible(Match3Goals.GoalBase goal)
        {
            return GetGoal(goal) != null;
        }

        private void OnCompleteGoal()
        {
            if (goalsList.Any(x => !x.goal.isComplete))
            {
                return;
            }

            gameObject.SetActive(false);
        }
    }
}
