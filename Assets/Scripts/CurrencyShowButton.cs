using GGMatch3;
using UnityEngine;

public class CurrencyShowButton : MonoBehaviour
{
	public void OnClick()
	{
		NavigationManager.instance.GetObject<CurrencyPurchaseDialog>().Show(ScriptableObjectSingleton<OffersDB>.instance);
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
	}
}
