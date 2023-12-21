using System.Net.NetworkInformation;

public class GGSupportMenu
{
	private static GGSupportMenu _instance;

	public static GGSupportMenu instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GGSupportMenuAndroid();
			}
			return _instance;
		}
	}

	public virtual void showRateApp(string rateProvider)
	{
		GGDebug.DebugLog("show rate");
	}

	public virtual bool isNetworkConnected()
	{
		try
		{
			return NetworkInterface.GetIsNetworkAvailable();
		}
		catch
		{
			return false;
		}
	}
}
