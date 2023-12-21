using GGMatch3;
using UnityEngine;

public class SettingsScreen : MonoBehaviour
{
	[SerializeField]
	private RectTransform experimentsButton;

	[SerializeField]
	private RectTransform stagesSelectionButton;

	private void ResetGameProgress()
	{
		Match3StagesDB.instance.ResetAll();
		SingletonInit<RoomsBackend>.instance.Reset();
		GGPlayerSettings.instance.ResetEverything();
		BehaviourSingleton<EnergyManager>.instance.FillEnergy();
		GGUIDPrivate.Reset();
	}

	public void ButtonCallback_ResetGameProgress()
	{
		ResetGameProgress();
		NavigationManager.instance.Pop();
	}

	public void ButtonCallback_ShowExperimentsScreen()
	{
		NavigationManager.instance.GetObject<ExperimentsScreen>().Show();
	}

	public void ButtonCallback_ShowStageSelectionScreen()
	{
		NavigationManager.instance.GetObject<StageSelectionScreen>().Show();
	}

	public void ButtonCallback_ClearCache()
	{
		Caching.ClearCache();
	}

	public void ButtonCallback_GiveStars()
	{
		GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.diamonds, 1000);
	}

	public void ButtonCallback_ResetAndGiveStars()
	{
		ResetGameProgress();
		GGPlayerSettings.instance.walletManager.AddCurrency(CurrencyType.diamonds, 1000);
		NavigationManager.instance.Pop();
	}

	public void OnEnable()
	{
		GGUtil.SetActive(experimentsButton, ConfigBase.instance.debug);
		GGUtil.SetActive(stagesSelectionButton, ConfigBase.instance.debug);
	}
}
