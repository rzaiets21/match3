using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class DiscoChipAffector : ChipAffectorBase
	{
		public class DistanceLight : LightSlotComponent.PermanentLight
		{
			public float intensityMult;

			public DiscoBallAffector.Settings settings => Match3Settings.instance.discoBallAffectorSettings;

			public override float GetCurrentIntensity(LightSlotComponent component)
			{
				return intensityMult * settings.lightIntensity;
			}
		}

		public class BoltSlot
		{
			public float delay;

			public Slot slot;

			public LightingBolt bolt;

			public List<LightingBolt> neighbourBolts = new List<LightingBolt>();
		}

		private Slot originSlot;

		private Slot otherSlot;

		private DistanceLight distanceLight = new DistanceLight();

		private float angle;

		private float timeActive;

		private Match3Game game;

		public List<Slot> affectedSlots = new List<Slot>();

		public List<LightingBolt> bolts = new List<LightingBolt>();

		public List<BoltSlot> boltSlots = new List<BoltSlot>();

		private float time;

		private FlyingSaucerBehaviour saucerBehaviour;

		public DiscoBallAffector.Settings settings => Match3Settings.instance.discoBallAffectorSettings;

		public override void Clear()
		{
			lockContainer.UnlockAll();
			boltSlots.Clear();
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
			if (saucerBehaviour != null)
			{
				saucerBehaviour.RemoveFromGame();
			}
		}

		public void Init(Slot originSlot, Slot otherSlot, Match3Game game)
		{
			ItemColor itemColor = otherSlot.GetSlotComponent<Chip>().itemColor;
			this.otherSlot = otherSlot;
			List<Slot> list = new List<Slot>();
			List<Slot> sortedSlotsUpdateList = game.board.sortedSlotsUpdateList;
			for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
			{
				Slot slot = sortedSlotsUpdateList[i];
				if (slot != null && slot.CanParticipateInDiscoBombAffectedArea(itemColor, replaceWithBombs: false))
				{
					base.globalLock.LockSlot(slot);
					list.Add(slot);
				}
			}
			Init(originSlot, game, list, itemColor);
		}

		public void Init(Slot originSlot, Match3Game game, List<Slot> slots, ItemColor itemColor)
		{
			Clear();
			timeActive = 0f;
			this.game = game;
			this.originSlot = originSlot;
			base.globalLock.isSlotGravitySuspended = true;
			base.globalLock.isChipGeneratorSuspended = true;
			for (int i = 0; i < slots.Count; i++)
			{
				Slot slot = slots[i];
				base.globalLock.LockSlot(slot);
				affectedSlots.Add(slot);
			}
			if (otherSlot != null)
			{
				affectedSlots.Add(otherSlot);
			}
			bolts.Clear();
			boltSlots.Clear();
			distanceLight.intensityMult = 0f;
			float num = 0f;
			if (affectedSlots.Count > 0)
			{
				num = settings.delayTime / (float)affectedSlots.Count;
			}
			for (int j = 0; j < affectedSlots.Count; j++)
			{
				Slot slot2 = affectedSlots[j];
				if (slot2 == null)
				{
					continue;
				}
				slot2.light.AddLight(distanceLight);
				LightingBolt lightingBolt = game.CreateLightingBoltPowerup();
				lightingBolt.Init(originSlot, slot2);
				bolts.Add(lightingBolt);
				lightingBolt.SetEndPositionFromLerp(0f);
				BoltSlot boltSlot = new BoltSlot();
				boltSlots.Add(boltSlot);
				boltSlot.bolt = lightingBolt;
				boltSlot.slot = slot2;
				boltSlot.delay = (float)j * num;
				IntVector2[] upDownLeftRight = IntVector2.upDownLeftRight;
				foreach (IntVector2 b in upDownLeftRight)
				{
					Slot slot3 = game.GetSlot(slot2.position + b);
					if (slot3 != null && slot3.isDestroyedByMatchingNextTo)
					{
						LightingBolt lightingBolt2 = game.CreateLightingBoltChip();
						lightingBolt2.Init(slot2, slot3);
						lightingBolt2.SetPositionFromSlots();
						lightingBolt2.HideParticle();
						GGUtil.SetActive(lightingBolt2, active: false);
						bolts.Add(lightingBolt2);
						boltSlot.neighbourBolts.Add(lightingBolt2);
					}
				}
			}
			saucerBehaviour = game.CreateFlyingSaucer();
			if (saucerBehaviour != null)
			{
				saucerBehaviour.Init(ChipType.Chip, itemColor);
				saucerBehaviour.transform.localPosition = game.LocalPositionOfCenter(originSlot.position);
				GGUtil.SetActive(saucerBehaviour, active: true);
			}
			game.Play(GGSoundSystem.SFXType.DiscoBallElectricity);
		}

		private void SetActive(List<LightingBolt> bolts, bool active)
		{
			for (int i = 0; i < bolts.Count; i++)
			{
				GGUtil.SetActive(bolts[i], active);
			}
		}

		private void UpdateIntensity()
		{
			DiscoBallAffector.Settings settings = this.settings;
			float intensityMult = Mathf.InverseLerp(0f, settings.timeToFullIntensity, timeActive);
			if (settings.intensityCurve != null)
			{
				intensityMult = settings.intensityCurve.Evaluate(intensityMult);
			}
			distanceLight.intensityMult = intensityMult;
		}

		public override void Update()
		{
			DiscoBallAffector.Settings settings = this.settings;
			timeActive += Time.deltaTime;
			UpdateIntensity();
			Slot[] slot2 = game.board.slots;
			Vector3 localPositionOfCenter = originSlot.localPositionOfCenter;
			angle += Time.deltaTime * settings.angleSpeed;
			for (int i = 0; i < boltSlots.Count; i++)
			{
				BoltSlot boltSlot = boltSlots[i];
				bool flag = time >= boltSlot.delay;
				LightingBolt bolt = boltSlot.bolt;
				GGUtil.SetActive(bolt, flag);
				if (flag)
				{
					float num = Mathf.InverseLerp(0f, settings.outDuration, time - boltSlot.delay);
					bolt.SetEndPositionFromLerp(num);
					bool active = num >= 1f;
					SetActive(boltSlot.neighbourBolts, active);
					Slot slot = boltSlot.slot;
					if (slot != null)
					{
						Vector3 vector = slot.localPositionOfCenter - localPositionOfCenter;
						vector.z = 0f;
						float d = Mathf.Sin((settings.phaseOffsetMult * vector.sqrMagnitude + angle) * 57.29578f) * settings.amplitude;
						Vector3 b = vector.normalized * d;
						slot.offsetPosition = Vector3.Lerp(slot.prevOffsetPosition, b, settings.displacePull * Time.deltaTime);
						slot.prevOffsetPosition = slot.offsetPosition;
						slot.positionIntegrator.SetPosition(slot.offsetPosition);
						slot.offsetScale = Vector3.one;
					}
				}
			}
			UpdateCombine();
		}

		public void UpdateCombine()
		{
			time += Time.deltaTime;
			if (otherSlot != null)
			{
				Slot slot = otherSlot;
				Slot slot2 = originSlot;
				DiscoBallAffector.Settings settings = this.settings;
				float t = Mathf.InverseLerp(0f, settings.durationToGetClose, time);
				float d = Mathf.Lerp(1f, settings.maxScale, t);
				float t2 = Mathf.Lerp(0f, settings.normalizedClosePosition, t);
				slot.offsetPosition = Vector3.Lerp(slot.localPositionOfCenter, slot2.localPositionOfCenter, t2) - slot.localPositionOfCenter;
				slot2.offsetScale = Vector3.one * d;
			}
		}
	}
}
