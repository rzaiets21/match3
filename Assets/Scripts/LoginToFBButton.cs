using GGMatch3;
using UnityEngine;

public class LoginToFBButton : MonoBehaviour
{
	private sealed class _003C_003Ec__DisplayClass0_0
	{
		public NavigationManager nav;

		public LoginToFBButton _003C_003E4__this;

		internal void _003CButtonCallback_OnButtonPress_003Eb__0(LoginProviderDialog.LoginResponse response)
		{
			nav.Pop();
			if (!response.isCancelled)
			{
				if (response.loginProvider == LoginProvider.FacebookLogin)
				{
					_003C_003E4__this.LoginToFacebook();
				}
				else if (response.loginProvider == LoginProvider.AppleLogin)
				{
					_003C_003E4__this.LoginToApple();
				}
			}
		}
	}

	public void ButtonCallback_OnButtonPress()
	{
		_003C_003Ec__DisplayClass0_0 _003C_003Ec__DisplayClass0_ = new _003C_003Ec__DisplayClass0_0();
		_003C_003Ec__DisplayClass0_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass0_.nav = NavigationManager.instance;
		if (BehaviourSingletonInit<GGAppleSignIn>.instance.isAvailable)
		{
			_003C_003Ec__DisplayClass0_.nav.GetObject<LoginProviderDialog>().Show(_003C_003Ec__DisplayClass0_._003CButtonCallback_OnButtonPress_003Eb__0);
		}
		else
		{
			LoginToFacebook();
		}
	}

	private void LoginToFacebook()
	{
		GGFacebook instance = BehaviourSingletonInit<GGFacebook>.instance;
		if (!instance.IsInitialized())
		{
			UnityEngine.Debug.Log("FACEBOOK NOT INITIALIZED");
			return;
		}
		FBLoginParams fBLoginParams = new FBLoginParams();
		fBLoginParams.scope = "public_profile";
		fBLoginParams.onComplete = OnLoginComplete;
		instance.Login(fBLoginParams);
	}

	private void LoginToApple()
	{
		GGAppleSignIn instance = BehaviourSingletonInit<GGAppleSignIn>.instance;
		if (!instance.isAvailable)
		{
			UnityEngine.Debug.Log("APPLE NOT AVAILABLE");
		}
		else
		{
			instance.SignIn(OnAppleLoginComplete);
		}
	}

	private void OnAppleLoginComplete(IAppleSignInProvider.SignInResponse response)
	{
		UnityEngine.Debug.LogFormat("Cancelled: {0}, UserId: {1}, Error: {2}", response.cancelled, response.user_id, response.error);
		bool num = GGUtil.HasText(response.user_id);
		if (response.isError)
		{
			UnityEngine.Debug.Log("ERROR: " + response.error);
		}
		if (num)
		{
			LoginWithAppleId(response.user_id);
		}
	}

	private void LoginWithAppleId(string userId)
	{
		NavigationManager.instance.GetObject<SyncGameScreen>().LoginToApple(userId);
	}

	private void OnLoginComplete(FBLoginResponse response)
	{
		if (ConfigBase.instance.debug)
		{
			UnityEngine.Debug.Log("Login Complete " + response.user_id);
		}
		bool num = GGUtil.HasText(response.user_id);
		if (response.isError)
		{
			UnityEngine.Debug.Log("FACEBOK ERROR: " + response.error);
		}
		if (num)
		{
			LoginWithFacebookId(response.user_id);
		}
	}

	private void LoginWithFacebookId(string facebookUserId)
	{
		NavigationManager.instance.GetObject<SyncGameScreen>().LoginToFacebook(facebookUserId);
	}
}
