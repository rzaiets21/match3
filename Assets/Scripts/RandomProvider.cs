using UnityEngine;

public class RandomProvider
{
	private int internalSeed;

	public virtual int seed
	{
		get
		{
			return Random.seed;
		}
		set
		{
			Random.seed = value;
			internalSeed = value;
		}
	}

	public virtual void Init()
	{
		internalSeed = seed;
	}

	public virtual float Range(float min, float max)
	{
		return Random.Range(min, max);
	}

	public virtual int Range(int min, int max)
	{
		return Random.Range(min, max);
	}
}
