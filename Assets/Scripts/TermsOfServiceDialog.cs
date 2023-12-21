using GGMatch3;
using System;
using UnityEngine;

public class TermsOfServiceDialog : MonoBehaviour
{
	private Action<bool> onComplete;

	public void Show(Action<bool> onComplete)
	{
		this.onComplete = onComplete;
		NavigationManager.instance.Push(base.gameObject, isModal: true);
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}

	public void ButtonCallback_OnCancel()
	{
		if (onComplete != null)
		{
			onComplete(obj: false);
		}
		GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
	}

	public void ButtonCallback_OnOK()
	{
		if (onComplete != null)
		{
			onComplete(obj: true);
		}
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
	}

	public void ButtonCallback_OnTermsOfService()
	{
		Application.OpenURL("https://sites.google.com/view/terms-services-home-design/home");
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
	}

	public void ButtonCallback_OnPrivacyPolicy()
	{
		Application.OpenURL("https://sites.google.com/view/artplaygames/home");
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
	}
}
