using System;
using System.Collections.Generic;

public class GGRequestCache : Singleton<GGRequestCache>
{
	public class CachedRequest
	{
		protected DateTime expireTime;

		public object cachedObject;

		public bool isExpired => DateTime.Now > expireTime;

		public CachedRequest(object obj, TimeSpan timeToLive)
		{
			cachedObject = obj;
			expireTime = DateTime.Now + timeToLive;
		}
	}

	private Dictionary<string, CachedRequest> cache = new Dictionary<string, CachedRequest>();

	public void Clear()
	{
		cache.Clear();
	}

	public void Put(string key, object cachedObject, TimeSpan timeToLive)
	{
		if (cachedObject != null)
		{
			GGDebug.DebugLog("CACHE MEM-PUT: " + key + " time to live " + timeToLive);
			cache[key] = new CachedRequest(cachedObject, timeToLive);
		}
	}

	public T Get<T>(string key) where T : class
	{
		CachedRequest cachedRequest = null;
		GGDebug.DebugLog("CACHE MEM-GET: " + key);
		if (cache.ContainsKey(key))
		{
			cachedRequest = cache[key];
			GGDebug.DebugLog("Cache Hit: " + key + ", expired " + cachedRequest.isExpired.ToString());
			if (cachedRequest.isExpired)
			{
				cache.Remove(key);
				return null;
			}
			return cachedRequest.cachedObject as T;
		}
		return null;
	}
}
