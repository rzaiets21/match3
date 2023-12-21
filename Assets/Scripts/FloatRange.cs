using System;
using UnityEngine;

namespace GGMatch3
{
	[Serializable]
	public struct FloatRange
	{
		public float min;

		public float max;

		public FloatRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float Random()
		{
			return UnityEngine.Random.Range(min, max);
		}

		public float Lerp(float t)
		{
			return Mathf.Lerp(min, max, t);
		}

		public float InverseLerp(float value)
		{
			return Mathf.InverseLerp(min, max, value);
		}
	}
}
