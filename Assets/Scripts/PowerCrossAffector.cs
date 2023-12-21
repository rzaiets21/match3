using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerCrossAffector : PlayerInput.AffectorBase
{
	[Serializable]
	public class Settings
	{
		public float minAffectorDuration = 0.25f;

		public float outDuration = 0.1f;

		public float angleSpeed = 100f;

		public float phaseOffsetMult = 1f;

		public float amplitude = 0.05f;
	}

	public IntVector2 startPosition;

	public List<Slot> affectedSlots;

	public List<Slot> lockedSlots;

	public List<LightingBolt> bolts;

	public Lock globalLock;

	private PlayerInput input;

	private float angle;

	private Settings settings => Match3Settings.instance.powerCrossAffectorSettings;

	public override float minAffectorDuration => settings.minAffectorDuration;

	public override void Clear()
	{
		affectorDuration = 0f;
		for (int i = 0; i < bolts.Count; i++)
		{
			bolts[i].RemoveFromGame();
		}
		bolts.Clear();
		Slot.RemoveLocks(lockedSlots, globalLock);
	}

	public override void OnUpdate(PlayerInput.AffectorUpdateParams updateParams)
	{
		angle += Time.deltaTime * settings.angleSpeed;
		affectorDuration += Time.deltaTime;
		float endPositionFromLerp = Mathf.InverseLerp(0f, settings.outDuration, affectorDuration);
		for (int i = 0; i < bolts.Count; i++)
		{
			bolts[i].SetEndPositionFromLerp(endPositionFromLerp);
		}
		Slot slot = input.game.GetSlot(startPosition);
		for (int j = 0; j < affectedSlots.Count; j++)
		{
			Slot slot2 = affectedSlots[j];
			ApplyShake(slot2, slot.localPositionOfCenter);
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
