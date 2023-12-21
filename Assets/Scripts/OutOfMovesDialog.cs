using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutOfMovesDialog : MonoBehaviour
{
	public delegate void OutOfMovesDelegate(OutOfMovesDialog dialog);

	[SerializeField]
	private string playButtonFormat;

	[SerializeField]
	private string movesCountFormat;

	[SerializeField]
	private TextMeshProUGUI buttonLabel;

	[SerializeField]
	private TextMeshProUGUI movesCounterLabel;

	[SerializeField]
	private TextMeshProUGUI coinsLabel;

	[SerializeField]
	private ComponentPool goalsPool = new ComponentPool();

	[SerializeField]
	private ComponentPool powerupsPool = new ComponentPool();

	[SerializeField]
	private RankedBoostersContainer rankedBoosters;

	[NonSerialized]
	public Match3Game game;

	private OutOfMovesDelegate onYes;

	private OutOfMovesDelegate onNo;

	public BuyMovesPricesConfig.OfferConfig offer;

	private MultiLevelGoals goals;

	public void Show(BuyMovesPricesConfig.OfferConfig offer, Match3Game game, MultiLevelGoals goals, OutOfMovesDelegate onYes, OutOfMovesDelegate onNo)
	{
		this.offer = offer;
		this.game = game;
		this.goals = goals;
		this.onYes = onYes;
		this.onNo = onNo;
		NavigationManager.instance.Push(base.gameObject, isModal: true);
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}

	public void Hide()
	{
		NavigationManager.instance.Pop();
	}

	public void OnBuyClicked()
	{
		if (onYes != null)
		{
			onYes(this);
		}
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
	}

	public void OnNotBuyClicked()
	{
		// AdsManager.ShowInterstitial(() =>
		// {
		// 	if (onNo != null)
		// 	{
		// 		onNo(this);
		// 	}
		// 	GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
		// });
	}

	public void Init(BuyMovesPricesConfig.OfferConfig offer)
	{
		buttonLabel.text = string.Format(playButtonFormat, offer.price.cost);
		movesCounterLabel.text = string.Format(movesCountFormat, offer.movesCount);
		List<MultiLevelGoals.Goal> activeGoals = goals.GetActiveGoals();
		if (rankedBoosters != null)
		{
			rankedBoosters.Init(game.initParams.giftBoosterLevel);
		}
		goalsPool.Clear();
		for (int i = 0; i < activeGoals.Count; i++)
		{
			MultiLevelGoals.Goal goal = activeGoals[i];
			GoalsPanelGoal goalsPanelGoal = goalsPool.Next<GoalsPanelGoal>(activate: true);
			goalsPanelGoal.Init(goal);
			goalsPanelGoal.UpdateCollectedCount();
		}
		goalsPool.HideNotUsed();
		powerupsPool.Clear();
		for (int j = 0; j < offer.powerups.Count; j++)
		{
			BuyMovesPricesConfig.OfferConfig.PowerupDefinition powerupDefinition = offer.powerups[j];
			powerupsPool.Next<OutOfMovesDialogPowerup>(activate: true).Init(powerupDefinition.powerupType, powerupDefinition.count);
		}
		powerupsPool.HideNotUsed();
		coinsLabel.text = GGPlayerSettings.instance.walletManager.CurrencyCount(CurrencyType.coins).ToString();
	}

	public void OnEnable()
	{
		Init(offer);
	}
}
