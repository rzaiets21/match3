using System.Collections.Generic;

namespace GGMatch3
{
	public class PowerupCombines
	{
		public enum CombineType
		{
			Unknown,
			DoubleSeekingMissle,
			DoubleRocket,
			DoubleBomb,
			DoubleDiscoBall,
			DiscoBallColor,
			DiscoBallSeekingMissle,
			DiscoBallRocket,
			DiscoBallBomb,
			RocketSeekingMissle,
			RocketBomb,
			BombSeekingMissle
		}

		public class PowerupCombine
		{
			public Slot powerupSlot;

			public Slot exchangeSlot;

			private List<Slot> affectedSlotsList = new List<Slot>();

			public CombineType combineType
			{
				get
				{
					if (Count(ChipType.SeekingMissle) == 2)
					{
						return CombineType.DoubleSeekingMissle;
					}
					if (Count(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 2)
					{
						return CombineType.DoubleRocket;
					}
					if (Count(ChipType.Bomb) == 2)
					{
						return CombineType.DoubleBomb;
					}
					if (Count(ChipType.DiscoBall) == 2)
					{
						return CombineType.DoubleDiscoBall;
					}
					if (Count(ChipType.DiscoBall) == 1 && Count(ChipType.Chip) == 1)
					{
						return CombineType.DiscoBallColor;
					}
					if (Count(ChipType.DiscoBall) == 1 && Count(ChipType.SeekingMissle) == 1)
					{
						return CombineType.DiscoBallSeekingMissle;
					}
					if (Count(ChipType.DiscoBall) == 1 && Count(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 1)
					{
						return CombineType.DiscoBallRocket;
					}
					if (Count(ChipType.DiscoBall) == 1 && Count(ChipType.Bomb) == 1)
					{
						return CombineType.DiscoBallBomb;
					}
					if (Count(ChipType.SeekingMissle) == 1 && Count(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 1)
					{
						return CombineType.RocketSeekingMissle;
					}
					if (Count(ChipType.Bomb) == 1 && Count(ChipType.HorizontalRocket, ChipType.VerticalRocket) == 1)
					{
						return CombineType.RocketBomb;
					}
					if (Count(ChipType.SeekingMissle) == 1 && Count(ChipType.SeekingMissle) == 1)
					{
						return CombineType.BombSeekingMissle;
					}
					return CombineType.Unknown;
				}
			}

			public ActionScore GetActionScore(Match3Game game, Match3Goals goals)
			{
				ActionScore actionScore = default(ActionScore);
				Slot slot = exchangeSlot;
				IntVector2 position = slot.position;
				affectedSlotsList.Clear();
				List<Slot> list = affectedSlotsList;
				if (combineType == CombineType.DoubleSeekingMissle)
				{
					actionScore.goalsCount += 3;
					list = game.GetAreaOfEffect(ChipType.SeekingMissle, position);
				}
				else if (combineType == CombineType.DoubleRocket)
				{
					list = game.GetCross(position, 0);
				}
				else if (combineType == CombineType.DoubleBomb)
				{
					list = game.GetArea(position, 3);
				}
				else if (combineType == CombineType.DoubleDiscoBall)
				{
					list = game.GetAllPlayingSlots();
				}
				else if (combineType == CombineType.DiscoBallColor)
				{
					Chip slotComponent = exchangeSlot.GetSlotComponent<Chip>();
					if (slotComponent.canFormColorMatches)
					{
						ItemColor itemColor = slotComponent.itemColor;
						list = game.SlotsInDiscoBallDestroy(itemColor, replaceWithBombs: false);
					}
				}
				else if (combineType == CombineType.DiscoBallSeekingMissle || combineType == CombineType.DiscoBallRocket || combineType == CombineType.DiscoBallBomb)
				{
					ItemColor itemColor2 = game.BestItemColorForDiscoBomb(replaceWithBombs: true);
					List<Slot> list2 = game.SlotsInDiscoBallDestroy(itemColor2, replaceWithBombs: true);
					ChipType chipType = ChipType.SeekingMissle;
					if (combineType == CombineType.DiscoBallSeekingMissle)
					{
						chipType = ChipType.SeekingMissle;
						actionScore.goalsCount += list2.Count;
					}
					else if (combineType == CombineType.DiscoBallBomb)
					{
						chipType = ChipType.Bomb;
					}
					affectedSlotsList.Clear();
					for (int i = 0; i < list2.Count; i++)
					{
						Slot slot2 = list2[i];
						if (combineType == CombineType.DiscoBallRocket)
						{
							chipType = ((i % 2 == 0) ? ChipType.HorizontalRocket : ChipType.VerticalRocket);
						}
						List<Slot> areaOfEffect = game.GetAreaOfEffect(chipType, slot2.position);
						affectedSlotsList.AddRange(areaOfEffect);
					}
					list = affectedSlotsList;
				}
				else if (combineType == CombineType.RocketBomb)
				{
					list = game.GetCross(position, 1);
				}
				else if (combineType == CombineType.RocketSeekingMissle || combineType == CombineType.BombSeekingMissle)
				{
					affectedSlotsList.Clear();
					List<Slot> areaOfEffect2 = game.GetAreaOfEffect(ChipType.SeekingMissle, position);
					affectedSlotsList.AddRange(areaOfEffect2);
					ChipType chipType2 = ChipType.Bomb;
					chipType2 = ((Count(ChipType.HorizontalRocket) > 0) ? ChipType.HorizontalRocket : ((Count(ChipType.VerticalRocket) <= 0) ? ChipType.Bomb : ChipType.VerticalRocket));
					List<Slot> list3 = game.goals.BestSlotsForSeekingMissle(game, slot);
					if (list3 != null && list3.Count > 0)
					{
						int index = 0;
						Slot slot3 = list3[index];
						areaOfEffect2 = game.GetAreaOfEffect(chipType2, slot3.position);
						affectedSlotsList.AddRange(areaOfEffect2);
					}
					list = affectedSlotsList;
				}
				bool includeNeighbours = combineType == CombineType.DiscoBallColor;
				bool isHavingCarpet = false;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].canCarpetSpreadFromHere)
					{
						isHavingCarpet = true;
						break;
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					Slot slot4 = list[k];
					actionScore += goals.ActionScoreForDestroyingSlot(slot4, game, isHavingCarpet, includeNeighbours);
				}
				return actionScore;
			}

			private bool IsChipType(Slot slot, ChipType chipType)
			{
				Chip slotComponent = slot.GetSlotComponent<Chip>();
				if (slotComponent == null)
				{
					return false;
				}
				return slotComponent.chipType == chipType;
			}

			public int Count(ChipType chipType)
			{
				int num = 0;
				if (IsChipType(powerupSlot, chipType))
				{
					num++;
				}
				if (IsChipType(exchangeSlot, chipType))
				{
					num++;
				}
				return num;
			}

			private bool IsChipType(Slot slot, ChipType chipType, ChipType chipType2)
			{
				Chip slotComponent = slot.GetSlotComponent<Chip>();
				if (slotComponent == null)
				{
					return false;
				}
				if (slotComponent.chipType != chipType)
				{
					return slotComponent.chipType == chipType2;
				}
				return true;
			}

			public int Count(ChipType chipType, ChipType chipType2)
			{
				int num = 0;
				if (IsChipType(powerupSlot, chipType, chipType2))
				{
					num++;
				}
				if (IsChipType(exchangeSlot, chipType, chipType2))
				{
					num++;
				}
				return num;
			}
		}

