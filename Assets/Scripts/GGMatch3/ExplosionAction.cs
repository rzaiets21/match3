using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class ExplosionAction : BoardAction
	{
		public struct ExplosionSettings
		{
			public Chip bombChip;

			public IntVector2 position;

			public int radius;

			public bool isHavingCarpet;

			public bool isUsingBombAreaOfEffect;
		}

		[Serializable]
		public class Settings
		{
			public float delay = 0.1f;

			public float lightIntensity = 0.7f;

			public FloatRange lightIntensityRange = new FloatRange(1.5f, 1f);

			public float lightDuration = 1f;

			public float maxLightDistance = 3f;

			public float shockWaveOffset = 0.02f;

			public float shockWaveOffsetR1 = 0.01f;

			public bool useSecondaryDelay;

			public float secondaryDelay;

			public bool useCameraShake;
		}

		public class CircleList
		{
			public int radius;

			public List<Slot> slotList = new List<Slot>();

			public void AddLock(Lock slotLock)
			{
				if (slotLock != null)
				{
					for (int i = 0; i < slotList.Count; i++)
					{
						Slot slot = slotList[i];
						slotLock.LockSlot(slot);
					}
				}
			}
		}

		private sealed class _003CDoExplosion_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ExplosionAction _003C_003E4__this;

			private Settings _003CexplosionSettings_003E5__2;

			private float _003Cdelay_003E5__3;

			private Lock _003CslotLock_003E5__4;

			private SlotDestroyParams _003CbombParams_003E5__5;

			private int _003Ci_003E5__6;

			private float _003Ctime_003E5__7;

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
			public _003CDoExplosion_003Ed__12(int _003C_003E1__state)
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
				ExplosionAction explosionAction = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					goto IL_025c;
				}
				_003C_003E1__state = -1;
				_003CexplosionSettings_003E5__2 = Match3Settings.instance.explosionSettings;
				_003Cdelay_003E5__3 = _003CexplosionSettings_003E5__2.delay;
				_003CslotLock_003E5__4 = explosionAction.lockContainer.NewLock();
				_003CslotLock_003E5__4.isSlotGravitySuspended = true;
				_003CbombParams_003E5__5 = new SlotDestroyParams();
				_003CbombParams_003E5__5.isHitByBomb = true;
				_003CbombParams_003E5__5.bombType = ChipType.Bomb;
				_003CbombParams_003E5__5.isExplosion = true;
				_003CbombParams_003E5__5.isHavingCarpet = explosionAction.isHavingCarpet;
				_003CbombParams_003E5__5.explosionCentre = explosionAction.settings.position;
				if (explosionAction.settings.bombChip != null)
				{
					DestroyChipActionGrow destroyChipActionGrow = new DestroyChipActionGrow();
					destroyChipActionGrow.Init(explosionAction.settings.bombChip, explosionAction.settings.bombChip.lastConnectedSlot);
					explosionAction.game.board.actionManager.AddAction(destroyChipActionGrow);
				}
				if (_003CexplosionSettings_003E5__2.useCameraShake)
				{
					explosionAction.game.ShakeCamera();
				}
				_003Ci_003E5__6 = 0;
				goto IL_027c;
				IL_027c:
				if (_003Ci_003E5__6 < explosionAction.circles.Count)
				{
					CircleList circleList = explosionAction.circles[_003Ci_003E5__6];
					circleList.AddLock(_003CslotLock_003E5__4);
					bool flag = _003Ci_003E5__6 == explosionAction.circles.Count - 1;
					bool flag2 = _003Ci_003E5__6 == 0;
					for (int i = 0; i < circleList.slotList.Count; i++)
					{
						Slot slot = circleList.slotList[i];
						slot.OnDestroySlot(_003CbombParams_003E5__5);
						slot.light.AddLightWithDuration(_003CexplosionSettings_003E5__2.lightIntensityRange.Lerp(Mathf.InverseLerp(0f, _003CexplosionSettings_003E5__2.maxLightDistance, circleList.radius)), _003CexplosionSettings_003E5__2.lightDuration);
					}
					_003Ctime_003E5__7 = 0f;
					_003Cdelay_003E5__3 = _003CexplosionSettings_003E5__2.delay;
					if (!flag2 && _003CexplosionSettings_003E5__2.useSecondaryDelay)
					{
						_003Cdelay_003E5__3 = _003CexplosionSettings_003E5__2.secondaryDelay;
					}
					if (!flag && _003Cdelay_003E5__3 > 0f)
					{
						goto IL_025c;
					}
					goto IL_026a;
				}
				_003CslotLock_003E5__4.UnlockAll();
				explosionAction.globalSlotLock.UnlockAll();
				explosionAction.AffectOuterCircleWithExplosion(explosionAction.settings.position, explosionAction.settings.radius, _003CexplosionSettings_003E5__2.shockWaveOffset);
				explosionAction.AffectOuterCircleWithExplosion(explosionAction.settings.position, explosionAction.settings.radius + 1, _003CexplosionSettings_003E5__2.shockWaveOffsetR1);
				explosionAction.isAlive = false;
				return false;
				IL_026a:
				_003Ci_003E5__6++;
				goto IL_027c;
				IL_025c:
				if (_003Ctime_003E5__7 < _003Cdelay_003E5__3)
				{
					_003Ctime_003E5__7 += explosionAction.game.board.currentDeltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_026a;
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

		protected ExplosionSettings settings;

		protected Match3Game game;

		private IEnumerator explosionEnumerator;

		private Lock globalSlotLock;

		private List<CircleList> circles;

		private List<Slot> allAffectedSlots = new List<Slot>();

		private bool isHavingCarpet;

		public void Init(Match3Game game, ExplosionSettings settings)
		{
			this.game = game;
			this.settings = settings;
			if (settings.bombChip != null)
			{
				settings.bombChip.RemoveFromSlot();
			}
			circles = new List<CircleList>();
			isHavingCarpet = settings.isHavingCarpet;
			IntVector2 position = settings.position;
			List<Slot> bombArea = game.GetBombArea(position, settings.radius - 1);
			for (int i = 0; i < settings.radius; i++)
			{
				CircleList circleList = new CircleList();
				circleList.radius = i;
				circles.Add(circleList);
				if (settings.isUsingBombAreaOfEffect)
				{
					for (int j = 0; j < bombArea.Count; j++)
					{
						Slot slot = bombArea[j];
						if (slot == null)
						{
							continue;
						}
						IntVector2 intVector = slot.position - position;
						int a = Mathf.Abs(intVector.x);
						int b = Mathf.Abs(intVector.y);
						if (Mathf.Max(a, b) == i)
						{
							circleList.slotList.Add(slot);
							allAffectedSlots.Add(slot);
							if (slot.canCarpetSpreadFromHere)
							{
								isHavingCarpet = true;
							}
						}
					}
					continue;
				}
				for (int k = position.x - i; k <= position.x + i; k++)
				{
					for (int l = position.y - i; l <= position.y + i; l++)
					{
						int a2 = Mathf.Abs(position.x - k);
						int b2 = Mathf.Abs(position.y - l);
						if (Mathf.Max(a2, b2) != i)
						{
							continue;
						}
						Slot slot2 = game.board.GetSlot(new IntVector2(k, l));
						if (slot2 != null)
						{
							circleList.slotList.Add(slot2);
							allAffectedSlots.Add(slot2);
							if (slot2.canCarpetSpreadFromHere)
							{
								isHavingCarpet = true;
							}
						}
					}
				}
			}
			globalSlotLock = lockContainer.NewLock();
			globalSlotLock.isAvailableForDiscoBombSuspended = true;
			globalSlotLock.isSlotGravitySuspended = true;
			globalSlotLock.isChipGeneratorSuspended = true;
			globalSlotLock.isAboutToBeDestroyed = true;
			globalSlotLock.LockSlots(allAffectedSlots);
			game.particles.CreateParticles(game.LocalPositionOfCenter(position), Match3Particles.PositionType.OnExplosion, ChipType.Bomb, ItemColor.Unknown);
			game.Play(GGSoundSystem.SFXType.BombExplode);
		}

		public override void OnStart(ActionManager manager)
		{
			base.OnStart(manager);
		}

		private IEnumerator DoExplosion()
		{
			return new _003CDoExplosion_003Ed__12(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (isAlive)
			{
				if (explosionEnumerator == null)
				{
					explosionEnumerator = DoExplosion();
					explosionEnumerator.MoveNext();
				}
				explosionEnumerator.MoveNext();
			}
		}

		private void AffectOuterCircleWithExplosion(IntVector2 center, int radius, float shockWaveOffset)
		{
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
	}
}
