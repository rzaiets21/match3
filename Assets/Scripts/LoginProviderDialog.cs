using GGMatch3;
using System;
using UnityEngine;

public class LoginProviderDialog : MonoBehaviour
{
	public struct LoginResponse
	{
		public bool isCancelled;

		public LoginProvider loginProvider;
	}

	private Action<LoginResponse> onComplete;

	public void Show(Action<LoginResponse> onComplete)
	{
		this.onComplete = onComplete;
		NavigationManager.instance.Push(base.gameObject, isModal: true);
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
	}

	public void ButtonCallback_OnCancel()
	{
		LoginResponse obj = default(LoginResponse);
		obj.isCancelled = true;
		onComplete?.Invoke(obj);
		GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
	}

	public void ButtonCallback_OnAppleLogin()
	{
		Complete(LoginProvider.AppleLogin);
	}

	public void ButtonCallback_OnFacebookLogin()
	{
		Complete(LoginProvider.FacebookLogin);
	}

	private void Complete(LoginProvider provider)
	{
		LoginResponse obj = default(LoginResponse);
		obj.loginProvider = provider;
		onComplete?.Invoke(obj);
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
	}
}
