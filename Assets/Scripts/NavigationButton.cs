using GGMatch3;
using UnityEngine;

public class NavigationButton : MonoBehaviour
{
	public enum ActionType
	{
		Push,
		PushModal,
		Pop
	}

	[SerializeField]
	private ActionType action;

	[SerializeField]
	private string screenName;

	[SerializeField]
	private GGSoundSystem.SFXType pressSound;

	public void OnClick()
	{
		NavigationManager instance = NavigationManager.instance;
		if (instance == null)
		{
			return;
		}
		if (action == ActionType.Pop)
		{
			GGSoundSystem.Play(pressSound);
			instance.Pop();
			return;
		}
		bool isModal = action == ActionType.PushModal;
		NavigationManager.ObjectDefinition objectByName = instance.GetObjectByName(screenName);
		if (objectByName != null)
		{
			instance.Push(objectByName.gameObject, isModal);
			GGSoundSystem.Play(pressSound);
		}
	}
}
