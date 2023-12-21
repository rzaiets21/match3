using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorGeneratorSetupChip : MonoBehaviour
{
	[Serializable]
	public class ChipDescriptor
	{
		public ItemColor color;

		public RectTransform container;
	}

	[SerializeField]
	private List<ChipDescriptor> chips = new List<ChipDescriptor>();

	[SerializeField]
	private RectTransform chipsContainer;

	[SerializeField]
	private RectTransform emptyContainer;

	[SerializeField]
	private Image colorCodeImage;

	public void Init(GeneratorSetup.GeneratorChipSetup chip, GeneratorSetup setup)
	{
		SetColorCode(setup);
		if (chip == null)
		{
			GGUtil.Hide(chipsContainer);
			GGUtil.Show(emptyContainer);
			return;
		}
		GGUtil.Hide(emptyContainer);
		GGUtil.Show(chipsContainer);
		for (int i = 0; i < chips.Count; i++)
		{
			ChipDescriptor chipDescriptor = chips[i];
			GGUtil.SetActive(chipDescriptor.container, chipDescriptor.color == chip.itemColor);
		}
	}

	public void SetColorCode(GeneratorSetup setup)
	{
		if (!(colorCodeImage == null))
		{
			Color color = GGUtil.colorProvider.GetColor(setup.position.y);
			colorCodeImage.color = color;
		}
	}
}
