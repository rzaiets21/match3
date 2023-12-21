using System;

namespace GGOptimize
{
	[Serializable]
	public class BucketRange
	{
		public int min;

		public int max = 100;

		public int count => max - min;

		public bool IsAcceptable(int bucket)
		{
			if (bucket >= min)
			{
				return bucket < max;
			}
			return false;
		}
	}
}
