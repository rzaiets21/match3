using System.Collections.Generic;

namespace GGMatch3
{
	public class SlotDestroyParams
	{
		public bool isFromTap;

		public bool isFromSwap;

		public bool isHitByBomb;

		public ChipType bombType;

		public float activationDelay;

		public Island matchIsland;

		public bool isBombAllowingNeighbourDestroy;

		public bool isHavingCarpet;

		public bool isCreatingPowerupFromThisMatch;

		public bool isNeigbourDestroySuspended;

		public bool isExplosion;

		public int goalsCollected;

		public IntVector2 explosionCentre;

		public bool isRocketStopped;

		public int chipBlockersDestroyed;

		public int chipsDestroyed;

		public int scoreAdded;

		public SwapParams swapParams;

		public List<Chip> chipsAvailableForPowerupCreateAnimation;

		public List<Slot> slotsWithSuspendCheckNeighbor;

		public bool isFromSwapOrTap
		{
			get
			{
				if (!isFromTap)
				{
					return isFromSwap;
				}
				return true;
			}
		}

		public void StartSlot(Slot slot)
		{
			chipBlockersDestroyed = 0;
			chipsDestroyed = 0;
			scoreAdded = 0;
		}

		public void EndSlot(Slot slot)
		{
			chipBlockersDestroyed = 0;
			chipsDestroyed = 0;
			scoreAdded = 0;
		}

		public void AddChipForPowerupCreateAnimation(Chip chip)
		{
			if (chipsAvailableForPowerupCreateAnimation == null)
			{
				chipsAvailableForPowerupCreateAnimation = new List<Chip>();
			}
			chipsAvailableForPowerupCreateAnimation.Add(chip);
		}

		public void AddSlotForSuspendedNeighbor(Slot slot)
		{
			if (slotsWithSuspendCheckNeighbor == null)
			{
				slotsWithSuspendCheckNeighbor = new List<Slot>();
			}
			if (!slotsWithSuspendCheckNeighbor.Contains(slot))
			{
				slotsWithSuspendCheckNeighbor.Add(slot);
			}
		}

		public bool IsNeigborDestraySuspended(Slot slot)
		{
			if (slotsWithSuspendCheckNeighbor == null)
			{
				return false;
			}
			return slotsWithSuspendCheckNeighbor.Contains(slot);
		}
	}
}
