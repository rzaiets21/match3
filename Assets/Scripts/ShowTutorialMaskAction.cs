using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class ShowTutorialMaskAction : BoardAction
{
	public class Parameters
	{
		public LevelDefinition.TutorialMatch match;

		public Match3Game game;

		public Action onMiddle;

		public Action onEnd;
	}

	public class SlotChipPair
	{
		private Slot slot;

		private Chip chip;

		public void Init(Slot slot, Chip chip)
		{
			this.slot = slot;
			this.chip = chip;
		}
	}

	private sealed class _003CDoShowTutorial_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ShowTutorialMaskAction _003C_003E4__this;

		private Lock _003CswapLock_003E5__2;

		private Lock _003CtouchLock_003E5__3;

		private Lock _003CswipeLock_003E5__4;

		private Match3Game _003Cgame_003E5__5;

		private Slot _003CslotToSwipe_003E5__6;

		private Slot _003CexchangeSlot_003E5__7;

		private SetLock _003CtutorialLockSwipeSlot_003E5__8;

		private SetLock _003CtutorialLockExchangeSlot_003E5__9;

		private int _003CstartMove_003E5__10;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDoShowTutorial_003Ed__9(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			ShowTutorialMaskAction showTutorialMaskAction = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				showTutorialMaskAction.isStarted = true;
				Match3Game.TutorialSlotHighlighter tutorialHighlighter = showTutorialMaskAction.parameters.game.tutorialHighlighter;
				tutorialHighlighter.Show(showTutorialMaskAction.provider);
				tutorialHighlighter.ShowGameScreenTutorialMask();
				_003CswapLock_003E5__2 = showTutorialMaskAction.lockContainer.NewLock();
				_003CswapLock_003E5__2.isSlotMatchingSuspended = true;
				_003CtouchLock_003E5__3 = showTutorialMaskAction.lockContainer.NewLock();
				_003CtouchLock_003E5__3.isSlotTouchingSuspended = true;
				_003CswipeLock_003E5__4 = showTutorialMaskAction.lockContainer.NewLock();
				_003CswipeLock_003E5__4.isSlotSwipeSuspended = true;
				for (int i = 0; i < showTutorialMaskAction.parameters.game.board.slots.Length; i++)
				{
					Slot slot = showTutorialMaskAction.parameters.game.board.slots[i];
					if (slot != null && !showTutorialMaskAction.parameters.match.Contains(slot.position))
					{
						_003CswapLock_003E5__2.LockSlot(slot);
					}
				}
				for (int j = 0; j < showTutorialMaskAction.parameters.match.matchingSlots.Count; j++)
				{
					IntVector2 intVector = showTutorialMaskAction.parameters.match.matchingSlots[j];
					if (!(intVector == showTutorialMaskAction.parameters.match.slotToSwipe))
					{
						Slot slot2 = showTutorialMaskAction.parameters.game.board.GetSlot(intVector);
						_003CswipeLock_003E5__4.LockSlot(slot2);
					}
				}
				ShowPotentialMatchAction.InitParams initParams = default(ShowPotentialMatchAction.InitParams);
				initParams.game = showTutorialMaskAction.parameters.game;
				initParams.tutorialMatch = showTutorialMaskAction.parameters.match;
				initParams.stayInfiniteTime = true;
				initParams.dontStopWhenInvalid = true;
				_003Cgame_003E5__5 = showTutorialMaskAction.parameters.game;
				_003CslotToSwipe_003E5__6 = showTutorialMaskAction.parameters.game.GetSlot(showTutorialMaskAction.parameters.match.slotToSwipe);
				Chip slotComponent = _003CslotToSwipe_003E5__6.GetSlotComponent<Chip>();
				_003CexchangeSlot_003E5__7 = showTutorialMaskAction.parameters.game.GetSlot(showTutorialMaskAction.parameters.match.exchangeSlot);
				Chip slotComponent2 = _003CexchangeSlot_003E5__7.GetSlotComponent<Chip>();
				if (_003CslotToSwipe_003E5__6 != null && _003CexchangeSlot_003E5__7 != null)
				{
					TutorialHandController.InitArguments initArguments = default(TutorialHandController.InitArguments);
					initArguments.repeat = true;
					initArguments.startLocalPosition = _003Cgame_003E5__5.gameScreen.transform.InverseTransformPoint(_003Cgame_003E5__5.LocalToWorldPosition(_003CslotToSwipe_003E5__6.localPositionOfCenter));
					initArguments.endLocalPosition = _003Cgame_003E5__5.gameScreen.transform.InverseTransformPoint(_003Cgame_003E5__5.LocalToWorldPosition(_003CexchangeSlot_003E5__7.localPositionOfCenter));
					initArguments.settings = Match3Settings.instance.tutorialSwipeSettings;
					if (_003CslotToSwipe_003E5__6 == _003CexchangeSlot_003E5__7)
					{
						initArguments.settings = Match3Settings.instance.tutorialTouchSettings;
					}
					_003Cgame_003E5__5.gameScreen.tutorialHand.Show(initArguments);
				}
				showTutorialMaskAction.showPotentialMatchAction = new ShowPotentialMatchAction();
				showTutorialMaskAction.showPotentialMatchAction.Init(initParams);
				showTutorialMaskAction.parameters.game.board.actionManager.AddAction(showTutorialMaskAction.showPotentialMatchAction);
				_003CtutorialLockSwipeSlot_003E5__8 = new SetLock();
				_003CtutorialLockSwipeSlot_003E5__8.isSwapingSuspended = true;
				_003CtutorialLockExchangeSlot_003E5__9 = new SetLock();
				_003CtutorialLockExchangeSlot_003E5__9.isSwapingSuspended = true;
				if (_003CexchangeSlot_003E5__7 == null || _003CslotToSwipe_003E5__6 == _003CexchangeSlot_003E5__7)
				{
					_003CswipeLock_003E5__4.LockSlot(_003CslotToSwipe_003E5__6);
				}
				else if (slotComponent.canBeTappedToActivate && slotComponent2.canBeTappedToActivate)
				{
					_003CtouchLock_003E5__3.LockSlot(_003CslotToSwipe_003E5__6);
					_003CtouchLock_003E5__3.LockSlot(_003CexchangeSlot_003E5__7);
					_003CswipeLock_003E5__4.LockSlot(_003CexchangeSlot_003E5__7);
					_003CtutorialLockSwipeSlot_003E5__8.slots.Add(_003CexchangeSlot_003E5__7);
					_003CslotToSwipe_003E5__6.AddSetLock(_003CtutorialLockSwipeSlot_003E5__8);
				}
				else
				{
					_003CtutorialLockSwipeSlot_003E5__8.slots.Add(_003CexchangeSlot_003E5__7);
					_003CslotToSwipe_003E5__6.AddSetLock(_003CtutorialLockSwipeSlot_003E5__8);
					_003CtouchLock_003E5__3.LockSlot(_003CslotToSwipe_003E5__6);
					_003CtutorialLockExchangeSlot_003E5__9.slots.Add(_003CslotToSwipe_003E5__6);
					_003CexchangeSlot_003E5__7.AddSetLock(_003CtutorialLockExchangeSlot_003E5__9);
					_003CtouchLock_003E5__3.LockSlot(_003CexchangeSlot_003E5__7);
				}
				showTutorialMaskAction.swipeChipSlotInfo.Init(_003CslotToSwipe_003E5__6, _003CslotToSwipe_003E5__6.GetSlotComponent<Chip>());
				_003CstartMove_003E5__10 = showTutorialMaskAction.parameters.game.board.userMovesCount;
				goto IL_04cf;
			}
			case 1:
				_003C_003E1__state = -1;
				goto IL_04cf;
			case 2:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_04cf:
				if (showTutorialMaskAction.parameters.game.board.userMovesCount <= _003CstartMove_003E5__10)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Cgame_003E5__5.gameScreen.tutorialHand.Hide();
				_003CswapLock_003E5__2.Unlock(_003CslotToSwipe_003E5__6);
				_003CtouchLock_003E5__3.Unlock(_003CslotToSwipe_003E5__6);
				_003CtouchLock_003E5__3.Unlock(_003CexchangeSlot_003E5__7);
				_003CswipeLock_003E5__4.Unlock(_003CexchangeSlot_003E5__7);
				_003CslotToSwipe_003E5__6.RemoveSetLock(_003CtutorialLockSwipeSlot_003E5__8);
				_003CexchangeSlot_003E5__7.RemoveSetLock(_003CtutorialLockExchangeSlot_003E5__9);
				for (int k = 0; k < showTutorialMaskAction.parameters.game.board.slots.Length; k++)
				{
					Slot slot3 = showTutorialMaskAction.parameters.game.board.slots[k];
					if (slot3 != null && !showTutorialMaskAction.parameters.match.Contains(slot3.position))
					{
						_003CswapLock_003E5__2.Unlock(slot3);
					}
				}
				for (int l = 0; l < showTutorialMaskAction.parameters.match.matchingSlots.Count; l++)
				{
					IntVector2 position = showTutorialMaskAction.parameters.match.matchingSlots[l];
					Slot slot4 = showTutorialMaskAction.parameters.game.board.GetSlot(position);
					_003CswipeLock_003E5__4.Unlock(slot4);
				}
				if (showTutorialMaskAction.parameters.onMiddle != null)
				{
					showTutorialMaskAction.parameters.onMiddle();
				}
				showTutorialMaskAction.showPotentialMatchAction.Stop();
				showTutorialMaskAction.parameters.game.tutorialHighlighter.Hide();
				break;
			}
			if (!showTutorialMaskAction.isBoardSettled)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			}
			if (showTutorialMaskAction.parameters.onEnd != null)
			{
				showTutorialMaskAction.parameters.onEnd();
			}
			showTutorialMaskAction.tutorialEnumerator = null;
			_003CswapLock_003E5__2.UnlockAll();
			_003CtouchLock_003E5__3.UnlockAll();
			_003CswipeLock_003E5__4.UnlockAll();
			showTutorialMaskAction.isAlive = false;
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private Parameters parameters;

	public bool isBoardSettled;

	private ListSlotsProvider provider = new ListSlotsProvider();

	private ShowPotentialMatchAction showPotentialMatchAction = new ShowPotentialMatchAction();

	private SlotChipPair swipeChipSlotInfo = new SlotChipPair();

	private IEnumerator tutorialEnumerator;

	public void Init(Parameters parameters)
	{
		this.parameters = parameters;
		provider.Clear();
		provider.Init(parameters.game);
		for (int i = 0; i < parameters.match.matchingSlots.Count; i++)
		{
			IntVector2 position = parameters.match.matchingSlots[i];
			provider.AddSlot(new TilesSlotsProvider.Slot(position, isOccupied: true));
		}
		if (!parameters.match.matchingSlots.Contains(parameters.match.exchangeSlot))
		{
			provider.AddSlot(new TilesSlotsProvider.Slot(parameters.match.exchangeSlot, isOccupied: true));
		}
		if (!parameters.match.matchingSlots.Contains(parameters.match.slotToSwipe))
		{
			provider.AddSlot(new TilesSlotsProvider.Slot(parameters.match.slotToSwipe, isOccupied: true));
		}
	}

	public override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
		if (tutorialEnumerator == null)
		{
			tutorialEnumerator = DoShowTutorial();
		}
		tutorialEnumerator.MoveNext();
	}

	private IEnumerator DoShowTutorial()
	{
		return new _003CDoShowTutorial_003Ed__9(0)
		{
			_003C_003E4__this = this
		};
	}

	public override void Stop()
	{
		base.Stop();
		if (parameters.onEnd != null)
		{
			parameters.onEnd();
		}
	}
}
