using GGMatch3;
using UnityEngine;

public class UILayer : MonoBehaviour
{
	[SerializeField]
	private bool escToGoBack;

	[SerializeField]
	protected GGSoundSystem.SFXType backSound = GGSoundSystem.SFXType.CancelPress;

	public virtual void OnGoBack(NavigationManager nav)
	{
		if (escToGoBack)
		{
			nav.Pop();
			GGSoundSystem.Play(backSound);
		}
	}
}
