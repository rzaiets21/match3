using System;
using UnityEngine;

public class GGSupportMenuAndroid : GGSupportMenu
{
	private AndroidJavaObject javaInstance;

	public GGSupportMenuAndroid()
	{
		return;
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.giraffegames.unityutil.SupportMenu"))
			{
				javaInstance = androidJavaClass.CallStatic<AndroidJavaObject>("instance", Array.Empty<object>());
			}
		}
	}

	public override void showRateApp(string rateProvider)
	{
		return;
		GGDebug.DebugLog("show rate");
		if (Application.platform == RuntimePlatform.Android)
		{
			javaInstance.Call("showRateApp", rateProvider);
		}
	}

	public override bool isNetworkConnected()
	{
		return true;
		if (Application.platform != RuntimePlatform.Android)
		{
			return base.isNetworkConnected();
		}
		try
		{
			return javaInstance.Call<bool>("isNetworkConnected", Array.Empty<object>());
		}
		catch
		{
			UnityEngine.Debug.Log("PROBLEM WITH LOADING IS NETWORK CONNECTED");
			return base.isNetworkConnected();
		}
	}
}
	