using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class MatchChipAffector : ChipAffectorBase
{
	public class InitArguments
	{
		public bool cameFromPositionSet;

		public IntVector2 cameFromPosition;

		public Chip otherChipToMove;

		public Match3Game game;

		public Slot originSlot;

		public bool isCreatingPowerup;

		public List<Slot> matchingSlots = new List<Slot>();

		public List<Slot> ignoreSlots = new List<Slot>();

		public bool ShouldIgnore(Slot slot)
		{
			return ignoreSlots.Contains(slot);
		}
	}

	private float angle;

	private InitArguments arguments;

	public List<LightingBolt> bolts = new List<LightingBolt>();

	private static IntVector2[] directionsDiagonal = new IntVector2[4]
	{
		new IntVector2(-1, 1),
		new IntVector2(1, 1),
		new IntVector2(-1, -1),
		new IntVector2(1, -1)
	};

	private static IntVector2[] directions = new IntVector2[4]
	{
		IntVector2.left,
		IntVector2.right,
		IntVector2.up,
		IntVector2.down
	};

	private float duration;

	private TransformBehaviour moveBehaviour;

	private TransformBehaviour otherChipMoveBehaviour;

	private static List<Slot> slotsAffectedByExplosion = new List<Slot>();

	public override void AddToInputAffectorExport(Match3Game.InputAffectorExport inputAffector)
	{
		if (arguments.originSlot != null)
		{
			SetBrightness(1f);
			UpdateMoveBehaviour(1f);
			Match3Game.InputAffectorExport.InputAffectorForSlot inputAffectorForSlot = new Match3Game.InputAffectorExport.InputAffectorForSlot();
			inputAffectorForSlot.bolts.AddRange(bolts);
			bolts.Clear();
			inputAffectorForSlot.slot = arguments.originSlot;
			inputAffector.affectorExports.Add(inputAffectorForSlot);
		}
	}

	public override void GiveLightingBoltsTo(List<LightingBolt> destinationBolts)
	{
		destinationBolts.AddRange(bolts);
		bolts.Clear();
	}

	public void Init(InitArguments initArguments)
	{
		arguments = initArguments;
		Match3Game game = initArguments.game;
		List<Slot> matchingSlots = initArguments.matchingSlots;
		Slot originSlot = initArguments.originSlot;
		Chip slotComponent = originSlot.GetSlotComponent<Chip>();
		if (slotComponent != null)
		{
			moveBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
		}
		if (arguments.otherChipToMove != null)
		{
			otherChipMoveBehaviour = arguments.otherChipToMove.GetComponentBehaviour<TransformBehaviour>();
		}
		if (!Match3Settings.instance.swipeAffectorSettings.ignoreBoltsWithoutPowerup || arguments.isCreatingPowerup || !arguments.cameFromPositionSet)
		{
			for (int i = 0; i < matchingSlots.Count; i++)
			{
				Slot slot = matchingSlots[i];
				LightingBolt lightingBolt = null;
				lightingBolt = ((!initArguments.isCreatingPowerup) ? game.CreateLightingBoltChip() : game.CreateLightingBoltPowerup());
				lightingBolt.Init(originSlot, slot);
				lightingBolt.SetPositionFromSlots();
				bolts.Add(lightingBolt);
				GGUtil.SetActive(lightingBolt, active: true);
				IntVector2[] upDownLeftRight = IntVector2.upDownLeftRight;
				foreach (IntVector2 b in upDownLeftRight)
				{
					Slot slot2 = game.GetSlot(slot.position + b);
					if (slot2 != null && slot2.isDestroyedByMatchingNextTo && !arguments.ShouldIgnore(slot2))
					{
						LightingBolt lightingBolt2 = game.CreateLightingBoltChip();
						lightingBolt2.Init(slot, slot2);
						lightingBolt2.SetPositionFromSlots();
						lightingBolt2.HideParticle();
						GGUtil.SetActive(lightingBolt2, active: true);
						bolts.Add(lightingBolt2);
					}
				}
			}
		}
		UpdateMoveBehaviour(0f);
		for (int k = 0; k < bolts.Count; k++)
		{
			bolts[k].SetPositionFromChips();
		}
	}

	private void ShowBolts(bool show)
	{
		for (int i = 0; i < bolts.Count; i++)
		{
			GGUtil.SetActive(bolts[i], show);
		}
	}

	private void SetBrightness(float brightness)
	{
		List<Slot> matchingSlots = arguments.matchingSlots;
		for (int i = 0; i < matchingSlots.Count; i++)
		{
			Chip slotComponent = matchingSlots[i].GetSlotComponent<Chip>();
			if (slotComponent != null)
			{
				ChipBehaviour componentBehaviour = slotComponent.GetComponentBehaviour<ChipBehaviour>();
				if (!(componentBehaviour == null))
				{
					componentBehaviour.SetBrightness(brightness);
				}
			}
		}
	}

	private void UpdateMoveBehaviour(float n)
	{
		if (!arguments.cameFromPositionSet)
		{
			ShowBolts(show: true);
			return;
		}
		Vector3 vector = arguments.game.LocalPositionOfCenter(arguments.cameFromPosition);
		Vector3 localPositionOfCenter = arguments.originSlot.localPositionOfCenter;
		if (moveBehaviour != null)
		{
			Vector3 localPosition = Vector3.Lerp(vector, localPositionOfCenter, n);
			moveBehaviour.localPosition = localPosition;
		}
		if (otherChipMoveBehaviour != null)
		{
			Vector3 localPosition2 = Vector3.Lerp(localPositionOfCenter, vector, n);
			otherChipMoveBehaviour.localPosition = localPosition2;
		}
		ShowBolts(n >= 1f);
	}

	public override void Clear()
	{
		List<Slot> matchingSlots = arguments.matchingSlots;
		for (int i = 0; i < matchingSlots.Count; i++)
		{
			Slot slot = matchingSlots[i];
			slot.offsetScale = Vector3.one;
			slot.offsetPosition = Vector3.zero;
			SetBrightness(1f);
		}
		for (int j = 0; j < bolts.Count; j++)
		{
			bolts[j].RemoveFromGame();
		}
		bolts.Clear();
	}

	public override void OnAfterDestroy()
	{
	}

	public override void Update()
	{
		SwipeAffector.Settings swipeAffectorSettings = Match3Settings.instance.swipeAffectorSettings;
		base.Update();
		duration += Time.deltaTime;
		angle += Time.deltaTime * swipeAffectorSettings.angleSpeed;
		float num = Mathf.InverseLerp(0f, swipeAffectorSettings.moveDuration, duration);
		float n = swipeAffectorSettings.moveCurve.Evaluate(num);
		UpdateMoveBehaviour(n);
		List<Slot> matchingSlots = arguments.matchingSlots;
		for (int i = 0; i < matchingSlots.Count; i++)
		{
			Slot slot = matchingSlots[i];
			Slot originSlot = arguments.originSlot;
			if (arguments.isCreatingPowerup)
			{
				slot.offsetScale = new Vector3(swipeAffectorSettings.createPowerupScale, swipeAffectorSettings.createPowerupScale, 1f);
				if (slot == originSlot)
				{
					slot.offsetPosition = Vector3.zero;
					continue;
				}
				slot.offsetPosition = (originSlot.localPositionOfCenter - slot.localPositionOfCenter).normalized * swipeAffectorSettings.pullOffset;
			}
			else
			{
				slot.offsetScale = new Vector3(swipeAffectorSettings.upScale, swipeAffectorSettings.upScale, 1f);
			}
			if (!swipeAffectorSettings.offsetTillMoveEnd || !arguments.cameFromPositionSet || num >= 1f)
			{
				ApplyShake(slot, originSlot.localPositionOfCenter);
				SetBrightness(swipeAffectorSettings.brightness);
			}
			if (slot.isSlotGravitySuspended)
			{
				slot.offsetPosition = Vector3.zero;
			}
		}
		for (int j = 0; j < bolts.Count; j++)
		{
			bolts[j].SetPositionFromChips();
		}
	}

	private void ApplyShake(Slot slot, Vector3 startLocalPosition)
	{
		if (bolts.Count != 0)
		{
			SwipeAffector.Settings swipeAffectorSettings = Match3Settings.instance.swipeAffectorSettings;
			Vector3 lhs = slot.localPositionOfCenter - startLocalPosition;
			lhs.z = 0f;
			if (lhs == Vector3.zero)
			{
				lhs = Vector3.right;
			}
			float d = Mathf.Sin((swipeAffectorSettings.phaseOffsetMult * lhs.sqrMagnitude + angle) * 57.29578f) * swipeAffectorSettings.amplitude;
			Vector3 vector = lhs.normalized * d;
			slot.offsetPosition += vector;
		}
	}
}
