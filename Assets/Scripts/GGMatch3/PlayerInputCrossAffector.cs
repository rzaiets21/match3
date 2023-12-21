using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class PlayerInputCrossAffector : PlayerInput.AffectorBase
	{
		public class DistanceLight : LightSlotComponent.PermanentLight
		{
			public IntVector2 startPosition;

			public float intensityMult;

			public Settings settings => Match3Settings.instance.playerInputCrossAffectorSettings;

			public override float GetCurrentIntensity(LightSlotComponent component)
			{
				Slot slot = component.slot;
				if (slot == null)
				{
					return 0f;
				}
				int num = Mathf.Max(Mathf.Abs(slot.position.x - startPosition.x), Mathf.Abs(slot.position.y - startPosition.y));
				return intensityMult * settings.lightIntensityRange.Lerp(Mathf.InverseLerp(settings.maxLightDistance, 0f, num));
			}
		}

		[Serializable]
		public class Settings
		{
			public float maxDistance = 10f;

			public FloatRange displaceRange = new FloatRange(0f, 1f);

			public AnimationCurve displaceCurve;

			public float affectedOrtho = 1f;

			public float displacePull = 10f;

			public float angleSpeed = 100f;

			public float phaseOffsetMult = 1f;

			public float amplitude = 0.05f;

			public float originScale = 2f;

			public float lightIntensity = 1f;

			public float distanceDelay = 0.05f;

			public FloatRange lightIntensityRange;

			public float maxLightDistance = 5f;

			public float timeToFullIntensity;

			public AnimationCurve intensityCurve;

			public bool lockLine;

			public FloatRange scaleRange = new FloatRange(1f, 0.5f);

			public float orthoScaleInfluence = 0.25f;
		}

		public IntVector2 startPosition;

		private DistanceLight distanceLight;

		private float angle;

		private float timeActive;

		private int radius;

		public List<Slot> affectedSlots;

		public List<Slot> lockedSlots;

		public Lock globalLock;

		public Settings settings => Match3Settings.instance.playerInputCrossAffectorSettings;

		public override void Clear()
		{
			Slot.RemoveLocks(lockedSlots, globalLock);
			for (int i = 0; i < affectedSlots.Count; i++)
			{
				Slot slot = affectedSlots[i];
				if (slot != null)
				{
					slot.light.RemoveLight(distanceLight);
					slot.light.AddLight(distanceLight.GetCurrentIntensity(slot.light));
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
			UpdateIntensity();
			Match3Game game = updateParams.input.game;
			Slot[] slots = game.board.slots;
			Vector3 b = game.LocalPositionOfCenter(startPosition);
			angle += Time.deltaTime * settings.angleSpeed;
			foreach (Slot slot in slots)
			{
				if (slot == null)
				{
					continue;
				}
				Vector3 a = slot.localPositionOfCenter;
				Chip slotComponent = slot.GetSlotComponent<Chip>();
				if (slotComponent != null)
				{
					TransformBehaviour componentBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
					if (componentBehaviour != null)
					{
						a = componentBehaviour.localPosition;
					}
				}
				Vector3 vector = a - b;
				vector.z = 0f;
				float num = Mathf.Abs(vector.x);
				float num2 = Mathf.Abs(vector.y);
				float num3 = Mathf.Min(num, num2);
				float num4 = Mathf.Max(num, num2);
				bool flag = num4 <= (float)radius;
				bool flag2 = num3 <= (float)radius;
				if (vector == Vector3.zero)
				{
					slot.prevOffsetPosition = (slot.offsetPosition = Vector3.zero);
					slot.offsetScale = new Vector3(settings.originScale, settings.originScale, 1f);
					slot.positionIntegrator.ResetAcceleration();
					continue;
				}
				if (flag)
				{
					slot.prevOffsetPosition = (slot.offsetPosition = Vector3.zero);
					slot.offsetScale = new Vector3(1f, 1f, 1f);
					slot.positionIntegrator.ResetAcceleration();
					continue;
				}
				Vector3 zero = Vector3.zero;
				zero = ((!(num > num2)) ? Vector3.up : Vector3.right);
				float num5 = 0f;
				float num6 = 0f;
				Vector3 vector2 = vector;
				if (zero.y != 0f)
				{
					vector2 = Vector3.right * Mathf.Sign(vector.x);
					num5 = Mathf.Abs(vector.y);
					num6 = Mathf.Abs(vector.x);
				}
				else
				{
					vector2 = Vector3.up * Mathf.Sign(vector.y);
					num5 = Mathf.Abs(vector.x);
					num6 = Mathf.Abs(vector.y);
				}
				num6 = Mathf.Max(0f, num6 - (float)radius);
				if (num6 >= settings.affectedOrtho)
				{
					continue;
				}
				if (flag2)
				{
					vector = zero * Mathf.Sign(Vector3.Dot(zero, vector));
				}
				float num7 = Mathf.InverseLerp(0f, settings.affectedOrtho, num6);
				float num8 = Mathf.Lerp(1f, 0f, num7);
				float num9 = Mathf.InverseLerp(num4, 0f, num5);
				if (settings.displaceCurve != null)
				{
					num9 = settings.displaceCurve.Evaluate(num9);
				}
				float num10 = settings.displaceRange.Lerp(num9);
				float num11 = Mathf.Sin((settings.phaseOffsetMult * num5 + angle) * 57.29578f) * settings.amplitude;
				Vector3 zero2 = Vector3.zero;
				float num12 = 1f;
				if (num7 != 0f)
				{
					float t = Mathf.InverseLerp(0.5f, 0f, Mathf.Max(Mathf.Abs(slot.prevOffsetPosition.x), Mathf.Abs(slot.prevOffsetPosition.y)));
					zero2 = vector2 * (num10 * num8);
					if (num == num2)
					{
						zero2 = vector.normalized * (num10 * num8);
					}
					num12 = settings.scaleRange.Lerp(t);
				}
				else
				{
					zero2 = vector.normalized * (num11 + num10);
				}
				slot.offsetPosition = Vector3.Lerp(slot.prevOffsetPosition, zero2, settings.displacePull * Time.deltaTime);
				slot.prevOffsetPosition = slot.offsetPosition;
				slot.positionIntegrator.SetPosition(slot.offsetPosition);
				slot.offsetScale = Vector3.one;
				if (num == num2)
				{
					slot.offsetScale = new Vector3(num12, num12, 1f);
					continue;
				}
				float num13 = 1f;
				if (num12 != 1f)
				{
					num13 = 1f + num12 * settings.orthoScaleInfluence;
				}
				if (zero.x != 0f)
				{
					slot.offsetScale = new Vector3(num13, num12, 1f);
				}
				else
				{
					slot.offsetScale = new Vector3(num12, num13, 1f);
				}
			}
		}
	}
}
