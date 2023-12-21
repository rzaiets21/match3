using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class DiscoBallAffector : PlayerInput.AffectorBase
	{
		public class DistanceLight : LightSlotComponent.PermanentLight
		{
			public float intensityMult;

			public Settings settings => Match3Settings.instance.discoBallAffectorSettings;

			public override float GetCurrentIntensity(LightSlotComponent component)
			{
				return intensityMult * settings.lightIntensity;
			}
		}

		[Serializable]
		public class Settings
		{
			public float displacePull = 10f;

			public float angleSpeed = 100f;

			public float phaseOffsetMult = 1f;

			public float amplitude = 0.05f;

			public float originScale = 2f;

			public float lightIntensity;

			public FloatRange lightIntensityRange;

			public float timeToFullIntensity;

			public AnimationCurve intensityCurve;

			public bool lockLine;

			public float durationToGetClose = 0.2f;

			public float maxScale = 1.1f;

			public float normalizedClosePosition = 0.5f;

			public float outDuration = 0.2f;

			public float minDuration = 0.75f;

			public float delayTime = 0.5f;

			public float activationDelay = 0.05f;
		}

		public IntVector2 startPosition;

		private DistanceLight distanceLight;

		private float angle;

		private float timeActive;

		public List<Slot> affectedSlots;

		public List<LightingBolt> bolts;

		public Lock globalLock;

		private FlyingSaucerBehaviour saucerBehaviour;

		public Settings settings => Match3Settings.instance.discoBallAffectorSettings;

		public void GiveLightingBoltsTo(List<LightingBolt> destinationBolts)
		{
			destinationBolts.Clear();
			destinationBolts.AddRange(bolts);
			bolts.Clear();
		}

		public override void AddToSwitchSlotArguments(ref Match3Game.SwitchSlotsArguments switchSlotsArguments)
		{
			base.AddToSwitchSlotArguments(ref switchSlotsArguments);
			switchSlotsArguments.bolts = new List<LightingBolt>();
			GiveLightingBoltsTo(switchSlotsArguments.bolts);
		}

		public override void Clear()
		{
			globalLock.UnlockAll();
			for (int i = 0; i < affectedSlots.Count; i++)
			{
				Slot slot = affectedSlots[i];
				if (slot != null)
				{
					slot.light.RemoveLight(distanceLight);
					slot.light.AddLight(distanceLight.GetCurrentIntensity(slot.light));
				}
			}
			for (int j = 0; j < bolts.Count; j++)
			{
				bolts[j].RemoveFromGame();
			}
			bolts.Clear();
			affectorDuration = 0f;
			if (saucerBehaviour != null)
			{
				saucerBehaviour.RemoveFromGame();
			}
		}

		public static void RemoveFromGame(List<LightingBolt> bolts)
		{
			if (bolts == null)
			{
				return;
			}
			for (int i = 0; i < bolts.Count; i++)
			{
				LightingBolt lightingBolt = bolts[i];
				if (!(lightingBolt == null))
				{
					lightingBolt.RemoveFromGame();
				}
			}
		}

		private void UpdateIntensity()
		{
			Settings settings = this.settings;
			float num = Mathf.InverseLerp(0f, settings.timeToFullIntensity, timeActive);
			if (settings.intensityCurve != null)
			{
				num = settings.intensityCurve.Evaluate(num);
			}
			distanceLight.intensityMult = num;
		}

		public override void OnUpdate(PlayerInput.AffectorUpdateParams updateParams)
		{
			Settings settings = this.settings;
			timeActive += Time.deltaTime;
			affectorDuration += Time.deltaTime;
			UpdateIntensity();
			Match3Game game = updateParams.input.game;
			Slot[] slot2 = game.board.slots;
			Vector3 b = game.LocalPositionOfCenter(startPosition);
			angle += Time.deltaTime * settings.angleSpeed;
			for (int i = 0; i < affectedSlots.Count; i++)
			{
				Slot slot = affectedSlots[i];
				if (slot != null)
				{
					Vector3 vector = slot.localPositionOfCenter - b;
					vector.z = 0f;
					float d = Mathf.Sin((settings.phaseOffsetMult * vector.sqrMagnitude + angle) * 57.29578f) * settings.amplitude;
					Vector3 b2 = vector.normalized * d;
					slot.offsetPosition = Vector3.Lerp(slot.prevOffsetPosition, b2, settings.displacePull * Time.deltaTime);
					slot.prevOffsetPosition = slot.offsetPosition;
					slot.positionIntegrator.SetPosition(slot.offsetPosition);
					slot.offsetScale = Vector3.one;
				}
			}
		}
	}
}
