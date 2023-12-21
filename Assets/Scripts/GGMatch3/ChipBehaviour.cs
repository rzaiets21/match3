using System;
using UnityEngine;

namespace GGMatch3
{
	public class ChipBehaviour : SlotComponentBehavoiour
	{
		[SerializeField]
		private SquareClothRenderer clothRenderer;

		[SerializeField]
		private ClothDemo cloth;

		[NonSerialized]
		private Chip chip;

		[SerializeField]
		private Transform feather;

		[SerializeField]
		private Transform partOfGoal;

		[SerializeField]
		private Animator chipAnimator;

		[SerializeField]
		private Transform chipTransform;

		[SerializeField]
		private Transform particleParent;

		private GameObject particlesGameObject;

		private Transform prevParticleParent;

		public Vector3 localPosition
		{
			get
			{
				return base.transform.localPosition;
			}
			set
			{
				base.transform.localPosition = value;
			}
		}

		public bool hasBounce
		{
			get
			{
				return !cloth.directlyFollow;
			}
			set
			{
				cloth.directlyFollow = !value;
			}
		}

		public void Init(Chip chip)
		{
			this.chip = chip;
			clothRenderer.SetClothTexture(chip.itemColor);
			chip.SetTransformToMove(base.transform);
			clothRenderer.UpdateMaterialSettings();
			GGUtil.SetActive(feather, active: false);
		}

		public void SetFeatherActive(bool active)
		{
			GGUtil.SetActive(feather, active);
		}

		public void ChangeClothTexture(ItemColor itemColor)
		{
			clothRenderer.SetClothTexture(chip.itemColor);
			ResetVisually();
		}

		public void ResetCloth()
		{
			cloth.Init();
		}

		public void ResetVisually()
		{
			if (chip != null && chip.slot != null)
			{
				base.transform.localPosition = chip.slot.localPositionOfCenter;
			}
			cloth.Init();
		}

		public void StartChipDestroyAnimation(GameObject particles)
		{
			if (!(chipAnimator == null))
			{
				chipAnimator.gameObject.SetActive(value: true);
				if (particles != null && particleParent != null)
				{
					prevParticleParent = particles.transform.parent;
					particlesGameObject = particles;
					particles.transform.parent = particleParent;
				}
			}
		}

		public void SetBrightness(float brightness)
		{
			clothRenderer.SetBrightness(brightness);
		}

		public override void RemoveFromGame()
		{
			if (particlesGameObject != null)
			{
				particlesGameObject.transform.parent = prevParticleParent;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public void SetActive(bool active)
		{
			GGUtil.SetActive(this, active);
		}

		private void LateUpdate()
		{
			if (!(chipAnimator == null) && chipAnimator.gameObject.activeSelf)
			{
				TransformBehaviour component = GetComponent<TransformBehaviour>();
				if (!(component == null))
				{
					component.localScale = chipTransform.localScale;
				}
			}
		}
	}
}
