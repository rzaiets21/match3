using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class PopulateBoard
	{
		public class BoardRepresentation
		{
			public class RepresentationSlot
			{
				public bool needsToBeGenerated;

				public bool isGenerated;

				public ItemColor cachedColor;

				public bool wallLeft;

				public bool wallRight;

				public bool wallUp;

				public bool wallDown;

				public bool isFormingColorMatchesSuspended;

				public IntVector2 position;

				public ItemColor itemColor;

				public bool canMove;

				public bool isOutOfPlayArea;

				public bool isPositionInEmptyNeighbourhoodAtStart;

				public bool isMatchCheckingRequired;

				public bool canFormColorMatches
				{
					get
					{
						if (isFormingColorMatchesSuspended)
						{
							return false;
						}
						if (needsToBeGenerated && !isGenerated)
						{
							return false;
						}
						if (isOutOfPlayArea)
						{
							return false;
						}
						return true;
					}
				}

				private bool IsBlocked(IntVector2 direction)
				{
					if (direction.x < 0 && wallLeft)
					{
						return true;
					}
					if (direction.x > 0 && wallRight)
					{
						return true;
					}
					if (direction.y < 0 && wallDown)
					{
						return true;
					}
					if (direction.y > 0 && wallUp)
					{
						return true;
					}
					return false;
				}

				public bool IsBlockedTo(RepresentationSlot slot)
				{
					IntVector2 intVector = slot.position - position;
					if (IsBlocked(intVector) || slot.IsBlocked(-intVector))
					{
						return true;
					}
					return false;
				}
			}

			public IntVector2 size;

			public List<RepresentationSlot> slots = new List<RepresentationSlot>();

			private List<IntVector2> slotNeighboursOffsets_;

			public List<IntVector2> slotNeighboursOffsets
			{
				get
				{
					if (slotNeighboursOffsets_ == null)
					{
						slotNeighboursOffsets_ = new List<IntVector2>(4);
						slotNeighboursOffsets_.Add(IntVector2.up);
						slotNeighboursOffsets_.Add(IntVector2.down);
						slotNeighboursOffsets_.Add(IntVector2.left);
						slotNeighboursOffsets_.Add(IntVector2.right);
					}
					return slotNeighboursOffsets_;
				}
			}

			public RepresentationSlot GetSlot(IntVector2 pos)
			{
				if (pos.x < 0 || pos.y < 0 || pos.x >= size.x || pos.y >= size.y)
				{
					return null;
				}
				int index = pos.x + pos.y * size.x;
				return slots[index];
			}

			public void Init(LevelDefinition level)
			{
				slots.Clear();
				size.x = level.size.width;
				size.y = level.size.height;
				List<LevelDefinition.SlotDefinition> list = level.slots;
				for (int i = 0; i < list.Count; i++)
				{
					LevelDefinition.SlotDefinition slotDefinition = list[i];
					RepresentationSlot representationSlot = new RepresentationSlot();
					representationSlot.position = slotDefinition.position;
					if (slotDefinition.slotType == SlotType.Empty)
					{
						representationSlot.isOutOfPlayArea = true;
						slots.Add(representationSlot);
						continue;
					}
					representationSlot.needsToBeGenerated = slotDefinition.needsToBeGenerated;
					representationSlot.itemColor = slotDefinition.itemColor;
					representationSlot.isFormingColorMatchesSuspended = slotDefinition.isFormingMatchesSuspended(level);
					representationSlot.canMove = !slotDefinition.isMoveSuspended(level);
					representationSlot.wallUp = slotDefinition.wallSettings.up;
					representationSlot.wallDown = slotDefinition.wallSettings.down;
					representationSlot.wallLeft = slotDefinition.wallSettings.left;
					representationSlot.wallRight = slotDefinition.wallSettings.right;
					slots.Add(representationSlot);
				}
			}

			public void Init(Match3Game game, bool generateFlowerChips)
			{
				slots.Clear();
				Match3Board board = game.board;
				size = board.size;
				Slot[] slot2 = board.slots;
				for (int i = 0; i < size.y; i++)
				{
					for (int j = 0; j < size.x; j++)
					{
						IntVector2 intVector = new IntVector2(j, i);
						Slot slot = board.GetSlot(intVector);
						RepresentationSlot representationSlot = new RepresentationSlot();
						representationSlot.position = intVector;
						if (slot == null)
						{
							representationSlot.isOutOfPlayArea = true;
							slots.Add(representationSlot);
							continue;
						}
						representationSlot.canMove = !slot.isSlotSwapSuspended;
						representationSlot.wallUp = Slot.IsPathBlockedBetween(slot, board.GetSlot(intVector + IntVector2.up));
						representationSlot.wallDown = Slot.IsPathBlockedBetween(slot, board.GetSlot(intVector + IntVector2.down));
						representationSlot.wallLeft = Slot.IsPathBlockedBetween(slot, board.GetSlot(intVector + IntVector2.left));
						representationSlot.wallRight = Slot.IsPathBlockedBetween(slot, board.GetSlot(intVector + IntVector2.right));
						representationSlot.isFormingColorMatchesSuspended = slot.isSlotMatchingSuspended;
						Chip slotComponent = slot.GetSlotComponent<Chip>();
						if (slotComponent == null)
						{
							representationSlot.needsToBeGenerated = false;
							representationSlot.itemColor = ItemColor.Unknown;
							slots.Add(representationSlot);
							continue;
						}
						ChipType chipType = slotComponent.chipType;
						representationSlot.needsToBeGenerated = (chipType == ChipType.Chip);
						if (!generateFlowerChips && slotComponent.hasGrowingElement)
						{
							representationSlot.needsToBeGenerated = false;
						}
						representationSlot.itemColor = slotComponent.itemColor;
						representationSlot.isFormingColorMatchesSuspended = !slotComponent.canFormColorMatches;
						slots.Add(representationSlot);
					}
				}
			}
		}

		public class PotentialMatch
		{
			public List<BoardRepresentation.RepresentationSlot> slotsThatNeedToBeTheSame = new List<BoardRepresentation.RepresentationSlot>();

			public PotentialMatch(BoardRepresentation.RepresentationSlot slot1, BoardRepresentation.RepresentationSlot slot2, BoardRepresentation.RepresentationSlot slot3)
			{
				slotsThatNeedToBeTheSame.Add(slot1);
				slotsThatNeedToBeTheSame.Add(slot2);
				slotsThatNeedToBeTheSame.Add(slot3);
			}
		}

		public class Params
		{
			public List<ItemColor> availableColors = new List<ItemColor>();

			public int maxPotentialMatches = 4;

			public RandomProvider randomProvider;
		}

		public class MatchBuilder
		{
			public class Match
			{
				public List<BoardRepresentation.RepresentationSlot> slots = new List<BoardRepresentation.RepresentationSlot>();

				public List<BoardRepresentation.RepresentationSlot> allSlots_ = new List<BoardRepresentation.RepresentationSlot>();

				public BoardRepresentation.RepresentationSlot movingSlot;

				public List<ItemColor> availableColor = new List<ItemColor>();

				public List<BoardRepresentation.RepresentationSlot> allSlots
				{
					get
					{
						allSlots_.Clear();
						allSlots_.AddRange(slots);
						allSlots_.Add(movingSlot);
						return allSlots_;
					}
				}
			}

			public class MatchPattern
			{
				public List<SlotCandidate> matching = new List<SlotCandidate>();

				public SlotCandidate moving;

				public BoardRepresentation.RepresentationSlot swipeSlot;

				public List<ItemColor> availableColors = new List<ItemColor>();

				public List<ItemColor> swipeSlotAvailableColors = new List<ItemColor>();

				public int match1Index;

				public int match2Index;

				public int movingIndex;

				public bool isMatch
				{
					get
					{
						if (swipeSlot != null)
						{
							return swipeSlotAvailableColors.Count > 0;
						}
						return false;
					}
				}

				public MatchPattern(int match1Index, int match2Index, int movingIndex)
				{
					this.match1Index = match1Index;
					this.match2Index = match2Index;
					this.movingIndex = movingIndex;
				}

				public void Clear()
				{
					matching.Clear();
					availableColors.Clear();
					swipeSlot = null;
					swipeSlotAvailableColors.Clear();
				}

				public void Init(List<SlotCandidate> candidates)
				{
					Clear();
					matching.Add(candidates[match1Index]);
					matching.Add(candidates[match2Index]);
					moving = candidates[movingIndex];
				}

				public void FillAvailableColors()
				{
					List<ItemColor> a = matching[0].availableColors;
					List<ItemColor> b = matching[1].availableColors;
					GGUtil.Intersection(a, b, availableColors);
					if (moving.slot.canFormColorMatches)
					{
						availableColors.Remove(moving.slot.itemColor);
					}
				}

				private bool IsInList(BoardRepresentation.RepresentationSlot slot, List<SlotCandidate> list)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].slot == slot)
						{
							return true;
						}
					}
					return false;
				}

				public void FindSwipeSlot(BoardRepresentation board, MatchingCheck matchingCheck)
				{
					List<IntVector2> slotNeighboursOffsets = board.slotNeighboursOffsets;
					BoardRepresentation.RepresentationSlot slot = moving.slot;
					IntVector2 position = moving.slot.position;
					int num = 0;
					BoardRepresentation.RepresentationSlot slot2;
					while (true)
					{
						if (num >= slotNeighboursOffsets.Count)
						{
							return;
						}
						IntVector2 b = slotNeighboursOffsets[num];
						IntVector2 pos = position + b;
						slot2 = board.GetSlot(pos);
						if (slot2 != null && slot2.canFormColorMatches && slot2.canMove && !slot2.IsBlockedTo(slot) && !IsInList(slot2, matching))
						{
							swipeSlotAvailableColors.Clear();
							if (!slot2.needsToBeGenerated)
							{
								if (!availableColors.Contains(slot2.itemColor))
								{
									goto IL_00f6;
								}
								swipeSlotAvailableColors.Add(slot2.itemColor);
							}
							else
							{
								matchingCheck.Check(board, pos, availableColors);
								swipeSlotAvailableColors.AddRange(matchingCheck.availableColors);
							}
							if (swipeSlotAvailableColors.Count != 0)
							{
								break;
							}
						}
						goto IL_00f6;
						IL_00f6:
						num++;
					}
					swipeSlot = slot2;
				}
			}

			public class SlotCandidate
			{
				public BoardRepresentation.RepresentationSlot slot;

				public List<ItemColor> availableColors = new List<ItemColor>();

				public ItemColor originalItemColor;

				public bool originalIsGenerated;

				public void Init(BoardRepresentation.RepresentationSlot slot)
				{
					this.slot = slot;
					availableColors.Clear();
					if (slot != null)
					{
						originalItemColor = slot.itemColor;
						originalIsGenerated = slot.isGenerated;
					}
				}
			}

			public List<SlotCandidate> candidates = new List<SlotCandidate>();

			private List<MatchPattern> matchPatterns = new List<MatchPattern>();

			public void Init(BoardRepresentation.RepresentationSlot slot1, BoardRepresentation.RepresentationSlot slot2, BoardRepresentation.RepresentationSlot slot3)
			{
				while (candidates.Count < 3)
				{
					candidates.Add(new SlotCandidate());
				}
				if (matchPatterns.Count < 3)
				{
					matchPatterns.Clear();
					matchPatterns.Add(new MatchPattern(0, 1, 2));
					matchPatterns.Add(new MatchPattern(1, 2, 0));
					matchPatterns.Add(new MatchPattern(0, 2, 1));
				}
				candidates[0].Init(slot1);
				candidates[1].Init(slot2);
				candidates[2].Init(slot3);
				for (int i = 0; i < matchPatterns.Count; i++)
				{
					matchPatterns[i].Clear();
				}
			}

			public bool Find(MatchingCheck matchCheck, BoardRepresentation board, List<ItemColor> availableColors)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < candidates.Count; i++)
				{
					SlotCandidate slotCandidate = candidates[i];
					if (slotCandidate.slot == null)
					{
						return false;
					}
					if (slotCandidate.slot.canFormColorMatches)
					{
						num++;
					}
					if (!slotCandidate.slot.canFormColorMatches && slotCandidate.slot.canMove)
					{
						num3++;
					}
					if (slotCandidate.slot.canMove)
					{
						num2++;
					}
				}
				if ((num != 2 || num3 != 1) && (num != 3 || num2 < 1))
				{
					return false;
				}
				FillAvailableColorsForAllCandidates(matchCheck, board, availableColors);
				bool result = false;
				for (int j = 0; j < matchPatterns.Count; j++)
				{
					MatchPattern matchPattern = matchPatterns[j];
					matchPattern.Init(candidates);
					matchPattern.FillAvailableColors();
					if (matchPattern.availableColors.Count != 0 && matchPattern.moving.slot.canMove)
					{
						matchPattern.FindSwipeSlot(board, matchCheck);
						if (matchPattern.swipeSlot != null)
						{
							result = true;
						}
					}
				}
				return result;
			}

			public void FillMatchesIn(List<Match> matchesList)
			{
				for (int i = 0; i < matchPatterns.Count; i++)
				{
					MatchPattern matchPattern = matchPatterns[i];
					if (matchPattern.isMatch)
					{
						Match match = new Match();
						for (int j = 0; j < matchPattern.matching.Count; j++)
						{
							SlotCandidate slotCandidate = matchPattern.matching[j];
							match.slots.Add(slotCandidate.slot);
						}
						match.slots.Add(matchPattern.swipeSlot);
						match.availableColor.AddRange(matchPattern.swipeSlotAvailableColors);
						matchesList.Add(match);
						match.movingSlot = matchPattern.moving.slot;
					}
				}
			}

			private void FillAvailableColorsForAllCandidates(MatchingCheck matchCheck, BoardRepresentation board, List<ItemColor> availableColors)
			{
				for (int i = 0; i < candidates.Count; i++)
				{
					SlotCandidate slotCandidate = candidates[i];
					if (slotCandidate.slot.needsToBeGenerated)
					{
						slotCandidate.slot.isGenerated = false;
					}
				}
				for (int j = 0; j < candidates.Count; j++)
				{
					SlotCandidate slotCandidate2 = candidates[j];
					if (!slotCandidate2.slot.isFormingColorMatchesSuspended)
					{
						if (!slotCandidate2.slot.needsToBeGenerated)
						{
							slotCandidate2.availableColors.Add(slotCandidate2.originalItemColor);
							continue;
						}
						matchCheck.Check(board, slotCandidate2.slot.position, availableColors);
						slotCandidate2.availableColors.AddRange(matchCheck.availableColors);
					}
				}
				for (int k = 0; k < candidates.Count; k++)
				{
					SlotCandidate slotCandidate3 = candidates[k];
					if (slotCandidate3.slot.needsToBeGenerated)
					{
						slotCandidate3.slot.isGenerated = slotCandidate3.originalIsGenerated;
					}
				}
			}
		}

		public class MatchingCheck
		{
			public struct MatchingResult
			{
				public bool isMatching;

				public ItemColor itemColor;
			}

			public class MatchPositionList
			{
				public List<IntVector2> positionOffsets = new List<IntVector2>();

				public MatchPositionList(IntVector2 pos1, IntVector2 pos2)
				{
					positionOffsets.Add(pos1);
					positionOffsets.Add(pos2);
				}

				public MatchPositionList(IntVector2 pos1, IntVector2 pos2, IntVector2 pos3)
				{
					positionOffsets.Add(pos1);
					positionOffsets.Add(pos2);
					positionOffsets.Add(pos3);
				}

				public MatchingResult GetMatchingResult(BoardRepresentation board, IntVector2 originPos)
				{
					MatchingResult result = default(MatchingResult);
					BoardRepresentation.RepresentationSlot representationSlot = null;
					for (int i = 0; i < positionOffsets.Count; i++)
					{
						BoardRepresentation.RepresentationSlot slot = board.GetSlot(originPos + positionOffsets[i]);
						if (slot == null)
						{
							return result;
						}
						if (!slot.canFormColorMatches)
						{
							return result;
						}
						if (representationSlot == null)
						{
							representationSlot = slot;
						}
						else if (!IsMatching(representationSlot, slot))
						{
							return result;
						}
					}
					if (representationSlot == null)
					{
						return result;
					}
					result.isMatching = true;
					result.itemColor = representationSlot.itemColor;
					return result;
				}
			}

			private BoardRepresentation board;

			private IntVector2 originPos;

			public List<ItemColor> colorsThatWouldFormAMatch = new List<ItemColor>();

			public List<ItemColor> availableColors = new List<ItemColor>();

			private List<MatchPositionList> matchingPositionsList = new List<MatchPositionList>();

			public MatchingCheck()
			{
				Init();
			}

			private void Init()
			{
				matchingPositionsList.Clear();
				matchingPositionsList.Add(new MatchPositionList(IntVector2.left, IntVector2.right));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.up, IntVector2.down));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.up, 2 * IntVector2.up));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.down, 2 * IntVector2.down));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.left, 2 * IntVector2.left));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.right, 2 * IntVector2.right));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.up, IntVector2.right, IntVector2.up + IntVector2.right));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.up, IntVector2.left, IntVector2.up + IntVector2.left));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.down, IntVector2.right, IntVector2.down + IntVector2.right));
				matchingPositionsList.Add(new MatchPositionList(IntVector2.down, IntVector2.left, IntVector2.down + IntVector2.left));
			}

			private void Clear()
			{
				colorsThatWouldFormAMatch.Clear();
				availableColors.Clear();
			}

			private void AddColorThatWouldFormAMatch(ItemColor color)
			{
				if (!colorsThatWouldFormAMatch.Contains(color))
				{
					colorsThatWouldFormAMatch.Add(color);
					int num = availableColors.IndexOf(color);
					if (num >= 0)
					{
						availableColors.RemoveAt(num);
					}
				}
			}

			public void Check(BoardRepresentation board, IntVector2 pos, List<ItemColor> availableColors)
			{
				Clear();
				this.board = board;
				originPos = pos;
				if (availableColors != null)
				{
					this.availableColors.AddRange(availableColors);
				}
				for (int i = 0; i < matchingPositionsList.Count; i++)
				{
					MatchingResult matchingResult = matchingPositionsList[i].GetMatchingResult(board, pos);
					if (matchingResult.isMatching)
					{
						AddColorThatWouldFormAMatch(matchingResult.itemColor);
					}
				}
			}

			public static bool IsMatching(BoardRepresentation.RepresentationSlot slot1, BoardRepresentation.RepresentationSlot slot2)
			{
				if (slot1 == null || slot2 == null)
				{
					return false;
				}
				if (!slot1.canFormColorMatches || !slot2.canFormColorMatches)
				{
					return false;
				}
				return slot1.itemColor == slot2.itemColor;
			}
		}

		private List<BoardRepresentation.RepresentationSlot> mustEnsureNoMatching = new List<BoardRepresentation.RepresentationSlot>();

		private List<BoardRepresentation.RepresentationSlot> canHaveAnyColor = new List<BoardRepresentation.RepresentationSlot>();

		private List<BoardRepresentation.RepresentationSlot> canNotFormMatches = new List<BoardRepresentation.RepresentationSlot>();

		private List<IntVector2> cachedList = new List<IntVector2>();

		private Params initParams;

		private MatchingCheck matchingCheck = new MatchingCheck();

		private MatchBuilder matchBuilder = new MatchBuilder();

		public BoardRepresentation board = new BoardRepresentation();

		private void Clear()
		{
			mustEnsureNoMatching.Clear();
			canHaveAnyColor.Clear();
			canNotFormMatches.Clear();
			cachedList.Clear();
		}

		private ItemColor RandomColor()
		{
			return RandomColor(initParams.availableColors);
		}

		private ItemColor RandomColor(List<ItemColor> availableColors)
		{
			return availableColors[initParams.randomProvider.Range(0, availableColors.Count)];
		}

		public void RandomPopulate(LevelDefinition level, Params initParams)
		{
			board.Init(level);
			RandomPopulate(board, initParams);
		}

		public bool RandomPopulate(BoardRepresentation board, Params initParams)
		{
			this.initParams = initParams;
			this.board = board;
			Clear();
			List<IntVector2> list = cachedList;
			list.Clear();
			list.Add(IntVector2.up);
			list.Add(IntVector2.down);
			list.Add(IntVector2.left);
			list.Add(IntVector2.right);
			List<BoardRepresentation.RepresentationSlot> slots = board.slots;
			for (int i = 0; i < slots.Count; i++)
			{
				BoardRepresentation.RepresentationSlot representationSlot = slots[i];
				if (!representationSlot.needsToBeGenerated || representationSlot.isOutOfPlayArea)
				{
					continue;
				}
				if (representationSlot.isFormingColorMatchesSuspended)
				{
					canNotFormMatches.Add(representationSlot);
					continue;
				}
				bool flag = false;
				for (int j = 0; j < list.Count; j++)
				{
					IntVector2 b = list[j];
					IntVector2 pos = representationSlot.position + b;
					BoardRepresentation.RepresentationSlot slot = board.GetSlot(pos);
					if (slot != null && (slot.canFormColorMatches || slot.isPositionInEmptyNeighbourhoodAtStart))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					representationSlot.isMatchCheckingRequired = true;
					mustEnsureNoMatching.Add(representationSlot);
				}
				else
				{
					representationSlot.isPositionInEmptyNeighbourhoodAtStart = true;
					canHaveAnyColor.Add(representationSlot);
				}
			}
			for (int k = 0; k < canNotFormMatches.Count; k++)
			{
				BoardRepresentation.RepresentationSlot representationSlot2 = canNotFormMatches[k];
				representationSlot2.isGenerated = true;
				representationSlot2.itemColor = RandomColor();
			}
			List<PotentialMatch> list2 = new List<PotentialMatch>();
			for (int l = 0; l < canHaveAnyColor.Count; l++)
			{
				BoardRepresentation.RepresentationSlot representationSlot3 = canHaveAnyColor[l];
				representationSlot3.isGenerated = true;
				representationSlot3.itemColor = RandomColor();
				BoardRepresentation.RepresentationSlot slot2 = board.GetSlot(representationSlot3.position + IntVector2.right);
				BoardRepresentation.RepresentationSlot slot3 = board.GetSlot(representationSlot3.position + IntVector2.right * 2);
				BoardRepresentation.RepresentationSlot slot4 = board.GetSlot(representationSlot3.position + IntVector2.right + IntVector2.up);
				BoardRepresentation.RepresentationSlot slot5 = board.GetSlot(representationSlot3.position + IntVector2.right + IntVector2.down);
				BoardRepresentation.RepresentationSlot slot6 = board.GetSlot(representationSlot3.position + IntVector2.up);
				BoardRepresentation.RepresentationSlot slot7 = board.GetSlot(representationSlot3.position + IntVector2.up * 2);
				bool num = IsAvailableForSwap(representationSlot3, slot2) && IsInCanHaveAnyColor(slot3);
				if (num && IsInCanHaveAnyColor(slot4))
				{
					PotentialMatch item = new PotentialMatch(representationSlot3, slot3, slot4);
					list2.Add(item);
				}
				if (num && IsInCanHaveAnyColor(slot5))
				{
					PotentialMatch item2 = new PotentialMatch(representationSlot3, slot3, slot5);
					list2.Add(item2);
				}
				bool num2 = IsAvailableForSwap(representationSlot3, slot6) && IsInCanHaveAnyColor(slot7);
				BoardRepresentation.RepresentationSlot slot8 = board.GetSlot(representationSlot3.position + IntVector2.left + IntVector2.up);
				BoardRepresentation.RepresentationSlot slot9 = board.GetSlot(representationSlot3.position + IntVector2.right + IntVector2.up);
				if (num2 && IsInCanHaveAnyColor(slot8))
				{
					PotentialMatch item3 = new PotentialMatch(representationSlot3, slot7, slot8);
					list2.Add(item3);
				}
				if (num2 && IsInCanHaveAnyColor(slot9))
				{
					PotentialMatch item4 = new PotentialMatch(representationSlot3, slot7, slot9);
					list2.Add(item4);
				}
			}
			GGUtil.Shuffle(list2, initParams.randomProvider);
			int maxPotentialMatches = initParams.maxPotentialMatches;
			for (int m = 0; m < Mathf.Min(list2.Count, maxPotentialMatches); m++)
			{
				PotentialMatch potentialMatch = list2[m];
				ItemColor itemColor = RandomColor();
				for (int n = 0; n < potentialMatch.slotsThatNeedToBeTheSame.Count; n++)
				{
					potentialMatch.slotsThatNeedToBeTheSame[n].itemColor = itemColor;
				}
			}
			for (int num3 = 0; num3 < mustEnsureNoMatching.Count; num3++)
			{
				BoardRepresentation.RepresentationSlot representationSlot4 = mustEnsureNoMatching[num3];
				matchingCheck.Check(board, representationSlot4.position, initParams.availableColors);
				List<ItemColor> availableColors = matchingCheck.availableColors;
				if (availableColors.Count > 0)
				{
					ItemColor itemColor2 = representationSlot4.itemColor = RandomColor(availableColors);
					representationSlot4.isGenerated = true;
					continue;
				}
				List<IntVector2> list3 = cachedList;
				list3.Clear();
				list3.Add(IntVector2.up);
				list3.Add(IntVector2.down);
				list3.Add(IntVector2.left);
				list3.Add(IntVector2.right);
				list3.Add(2 * IntVector2.up);
				list3.Add(2 * IntVector2.down);
				list3.Add(2 * IntVector2.left);
				list3.Add(2 * IntVector2.right);
				BoardRepresentation.RepresentationSlot representationSlot5 = null;
				for (int num4 = 0; num4 < list3.Count; num4++)
				{
					IntVector2 pos2 = representationSlot4.position + list3[num4];
					BoardRepresentation.RepresentationSlot slot10 = board.GetSlot(pos2);
					if (slot10 != null && slot10.needsToBeGenerated && slot10.canFormColorMatches)
					{
						matchingCheck.Check(board, slot10.position, initParams.availableColors);
						availableColors = matchingCheck.availableColors;
						if (availableColors.Count >= 2)
						{
							representationSlot5 = slot10;
							break;
						}
					}
				}
				if (representationSlot5 == null)
				{
					GenerateSlotInMatch(representationSlot4);
					continue;
				}
				representationSlot5.itemColor = RandomColor(availableColors);
				matchingCheck.Check(board, representationSlot4.position, initParams.availableColors);
				availableColors = matchingCheck.availableColors;
				if (availableColors.Count <= 0)
				{
					GenerateSlotInMatch(representationSlot4);
					continue;
				}
				representationSlot4.itemColor = RandomColor(availableColors);
				representationSlot4.isGenerated = true;
			}
			if (list2.Count > 0)
			{
				return true;
			}
			List<MatchBuilder.Match> list4 = new List<MatchBuilder.Match>();
			IntVector2 right = IntVector2.right;
			for (int num5 = 0; num5 < board.size.y; num5++)
			{
				for (int num6 = 0; num6 < board.size.x - 2; num6++)
				{
					IntVector2 intVector = new IntVector2(num6, num5);
					BoardRepresentation.RepresentationSlot slot11 = board.GetSlot(intVector);
					BoardRepresentation.RepresentationSlot slot12 = board.GetSlot(intVector + right);
					BoardRepresentation.RepresentationSlot slot13 = board.GetSlot(intVector + right * 2);
					matchBuilder.Init(slot11, slot12, slot13);
					if (matchBuilder.Find(matchingCheck, board, initParams.availableColors))
					{
						matchBuilder.FillMatchesIn(list4);
					}
				}
			}
			right = IntVector2.up;
			for (int num7 = 0; num7 < board.size.x; num7++)
			{
				for (int num8 = 0; num8 < board.size.y - 2; num8++)
				{
					IntVector2 intVector2 = new IntVector2(num7, num8);
					BoardRepresentation.RepresentationSlot slot14 = board.GetSlot(intVector2);
					BoardRepresentation.RepresentationSlot slot15 = board.GetSlot(intVector2 + right);
					BoardRepresentation.RepresentationSlot slot16 = board.GetSlot(intVector2 + right * 2);
					matchBuilder.Init(slot14, slot15, slot16);
					if (matchBuilder.Find(matchingCheck, board, initParams.availableColors))
					{
						matchBuilder.FillMatchesIn(list4);
					}
				}
			}
			if (list4.Count == 0)
			{
				return false;
			}
			GGUtil.Shuffle(list4, initParams.randomProvider);
			List<ItemColor> list5 = new List<ItemColor>();
			List<MatchBuilder.Match> list6 = new List<MatchBuilder.Match>();
			for (int num9 = 0; num9 < list4.Count; num9++)
			{
				MatchBuilder.Match match = list4[num9];
				if (IsIntersectingWithList(match, list6))
				{
					continue;
				}
				for (int num10 = 0; num10 < match.slots.Count; num10++)
				{
					BoardRepresentation.RepresentationSlot representationSlot6 = match.slots[num10];
					representationSlot6.cachedColor = representationSlot6.itemColor;
				}
				bool flag2 = false;
				for (int num11 = 0; num11 < match.availableColor.Count; num11++)
				{
					ItemColor itemColor3 = match.availableColor[num11];
					list5.Clear();
					list5.Add(itemColor3);
					for (int num12 = 0; num12 < match.slots.Count; num12++)
					{
						BoardRepresentation.RepresentationSlot representationSlot7 = match.slots[num12];
						representationSlot7.isGenerated = true;
						representationSlot7.itemColor = itemColor3;
					}
					bool flag3 = true;
					for (int num13 = 0; num13 < match.slots.Count; num13++)
					{
						BoardRepresentation.RepresentationSlot representationSlot8 = match.slots[num13];
						matchingCheck.Check(board, representationSlot8.position, list5);
						if (matchingCheck.availableColors.Count == 0)
						{
							flag3 = false;
							break;
						}
					}
					if (flag3)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					for (int num14 = 0; num14 < match.slots.Count; num14++)
					{
						BoardRepresentation.RepresentationSlot representationSlot9 = match.slots[num14];
						representationSlot9.isGenerated = true;
						representationSlot9.itemColor = representationSlot9.cachedColor;
					}
				}
				else
				{
					list6.Add(match);
					if (list6.Count >= maxPotentialMatches)
					{
						break;
					}
				}
			}
			return true;
		}

		private bool IsIntersectingWithList(MatchBuilder.Match match, List<MatchBuilder.Match> list)
		{
			List<BoardRepresentation.RepresentationSlot> allSlots = match.allSlots;
			for (int i = 0; i < allSlots.Count; i++)
			{
				BoardRepresentation.RepresentationSlot representationSlot = allSlots[i];
				for (int j = 0; j < list.Count; j++)
				{
					List<BoardRepresentation.RepresentationSlot> allSlots2 = list[j].allSlots;
					for (int k = 0; k < allSlots2.Count; k++)
					{
						if (allSlots2[k] == representationSlot)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool IsAvailableForSwap(BoardRepresentation.RepresentationSlot fromSlot, BoardRepresentation.RepresentationSlot slot)
		{
			if (slot == null)
			{
				return false;
			}
			if (fromSlot == null)
			{
				return false;
			}
			if (slot.isOutOfPlayArea)
			{
				return false;
			}
			if (fromSlot.IsBlockedTo(slot))
			{
				return false;
			}
			return slot.canMove;
		}

		private bool IsInCanHaveAnyColor(BoardRepresentation.RepresentationSlot slot)
		{
			return slot?.isPositionInEmptyNeighbourhoodAtStart ?? false;
		}

		private void GenerateSlotInMatch(BoardRepresentation.RepresentationSlot slot)
		{
			UnityEngine.Debug.LogError("Can't generate slots that are free of matches");
			slot.itemColor = RandomColor();
			slot.isGenerated = true;
		}
	}
}
