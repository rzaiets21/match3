using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DestroyMatchingIslandAction : BoardAction
	{
		public struct InitArguments
		{
			public Match3Game game;

			public List<Slot> allSlots;

			public SlotDestroyParams slotDestroyParams;

			public Slot slotWherePowerupIsCreated;

			public ChipType powerupToCreate;

			public BoltCollection boltCollection;

			public int matchComboIndex;
		}

		private sealed class _003CDoAnimate_003Ed__6 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DestroyMatchingIslandAction _003C_003E4__this;

			private MatchChipAffector _003CchipAffector_003E5__2;

			private float _003Ctime_003E5__3;

			private float _003Cduration_003E5__4;

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
			public _003CDoAnimate_003Ed__6(int _003C_003E1__state)
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
				DestroyMatchingIslandAction destroyMatchingIslandAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					MatchChipAffector.InitArguments initArguments = new MatchChipAffector.InitArguments();
					initArguments.game = destroyMatchingIslandAction.initArguments.game;
					initArguments.matchingSlots.AddRange(destroyMatchingIslandAction.initArguments.allSlots);
					if (destroyMatchingIslandAction.initArguments.slotWherePowerupIsCreated != null)
					{
						initArguments.originSlot = destroyMatchingIslandAction.initArguments.slotWherePowerupIsCreated;
					}
					else
					{
						initArguments.originSlot = destroyMatchingIslandAction.initArguments.allSlots[0];
					}
					initArguments.ignoreSlots.AddRange(destroyMatchingIslandAction.initArguments.allSlots);
					_003CchipAffector_003E5__2 = new MatchChipAffector();
					_003CchipAffector_003E5__2.Init(initArguments);
					_003Ctime_003E5__3 = 0f;
					_003Cduration_003E5__4 = Match3Settings.instance.swipeAffectorSettings.minAffectorDuration;
					if (Match3Settings.instance.swipeAffectorSettings.useAutoMatchDuration)
					{
						_003Cduration_003E5__4 = Mathf.Max(_003Cduration_003E5__4, Match3Settings.instance.swipeAffectorSettings.autoMatchDuration);
					}
					break;
				}
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__3 < _003Cduration_003E5__4)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003CchipAffector_003E5__2.Update();
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003CchipAffector_003E5__2.GiveLightingBoltsTo(destroyMatchingIslandAction.initArguments.boltCollection.bolts);
				_003CchipAffector_003E5__2.OnAfterDestroy();
				_003CchipAffector_003E5__2.Clear();
				destroyMatchingIslandAction.globalLock.UnlockAll();
				destroyMatchingIslandAction.initArguments.game.FinishDestroySlots(destroyMatchingIslandAction.initArguments);
				destroyMatchingIslandAction.isAlive = false;
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

		private Lock globalLock;

		private InitArguments initArguments;

		private float deltaTime;

		private IEnumerator enumerator;

		public void Init(InitArguments initArguments)
		{
			globalLock = lockContainer.NewLock();
			this.initArguments = initArguments;
			globalLock.SuspendAll();
			globalLock.LockSlots(initArguments.allSlots);
			enumerator = DoAnimate();
		}

		private IEnumerator DoAnimate()
		{
			return new _003CDoAnimate_003Ed__6(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.deltaTime = deltaTime;
			if (enumerator != null && !enumerator.MoveNext())
			{
				enumerator = null;
			}
		}
	}
}
