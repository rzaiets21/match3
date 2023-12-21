using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class PowerCrossChipAffector : ChipAffectorBase
	{
		public Slot originSlot;

		public List<Slot> affectedSlots = new List<Slot>();

		public List<LightingBolt> bolts = new List<LightingBolt>();

		private Match3Game game;

		private int radius;

		private float angle;

		private float affectorDuration;

		private PowerCrossAffector.Settings settings => Match3Settings.instance.powerCrossAffectorSettings;

		public override void Clear()
		{
			for (int i = 0; i < bolts.Count; i++)
			{
				bolts[i].RemoveFromGame();
			}
			bolts.Clear();
			lockContainer.UnlockAll();
			for (int j = 0; j < affectedSlots.Count; j++)
			{
				affectedSlots[j].offsetPosition = Vector3.zero;
			}
		}

		public override void AddToInputAffectorExport(Match3Game.InputAffectorExport inputAffector)
		{
			if (originSlot != null)
			{
				Match3Game.InputAffectorExport.InputAffectorForSlot inputAffectorForSlot = new Match3Game.InputAffectorExport.InputAffectorForSlot();
				inputAffectorForSlot.bolts.AddRange(bolts);
				bolts.Clear();
				inputAffectorForSlot.slot = originSlot;
				inputAffector.affectorExports.Add(inputAffectorForSlot);
			}
		}

		public void Init(Slot originSlot, Match3Game game, int radius)
		{
			Clear();
			this.game = game;
			this.radius = radius;
			this.originSlot = originSlot;
			base.globalLock.isSlotGravitySuspended = true;
			base.globalLock.isChipGeneratorSuspended = true;
			IntVector2 position = originSlot.position;
			Slot[] slots = game.board.slots;
			foreach (Slot slot in slots)
			{
				if (slot == null || slot == originSlot)
				{
					continue;
				}
				IntVector2 intVector = slot.position - position;
				int num = Mathf.Abs(intVector.x);
				int num2 = Mathf.Abs(intVector.y);
				int num3 = Mathf.Min(num, num2);
				int num4 = Mathf.Max(num, num2);
				if (num3 > radius)
				{
					continue;
				}
				base.globalLock.LockSlot(slot);
				if (!Match3Settings.instance.playerInputSettings.useSimpleLineBolts)
				{
					LightingBolt lightingBolt = game.CreateLightingBoltChip();
					lightingBolt.Init(originSlot, slot);
					IntVector2 position2 = originSlot.position;
					if (num3 > 0)
					{
						if (num4 <= radius)
						{
							position2.y = slot.position.y;
						}
						else if (num2 < num)
						{
							position2.y = slot.position.y;
						}
						else
						{
							position2.x = slot.position.x;
						}
					}
					lightingBolt.SetSlotPositions(position2, slot.position);
					lightingBolt.SetStartPosition(game.LocalPositionOfCenter(position2));
					lightingBolt.SetEndPositionFromLerp(0f);
					bolts.Add(lightingBolt);
				}
				affectedSlots.Add(slot);
			}
			if (!Match3Settings.instance.playerInputSettings.useSimpleLineBolts)
			{
				return;
			}
			for (int j = 0; j < 2; j++)
			{
				IntVector2 a = (j == 0) ? IntVector2.up : IntVector2.left;
				for (int k = -radius; k <= radius; k++)
				{
					for (int l = 0; l < 2; l++)
					{
						IntVector2 direction = (l == 0) ? a.orthogonal : (-a.orthogonal);
						IntVector2 intVector2 = originSlot.position + a * k;
						Slot slot2 = game.FirstSlotOnDirection(intVector2, direction);
						if (slot2 != null)
						{
							Slot slot3 = game.LastSlotOnDirection(slot2, direction);
							if (slot3 != null)
							{
								LightingBolt lightingBolt2 = game.CreateLightingBoltChip();
								lightingBolt2.Init(originSlot, slot3);
								lightingBolt2.SetSlotPositions(intVector2, slot3.position);
								lightingBolt2.SetStartPosition(game.LocalPositionOfCenter(intVector2));
								lightingBolt2.SetEndPositionFromLerp(0f);
								bolts.Add(lightingBolt2);
							}
						}
					}
				}
			}
		}

		public override void Update()
		{
			angle += Time.deltaTime * settings.angleSpeed;
			affectorDuration += Time.deltaTime;
			float endPositionFromLerp = Mathf.InverseLerp(0f, settings.outDuration, affectorDuration);
			for (int i = 0; i < bolts.Count; i++)
			{
				bolts[i].SetEndPositionFromLerp(endPositionFromLerp);
			}
			for (int j = 0; j < affectedSlots.Count; j++)
			{
				Slot slot = affectedSlots[j];
				ApplyShake(slot, originSlot.localPositionOfCenter);
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
	}
}
