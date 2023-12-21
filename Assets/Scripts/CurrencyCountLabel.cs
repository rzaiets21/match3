using TMPro;
using UnityEngine;

public class CurrencyCountLabel : MonoBehaviour
{
	[SerializeField]
	private CurrencyType currencyType;

	[SerializeField]
	private TextMeshProUGUI coinsLabel;

	public void Reinit()
	{
		long price = GGPlayerSettings.instance.walletManager.CurrencyCount(CurrencyType.coins);
		GGUtil.ChangeText(coinsLabel, GGFormat.FormatPrice(price));
	}

	private void OnEnable()
	{
		Reinit();
	}
}
