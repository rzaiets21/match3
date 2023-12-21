using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class GrowingElementChip : SlotComponent
	{
		public struct PotentialElementToReceieve
		{
			public Slot slot;

			public Chip chip;
		}

		private ItemColor itemColor = ItemColor.Uncolored;
		public ItemColor ItemColor => itemColor;

		private WobbleAnimation wobbleAnimation = new WobbleAnimation();

		private int currentLevel;

		private List<PotentialElementToReceieve> potentialElements = new List<PotentialElementToReceieve>();

		private List<PotentialElementToReceieve> selectedElements = new List<PotentialElementToReceieve>();

		public bool isGeneratingSpecificColor
		{
			get
			{
				if (itemColor != ItemColor.Uncolored)
				{
					return itemColor != ItemColor.Unknown;
				}
				return false;
			}
		}

		private GrowingElementBehaviour elementBehaviour => GetComponentBehaviour<GrowingElementBehaviour>();

		public override bool isPlaceBubbleSuspended => true;

		public override bool isSlotMatchingSuspended => true;

		public override bool isPreventingOtherChipsToFallIntoSlot => true;

		public override bool isAttachGrowingElementSuspended => true;

		public override bool isMoveIntoSlotSuspended => true;

		public override bool isSlotGravitySuspended => true;

		public override bool isSlotSwapSuspended => true;

		public override bool isPreventingGravity => true;

		public override bool isCreatePowerupWithThisSlotSuspended => true;

		public override bool isMovingWithConveyor => true;

		public override bool isDestroyedByMatchingNextTo => true;

		public void Init(ItemColor itemColor)
		{
			this.itemColor = itemColor;
		}

		public override bool IsAvailableForDiscoBombSuspended(bool replaceWithBombs)
		{
			return true;
		}

		public override bool IsCompatibleWithPickupGoal(Match3Goals.ChipTypeDef chipTypeDef)
		{
			Match3Goals.ChipTypeDef b = default(Match3Goals.ChipTypeDef);
			b.chipType = ChipType.GrowingElementPiece;
			b.itemColor = ItemColor.Unknown;
			return chipTypeDef.IsEqual(b);
		}

		public override SlotDestroyResolution OnDestroyNeighbourSlotComponent(Slot slotBeingDestroyed, SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			if (destroyParams.isHitByBomb && !destroyParams.isBombAllowingNeighbourDestroy)
			{
				return result;
			}
			result.isDestroyed = true;
			result.stopPropagation = true;
			UpdateLevel(destroyParams);
			return result;
		}

		public override SlotDestroyResolution OnDestroySlotComponent(SlotDestroyParams destroyParams)
		{
			SlotDestroyResolution result = default(SlotDestroyResolution);
			result.isDestroyed = true;
			if (destroyParams.isFromTap)
			{
				result.stopPropagation = true;
				return result;
			}
			result.stopPropagation = true;
			result.isNeigbourDestroySuspendedForThisChipOnly = true;
			UpdateLevel(destroyParams);
			return result;
		}

		private void UpdateLevel(SlotDestroyParams destroyParams)
		{
			currentLevel++;
			if (currentLevel >= 3)
			{
				lastConnectedSlot.game.Play(GGSoundSystem.SFXType.GrowingElementFinish);
				CreateFlowers();
				currentLevel = 0;
			}
			else
			{
				GGSoundSystem.PlayParameters sound = default(GGSoundSystem.PlayParameters);
				sound.soundType = GGSoundSystem.SFXType.GrowingElementGrowFlower;
				sound.variationIndex = currentLevel - 1;
				lastConnectedSlot.game.Play(sound);
			}
			CollectPointsAction.OnBlockerDestroy(lastConnectedSlot, destroyParams);
			VisualUpdate();
			if (slot != null)
			{
				slot.Wobble(Match3Settings.instance.chipWobbleSettings);
			}
		}

		private List<PotentialElementToReceieve> PotentialElementToAttach()
		{
			potentialElements.Clear();
			List<Slot> sortedSlotsUpdateList = base.slot.game.board.sortedSlotsUpdateList;
			for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
			{
				Slot slot = sortedSlotsUpdateList[i];
				if (slot.CanAttachGrowingElement())
				{
					Chip slotComponent = slot.GetSlotComponent<Chip>();
					if (slotComponent != null)
					{
						PotentialElementToReceieve item = default(PotentialElementToReceieve);
						item.slot = slot;
						item.chip = slotComponent;
						potentialElements.Add(item);
					}
				}
			}
			return potentialElements;
		}

		private List<PotentialElementToReceieve> PotentialElementToAttachEmptySlots()
		{
			potentialElements.Clear();
			List<Slot> sortedSlotsUpdateList = base.slot.game.board.sortedSlotsUpdateList;
			for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
			{
				Slot slot = sortedSlotsUpdateList[i];
				if (slot.CanAttachGrowingElement())
				{
					Chip slotComponent = slot.GetSlotComponent<Chip>();
					if (slotComponent == null)
					{
						PotentialElementToReceieve item = default(PotentialElementToReceieve);
						item.slot = slot;
						item.chip = slotComponent;
						potentialElements.Add(item);
					}
				}
			}
			return potentialElements;
		}

		public Slot RandomDesiredSlot()
		{
			List<PotentialElementToReceieve> list = PotentialElementToAttach();
			GGUtil.Shuffle(list);
			if (list.Count > 0)
			{
				return list[0].slot;
			}
			list = PotentialElementToAttachEmptySlots();
			GGUtil.Shuffle(list);
			if (list.Count > 0)
			{
				return list[0].slot;
			}
			return null;
		}

		private void CreateFlowers()
		{
			Match3Game game = slot.game;
			GrowingElementBehaviour elementBehaviour = this.elementBehaviour;
			selectedElements.Clear();
			int num = 3;
			List<PotentialElementToReceieve> list = PotentialElementToAttach();
			GGUtil.Shuffle(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (selectedElements.Count >= num)
				{
					break;
				}
				PotentialElementToReceieve item = list[i];
				selectedElements.Add(item);
			}
			list = PotentialElementToAttachEmptySlots();
			GGUtil.Shuffle(list);
			for (int j = 0; j < list.Count; j++)
			{
				if (selectedElements.Count >= num)
				{
					break;
				}
				PotentialElementToReceieve item2 = list[j];
				selectedElements.Add(item2);
			}
			for (int k = 0; k < selectedElements.Count; k++)
			{
				PotentialElementToReceieve potentialElementToReceieve = selectedElements[k];
				AnimateGrowingElementOnChip animateGrowingElementOnChip = new AnimateGrowingElementOnChip();
				AnimateGrowingElementOnChip.InitArguments initArguments = default(AnimateGrowingElementOnChip.InitArguments);
				initArguments.destinationSlot = potentialElementToReceieve.slot;
				initArguments.game = game;
				initArguments.itemColor = game.board.RandomColor();
				initArguments.growingElement = this;
				if (potentialElementToReceieve.chip != null)
				{
					initArguments.itemColor = potentialElementToReceieve.chip.itemColor;
				}
				if (isGeneratingSpecificColor)
				{
					initArguments.itemColor = itemColor;
					initArguments.forceConvertChip = true;
				}
				initArguments.destinationSlot = null;
				initArguments.worldPositionStart = Vector3.zero;
				if (elementBehaviour != null)
				{
					initArguments.worldPositionStart = elementBehaviour.WorldPositionForElement(k);
				}
				animateGrowingElementOnChip.Init(initArguments);
				game.board.actionManager.AddAction(animateGrowingElementOnChip);
			}
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			wobbleAnimation.Update(deltaTime);
		}

		public override void Wobble(WobbleAnimation.Settings settings)
		{
			if (settings != null)
			{
				wobbleAnimation.Init(settings, GetComponentBehaviour<TransformBehaviour>());
			}
		}

		private void VisualUpdate()
		{
			GrowingElementBehaviour elementBehaviour = this.elementBehaviour;
			if (!(elementBehaviour == null))
			{
				elementBehaviour.SetLevel(currentLevel);
				if (currentLevel <= 0)
				{
					elementBehaviour.StopAllAnimations();
				}
				else
				{
					elementBehaviour.StartAnimationFor(currentLevel - 1);
				}
			}
		}
	}
}
