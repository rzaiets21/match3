using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupsPanelPowerup : MonoBehaviour
{
	[Serializable]
	public class PowerupDefinition
	{
		public PowerupType powerupType;

		public RectTransform container;

		public void Show()
		{
			GGUtil.Show(container);
		}
	}

	[Serializable]
	public class ItemStyleSet
	{
		[SerializeField]
		private List<ItemStyle> styleChanges = new List<ItemStyle>();

		public void Apply()
		{
			for (int i = 0; i < styleChanges.Count; i++)
			{
				styleChanges[i].Apply();
			}
		}
	}

	[Serializable]
	public class ItemStyle
	{
		[SerializeField]
		private Image image;

		[SerializeField]
		private Color color;

		public void Apply()
		{
			GGUtil.SetColor(image, color);
		}
	}

	[SerializeField]
	private List<RectTransform> objectsToHide = new List<RectTransform>();

	[SerializeField]
	private List<PowerupDefinition> powerups = new List<PowerupDefinition>();

	[SerializeField]
	private Transform arrowAnimator;

	[SerializeField]
	private TextMeshProUGUI countLabel;

	[SerializeField]
	private ItemStyleSet activeStyle = new ItemStyleSet();

	[SerializeField]
	private ItemStyleSet notActiveStyle = new ItemStyleSet();

	[NonSerialized]
	private PowerupsPanel panel;

	[NonSerialized]
	public PowerupsDB.PowerupDefinition powerup;

	public void ShowArrow()
	{
		if (powerup != null)
		{
			bool active = powerup.ownedCount > 0;
			GGUtil.SetActive(arrowAnimator, active);
		}
	}

	public void Init(PowerupsDB.PowerupDefinition powerup, PowerupsPanel panel)
	{
		this.panel = panel;
		this.powerup = powerup;
		GGUtil.SetActive(objectsToHide, active: false);
		GetDefinition(powerup)?.Show();
		long ownedCount = powerup.ownedCount;
		if (ownedCount > 0)
		{
			activeStyle.Apply();
		}
		else
		{
			notActiveStyle.Apply();
		}
		if (ownedCount <= 0)
		{
			GGUtil.ChangeText(countLabel, "-");
		}
		else
		{
			GGUtil.ChangeText(countLabel, ownedCount);
		}
		bool flag = powerup.ownedCount > 0 && powerup.usedCount == 0;
		flag = false;
		GGUtil.SetActive(arrowAnimator, flag);
	}

	private PowerupDefinition GetDefinition(PowerupsDB.PowerupDefinition powerup)
	{
		for (int i = 0; i < powerups.Count; i++)
		{
			PowerupDefinition powerupDefinition = powerups[i];
			if (powerupDefinition.powerupType == powerup.type)
			{
				return powerupDefinition;
			}
		}
		return null;
	}

	public void ButtonCallback_OnPressed()
	{
		GGUtil.SetActive(arrowAnimator, active: false);
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		panel.gameScreen.Callback_ShowActivatePowerup(this);
	}
}
