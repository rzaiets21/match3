using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class CrossExplosionAction : BoardAction
	{
		[Serializable]
		public class Settings
		{
			public float delay = 0.1f;

			public float lightIntensity = 0.7f;

			public FloatRange lightIntensityRange = new FloatRange(1.5f, 1f);

			public float maxLightDistance = 3f;

			public float lockReleaseDelay;

			public float shockWaveOffset = 0.05f;
		}

		public class Circle
		{
			public float radius;

			public List<Slot> slots = new List<Slot>();

			public void Add(Slot slot)
			{
				slots.Add(slot);
			}

			public void AddLock(Lock slotLock)
			{
				for (int i = 0; i < slots.Count; i++)
				{
					Slot slot = slots[i];
					slotLock.LockSlot(slot);
				}
			}
		}

		public class Parameters
		{
			public int radius = 1;

			public IntVector2 startPosition;

			public Match3Game game;

			public Chip bombChip;

			public bool isHavingCarpet;

			public bool explode;
		}

		private sealed class _003CDoExplosion_003Ed__8 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CrossExplosionAction _003C_003E4__this;

			private Lock _003CslotLock_003E5__2;

			private SlotDestroyParams _003CbombParams_003E5__3;

			private Settings _003CexplosionSettings_003E5__4;

			private float _003Ctime_003E5__5;

			private int _003Ci_003E5__6;

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
			public _003CDoExplosion_003Ed__8(int _003C_003E1__state)
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
				CrossExplosionAction crossExplosionAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003CslotLock_003E5__2 = crossExplosionAction.lockContainer.NewLock();
					_003CslotLock_003E5__2.isSlotGravitySuspended = true;
					_003CbombParams_003E5__3 = new SlotDestroyParams();
					_003CbombParams_003E5__3.isHitByBomb = true;
					_003CbombParams_003E5__3.isHavingCarpet = crossExplosionAction.parameters.isHavingCarpet;
					_003CexplosionSettings_003E5__4 = Match3Settings.instance.crossExplosionSettings;
					_003CbombParams_003E5__3.isExplosion = crossExplosionAction.parameters.explode;
					_003CbombParams_003E5__3.explosionCentre = crossExplosionAction.parameters.startPosition;
					if (crossExplosionAction.parameters.bombChip != null)
					{
						DestroyChipAction destroyChipAction = new DestroyChipAction();
						DestroyChipAction.InitArguments initArguments = default(DestroyChipAction.InitArguments);
						initArguments.chip = crossExplosionAction.parameters.bombChip;
						initArguments.slot = crossExplosionAction.parameters.game.board.GetSlot(crossExplosionAction.parameters.startPosition);
						destroyChipAction.Init(initArguments);
						crossExplosionAction.parameters.game.board.actionManager.AddAction(destroyChipAction);
					}
					_003Ctime_003E5__5 = 0f;
					_003Ci_003E5__6 = 0;
					goto IL_024c;
				case 1:
					_003C_003E1__state = -1;
					goto IL_0227;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_0227:
					if (_003Ctime_003E5__5 < _003CexplosionSettings_003E5__4.delay)
					{
						_003Ctime_003E5__5 += crossExplosionAction.parameters.game.board.currentDeltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_023a;
					IL_024c:
					if (_003Ci_003E5__6 < crossExplosionAction.circles.Count)
					{
						bool flag = _003Ci_003E5__6 == crossExplosionAction.circles.Count - 1;
						Circle circle = crossExplosionAction.circles[_003Ci_003E5__6];
						circle.AddLock(_003CslotLock_003E5__2);
						List<Slot> slots = circle.slots;
						for (int i = 0; i < slots.Count; i++)
						{
							Slot slot = slots[i];
							slot.OnDestroySlot(_003CbombParams_003E5__3);
							slot.light.AddLight(_003CexplosionSettings_003E5__4.lightIntensityRange.Lerp(Mathf.InverseLerp(0f, _003CexplosionSettings_003E5__4.maxLightDistance, circle.radius)));
						}
						_003Ctime_003E5__5 = 0f;
						if (!flag)
						{
							goto IL_0227;
						}
						goto IL_023a;
					}
					crossExplosionAction.AffectOuterCircleWithExplosion(crossExplosionAction.parameters.startPosition, crossExplosionAction.parameters.radius, _003CexplosionSettings_003E5__4.shockWaveOffset);
					_003Ctime_003E5__5 = 0f;
					break;
					IL_023a:
					_003Ci_003E5__6++;
					goto IL_024c;
				}
				if (_003Ctime_003E5__5 < _003CexplosionSettings_003E5__4.lockReleaseDelay)
				{
					_003Ctime_003E5__5 += crossExplosionAction.parameters.game.board.currentDeltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003CslotLock_003E5__2.UnlockAll();
				crossExplosionAction.globalSlotLock.UnlockAll();
				crossExplosionAction.isAlive = false;
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

		private Lock globalSlotLock;

		private List<Slot> allAffectedSlots;

		private Parameters parameters;

		public List<Circle> circles = new List<Circle>();

		private bool isHavingCarpet;

		private IEnumerator explosionCoroutine;

		public void Init(Parameters parameters)
		{
			this.parameters = parameters;
			allAffectedSlots = new List<Slot>();
			circles = new List<Circle>();
			Circle item = new Circle();
			circles.Add(item);
			Slot slot = parameters.game.board.GetSlot(parameters.startPosition);
			if (slot != null)
			{
				allAffectedSlots.Add(slot);
			}
			Circle circle = new Circle();
			circle.Add(slot);
			circles.Add(circle);
			for (int i = 1; i <= parameters.radius; i++)
			{
				Circle circle2 = new Circle();
				circles.Add(circle2);
				IntVector2 startPosition = parameters.startPosition;
				Match3Board board = parameters.game.board;
				Slot slot2 = board.GetSlot(startPosition + IntVector2.up * i);
				Slot slot3 = board.GetSlot(startPosition + IntVector2.down * i);
				Slot slot4 = board.GetSlot(startPosition + IntVector2.left * i);
				Slot slot5 = board.GetSlot(startPosition + IntVector2.right * i);
				if (slot2 != null)
				{
					circle2.Add(slot2);
				}
				if (slot3 != null)
				{
					circle2.Add(slot3);
				}
				if (slot4 != null)
				{
					circle2.Add(slot4);
				}
				if (slot5 != null)
				{
					circle2.Add(slot5);
				}
				allAffectedSlots.AddRange(circle2.slots);
			}
			for (int j = 0; j < circles.Count; j++)
			{
				Circle circle3 = circles[j];
				for (int k = 0; k < circle3.slots.Count; k++)
				{
					Slot slot6 = circle3.slots[k];
					if (slot6 != null && slot6.canCarpetSpreadFromHere)
					{
						isHavingCarpet = true;
					}
				}
			}
			globalSlotLock = lockContainer.NewLock();
			globalSlotLock.isAvailableForDiscoBombSuspended = true;
			globalSlotLock.isSlotGravitySuspended = true;
			globalSlotLock.isChipGeneratorSuspended = true;
			globalSlotLock.isAboutToBeDestroyed = true;
			globalSlotLock.LockSlots(allAffectedSlots);
			parameters.game.particles.CreateParticles(parameters.game.LocalPositionOfCenter(parameters.startPosition), Match3Particles.PositionType.OnSeekingMissleExplosion, ChipType.SeekingMissle, ItemColor.Unknown);
		}

		public IEnumerator DoExplosion()
		{
			return new _003CDoExplosion_003Ed__8(0)
			{
				_003C_003E4__this = this
			};
		}

		private void AffectOuterCircleWithExplosion(IntVector2 center, int radius, float shockWaveOffset)
		{
			Match3Game game = parameters.game;
			Vector3 b = game.LocalPositionOfCenter(center);
			for (int i = center.x - radius; i <= center.x + radius; i++)
			{
				for (int j = center.y - radius; j <= center.y + radius; j++)
				{
					int a = Mathf.Abs(center.x - i);
					int b2 = Mathf.Abs(center.y - j);
					if (Mathf.Max(a, b2) == radius)
					{
						Slot slot = game.board.GetSlot(new IntVector2(i, j));
						if (slot != null)
						{
							slot.offsetPosition = (slot.localPositionOfCenter - b).normalized * shockWaveOffset;
							slot.positionIntegrator.currentPosition = slot.offsetPosition;
						}
					}
				}
			}
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (isAlive)
			{
				if (explosionCoroutine == null)
				{
					explosionCoroutine = DoExplosion();
				}
				explosionCoroutine.MoveNext();
			}
		}

		public List<Slot> GetAffectedSlots()
		{
			return allAffectedSlots;
		}
	}
}
