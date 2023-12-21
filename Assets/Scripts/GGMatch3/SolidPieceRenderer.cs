using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class SolidPieceRenderer : SlotComponentBehavoiour
	{
		[Serializable]
		public class ChipTypeSettings
		{
			public ChipType chipType;

			public GameObject obj;

			public Vector3 rotation;
		}

		[SerializeField]
		private List<ChipTypeSettings> chipTypeSettings = new List<ChipTypeSettings>();

		[SerializeField]
		private List<SpriteRenderer> boxSprites = new List<SpriteRenderer>();

		[SerializeField]
		private Transform rotator;

		[NonSerialized]
		private Chip chip;

		public void Init(Chip chip)
		{
			ChipTypeSettings chipTypeSettings = null;
			for (int i = 0; i < this.chipTypeSettings.Count; i++)
			{
				ChipTypeSettings chipTypeSettings2 = this.chipTypeSettings[i];
				bool flag = chipTypeSettings2.chipType == chip.chipType;
				chipTypeSettings2.obj.SetActive(flag);
				if (flag)
				{
					chipTypeSettings = chipTypeSettings2;
				}
			}
			if (chipTypeSettings != null)
			{
				chipTypeSettings.obj.SetActive(value: true);
				chipTypeSettings.obj.transform.localRotation = Quaternion.Euler(chipTypeSettings.rotation);
			}
			this.chip = chip;
			chip.SetTransformToMove(base.transform);
			UpdateLook();
		}

		public void Init(ChipType chipType)
		{
			ChipTypeSettings chipTypeSettings = null;
			for (int i = 0; i < this.chipTypeSettings.Count; i++)
			{
				ChipTypeSettings chipTypeSettings2 = this.chipTypeSettings[i];
				bool flag = chipTypeSettings2.chipType == chipType;
				chipTypeSettings2.obj.SetActive(flag);
				if (flag)
				{
					chipTypeSettings = chipTypeSettings2;
				}
			}
			if (chipTypeSettings != null)
			{
				chipTypeSettings.obj.SetActive(value: true);
				chipTypeSettings.obj.transform.localRotation = Quaternion.Euler(chipTypeSettings.rotation);
			}
		}

		public void ResetVisually()
		{
			if (chip != null && chip.slot != null)
			{
				base.transform.localPosition = chip.slot.localPositionOfCenter;
			}
		}

		public override void RemoveFromGame()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void UpdateLook()
		{
			Match3Settings.ChipColorSettings colorSettings = Match3Settings.instance.GetColorSettings();
			if (colorSettings != null)
			{
				for (int i = 0; i < boxSprites.Count; i++)
				{
					boxSprites[i].gameObject.SetActive(colorSettings.hasBoxes);
				}
			}
		}

		private void Update()
		{
			bool isEditor = Application.isEditor;
		}
	}
}
