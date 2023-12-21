using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DestroyMatchingIslandBlinkAction : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public float dulationToHold = 0.2f;

			public float durationToHoldMax;

			public bool useBlink;

			public bool changeBrightness;

			public float brightness;

			public AnimationCurve brightnessCurve;
		}

		private sealed class _003CDoAnimate_003Ed__6 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DestroyMatchingIslandBlinkAction _003C_003E4__this;

			private Settings _003Csettings_003E5__2;

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
				DestroyMatchingIslandBlinkAction destroyMatchingIslandBlinkAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Csettings_003E5__2 = Match3Settings.instance.destroyIslandBlinkSettings;
					_003Ctime_003E5__3 = 0f;
					_003Cduration_003E5__4 = _003Csettings_003E5__2.dulationToHold;
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
						for (int i = 0; i < destroyMatchingIslandBlinkAction.initArguments.allSlots.Count; i++)
						{
							Chip slotComponent = destroyMatchingIslandBlinkAction.initArguments.allSlots[i].GetSlotComponent<Chip>();
							if (slotComponent != null)
							{
								TransformBehaviour componentBehaviour = slotComponent.GetComponentBehaviour<TransformBehaviour>();
								if (!(componentBehaviour == null))
								{
									componentBehaviour.SetBrightness(brightness);
								}
							}
						}
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (_003Csettings_003E5__2.useBlink)
				{
					for (int j = 0; j < destroyMatchingIslandBlinkAction.initArguments.allSlots.Count; j++)
					{
						Chip slotComponent2 = destroyMatchingIslandBlinkAction.initArguments.allSlots[j].GetSlotComponent<Chip>();
						if (slotComponent2 != null)
						{
							TransformBehaviour componentBehaviour2 = slotComponent2.GetComponentBehaviour<TransformBehaviour>();
							if (!(componentBehaviour2 == null))
							{
								componentBehaviour2.SetBrightness(1f);
							}
						}
					}
				}
				destroyMatchingIslandBlinkAction.globalLock.UnlockAll();
				destroyMatchingIslandBlinkAction.initArguments.game.FinishDestroySlots(destroyMatchingIslandBlinkAction.initArguments);
				destroyMatchingIslandBlinkAction.isAlive = false;
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

		private DestroyMatchingIslandAction.InitArguments initArguments;

		private float deltaTime;

		private IEnumerator enumerator;

		public void Init(DestroyMatchingIslandAction.InitArguments initArguments)
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
