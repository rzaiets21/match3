using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameDialogBoosterPrefab : MonoBehaviour
{
	public class BoosterDefinition
	{
		public BoosterConfig config;

		public bool active;
	}

	[Serializable]
	public class NamedSprite
	{
		public PreGameDialogBoosterPrefabVisualConfig visualConfig;

		public BoosterType type;

		public ChipType chipTypeUsedForRepresentation;

		public bool IsMatching(BoosterDefinition booster)
		{
			return booster.config.boosterType == type;
		}

		public void SetActive(bool active)
		{
			GGUtil.SetActive(visualConfig.transform, active);
			if (active)
			{
				visualConfig.Init(chipTypeUsedForRepresentation);
			}
		}

		public void SetLabel(string text)
		{
			visualConfig.SetLabel(text);
		}

		public void SetStyle(bool owned)
		{
			visualConfig.SetStyle(owned);
		}
	}

	[SerializeField]
	private Image activeImage;

	[SerializeField]
	public List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	public List<NamedSprite> namedSprites = new List<NamedSprite>();

	[SerializeField]
	private Transform arrowAnimation;

	[SerializeField]
	private PowerupsPanelPowerup.ItemStyleSet activeStyle = new PowerupsPanelPowerup.ItemStyleSet();

	[SerializeField]
	private PowerupsPanelPowerup.ItemStyleSet notActiveStyle = new PowerupsPanelPowerup.ItemStyleSet();

	private PreGameDialog screen;

	private BoosterDefinition booster = new BoosterDefinition();

	public void Init(BoosterConfig boosterConfig, PreGameDialog screen, bool resetSelection = false)
	{
		this.screen = screen;
		booster.config = boosterConfig;
		GGUtil.SetActive(widgetsToHide, active: false);
		if (resetSelection)
		{
			booster.active = false;
		}

		long num = PlayerInventory.instance.OwnedCount(boosterConfig.boosterType);
		PlayerInventory.instance.UsedCount(boosterConfig.boosterType);
		if (num > 0)
		{
			activeStyle.Apply();
		}
		else
		{
			notActiveStyle.Apply();
		}
		for (int i = 0; i < namedSprites.Count; i++)
		{
			NamedSprite namedSprite = namedSprites[i];
			if (namedSprite.IsMatching(booster))
			{
				namedSprite.SetActive(active: true);
				namedSprite.SetLabel($"x{num}");
				namedSprite.SetStyle(num > 0);
			}
		}
		bool active = num > 0 && !booster.active && screen.stage.timesPlayed > 0;
		GGUtil.SetActive(arrowAnimation, active);
		GGUtil.SetActive(activeImage, booster.active);
	}

	public bool IsActive()
	{
		return booster.active;
	}

	public BoosterConfig GetBooster()
	{
		return booster.config;
	}

	public void ButtonCallback_OnClick()
	{
		screen.OnBoosterClicked(booster);
		Init(booster.config, screen);
	}
}
