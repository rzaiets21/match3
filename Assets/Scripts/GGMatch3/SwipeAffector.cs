using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class SwipeAffector : PlayerInput.AffectorBase
	{
		public class DistanceLight : LightSlotComponent.PermanentLight
		{
			public float intensityMult;

			public Settings settings => Match3Settings.instance.swipeAffectorSettings;

			public override float GetCurrentIntensity(LightSlotComponent component)
			{
				return intensityMult * settings.lightIntensity;
			}
		}

		[Serializable]
		public class Settings
		{
			public float upScale = 1.2f;

			public float createPowerupScale = 1f;

			public float pullOffset;

			public bool offsetTillMoveEnd;

			public float moveDuration = 0.1f;

			public AnimationCurve moveCurve;

			public float brightness = 1f;

			public float angleSpeed = 100f;

			public float phaseOffsetMult = 1f;

			public float amplitude = 0.05f;

			public float minAffectorDuration = 0.5f;

			public bool ignoreBoltsWithoutPowerup;

			public bool hasMaxAffectorDuration;

			public float maxMaxAffectorDuration = 0.1f;

			public float minAutoMatchDuration = 0.15f;

			public float minAffectorDurationPowerup = 0.5f;

			public float minAffectorDurationMix = 0.5f;

			public float lightIntensity;

			public float shockWaveOffset = 0.1f;

			public bool useParticles;

			public bool autoMatchesProduceLighting;

			public float activeGoalUpScale = 1.4f;

			public bool useAutoMatchDuration;

			public float autoMatchDuration = 0.5f;
		}

		public class AffectedSlot
		{
			public Slot slot;

			public List<Slot> matchingSlotsWithChips;

			public List<Slot> matchingSlots;

			public bool isMatching;

			public bool isCreatingPowerup;

			public bool isPartOfActiveGoal;
		}

		public class InitArguments
		{
			public PlayerInput input;

			public List<AffectedSlot> affectedSlots;

			public bool isMatching
			{
				get
				{
					for (int i = 0; i < affectedSlots.Count; i++)
					{
						if (affectedSlots[i].isMatching)
						{
							return true;
						}
					}
					return false;
				}
			}

			public bool isCreatingPowerup
			{
				get
				{
					for (int i = 0; i < affectedSlots.Count; i++)
					{
						AffectedSlot affectedSlot = affectedSlots[i];
						if (affectedSlot.isMatching && affectedSlot.isCreatingPowerup)
						{
							return true;
						}
					}
					return false;
				}
			}
		}

		private DistanceLight distanceLight;

		public List<LightingBolt> bolts;

		private List<Slot> swapAffectedSlots;

		private InitArguments initArguments;

		private float angle;

		private static IntVector2[] directions = new IntVector2[4]
		{
			IntVector2.left,
			IntVector2.right,
			IntVector2.up,
			IntVector2.down
		};

		public Settings settings => Match3Settings.instance.swipeAffectorSettings;

		public override float minAffectorDuration
		{
			get
			{
				if (initArguments.isCreatingPowerup)
				{
					return settings.minAffectorDurationPowerup;
				}
				if (initArguments.isMatching)
				{
					return settings.minAffectorDuration;
				}
				return 0f;
			}
		}

		public override void AddToSwitchSlotArguments(ref Match3Game.SwitchSlotsArguments switchSlotsArguments)
		{
			base.AddToSwitchSlotArguments(ref switchSlotsArguments);
			switchSlotsArguments.bolts = new List<LightingBolt>();
			GiveLightingBoltsTo(switchSlotsArguments.bolts);
		}

		public override void Clear()
		{
			for (int i = 0; i < swapAffectedSlots.Count; i++)
			{
				swapAffectedSlots[i].light.RemoveLight(distanceLight);
			}
			swapAffectedSlots.Clear();
			affectorDuration = 0f;
			for (int j = 0; j < bolts.Count; j++)
			{
				bolts[j].RemoveFromGame();
			}
			bolts.Clear();
			List<AffectedSlot> affectedSlots = initArguments.affectedSlots;
			for (int k = 0; k < affectedSlots.Count; k++)
			{
				AffectedSlot affectedSlot = affectedSlots[k];
				for (int l = 0; l < affectedSlot.matchingSlotsWithChips.Count; l++)
				{
					Slot slot = affectedSlot.matchingSlotsWithChips[l];
					slot.offsetScale = Vector3.one;
					slot.offsetPosition = Vector3.zero;
				}
			}
		}

		public override void OnBeforeDestroy()
		{
			List<AffectedSlot> affectedSlots = initArguments.affectedSlots;
			Match3Game game = initArguments.input.game;
			Settings settings = this.settings;
			for (int i = 0; i < affectedSlots.Count; i++)
			{
				AffectedSlot affectedSlot = affectedSlots[i];
				for (int j = 0; j < affectedSlot.matchingSlots.Count; j++)
				{
					Slot slot = affectedSlot.matchingSlots[j];
					for (int k = 0; k < directions.Length; k++)
					{
						IntVector2 b = directions[k];
						Slot slot2 = game.GetSlot(slot.position + b);
						if (slot2 != null && !swapAffectedSlots.Contains(slot2))
						{
							slot2.offsetPosition = (slot2.localPositionOfCenter - slot.localPositionOfCenter).normalized * settings.shockWaveOffset;
							slot2.positionIntegrator.currentPosition = slot2.offsetPosition;
						}
					}
				}
			}
		}

		public override void OnUpdate(PlayerInput.AffectorUpdateParams updateParams)
		{
			Settings settings = this.settings;
			affectorDuration += Time.deltaTime;
			angle += Time.deltaTime * settings.angleSpeed;
			PlayerInput.MouseParams mouseParams = updateParams.mouseParams;
			Slot firstHitSlot = mouseParams.firstHitSlot;
			Slot slotToSwitchWith = mouseParams.slotToSwitchWith;
			List<AffectedSlot> affectedSlots = initArguments.affectedSlots;
			for (int i = 0; i < affectedSlots.Count; i++)
			{
				AffectedSlot affectedSlot = affectedSlots[i];
				Slot slot = affectedSlot.slot;
				for (int j = 0; j < affectedSlot.matchingSlotsWithChips.Count; j++)
				{
					Slot slot2 = affectedSlot.matchingSlotsWithChips[j];
					if (affectedSlot.isCreatingPowerup)
					{
						slot2.offsetScale = new Vector3(settings.createPowerupScale, settings.createPowerupScale, 1f);
						if (slot2 == slot)
						{
							slot2.offsetPosition = Vector3.zero;
							continue;
						}
						slot2.offsetPosition = (slot.localPositionOfCenter - slot2.localPositionOfCenter).normalized * settings.pullOffset;
					}
					else if (affectedSlot.isPartOfActiveGoal)
					{
						slot2.offsetScale = new Vector3(settings.activeGoalUpScale, settings.activeGoalUpScale, 1f);
					}
					else
					{
						slot2.offsetScale = new Vector3(settings.upScale, settings.upScale, 1f);
					}
					if (!affectedSlot.isPartOfActiveGoal)
					{
						ApplyShake(slot2, slot.localPositionOfCenter);
					}
					if (slot2.isSlotGravitySuspended)
					{
						slot2.offsetPosition = Vector3.zero;
					}
				}
			}
			for (int k = 0; k < bolts.Count; k++)
			{
				bolts[k].SetPositionFromSlots();
			}
		}

		private void ApplyShake(Slot slot, Vector3 startLocalPosition)
		{
			Vector3 lhs = slot.localPositionOfCenter - startLocalPosition;
			if (lhs == Vector3.zero)
			{
				lhs = Vector3.right;
			}
			lhs.z = 0f;
			float d = Mathf.Sin((settings.phaseOffsetMult * lhs.sqrMagnitude + angle) * 57.29578f) * settings.amplitude;
			Vector3 vector = lhs.normalized * d;
			slot.offsetPosition += vector;
		}

		private void GiveLightingBoltsTo(List<LightingBolt> destinationBolts)
		{
			destinationBolts.Clear();
			destinationBolts.AddRange(bolts);
			bolts.Clear();
		}
	}
}
