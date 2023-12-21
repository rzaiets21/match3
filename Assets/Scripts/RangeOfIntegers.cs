using System;

namespace DigitalRuby.ThunderAndLightning
{
	[Serializable]
	public struct RangeOfIntegers
	{
		public int Minimum;

		public int Maximum;

		public int Random(Random r)
		{
			return r.Next(Minimum, Maximum + 1);
		}
	}
}
