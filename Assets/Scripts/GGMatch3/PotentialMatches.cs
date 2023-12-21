using System.Collections.Generic;

namespace GGMatch3
{
	public class PotentialMatches
	{
		public class CompoundSlotsSet
		{
			public enum MatchType
			{
				Match,
				DiscoBall,
				Bomb,
				Rocket,
				SeekingMissle,
				CompleatingGoals
			}

			public struct SlotsCount
			{
				public int count;

				public BoardRepresentation.RepresentationSlot slot;
			}

			public List<SlotsSet> slotsSets = new List<SlotsSet>();

			public BoardRepresentation.RepresentationSlot swipeSlot;

			private List<SlotsCount> slotsCount = new List<SlotsCount>();

			private List<BoardRepresentation.RepresentationSlot> slotsThatCanSwipe_ = new List<BoardRepresentation.RepresentationSlot>();

			public IntVector2 positionOfSlotMissingForMatch => slotsSets[0].positionOfSlotMissingForMatch;

			public ChipType createdPowerup
			{
				get
				{
					if (isDiscoBall)
					{
						return ChipType.DiscoBall;
					}
					if (isBomb)
					{
						return ChipType.Bomb;
					}
					if (isRocket)
					{
						if (CountOfType(SlotsSet.ConnectionType.Horizontal) >= 2)
						{
							return ChipType.HorizontalRocket;
						}
						return ChipType.VerticalRocket;
					}
					if (isSeekingMissle)
					{
						return ChipType.SeekingMissle;
					}
					return ChipType.RandomChip;
				}
			}

			public MatchType matchType
			{
				get
				{
					if (isDiscoBall)
					{
						return MatchType.DiscoBall;
					}
					if (isBomb)
					{
						return MatchType.Bomb;
					}
					if (isRocket)
					{
						return MatchType.Rocket;
					}
					if (isSeekingMissle)
					{
						return MatchType.SeekingMissle;
					}
					return MatchType.Match;
				}
			}

			private bool isDiscoBall
			{
				get
				{
					if (CountOfType(SlotsSet.ConnectionType.Horizontal) < 3)
					{
						return CountOfType(SlotsSet.ConnectionType.Vertical) >= 3;
					}
					return true;
				}
			}

			private bool isBomb
			{
				get
				{
					if (CountOfType(SlotsSet.ConnectionType.Horizontal) >= 1)
					{
						return CountOfType(SlotsSet.ConnectionType.Vertical) >= 1;
					}
					return false;
				}
			}

			private bool isRocket
			{
				get
				{
					if (CountOfType(SlotsSet.ConnectionType.Horizontal) < 2)
					{
						return CountOfType(SlotsSet.ConnectionType.Vertical) >= 2;
					}
					return true;
				}
			}

			private bool isSeekingMissle => CountOfType(SlotsSet.ConnectionType.Square) > 0;

			public bool HasCarpet(Match3Game game)
			{
				for (int i = 0; i < slotsSets.Count; i++)
				{
					if (slotsSets[i].HasCarpet(game))
					{
						return true;
					}
				}
				return false;
			}

			public ActionScore GetActionScore(Match3Game game, Match3Goals goals)
			{
				ActionScore actionScore = default(ActionScore);
				MatchType matchType = this.matchType;
				if (matchType == MatchType.DiscoBall || matchType == MatchType.Bomb || matchType == MatchType.Rocket || matchType == MatchType.SeekingMissle)
				{
					actionScore.powerupsCreated++;
				}
				bool isHavingCarpet = HasCarpet(game);
				for (int i = 0; i < slotsSets.Count; i++)
				{
					SlotsSet slotsSet = slotsSets[i];
					Slot slot = game.GetSlot(slotsSet.positionOfSlotMissingForMatch);
					Slot slot2 = game.GetSlot(swipeSlot.position);
					actionScore += goals.ActionScoreForDestroyingSwitchingSlots(slot, slot2, game, isHavingCarpet, includeNeighbours: true);
					List<BoardRepresentation.RepresentationSlot> sameColorSlots = slotsSet.sameColorSlots;
					for (int j = 0; j < sameColorSlots.Count; j++)
					{
						BoardRepresentation.RepresentationSlot representationSlot = sameColorSlots[j];
						Slot slot3 = game.GetSlot(representationSlot.position);
						actionScore += goals.ActionScoreForDestroyingSlot(slot3, game, isHavingCarpet, includeNeighbours: true);
					}
				}
				return actionScore;
			}

