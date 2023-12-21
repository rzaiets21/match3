using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerupPlacementHandler : MonoBehaviour
{
	[Serializable]
	private class PowerupDefinition
	{
		public PowerupType powerupType;

		public RectTransform container;

		public void Show()
		{
			GGUtil.Show(container);
		}
	}

	public struct PlacementCompleteArguments
	{
		public InitArguments initArguments;

		public bool isSuccess;

		public Slot targetSlot;

		public bool isCancel => !isSuccess;
	}

	public struct InitArguments
	{
		public PowerupsDB.PowerupDefinition powerup;

		public Match3Game game;

		public Action<PlacementCompleteArguments> onComplete;
	}

	private struct State
	{
		public bool isActive;
	}

	[SerializeField]
	private InputHandler inputHandler;

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private List<PowerupDefinition> powerups = new List<PowerupDefinition>();

	[SerializeField]
	private TextMeshProUGUI descriptionLabel;

	private InitArguments initArguments;

	private State state;

	public void Show(InitArguments initArguments)
	{
		this.initArguments = initArguments;
		inputHandler.Clear();
		GGUtil.Show(this);
		Match3Game game = initArguments.game;
		state.isActive = true;
		LevelDefinitionTilesSlotsProvider provider = new LevelDefinitionTilesSlotsProvider(game.createBoardArguments.level);
		Match3Game.TutorialSlotHighlighter tutorialHighlighter = game.tutorialHighlighter;
		tutorialHighlighter.Show(provider);
		tutorialHighlighter.SetTutorialBackgroundActive(active: false);
		GGUtil.SetActive(widgetsToHide, active: false);
		GetDefinition(initArguments.powerup.type)?.Show();
		GGUtil.ChangeText(descriptionLabel, initArguments.powerup.description);
	}

	private PowerupDefinition GetDefinition(PowerupType powerupType)
	{
		for (int i = 0; i < powerups.Count; i++)
		{
			PowerupDefinition powerupDefinition = powerups[i];
			if (powerupDefinition.powerupType == powerupType)
			{
				return powerupDefinition;
			}
		}
		return null;
	}

	private void Hide()
	{
		Match3Game game = initArguments.game;
		if (game != null)
		{
			game.tutorialHighlighter.Hide();
			game.gameScreen.inputHandler.Clear();
		}
		GGUtil.Hide(this);
		state.isActive = false;
	}

	private void Update()
	{
		if (state.isActive)
		{
			InputHandler.PointerData pointerData = inputHandler.FirstDownPointer();
			if (pointerData != null)
			{
				OnPress(pointerData);
			}
		}
	}

	private void OnComplete(PlacementCompleteArguments completeArguments)
	{
		if (initArguments.onComplete != null)
		{
			initArguments.onComplete(completeArguments);
		}
	}

	private void OnPress(InputHandler.PointerData pointer)
	{
		PlacementCompleteArguments completeArguments = default(PlacementCompleteArguments);
		completeArguments.initArguments = initArguments;
		Vector2 position = pointer.position;
		Match3Game game = initArguments.game;
		if ((completeArguments.targetSlot = game.input.GetSlotFromMousePosition(pointer.position)) == null)
		{
			game.Play(GGSoundSystem.SFXType.CancelPress);
			Hide();
			OnComplete(completeArguments);
			return;
		}
		PowerupsDB.PowerupDefinition powerup = initArguments.powerup;
		powerup.ownedCount = Math.Max(0L, powerup.ownedCount - 1);
		game.gameScreen.powerupsPanel.Refresh();
		Hide();
		completeArguments.isSuccess = true;
		OnComplete(completeArguments);
		game.Play(GGSoundSystem.SFXType.ButtonPress);
	}
}
