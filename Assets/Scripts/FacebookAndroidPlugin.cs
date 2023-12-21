using System;
using UnityEngine;

public class FacebookAndroidPlugin : IFacebookProvider
{
	private sealed class _003C_003Ec__DisplayClass2_0
	{
		public FBLoginParams loginParams;

		internal void _003CLogin_003Eb__0(PluginCallbackManager.Response response)
		{
			FBLoginResponse obj = JsonUtility.FromJson<FBLoginResponse>(response.jsonResponse);
			loginParams.onComplete?.Invoke(obj);
		}
	}

	private AndroidJavaObject javaInstance;

	public FacebookAndroidPlugin()
	{
		// return;
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.giraffegames.unityutil.GGFacebook"))
			{
				javaInstance = androidJavaClass.CallStatic<AndroidJavaObject>("instance", Array.Empty<object>());
			}
		}
	}

	public override void Login(FBLoginParams loginParams)
	{
		// return;
		_003C_003Ec__DisplayClass2_0 _003C_003Ec__DisplayClass2_ = new _003C_003Ec__DisplayClass2_0();
		_003C_003Ec__DisplayClass2_.loginParams = loginParams;
		if (Application.platform == RuntimePlatform.Android)
		{
			string text = BehaviourSingletonInit<PluginCallbackManager>.instance.RegisterCallback(_003C_003Ec__DisplayClass2_._003CLogin_003Eb__0);
			javaInstance.Call("login", _003C_003Ec__DisplayClass2_.loginParams.scope, text, _003C_003Ec__DisplayClass2_.loginParams.isPublishPermissionLogin);
		}
	}

	public override bool IsInitialized()
	{
		// return false;
		if (Application.platform != RuntimePlatform.Android)
		{
			return true;
		}
		return javaInstance.Call<bool>("isInitialized", Array.Empty<object>());
	}
}
