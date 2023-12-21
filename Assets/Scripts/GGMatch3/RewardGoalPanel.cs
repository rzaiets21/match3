using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GGMatch3
{
    public class RewardGoalPanel : MonoBehaviour
    {
		[Serializable]
		public class RewardTypeDescriptor
		{
			public RewardType rewardType;

			public RectTransform container;
		}

		[SerializeField]
		private List<RewardTypeDescriptor> rewardTypes = new List<RewardTypeDescriptor>();

		[SerializeField]
		private List<RectTransform> widgetsToHide = new List<RectTransform>();

		[SerializeField]
		private TextMeshProUGUI collectedCount;

		[NonSerialized]
		public GoalReward goalReward;

		public void Init(GoalReward goalReward)
		{
			this.goalReward = goalReward;
			GGUtil.SetActive(widgetsToHide, active: false);
			RewardType rewardType = goalReward.RewardType;
			for (int j = 0; j < rewardTypes.Count; j++)
			{
				RewardTypeDescriptor rewardTypeDescriptor = rewardTypes[j];
				GGUtil.SetActive(rewardTypeDescriptor.container, rewardTypeDescriptor.rewardType == rewardType);
			}
			GGUtil.ChangeText(collectedCount, $"+{goalReward.rewardCount}");
			GGUtil.Show(collectedCount);
		}
    }
}