			public void CopyFrom(CompoundSlotsSet c)
			{
				slotsSets.Clear();
				slotsSets.AddRange(c.slotsSets);
				swipeSlot = c.swipeSlot;
			}

			public int CountOfType(SlotsSet.ConnectionType connectionType)
			{
				int num = 0;
				for (int i = 0; i < slotsSets.Count; i++)
				{
					if (slotsSets[i].connectionType == connectionType)
					{
						num++;
					}
				}
				return num;
			}

			public bool IsAcceptable(SlotsSet otherSlotSet, BoardRepresentation.RepresentationSlot slotToSwipe)
			{
				if (slotsSets.Count == 0)
				{
					return true;
				}
				if (otherSlotSet.connectionType == SlotsSet.ConnectionType.Square)
				{
					return false;
				}
				SlotsSet slotsSet = slotsSets[0];
				if (slotsSet.itemColor != otherSlotSet.itemColor)
				{
					return false;
				}
				if (slotsSet.positionOfSlotMissingForMatch != otherSlotSet.positionOfSlotMissingForMatch)
				{
					return false;
				}
				if (!otherSlotSet.SwipeSlotsContains(slotToSwipe))
				{
					return false;
				}
				return true;
			}

			public void Add(SlotsSet slotsSet)
			{
				slotsSets.Add(slotsSet);
			}

			public void Clear()
			{
				slotsSets.Clear();
			}
		}

		public class SlotsSet
		{
			public struct ColorCount
			{
				public ItemColor color;

				public int count;
			}

			public enum ConnectionType
			{
				Vertical,
				Horizontal,
				Square
			}

			public List<BoardRepresentation.RepresentationSlot> sameColorSlots = new List<BoardRepresentation.RepresentationSlot>();

			public List<BoardRepresentation.RepresentationSlot> differentColorSlots = new List<BoardRepresentation.RepresentationSlot>();

			public List<BoardRepresentation.RepresentationSlot> slotsThatCanSwipeToMatch = new List<BoardRepresentation.RepresentationSlot>();

			public List<BoardRepresentation.RepresentationSlot> allSlots = new List<BoardRepresentation.RepresentationSlot>();

			public List<ColorCount> colorCount = new List<ColorCount>();

			public ConnectionType connectionType;

			public ItemColor itemColor
			{
				get
				{
					if (sameColorSlots.Count == 0)
					{
						return ItemColor.Unknown;
					}
					return sameColorSlots[0].itemColor;
				}
			}

			public IntVector2 positionOfSlotMissingForMatch => differentColorSlots[0].position;

			public bool isMatch
			{
				get
				{
					if (sameColorSlots.Count >= 3)
					{
						return differentColorSlots.Count == 0;
					}
					return false;
				}
			}

			public bool isPotentialMatch
			{
				get
				{
					if (sameColorSlots.Count < 2)
					{
						return false;
					}
					int num = 1;
					if (differentColorSlots.Count != num)
					{
						return false;
					}
					return differentColorSlots[0].canMove;
				}
			}

			public bool isMatchWhenSwipe => slotsThatCanSwipeToMatch.Count > 0;

			public ColorCount DominantColorCount
			{
				get
				{
					this.colorCount.Clear();
					for (int i = 0; i < allSlots.Count; i++)
					{
						BoardRepresentation.RepresentationSlot representationSlot = allSlots[i];
						IncrementColorCount(representationSlot.itemColor);
					}
					ColorCount colorCount = default(ColorCount);
					for (int j = 0; j < this.colorCount.Count; j++)
					{
						ColorCount colorCount2 = this.colorCount[j];
						if (colorCount2.count > colorCount.count)
						{
							colorCount = colorCount2;
						}
					}
					return colorCount;
				}
			}

