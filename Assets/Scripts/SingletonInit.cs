public class SingletonInit<T> : InitClass where T : InitClass, new()
{
	private static T _instance;

	public static T Instance => instance;

	public static T instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new T();
				_instance.Init();
			}
			return _instance;
		}
	}
}
