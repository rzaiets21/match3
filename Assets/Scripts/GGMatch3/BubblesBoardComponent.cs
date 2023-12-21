using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class BubblesBoardComponent : BoardComponent
	{
		private Match3Game game;

		private bool isAnyBubbleBurst;

		private int movesCountWhenTookAction;

		private LockContainer lockContainer = new LockContainer();

		private Lock globalLock;

		private List<Slot> allSlotsList = new List<Slot>();

		private List<Slot> slotsWhereCanPlaceNewBubble = new List<Slot>();

		private bool _isCleared;

		private bool isCleared
		{
			get
			{
				if (_isCleared)
				{
					return true;
				}
				List<Slot> allSlotsContainingBubbles = this.allSlotsContainingBubbles;
				_isCleared = (allSlotsContainingBubbles.Count == 0);
				return _isCleared;
			}
		}

		private List<Slot> allSlotsContainingBubbles
		{
			get
			{
				allSlotsList.Clear();
				List<Slot> sortedSlotsUpdateList = game.board.sortedSlotsUpdateList;
				for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
				{
					Slot slot = sortedSlotsUpdateList[i];
					if (slot.GetSlotComponent<BubblesPieceBlocker>() != null)
					{
						allSlotsList.Add(slot);
					}
				}
				return allSlotsList;
			}
		}

		public bool isWaitingForBubblesToBurst
		{
			get
			{
				if (isCleared)
				{
					return false;
				}
				if (game.board.userMovesCount <= movesCountWhenTookAction)
				{
					return false;
				}
				if (isAnyBubbleBurst)
				{
					return false;
				}
				return true;
			}
		}

		public void OnBubbleBurst(BubblesPieceBlocker bubble)
		{
			isAnyBubbleBurst = true;
		}

		public void Init(Match3Game game)
		{
			this.game = game;
			globalLock = lockContainer.NewLock();
			globalLock.isAvailableForDiscoBombSuspended = true;
			globalLock.isSlotMatchingSuspended = true;
			globalLock.isChipGeneratorSuspended = true;
			globalLock.isDestroySuspended = true;
			globalLock.isSlotGravitySuspended = true;
		}

		public void CancelSpread()
		{
			movesCountWhenTookAction = game.board.userMovesCount;
		}

		public void OnUserMadeMove()
		{
			if (isCleared)
			{
				CancelSpread();
				return;
			}
			List<Slot> allSlotsContainingBubbles = this.allSlotsContainingBubbles;
			if (allSlotsContainingBubbles.Count == 0)
			{
				movesCountWhenTookAction = game.board.userMovesCount;
				return;
			}
			bool flag = false;
			for (int i = 0; i < allSlotsContainingBubbles.Count; i++)
			{
				List<Slot> neigbourSlots = allSlotsContainingBubbles[i].neigbourSlots;
				for (int j = 0; j < neigbourSlots.Count; j++)
				{
					Slot slot = neigbourSlots[j];
					if (!slot.isSlotGravitySuspended && !slot.isSlotSwapSuspended && !slot.isDestroySuspended && !slot.isPlaceBubbleSuspended)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				CancelSpread();
				isAnyBubbleBurst = false;
			}
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			lockContainer.UnlockAll();
			if (game.isBubblesSuspended || game.board.userMovesCount <= movesCountWhenTookAction || game.board.actionManager.ActionCount > 0 || !game.isBoardFullySettled)
			{
				return;
			}
			if (isAnyBubbleBurst)
			{
				CancelSpread();
				isAnyBubbleBurst = false;
				return;
			}
			List<Slot> allSlotsContainingBubbles = this.allSlotsContainingBubbles;
			if (allSlotsContainingBubbles.Count == 0)
			{
				CancelSpread();
				return;
			}
			slotsWhereCanPlaceNewBubble.Clear();
			for (int i = 0; i < allSlotsContainingBubbles.Count; i++)
			{
				List<Slot> neigbourSlots = allSlotsContainingBubbles[i].neigbourSlots;
				for (int j = 0; j < neigbourSlots.Count; j++)
				{
					Slot slot = neigbourSlots[j];
					if (!slot.isSlotGravitySuspended && !slot.isSlotSwapSuspended && !slot.isDestroySuspended && !slot.isPlaceBubbleSuspended)
					{
						slotsWhereCanPlaceNewBubble.Add(slot);
					}
				}
			}
			if (slotsWhereCanPlaceNewBubble.Count == 0)
			{
				CancelSpread();
				return;
			}
			Slot slot2 = slotsWhereCanPlaceNewBubble[Random.Range(0, slotsWhereCanPlaceNewBubble.Count)];
			game.AddBubblesToSlot(slot2);
			movesCountWhenTookAction = game.board.userMovesCount;
			game.particles.CreateParticles(slot2.localPositionOfCenter, Match3Particles.PositionType.BubblesCreate);
			game.Play(GGSoundSystem.SFXType.SnowCreate);
		}
	}
}
