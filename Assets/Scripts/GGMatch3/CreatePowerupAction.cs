using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class CreatePowerupAction : BoardAction
	{
		public struct CreateParams
		{
			public Match3Game game;

			public IntVector2 positionWherePowerupWillBeCreated;

			public ChipType powerupToCreate;

			public int addCoins;
		}

		[Serializable]
		public class Settings
		{
			public bool useParticles;

			public float chipStartScale;

			public float chipEndScale;

			public float chipStartAlpha;

			public float chipEndAlpha;

			public float durationForChip;

			public float durationForGoalMin;

			public float durationForGoalMax = 1f;

			public AnimationCurve positionCurve;

			public float lightIntensity = 0.4f;

			public float lightDuration = 1f;

			public float goalDelay = 0.1f;

			public bool skipScale;

			public float delayForPowerup;

			public float durationForPowerup;

			public AnimationCurve powerupCurve;

			public float startScale = 2f;

			public float startAlpha = 0.5f;

			public int lightRadius = 1;

			public float powerupLigtIntensity = 1f;

			public float additionalKeepLock = 0.3f;

			public float holdGravityDuration = 0.4f;
		}

		public class ChipToMoveDescriptor
		{
			public Chip chip;

			public LightingBolt bolt;
		}

		private sealed class _003CPowerupCreation_003Ed__14 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CreatePowerupAction _003C_003E4__this;

			private float _003Ctime_003E5__2;

			private Match3Game _003Cgame_003E5__3;

			private Slot _003CpowerupSlot_003E5__4;

			private TransformBehaviour _003CchipTransform_003E5__5;

			private float _003Cduration_003E5__6;

			private AnimationCurve _003Ccurve_003E5__7;

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
			public _003CPowerupCreation_003Ed__14(int _003C_003E1__state)
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
				CreatePowerupAction createPowerupAction = _003C_003E4__this;
				Chip chip;
				IntVector2 positionWherePowerupWillBeCreated;
				int lightRadius;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Cgame_003E5__3 = createPowerupAction.createParams.game;
					_003CpowerupSlot_003E5__4 = _003Cgame_003E5__3.GetSlot(createPowerupAction.createParams.positionWherePowerupWillBeCreated);
					_003Cgame_003E5__3.particles.CreateParticles(_003CpowerupSlot_003E5__4, Match3Particles.PositionType.BombCreate);
					_003Cgame_003E5__3.particles.CreateParticles(_003CpowerupSlot_003E5__4, Match3Particles.PositionType.PlacePowerupParticles);
					goto IL_00c3;
				case 1:
					_003C_003E1__state = -1;
					goto IL_00c3;
				case 2:
					_003C_003E1__state = -1;
					goto IL_02a6;
				case 3:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_00c3:
					if (_003Ctime_003E5__2 <= createPowerupAction.settings.delayForPowerup)
					{
						_003Ctime_003E5__2 += createPowerupAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					_003CpowerupSlot_003E5__4.GetSlotComponent<Chip>()?.RemoveFromGame();
					chip = _003Cgame_003E5__3.CreatePowerupInSlot(_003CpowerupSlot_003E5__4, createPowerupAction.createParams.powerupToCreate);
					chip.carriesCoins = createPowerupAction.createParams.addCoins;
					positionWherePowerupWillBeCreated = createPowerupAction.createParams.positionWherePowerupWillBeCreated;
					lightRadius = createPowerupAction.settings.lightRadius;
					for (int i = positionWherePowerupWillBeCreated.x - lightRadius; i <= positionWherePowerupWillBeCreated.x + lightRadius; i++)
					{
						for (int j = positionWherePowerupWillBeCreated.y - lightRadius; j <= positionWherePowerupWillBeCreated.y + lightRadius; j++)
						{
							_003Cgame_003E5__3.GetSlot(new IntVector2(i, j))?.light.AddLight(createPowerupAction.settings.powerupLigtIntensity);
						}
					}
					_003CchipTransform_003E5__5 = chip.GetComponentBehaviour<TransformBehaviour>();
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__6 = createPowerupAction.settings.durationForPowerup;
					_003Ccurve_003E5__7 = createPowerupAction.settings.powerupCurve;
					goto IL_02a6;
					IL_02a6:
					if (_003Ctime_003E5__2 <= _003Cduration_003E5__6)
					{
						_003Ctime_003E5__2 += createPowerupAction.deltaTime;
						float time = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__2);
						time = _003Ccurve_003E5__7.Evaluate(time);
						Vector3 localScale = Vector3.LerpUnclamped(new Vector3(createPowerupAction.settings.startScale, createPowerupAction.settings.startScale, 1f), Vector3.one, time);
						float alpha = Mathf.Lerp(createPowerupAction.settings.startAlpha, 1f, time);
						if (_003CchipTransform_003E5__5 != null)
						{
							_003CchipTransform_003E5__5.localScale = localScale;
							_003CchipTransform_003E5__5.SetAlpha(alpha);
						}
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					_003Ctime_003E5__2 = 0f;
					break;
				}
				if (_003Ctime_003E5__2 <= createPowerupAction.settings.additionalKeepLock)
				{
					_003Ctime_003E5__2 += createPowerupAction.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 3;
					return true;
				}
				createPowerupAction.powerupCreateLock.UnlockAll();
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

		private sealed class _003CCreatePowerupAnimation_003Ed__15 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CreatePowerupAction _003C_003E4__this;

			private EnumeratorsList _003CenumList_003E5__2;

			private float _003Ctime_003E5__3;

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
			public _003CCreatePowerupAnimation_003Ed__15(int _003C_003E1__state)
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
				CreatePowerupAction createPowerupAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003CenumList_003E5__2 = new EnumeratorsList();
					_003Ctime_003E5__3 = 0f;
					float lightIntensity = createPowerupAction.settings.lightIntensity;
					float a = 0f;
					for (int i = 0; i < createPowerupAction.chipsToMove.Count; i++)
					{
						ChipToMoveDescriptor chipToMoveDescriptor = createPowerupAction.chipsToMove[i];
						a = Mathf.Max(a, createPowerupAction.GetDistance(chipToMoveDescriptor.chip));
					}
					for (int j = 0; j < createPowerupAction.chipsToMove.Count; j++)
					{
						ChipToMoveDescriptor chipDescriptor = createPowerupAction.chipsToMove[j];
						_003CenumList_003E5__2.Add(createPowerupAction.MoveSingleChip(chipDescriptor, j));
					}
					_003CenumList_003E5__2.Add(createPowerupAction.PowerupCreation());
					goto IL_0133;
				}
				case 1:
					_003C_003E1__state = -1;
					goto IL_0133;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_0133:
					if (_003CenumList_003E5__2.Update())
					{
						_003Ctime_003E5__3 += Time.deltaTime;
						if (_003Ctime_003E5__3 > createPowerupAction.settings.holdGravityDuration)
						{
							createPowerupAction.slotLock.UnlockAll();
						}
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					break;
				}
				if (_003Ctime_003E5__3 < createPowerupAction.settings.holdGravityDuration)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				createPowerupAction.slotLock.UnlockAll();
				createPowerupAction.powerupCreateLock.UnlockAll();
				createPowerupAction.isAlive = false;
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

		private sealed class _003CMoveSingleChip_003Ed__18 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ChipToMoveDescriptor chipDescriptor;

			public CreatePowerupAction _003C_003E4__this;

			public int index;

			private Chip _003Cchip_003E5__2;

			private LightingBolt _003Cbolt_003E5__3;

			private Match3Game _003Cgame_003E5__4;

			private TransformBehaviour _003CchipTransform_003E5__5;

			private Vector3 _003CstartPosition_003E5__6;

			private Vector3 _003CendPosition_003E5__7;

			private float _003Ctime_003E5__8;

			private float _003Cduration_003E5__9;

			private AnimationCurve _003CpositionCurve_003E5__10;

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
			public _003CMoveSingleChip_003Ed__18(int _003C_003E1__state)
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
				CreatePowerupAction createPowerupAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Cchip_003E5__2 = chipDescriptor.chip;
					_003Cbolt_003E5__3 = chipDescriptor.bolt;
					_003Cgame_003E5__4 = createPowerupAction.createParams.game;
					_003CchipTransform_003E5__5 = _003Cchip_003E5__2.GetComponentBehaviour<TransformBehaviour>();
					_003CstartPosition_003E5__6 = _003Cchip_003E5__2.lastConnectedSlot.localPositionOfCenter;
					if (_003CchipTransform_003E5__5 != null)
					{
						_003CstartPosition_003E5__6 = _003CchipTransform_003E5__5.localPosition;
					}
					_003CendPosition_003E5__7 = createPowerupAction.createParams.game.LocalPositionOfCenter(createPowerupAction.createParams.positionWherePowerupWillBeCreated);
					_003Ctime_003E5__8 = 0f;
					_003Cduration_003E5__9 = createPowerupAction.settings.durationForChip;
					_003CpositionCurve_003E5__10 = createPowerupAction.settings.positionCurve;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__8 <= _003Cduration_003E5__9)
				{
					_003Ctime_003E5__8 += createPowerupAction.deltaTime;
					float time = Mathf.InverseLerp(0f, _003Cduration_003E5__9, _003Ctime_003E5__8);
					time = _003CpositionCurve_003E5__10.Evaluate(time);
					float alpha = Mathf.Lerp(createPowerupAction.settings.chipStartAlpha, createPowerupAction.settings.chipEndAlpha, time);
					Vector3 vector = Vector3.LerpUnclamped(_003CstartPosition_003E5__6, _003CendPosition_003E5__7, time);
					Vector3 localScale = Vector3.Lerp(new Vector3(createPowerupAction.settings.chipStartScale, createPowerupAction.settings.chipStartScale, 1f), new Vector3(createPowerupAction.settings.chipEndScale, createPowerupAction.settings.chipEndScale, 1f), time);
					if (_003CchipTransform_003E5__5 != null)
					{
						_003CchipTransform_003E5__5.localPosition = vector;
						_003CchipTransform_003E5__5.localScale = localScale;
						_003CchipTransform_003E5__5.SetAlpha(alpha);
					}
					if (_003Cbolt_003E5__3 != null)
					{
						_003Cbolt_003E5__3.SetEndPosition(vector);
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				if (_003Cbolt_003E5__3 != null)
				{
					_003Cbolt_003E5__3.RemoveFromGame();
				}
				Match3Goals.GoalBase goalBase = null;
				if (_003Cchip_003E5__2.hasGrowingElement)
				{
					Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
					chipTypeDef.chipType = ChipType.GrowingElementPiece;
					chipTypeDef.itemColor = _003Cchip_003E5__2.itemColor;
					goalBase = _003Cgame_003E5__4.goals.GetActiveGoal(chipTypeDef);
				}
				if (goalBase == null)
				{
					Match3Goals.ChipTypeDef chipTypeDef2 = Match3Goals.ChipTypeDef.Create(_003Cchip_003E5__2);
					goalBase = _003Cgame_003E5__4.goals.GetActiveGoal(chipTypeDef2);
				}
				if (goalBase == null)
				{
					_003Cchip_003E5__2.RemoveFromGame();
					return false;
				}
				if (_003Cchip_003E5__2.chipType == ChipType.Chip && _003Cchip_003E5__2.isFeatherShown)
				{
					Slot slot = _003Cgame_003E5__4.GetSlot(createPowerupAction.createParams.positionWherePowerupWillBeCreated);
					slot = _003Cchip_003E5__2.lastConnectedSlot;
					_003Cgame_003E5__4.particles.CreateParticles(slot.localPositionOfCenter, Match3Particles.PositionType.OnDestroyChip, _003Cchip_003E5__2.chipType, _003Cchip_003E5__2.itemColor);
					CollectGoalAction collectGoalAction = new CollectGoalAction();
					CollectGoalAction.CollectGoalParams collectParams = default(CollectGoalAction.CollectGoalParams);
					collectParams.chip = null;
					collectParams.chipSlot = slot;
					collectParams.moveTransform = _003Cgame_003E5__4.CreateChipFeather(slot, _003Cchip_003E5__2.itemColor);
					collectParams.game = _003Cgame_003E5__4;
					collectParams.goal = goalBase;
					collectParams.skipScale = createPowerupAction.settings.skipScale;
					collectParams.smallScale = true;
					collectGoalAction.Init(collectParams);
					_003Cgame_003E5__4.board.actionManager.AddAction(collectGoalAction);
					_003Cchip_003E5__2.RemoveFromGame();
				}
				else
				{
					CollectGoalAction collectGoalAction2 = new CollectGoalAction();
					CollectGoalAction.CollectGoalParams collectParams2 = default(CollectGoalAction.CollectGoalParams);
					collectParams2.chip = _003Cchip_003E5__2;
					collectParams2.chipSlot = _003Cchip_003E5__2.lastConnectedSlot;
					collectParams2.game = _003Cgame_003E5__4;
					collectParams2.goal = goalBase;
					collectParams2.delay = (float)index * createPowerupAction.settings.goalDelay;
					collectParams2.skipScale = createPowerupAction.settings.skipScale;
					collectParams2.smallScale = true;
					collectGoalAction2.Init(collectParams2);
					_003Cgame_003E5__4.board.actionManager.AddAction(collectGoalAction2);
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

		private CreateParams createParams;

		private List<ChipToMoveDescriptor> chipsToMove = new List<ChipToMoveDescriptor>();

		private List<Slot> slots = new List<Slot>();

		private float deltaTime;

		private IEnumerator animation;

		private Lock slotLock;

		private Lock powerupCreateLock;

		public Settings settings => Match3Settings.instance.createPowerupActionSettings;

		public void AddChip(Chip chip, LightingBolt bolt)
		{
			ChipToMoveDescriptor chipToMoveDescriptor = new ChipToMoveDescriptor();
			chipToMoveDescriptor.chip = chip;
			chipToMoveDescriptor.bolt = bolt;
			chipsToMove.Add(chipToMoveDescriptor);
		}

		public void Init(CreateParams createParams)
		{
			this.createParams = createParams;
			Match3Game game = createParams.game;
			slotLock = lockContainer.NewLock();
			slotLock.isSlotGravitySuspended = true;
			slotLock.isChipGeneratorSuspended = true;
			powerupCreateLock = lockContainer.NewLock();
			powerupCreateLock.isDestroySuspended = true;
			powerupCreateLock.isSlotGravitySuspended = true;
			powerupCreateLock.isSlotMatchingSuspended = true;
			powerupCreateLock.isSlotSwipeSuspended = true;
			powerupCreateLock.isAttachGrowingElementSuspended = true;
			for (int i = 0; i < chipsToMove.Count; i++)
			{
				Chip chip = chipsToMove[i].chip;
				chip.RemoveFromSlot();
				slots.Add(chip.lastConnectedSlot);
			}
			slotLock.LockSlots(slots);
			float lightIntensity = settings.lightIntensity;
			for (int j = 0; j < chipsToMove.Count; j++)
			{
				chipsToMove[j].chip.lastConnectedSlot?.light.AddLightWithDuration(lightIntensity, settings.lightDuration);
			}
			if (settings.useParticles)
			{
				for (int k = 0; k < chipsToMove.Count; k++)
				{
					Chip chip2 = chipsToMove[k].chip;
					game.particles.CreateParticles(chip2, Match3Particles.PositionType.OnDestroyChip, chip2.chipType, chip2.itemColor);
				}
			}
			Slot slot = game.GetSlot(createParams.positionWherePowerupWillBeCreated);
			powerupCreateLock.LockSlot(slot);
			game.Play(GGSoundSystem.SFXType.CreatePowerup);
		}

		private IEnumerator PowerupCreation()
		{
			return new _003CPowerupCreation_003Ed__14(0)
			{
				_003C_003E4__this = this
			};
		}

		private IEnumerator CreatePowerupAnimation()
		{
			return new _003CCreatePowerupAnimation_003Ed__15(0)
			{
				_003C_003E4__this = this
			};
		}

		private float GetDistance(Chip chip)
		{
			IntVector2 intVector = createParams.positionWherePowerupWillBeCreated - chip.lastConnectedSlot.position;
			return Mathf.Max(Mathf.Abs(intVector.x), Mathf.Abs(intVector.y));
		}

		private IEnumerator MoveSingleChip(ChipToMoveDescriptor chipDescriptor, int index)
		{
			return new _003CMoveSingleChip_003Ed__18(0)
			{
				_003C_003E4__this = this,
				chipDescriptor = chipDescriptor,
				index = index
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.deltaTime = deltaTime;
			if (animation == null)
			{
				animation = CreatePowerupAnimation();
			}
			animation.MoveNext();
		}
	}
}
