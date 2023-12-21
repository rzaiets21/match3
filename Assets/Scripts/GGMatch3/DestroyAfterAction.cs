using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DestroyAfterAction : BoardAction
	{
		public struct InitArguments
		{
			public Match3Game game;

			public SlotDestroyParams destroyParams;

			public Slot slot;

			public Chip chip;

			public float delay;
		}

		private sealed class _003CDoAnimate_003Ed__6 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DestroyAfterAction _003C_003E4__this;

			private DestroyMatchingIslandBlinkAction.Settings _003Csettings_003E5__2;

			private float _003Ctime_003E5__3;

			private float _003Cduration_003E5__4;

			private Chip _003Cchip_003E5__5;

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
				DestroyAfterAction destroyAfterAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Csettings_003E5__2 = Match3Settings.instance.destroyAfterSettings;
					_003Ctime_003E5__3 = 0f;
					_003Cduration_003E5__4 = UnityEngine.Random.Range(_003Csettings_003E5__2.dulationToHold, _003Csettings_003E5__2.durationToHoldMax);
					if (destroyAfterAction.initArguments.delay > 0f)
					{
						_003Cduration_003E5__4 = destroyAfterAction.initArguments.delay;
					}
					_003Cchip_003E5__5 = destroyAfterAction.initArguments.chip;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__3 < _003Cduration_003E5__4)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					if (_003Csettings_003E5__2.useBlink)
					{
						float time = Mathf.InverseLerp(0f, _003Cduration_003E5__4, _003Ctime_003E5__3);
						float t = _003Csettings_003E5__2.brightnessCurve.Evaluate(time);
						float brightness = Mathf.LerpUnclamped(1f, _003Csettings_003E5__2.brightness, t);
						TransformBehaviour componentBehaviour = _003Cchip_003E5__5.GetComponentBehaviour<TransformBehaviour>();
						if (componentBehaviour != null)
						{
							componentBehaviour.SetBrightness(brightness);
						}
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (_003Csettings_003E5__2.useBlink)
				{
					TransformBehaviour componentBehaviour2 = _003Cchip_003E5__5.GetComponentBehaviour<TransformBehaviour>();
					if (componentBehaviour2 != null)
					{
						componentBehaviour2.SetBrightness(1f);
					}
				}
				destroyAfterAction.globalLock.UnlockAll();
				destroyAfterAction.initArguments.chip.DestroyBomb(destroyAfterAction.initArguments.destroyParams);
				destroyAfterAction.isAlive = false;
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
			globalLock.LockSlot(initArguments.slot);
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
