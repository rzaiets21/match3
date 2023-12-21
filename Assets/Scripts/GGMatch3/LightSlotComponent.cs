using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class LightSlotComponent : SlotComponent
	{
		public class PermanentLight
		{
			private float _003CcurrentIntensity_003Ek__BackingField;

			public virtual float currentIntensity => _003CcurrentIntensity_003Ek__BackingField;

			public virtual float GetCurrentIntensity(LightSlotComponent component)
			{
				return currentIntensity;
			}
		}

		private class IntensityChangeProcessor
		{
			public IntensityChange change;

			public bool isActive;

			public float timeActive;

			public float currentIntensity
			{
				get
				{
					if (timeActive < change.delay)
					{
						return 0f;
					}
					float t = Mathf.InverseLerp(change.delay, change.delay + change.duration, timeActive);
					return change.Intensity(t);
				}
			}

			public void Activate(IntensityChange change)
			{
				this.change = change;
				isActive = true;
				timeActive = 0f;
			}
		}

		[Serializable]
		public class Settings
		{
			public float lightFadeoutSpeed = 2f;

			public float maxIntensity = 2f;

			public IntensityChange fadeOut;
		}

		private List<IntensityChangeProcessor> intensityChanges = new List<IntensityChangeProcessor>();

		private List<PermanentLight> permanentLights = new List<PermanentLight>();

		private SlotLightBehaviour lightBehaviour;

		private Settings settings => Match3Settings.instance.lightSlotSettings;

		public float maxIntensity => settings.maxIntensity;

		public void Init(SlotLightBehaviour lightBehaviour)
		{
			this.lightBehaviour = lightBehaviour;
		}

		public void AddIntensityChange(IntensityChange change)
		{
			IntensityChangeProcessor intensityChangeProcessor = null;
			for (int i = 0; i < intensityChanges.Count; i++)
			{
				IntensityChangeProcessor intensityChangeProcessor2 = intensityChanges[i];
				if (!intensityChangeProcessor2.isActive)
				{
					intensityChangeProcessor = intensityChangeProcessor2;
					break;
				}
			}
			if (intensityChangeProcessor == null)
			{
				intensityChangeProcessor = new IntensityChangeProcessor();
				intensityChanges.Add(intensityChangeProcessor);
			}
			intensityChangeProcessor.Activate(change);
			OnUpdate(0f);
		}

		public void AddLight(PermanentLight light)
		{
			if (!permanentLights.Contains(light))
			{
				permanentLights.Add(light);
				OnUpdate(0f);
			}
		}

		public void RemoveLight(PermanentLight light)
		{
			permanentLights.Remove(light);
			OnUpdate(0f);
		}

		public void AddLight(float intensity)
		{
			IntensityChange fadeOut = settings.fadeOut;
			fadeOut.intensityRange.min = intensity;
			AddIntensityChange(fadeOut);
		}

		public void AddLightWithDuration(float intensity, float duration)
		{
			IntensityChange fadeOut = settings.fadeOut;
			fadeOut.intensityRange.min = intensity;
			fadeOut.duration = duration;
			AddIntensityChange(fadeOut);
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			float num = 0f;
			for (int i = 0; i < intensityChanges.Count; i++)
			{
				IntensityChangeProcessor intensityChangeProcessor = intensityChanges[i];
				if (intensityChangeProcessor.isActive)
				{
					intensityChangeProcessor.timeActive += deltaTime;
					if (intensityChangeProcessor.timeActive >= intensityChangeProcessor.change.delay + intensityChangeProcessor.change.duration)
					{
						intensityChangeProcessor.isActive = false;
					}
					else
					{
						num += intensityChangeProcessor.currentIntensity;
					}
				}
			}
			for (int j = 0; j < permanentLights.Count; j++)
			{
				PermanentLight permanentLight = permanentLights[j];
				num += permanentLight.GetCurrentIntensity(this);
			}
			if (lightBehaviour != null)
			{
				lightBehaviour.SetLight(Mathf.InverseLerp(0f, settings.maxIntensity, num));
			}
		}
	}
}
