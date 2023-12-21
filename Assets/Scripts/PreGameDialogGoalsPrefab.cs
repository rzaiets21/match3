using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PreGameDialogGoalsPrefab : MonoBehaviour
{
	[Serializable]
	public class NamedSprite
	{
		public ChipType chipType;

		public GoalType goalType;

		public ItemColor itemColor;

		public bool useItemColor;

		public PreGameDialogGoalPrefabVisualConfig iconWithText;

		public bool IsMatching(GoalConfig config)
		{
			if (goalType != config.goalType)
			{
				return false;
			}
			if (useItemColor)
			{
				if (chipType == config.chipType)
				{
					return itemColor == config.itemColor;
				}
				return false;
			}
			return chipType == config.chipType;
		}

		public void SetActive(bool flag)
		{
			GGUtil.SetActive(iconWithText.transform, flag);
		}

		public void SetLabel(string text)
		{
			iconWithText.SetLabel(text);
		}
	}

	[SerializeField]
	private List<NamedSprite> namedSprites = new List<NamedSprite>();

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private PreGameDialogGoalPrefabVisualConfig genericIconWithText;

	public void Init(GoalConfig config, Match3StagesDB.Stage stage)
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		string text = "";
		if (config.isCollectAllPresentAtStart)
		{
			List<Match3StagesDB.LevelReference> allLevelReferences = stage.allLevelReferences;
			int num = 0;
			for (int i = 0; i < allLevelReferences.Count; i++)
			{
				LevelDefinition level = allLevelReferences[i].level;
				num += level.CountChips(config.chipType);
			}
			text = num.ToString();
		}
		else
		{
			text = config.collectCount.ToString();
		}
		ChipDisplaySettings chipDisplaySettings = Match3Settings.instance.GetChipDisplaySettings(config.chipType, config.itemColor);
		if (chipDisplaySettings != null)
		{
			GGUtil.Show(genericIconWithText);
			genericIconWithText.SetSprite(chipDisplaySettings.displaySprite);
			genericIconWithText.SetLabel(text);
			return;
		}
		for (int j = 0; j < namedSprites.Count; j++)
		{
			NamedSprite namedSprite = namedSprites[j];
			if (namedSprite.IsMatching(config))
			{
				namedSprite.SetActive(flag: true);
				namedSprite.SetLabel(text);
			}
			else
			{
				namedSprite.SetActive(flag: false);
			}
		}
	}
}
