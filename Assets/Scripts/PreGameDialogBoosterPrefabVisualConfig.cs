using GGMatch3;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreGameDialogBoosterPrefabVisualConfig : MonoBehaviour
{
	[SerializeField]
	private Image image;

	[SerializeField]
	private VisualStyleSet ownedStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet notOwnedStyle = new VisualStyleSet();

	[SerializeField]
	private List<RectTransform> hideAtStart = new List<RectTransform>();

	[SerializeField]
	private TextMeshProUGUI label;

	public void Init(ChipType chipTypeUsedForRepresentation)
	{
		ChipDisplaySettings chipDisplaySettings = Match3Settings.instance.GetChipDisplaySettings(chipTypeUsedForRepresentation, ItemColor.Uncolored);
		if (chipDisplaySettings != null)
		{
			GGUtil.SetSprite(image, chipDisplaySettings.displaySprite);
		}
	}

	public void SetLabel(string text)
	{
		GGUtil.ChangeText(label, text);
	}

	public void SetStyle(bool owned)
	{
		GGUtil.SetActive(hideAtStart, active: false);
		if (owned)
		{
			ownedStyle.Apply();
		}
		else
		{
			notOwnedStyle.Apply();
		}
	}
}
