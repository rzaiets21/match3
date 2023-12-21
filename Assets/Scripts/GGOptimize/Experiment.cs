using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGOptimize
{
	[Serializable]
	public class Experiment
	{
		public string name;

		public string guid;

		public BucketRange bucketRange = new BucketRange();

		public string customDimensionToMark;

		public bool onlyNewUsers;

		public bool acceptsNewUsers = true;

		public bool useLocalBucket;

		public bool isArchived;

		public List<Variation> variations = new List<Variation>();

		public bool IsActive(Optimize optimize)
		{
			if (isArchived)
			{
				return false;
			}
			if (onlyNewUsers && !optimize.IsNewUserOnExperiment(this))
			{
				return false;
			}
			return bucketRange.IsAcceptable(optimize.GetUserBucket(this));
		}

		public Variation GetActiveVariation(int userBucket)
		{
			if (!bucketRange.IsAcceptable(userBucket))
			{
				return null;
			}
			if (variations.Count == 0)
			{
				return null;
			}
			int num = bucketRange.count / variations.Count;
			int index = Mathf.Clamp((userBucket - bucketRange.min) / num, 0, variations.Count - 1);
			return variations[index];
		}
	}
}
