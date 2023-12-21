using GGMatch3;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
	[SerializeField]
	private LoadingPanel panel;

	[SerializeField]
	private FloatRange coins;

	public void OnClick()
	{
		GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.coins, (int)coins.Random());
	}
}
