using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class BombChipAffector : ChipAffectorBase
{
	public enum PowerupType
	{
		Block,
		Bomb,
		Seeking
	}

	public Slot originSlot;

	public List<Slot> affectedSlots = new List<Slot>();

	public List<LightingBolt> bolts = new List<LightingBolt>();

	private Match3Game game;

	private int radius;

	private bool doPlus;

	private float affectorDuration;

	private float angle;

	private SeekingMissleAffector.Settings settings => Match3Settings.instance.seekingMissleAffectorSettings;

	public override void Clear()
	{
		for (int i = 0; i < affectedSlots.Count; i++)
		{
			Slot slot = affectedSlots[i];
			slot.offsetPosition = Vector3.zero;
			slot.offsetScale = Vector3.one;
		}
		for (int j = 0; j < bolts.Count; j++)
		{
			bolts[j].RemoveFromGame();
		}
		affectedSlots.Clear();
		bolts.Clear();
		lockContainer.UnlockAll();
	}

	public void Init(Slot originSlot, Match3Game game, int radius, bool doPlus, PowerupType powerupType)
	{
		Clear();
		this.originSlot = originSlot;
		this.game = game;
		this.doPlus = doPlus;
		base.globalLock.isSlotGravitySuspended = true;
		base.globalLock.isChipGeneratorSuspended = true;
		this.radius = radius;
		IntVector2 position = originSlot.position;
		List<Slot> list = game.GetArea(position, radius);
		switch (powerupType)
		{
		case PowerupType.Bomb:
			list = game.GetBombArea(position, radius);
			break;
		case PowerupType.Seeking:
			list = game.GetSeekingMissleArea(position);
			break;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Slot slot = list[i];
			if (slot == null)
			{
				continue;
			}
			IntVector2 intVector = slot.position - position;
			int num = Mathf.Max(Mathf.Abs(intVector.x), Mathf.Abs(intVector.y));
			if (doPlus)
			{
				num = Mathf.Abs(intVector.x) + Mathf.Abs(intVector.y);
			}
			if (num <= radius)
			{
				base.globalLock.LockSlot(slot);
				if (!Match3Settings.instance.playerInputSettings.disableBombLighting | doPlus)
				{
					LightingBolt lightingBolt = game.CreateLightingBoltPowerup();
					lightingBolt.Init(originSlot, slot);
					lightingBolt.SetEndPositionFromLerp(0f);
					bolts.Add(lightingBolt);
				}
				affectedSlots.Add(slot);
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
