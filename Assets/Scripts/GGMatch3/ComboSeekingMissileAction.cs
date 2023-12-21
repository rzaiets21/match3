using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace GGMatch3
{
	public class ComboSeekingMissileAction : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public float delay = 0.4f;
		}

		public class Parameters
		{
			public int rocketsCount;

			public Match3Game game;

			public Slot startSlot;

			public bool isHavingCarpet;
		}

		private sealed class _003CDoShootRockets_003Ed__7 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ComboSeekingMissileAction _003C_003E4__this;

			private int _003Ci_003E5__2;

			private SeekingMissileAction.Parameters _003CseekingMissileParameters_003E5__3;

			private float _003Cdelay_003E5__4;

			private float _003Ctime_003E5__5;

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
			public _003CDoShootRockets_003Ed__7(int _003C_003E1__state)
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
				ComboSeekingMissileAction comboSeekingMissileAction = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					goto IL_013a;
				}
				_003C_003E1__state = -1;
				comboSeekingMissileAction.sourceLock.UnlockAll();
				_003Ci_003E5__2 = 0;
				goto IL_0161;
				IL_0161:
				if (_003Ci_003E5__2 < comboSeekingMissileAction.parameters.rocketsCount)
				{
					bool num2 = _003Ci_003E5__2 == comboSeekingMissileAction.parameters.rocketsCount - 1;
					bool doCrossExplosion = _003Ci_003E5__2 == 0;
					SeekingMissileAction seekingMissileAction = new SeekingMissileAction();
					_003CseekingMissileParameters_003E5__3 = new SeekingMissileAction.Parameters();
					_003CseekingMissileParameters_003E5__3.doCrossExplosion = doCrossExplosion;
					_003CseekingMissileParameters_003E5__3.startSlot = comboSeekingMissileAction.parameters.startSlot;
					_003CseekingMissileParameters_003E5__3.game = comboSeekingMissileAction.parameters.game;
					_003CseekingMissileParameters_003E5__3.isHavingCarpet = comboSeekingMissileAction.parameters.isHavingCarpet;
					seekingMissileAction.Init(_003CseekingMissileParameters_003E5__3);
					comboSeekingMissileAction.parameters.game.board.actionManager.AddAction(seekingMissileAction);
					if (!num2)
					{
						_003Cdelay_003E5__4 = Match3Settings.instance.seekingMissileComboSettings.delay;
						_003Ctime_003E5__5 = 0f;
						goto IL_013a;
					}
					goto IL_0148;
				}
				comboSeekingMissileAction.isAlive = false;
				return false;
				IL_0148:
				_003CseekingMissileParameters_003E5__3 = null;
				_003Ci_003E5__2++;
				goto IL_0161;
				IL_013a:
				if (_003Ctime_003E5__5 < _003Cdelay_003E5__4)
				{
					_003Ctime_003E5__5 += _003CseekingMissileParameters_003E5__3.game.board.currentDeltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_0148;
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

		private Lock sourceLock;

		private IEnumerator shootRocketsCoroutine;

		public void Init(Parameters parameters)
		{
			this.parameters = parameters;
			sourceLock = lockContainer.NewLock();
			sourceLock.isSlotGravitySuspended = true;
			sourceLock.isChipGeneratorSuspended = true;
			sourceLock.LockSlot(parameters.startSlot);
		}

		public override void OnUpdate(float deltaTime)
		{
			if (shootRocketsCoroutine == null)
			{
				shootRocketsCoroutine = DoShootRockets();
			}
			shootRocketsCoroutine.MoveNext();
		}

		public IEnumerator DoShootRockets()
		{
			return new _003CDoShootRockets_003Ed__7(0)
			{
				_003C_003E4__this = this
			};
		}
	}
}
