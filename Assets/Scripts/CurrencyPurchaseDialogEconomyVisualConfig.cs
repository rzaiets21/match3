using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyPurchaseDialogEconomyVisualConfig : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	[SerializeField]
	private List<TextMeshProUGUI> labels;

	public void SetLabel(string text)
	{
		for (int i = 0; i < labels.Count; i++)
		{
			labels[i].text = text;
		}
	}
}
