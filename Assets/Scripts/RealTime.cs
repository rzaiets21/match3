using System;
using UnityEngine;

public class RealTime : MonoBehaviour
{
	private static RealTime mInst;

	private float mRealTime;

	private float mRealDelta;

	private float mRealTimeIgnoreApplicationPause;

	private float mRealDeltaIgnoreApplicationPause;

	private bool isCurrentFrameTimeSet;

	private DateTime currentFrameTime;

	private static bool applicationIsQuitting;

	public static float time
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealTime;
		}
	}

	public static DateTime frameDateTime
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			if (!mInst.isCurrentFrameTimeSet)
			{
				mInst.currentFrameTime = DateTime.UtcNow;
				mInst.isCurrentFrameTimeSet = true;
			}
			return mInst.currentFrameTime;
		}
	}

	public static float deltaTimeIgnoreApplicationPause
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealDeltaIgnoreApplicationPause;
		}
	}

	public static float deltaTime
	{
		get
		{
			if (mInst == null)
			{
				Spawn();
			}
			return mInst.mRealDelta;
		}
	}

	private static void Spawn()
	{
		if (!applicationIsQuitting)
		{
			GameObject gameObject = new GameObject("_RealTime");
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			mInst = gameObject.AddComponent<RealTime>();
			mInst.mRealTime = Time.realtimeSinceStartup;
			mInst.mRealTimeIgnoreApplicationPause = Time.realtimeSinceStartup;
		}
	}

	private void OnDestroy()
	{
		applicationIsQuitting = true;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mRealDelta = realtimeSinceStartup - mRealTime;
		mRealDeltaIgnoreApplicationPause = realtimeSinceStartup - mRealTimeIgnoreApplicationPause;
		mRealTime = realtimeSinceStartup;
		mRealTimeIgnoreApplicationPause = realtimeSinceStartup;
		mInst.currentFrameTime = DateTime.UtcNow;
		mInst.isCurrentFrameTimeSet = true;
	}

	private void OnApplicationPause(bool paused)
	{
		mRealTime = Time.realtimeSinceStartup;
		mRealDelta = 0f;
	}
}
