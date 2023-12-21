using System;
using UnityEngine;

namespace GGMatch3
{
	public class WobbleAnimation
	{
		[Serializable]
		public class Settings
		{
			public float startScale = 0.8f;

			public float endScale = 1f;

			public bool directDriveScaleCurve;

			public AnimationCurve scaleCurve;

			public float duration;
		}

		private float time;

		private Settings settings;

		private TransformBehaviour transform;

		public bool isActive
		{
			get
			{
				if (settings == null)
				{
					return false;
				}
				if (time < settings.duration)
				{
					return true;
				}
				return false;
			}
		}

		public Vector3 scale
		{
			get
			{
				if (settings == null)
				{
					return Vector3.one;
				}
				float num = Mathf.InverseLerp(0f, settings.duration, time);
				num = settings.scaleCurve.Evaluate(num);
				float num2 = Mathf.LerpUnclamped(settings.startScale, settings.endScale, num);
				if (settings.directDriveScaleCurve)
				{
					num2 = num;
				}
				return new Vector3(num2, num2, 1f);
			}
		}

		public void Init(Settings settings, TransformBehaviour transform)
		{
			this.settings = settings;
			time = 0f;
			this.transform = transform;
		}

		public void Update(float deltaTime)
		{
			if (settings != null && !(time >= settings.duration))
			{
				time += deltaTime;
				if (transform != null)
				{
					transform.wobbleLocalScale = scale;
				}
			}
		}
	}
}
