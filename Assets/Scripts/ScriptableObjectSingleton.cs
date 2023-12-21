using UnityEngine;

public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
{
	private static bool applicationIsQuitting;

	protected static bool didTryToLoadSingleton_;

	protected static T instance_;

	public static T instance
	{
		get
		{
			if (!didTryToLoadSingleton_ && (Object)instance_ == (Object)null)
			{
				if (applicationIsQuitting)
				{
					return null;
				}
				didTryToLoadSingleton_ = true;
				UnityEngine.Debug.Log("Loading singleton from " + typeof(T).ToString());
				instance_ = Resources.Load<T>(typeof(T).ToString());
				if ((Object)instance_ != (Object)null)
				{
					(instance_ as ScriptableObjectSingleton<T>).UpdateData();
				}
			}
			return instance_;
		}
	}

	public void OnDestroy()
	{
		applicationIsQuitting = true;
	}

	protected virtual void UpdateData()
	{
	}
}
