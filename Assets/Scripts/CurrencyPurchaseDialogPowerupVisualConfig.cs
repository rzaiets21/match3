using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyPurchaseDialogPowerupVisualConfig : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private Image image;

	public void SetLabel(string text)
	{
		label.text = text;
	}
}
