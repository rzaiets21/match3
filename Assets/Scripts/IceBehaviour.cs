using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class IceBehaviour : MonoBehaviour
{
	[Serializable]
	public class GraphicsDefinition
	{
		[Serializable]
		public class TypeDefinition
		{
			public ChipType chipType;

			public ItemColor itemColor;

			public bool ContainsType(ChipType chipType, ItemColor itemColor)
			{
				if (this.chipType != chipType)
				{
					return false;
				}
				if (chipType != 0)
				{
					return true;
				}
				return this.itemColor == itemColor;
			}
		}

		public List<TypeDefinition> types = new List<TypeDefinition>();

		public Transform container;

		public List<Transform> levels = new List<Transform>();

		public bool isChipActive;

		public bool isForAnyChip;

		public bool ContainsType(ChipType chipType, ItemColor itemColor)
		{
			if (isForAnyChip)
			{
				return true;
			}
			for (int i = 0; i < types.Count; i++)
			{
				if (types[i].ContainsType(chipType, itemColor))
				{
					return true;
				}
			}
			return false;
		}

		public void ShowLevel(int level)
		{
			if (levels.Count != 0)
			{
				GGUtil.SetActive(levels[Mathf.Clamp(level, 0, levels.Count - 1)], active: true);
			}
		}

		public void Hide()
		{
			GGUtil.SetActive(container, active: false);
			for (int i = 0; i < levels.Count; i++)
			{
				GGUtil.SetActive(levels[i], active: false);
			}
		}
	}

	private ChipType shownType;

	private ItemColor shownColor;

	private int shownLevel;

	[SerializeField]
	private List<GraphicsDefinition> graphics = new List<GraphicsDefinition>();

	public void DoOnDestroy(Chip chip)
	{
		for (int i = 0; i < graphics.Count; i++)
		{
			graphics[i].Hide();
		}
		ChipBehaviour chipBehaviour = null;
		if (chip != null)
		{
			chipBehaviour = chip.GetComponentBehaviour<ChipBehaviour>();
		}
		if (chipBehaviour != null)
		{
			chipBehaviour.SetActive(active: true);
		}
	}

	public void TryInitIfDifferent(Chip chip, int level)
	{
		ChipType chipType = ChipType.Unknown;
		ItemColor itemColor = ItemColor.Unknown;
		if (chip != null)
		{
			chipType = chip.chipType;
			itemColor = chip.itemColor;
		}
		if (chipType != shownType || level != shownLevel || itemColor != shownColor)
		{
			Init(chip, level);
		}
	}

	public void Init(Chip chip, int level)
	{
		shownType = ChipType.Unknown;
		shownColor = ItemColor.Unknown;
		if (chip != null)
		{
			shownType = chip.chipType;
			shownColor = chip.itemColor;
		}
		shownLevel = level;
		for (int i = 0; i < graphics.Count; i++)
		{
			graphics[i].Hide();
		}
		ChipBehaviour chipBehaviour = null;
		if (chip != null)
		{
			chipBehaviour = chip.GetComponentBehaviour<ChipBehaviour>();
		}
		if (level <= 0)
		{
			chipBehaviour.SetActive(active: false);
			return;
		}
		GraphicsDefinition graphicsDefinition = DefinitionForChip(chip);
		if (graphicsDefinition != null)
		{
			graphicsDefinition.ShowLevel(level - 1);
			chipBehaviour = chip.GetComponentBehaviour<ChipBehaviour>();
			if (!(chipBehaviour == null))
			{
				chipBehaviour.SetActive(graphicsDefinition.isChipActive);
			}
		}
	}

	private GraphicsDefinition DefinitionForChip(Chip chip)
	{
		ChipType chipType = ChipType.Unknown;
		ItemColor itemColor = ItemColor.Unknown;
		if (chip != null)
		{
			chipType = chip.chipType;
			itemColor = chip.itemColor;
		}
		for (int i = 0; i < graphics.Count; i++)
		{
			GraphicsDefinition graphicsDefinition = graphics[i];
			if (graphicsDefinition.ContainsType(chipType, itemColor))
			{
				return graphicsDefinition;
			}
		}
		return null;
	}
}
