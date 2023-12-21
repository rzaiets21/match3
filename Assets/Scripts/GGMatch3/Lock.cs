using System.Collections.Generic;

namespace GGMatch3
{
	public class Lock
	{
		public bool isSlotGravitySuspended;

		public bool isChipGravitySuspended;

		public bool isChipGeneratorSuspended;

		public bool isSlotMatchingSuspended;

		public bool isSlotTouchingSuspended;

		public bool isSlotSwipeSuspended;

		public bool isDestroySuspended;

		public bool isAvailableForDiscoBombSuspended;

		public bool isAvailableForSeekingMissileSuspended;

		public bool isAboutToBeDestroyed;

		public bool isPowerupReplacementSuspended;

		public bool isAttachGrowingElementSuspended;

		private List<Slot> connectedSlots = new List<Slot>();

		private List<Slot> temporaryList;

		public void SuspendAll()
		{
			isSlotGravitySuspended = true;
			isChipGravitySuspended = true;
			isChipGeneratorSuspended = true;
			isSlotMatchingSuspended = true;
			isSlotTouchingSuspended = true;
			isSlotSwipeSuspended = true;
			isDestroySuspended = true;
			isAvailableForDiscoBombSuspended = true;
			isAvailableForSeekingMissileSuspended = true;
			isPowerupReplacementSuspended = true;
			isAttachGrowingElementSuspended = true;
		}

		public void LockSlots(List<Slot> slots)
		{
			if (slots != null)
			{
				for (int i = 0; i < slots.Count; i++)
				{
					Slot slot = slots[i];
					LockSlot(slot);
				}
			}
		}

		public void LockSlot(Slot slot)
		{
			if (slot != null)
			{
				slot.AddLock(this);
				if (!connectedSlots.Contains(slot))
				{
					connectedSlots.Add(slot);
				}
			}
		}

		public void UnlockAllAndSaveToTemporaryList()
		{
			SaveToTemporaryList();
			UnlockAll();
		}

		public void SaveToTemporaryList()
		{
			if (temporaryList == null)
			{
				temporaryList = new List<Slot>();
			}
			temporaryList.Clear();
			temporaryList.AddRange(connectedSlots);
		}

		public void LockTemporaryListAndClear()
		{
			LockSlots(temporaryList);
			temporaryList.Clear();
		}

		public void Unlock(Slot slot)
		{
			if (slot != null)
			{
				connectedSlots.Remove(slot);
				slot.RemoveLock(this);
			}
		}

		public void UnlockAll()
		{
			for (int i = 0; i < connectedSlots.Count; i++)
			{
				connectedSlots[i]?.RemoveLock(this);
			}
			connectedSlots.Clear();
		}
	}
}
