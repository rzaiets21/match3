using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class DiscoBallDestroyAction : BoardAction
	{
		public class DiscoParams
		{
			public Match3Game game;

			public Slot originSlot;

			public Chip originBomb;

			public List<Slot> affectedSlotsList = new List<Slot>();

			public bool replaceWithBombs;

			public ChipType bombType;

			public Chip otherBomb;

			public bool isInstant;

			public bool isHavingCarpet;

			public List<LightingBolt> bolts;

			public float affectorDuration;

			public ItemColor itemColor;

			public bool hasBolts
			{
				get
				{
					if (bolts != null)
					{
						return bolts.Count > 0;
					}
					return false;
				}
			}

			public void InitWithItemColor(Slot originSlot, Match3Game game, ItemColor itemColor, bool replaceWithBombs)
			{
				this.game = game;
				this.originSlot = originSlot;
				this.itemColor = itemColor;
				Slot[] slots = game.board.slots;
				foreach (Slot slot in slots)
				{
					if (slot != null && slot.CanParticipateInDiscoBombAffectedArea(itemColor, replaceWithBombs))
					{
						affectedSlotsList.Add(slot);
					}
				}
			}
		}

		[Serializable]
		public class Settings
		{
			public FloatRange amplitudeRange;

			public float angleSpeed;

			public float delayBetweenAffectedSlots = 0.2f;

			public float initialShakeTime = 0.5f;

			public float shakeTimeForSlot = 0.5f;

			public float minTotalDuration = 2f;

			public float touchSlotLightIntensity = 0.6f;

			public float shockWaveOffset = 0.2f;

			public bool useCameraShake;

			public GeneralSettings.CameraShakeSettings cameraShake = new GeneralSettings.CameraShakeSettings();
		}

		private sealed class _003CShakeChip_003Ed__15 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public Chip chip;

			public DiscoBallDestroyAction _003C_003E4__this;

			public float delay;

			public bool replaceWithBomb;

			public float duration;

			private TransformBehaviour _003Cbeh_003E5__2;

			private float _003Ctime_003E5__3;

			private Vector3 _003Cdirection_003E5__4;

			private Vector3 _003CstartPostion_003E5__5;

			private float _003Camplitude_003E5__6;

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
			public _003CShakeChip_003Ed__15(int _003C_003E1__state)
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
				DiscoBallDestroyAction discoBallDestroyAction = _003C_003E4__this;
				Slot lastConnectedSlot;
				float num2;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Cbeh_003E5__2 = chip.GetComponentBehaviour<TransformBehaviour>();
					if (_003Cbeh_003E5__2 == null)
					{
						return false;
					}
					_003Ctime_003E5__3 = 0f;
					goto IL_0081;
				case 1:
					_003C_003E1__state = -1;
					goto IL_0081;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_0081:
					if (_003Ctime_003E5__3 < delay)
					{
						_003Ctime_003E5__3 += discoBallDestroyAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					lastConnectedSlot = chip.lastConnectedSlot;
					if (replaceWithBomb)
					{
						chip.RemoveFromGame();
						ChipType chipType = discoBallDestroyAction.discoParams.bombType;
						if (chipType == ChipType.HorizontalRocket || chipType == ChipType.VerticalRocket)
						{
							chipType = ((discoBallDestroyAction.discoParams.game.RandomRange(0, 100) < 50) ? ChipType.VerticalRocket : ChipType.HorizontalRocket);
						}
						chip = discoBallDestroyAction.discoParams.game.CreatePowerupInSlot(lastConnectedSlot, chipType);
						chip.AddLock(discoBallDestroyAction.slotComponentLock);
						_003Cbeh_003E5__2 = chip.GetComponentBehaviour<TransformBehaviour>();
					}
					_003Cdirection_003E5__4 = lastConnectedSlot.localPositionOfCenter - discoBallDestroyAction.discoParams.originSlot.localPositionOfCenter;
					if (_003Cdirection_003E5__4 == Vector3.zero)
					{
						_003Cdirection_003E5__4 = Vector3.up;
					}
					_003Cdirection_003E5__4.Normalize();
					_003CstartPostion_003E5__5 = Vector3.zero;
					_003Camplitude_003E5__6 = discoBallDestroyAction.settings.amplitudeRange.Random();
					num2 = 0f;
					_003Ctime_003E5__3 = 0f;
					break;
				}
				if (_003Ctime_003E5__3 <= duration)
				{
					_003Ctime_003E5__3 += discoBallDestroyAction.deltaTime;
					num2 = discoBallDestroyAction.settings.angleSpeed * _003Ctime_003E5__3;
					Vector3 localOffsetPosition = _003CstartPostion_003E5__5 + _003Cdirection_003E5__4 * (Mathf.Sin(num2 * 57.29578f) * _003Camplitude_003E5__6);
					if (_003Cbeh_003E5__2 != null)
					{
						_003Cbeh_003E5__2.localOffsetPosition = localOffsetPosition;
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				if (_003Cbeh_003E5__2 != null)
				{
					_003Cbeh_003E5__2.localOffsetPosition = Vector3.zero;
				}
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

		private sealed class _003CShakeSlot_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DiscoBallDestroyAction _003C_003E4__this;

			public float delay;

			public Slot slot;

			public bool replaceWithBomb;

			public float duration;

			public float lightIntensityWhenStart;

			private float _003Ctime_003E5__2;

			private TransformBehaviour _003Cbeh_003E5__3;

			private Vector3 _003Cdirection_003E5__4;

			private Vector3 _003CstartPostion_003E5__5;

			private float _003Camplitude_003E5__6;

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
			public _003CShakeSlot_003Ed__16(int _003C_003E1__state)
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
				DiscoBallDestroyAction discoBallDestroyAction = _003C_003E4__this;
				Chip slotComponent;
				IntensityChange intensityChange = default(IntensityChange);
				IntensityChange change;
				IntensityChange change2;
				float num2;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					goto IL_0060;
				case 1:
					_003C_003E1__state = -1;
					goto IL_0060;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_0060:
					if (_003Ctime_003E5__2 < delay)
					{
						_003Ctime_003E5__2 += discoBallDestroyAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					slotComponent = slot.GetSlotComponent<Chip>();
					if (slotComponent == null)
					{
						return false;
					}
					_003Cbeh_003E5__3 = slotComponent.GetComponentBehaviour<TransformBehaviour>();
					if (_003Cbeh_003E5__3 == null)
					{
						return false;
					}
					if (replaceWithBomb)
					{
						slotComponent.RemoveFromGame();
						ChipType chipType = discoBallDestroyAction.discoParams.bombType;
						if (chipType == ChipType.HorizontalRocket || chipType == ChipType.VerticalRocket)
						{
							chipType = ((discoBallDestroyAction.discoParams.game.RandomRange(0, 100) < 50) ? ChipType.VerticalRocket : ChipType.HorizontalRocket);
						}
						slotComponent = discoBallDestroyAction.discoParams.game.CreatePowerupInSlot(slot, chipType);
						slotComponent.AddLock(discoBallDestroyAction.slotComponentLock);
						_003Cbeh_003E5__3 = slotComponent.GetComponentBehaviour<TransformBehaviour>();
					}
					intensityChange = default(IntensityChange);
					intensityChange = intensityChange.Duration(duration);
					intensityChange = intensityChange.EaseCurve(GGMath.Ease.Linear);
					change = intensityChange.IntensityRange(lightIntensityWhenStart, lightIntensityWhenStart);
					intensityChange = default(IntensityChange);
					intensityChange = intensityChange.Delay(delay);
					intensityChange = intensityChange.Duration(0.5f);
					intensityChange = intensityChange.EaseCurve(GGMath.Ease.EaseOutCubic);
					change2 = intensityChange.IntensityRange(lightIntensityWhenStart, 0f);
					slot.light.AddIntensityChange(change);
					slot.light.AddIntensityChange(change2);
					_003Cdirection_003E5__4 = slot.localPositionOfCenter - discoBallDestroyAction.discoParams.originSlot.localPositionOfCenter;
					if (_003Cdirection_003E5__4 == Vector3.zero)
					{
						_003Cdirection_003E5__4 = Vector3.up;
					}
					_003Cdirection_003E5__4.Normalize();
					_003CstartPostion_003E5__5 = Vector3.zero;
					_003Camplitude_003E5__6 = discoBallDestroyAction.settings.amplitudeRange.Random();
					num2 = 0f;
					_003Ctime_003E5__2 = 0f;
					break;
				}
				if (_003Ctime_003E5__2 <= duration)
				{
					_003Ctime_003E5__2 += discoBallDestroyAction.deltaTime;
					num2 = discoBallDestroyAction.settings.angleSpeed * _003Ctime_003E5__2;
					Vector3 localOffsetPosition = _003CstartPostion_003E5__5 + _003Cdirection_003E5__4 * (Mathf.Sin(num2 * 57.29578f) * _003Camplitude_003E5__6);
					if (_003Cbeh_003E5__3 != null)
					{
						_003Cbeh_003E5__3.localOffsetPosition = localOffsetPosition;
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				if (_003Cbeh_003E5__3 != null)
				{
					_003Cbeh_003E5__3.localOffsetPosition = Vector3.zero;
				}
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

		private sealed class _003CDestroyAnimation_003Ed__17 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public DiscoBallDestroyAction _003C_003E4__this;

			private Settings _003Csettings_003E5__2;

			private List<Slot> _003CaffectedSlots_003E5__3;

			private EnumeratorsList _003CenumList_003E5__4;

			private bool _003CisHavingCarpet_003E5__5;

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
			public _003CDestroyAnimation_003Ed__17(int _003C_003E1__state)
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
				DiscoBallDestroyAction discoBallDestroyAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003Csettings_003E5__2 = discoBallDestroyAction.settings;
					_003CaffectedSlots_003E5__3 = discoBallDestroyAction.discoParams.affectedSlotsList;
					float num2 = _003Csettings_003E5__2.initialShakeTime;
					if (_003CaffectedSlots_003E5__3.Count > 0)
					{
						num2 += _003Csettings_003E5__2.delayBetweenAffectedSlots * (float)_003CaffectedSlots_003E5__3.Count + _003Csettings_003E5__2.shakeTimeForSlot;
					}
					num2 = Mathf.Max(_003Csettings_003E5__2.minTotalDuration, num2);
					if (discoBallDestroyAction.discoParams.hasBolts && discoBallDestroyAction.discoParams.isInstant)
					{
						num2 = Mathf.Max(0f, num2 * 0.5f - discoBallDestroyAction.discoParams.affectorDuration);
					}
					else if (discoBallDestroyAction.discoParams.isInstant)
					{
						num2 = 0f;
					}
					_003CenumList_003E5__4 = new EnumeratorsList();
					_003CenumList_003E5__4.Add(discoBallDestroyAction.ShakeChip(discoBallDestroyAction.discoParams.originBomb, 0f, num2, replaceWithBomb: false));
					_003CisHavingCarpet_003E5__5 = discoBallDestroyAction.discoParams.isHavingCarpet;
					for (int i = 0; i < _003CaffectedSlots_003E5__3.Count; i++)
					{
						Slot slot = _003CaffectedSlots_003E5__3[i];
						float num3 = (float)i * _003Csettings_003E5__2.delayBetweenAffectedSlots;
						if (discoBallDestroyAction.discoParams.isInstant)
						{
							num3 = 0f;
						}
						if (slot.canCarpetSpreadFromHere)
						{
							_003CisHavingCarpet_003E5__5 = true;
						}
						_003CenumList_003E5__4.Add(discoBallDestroyAction.ShakeSlot(slot, num3, num2 - num3, discoBallDestroyAction.discoParams.replaceWithBombs, _003Csettings_003E5__2.touchSlotLightIntensity));
					}
					break;
				}
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003CenumList_003E5__4.Update())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				for (int j = 0; j < discoBallDestroyAction.discoParams.affectedSlotsList.Count; j++)
				{
					discoBallDestroyAction.discoParams.affectedSlotsList[j].GetSlotComponent<Chip>()?.RemoveLock(discoBallDestroyAction.slotComponentLock);
				}
				SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
				slotDestroyParams.isHitByBomb = true;
				slotDestroyParams.isHavingCarpet = _003CisHavingCarpet_003E5__5;
				slotDestroyParams.isBombAllowingNeighbourDestroy = true;
				slotDestroyParams.bombType = ChipType.DiscoBall;
				bool flag = !discoBallDestroyAction.discoParams.replaceWithBombs;
				List<Slot> list = new List<Slot>();
				list.AddRange(_003CaffectedSlots_003E5__3);
				list.Add(discoBallDestroyAction.discoParams.originSlot);
				if (_003Csettings_003E5__2.useCameraShake && !discoBallDestroyAction.discoParams.replaceWithBombs)
				{
					discoBallDestroyAction.discoParams.game.ShakeCamera(_003Csettings_003E5__2.cameraShake);
				}
				for (int k = 0; k < list.Count; k++)
				{
					Slot slot2 = list[k];
					if (slot2 == null)
					{
						continue;
					}
					slot2.OnDestroySlot(slotDestroyParams);
					discoBallDestroyAction.AffectOuterCircleWithExplosion(slot2.position, 1, Match3Settings.instance.discoBallDestroyActionSettings.shockWaveOffset);
					if (flag)
					{
						List<Slot> neigbourSlots = slot2.neigbourSlots;
						for (int l = 0; l < neigbourSlots.Count; l++)
						{
							neigbourSlots[l].OnDestroyNeighbourSlot(slot2, slotDestroyParams);
						}
					}
				}
				discoBallDestroyAction.discoParams.game.Play(GGSoundSystem.SFXType.DiscoBallExplode);
				if (discoBallDestroyAction.originChip != null)
				{
					DestroyChipActionGrow destroyChipActionGrow = new DestroyChipActionGrow();
					destroyChipActionGrow.Init(discoBallDestroyAction.originChip, discoBallDestroyAction.originChip.lastConnectedSlot);
					discoBallDestroyAction.discoParams.game.board.actionManager.AddAction(destroyChipActionGrow);
				}
				DiscoBallAffector.RemoveFromGame(discoBallDestroyAction.discoParams.bolts);
				discoBallDestroyAction.stopGeneratorsLock.UnlockAll();
				discoBallDestroyAction.slotLock.UnlockAll();
				discoBallDestroyAction.isAlive = false;
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

		private float deltaTime;

		private DiscoParams discoParams;

		private Lock slotLock;

		private Lock stopGeneratorsLock;

		private SlotComponentLock slotComponentLock;

		private List<Slot> allLockedSlots = new List<Slot>();

		private List<Chip> allAffectedChips = new List<Chip>();

		private IEnumerator destroyAnimation;

		private Chip originChip;

		private Slot otherBombSlot;

		private Settings settings => Match3Settings.instance.discoBallDestroyActionSettings;

		public void Init(DiscoParams discoParams)
		{
			this.discoParams = discoParams;
			allLockedSlots.Add(discoParams.originSlot);
			allLockedSlots.AddRange(discoParams.affectedSlotsList);
			stopGeneratorsLock = lockContainer.NewLock();
			stopGeneratorsLock.isChipGeneratorSuspended = true;
			discoParams.game.AddLockToAllSlots(stopGeneratorsLock);
			slotLock = lockContainer.NewLock();
			slotLock.isSlotGravitySuspended = true;
			slotLock.isChipGravitySuspended = false;
			slotLock.isSlotMatchingSuspended = true;
			slotLock.isAvailableForDiscoBombSuspended = true;
			slotComponentLock = new SlotComponentLock();
			slotComponentLock.isDestroySuspended = true;
			originChip = discoParams.originBomb;
			if (originChip != null)
			{
				originChip.RemoveFromSlot();
			}
			if (discoParams.otherBomb != null)
			{
				discoParams.otherBomb.RemoveFromGame();
				otherBombSlot = discoParams.otherBomb.lastConnectedSlot;
				allLockedSlots.Add(otherBombSlot);
			}
			slotLock.LockSlots(allLockedSlots);
			Match3Game game = discoParams.game;
			allAffectedChips.Clear();
			for (int i = 0; i < discoParams.affectedSlotsList.Count; i++)
			{
				Chip slotComponent = discoParams.affectedSlotsList[i].GetSlotComponent<Chip>();
				if (slotComponent != null)
				{
					allAffectedChips.Add(slotComponent);
					slotComponent.AddLock(slotComponentLock);
				}
			}
		}

		private IEnumerator ShakeChip(Chip chip, float delay, float duration, bool replaceWithBomb)
		{
			return new _003CShakeChip_003Ed__15(0)
			{
				_003C_003E4__this = this,
				chip = chip,
				delay = delay,
				duration = duration,
				replaceWithBomb = replaceWithBomb
			};
		}

		private IEnumerator ShakeSlot(Slot slot, float delay, float duration, bool replaceWithBomb, float lightIntensityWhenStart)
		{
			return new _003CShakeSlot_003Ed__16(0)
			{
				_003C_003E4__this = this,
				slot = slot,
				delay = delay,
				duration = duration,
				replaceWithBomb = replaceWithBomb,
				lightIntensityWhenStart = lightIntensityWhenStart
			};
		}

		private IEnumerator DestroyAnimation()
		{
			return new _003CDestroyAnimation_003Ed__17(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.deltaTime = deltaTime;
			if (destroyAnimation == null)
			{
				destroyAnimation = DestroyAnimation();
			}
			destroyAnimation.MoveNext();
		}

		private void AffectOuterCircleWithExplosion(IntVector2 center, int radius, float shockWaveOffset)
		{
			Match3Game game = discoParams.game;
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
