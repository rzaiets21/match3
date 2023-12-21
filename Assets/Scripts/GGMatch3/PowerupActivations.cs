using System.Collections.Generic;

namespace GGMatch3
{
	public class PowerupActivations
	{
		public class PowerupActivation
		{
			public Slot powerupSlot;

			public ChipType powerupType;

			public Slot exchangeSlot;

			public bool isSwipe => !isTap;

			public bool isTap => exchangeSlot == null;

			public ActionScore GetActionScore(Match3Game game, Match3Goals goals)
			{
				ActionScore actionScore = default(ActionScore);
				IntVector2 position = powerupSlot.position;
				if (exchangeSlot != null)
				{
					position = exchangeSlot.position;
				}
				if (powerupType == ChipType.SeekingMissle)
				{
					actionScore.goalsCount++;
				}
				List<Slot> areaOfEffect = game.GetAreaOfEffect(powerupType, position);
				bool isHavingCarpet = false;
				for (int i = 0; i < areaOfEffect.Count; i++)
				{
					if (areaOfEffect[i].canCarpetSpreadFromHere)
					{
						isHavingCarpet = true;
						break;
					}
				}
				for (int j = 0; j < areaOfEffect.Count; j++)
				{
					Slot slot = areaOfEffect[j];
					actionScore = ((slot != exchangeSlot || slot == powerupSlot) ? (actionScore + goals.ActionScoreForDestroyingSlot(slot, game, isHavingCarpet, includeNeighbours: false)) : (actionScore + goals.ActionScoreForDestroyingSwitchingSlots(exchangeSlot, powerupSlot, game, isHavingCarpet, includeNeighbours: false)));
				}
				return actionScore;
			}

			public void InitWithTap(Slot slot, ChipType chipType)
			{
				powerupSlot = slot;
				exchangeSlot = null;
				powerupType = chipType;
			}

			public void InitWithTap(Slot slot)
			{
				powerupSlot = slot;
				exchangeSlot = null;
				powerupType = slot.GetSlotComponent<Chip>().chipType;
			}

			public void InitWithSwap(Slot powerupSlot, Slot exchangeSlot)
			{
				this.powerupSlot = powerupSlot;
				this.exchangeSlot = exchangeSlot;
				powerupType = powerupSlot.GetSlotComponent<Chip>().chipType;
			}

			public void InitWithSwap(Slot powerupSlot, Slot exchangeSlot, ChipType chipType)
			{
				this.powerupSlot = powerupSlot;
				this.exchangeSlot = exchangeSlot;
				powerupType = chipType;
			}
		}

		public List<PowerupActivation> powerups = new List<PowerupActivation>();

		private List<PowerupActivation> powerupsPool = new List<PowerupActivation>();

		private void ReturnToPool()
		{
			powerupsPool.AddRange(powerups);
			powerups.Clear();
		}

		private PowerupActivation GetPowerupActivation()
		{
			if (powerupsPool.Count == 0)
			{
				return new PowerupActivation();
			}
			int index = powerupsPool.Count - 1;
			PowerupActivation result = powerupsPool[index];
			powerupsPool.RemoveAt(index);
			return result;
		}

		private bool IsValidSpaceToSwapPowerup(Slot slot)
		{
			if (slot.isSlotGravitySuspended || slot.isSlotSwapSuspended || slot.isSlotMatchingSuspended)
			{
				return false;
			}
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			if (slotComponent != null && slotComponent.isPowerup)
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
			if (slot.isSlotGravitySuspended || slot.isSlotSwapSuspended || slot.isTapToActivateSuspended)
			{
				return false;
			}
			if (!slotComponent.isPowerup)
			{
				return false;
			}
			if (slotComponent.chipType == ChipType.DiscoBall)
			{
				return false;
			}
			return true;
		}

		public List<PowerupActivation> CreatePotentialActivations(ChipType chipType, Slot slot)
		{
			List<PowerupActivation> list = new List<PowerupActivation>();
			PowerupActivation powerupActivation = GetPowerupActivation();
			powerupActivation.InitWithTap(slot, chipType);
			list.Add(powerupActivation);
			List<Slot> neigbourSlots = slot.neigbourSlots;
			for (int i = 0; i < neigbourSlots.Count; i++)
			{
				Slot slot2 = neigbourSlots[i];
				if (IsValidSpaceToSwapPowerup(slot2))
				{
					PowerupActivation powerupActivation2 = GetPowerupActivation();
					powerupActivation2.InitWithSwap(slot, slot2, chipType);
					list.Add(powerupActivation2);
				}
			}
			return list;
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
				PowerupActivation powerupActivation = GetPowerupActivation();
				powerupActivation.InitWithTap(slot);
				powerups.Add(powerupActivation);
				List<Slot> neigbourSlots = slot.neigbourSlots;
				for (int j = 0; j < neigbourSlots.Count; j++)
				{
					Slot slot2 = neigbourSlots[j];
					if (IsValidSpaceToSwapPowerup(slot2) && !Slot.IsPathBlockedBetween(slot, slot2))
					{
						PowerupActivation powerupActivation2 = GetPowerupActivation();
						powerupActivation2.InitWithSwap(slot, slot2);
						powerups.Add(powerupActivation2);
					}
				}
			}
		}
	}
}
