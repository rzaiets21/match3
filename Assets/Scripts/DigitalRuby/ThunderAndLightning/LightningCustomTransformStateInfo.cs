using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningCustomTransformStateInfo
	{
		private LightningCustomTransformState _003CState_003Ek__BackingField;

		private LightningBoltParameters _003CParameters_003Ek__BackingField;

		public Vector3 BoltStartPosition;

		public Vector3 BoltEndPosition;

		public Transform Transform;

		public Transform StartTransform;

		public Transform EndTransform;

		public object UserInfo;

		private static readonly List<LightningCustomTransformStateInfo> cache = new List<LightningCustomTransformStateInfo>();

		public LightningCustomTransformState State
		{
			get
			{
				return _003CState_003Ek__BackingField;
			}
			set
			{
				_003CState_003Ek__BackingField = value;
			}
		}

		public LightningBoltParameters Parameters
		{
			set
			{
				_003CParameters_003Ek__BackingField = value;
			}
		}

		public static LightningCustomTransformStateInfo GetOrCreateStateInfo()
		{
			if (cache.Count == 0)
			{
				return new LightningCustomTransformStateInfo();
			}
			int index = cache.Count - 1;
			LightningCustomTransformStateInfo result = cache[index];
			cache.RemoveAt(index);
			return result;
		}

		public static void ReturnStateInfoToCache(LightningCustomTransformStateInfo info)
		{
			if (info != null)
			{
				info.Transform = (info.StartTransform = (info.EndTransform = null));
				info.UserInfo = null;
				cache.Add(info);
			}
		}
	}
}
