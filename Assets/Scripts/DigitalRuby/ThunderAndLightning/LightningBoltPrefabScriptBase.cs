using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public abstract class LightningBoltPrefabScriptBase : LightningBoltScript
	{
		private readonly List<LightningBoltParameters> batchParameters = new List<LightningBoltParameters>();

		private readonly System.Random random = new System.Random();

		public RangeOfFloats IntervalRange;

		public RangeOfIntegers CountRange;

		public float CountProbabilityModifier;

		public RangeOfFloats DelayRange;

		public RangeOfFloats DurationRange;

		public RangeOfFloats TrunkWidthRange;

		public float LifeTime;

		public int Generations;

		public float ChaosFactor;

		public float ChaosFactorForks;

		public float Intensity;

		public float GlowIntensity;

		public float GlowWidthMultiplier;

		public float FadePercent;

		public float FadeInMultiplier;

		public float FadeFullyLitMultiplier;

		public float FadeOutMultiplier;

		public float GrowthMultiplier;

		public float EndWidthMultiplier;

		public float Forkedness;

		public float ForkLengthMultiplier;

		public float ForkLengthVariance;

		public float ForkEndWidthMultiplier;

		public LightningLightParameters LightParameters;

		public int MaximumLightsPerBatch;

		public bool ManualMode;

		public float AutomaticModeSeconds;

		public LightningCustomTransformDelegate CustomTransformHandler;

		private System.Random _003CRandomOverride_003Ek__BackingField;

		private float nextLightningTimestamp;

		private float lifeTimeRemaining;

		public System.Random RandomOverride
		{
			get
			{
				return _003CRandomOverride_003Ek__BackingField;
			}
			set
			{
				_003CRandomOverride_003Ek__BackingField = value;
			}
		}

		private void CalculateNextLightningTimestamp(float offset)
		{
			nextLightningTimestamp = ((IntervalRange.Minimum == IntervalRange.Maximum) ? IntervalRange.Minimum : (offset + IntervalRange.Random()));
		}

		private void CustomTransform(LightningCustomTransformStateInfo state)
		{
			if (CustomTransformHandler != null)
			{
				CustomTransformHandler.Invoke(state);
			}
		}

		private void CallLightning()
		{
			CallLightning(null, null);
		}

		private void CallLightning(Vector3? start, Vector3? end)
		{
			System.Random r = RandomOverride ?? random;
			int num = CountRange.Random(r);
			for (int i = 0; i < num; i++)
			{
				LightningBoltParameters lightningBoltParameters = CreateParameters();
				if (CountProbabilityModifier >= 0.9999f || i == 0 || (float)lightningBoltParameters.Random.NextDouble() <= CountProbabilityModifier)
				{
					lightningBoltParameters.CustomTransform = ((CustomTransformHandler == null) ? null : new Action<LightningCustomTransformStateInfo>(CustomTransform));
					CreateLightningBolt(lightningBoltParameters);
					if (start.HasValue)
					{
						lightningBoltParameters.Start = start.Value;
					}
					if (end.HasValue)
					{
						lightningBoltParameters.End = end.Value;
					}
				}
				else
				{
					LightningBoltParameters.ReturnParametersToCache(lightningBoltParameters);
				}
			}
			CreateLightningBoltsNow();
		}

		protected void CreateLightningBoltsNow()
		{
			int maximumLightsPerBatch = LightningBolt.MaximumLightsPerBatch;
			LightningBolt.MaximumLightsPerBatch = MaximumLightsPerBatch;
			CreateLightningBolts(batchParameters);
			LightningBolt.MaximumLightsPerBatch = maximumLightsPerBatch;
			batchParameters.Clear();
		}

		protected override void PopulateParameters(LightningBoltParameters p)
		{
			base.PopulateParameters(p);
			p.RandomOverride = RandomOverride;
			float lifeTime = DurationRange.Random(p.Random);
			float trunkWidth = TrunkWidthRange.Random(p.Random);
			p.Generations = Generations;
			p.LifeTime = lifeTime;
			p.ChaosFactor = ChaosFactor;
			p.ChaosFactorForks = ChaosFactorForks;
			p.TrunkWidth = trunkWidth;
			p.Intensity = Intensity;
			p.GlowIntensity = GlowIntensity;
			p.GlowWidthMultiplier = GlowWidthMultiplier;
			p.Forkedness = Forkedness;
			p.ForkLengthMultiplier = ForkLengthMultiplier;
			p.ForkLengthVariance = ForkLengthVariance;
			p.FadePercent = FadePercent;
			p.FadeInMultiplier = FadeInMultiplier;
			p.FadeOutMultiplier = FadeOutMultiplier;
			p.FadeFullyLitMultiplier = FadeFullyLitMultiplier;
			p.GrowthMultiplier = GrowthMultiplier;
			p.EndWidthMultiplier = EndWidthMultiplier;
			p.ForkEndWidthMultiplier = ForkEndWidthMultiplier;
			p.DelayRange = DelayRange;
			p.LightParameters = LightParameters;
		}

		protected override void Start()
		{
			base.Start();
			CalculateNextLightningTimestamp(0f);
			lifeTimeRemaining = ((LifeTime <= 0f) ? float.MaxValue : LifeTime);
		}

		protected override void Update()
		{
			base.Update();
			if ((lifeTimeRemaining -= LightningBoltScript.DeltaTime) < 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if ((nextLightningTimestamp -= LightningBoltScript.DeltaTime) <= 0f)
			{
				CalculateNextLightningTimestamp(nextLightningTimestamp);
				if (!ManualMode)
				{
					CallLightning();
				}
			}
			if (AutomaticModeSeconds > 0f)
			{
				AutomaticModeSeconds = Mathf.Max(0f, AutomaticModeSeconds - LightningBoltScript.DeltaTime);
				ManualMode = (AutomaticModeSeconds == 0f);
			}
		}

		public override void CreateLightningBolt(LightningBoltParameters p)
		{
			batchParameters.Add(p);
		}

		public void Trigger()
		{
			Trigger(-1f);
		}

		public void Trigger(float seconds)
		{
			CallLightning();
			if (seconds >= 0f)
			{
				AutomaticModeSeconds = Mathf.Max(0f, seconds);
			}
		}

		public void Trigger(Vector3? start, Vector3? end)
		{
			CallLightning(start, end);
		}

		protected LightningBoltPrefabScriptBase()
		{
			RangeOfFloats rangeOfFloats = new RangeOfFloats
			{
				Minimum = 0.05f,
				Maximum = 0.1f
			};
			IntervalRange = rangeOfFloats;
			CountRange = new RangeOfIntegers
			{
				Minimum = 1,
				Maximum = 1
			};
			CountProbabilityModifier = 1f;
			rangeOfFloats = new RangeOfFloats
			{
				Minimum = 0f,
				Maximum = 0f
			};
			DelayRange = rangeOfFloats;
			rangeOfFloats = new RangeOfFloats
			{
				Minimum = 0.06f,
				Maximum = 0.12f
			};
			DurationRange = rangeOfFloats;
			rangeOfFloats = new RangeOfFloats
			{
				Minimum = 0.1f,
				Maximum = 0.2f
			};
			TrunkWidthRange = rangeOfFloats;
			Generations = 6;
			ChaosFactor = 0.075f;
			ChaosFactorForks = 0.095f;
			Intensity = 1f;
			GlowIntensity = 0.1f;
			GlowWidthMultiplier = 4f;
			FadePercent = 0.15f;
			FadeInMultiplier = 1f;
			FadeFullyLitMultiplier = 1f;
			FadeOutMultiplier = 1f;
			EndWidthMultiplier = 0.5f;
			Forkedness = 0.25f;
			ForkLengthMultiplier = 0.6f;
			ForkLengthVariance = 0.2f;
			ForkEndWidthMultiplier = 1f;
			MaximumLightsPerBatch = 8;
			// base._002Ector();
		}
	}
}
