using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGMatch3
{
	public class GoalsPanelGoal : MonoBehaviour
	{
		[Serializable]
		public class ChipDescriptor
		{
			public ItemColor color;

			public RectTransform container;
		}

		[Serializable]
		public class ChipTypeDescriptor
		{
			public ChipType chipType;

			public RectTransform container;
		}

		[SerializeField]
		private Image genericChipImage;

		[SerializeField]
		private List<ChipDescriptor> chips = new List<ChipDescriptor>();

		[SerializeField]
		private List<ChipTypeDescriptor> chipsTypes = new List<ChipTypeDescriptor>();

		[SerializeField]
		private List<RectTransform> widgetsToHide = new List<RectTransform>();

		[SerializeField]
		private TextMeshProUGUI collectedCount;

		[SerializeField]
		private RectTransform completeTickMark;

		[NonSerialized]
		public MultiLevelGoals.Goal goal;

		public event Action OnCompleteGoal; 

		public void Init(MultiLevelGoals.Goal goal)
		{
			this.goal = goal;
			GGUtil.SetActive(widgetsToHide, active: false);
			ChipType chipType = goal.config.chipType;
			ItemColor itemColor = goal.config.itemColor;
			ChipDisplaySettings chipDisplaySettings = Match3Settings.instance.GetChipDisplaySettings(chipType, itemColor);
			if (chipDisplaySettings != null)
			{
				GGUtil.SetActive(genericChipImage, active: true);
				GGUtil.SetSprite(genericChipImage, chipDisplaySettings.displaySprite);
			}
			else
			{
				for (int i = 0; i < chips.Count; i++)
				{
					ChipDescriptor chipDescriptor = chips[i];
					GGUtil.SetActive(chipDescriptor.container, chipType == ChipType.Chip && chipDescriptor.color == itemColor);
				}
				for (int j = 0; j < chipsTypes.Count; j++)
				{
					ChipTypeDescriptor chipTypeDescriptor = chipsTypes[j];
					GGUtil.SetActive(chipTypeDescriptor.container, chipTypeDescriptor.chipType == chipType);
				}
			}
			GGUtil.ChangeText(collectedCount, goal.RemainingCount.ToString());
			GGUtil.Hide(completeTickMark);
			GGUtil.Show(collectedCount);
		}

		public void UpdateCollectedCount()
		{
			int num = Mathf.Max(goal.RemainingCount, 0);
			bool flag = num <= 0;
			GGUtil.ChangeText(collectedCount, num.ToString());
			GGUtil.SetActive(collectedCount, !flag);
			GGUtil.SetActive(completeTickMark, flag);

			if (flag)
				CompleteGoal();
		}

		private void CompleteGoal()
		{
			OnCompleteGoal?.Invoke();
		}
	}
}
