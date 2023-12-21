using ProtoModels;
using System;
using System.Collections.Generic;

public class GGRequestFileCache : SingletonInit<GGRequestFileCache>
{
	public string filename = "requestFileCache.bytes";

	private RequestCache cache;

	public override void Init()
	{
		if (!ProtoIO.LoadFromFile(filename, out cache))
		{
			cache = new RequestCache();
			cache.requests = new List<RequestCache.Request>();
			Save();
		}
	}

	public void Clear()
	{
		cache = new RequestCache();
		cache.requests = new List<RequestCache.Request>();
		Save();
	}

	private void Save()
	{
		ProtoIO.SaveToFile(filename, cache);
	}

	public void Put(string key, byte[] cachedObject, TimeSpan timeToLive)
	{
		GGDebug.DebugLog("CACHE FILE-PUT: " + key);
		RequestCache.Request request = GetRequest(key);
		if (request == null)
		{
			request = new RequestCache.Request();
			if (cache.requests == null)
			{
				cache.requests = new List<RequestCache.Request>();
			}
			cache.requests.Add(request);
		}
		request.key = key;
		request.bytesString = Convert.ToBase64String(cachedObject, 0, cachedObject.Length);
		request.ticksToExpire = (DateTime.Now + timeToLive).Ticks;
		Save();
	}

	private RequestCache.Request GetRequest(string key)
	{
		if (cache == null || cache.requests == null)
		{
			return null;
		}
		foreach (RequestCache.Request request in cache.requests)
		{
			if (request.key == key)
			{
				return request;
			}
		}
		return null;
	}

	public int Count()
	{
		return cache.requests.Count;
	}

	private bool IsExpired(RequestCache.Request request)
	{
		return DateTime.Now >= new DateTime(request.ticksToExpire);
	}

	public T Get<T>(string key) where T : class
	{
		GGDebug.DebugLog("CACHE FILE-GET: " + key);
		RequestCache.Request request = GetRequest(key);
		if (request != null)
		{
			GGDebug.DebugLog("Cache Hit: " + key + ", expired " + IsExpired(request).ToString());
			if (IsExpired(request))
			{
				cache.requests.Remove(request);
				Save();
				return null;
			}
			return Convert.FromBase64String(request.bytesString) as T;
		}
		return null;
	}
}