		public List<PowerupCombine> combines = new List<PowerupCombine>();

		private List<PowerupCombine> combinesPool = new List<PowerupCombine>();

		private List<PowerupCombine> filteredCombines = new List<PowerupCombine>();

		private void ReturnToPool()
		{
			combinesPool.AddRange(combines);
			combines.Clear();
		}

		private PowerupCombine GetCombineFromPool()
		{
			if (combinesPool.Count == 0)
			{
				return new PowerupCombine();
			}
			int index = combinesPool.Count - 1;
			PowerupCombine result = combinesPool[index];
			combinesPool.RemoveAt(index);
			return result;
		}

		private bool IsValidPowerupInSlot(ChipType chipType, Slot slot)
		{
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			if (slotComponent == null)
			{
				return false;
			}
			if (slot.isSlotGravitySuspended || slot.isSlotSwapSuspended)
			{
				return false;
			}
			if (chipType == ChipType.DiscoBall)
			{
				if (slot.isLockedForDiscoBomb)
				{
					return false;
				}
				if (!slotComponent.isPowerup && !slotComponent.canFormColorMatches)
				{
					return false;
				}
			}
			else if (!slotComponent.isPowerup)
			{
				return false;
			}
			return true;
		}

		private bool IsValidPowerupInSlot(Slot slot)
		{
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			if (slotComponent == null)
			{
				return false;
			}
			if (!slotComponent.isPowerup)
			{
				return false;
			}
			if (slot.isSlotGravitySuspended || slot.isSlotSwapSuspended)
			{
				return false;
			}
			return true;
		}

		public List<PowerupCombine> FilterCombines(CombineType combineType)
		{
			filteredCombines.Clear();
			for (int i = 0; i < combines.Count; i++)
			{
				PowerupCombine powerupCombine = combines[i];
				if (powerupCombine.combineType == combineType)
				{
					filteredCombines.Add(powerupCombine);
				}
			}
			return filteredCombines;
		}

		public void Fill(Match3Game game)
		{
			ReturnToPool();
			Slot[] slots = game.board.slots;
			foreach (Slot slot in slots)
			{
				if (slot == null || !IsValidPowerupInSlot(slot))
				{
					continue;
				}
				ChipType chipType = slot.GetSlotComponent<Chip>().chipType;
				List<Slot> neigbourSlots = slot.neigbourSlots;
				for (int j = 0; j < neigbourSlots.Count; j++)
				{
					Slot slot2 = neigbourSlots[j];
					if (IsValidPowerupInSlot(chipType, slot2) && !Slot.IsPathBlockedBetween(slot, slot2))
					{
						PowerupCombine combineFromPool = GetCombineFromPool();
						combineFromPool.powerupSlot = slot;
						combineFromPool.exchangeSlot = slot2;
						combines.Add(combineFromPool);
						if (chipType != ChipType.DiscoBall)
						{
							PowerupCombine combineFromPool2 = GetCombineFromPool();
							combineFromPool2.powerupSlot = slot2;
							combineFromPool2.exchangeSlot = slot;
							combines.Add(combineFromPool2);
						}
					}
				}
			}
		}
	}
}
