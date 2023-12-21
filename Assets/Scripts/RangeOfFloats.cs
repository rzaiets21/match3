using System;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	[Serializable]
	public struct RangeOfFloats
	{
		public float Minimum;

		public float Maximum;

		public float Random()
		{
			return UnityEngine.Random.Range(Minimum, Maximum);
		}

		public float Random(System.Random r)
		{
			return Minimum + (float)r.NextDouble() * (Maximum - Minimum);
		}
	}
}
