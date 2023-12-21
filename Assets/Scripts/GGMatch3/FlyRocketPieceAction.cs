using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class FlyRocketPieceAction : BoardAction
	{
		public struct Params
		{
			public Match3Game game;

			public IntVector2 position;

			public IntVector2 direction;

			public bool prelock;

			public bool isInstant;

			public bool ignoreOriginSlot;

			public bool canUseScale;

			public bool isHavingCarpet;

			public Match3Game.InputAffectorExport affectorExport;
		}

		[Serializable]
		public class Settings
		{
			public FloatRange speedRange;

			public FloatRange accelerationTime;

			public AnimationCurve velocityChangeCurve;

			public float timeScale = 1f;

			public float lightIntensity = 0.5f;

			public FloatRange lightIntensityRange = new FloatRange(1.5f, 0.75f);

			public float maxLightDistance = 6f;

			public float lightDuration = 1f;

			public Vector3 scale = Vector3.one;

			public Vector3 scaleMin = Vector3.one;

			public Vector3 scaleMax = Vector3.one;

			public float initialDisplace;

			public float initialDelay;

			public float additionalTimeToKeepLock;

			public float keepParticlesFor = 2f;

			public int slotsOutside = 5;

			public float shockWaveOffset = 0.02f;

			public float timeAheadDestroy;

			public bool putBoltOnLastSlot;

			public float boltDelay;

			public bool keepBaltsDistance;

			public float minBoltDuration;

			public bool useCameraShake;

			public GeneralSettings.CameraShakeSettings cameraShake = new GeneralSettings.CameraShakeSettings();
		}

		public struct AffectedSlot
		{
			public Slot slot;

			public LightingBolt bolt;
		}

		public struct LockedSlot
		{
			public Slot slot;

			public float timeLocked;

			public bool isUnlocked;
		}

		private sealed class _003CDoFly_003Ed__22 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public FlyRocketPieceAction _003C_003E4__this;

			private float _003Ctime_003E5__2;

			private Match3Game _003Cgame_003E5__3;

			private Vector3 _003CcurrentPositon_003E5__4;

			private Vector3 _003Cdirection_003E5__5;

			private List<Slot> _003CvisitedSlots_003E5__6;

			private IntVector2 _003ClastSlotPosition_003E5__7;

			private SlotDestroyParams _003CdestroyParams_003E5__8;

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
			public _003CDoFly_003Ed__22(int _003C_003E1__state)
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
				FlyRocketPieceAction flyRocketPieceAction = _003C_003E4__this;
				Vector3 localPosition;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					if (flyRocketPieceAction.createdBolt && flyRocketPieceAction.settings.boltDelay > 0f)
					{
						goto IL_0086;
					}
					goto IL_0099;
				case 1:
					_003C_003E1__state = -1;
					goto IL_0086;
				case 2:
					_003C_003E1__state = -1;
					goto IL_01e7;
				case 3:
					_003C_003E1__state = -1;
					goto IL_0253;
				case 4:
					_003C_003E1__state = -1;
					goto IL_07d1;
				case 5:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_0086:
					if (_003Ctime_003E5__2 < flyRocketPieceAction.settings.boltDelay)
					{
						_003Ctime_003E5__2 += flyRocketPieceAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_0099;
					IL_07d1:
					if (_003Ctime_003E5__2 < flyRocketPieceAction.settings.minBoltDuration)
					{
						_003Ctime_003E5__2 += flyRocketPieceAction.deltaTime;
						flyRocketPieceAction.UpdateLockedSlots();
						_003C_003E2__current = null;
						_003C_003E1__state = 4;
						return true;
					}
					goto IL_07e4;
					IL_0099:
					if (flyRocketPieceAction.settings.useCameraShake)
					{
						flyRocketPieceAction.initParams.game.ShakeCamera(flyRocketPieceAction.settings.cameraShake);
					}
					if (flyRocketPieceAction.rocketPiece == null)
					{
						flyRocketPieceAction.rocketPiece = flyRocketPieceAction.CreateRocketPiece();
					}
					flyRocketPieceAction.lockedSlotsList.Clear();
					_003Cgame_003E5__3 = flyRocketPieceAction.initParams.game;
					localPosition = (_003CcurrentPositon_003E5__4 = _003Cgame_003E5__3.LocalPositionOfCenter(flyRocketPieceAction.initParams.position));
					_003Cdirection_003E5__5 = new Vector3(flyRocketPieceAction.initParams.direction.x, flyRocketPieceAction.initParams.direction.y, 0f);
					_003CcurrentPositon_003E5__4 += _003Cdirection_003E5__5 * flyRocketPieceAction.settings.initialDisplace;
					_003CvisitedSlots_003E5__6 = new List<Slot>();
					_003ClastSlotPosition_003E5__7 = flyRocketPieceAction.initParams.position;
					if (flyRocketPieceAction.rocketPiece != null)
					{
						flyRocketPieceAction.rocketPiece.localPosition = localPosition;
						if (!flyRocketPieceAction.initParams.isInstant)
						{
							_003Ctime_003E5__2 = 0f;
							goto IL_01e7;
						}
					}
					goto IL_01fa;
					IL_0253:
					while (true)
					{
						_003Ctime_003E5__2 += flyRocketPieceAction.deltaTime;
						if (flyRocketPieceAction.initParams.isInstant)
						{
							_003Ctime_003E5__2 += 1000f;
						}
						float time = flyRocketPieceAction.settings.accelerationTime.InverseLerp(_003Ctime_003E5__2);
						float t = flyRocketPieceAction.settings.velocityChangeCurve.Evaluate(time);
						float d = flyRocketPieceAction.settings.speedRange.Lerp(t);
						if (flyRocketPieceAction.initParams.canUseScale)
						{
							Vector3 vector = Vector3.Lerp(flyRocketPieceAction.settings.scaleMin, flyRocketPieceAction.settings.scaleMax, t);
							if (flyRocketPieceAction.initParams.direction.y != 0)
							{
								vector.x = vector.y;
								vector.y = vector.x;
							}
							if (flyRocketPieceAction.rocketPiece != null)
							{
								flyRocketPieceAction.rocketPiece.localScale = vector;
							}
						}
						_003CcurrentPositon_003E5__4 += _003Cdirection_003E5__5 * d * flyRocketPieceAction.deltaTime;
						IntVector2 position = _003Cgame_003E5__3.BoardPositionFromLocalPositionRound(_003CcurrentPositon_003E5__4);
						Vector3 vector2 = _003CcurrentPositon_003E5__4 + _003Cdirection_003E5__5 * d * flyRocketPieceAction.settings.timeAheadDestroy;
						IntVector2 intVector = _003Cgame_003E5__3.BoardPositionFromLocalPositionRound(vector2);
						int num2 = Mathf.Min(_003ClastSlotPosition_003E5__7.x, intVector.x);
						int num3 = Mathf.Max(_003ClastSlotPosition_003E5__7.x, intVector.x);
						int num4 = Mathf.Min(_003ClastSlotPosition_003E5__7.y, intVector.y);
						int num5 = Mathf.Max(_003ClastSlotPosition_003E5__7.y, intVector.y);
						bool flag = false;
						for (int i = num2; i <= num3; i++)
						{
							for (int j = num4; j <= num5; j++)
							{
								IntVector2 intVector2 = new IntVector2(i, j);
								Slot slot = _003Cgame_003E5__3.GetSlot(intVector2);
								if (slot == null || _003CvisitedSlots_003E5__6.Contains(slot))
								{
									continue;
								}
								if (slot.canCarpetSpreadFromHere)
								{
									_003CdestroyParams_003E5__8.isHavingCarpet = true;
								}
								if (Vector3.Dot(_003Cdirection_003E5__5, vector2 - slot.localPositionOfCenter) < 0f)
								{
									continue;
								}
								int num6 = Mathf.Abs(intVector2.x - flyRocketPieceAction.initParams.position.x) + Mathf.Abs(intVector2.y - flyRocketPieceAction.initParams.position.y);
								slot.light.AddLightWithDuration(flyRocketPieceAction.settings.lightIntensityRange.Lerp(Mathf.InverseLerp(0f, flyRocketPieceAction.settings.maxLightDistance, num6)), flyRocketPieceAction.settings.lightDuration);
								if (flyRocketPieceAction.settings.additionalTimeToKeepLock > 0f)
								{
									LockedSlot item = default(LockedSlot);
									item.slot = slot;
									flyRocketPieceAction.individualLock.LockSlot(slot);
									flyRocketPieceAction.lockedSlotsList.Add(item);
								}
								if (flyRocketPieceAction.initParams.ignoreOriginSlot && intVector2 == flyRocketPieceAction.initParams.position)
								{
									_003CvisitedSlots_003E5__6.Add(slot);
									continue;
								}
								for (int k = 0; k < flyRocketPieceAction.affectedSlots.Count; k++)
								{
									AffectedSlot affectedSlot = flyRocketPieceAction.affectedSlots[k];
									if (affectedSlot.slot == slot)
									{
										if (affectedSlot.bolt != null && !flyRocketPieceAction.settings.keepBaltsDistance)
										{
											affectedSlot.bolt.RemoveFromGame();
											affectedSlot.bolt = null;
										}
										flyRocketPieceAction.affectedSlots[k] = affectedSlot;
									}
								}
								slot.OnDestroySlot(_003CdestroyParams_003E5__8);
								flyRocketPieceAction.prelock.Unlock(slot);
								_003CvisitedSlots_003E5__6.Add(slot);
								flyRocketPieceAction.ApplyDisplaceAfterDestroy(slot);
								if (_003CdestroyParams_003E5__8.isRocketStopped)
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								break;
							}
						}
						_003ClastSlotPosition_003E5__7 = position;
						if (flyRocketPieceAction.rocketPiece != null)
						{
							flyRocketPieceAction.rocketPiece.localPosition = _003CcurrentPositon_003E5__4;
						}
						for (int l = 0; l < flyRocketPieceAction.affectedSlots.Count; l++)
						{
							AffectedSlot affectedSlot2 = flyRocketPieceAction.affectedSlots[l];
							if (!(affectedSlot2.bolt == null) && !flyRocketPieceAction.settings.keepBaltsDistance)
							{
								affectedSlot2.bolt.SetStartPosition(_003CcurrentPositon_003E5__4 + _003Cdirection_003E5__5 * _003Cgame_003E5__3.slotPhysicalSize.x * 0.5f);
							}
						}
						flyRocketPieceAction.UpdateLockedSlots();
						if (flag || (!_003Cgame_003E5__3.board.IsInBoard(position) && _003Cgame_003E5__3.board.DistanceOutsideBoard(position) >= flyRocketPieceAction.settings.slotsOutside))
						{
							break;
						}
						if (!flyRocketPieceAction.initParams.isInstant)
						{
							_003C_003E2__current = null;
							_003C_003E1__state = 3;
							return true;
						}
					}
					if (flyRocketPieceAction.rocketPiece != null)
					{
						flyRocketPieceAction.rocketPiece.RemoveFromGameAfter(flyRocketPieceAction.settings.keepParticlesFor);
						flyRocketPieceAction.rocketPiece = null;
					}
					if (flyRocketPieceAction.settings.minBoltDuration > 0f)
					{
						goto IL_07d1;
					}
					goto IL_07e4;
					IL_01e7:
					if (_003Ctime_003E5__2 < flyRocketPieceAction.settings.initialDelay)
					{
						_003Ctime_003E5__2 += Time.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_01fa;
					IL_07e4:
					flyRocketPieceAction.ClearAffectedSlots();
					break;
					IL_01fa:
					_003CdestroyParams_003E5__8 = new SlotDestroyParams();
					_003CdestroyParams_003E5__8.isHitByBomb = true;
					_003CdestroyParams_003E5__8.bombType = ((Mathf.Abs(flyRocketPieceAction.initParams.direction.y) != 0) ? ChipType.VerticalRocket : ChipType.HorizontalRocket);
					_003CdestroyParams_003E5__8.isHavingCarpet = flyRocketPieceAction.hasCarpet;
					_003Ctime_003E5__2 = 0f;
					goto IL_0253;
				}
				if (flyRocketPieceAction.UpdateLockedSlots())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 5;
					return true;
				}
				flyRocketPieceAction.individualLock.UnlockAll();
				flyRocketPieceAction.prelock.UnlockAll();
				flyRocketPieceAction.isAlive = false;
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

		private Params initParams;

		private Lock prelock;

		private float deltaTime;

		private IEnumerator animation;

		private RocketPieceBehaviour rocketPiece;

		private List<Slot> pathSlots;

		private Lock individualLock;

		private bool createdBolt;

		private List<AffectedSlot> affectedSlots = new List<AffectedSlot>();

		private List<LockedSlot> lockedSlotsList = new List<LockedSlot>();

		private bool hasCarpet;

		private Settings settings => Match3Settings.instance.flyRocketPieceSettings;

		public void Init(Params initParams)
		{
			this.initParams = initParams;
			individualLock = lockContainer.NewLock();
			individualLock.isSlotGravitySuspended = true;
			prelock = lockContainer.NewLock();
			prelock.isSlotGravitySuspended = true;
			prelock.isChipGravitySuspended = true;
			prelock.isChipGeneratorSuspended = true;
			prelock.isAboutToBeDestroyed = true;
			prelock.isAvailableForDiscoBombSuspended = true;
			Match3Game game = initParams.game;
			pathSlots = GetPathSlots();
			if (initParams.prelock)
			{
				prelock.LockSlots(pathSlots);
			}
			Slot slot = initParams.game.GetSlot(initParams.position);
			hasCarpet = initParams.isHavingCarpet;
			if (slot != null && slot.canCarpetSpreadFromHere)
			{
				hasCarpet = true;
			}
			affectedSlots.Clear();
			Slot slot2 = game.LastSlotOnDirection(slot, initParams.direction);
			IntVector2 intVector = initParams.position;
			while (true)
			{
				Slot slot3 = game.GetSlot(intVector);
				if (!game.board.IsInBoard(intVector))
				{
					break;
				}
				intVector += initParams.direction;
				if (slot3 != null && (!(intVector == initParams.position) || !initParams.ignoreOriginSlot))
				{
					AffectedSlot affectedSlot = default(AffectedSlot);
					affectedSlot.slot = slot3;
					if (initParams.affectorExport != null)
					{
						affectedSlot.bolt = initParams.affectorExport.GetLigtingBoltForSlots(initParams.position, slot3.position);
					}
					if (affectedSlot.slot == slot2 && settings.putBoltOnLastSlot && affectedSlot.bolt == null)
					{
						LightingBolt lightingBolt = game.CreateLightingBoltPowerup();
						lightingBolt.Init(slot, affectedSlot.slot);
						affectedSlot.bolt = lightingBolt;
						createdBolt = true;
					}
					affectedSlots.Add(affectedSlot);
				}
			}
			if (!initParams.ignoreOriginSlot)
			{
				game.particles.CreateParticles(game.LocalPositionOfCenter(initParams.position), Match3Particles.PositionType.OnRocketStart, (initParams.direction.x != 0) ? ChipType.HorizontalRocket : ChipType.VerticalRocket, ItemColor.Unknown);
			}
		}

		private void ClearAffectedSlots()
		{
			for (int i = 0; i < affectedSlots.Count; i++)
			{
				AffectedSlot affectedSlot = affectedSlots[i];
				if (affectedSlot.bolt != null)
				{
					affectedSlot.bolt.RemoveFromGame();
				}
			}
			affectedSlots.Clear();
		}

		private RocketPieceBehaviour CreateRocketPiece()
		{
			Match3Game game = initParams.game;
			RocketPieceBehaviour rocketPieceBehaviour = game.CreateRocketPiece();
			if (rocketPieceBehaviour == null)
			{
				return null;
			}
			Vector3 vector2 = rocketPieceBehaviour.localPosition = game.LocalPositionOfCenter(initParams.position);
			rocketPieceBehaviour.SetDirection(initParams.direction);
			return rocketPieceBehaviour;
		}

		public List<Slot> GetPathSlots()
		{
			List<Slot> list = new List<Slot>();
			Match3Game game = initParams.game;
			IntVector2 position = initParams.position;
			IntVector2 direction = initParams.direction;
			IntVector2 size = game.board.size;
			IntVector2 intVector = position;
			list.Clear();
			while (game.board.IsInBoard(intVector))
			{
				Slot slot = game.board.GetSlot(intVector);
				if (slot != null)
				{
					list.Add(slot);
				}
				intVector += direction;
			}
			return list;
		}

		private bool UpdateLockedSlots()
		{
			float num = deltaTime;
			float additionalTimeToKeepLock = settings.additionalTimeToKeepLock;
			bool result = false;
			for (int i = 0; i < lockedSlotsList.Count; i++)
			{
				LockedSlot lockedSlot = lockedSlotsList[i];
				if (!lockedSlot.isUnlocked)
				{
					lockedSlot.timeLocked += num;
					if (lockedSlot.timeLocked >= additionalTimeToKeepLock)
					{
						lockedSlot.isUnlocked = true;
						individualLock.Unlock(lockedSlot.slot);
					}
					lockedSlotsList[i] = lockedSlot;
					result = true;
				}
			}
			return result;
		}

		private IEnumerator DoFly()
		{
			return new _003CDoFly_003Ed__22(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnStart(ActionManager manager)
		{
			base.OnStart(manager);
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.deltaTime = deltaTime * settings.timeScale;
			if (animation == null)
			{
				animation = DoFly();
			}
			animation.MoveNext();
		}

		private void ApplyDisplaceAfterDestroy(Slot slot)
		{
			IntVector2[] array = IntVector2.leftRight;
			if (Mathf.Abs(initParams.direction.x) > Mathf.Abs(initParams.direction.y))
			{
				array = IntVector2.upDown;
			}
			foreach (IntVector2 b in array)
			{
				Slot slot2 = initParams.game.GetSlot(slot.position + b);
				if (slot2 != null)
				{
					slot2.offsetPosition = (slot2.localPositionOfCenter - slot.localPositionOfCenter).normalized * settings.shockWaveOffset;
					slot2.positionIntegrator.currentPosition = slot2.offsetPosition;
				}
			}
		}
	}
}