			public bool HasCarpet(Match3Game game)
			{
				for (int i = 0; i < allSlots.Count; i++)
				{
					BoardRepresentation.RepresentationSlot representationSlot = allSlots[i];
					Slot slot = game.GetSlot(representationSlot.position);
					if (slot == null)
					{
						continue;
					}
					bool flag = false;
					for (int j = 0; j < slotsThatCanSwipeToMatch.Count; j++)
					{
						if (slotsThatCanSwipeToMatch[j].position == representationSlot.position)
						{
							flag = true;
							break;
						}
					}
					if (!flag && slot.canCarpetSpreadFromHere)
					{
						return true;
					}
				}
				return false;
			}

			public void CopyFrom(SlotsSet s)
			{
				sameColorSlots.Clear();
				differentColorSlots.Clear();
				slotsThatCanSwipeToMatch.Clear();
				allSlots.Clear();
				colorCount.Clear();
				sameColorSlots.AddRange(s.sameColorSlots);
				differentColorSlots.AddRange(s.differentColorSlots);
				slotsThatCanSwipeToMatch.AddRange(s.slotsThatCanSwipeToMatch);
				connectionType = s.connectionType;
				allSlots.AddRange(s.allSlots);
				colorCount.AddRange(s.colorCount);
			}

			public void AddToAllSlots(BoardRepresentation.RepresentationSlot slot)
			{
				if (!slot.canFormColorMatches)
				{
					differentColorSlots.Add(slot);
				}
				else
				{
					allSlots.Add(slot);
				}
			}

			public void SortSlotsUsingDominantColor()
			{
				ColorCount dominantColorCount = DominantColorCount;
				for (int i = 0; i < allSlots.Count; i++)
				{
					BoardRepresentation.RepresentationSlot representationSlot = allSlots[i];
					if (representationSlot.itemColor == dominantColorCount.color)
					{
						sameColorSlots.Add(representationSlot);
					}
					else
					{
						differentColorSlots.Add(representationSlot);
					}
				}
			}

			private void IncrementColorCount(ItemColor color)
			{
				for (int i = 0; i < this.colorCount.Count; i++)
				{
					ColorCount colorCount = this.colorCount[i];
					if (colorCount.color == color)
					{
						colorCount.count++;
						this.colorCount[i] = colorCount;
						break;
					}
				}
				ColorCount item = default(ColorCount);
				item.count = 1;
				item.color = color;
				this.colorCount.Add(item);
			}

			public void AddSlot(BoardRepresentation.RepresentationSlot slot)
			{
				if (sameColorSlots.Count == 0)
				{
					sameColorSlots.Add(slot);
					return;
				}
				BoardRepresentation.RepresentationSlot representationSlot = sameColorSlots[0];
				if (slot.canFormColorMatches && representationSlot.itemColor == slot.itemColor)
				{
					sameColorSlots.Add(slot);
				}
				else
				{
					differentColorSlots.Add(slot);
				}
			}

			public void Clear(ConnectionType connectionType)
			{
				this.connectionType = connectionType;
				sameColorSlots.Clear();
				differentColorSlots.Clear();
				slotsThatCanSwipeToMatch.Clear();
				allSlots.Clear();
				colorCount.Clear();
			}

			public bool SwipeSlotsContains(BoardRepresentation.RepresentationSlot slot)
			{
				for (int i = 0; i < slotsThatCanSwipeToMatch.Count; i++)
				{
					if (slotsThatCanSwipeToMatch[i].position == slot.position)
					{
						return true;
					}
				}
				return false;
			}

			public bool MatchingSlotsContains(BoardRepresentation.RepresentationSlot slot)
			{
				for (int i = 0; i < sameColorSlots.Count; i++)
				{
					if (sameColorSlots[i].position == slot.position)
					{
						return true;
					}
				}
				return false;
			}

