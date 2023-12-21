using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameConfirmDialog : MonoBehaviour
{
	public struct ExitGameConfirmArguments
	{
		public Action<bool> onCompleteCallback;

		public MultiLevelGoals goals;

		public Match3Game game;
	}

	[SerializeField]
	private ComponentPool goalPrefabsPool = new ComponentPool();

	[SerializeField]
	private RankedBoostersContainer rankedContainer;

	private ExitGameConfirmArguments arguments;

	public void Show(ExitGameConfirmArguments arguments)
	{
		this.arguments = arguments;
		NavigationManager.instance.Push(base.gameObject, isModal: true);
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}

	public void Init()
	{
		goalPrefabsPool.Clear();
		goalPrefabsPool.HideNotUsed();
		List<MultiLevelGoals.Goal> activeGoals = arguments.goals.GetActiveGoals();
		for (int i = 0; i < activeGoals.Count; i++)
		{
			MultiLevelGoals.Goal goal = activeGoals[i];
			goalPrefabsPool.Next<GoalsPanelGoal>(activate: true).Init(goal);
		}
		if (rankedContainer != null)
		{
			rankedContainer.Init(arguments.game.initParams.giftBoosterLevel);
		}
	}

	public void OnEnable()
	{
		Init();
	}

	public void ButtonCallback_OnExit()
	{
		NavigationManager.instance.Pop();
		if (arguments.onCompleteCallback != null)
		{
			arguments.onCompleteCallback(obj: false);
		}
		GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
	}

	public void ButtonCallback_OnQuit()
	{
		NavigationManager.instance.Pop();
		if (arguments.onCompleteCallback != null)
		{
			arguments.onCompleteCallback(obj: true);
		}
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonConfirm);
	}
}
