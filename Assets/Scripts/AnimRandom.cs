using System;
using UnityEngine;

public class AnimRandom
{
	public static System.Random random = new System.Random();

	public static float Range(float min, float max)
	{
		return Mathf.Lerp(min, max, (float)random.NextDouble());
	}

	public static int Range(int min, int max)
	{
		return Mathf.FloorToInt(Mathf.Lerp(min, max, (float)random.NextDouble()));
	}
}