			private void TryAddSlotThatCanSwipeToMatch(PotentialMatches potentialMatches, BoardRepresentation.RepresentationSlot fromSlot, BoardRepresentation.RepresentationSlot slot, ItemColor desiredItemColor)
			{
				if (slot.canFormColorMatches && slot.canMove && fromSlot.canMove && !fromSlot.IsBlockedTo(slot) && slot.itemColor == desiredItemColor && !MatchingSlotsContains(slot) && !potentialMatches.IsPartOfMatch(slot))
				{
					slotsThatCanSwipeToMatch.Add(slot);
				}
			}

			public void FillSlotsThatCanSwipeToMatch(PotentialMatches potentialMatches, BoardRepresentation board)
			{
				BoardRepresentation.RepresentationSlot representationSlot = sameColorSlots[0];
				BoardRepresentation.RepresentationSlot representationSlot2 = differentColorSlots[0];
				ItemColor itemColor = representationSlot.itemColor;
				TryAddSlotThatCanSwipeToMatch(potentialMatches, representationSlot2, board.GetSlot(representationSlot2.position + IntVector2.right), itemColor);
				TryAddSlotThatCanSwipeToMatch(potentialMatches, representationSlot2, board.GetSlot(representationSlot2.position + IntVector2.left), itemColor);
				TryAddSlotThatCanSwipeToMatch(potentialMatches, representationSlot2, board.GetSlot(representationSlot2.position + IntVector2.up), itemColor);
				TryAddSlotThatCanSwipeToMatch(potentialMatches, representationSlot2, board.GetSlot(representationSlot2.position + IntVector2.down), itemColor);
			}
		}

		public List<SlotsSet> slotSetPool = new List<SlotsSet>();

		public List<CompoundSlotsSet> compoundPool = new List<CompoundSlotsSet>();

		private List<CompoundSlotsSet> filteredList = new List<CompoundSlotsSet>();

		public List<CompoundSlotsSet> matchesList = new List<CompoundSlotsSet>();

		public BoardRepresentation board = new BoardRepresentation();

		public List<SlotsSet> setsThatCanFormMatches = new List<SlotsSet>();

		public List<SlotsSet> setsThatAreInMatch = new List<SlotsSet>();

		private SlotsSet searchingSlotSet = new SlotsSet();

		private CompoundSlotsSet searchingCompoundSlotsSet = new CompoundSlotsSet();

		public int MatchesCount => matchesList.Count;

		public List<CompoundSlotsSet> FilterForTypeCompleatingGoals(Match3Game game)
		{
			filteredList.Clear();
			Match3Goals goal = game.goals;
			for (int i = 0; i < matchesList.Count; i++)
			{
				CompoundSlotsSet compoundSlotsSet = matchesList[i];
				if (compoundSlotsSet.GetActionScore(game, game.goals).goalsCount > 0)
				{
					filteredList.Add(compoundSlotsSet);
				}
			}
			return filteredList;
		}

		public List<CompoundSlotsSet> FilterForType(CompoundSlotsSet.MatchType matchType)
		{
			filteredList.Clear();
			for (int i = 0; i < matchesList.Count; i++)
			{
				CompoundSlotsSet compoundSlotsSet = matchesList[i];
				if (compoundSlotsSet.matchType == matchType)
				{
					filteredList.Add(compoundSlotsSet);
				}
			}
			return filteredList;
		}

		private void Clear()
		{
			slotSetPool.AddRange(setsThatCanFormMatches);
			slotSetPool.AddRange(setsThatAreInMatch);
			setsThatCanFormMatches.Clear();
			setsThatAreInMatch.Clear();
			compoundPool.AddRange(matchesList);
			matchesList.Clear();
		}

		private CompoundSlotsSet NextCompound()
		{
			CompoundSlotsSet compoundSlotsSet = null;
			if (compoundPool.Count > 0)
			{
				int index = compoundPool.Count - 1;
				compoundSlotsSet = compoundPool[index];
				compoundPool.RemoveAt(index);
			}
			if (compoundSlotsSet == null)
			{
				compoundSlotsSet = new CompoundSlotsSet();
			}
			compoundSlotsSet.Clear();
			return compoundSlotsSet;
		}

