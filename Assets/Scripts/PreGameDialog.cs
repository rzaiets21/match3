using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreGameDialog : MonoBehaviour
{
	public class StageGoalsConfig
	{
		public List<GoalConfig> goals = new List<GoalConfig>();

		public void AddGoal(GoalConfig newGoal)
		{
			for (int i = 0; i < goals.Count; i++)
			{
				GoalConfig goalConfig = goals[i];
				if (goalConfig.IsCompatible(newGoal))
				{
					goalConfig.collectCount += newGoal.collectCount;
					return;
				}
			}
			goals.Add(newGoal.Clone());
		}
	}

	[SerializeField]
	private List<Transform> levelDifficultyWidgets = new List<Transform>();

	[SerializeField]
	private VisualStyleSet normalDifficultySlyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet hardDifficultySlyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet nightmareDifficultySlyle = new VisualStyleSet();

	[SerializeField]
	private TextMeshProUGUI stageNameLabel;

	[SerializeField]
	private ComponentPool goalsPool = new ComponentPool();

	[SerializeField]
	private ComponentPool boostersPool = new ComponentPool();

	private DecorateRoomScreen screen;

	private List<PreGameDialogBoosterPrefab> usedBoosters = new List<PreGameDialogBoosterPrefab>();

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private VisualStyleSet starStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet boostersStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet collectableGoalStyle;

	[SerializeField]
	private VisualStyleSet messageGoalStyleA;

	[SerializeField]
	private VisualStyleSet messageGoalStyleB;

	[SerializeField]
	private RankedBoostersContainer rankedBooster;

	public Action onHide;

	public Action<Match3GameParams> onStart;

	public Match3StagesDB.Stage stage;

	public void Show(Match3StagesDB.Stage stage, DecorateRoomScreen screen, Action onHide = null, Action<Match3GameParams> onStart = null)
	{
		this.screen = screen;
		this.onHide = onHide;
		this.stage = stage;
		this.onStart = onStart;
		NavigationManager.instance.Push(base.gameObject, isModal: true);
	}

	public void Init(Match3StagesDB.Stage stages, DecorateRoomScreen screen, Action onHide = null, Action<Match3GameParams> onStart = null)
	{
		this.screen = screen;
		this.onHide = onHide;
		this.onStart = onStart;
		this.stage = stage;
		if (rankedBooster != null)
		{
			rankedBooster.Init(ScriptableObjectSingleton<GiftsDefinitionDB>.instance.buildupBooster.currentBoosterLevel);
		}
		GGUtil.SetActive(widgetsToHide, active: false);
		//GoalsDefinition goal = stage.levelReference.level.goals;
		GGUtil.ChangeText(stageNameLabel, $"Level {stage.index + 1}");
		// var shouldUseStarDialog = stage.shouldUseStarDialog;
		var shouldUseStarDialog = true;
		if (shouldUseStarDialog)
		{
			GGUtil.ChangeText(stageNameLabel, "Get Diamond");
			starStyle.Apply();
		}
		else
		{
			boostersStyle.Apply();
		}
		goalsPool.Clear();
		goalsPool.HideNotUsed();
		var goalsDB = Resources.Load<GoalsDB>(nameof(GoalsDB));
		List<GoalConfig> list = new List<GoalConfig>();
		for (int i = 0; i < goalsDB.GoalInfos.Count; i++)
		{
			var goalInfo = goalsDB.GoalInfos[i];
			foreach (var goal in goalInfo.goals)
			{
				list.Add(goal);
			}
		}
		GGUtil.Hide(levelDifficultyWidgets);
		if (stage.difficulty == Match3StagesDB.Stage.Difficulty.Normal)
		{
			normalDifficultySlyle.Apply();
		}
		else if (stage.difficulty == Match3StagesDB.Stage.Difficulty.Hard)
		{
			hardDifficultySlyle.Apply();
		}
		else if (stage.difficulty == Match3StagesDB.Stage.Difficulty.Nightmare)
		{
			nightmareDifficultySlyle.Apply();
		}
		for (int k = 0; k < list.Count; k++)
		{
			GoalConfig config = list[k];
			goalsPool.Next<PreGameDialogGoalsPrefab>(activate: true).Init(config, stage);
		}
		List<BoosterConfig> list2 = new List<BoosterConfig>();
		for (int l = 0; l < Match3StagesDB.instance.defaultBoosters.Count; l++)
		{
			BoosterConfig boosterConfig = Match3StagesDB.instance.defaultBoosters[l];
			if (!stage.forbittenBoosters.Contains(boosterConfig.boosterType))
			{
				list2.Add(boosterConfig);
			}
		}
		boostersPool.Clear();
		boostersPool.HideNotUsed();
		usedBoosters.Clear();
		for (int m = 0; m < list2.Count; m++)
		{
			BoosterConfig boosterConfig2 = list2[m];
			PreGameDialogBoosterPrefab preGameDialogBoosterPrefab = boostersPool.Next<PreGameDialogBoosterPrefab>(activate: true);
			preGameDialogBoosterPrefab.Init(boosterConfig2, this, resetSelection: true);
			usedBoosters.Add(preGameDialogBoosterPrefab);
		}
		collectableGoalStyle.Apply();
	}

	public void ButtonCallback_OnPlayButtonClicked()
	{
		GGUtil.SetActive(base.gameObject, active: false);
		NavigationManager.instance.Pop();
		Match3GameParams match3GameParams = new Match3GameParams();
		// for (int i = 0; i < usedBoosters.Count; i++)
		// {
		// 	PreGameDialogBoosterPrefab preGameDialogBoosterPrefab = usedBoosters[i];
		// 	if (PlayerInventory.instance.OwnedCount(preGameDialogBoosterPrefab.GetBooster().boosterType) > 0 && preGameDialogBoosterPrefab.IsActive())
		// 	{
		// 		match3GameParams.activeBoosters.Add(preGameDialogBoosterPrefab.GetBooster());
		// 		match3GameParams.boughtBoosters.Add(preGameDialogBoosterPrefab.GetBooster());
		// 	}
		// }
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		if (onStart != null)
		{
			onStart(match3GameParams);
		}
		if (screen != null)
		{
			screen.StartGame();
		}
	}

	public void OnBoosterClicked(PreGameDialogBoosterPrefab.BoosterDefinition booster)
	{
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		if (PlayerInventory.instance.OwnedCount(booster.config.boosterType) > 0)
		{
			booster.active = !booster.active;
		}
		else
		{
			NavigationManager.instance.GetObject<CurrencyPurchaseDialog>().Show(ScriptableObjectSingleton<OffersDB>.instance);
		}
	}

	public void ButtonCallback_Hide()
	{
		GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
		if (onHide != null)
		{
			onHide();
		}
		GGUtil.SetActive(base.gameObject, active: false);
		NavigationManager.instance.Pop();
	}

	private void OnEnable()
	{
		Init(stage, screen, onHide, onStart);
	}
}
