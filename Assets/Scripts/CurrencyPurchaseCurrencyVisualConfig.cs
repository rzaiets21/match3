using TMPro;
using UnityEngine;

public class CurrencyPurchaseCurrencyVisualConfig : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI label;

	public void SetLabel(string text)
	{
		label.text = text;
	}
}
