using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class MultiLayerItemBehaviour : SlotComponentBehavoiour
	{
		[Serializable]
		public class LayerSet
		{
			public ChipType chipType;

			public GameObject container;

			public List<LayerDesc> layers = new List<LayerDesc>();

			public void HideAllLayers()
			{
				for (int i = 0; i < layers.Count; i++)
				{
					GGUtil.SetActive(layers[i].objectToShow, active: false);
				}
			}

			public void ShowLayer(int index)
			{
				GGUtil.SetActive(container, active: true);
				int num = Mathf.Clamp(index, 0, layers.Count - 1);
				for (int i = 0; i < layers.Count; i++)
				{
					LayerDesc layerDesc = layers[i];
					bool active = i == num;
					GGUtil.SetActive(layerDesc.objectToShow, active);
				}
			}
		}

		[Serializable]
		public class LayerDesc
		{
			public GameObject objectToShow;
		}

		[SerializeField]
		private List<LayerSet> layerSetList = new List<LayerSet>();

		[SerializeField]
		private Transform emptyChipTransform;

		[SerializeField]
		private Transform slotPatterTransform;

		private ChipType chipType;

		public void SetHasEmptyChip(bool hasEmptyChip)
		{
			GGUtil.SetActive(emptyChipTransform, hasEmptyChip);
		}

		private LayerSet GetLayerSet(ChipType chipType)
		{
			for (int i = 0; i < layerSetList.Count; i++)
			{
				LayerSet layerSet = layerSetList[i];
				if (layerSet.chipType == chipType)
				{
					return layerSet;
				}
			}
			return null;
		}

		public void HideAllLayers()
		{
			for (int i = 0; i < layerSetList.Count; i++)
			{
				layerSetList[i].HideAllLayers();
			}
		}

		public void SetLayerIndex(int layerIndex)
		{
			GetLayerSet(chipType)?.ShowLayer(layerIndex);
		}

		public void SetPattern(Slot slot)
		{
			GGUtil.SetActive(slotPatterTransform, slot.isBackgroundPatternActive);
		}

		public bool HasChipType(ChipType chipType)
		{
			return GetLayerSet(chipType) != null;
		}

		public void Init(ChipType chipType, int layerIndex)
		{
			for (int i = 0; i < layerSetList.Count; i++)
			{
				LayerSet layerSet = layerSetList[i];
				GGUtil.SetActive(layerSet.container, active: false);
				layerSet.HideAllLayers();
			}
			this.chipType = chipType;
			SetLayerIndex(layerIndex);
			GGUtil.SetActive(this, active: true);
		}

		public override void RemoveFromGame()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
