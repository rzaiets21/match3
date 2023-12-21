using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGMatch3
{
	public class LevelEditorSlot : MonoBehaviour
	{
		[Serializable]
		public class PortalDefinition
		{
			public RectTransform container;

			public Color lowColor;

			public Color highColor;

			public Image coloredImage;

			public string suffix;

			public Text label;

			public void SetActive(bool active)
			{
				GGUtil.SetActive(container, active);
			}

			public void Init(int index, int totalCount)
			{
				float t = Mathf.InverseLerp(0f, totalCount, index);
				Color color = Color.Lerp(lowColor, highColor, t);
				coloredImage.color = color;
				string text = $"{index} - {suffix}";
				SetText(label, text);
			}
		}

		[Serializable]
		public class LayerDesc
		{
			public GameObject objectToShow;
		}

		[Serializable]
		public class LayerSet
		{
			public List<LayerDesc> layers = new List<LayerDesc>();

			public void ShowLayer(int index)
			{
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
		public class ChipDescriptor
		{
			public ItemColor color;

			public ChipType chipType;

			public RectTransform container;
		}

		[Serializable]
		public class ChipTypeDescriptor
		{
			public ChipType chipType;

			public RectTransform container;
		}

		[SerializeField]
		private List<RectTransform> widgetsToHide = new List<RectTransform>();

		[SerializeField]
		private Color normalGravityColor = Color.white;

		[SerializeField]
		private Color jumpGravityColor = Color.green;

		[SerializeField]
		private List<Text> magicHatItemsCount = new List<Text>();

		[SerializeField]
		private List<Image> coloredGravityImages = new List<Image>();

		[SerializeField]
		private RectTransform generatorContainer;

		[SerializeField]
		private Text generatorText;

		[SerializeField]
		private Image generatorSetupImage;

		[SerializeField]
		private RectTransform upArrow;

		[SerializeField]
		private RectTransform downArrow;

		[SerializeField]
		private RectTransform leftArrow;

		[SerializeField]
		private RectTransform rightArrow;

		[SerializeField]
		private RectTransform emptyContainer;

		[SerializeField]
		private RectTransform fillContainer;

		[SerializeField]
		private RectTransform wallContainer;

		[SerializeField]
		private RectTransform wallLeft;

		[SerializeField]
		private RectTransform wallRight;

		[SerializeField]
		private RectTransform wallUp;

		[SerializeField]
		private RectTransform wallDown;

		[SerializeField]
		private RectTransform chipContainer;

		[SerializeField]
		private RectTransform randomChipContainer;

		[SerializeField]
		private RectTransform netContainer;

		[SerializeField]
		private List<RectTransform> boxContainer = new List<RectTransform>();

		[SerializeField]
		private RectTransform iceContainer;

		[SerializeField]
		private LayerSet iceLayerSet = new LayerSet();

		[SerializeField]
		private RectTransform conveyorContainer;

		[SerializeField]
		private RectTransform conveyorContainerDown;

		[SerializeField]
		private RectTransform conveyorContainerUp;

		[SerializeField]
		private RectTransform exitForFallingChip;

		[SerializeField]
		private Text moreMovesLabel;

		[SerializeField]
		private RectTransform bubbleContainer;

		[SerializeField]
		private RectTransform snowCoverContainer;

		[SerializeField]
		private List<RectTransform> basketContainer = new List<RectTransform>();

		[SerializeField]
		private LayerSet basketLayerSet = new LayerSet();

		[SerializeField]
		private LayerSet boxLayerSet = new LayerSet();

		[SerializeField]
		private RectTransform chainContainer;

		[SerializeField]
		private LayerSet chainLayerSet = new LayerSet();

		[SerializeField]
		private LayerSet bunnyLeyerSet = new LayerSet();

		[SerializeField]
		private LayerSet rockBlockLayerSet = new LayerSet();

		[SerializeField]
		private LayerSet cookiePickupLayerSet = new LayerSet();

		[SerializeField]
		private RectTransform slotColorSlateContainer;

		[SerializeField]
		private LayerSet slotColorSlateLevelSet = new LayerSet();

		[SerializeField]
		private List<ChipDescriptor> chips = new List<ChipDescriptor>();

		[SerializeField]
		private List<ChipTypeDescriptor> chipsTypes = new List<ChipTypeDescriptor>();

		[SerializeField]
		private RectTransform portalContainer;

		[SerializeField]
		private PortalDefinition enterPortal = new PortalDefinition();

		[SerializeField]
		private PortalDefinition exitPortal = new PortalDefinition();

		[SerializeField]
		private RectTransform carpetContainer;

		public static void SetActive(List<RectTransform> list, bool active)
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					SetActive(list[i], active);
				}
			}
		}

		public static void SetActive(RectTransform t, bool active)
		{
			if (!(t == null))
			{
				GameObject gameObject = t.gameObject;
				if (!(gameObject == null))
				{
					gameObject.SetActive(active);
				}
			}
		}

		private static void SetText(Text label, string text)
		{
			if (!(label == null) && !(label.text == text))
			{
				label.text = text;
			}
		}

		public void Init(LevelDefinition level, LevelDefinition.SlotDefinition slot)
		{
			SetActive(widgetsToHide, active: false);
			SetGeneratorSetup(level, slot);
			SetActive(generatorContainer, slot.generatorSettings.isGeneratorOn);
			SetText(moreMovesLabel, slot.moreMovesCount.ToString());
			string text = slot.generatorSettings.generateOnlyBunnies ? "BUNNIES GEN" : "GEN";
			if (level.GetGeneratorSlotSettings(slot.generatorSettings.slotGeneratorSetupIndex) != null)
			{
				text = text + " - " + slot.generatorSettings.slotGeneratorSetupIndex;
				generatorText.color = Color.cyan;
			}
			else
			{
				generatorText.color = Color.white;
			}
			SetText(generatorText, text);
			for (int i = 0; i < magicHatItemsCount.Count; i++)
			{
				SetText(magicHatItemsCount[i], slot.magicHatItemsCount.ToString());
			}
			SetActive(wallContainer, slot.wallSettings.isWallActive);
			SetActive(wallUp, slot.wallSettings.up);
			SetActive(wallDown, slot.wallSettings.down);
			SetActive(wallLeft, slot.wallSettings.left);
			SetActive(wallRight, slot.wallSettings.right);
			SetActive(downArrow, slot.gravitySettings.down && (slot.gravitySettings.up || slot.gravitySettings.left || slot.gravitySettings.right || slot.gravitySettings.canJumpWithGravity));
			SetActive(upArrow, slot.slotType == SlotType.PlayingSpace && slot.gravitySettings.up);
			SetActive(leftArrow, slot.slotType == SlotType.PlayingSpace && slot.gravitySettings.left);
			SetActive(rightArrow, slot.slotType == SlotType.PlayingSpace && slot.gravitySettings.right);
			SetActive(emptyContainer, slot.slotType == SlotType.Empty);
			SetActive(fillContainer, slot.slotType == SlotType.PlayingSpace);
			SetActive(chipContainer, slot.slotType == SlotType.PlayingSpace);
			SetActive(bubbleContainer, slot.hasBubbles);
			SetActive(snowCoverContainer, slot.hasSnowCover);
			bool flag = slot.chipType == ChipType.Chip || slot.chipType == ChipType.RandomChip;
			for (int j = 0; j < chips.Count; j++)
			{
				ChipDescriptor chipDescriptor = chips[j];
				ChipType chipType = (!flag) ? slot.chipType : ChipType.Chip;
				SetActive(chipDescriptor.container, chipType == chipDescriptor.chipType && chipDescriptor.color == slot.itemColor);
			}
			for (int k = 0; k < chipsTypes.Count; k++)
			{
				ChipTypeDescriptor chipTypeDescriptor = chipsTypes[k];
				SetActive(chipTypeDescriptor.container, chipTypeDescriptor.chipType == slot.chipType);
				if (chipTypeDescriptor.chipType == ChipType.EmptyConveyorSpace && slot.holeBlocker)
				{
					SetActive(chipTypeDescriptor.container, active: true);
				}
			}
			Color color = slot.gravitySettings.canJumpWithGravity ? jumpGravityColor : normalGravityColor;
			for (int l = 0; l < coloredGravityImages.Count; l++)
			{
				coloredGravityImages[l].color = color;
			}
			if (slot.gravitySettings.noGravity)
			{
				SetActive(downArrow, active: true);
				SetActive(upArrow, active: true);
				SetActive(leftArrow, active: true);
				SetActive(rightArrow, active: true);
				for (int m = 0; m < coloredGravityImages.Count; m++)
				{
					coloredGravityImages[m].color = Color.red;
				}
			}
			SetActive(randomChipContainer, slot.chipType == ChipType.RandomChip);
			SetActive(netContainer, slot.hasNet);
			GGUtil.SetActive(iceContainer, slot.hasIce);
			iceLayerSet.ShowLayer(slot.iceLevel - 1);
			GGUtil.SetActive(boxContainer, slot.hasBox);
			boxLayerSet.ShowLayer(slot.boxLevel - 1);
			GGUtil.SetActive(basketContainer, slot.hasBasket);
			basketLayerSet.ShowLayer(slot.basketLevel - 1);
			GGUtil.SetActive(chainContainer, slot.hasChain);
			chainLayerSet.ShowLayer(slot.chainLevel - 1);
			if (slot.chipType == ChipType.BunnyChip)
			{
				bunnyLeyerSet.ShowLayer(slot.itemLevel);
			}
			if (slot.chipType == ChipType.RockBlocker || slot.chipType == ChipType.SnowRockBlocker)
			{
				rockBlockLayerSet.ShowLayer(slot.itemLevel);
			}
			if (slot.chipType == ChipType.CookiePickup)
			{
				cookiePickupLayerSet.ShowLayer(slot.itemLevel);
			}
			SetActive(exitForFallingChip, slot.isExitForFallingChip);
			SetActive(conveyorContainer, slot.isPartOfConveyor);
			SetConveyor(slot);
			SetPortals(level, slot);
			GGUtil.SetActive(slotColorSlateContainer, slot.hasColorSlate);
			if (slot.hasColorSlate)
			{
				slotColorSlateLevelSet.ShowLayer(slot.colorSlateLevel - 1);
			}
			SetActive(carpetContainer, slot.hasCarpet);
		}

		private void SetGeneratorSetup(LevelDefinition level, LevelDefinition.SlotDefinition slot)
		{
			if (generatorSetupImage == null)
			{
				return;
			}
			GeneratorSetup generatorSetup = null;
			List<GeneratorSetup> generatorSetups = level.generatorSetups;
			for (int i = 0; i < generatorSetups.Count; i++)
			{
				GeneratorSetup generatorSetup2 = generatorSetups[i];
				if (generatorSetup2.position == slot.position)
				{
					generatorSetup = generatorSetup2;
				}
			}
			if (generatorSetup == null)
			{
				generatorSetupImage.color = Color.black;
				return;
			}
			Color color = GGUtil.colorProvider.GetColor(generatorSetup.position.y);
			generatorSetupImage.color = color;
		}

		private void SetConveyor(LevelDefinition.SlotDefinition slot)
		{
			if (slot.isPartOfConveyor)
			{
				conveyorContainer.localScale = new Vector3(1f, 1f, 1f);
				bool flag = slot.conveyorDirection == IntVector2.down || slot.conveyorDirection == IntVector2.right;
				if (!slot.isConveyorDirectionSet)
				{
					SetActive(conveyorContainerUp, active: false);
					SetActive(conveyorContainerDown, active: false);
				}
				else
				{
					SetActive(conveyorContainerDown, flag);
					SetActive(conveyorContainerUp, !flag);
				}
				Quaternion localRotation = Quaternion.identity;
				if (slot.conveyorDirection.x != 0)
				{
					localRotation = Quaternion.AngleAxis(90f, Vector3.forward);
				}
				conveyorContainer.localRotation = localRotation;
			}
		}

		private void SetPortals(LevelDefinition level, LevelDefinition.SlotDefinition slot)
		{
			if (!(portalContainer == null))
			{
				enterPortal.SetActive(slot.isPortalEntrance);
				exitPortal.SetActive(slot.isPortalExit);
				int portalIndexCount = level.portalIndexCount;
				enterPortal.Init(slot.portalEntranceIndex, portalIndexCount);
				exitPortal.Init(slot.portalExitIndex, portalIndexCount);
				Quaternion localRotation = Quaternion.identity;
				if (slot.gravitySettings.left)
				{
					localRotation = Quaternion.AngleAxis(270f, Vector3.forward);
				}
				if (slot.gravitySettings.right)
				{
					localRotation = Quaternion.AngleAxis(90f, Vector3.forward);
				}
				portalContainer.localRotation = localRotation;
			}
		}
	}
}
