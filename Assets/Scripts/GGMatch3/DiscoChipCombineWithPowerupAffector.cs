using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class DiscoChipCombineWithPowerupAffector : ChipAffectorBase
	{
		public class ExecuteAction : Match3Game.IAffectorExportAction
		{
			public struct InitArguments
			{
				public Match3Game game;

				public Slot originSlot;

				public Slot powerupSlot;

				public List<BoltSlot> boltSlots;
			}

			private InitArguments initArguments;

			public void Init(InitArguments initArguments)
			{
				this.initArguments = initArguments;
			}

			public void Execute()
			{
				DiscoBallDestroyAction.Settings discoBallDestroyActionSetting = Match3Settings.instance.discoBallDestroyActionSettings;
				Slot originSlot = initArguments.originSlot;
				Slot powerupSlot = initArguments.powerupSlot;
				List<BoltSlot> boltSlots = initArguments.boltSlots;
				Match3Game game = initArguments.game;
				Chip slotComponent = originSlot.GetSlotComponent<Chip>();
				slotComponent?.RemoveFromGame();
				bool isHavingCarpet = false;
				if (originSlot.canCarpetSpreadFromHere || powerupSlot.canCarpetSpreadFromHere)
				{
					isHavingCarpet = true;
				}
				SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
				slotDestroyParams.isHitByBomb = true;
				slotDestroyParams.isHavingCarpet = isHavingCarpet;
				slotDestroyParams.isBombAllowingNeighbourDestroy = true;
				slotDestroyParams.bombType = ChipType.DiscoBall;
				powerupSlot.OnDestroySlot(slotDestroyParams);
				for (int i = 0; i < boltSlots.Count; i++)
				{
					Slot slot = boltSlots[i].slot;
					if (slot != null)
					{
						slotDestroyParams.activationDelay = (float)i * Match3Settings.instance.discoBallAffectorSettings.activationDelay;
						slot.OnDestroySlot(slotDestroyParams);
					}
				}
				if (slotComponent != null)
				{
					DestroyChipActionGrow destroyChipActionGrow = new DestroyChipActionGrow();
					destroyChipActionGrow.Init(slotComponent, slotComponent.lastConnectedSlot);
					game.board.actionManager.AddAction(destroyChipActionGrow);
				}
			}

			public void OnCancel()
			{
				Execute();
			}
		}

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

			public BoardAction action;

			public bool shouldReplace;

			public bool isReplaced;

			public bool isNotReplaced => !isReplaced;
		}

		private Slot originSlot;

		private Slot otherSlot;

		private ChipType bombType;

		private DistanceLight distanceLight = new DistanceLight();

		private float angle;

		private float timeActive;

		private Match3Game game;

		public List<Slot> affectedSlots = new List<Slot>();

		public List<LightingBolt> bolts = new List<LightingBolt>();

		private FlyingSaucerBehaviour saucerBehaviour;

		public List<BoltSlot> boltSlots = new List<BoltSlot>();

		private float time;

		public DiscoBallAffector.Settings settings => Match3Settings.instance.discoBallAffectorSettings;

		public override bool canFinish
		{
			get
			{
				for (int i = 0; i < boltSlots.Count; i++)
				{
					BoltSlot boltSlot = boltSlots[i];
					if (boltSlot.shouldReplace && boltSlot.isNotReplaced)
					{
						return false;
					}
					if (boltSlot.action != null && boltSlot.action.isAlive)
					{
						return false;
					}
				}
				return true;
			}
		}

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

		public override void AddToInputAffectorExport(Match3Game.InputAffectorExport inputAffector)
		{
			base.AddToInputAffectorExport(inputAffector);
			ExecuteAction executeAction = new ExecuteAction();
			ExecuteAction.InitArguments initArguments = default(ExecuteAction.InitArguments);
			initArguments.originSlot = originSlot;
			initArguments.powerupSlot = otherSlot;
			initArguments.game = game;
			initArguments.boltSlots = new List<BoltSlot>();
			for (int i = 0; i < boltSlots.Count; i++)
			{
				BoltSlot boltSlot = boltSlots[i];
				if (boltSlot.shouldReplace)
				{
					initArguments.boltSlots.Add(boltSlot);
				}
			}
			executeAction.Init(initArguments);
			inputAffector.AddAction(executeAction);
		}

		public void Init(Slot originSlot, Slot powerupSlot, ChipType bombType, Match3Game game, List<Slot> slots)
		{
			Clear();
			this.bombType = bombType;
			timeActive = 0f;
			this.game = game;
			this.originSlot = originSlot;
			otherSlot = powerupSlot;
			base.globalLock.SuspendAll();
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
				if (slot2 != null)
				{
					slot2.light.AddLight(distanceLight);
					LightingBolt lightingBolt = game.CreateLightingBoltPowerup();
					lightingBolt.HideParticle();
					lightingBolt.Init(originSlot, slot2);
					bolts.Add(lightingBolt);
					lightingBolt.SetEndPositionFromLerp(0f);
					BoltSlot boltSlot = new BoltSlot();
					boltSlots.Add(boltSlot);
					boltSlot.bolt = lightingBolt;
					boltSlot.slot = slot2;
					boltSlot.delay = (float)j * num;
					boltSlot.shouldReplace = (slot2 != otherSlot && slot2 != originSlot);
				}
			}
			saucerBehaviour = game.CreateFlyingSaucer();
			if (saucerBehaviour != null)
			{
				saucerBehaviour.Init(bombType, ItemColor.Uncolored);
				saucerBehaviour.transform.localPosition = game.LocalPositionOfCenter(originSlot.position);
				GGUtil.SetActive(saucerBehaviour, active: true);
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
					bool flag2 = num >= 1f;
					if ((boltSlot.shouldReplace && boltSlot.isNotReplaced) & flag2)
					{
						boltSlot.isReplaced = true;
						ReplaceChipWithPowerup(boltSlot);
					}
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

		private void ReplaceChipWithPowerup(BoltSlot boltSlot)
		{
			Slot slot = boltSlot.slot;
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
			slotDestroyParams.isHitByBomb = true;
			slotDestroyParams.isHavingCarpet = false;
			slotDestroyParams.isBombAllowingNeighbourDestroy = true;
			slotDestroyParams.bombType = ChipType.DiscoBall;
			slotComponent?.RemoveFromGameWithPickupGoal(slotDestroyParams);
			ChipType chipType = bombType;
			if (chipType == ChipType.HorizontalRocket || chipType == ChipType.VerticalRocket)
			{
				chipType = ((game.RandomRange(0, 100) < 50) ? ChipType.VerticalRocket : ChipType.HorizontalRocket);
			}
			CreatePowerupAction createPowerupAction = new CreatePowerupAction();
			CreatePowerupAction.CreateParams createParams = default(CreatePowerupAction.CreateParams);
			createParams.positionWherePowerupWillBeCreated = slot.position;
			createParams.powerupToCreate = chipType;
			createParams.game = game;
			createPowerupAction.Init(createParams);
			boltSlot.action = createPowerupAction;
			game.board.actionManager.AddAction(createPowerupAction);
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
