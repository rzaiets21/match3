using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GGMatch3
{
	public class GoalsPanel : MonoBehaviour
	{
		[SerializeField] private CurrencyPanel CurrencyPanel;
		
		[SerializeField]
		private List<Transform> widgetsToHide = new List<Transform>();

		[SerializeField]
		private VisualStyleSet goalsDisplayStyle = new VisualStyleSet();

		[SerializeField]
		private VisualStyleSet coinsDisplayStyle = new VisualStyleSet();

		[SerializeField]
		private ComponentPool goalsPool = new ComponentPool();

		[SerializeField]
		public TextMeshProUGUI coinsRewardCountLabel;
		[SerializeField]
		public TextMeshProUGUI diamondsRewardCountLabel;
		[SerializeField]
		public TextMeshProUGUI movesCountLabel;

		[SerializeField]
		private RectTransform coinsCheckMark;
		[SerializeField]
		private RectTransform diamondsCheckMark;

		[SerializeField]
		private RectTransform goalsContainer;

		[SerializeField]
		public RectTransform coinsTransform;

		[SerializeField]
		private TextMeshProUGUI coinsCountLabel;

		// private List<GoalInfoPanel> uiGoalsList = new List<GoalInfoPanel>();
		private List<GoalsPanelGoal> uiGoalsList = new List<GoalsPanelGoal>();

		private GameScreen.StageState stageState;

		private VisibilityHelper visibilityHelper = new VisibilityHelper();

		[SerializeField]
		private TextMeshProUGUI pointsLabel;

		private int currentDisplayedScore;

		private int desiredScore;

		public void Init(GameScreen.StageState stageState)
		{
			var currentStage = Match3StagesDB.instance.currentStage;
			coinsRewardCountLabel.text = $"{50 + 20}";
			diamondsRewardCountLabel.text = "1";
			
			visibilityHelper.SetActive(widgetsToHide, isVisible: false);
			goalsDisplayStyle.Apply(visibilityHelper);
			visibilityHelper.Complete();
			this.stageState = stageState;
			Vector2 prefabSizeDelta = goalsPool.prefabSizeDelta;
			List<MultiLevelGoals.Goal> allGoals = stageState.goals.allGoals.Where(x => !x.isComplete).ToList();
			Vector2 sizeDeltum = goalsContainer.sizeDelta;
			Vector3 a = new Vector3(0f, prefabSizeDelta.y * ((float)allGoals.Count * 0.5f - 0.5f), 0f);
			goalsPool.Clear();
			uiGoalsList.Clear();
			for (int i = 0; i < allGoals.Count; i++)
			{
				MultiLevelGoals.Goal goal = allGoals[i];
				if(goal.isComplete)
					continue;
				GoalsPanelGoal goalsPanelGoal = goalsPool.Next<GoalsPanelGoal>(activate: true);
				goalsPanelGoal.transform.localPosition = a + Vector3.down * (prefabSizeDelta.y * (float)i);
				goalsPanelGoal.Init(goal);
				
				goalsPanelGoal.OnCompleteGoal += OnCompleteGoal;
				
				uiGoalsList.Add(goalsPanelGoal);
			}
			goalsPool.HideNotUsed();
			UpdateMovesCount();
			currentDisplayedScore = (desiredScore = stageState.userScore);
			UpdateScore();
			// visibilityHelper.SetActive(widgetsToHide, isVisible: false);
			// goalsDisplayStyle.Apply(visibilityHelper);
			// visibilityHelper.Complete();
			// this.stageState = stageState;
			// Vector2 prefabSizeDelta = goalsPool.prefabSizeDelta;
			// List<GoalInfo> allGoals = stageState.goals.goalsList;
			// Vector2 sizeDeltum = goalsContainer.sizeDelta;
			// Vector3 a = new Vector3(0f, prefabSizeDelta.y * ((float)allGoals.Count * 0.5f - 0.5f), 0f);
			// goalsPool.Clear();
			// uiGoalsList.Clear();
			// for (int i = 0; i < allGoals.Count; i++)
			// {
			// 	GoalInfo goals = allGoals[i];
			// 	GoalInfoPanel goalsPanel = goalsPool.Next<GoalInfoPanel>(activate: true);
			// 	//goalsPanel.transform.localPosition = a + Vector3.down * (prefabSizeDelta.y * (float)i);
			// 	goalsPanel.Init(stageState, goals);
			// 	goalsPanel.onCompleteGoal += OnCompleteGoal;
			// 	uiGoalsList.Add(goalsPanel);
			// }
			// goalsPool.HideNotUsed();
			// //UpdateMovesCount();
			// //currentDisplayedScore = (desiredScore = stageState.userScore);
			// //UpdateScore();
		}

		public void UpdateScore()
		{
			desiredScore = stageState.userScore;
		}

		public void ShowCoins()
		{
			visibilityHelper.SetActive(widgetsToHide, isVisible: false);
			coinsDisplayStyle.Apply(visibilityHelper);
			visibilityHelper.Complete();
		}

		public void SetCoinsCount(long coinsCount)
		{
			GGUtil.ChangeText(coinsCountLabel, coinsCount);
		}

		public GoalsPanelGoal GetGoal(Match3Goals.GoalBase goal)
		{
			if (goal == null)
			{
				return null;
			}
			for (int i = 0; i < uiGoalsList.Count; i++)
			{
				GoalsPanelGoal goalsPanelGoal = uiGoalsList[i];
				if (goalsPanelGoal.goal.IsCompatible(goal))
				{
					return goalsPanelGoal;
				}
			}
			return null;
			// if (goal == null)
			// {
			// 	return null;
			// }
			// for (int i = 0; i < uiGoalsList.Count; i++)
			// {
			// 	GoalInfoPanel goalsPanelGoal = uiGoalsList[i];
			// 	if (goalsPanelGoal.IsCompatible(goal))
			// 	{
			// 		return goalsPanelGoal.GetGoal(goal);
			// 	}
			// }
			// return null;
		}

		public void OnCompleteGoal()
		{
			if(uiGoalsList.Any(x => !x.goal.isComplete))
				return;
			
			coinsCheckMark.gameObject.SetActive(true);
			diamondsCheckMark.gameObject.SetActive(true);
		}

		public void UpdateMovesCount()
		{
			//GGUtil.ChangeText(text: stageState.MovesRemaining.ToString(), label: movesCountLabel);
		}

		private void Update()
		{
			if (currentDisplayedScore < desiredScore)
			{
				GeneralSettings generalSettings = Match3Settings.instance.generalSettings;
				float a = (float)currentDisplayedScore + generalSettings.scoreSpeed * Time.deltaTime;
				float b = Mathf.Lerp(currentDisplayedScore, desiredScore, Time.deltaTime * generalSettings.lerpSpeed);
				float f = Mathf.Max(a, b);
				currentDisplayedScore = Mathf.Min(Mathf.RoundToInt(f), desiredScore);
				GGUtil.ChangeText(pointsLabel, currentDisplayedScore.ToString());
			}
		}
	}
}
