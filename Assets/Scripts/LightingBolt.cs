using DigitalRuby.ThunderAndLightning;
using GGMatch3;
using System;
using UnityEngine;

public class LightingBolt : MonoBehaviour
{
	[SerializeField]
	private LightningBoltPrefabScript bolt;

	[SerializeField]
	private Transform particleTransform;

	[NonSerialized]
	public Slot startSlot;

	[NonSerialized]
	public bool isSlotPositionsSet;

	[NonSerialized]
	public IntVector2 startSlotPosition;

	[NonSerialized]
	public IntVector2 endSlotPosition;

	[NonSerialized]
	public Slot endSlot;

	public void Init(Slot startSlot, Slot endSlot)
	{
		this.startSlot = startSlot;
		this.endSlot = endSlot;
		Init(startSlot.localPositionOfCenter, endSlot.localPositionOfCenter);
		SetSlotPositions(startSlot.position, endSlot.position);
	}

	public void SetSlotPositions(IntVector2 startSlotPosition, IntVector2 endSlotPosition)
	{
		isSlotPositionsSet = true;
		this.startSlotPosition = startSlotPosition;
		this.endSlotPosition = endSlotPosition;
	}

	public void Init(Vector3 localStart, Vector3 localEnd)
	{
		base.transform.localPosition = Vector3.zero;
		bolt.Source.transform.localPosition = localStart;
		bolt.Destination.transform.localPosition = localEnd;
		Camera camera = NavigationManager.instance.GetCamera();
		bolt.Camera = camera;
		GGUtil.SetActive(this, active: true);
		GGUtil.SetActive(bolt, active: true);
	}

	public void SetEndPositionFromLerp(float n)
	{
		if (endSlot != null)
		{
			SetEndPosition(Vector3.Lerp(bolt.Source.transform.localPosition, endSlot.localPositionOfCenter + endSlot.offsetPosition, n));
		}
	}

	public void SetEndPosition(Vector3 localPosition)
	{
		if (startSlot != null)
		{
			bolt.Destination.transform.localPosition = localPosition;
		}
	}

	public void SetStartPosition(Vector3 localPosition)
	{
		bolt.Source.transform.localPosition = localPosition;
	}

	public void SetPositionFromSlots()
	{
		if (startSlot != null)
		{
			bolt.Source.transform.localPosition = startSlot.localPositionOfCenter + startSlot.offsetPosition;
		}
		if (endSlot != null)
		{
			bolt.Destination.transform.localPosition = endSlot.localPositionOfCenter + endSlot.offsetPosition;
		}
	}

	public void SetPositionFromChips()
	{
		if (startSlot != null)
		{
			Chip slotComponent = startSlot.GetSlotComponent<Chip>();
			TransformBehaviour transformBehaviour = null;
			if (slotComponent != null)
			{
				transformBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
			}
			if (transformBehaviour != null)
			{
				bolt.Source.transform.localPosition = transformBehaviour.localPosition;
			}
			else
			{
				bolt.Source.transform.localPosition = startSlot.localPositionOfCenter + startSlot.offsetPosition;
			}
		}
		if (endSlot != null)
		{
			Chip slotComponent2 = endSlot.GetSlotComponent<Chip>();
			TransformBehaviour transformBehaviour2 = null;
			if (slotComponent2 != null)
			{
				transformBehaviour2 = slotComponent2.GetComponentBehaviour<TransformBehaviour>();
			}
			if (transformBehaviour2 != null)
			{
				bolt.Destination.transform.localPosition = transformBehaviour2.localPosition;
			}
			else
			{
				bolt.Destination.transform.localPosition = endSlot.localPositionOfCenter + endSlot.offsetPosition;
			}
		}
	}

	public void HideParticle()
	{
		GGUtil.Hide(particleTransform);
	}

	public void RemoveFromGame()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
