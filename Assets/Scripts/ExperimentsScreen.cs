using GGMatch3;
using TMPro;
using UnityEngine;

public class ExperimentsScreen : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI label;

	public void Show()
	{
		NavigationManager.instance.Push(base.gameObject);
	}

	public void OnEnable()
	{
		Init();
	}

	public void Init()
	{
		label.text = GGPlayerSettings.instance.GetName();
	}

	public void ButtonCallback_StartExperiment()
	{
		Match3StagesDB.instance.ResetAll();
		SingletonInit<RoomsBackend>.instance.Reset();
		BehaviourSingleton<EnergyManager>.instance.FillEnergy();
		GGPlayerSettings.instance.ResetEverything();
		GGUIDPrivate.Reset();
		string text = label.text;
		UnityEngine.Debug.LogFormat("Tester name is {0}", text);
		GGPlayerSettings.instance.SetName(text);
		GGPlayerSettings.instance.Save();
		NavigationManager.instance.Pop();
	}
}
