using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerupsPanel : MonoBehaviour
{
	[SerializeField]
	private ComponentPool powerupsPool = new ComponentPool();

	[SerializeField]
	private RectTransform container;

	[NonSerialized]
	public GameScreen gameScreen;

	[NonSerialized]
	private List<PowerupsPanelPowerup> powerups = new List<PowerupsPanelPowerup>();

	public void Refresh()
	{
		if (!(gameScreen == null))
		{
			Init(gameScreen);
		}
	}

	public void ShowArrowsOnAvailablePowerups()
	{
		List<GameObject> usedObjects = powerupsPool.usedObjects;
		for (int i = 0; i < usedObjects.Count; i++)
		{
			GameObject gameObject = usedObjects[i];
			if (!(gameObject == null))
			{
				PowerupsPanelPowerup component = gameObject.GetComponent<PowerupsPanelPowerup>();
				if (!(component == null))
				{
					component.ShowArrow();
				}
			}
		}
	}

	public void ReinitPowerups()
	{
		List<GameObject> usedObjects = powerupsPool.usedObjects;
		for (int i = 0; i < usedObjects.Count; i++)
		{
			GameObject gameObject = usedObjects[i];
			if (!(gameObject == null))
			{
				PowerupsPanelPowerup component = gameObject.GetComponent<PowerupsPanelPowerup>();
				if (!(component == null))
				{
					component.Init(component.powerup, this);
				}
			}
		}
	}

	public void Init(GameScreen gameScreen)
	{
		this.gameScreen = gameScreen;
		Vector2 prefabSizeDelta = powerupsPool.prefabSizeDelta;
		List<PowerupsDB.PowerupDefinition> list = ScriptableObjectSingleton<PowerupsDB>.instance.powerups;
		Vector2 sizeDeltum = container.sizeDelta;
		Vector3 a = new Vector3(0f, prefabSizeDelta.y * ((float)list.Count * 0.5f - 0.5f), 0f);
		powerupsPool.Clear();
		powerups.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			PowerupsDB.PowerupDefinition powerup = list[i];
			PowerupsPanelPowerup powerupsPanelPowerup = powerupsPool.Next<PowerupsPanelPowerup>(activate: true);
			powerupsPanelPowerup.transform.localPosition = a + Vector3.down * (prefabSizeDelta.y * (float)i);
			powerupsPanelPowerup.Init(powerup, this);
			GGUtil.Show(powerupsPanelPowerup);
			powerups.Add(powerupsPanelPowerup);
		}
		powerupsPool.HideNotUsed();
	}

	private void OnEnable()
	{
		if (gameScreen != null)
		{
			Init(gameScreen);
		}
	}
}
