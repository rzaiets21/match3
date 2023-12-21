using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace GGMatch3
{
	[Serializable]
	public class LevelDefinition
	{
		[Serializable]
		public class BurriedElement
		{
			public enum Orientation
			{
				Horizontal,
				Vertical
			}

			public IntVector2 position;

			public IntVector2 size;

			public Orientation orientation;

			public IntVector2 oppositeCornerPosition
			{
				get
				{
					IntVector2 b = new IntVector2(size.x, -size.y);
					if (orientation == Orientation.Horizontal)
					{
						b = new IntVector2(size.y, -size.x);
					}
					return position + b + new IntVector2(-1, 1);
				}
			}

			public bool ContainsPosition(IntVector2 positionToCheck)
			{
				IntVector2 oppositeCornerPosition = this.oppositeCornerPosition;
				FloatRange floatRange = new FloatRange(Mathf.Min(position.x, oppositeCornerPosition.x), Mathf.Max(position.x, oppositeCornerPosition.x));
				FloatRange floatRange2 = new FloatRange(Mathf.Min(position.y, oppositeCornerPosition.y), Mathf.Max(position.y, oppositeCornerPosition.y));
				if ((float)positionToCheck.x >= floatRange.min && (float)positionToCheck.x <= floatRange.max && (float)positionToCheck.y >= floatRange2.min)
				{
					return (float)positionToCheck.y <= floatRange2.max;
				}
				return false;
			}

			public BurriedElement Clone()
			{
				return new BurriedElement
				{
					position = position,
					size = size,
					orientation = orientation
				};
			}
		}

		[Serializable]
		public class MonsterElement
		{
			public IntVector2 position;

			public IntVector2 size;

			public ItemColor itemColor;

			public int numberToCollect;

			public IntVector2 oppositeCornerPosition
			{
				get
				{
					IntVector2 b = new IntVector2(size.x, -size.y);
					return position + b + new IntVector2(-1, 1);
				}
			}

			public bool ContainsPosition(IntVector2 positionToCheck)
			{
				IntVector2 oppositeCornerPosition = this.oppositeCornerPosition;
				FloatRange floatRange = new FloatRange(Mathf.Min(position.x, oppositeCornerPosition.x), Mathf.Max(position.x, oppositeCornerPosition.x));
				FloatRange floatRange2 = new FloatRange(Mathf.Min(position.y, oppositeCornerPosition.y), Mathf.Max(position.y, oppositeCornerPosition.y));
				if ((float)positionToCheck.x >= floatRange.min && (float)positionToCheck.x <= floatRange.max && (float)positionToCheck.y >= floatRange2.min)
				{
					return (float)positionToCheck.y <= floatRange2.max;
				}
				return false;
			}

			public MonsterElement Clone()
			{
				return new MonsterElement
				{
					position = position,
					size = size,
					itemColor = itemColor,
					numberToCollect = numberToCollect
				};
			}
		}

		[Serializable]
		public class BurriedElements
		{
			public List<BurriedElement> elements = new List<BurriedElement>();

			public int CountAllElements()
			{
				int num = 0;
				for (int i = 0; i < elements.Count; i++)
				{
					BurriedElement burriedElement = elements[i];
					num += burriedElement.size.x * burriedElement.size.y;
				}
				return num;
			}

			public void MoveByOffset(IntVector2 offset)
			{
				for (int i = 0; i < elements.Count; i++)
				{
					elements[i].position += offset;
				}
			}

			public bool HasElementsUnderPosition(IntVector2 position)
			{
				for (int i = 0; i < elements.Count; i++)
				{
					if (elements[i].ContainsPosition(position))
					{
						return true;
					}
				}
				return false;
			}

			public BurriedElements Clone()
			{
				BurriedElements burriedElements = new BurriedElements();
				for (int i = 0; i < elements.Count; i++)
				{
					BurriedElement burriedElement = elements[i];
					burriedElements.elements.Add(burriedElement.Clone());
				}
				return burriedElements;
			}
		}

		[Serializable]
		public class MonsterElements
		{
			public List<MonsterElement> elements = new List<MonsterElement>();

			public void MoveByOffset(IntVector2 offset)
			{
				for (int i = 0; i < elements.Count; i++)
				{
					elements[i].position += offset;
				}
			}

			public bool HasElementsUnderPosition(IntVector2 position)
			{
				for (int i = 0; i < elements.Count; i++)
				{
					if (elements[i].ContainsPosition(position))
					{
						return true;
					}
				}
				return false;
			}

			public MonsterElements Clone()
			{
				MonsterElements monsterElements = new MonsterElements();
				for (int i = 0; i < elements.Count; i++)
				{
					MonsterElement monsterElement = elements[i];
					monsterElements.elements.Add(monsterElement.Clone());
				}
				return monsterElements;
			}
		}

		[Serializable]
		public class Size
		{
			public int width = 12;

			public int height = 12;

			public Size Clone()
			{
				return new Size
				{
					width = width,
					height = height
				};
			}
		}

		[Serializable]
		public class GeneratorSettings
		{
			public bool isGeneratorOn;

			public bool generateOnlyBunnies;

			public int maxFallingElementsToGenerate;

			public int chipTag;

			public int slotGeneratorSetupIndex = -1;

			public GeneratorSettings Clone()
			{
				return new GeneratorSettings
				{
					isGeneratorOn = isGeneratorOn,
					generateOnlyBunnies = generateOnlyBunnies,
					maxFallingElementsToGenerate = maxFallingElementsToGenerate,
					chipTag = chipTag,
					slotGeneratorSetupIndex = slotGeneratorSetupIndex
				};
			}
		}

		[Serializable]
		public class WallSettings
		{
			public bool up;

			public bool down;

			public bool left;

			public bool right;

			public bool isWallActive => !noWall;

			public bool noWall
			{
				get
				{
					if (!up && !down && !left)
					{
						return !right;
					}
					return false;
				}
			}

			public WallSettings Clone()
			{
				return new WallSettings
				{
					up = up,
					down = down,
					left = left,
					right = right
				};
			}
		}

		[Serializable]
		public class GravitySettings
		{
			public bool up;

			public bool down = true;

			public bool left;

			public bool right;

			public bool canJumpWithGravity;

			public bool noGravity
			{
				get
				{
					if (!up && !down && !left)
					{
						return !right;
					}
					return false;
				}
			}

			public GravitySettings Clone()
			{
				return new GravitySettings
				{
					up = up,
					down = down,
					left = left,
					right = right,
					canJumpWithGravity = canJumpWithGravity
				};
			}
		}

		[Serializable]
		public class SlotDefinition
		{
			public IntVector2 position;

			public GeneratorSettings generatorSettings = new GeneratorSettings();

			public GravitySettings gravitySettings = new GravitySettings();

			public WallSettings wallSettings = new WallSettings();

			public SlotType slotType;

			public ChipType chipType = ChipType.RandomChip;

			public int chipTag;

			public ItemColor itemColor = ItemColor.Unknown;

			public int netLevel;

			public int magicHatItemsCount;

			public int boxLevel;

			public bool hasBubbles;

			public bool holeBlocker;

			public int snowCoverLevel;

			public int basketLevel;

			public int iceLevel;

			public bool hasCarpet;

			public int chainLevel;

			public int itemLevel;

			public bool isPartOfConveyor;

			public int portalEntranceIndex = -1;

			public int portalExitIndex = -1;

			public IntVector2 conveyorDirection;

			public bool isExitForFallingChip;

			public int colorSlateLevel;

			[JsonIgnore] public Color colorSlateColor;

			public string colorSlateSpriteName;

			public bool hasHoleInSlot
			{
				get
				{
					if (chipType != ChipType.EmptyConveyorSpace)
					{
						return holeBlocker;
					}
					return true;
				}
			}

			public bool hasSnowCover => snowCoverLevel > 0;

			public int moreMovesCount
			{
				get
				{
					if (chipType != ChipType.MoreMovesChip)
					{
						return 0;
					}
					return itemLevel + 1;
				}
			}

			public bool needsToBeGenerated
			{
				get
				{
					if (chipType != ChipType.RandomChip)
					{
						if (chipType == ChipType.MonsterChip)
						{
							return itemColor == ItemColor.RandomColor;
						}
						return false;
					}
					return true;
				}
			}

			public bool isPortalEntrance => portalEntranceIndex >= 0;

			public bool isPortalExit => portalExitIndex >= 0;

			public bool isConveyorDirectionSet
			{
				get
				{
					if (conveyorDirection != IntVector2.zero)
					{
						return isPartOfConveyor;
					}
					return false;
				}
			}

			public bool hasChain => chainLevel > 0;

			public bool hasBox => boxLevel > 0;

			public bool hasBasket => basketLevel > 0;

			public bool hasIce
			{
				get
				{
					if (iceLevel > 0)
					{
						return chipType != ChipType.EmptyChipSlot;
					}
					return false;
				}
			}

			public bool hasNet => netLevel > 0;

			public bool hasColorSlate => colorSlateLevel > 0;

			public bool isFormingMatchesSuspended(LevelDefinition level)
			{
				if (IsMonsterInSlot(level))
				{
					return true;
				}
				bool result = chipType != 0 && chipType != ChipType.RandomChip && chipType != ChipType.MonsterChip;
				if (hasBox || hasBubbles || hasSnowCover)
				{
					result = true;
				}
				return result;
			}

			public bool isMoveSuspended(LevelDefinition level)
			{
				if (IsMonsterInSlot(level))
				{
					return true;
				}
				if (!hasBox && !hasChain && !hasNet && !hasIce && !hasBubbles)
				{
					return hasSnowCover;
				}
				return true;
			}

			public bool IsMonsterInSlot(LevelDefinition level)
			{
				return level.monsterElements.HasElementsUnderPosition(position);
			}

			public SlotDefinition Clone()
			{
				return new SlotDefinition
				{
					position = position,
					generatorSettings = generatorSettings.Clone(),
					gravitySettings = gravitySettings.Clone(),
					wallSettings = wallSettings.Clone(),
					slotType = slotType,
					chipType = chipType,
					magicHatItemsCount = magicHatItemsCount,
					itemColor = itemColor,
					netLevel = netLevel,
					boxLevel = boxLevel,
					hasBubbles = hasBubbles,
					snowCoverLevel = snowCoverLevel,
					basketLevel = basketLevel,
					iceLevel = iceLevel,
					chainLevel = chainLevel,
					itemLevel = itemLevel,
					isPartOfConveyor = isPartOfConveyor,
					conveyorDirection = conveyorDirection,
					portalEntranceIndex = portalEntranceIndex,
					portalExitIndex = portalExitIndex,
					isExitForFallingChip = isExitForFallingChip,
					colorSlateLevel = colorSlateLevel,
					colorSlateColor = colorSlateColor,
					colorSlateSpriteName = colorSlateSpriteName,
					hasCarpet = hasCarpet,
					chipTag = chipTag,
					holeBlocker = holeBlocker
				};
			}
		}

		[Serializable]
		public class ChipGenerationSettings
		{
			[Serializable]
			public class ChipSetting
			{
				public ChipType chipType;

				public ItemColor itemColor;

				public float weight;

				public ChipSetting Clone()
				{
					return new ChipSetting
					{
						chipType = chipType,
						itemColor = itemColor,
						weight = weight
					};
				}
			}

			public List<ChipSetting> chipSettings = new List<ChipSetting>();

			public bool isConfigured => chipSettings.Count > 0;

			public ChipGenerationSettings Clone()
			{
				ChipGenerationSettings chipGenerationSettings = new ChipGenerationSettings();
				for (int i = 0; i < chipSettings.Count; i++)
				{
					ChipSetting chipSetting = chipSettings[i];
					chipGenerationSettings.chipSettings.Add(chipSetting.Clone());
				}
				return chipGenerationSettings;
			}
		}

		[Serializable]
		public class TutorialMatch
		{
			public bool isEnabled;

			public List<IntVector2> matchingSlots = new List<IntVector2>();

			public IntVector2 slotToSwipe;

			public IntVector2 exchangeSlot;

			public void OffsetAllSlots(IntVector2 offset)
			{
				exchangeSlot += offset;
				slotToSwipe += offset;
				for (int i = 0; i < matchingSlots.Count; i++)
				{
					List<IntVector2> list = matchingSlots;
					int index = i;
					list[index] += offset;
				}
			}

			public bool Contains(IntVector2 slot)
			{
				if (!matchingSlots.Contains(slot) && !(exchangeSlot == slot))
				{
					return slotToSwipe == slot;
				}
				return true;
			}

			public TutorialMatch Clone()
			{
				TutorialMatch tutorialMatch = new TutorialMatch();
				tutorialMatch.isEnabled = isEnabled;
				tutorialMatch.matchingSlots.AddRange(matchingSlots);
				tutorialMatch.slotToSwipe = slotToSwipe;
				tutorialMatch.exchangeSlot = exchangeSlot;
				return tutorialMatch;
			}
		}

		public class Portal
		{
			public SlotDefinition entranceSlot;

			public SlotDefinition exitSlot;

			public bool isValid
			{
				get
				{
					if (entranceSlot != null)
					{
						return exitSlot != null;
					}
					return false;
				}
			}

			public bool HasEntranceIndex(int index)
			{
				if (entranceSlot != null && entranceSlot.portalEntranceIndex == index)
				{
					return true;
				}
				return false;
			}

			public bool HasExitIndex(int index)
			{
				if (exitSlot != null && exitSlot.portalExitIndex == index)
				{
					return true;
				}
				return false;
			}
		}

		public class ConveyorBeltLinearSegment
		{
			public List<SlotDefinition> slotList = new List<SlotDefinition>();

			private List<IntVector2> possibleEntryPositions_ = new List<IntVector2>();

			public IntVector2 direction
			{
				get
				{
					if (enterSlot == null)
					{
						return IntVector2.zero;
					}
					return enterSlot.conveyorDirection;
				}
			}

			public SlotDefinition enterSlot
			{
				get
				{
					if (slotList.Count == 0)
					{
						return null;
					}
					return slotList[0];
				}
			}

			public SlotDefinition exitSlot
			{
				get
				{
					if (slotList.Count == 0)
					{
						return null;
					}
					return slotList[slotList.Count - 1];
				}
			}

			public IntVector2 exitPosition
			{
				get
				{
					SlotDefinition exitSlot = this.exitSlot;
					if (exitSlot == null)
					{
						return IntVector2.zero;
					}
					return exitSlot.position + exitSlot.conveyorDirection;
				}
			}

			public void FillLinearSegmentFromSlot(SlotDefinition slot, LevelDefinition level)
			{
				SlotDefinition slotDefinition = slot;
				while (true)
				{
					SlotDefinition slot2 = level.GetSlot(slotDefinition.position - slotDefinition.conveyorDirection);
					if (slot2 == null || !slot2.isConveyorDirectionSet || slot2.conveyorDirection != slotDefinition.conveyorDirection)
					{
						break;
					}
					slotDefinition = slot2;
				}
				SlotDefinition slotDefinition2 = slotDefinition;
				while (true)
				{
					slotList.Add(slotDefinition2);
					SlotDefinition slot3 = level.GetSlot(slotDefinition2.position + slotDefinition2.conveyorDirection);
					if (slot3 != null && slot3.isConveyorDirectionSet && !(slot3.conveyorDirection != slot.conveyorDirection))
					{
						slotDefinition2 = slot3;
						continue;
					}
					break;
				}
			}

			public bool IsContaining(IntVector2 position)
			{
				for (int i = 0; i < slotList.Count; i++)
				{
					if (slotList[i].position == position)
					{
						return true;
					}
				}
				return false;
			}

			public bool IsContaining(SlotDefinition slot)
			{
				for (int i = 0; i < slotList.Count; i++)
				{
					if (slotList[i] == slot)
					{
						return true;
					}
				}
				return false;
			}
		}

		public class ConveyorBelts
		{
			public List<ConveyorBelt> conveyorBeltList = new List<ConveyorBelt>();

			private List<ConveyorBeltLinearSegment> linearSegmentsList = new List<ConveyorBeltLinearSegment>();

			private ConveyorBeltLinearSegment SegmentPriorTo(ConveyorBeltLinearSegment nextSegment)
			{
				for (int i = 0; i < linearSegmentsList.Count; i++)
				{
					ConveyorBeltLinearSegment conveyorBeltLinearSegment = linearSegmentsList[i];
					if (nextSegment.IsContaining(conveyorBeltLinearSegment.exitPosition))
					{
						return conveyorBeltLinearSegment;
					}
				}
				return null;
			}

			private ConveyorBeltLinearSegment SegmentAfter(ConveyorBeltLinearSegment prevSegment)
			{
				for (int i = 0; i < linearSegmentsList.Count; i++)
				{
					ConveyorBeltLinearSegment conveyorBeltLinearSegment = linearSegmentsList[i];
					if (conveyorBeltLinearSegment.IsContaining(prevSegment.exitPosition))
					{
						return conveyorBeltLinearSegment;
					}
				}
				return null;
			}

			public bool IsPartOfConveyor(IntVector2 position)
			{
				for (int i = 0; i < conveyorBeltList.Count; i++)
				{
					if (conveyorBeltList[i].IsContaining(position))
					{
						return true;
					}
				}
				return false;
			}

			public bool IsPartOfConveyor(ConveyorBeltLinearSegment segment)
			{
				for (int i = 0; i < conveyorBeltList.Count; i++)
				{
					if (conveyorBeltList[i].IsContaining(segment))
					{
						return true;
					}
				}
				return false;
			}

			public bool IsPartOfLinearSegmentList(SlotDefinition slot)
			{
				for (int i = 0; i < linearSegmentsList.Count; i++)
				{
					if (linearSegmentsList[i].IsContaining(slot))
					{
						return true;
					}
				}
				return false;
			}

			public void Init(LevelDefinition level)
			{
				new List<ConveyorBelt>();
				List<SlotDefinition> slots = level.slots;
				linearSegmentsList.Clear();
				for (int i = 0; i < slots.Count; i++)
				{
					SlotDefinition slotDefinition = slots[i];
					if (slotDefinition != null && slotDefinition.isPartOfConveyor && slotDefinition.isConveyorDirectionSet && !IsPartOfLinearSegmentList(slotDefinition))
					{
						ConveyorBeltLinearSegment conveyorBeltLinearSegment = new ConveyorBeltLinearSegment();
						conveyorBeltLinearSegment.FillLinearSegmentFromSlot(slotDefinition, level);
						linearSegmentsList.Add(conveyorBeltLinearSegment);
					}
				}
				CreateConveyorBeltsFromSegments(level);
			}

			private void CreateConveyorBeltsFromSegments(LevelDefinition level)
			{
				for (int i = 0; i < linearSegmentsList.Count; i++)
				{
					ConveyorBeltLinearSegment conveyorBeltLinearSegment = linearSegmentsList[i];
					if (IsPartOfConveyor(conveyorBeltLinearSegment))
					{
						continue;
					}
					ConveyorBelt conveyorBelt = new ConveyorBelt();
					bool flag = false;
					ConveyorBeltLinearSegment conveyorBeltLinearSegment2 = conveyorBeltLinearSegment;
					int num = 0;
					while (true)
					{
						ConveyorBeltLinearSegment conveyorBeltLinearSegment3 = SegmentPriorTo(conveyorBeltLinearSegment2);
						if (conveyorBeltLinearSegment3 == null)
						{
							break;
						}
						conveyorBeltLinearSegment2 = conveyorBeltLinearSegment3;
						if (conveyorBeltLinearSegment2 == conveyorBeltLinearSegment)
						{
							break;
						}
						num++;
						if (num > linearSegmentsList.Count)
						{
							UnityEngine.Debug.LogError("CONVEYOR LIST HAS MULTIPLE ENTRIES");
							flag = true;
							break;
						}
					}
					num = 0;
					ConveyorBeltLinearSegment conveyorBeltLinearSegment4 = conveyorBeltLinearSegment2;
					while (true)
					{
						conveyorBelt.segmentList.Add(conveyorBeltLinearSegment4);
						ConveyorBeltLinearSegment conveyorBeltLinearSegment5 = SegmentAfter(conveyorBeltLinearSegment4);
						if (conveyorBeltLinearSegment5 == null)
						{
							break;
						}
						if (conveyorBeltLinearSegment5 == conveyorBeltLinearSegment2)
						{
							conveyorBelt.isLoop = true;
							break;
						}
						conveyorBeltLinearSegment4 = conveyorBeltLinearSegment5;
						num++;
						if (num > linearSegmentsList.Count)
						{
							conveyorBelt.segmentList.Clear();
							UnityEngine.Debug.LogError("CONVEYOR LIST HAS MULTIPLE ENTRIES");
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						conveyorBeltList.Add(conveyorBelt);
					}
				}
			}
		}

		public class ConveyorBelt
		{
			public List<ConveyorBeltLinearSegment> segmentList = new List<ConveyorBeltLinearSegment>();

			public bool isLoop;

			public ConveyorBeltLinearSegment firstSegment => segmentList[0];

			public ConveyorBeltLinearSegment lastSegment => segmentList[segmentList.Count - 1];

			public IntVector2 firstPosition => firstSegment.enterSlot.position;

			public IntVector2 lastPosition => lastSegment.exitSlot.position;

			public bool IsContaining(IntVector2 position)
			{
				for (int i = 0; i < segmentList.Count; i++)
				{
					if (segmentList[i].IsContaining(position))
					{
						return true;
					}
				}
				return false;
			}

			public bool IsContaining(ConveyorBeltLinearSegment segment)
			{
				for (int i = 0; i < segmentList.Count; i++)
				{
					if (segmentList[i] == segment)
					{
						return true;
					}
				}
				return false;
			}
		}

		[SerializeField]
		public string name;

		[SerializeField]
		public string nextLevelName;

		[SerializeField]
		public string prevLevelName;

		[SerializeField]
		public bool lockLevelForEditing;

		[SerializeField]
		private string description;

		[SerializeField]
		private string notes;

		[SerializeField]
		public string tags;

		public long versionIndex;

		public Size size = new Size();

		public SuggestMoveType suggestMoveType;

		public ShowPotentialMatchSetting suggestMoveSetting;

		public int numChips = 5;

		[SerializeField]
		public ChipGenerationSettings generationSettings = new ChipGenerationSettings();

		public bool isPowerupPlacementSuspended;

		public List<TutorialMatch> tutorialMatches = new List<TutorialMatch>();

		public bool isPreventingGeneratorChipMatching;

		public bool useChanceToNotPreventChipMatching;

		public float chanceToNotPreventChipMatching = 50f;

		public List<SlotDefinition> slots = new List<SlotDefinition>();

		public BurriedElements burriedElements = new BurriedElements();

		public MonsterElements monsterElements = new MonsterElements();

		public GoalsDefinition goals = new GoalsDefinition();

		public ExtraFallingElements extraFallingElements = new ExtraFallingElements();

		public List<GeneratorSetup> generatorSetups = new List<GeneratorSetup>();

		public List<GeneratorSlotSettings> generatorSlotSettings = new List<GeneratorSlotSettings>();

		public int portalIndexCount
		{
			get
			{
				int num = 0;
				for (int i = 0; i < slots.Count; i++)
				{
					num = Mathf.Max(slots[i].portalEntranceIndex + 1, num);
				}
				return num;
			}
		}

		public ConveyorBelts GetConveyorBelts()
		{
			ConveyorBelts conveyorBelts = new ConveyorBelts();
			conveyorBelts.Init(this);
			return conveyorBelts;
		}

		public List<Portal> GetAllPortals()
		{
			List<Portal> list = new List<Portal>();
			for (int i = 0; i < slots.Count; i++)
			{
				SlotDefinition slotDefinition = slots[i];
				if (slotDefinition.isPortalExit)
				{
					int portalExitIndex = slotDefinition.portalExitIndex;
					Portal portal = null;
					for (int j = 0; j < list.Count; j++)
					{
						Portal portal2 = list[j];
						if (portal2.HasEntranceIndex(portalExitIndex))
						{
							portal = portal2;
							break;
						}
					}
					if (portal == null)
					{
						portal = new Portal();
						list.Add(portal);
					}
					portal.exitSlot = slotDefinition;
				}
				if (!slotDefinition.isPortalEntrance)
				{
					continue;
				}
				int portalEntranceIndex = slotDefinition.portalEntranceIndex;
				Portal portal3 = null;
				for (int k = 0; k < list.Count; k++)
				{
					Portal portal4 = list[k];
					if (portal4.HasExitIndex(portalEntranceIndex))
					{
						portal3 = portal4;
						break;
					}
				}
				if (portal3 == null)
				{
					portal3 = new Portal();
					list.Add(portal3);
				}
				portal3.entranceSlot = slotDefinition;
			}

			foreach (var VARIABLE in list)
			{
				Debug.LogError(VARIABLE.entranceSlot.position);
				Debug.LogError(VARIABLE.exitSlot.position);
			}
			
			Debug.LogError(list.Count);
			return list;
		}

		public GeneratorSlotSettings GetGeneratorSlotSettings(int index)
		{
			if (index < 0)
			{
				return null;
			}
			if (index >= generatorSlotSettings.Count)
			{
				return null;
			}
			return generatorSlotSettings[index];
		}

		public SlotDefinition GetSlot(IntVector2 position)
		{
			if (position.x < 0 || position.y < 0 || position.x >= size.width || position.y >= size.height)
			{
				return null;
			}
			int index = size.width * position.y + position.x;
			return slots[index];
		}

		public void SetSlot(IntVector2 position, SlotDefinition slot)
		{
			int index = size.width * position.y + position.x;
			slots[index] = slot;
		}

		public void ExchangeBurriedElementsForSmallOnes()
		{
			List<BurriedElement> list = new List<BurriedElement>();
			list.AddRange(burriedElements.elements);
			burriedElements.elements.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				BurriedElement burriedElement = list[i];
				IntVector2 position = burriedElement.position;
				IntVector2 oppositeCornerPosition = burriedElement.oppositeCornerPosition;
				int num = Mathf.Min(position.x, oppositeCornerPosition.x);
				int num2 = Mathf.Max(position.x, oppositeCornerPosition.x);
				int num3 = Mathf.Min(position.y, oppositeCornerPosition.y);
				int num4 = Mathf.Max(position.y, oppositeCornerPosition.y);
				for (int j = num; j <= num2; j++)
				{
					for (int k = num3; k <= num4; k++)
					{
						BurriedElement burriedElement2 = new BurriedElement();
						burriedElement2.position = new IntVector2(j, k);
						burriedElement2.size = new IntVector2(1, 1);
						burriedElement2.orientation = BurriedElement.Orientation.Vertical;
						burriedElements.elements.Add(burriedElement2);
					}
				}
			}
		}

		public LevelDefinition Clone()
		{
			LevelDefinition levelDefinition = new LevelDefinition();
			levelDefinition.name = name;
			levelDefinition.nextLevelName = nextLevelName;
			levelDefinition.prevLevelName = prevLevelName;
			levelDefinition.size = size.Clone();
			levelDefinition.numChips = numChips;
			levelDefinition.generationSettings = generationSettings.Clone();
			levelDefinition.suggestMoveType = suggestMoveType;
			levelDefinition.suggestMoveSetting = suggestMoveSetting;
			levelDefinition.isPowerupPlacementSuspended = isPowerupPlacementSuspended;
			levelDefinition.isPreventingGeneratorChipMatching = isPreventingGeneratorChipMatching;
			levelDefinition.chanceToNotPreventChipMatching = chanceToNotPreventChipMatching;
			levelDefinition.useChanceToNotPreventChipMatching = useChanceToNotPreventChipMatching;
			levelDefinition.burriedElements = burriedElements.Clone();
			levelDefinition.monsterElements = monsterElements.Clone();
			levelDefinition.goals = goals.Clone();
			levelDefinition.extraFallingElements = extraFallingElements.Clone();
			for (int i = 0; i < this.generatorSlotSettings.Count; i++)
			{
				GeneratorSlotSettings generatorSlotSettings = this.generatorSlotSettings[i];
				levelDefinition.generatorSlotSettings.Add(generatorSlotSettings.Clone());
			}
			for (int j = 0; j < tutorialMatches.Count; j++)
			{
				TutorialMatch tutorialMatch = tutorialMatches[j];
				levelDefinition.tutorialMatches.Add(tutorialMatch.Clone());
			}
			for (int k = 0; k < slots.Count; k++)
			{
				SlotDefinition slotDefinition = slots[k];
				levelDefinition.slots.Add(slotDefinition.Clone());
			}
			for (int l = 0; l < generatorSetups.Count; l++)
			{
				GeneratorSetup generatorSetup = generatorSetups[l];
				levelDefinition.generatorSetups.Add(generatorSetup.Clone());
			}
			return levelDefinition;
		}

		public void EnsureSizeAndInit()
		{
			for (int i = 0; i < size.height; i++)
			{
				for (int j = 0; j < size.width; j++)
				{
					int num = size.width * i + j;
					if (num >= slots.Count)
					{
						SlotDefinition slotDefinition = new SlotDefinition();
						slotDefinition.gravitySettings.down = true;
						slotDefinition.slotType = SlotType.Empty;
						slots.Add(slotDefinition);
					}
					slots[num].position = new IntVector2(j, i);
				}
			}
		}

		public int CountChips(ChipType type)
		{
			int num = 0;
			for (int i = 0; i < slots.Count; i++)
			{
				SlotDefinition slotDefinition = slots[i];
				if (slotDefinition.chipType == type)
				{
					num++;
				}
				if (type == ChipType.Carpet && !slotDefinition.hasCarpet)
				{
					num++;
				}
			}
			if (type == ChipType.FallingGingerbreadMan)
			{
				num += extraFallingElements.fallingElementsList.Count;
			}
			if (type == ChipType.BurriedElement)
			{
				num = burriedElements.CountAllElements();
			}
			return num;
		}
	}
}