		private SlotsSet NextSlotsSet()
		{
			SlotsSet slotsSet = null;
			if (slotSetPool.Count > 0)
			{
				int index = slotSetPool.Count - 1;
				slotsSet = slotSetPool[index];
				slotSetPool.RemoveAt(index);
				return slotsSet;
			}
			if (slotsSet == null)
			{
				slotsSet = new SlotsSet();
			}
			return slotsSet;
		}

		private void AddMatch(CompoundSlotsSet c)
		{
			CompoundSlotsSet compoundSlotsSet = NextCompound();
			compoundSlotsSet.CopyFrom(c);
			matchesList.Add(compoundSlotsSet);
		}

		private bool IsPartOfMatchList(IntVector2 positionOfSlotMissingForMatch, IntVector2 positionToSwipeFrom)
		{
			for (int i = 0; i < matchesList.Count; i++)
			{
				CompoundSlotsSet compoundSlotsSet = matchesList[i];
				if (!(compoundSlotsSet.swipeSlot.position != positionToSwipeFrom) && !(compoundSlotsSet.positionOfSlotMissingForMatch != positionOfSlotMissingForMatch))
				{
					return true;
				}
			}
			return false;
		}

		private void AddSetThatCanFormMatches(SlotsSet slotsSet)
		{
			SlotsSet slotsSet2 = NextSlotsSet();
			slotsSet2.CopyFrom(slotsSet);
			setsThatCanFormMatches.Add(slotsSet2);
		}

		private void AddSetThatIsInMatch(SlotsSet slotsSet)
		{
			SlotsSet slotsSet2 = NextSlotsSet();
			slotsSet2.CopyFrom(slotsSet);
			setsThatAreInMatch.Add(slotsSet2);
		}

		public bool IsPartOfMatch(BoardRepresentation.RepresentationSlot slot)
		{
			for (int i = 0; i < setsThatAreInMatch.Count; i++)
			{
				if (setsThatAreInMatch[i].MatchingSlotsContains(slot))
				{
					return true;
				}
			}
			return false;
		}

		private SlotsSet FillLineSet(BoardRepresentation board, IntVector2 pos, IntVector2 direction, SlotsSet.ConnectionType connectionType)
		{
			SlotsSet slotsSet = searchingSlotSet;
			slotsSet.Clear(connectionType);
			for (int i = 0; i < 3; i++)
			{
				BoardRepresentation.RepresentationSlot slot = board.GetSlot(pos + i * direction);
				if (IsPartOfMatch(slot))
				{
					slotsSet.Clear(connectionType);
					return slotsSet;
				}
				slotsSet.AddToAllSlots(slot);
			}
			slotsSet.SortSlotsUsingDominantColor();
			if (slotsSet.isMatch)
			{
				int num = MatchLength(board, pos, direction);
				slotsSet.Clear(connectionType);
				for (int j = 0; j < num; j++)
				{
					IntVector2 pos2 = pos + direction * j;
					slotsSet.AddSlot(board.GetSlot(pos2));
				}
				return slotsSet;
			}
			if (!slotsSet.isPotentialMatch)
			{
				return slotsSet;
			}
			slotsSet.FillSlotsThatCanSwipeToMatch(this, board);
			return slotsSet;
		}

