using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GGMatch3
{
	public class TransformBehaviour : SlotComponentBehavoiour
	{
		public class SortingLayerSettings
		{
			public SpriteRenderer spriteRenderer;

			public SquareClothRenderer squareClothRenderer;

			public SpriteSortingSettings sortingSettings = new SpriteSortingSettings();
		}

		private List<SortingLayerSettings> sortingSettings = new List<SortingLayerSettings>();

		[SerializeField]
		private Transform shadowTransform;

		[SerializeField]
		private Transform partOfGoalContainer;

		[SerializeField]
		private bool destroyWhenRemovedFromGame;

		[SerializeField]
		private Transform offsetTransform;

		[SerializeField]
		public Transform scalerTransform;

		[SerializeField]
		private Transform childrenContainer;

		private List<TransformBehaviour> children = new List<TransformBehaviour>();

		[SerializeField]
		private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

		[SerializeField]
		private List<SquareClothRenderer> clothRenderers = new List<SquareClothRenderer>();

		[SerializeField]
		private List<TextMeshPro> textRenderers = new List<TextMeshPro>();

		private Vector3 showMatchActionLocalScale_ = Vector3.one;

		private Vector3 slotLocalScale_ = Vector3.one;

		private Vector3 wobbleLocalScale_ = Vector3.one;

		private Vector3 localScale_ = Vector3.one;

		private Vector3 slotOffsetPosition_ = Vector3.zero;

		private Vector3 localOffsetPosition_;

		private Vector3 localPotentialMatchOffsetPosition_;

		public Vector3 showMatchActionLocalScale
		{
			get
			{
				return showMatchActionLocalScale_;
			}
			set
			{
				showMatchActionLocalScale_ = value;
				ApplyScale();
			}
		}

		public Vector3 slotLocalScale
		{
			get
			{
				return slotLocalScale_;
			}
			set
			{
				slotLocalScale_ = value;
				ApplyScale();
			}
		}

		public Vector3 wobbleLocalScale
		{
			get
			{
				return wobbleLocalScale_;
			}
			set
			{
				wobbleLocalScale_ = value;
				ApplyScale();
			}
		}

		public Vector3 localScale
		{
			get
			{
				return localScale_;
			}
			set
			{
				localScale_ = value;
				ApplyScale();
			}
		}

		public Vector3 totalLocalScale => Vector3.Scale(Vector3.Scale(Vector3.Scale(Vector3.Scale(Vector3.one, localScale_), showMatchActionLocalScale_), slotLocalScale_), wobbleLocalScale_);

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

		public Vector3 slotOffsetPosition
		{
			get
			{
				return slotOffsetPosition_;
			}
			set
			{
				slotOffsetPosition_ = value;
				ApplyPosition();
			}
		}

		public Vector3 localOffsetPosition
		{
			get
			{
				return localOffsetPosition_;
			}
			set
			{
				localOffsetPosition_ = value;
				ApplyPosition();
			}
		}

		public Vector3 localPotentialMatchOffsetPosition
		{
			get
			{
				return localPotentialMatchOffsetPosition_;
			}
			set
			{
				localPotentialMatchOffsetPosition_ = value;
				ApplyPosition();
			}
		}

		public Quaternion localRotationOffset
		{
			set
			{
				base.transform.localRotation = value;
			}
		}

		public void SetPartOfGoalActive(bool active)
		{
			GGUtil.SetActive(partOfGoalContainer, active);
		}

		public void SetShadowActive(bool active)
		{
			GGUtil.SetActive(shadowTransform, active);
		}

		public void AddChild(TransformBehaviour t)
		{
			if (!(t == null))
			{
				t.transform.parent = childrenContainer;
				children.Add(t);
			}
		}

		private void ApplyScale()
		{
			if (!(scalerTransform == null))
			{
				scalerTransform.localScale = totalLocalScale;
			}
		}

		public Vector3 WorldToLocalPosition(Vector3 worldPosition)
		{
			return base.transform.parent.InverseTransformPoint(worldPosition);
		}

		public List<SortingLayerSettings> SaveSortingLayerSettings()
		{
			sortingSettings.Clear();
			for (int i = 0; i < spriteRenderers.Count; i++)
			{
				SpriteRenderer spriteRenderer = spriteRenderers[i];
				if (!(spriteRenderer == null))
				{
					SortingLayerSettings sortingLayerSettings = new SortingLayerSettings();
					sortingLayerSettings.sortingSettings.sortingLayerId = spriteRenderer.sortingLayerID;
					sortingLayerSettings.sortingSettings.sortingOrder = spriteRenderer.sortingOrder;
					sortingLayerSettings.spriteRenderer = spriteRenderer;
					sortingSettings.Add(sortingLayerSettings);
				}
			}
			for (int j = 0; j < clothRenderers.Count; j++)
			{
				SquareClothRenderer squareClothRenderer = clothRenderers[j];
				if (!(squareClothRenderer == null))
				{
					SortingLayerSettings sortingLayerSettings2 = new SortingLayerSettings();
					sortingLayerSettings2.sortingSettings.sortingLayerId = squareClothRenderer.sortingLayerID;
					sortingLayerSettings2.sortingSettings.sortingOrder = squareClothRenderer.sortingLayerOrder;
					sortingLayerSettings2.squareClothRenderer = squareClothRenderer;
					sortingSettings.Add(sortingLayerSettings2);
				}
			}
			return sortingSettings;
		}

		public void ResetSortingLayerSettings()
		{
			for (int i = 0; i < sortingSettings.Count; i++)
			{
				SortingLayerSettings sortingLayerSettings = sortingSettings[i];
				sortingLayerSettings.sortingSettings.Set(sortingLayerSettings.spriteRenderer);
				if (sortingLayerSettings.squareClothRenderer != null)
				{
					sortingLayerSettings.squareClothRenderer.SetSortingLayers(sortingLayerSettings.sortingSettings.sortingLayerId, sortingLayerSettings.sortingSettings.sortingOrder);
				}
			}
		}

		private void ApplyPosition()
		{
			if (!(offsetTransform == null))
			{
				offsetTransform.localPosition = slotOffsetPosition_ + localOffsetPosition_ + localPotentialMatchOffsetPosition_;
			}
		}

		public void SetAlpha(float alpha)
		{
			for (int i = 0; i < spriteRenderers.Count; i++)
			{
				SpriteRenderer spriteRenderer = spriteRenderers[i];
				if (!(spriteRenderer == null))
				{
					Color color = spriteRenderer.color;
					color.a = alpha;
					spriteRenderer.color = color;
				}
			}
			for (int j = 0; j < clothRenderers.Count; j++)
			{
				SquareClothRenderer squareClothRenderer = clothRenderers[j];
				if (!(squareClothRenderer == null))
				{
					squareClothRenderer.SetAlpha(alpha);
				}
			}
			for (int k = 0; k < textRenderers.Count; k++)
			{
				TextMeshPro textMeshPro = textRenderers[k];
				if (!(textMeshPro == null))
				{
					textMeshPro.alpha = alpha;
				}
			}
		}

		public void SetText(string text)
		{
			for (int i = 0; i < textRenderers.Count; i++)
			{
				TextMeshPro textMeshPro = textRenderers[i];
				if (!(textMeshPro == null))
				{
					textMeshPro.text = text;
				}
			}
		}

		public void SetColor(Color color)
		{
			for (int i = 0; i < spriteRenderers.Count; i++)
			{
				SpriteRenderer spriteRenderer = spriteRenderers[i];
				if (!(spriteRenderer == null))
				{
					spriteRenderer.color = color;
				}
			}
			for (int j = 0; j < clothRenderers.Count; j++)
			{
				SquareClothRenderer squareClothRenderer = clothRenderers[j];
				if (!(squareClothRenderer == null))
				{
					squareClothRenderer.SetColor(color);
				}
			}
			for (int k = 0; k < textRenderers.Count; k++)
			{
				TextMeshPro textMeshPro = textRenderers[k];
				if (!(textMeshPro == null))
				{
					textMeshPro.color = color;
				}
			}
		}

		public void SetBrightness(float brightness)
		{
			for (int i = 0; i < spriteRenderers.Count; i++)
			{
				SpriteRenderer spriteRenderer = spriteRenderers[i];
				if (!(spriteRenderer == null))
				{
					spriteRenderer.material.SetFloat("_ColorHSV_Brightness_1", brightness);
				}
			}
			for (int j = 0; j < clothRenderers.Count; j++)
			{
				SquareClothRenderer squareClothRenderer = clothRenderers[j];
				if (!(squareClothRenderer == null))
				{
					squareClothRenderer.SetBrightness(brightness);
				}
			}
		}

		public void SetSortingLayer(SpriteSortingSettings s)
		{
			SetSortingLayer(s.sortingLayerId, s.sortingOrder);
		}

		public void SetSortingLayer(int sortingLayerId, int orderInLayer)
		{
			for (int i = 0; i < spriteRenderers.Count; i++)
			{
				SpriteRenderer spriteRenderer = spriteRenderers[i];
				if (!(spriteRenderer == null))
				{
					spriteRenderer.sortingLayerID = sortingLayerId;
					spriteRenderer.sortingOrder = orderInLayer;
				}
			}
			for (int j = 0; j < clothRenderers.Count; j++)
			{
				SquareClothRenderer squareClothRenderer = clothRenderers[j];
				if (!(squareClothRenderer == null))
				{
					squareClothRenderer.SetSortingLayers(sortingLayerId, orderInLayer);
				}
			}
			for (int k = 0; k < children.Count; k++)
			{
				TransformBehaviour transformBehaviour = children[k];
				if (!(transformBehaviour == null))
				{
					transformBehaviour.SetSortingLayer(sortingLayerId, orderInLayer + k + 1);
				}
			}
		}

		public void ForceRemoveFromGame()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public override void RemoveFromGame()
		{
			if (destroyWhenRemovedFromGame)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
