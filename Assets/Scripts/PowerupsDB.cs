using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerupsDB : ScriptableObjectSingleton<PowerupsDB>
{
	[Serializable]
	public class PowerupDefinition
	{
		public PowerupType type;

		public string name;

		public string description;

		public int buyQuanitty = 3;

		public SingleCurrencyPrice buyPrice = new SingleCurrencyPrice();

		public long ownedCount
		{
			get
			{
				return PlayerInventory.instance.OwnedCount(type);
			}
			set
			{
				PlayerInventory.instance.SetOwned(type, Math.Max(value, 0L));
			}
		}

		public long usedCount
		{
			get
			{
				return PlayerInventory.instance.UsedCount(type);
			}
			set
			{
				PlayerInventory.instance.SetUsedCount(type, value);
			}
		}
	}

	[SerializeField]
	public List<PowerupDefinition> powerups = new List<PowerupDefinition>();

	public PowerupDefinition Powerup(PowerupType powerupType)
	{
		for (int i = 0; i < powerups.Count; i++)
		{
			PowerupDefinition powerupDefinition = powerups[i];
			if (powerupDefinition.type == powerupType)
			{
				return powerupDefinition;
			}
		}
		return null;
	}

	protected override void UpdateData()
	{
		base.UpdateData();
	}
}