		private SlotsSet FillSquareSet(BoardRepresentation board, IntVector2 pos)
		{
			SlotsSet.ConnectionType connectionType = SlotsSet.ConnectionType.Square;
			SlotsSet slotsSet = searchingSlotSet;
			slotsSet.Clear(connectionType);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					BoardRepresentation.RepresentationSlot slot = board.GetSlot(pos + new IntVector2(i, j));
					if (IsPartOfMatch(slot))
					{
						slotsSet.Clear(connectionType);
						return slotsSet;
					}
					slotsSet.AddToAllSlots(slot);
				}
			}
			slotsSet.SortSlotsUsingDominantColor();
			if (slotsSet.isMatch)
			{
				return slotsSet;
			}
			if (!slotsSet.isPotentialMatch)
			{
				return slotsSet;
			}
			slotsSet.FillSlotsThatCanSwipeToMatch(this, board);
			return slotsSet;
		}

		public void FindPotentialMatches(Match3Game match3Game)
		{
			Clear();
			board.Init(match3Game);
			IntVector2 size = board.size;
			SlotsSet searchingSlotSet2 = searchingSlotSet;
			for (int i = 0; i < size.y; i++)
			{
				for (int j = 0; j < size.x; j++)
				{
					IntVector2 pos = new IntVector2(j, i);
					IntVector2 right = IntVector2.right;
					SlotsSet slotsSet = FillLineSet(board, pos, right, SlotsSet.ConnectionType.Horizontal);
					if (slotsSet.isMatch)
					{
						AddSetThatIsInMatch(slotsSet);
					}
					else if (slotsSet.isMatchWhenSwipe)
					{
						AddSetThatCanFormMatches(slotsSet);
					}
				}
			}
			for (int k = 0; k < size.x; k++)
			{
				for (int l = 0; l < size.y; l++)
				{
					IntVector2 pos2 = new IntVector2(k, l);
					IntVector2 up = IntVector2.up;
					SlotsSet slotsSet2 = FillLineSet(board, pos2, up, SlotsSet.ConnectionType.Vertical);
					if (slotsSet2.isMatch)
					{
						AddSetThatIsInMatch(slotsSet2);
					}
					else if (slotsSet2.isMatchWhenSwipe)
					{
						AddSetThatCanFormMatches(slotsSet2);
					}
				}
			}
			for (int m = 0; m < size.x; m++)
			{
				for (int n = 0; n < size.y; n++)
				{
					SlotsSet slotsSet3 = FillSquareSet(pos: new IntVector2(m, n), board: board);
					if (slotsSet3.isMatch)
					{
						AddSetThatIsInMatch(slotsSet3);
					}
					else if (slotsSet3.isMatchWhenSwipe)
					{
						AddSetThatCanFormMatches(slotsSet3);
					}
				}
			}
			CompoundSlotsSet compoundSlotsSet = searchingCompoundSlotsSet;
			for (int num = 0; num < setsThatCanFormMatches.Count; num++)
			{
				SlotsSet slotsSet4 = setsThatCanFormMatches[num];
				for (int num2 = 0; num2 < slotsSet4.slotsThatCanSwipeToMatch.Count; num2++)
				{
					BoardRepresentation.RepresentationSlot representationSlot = slotsSet4.slotsThatCanSwipeToMatch[num2];
					if (IsPartOfMatchList(slotsSet4.positionOfSlotMissingForMatch, representationSlot.position))
					{
						continue;
					}
					compoundSlotsSet.Clear();
					compoundSlotsSet.swipeSlot = representationSlot;
					compoundSlotsSet.Add(slotsSet4);
					for (int num3 = num + 1; num3 < setsThatCanFormMatches.Count; num3++)
					{
						SlotsSet slotsSet5 = setsThatCanFormMatches[num3];
						if (slotsSet4 != slotsSet5 && compoundSlotsSet.IsAcceptable(slotsSet5, representationSlot))
						{
							compoundSlotsSet.Add(slotsSet5);
						}
					}
					AddMatch(compoundSlotsSet);
				}
			}
		}

		private int MatchLength(BoardRepresentation board, IntVector2 pos, IntVector2 direction)
		{
			int num = 1;
			BoardRepresentation.RepresentationSlot slot = board.GetSlot(pos);
			if (!slot.canFormColorMatches)
			{
				return num;
			}
			ItemColor itemColor = slot.itemColor;
			while (true)
			{
				pos += direction;
				BoardRepresentation.RepresentationSlot slot2 = board.GetSlot(pos);
				if (slot2.isOutsideBoard || !slot2.canFormColorMatches || slot2.itemColor != itemColor)
				{
					break;
				}
				num++;
			}
			return num;
		}
	}
}
