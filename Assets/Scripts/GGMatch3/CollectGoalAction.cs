using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GGMatch3
{
	public class CollectGoalAction : BoardAction
	{
		public struct CollectGoalParams
		{
			public Match3Goals.GoalBase goal;

			public MonsterElements.MonsterElementPieces monsterToFeed;

			public Chip chip;

			public TransformBehaviour moveTransform;

			public Match3Game game;

			public Slot chipSlot;

			public List<Slot> otherAffectedSlots;

			public bool isExplosion;

			public bool isMagicHat;

			public IntVector2 explosionCentre;

			public SlotDestroyParams destroyParams;

			public float delay;

			public bool skipScale;

			public bool smallScale;

			public int collectMoreMovesCount;

			public bool isFromRocket
			{
				get
				{
					if (destroyParams == null)
					{
						return false;
					}
					if (destroyParams.isHitByBomb)
					{
						if (destroyParams.bombType != ChipType.HorizontalRocket)
						{
							return destroyParams.bombType == ChipType.VerticalRocket;
						}
						return true;
					}
					return false;
				}
			}

			public TransformBehaviour transformToMove
			{
				get
				{
					if (moveTransform != null)
					{
						return moveTransform;
					}
					if (chip == null)
					{
						return null;
					}
					return chip.GetComponentBehaviour<TransformBehaviour>();
				}
			}

			public bool isCollectMoreMovesChip => collectMoreMovesCount > 0;
		}

		[Serializable]
		public class Settings
		{
			public bool useParticlesForChip;

			public SpriteSortingSettings sortingLayer = new SpriteSortingSettings();

			public SpriteSortingSettings sortingLayerFly = new SpriteSortingSettings();

			public float timeToLockSlot = 0.2f;

			public float travelDuration = 1f;

			public float travelSpeed;

			public AnimationCurve travelCurve;

			public float scaleUpScale = 1f;

			public bool useSmallEndScale;

			public float smallEndScale;

			public float smallScale;

			public float scaleUpDuration = 1f;

			public AnimationCurve scaleUpCurve;

			public float distanceWithSwap;

			public float distanceWithBomb;

			public float distanceWithMagicHat;

			public AnimationCurve bombCurve;

			public float bombDuration;

			public float orthoDistance;

			public AnimationCurve orthoCurve;

			public bool useScaleCurve;

			public AnimationCurve scaleCurve;
		}

		private sealed class _003CScalePart_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectGoalAction _003C_003E4__this;

			private TransformBehaviour _003CtransformBehaviour_003E5__2;

			private Settings _003Csettings_003E5__3;

			private Vector3 _003CstartScale_003E5__4;

			private Vector3 _003CendScale_003E5__5;

			private float _003Ctime_003E5__6;

			private float _003Cduration_003E5__7;

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
			public _003CScalePart_003Ed__12(int _003C_003E1__state)
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
				CollectGoalAction collectGoalAction = _003C_003E4__this;
				float time;
				Vector3 localScale;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003CtransformBehaviour_003E5__2 = collectGoalAction.collectParams.transformToMove;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CtransformBehaviour_003E5__2.SetAlpha(1f);
					}
					_003Csettings_003E5__3 = collectGoalAction.settings;
					_003CstartScale_003E5__4 = Vector3.one;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CstartScale_003E5__4 = _003CtransformBehaviour_003E5__2.localScale;
					}
					if (collectGoalAction.collectParams.smallScale)
					{
						_003CstartScale_003E5__4 = new Vector3(_003Csettings_003E5__3.smallScale, _003Csettings_003E5__3.smallScale, 1f);
					}
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CtransformBehaviour_003E5__2.localScale = _003CstartScale_003E5__4;
					}
					_003CendScale_003E5__5 = new Vector3(_003Csettings_003E5__3.scaleUpScale, _003Csettings_003E5__3.scaleUpScale, 1f);
					if (collectGoalAction.collectParams.smallScale && _003Csettings_003E5__3.useSmallEndScale)
					{
						_003CendScale_003E5__5 = new Vector3(_003Csettings_003E5__3.smallEndScale, _003Csettings_003E5__3.smallEndScale, 1f);
					}
					_003Ctime_003E5__6 = 0f;
					if (collectGoalAction.collectParams.delay > 0f)
					{
						goto IL_018f;
					}
					goto IL_01a2;
				case 1:
					_003C_003E1__state = -1;
					goto IL_018f;
				case 2:
					{
						_003C_003E1__state = -1;
						if (!(_003Ctime_003E5__6 < _003Cduration_003E5__7))
						{
							return false;
						}
						goto IL_01ec;
					}
					IL_018f:
					if (_003Ctime_003E5__6 < collectGoalAction.collectParams.delay)
					{
						_003Ctime_003E5__6 += collectGoalAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_01a2;
					IL_01a2:
					if (collectGoalAction.collectParams.skipScale)
					{
						if (_003CtransformBehaviour_003E5__2 != null)
						{
							_003CtransformBehaviour_003E5__2.localScale = _003CendScale_003E5__5;
						}
						return false;
					}
					_003Ctime_003E5__6 = 0f;
					_003Cduration_003E5__7 = _003Csettings_003E5__3.scaleUpDuration;
					goto IL_01ec;
					IL_01ec:
					_003Ctime_003E5__6 += collectGoalAction.deltaTime;
					time = Mathf.InverseLerp(0f, _003Cduration_003E5__7, _003Ctime_003E5__6);
					time = _003Csettings_003E5__3.scaleUpCurve.Evaluate(time);
					localScale = Vector3.LerpUnclamped(_003CstartScale_003E5__4, _003CendScale_003E5__5, time);
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CtransformBehaviour_003E5__2.localScale = localScale;
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
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

		private sealed class _003CBombAffectPart_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectGoalAction _003C_003E4__this;

			public float distance;

			private TransformBehaviour _003CtransformBehaviour_003E5__2;

			private Settings _003Csettings_003E5__3;

			private Vector3 _003Cdirection_003E5__4;

			private Vector3 _003CstartPosition_003E5__5;

			private float _003Ctime_003E5__6;

			private float _003Cduration_003E5__7;

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
			public _003CBombAffectPart_003Ed__13(int _003C_003E1__state)
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
				CollectGoalAction collectGoalAction = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					if (!(_003Ctime_003E5__6 < _003Cduration_003E5__7))
					{
						return false;
					}
				}
				else
				{
					_003C_003E1__state = -1;
					_003CtransformBehaviour_003E5__2 = collectGoalAction.collectParams.transformToMove;
					_003Csettings_003E5__3 = collectGoalAction.settings;
					Vector3 a = Vector3.zero;
					if (collectGoalAction.collectParams.chipSlot != null)
					{
						a = collectGoalAction.collectParams.chipSlot.localPositionOfCenter;
					}
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						a = _003CtransformBehaviour_003E5__2.localPosition;
					}
					_003Cdirection_003E5__4 = (a - collectGoalAction.collectParams.game.LocalPositionOfCenter(collectGoalAction.collectParams.explosionCentre)).normalized;
					_003CstartPosition_003E5__5 = a;
					_003Ctime_003E5__6 = 0f;
					_003Cduration_003E5__7 = _003Csettings_003E5__3.bombDuration;
				}
				_003Ctime_003E5__6 += collectGoalAction.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Cduration_003E5__7, _003Ctime_003E5__6);
				time = _003Csettings_003E5__3.bombCurve.Evaluate(time);
				if (_003CtransformBehaviour_003E5__2 != null)
				{
					_003CtransformBehaviour_003E5__2.localPosition = Vector3.LerpUnclamped(_003CstartPosition_003E5__5, _003CstartPosition_003E5__5 + _003Cdirection_003E5__4 * distance, time);
				}
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
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

		private sealed class _003CTravelPart_003Ed__15 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectGoalAction _003C_003E4__this;

			private TransformBehaviour _003CtransformBehaviour_003E5__2;

			private Settings _003Csettings_003E5__3;

			private Vector3 _003CstartScale_003E5__4;

			private Vector3 _003CstartLocalPos_003E5__5;

			private Vector3 _003CendLocalPos_003E5__6;

			private float _003Ctime_003E5__7;

			private float _003Cduration_003E5__8;

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
			public _003CTravelPart_003Ed__15(int _003C_003E1__state)
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
				CollectGoalAction collectGoalAction = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					if (!(_003Ctime_003E5__7 < _003Cduration_003E5__8))
					{
						return false;
					}
				}
				else
				{
					_003C_003E1__state = -1;
					_003CtransformBehaviour_003E5__2 = collectGoalAction.collectParams.transformToMove;
					_003Csettings_003E5__3 = collectGoalAction.settings;
					Vector3 vector = Vector3.zero;
					_003CstartScale_003E5__4 = Vector3.one;
					if (collectGoalAction.collectParams.chipSlot != null)
					{
						vector = collectGoalAction.collectParams.chipSlot.localPositionOfCenter;
					}
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						vector = _003CtransformBehaviour_003E5__2.localPosition;
						_003CstartScale_003E5__4 = _003CtransformBehaviour_003E5__2.localScale;
					}
					_003CstartLocalPos_003E5__5 = vector;
					Match3Game game = collectGoalAction.collectParams.game;
					GoalsPanelGoal goal = game.gameScreen.goalsPanel.GetGoal(collectGoalAction.collectParams.goal);
					_003CendLocalPos_003E5__6 = _003CstartLocalPos_003E5__5;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						//_003CtransformBehaviour_003E5__2.Disable();
						_003CendLocalPos_003E5__6 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(game.gameScreen.goalsPanel.transform.position);
						if (collectGoalAction.collectParams.monsterToFeed != null)
						{
							_003CendLocalPos_003E5__6 = collectGoalAction.collectParams.monsterToFeed.LocalPositionOfCenter(game);
						}
						else if (collectGoalAction.collectParams.isCollectMoreMovesChip)
						{
							_003CendLocalPos_003E5__6 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(game.gameScreen.goalsPanel.movesCountLabel.transform.position);
						}
						if (goal != null)
						{
							_003CendLocalPos_003E5__6 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(goal.transform.position);
						}
					}
					_003CendLocalPos_003E5__6.z = 0f;
					_003Ctime_003E5__7 = 0f;
					_003Cduration_003E5__8 = collectGoalAction.TravelDuration(_003CstartLocalPos_003E5__5, _003CendLocalPos_003E5__6);
				}
				_003Ctime_003E5__7 += collectGoalAction.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Cduration_003E5__8, _003Ctime_003E5__7);
				float t = _003Csettings_003E5__3.travelCurve.Evaluate(time);
				Vector3 localPosition = Vector3.LerpUnclamped(_003CstartLocalPos_003E5__5, _003CendLocalPos_003E5__6, t);
				if (_003Csettings_003E5__3.orthoDistance != 0f)
				{
					float t2 = _003Csettings_003E5__3.orthoCurve.Evaluate(time);
					localPosition.y += Mathf.LerpUnclamped(0f, _003Csettings_003E5__3.orthoDistance, t2);
				}
				if (_003Csettings_003E5__3.useScaleCurve)
				{
					float d = _003Csettings_003E5__3.scaleCurve.Evaluate(time);
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CtransformBehaviour_003E5__2.localScale = _003CstartScale_003E5__4 * d;
					}
				}
				if (_003CtransformBehaviour_003E5__2 != null)
				{
					_003CtransformBehaviour_003E5__2.localPosition = localPosition;
				}
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
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

		private sealed class _003CDoAnimation_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectGoalAction _003C_003E4__this;

			private Chip _003Cchip_003E5__2;

			private Match3Game _003Cgame_003E5__3;

			private TransformBehaviour _003CtransformBehaviour_003E5__4;

			private IEnumerator _003Canimation_003E5__5;

			private EnumeratorsList _003CenumList_003E5__6;

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
			public _003CDoAnimation_003Ed__16(int _003C_003E1__state)
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
				CollectGoalAction collectGoalAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					if (collectGoalAction.collectParams.collectMoreMovesCount > 0)
					{
						collectGoalAction.collectParams.game.Play(GGSoundSystem.SFXType.CollectMoreMovesStart);
					}
					else
					{
						collectGoalAction.collectParams.game.Play(GGSoundSystem.SFXType.CollectGoalStart);
					}
					_003Cchip_003E5__2 = collectGoalAction.collectParams.chip;
					_003Cgame_003E5__3 = collectGoalAction.collectParams.game;
					_003CtransformBehaviour_003E5__4 = collectGoalAction.collectParams.transformToMove;
					if (_003CtransformBehaviour_003E5__4 != null)
					{
						_003CtransformBehaviour_003E5__4.SetSortingLayer(collectGoalAction.settings.sortingLayer);
					}
					_003Canimation_003E5__5 = null;
					if (collectGoalAction.settings.useParticlesForChip && _003Cchip_003E5__2 != null && _003Cchip_003E5__2.canFormColorMatches && !collectGoalAction.collectParams.isFromRocket && !collectGoalAction.collectParams.skipScale)
					{
						if (collectGoalAction.collectParams.isExplosion)
						{
							_003Cgame_003E5__3.particles.CreateParticles(_003Cchip_003E5__2, Match3Particles.PositionType.OnDestroyChipExplosion, _003Cchip_003E5__2.chipType, _003Cchip_003E5__2.itemColor);
						}
						else
						{
							_003Cgame_003E5__3.particles.CreateParticles(_003Cchip_003E5__2, Match3Particles.PositionType.OnDestroyChip, _003Cchip_003E5__2.chipType, _003Cchip_003E5__2.itemColor);
						}
					}
					_003CenumList_003E5__6 = new EnumeratorsList();
					_003CenumList_003E5__6.Add(collectGoalAction.ScalePart());
					if (collectGoalAction.collectParams.isExplosion)
					{
						_003CenumList_003E5__6.Add(collectGoalAction.BombAffectPart(collectGoalAction.settings.distanceWithBomb));
					}
					else if (collectGoalAction.collectParams.isMagicHat)
					{
						_003CenumList_003E5__6.Add(collectGoalAction.BombAffectPart(collectGoalAction.settings.distanceWithMagicHat));
					}
					else
					{
						_003CenumList_003E5__6.Add(collectGoalAction.BombAffectPart(collectGoalAction.settings.distanceWithSwap));
					}
					goto IL_0237;
				case 1:
					_003C_003E1__state = -1;
					goto IL_0237;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_0237:
					if (_003CenumList_003E5__6.Update())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					if (_003CtransformBehaviour_003E5__4 != null)
					{
						_003CtransformBehaviour_003E5__4.SetSortingLayer(collectGoalAction.settings.sortingLayerFly);
					}
					_003Canimation_003E5__5 = collectGoalAction.TravelPart();
					break;
				}
				if (_003Canimation_003E5__5.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				if (collectGoalAction.collectParams.moveTransform != null)
				{
					collectGoalAction.collectParams.moveTransform.RemoveFromGame();
				}
				if (_003Cchip_003E5__2 != null)
				{
					_003Cchip_003E5__2.RemoveLock(collectGoalAction.chipLock);
					_003Cchip_003E5__2.RemoveFromGame();
				}
				collectGoalAction.globalLock.UnlockAll();
				_003Cgame_003E5__3.OnCollectedMoreMoves(collectGoalAction.collectParams.collectMoreMovesCount);
				_003Cgame_003E5__3.OnPickupGoal(new GoalCollectParams(collectGoalAction.collectParams.goal, collectGoalAction.collectParams.destroyParams));
				if (collectGoalAction.collectParams.monsterToFeed != null)
				{
					collectGoalAction.collectParams.monsterToFeed.OnCollected(_003Cgame_003E5__3);
				}
				collectGoalAction.isAlive = false;
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

		private CollectGoalParams collectParams;

		private float deltaTime;

		private IEnumerator animationEnum;

		private Lock globalLock;

		private SlotComponentLock chipLock;

		private float lockedTime;

		public bool isUnlocked;

		private Settings settings => Match3Settings.instance.collectGoalSettings;

		public void Init(CollectGoalParams collectParams)
		{
			this.collectParams = collectParams;
			globalLock = lockContainer.NewLock();
			globalLock.isSlotGravitySuspended = true;
			globalLock.isChipGeneratorSuspended = true;
			if (collectParams.monsterToFeed != null)
			{
				collectParams.monsterToFeed.OnStartCollectAnimation();
			}
			Chip chip = collectParams.chip;
			if (chip != null)
			{
				chip.RemoveFromSlot();
				chipLock = new SlotComponentLock();
				chipLock.isRemoveFromGameDestroySuspended = true;
				chip.AddLock(chipLock);
			}
			globalLock.LockSlot(collectParams.chipSlot);
			globalLock.LockSlots(collectParams.otherAffectedSlots);
		}

		private IEnumerator ScalePart()
		{
			return new _003CScalePart_003Ed__12(0)
			{
				_003C_003E4__this = this
			};
		}

		private IEnumerator BombAffectPart(float distance)
		{
			return new _003CBombAffectPart_003Ed__13(0)
			{
				_003C_003E4__this = this,
				distance = distance
			};
		}

		private float TravelDuration(Vector3 startPos, Vector3 endPos)
		{
			float result = settings.travelDuration;
			if (settings.travelSpeed > 0f)
			{
				result = Vector3.Distance(startPos, endPos) / settings.travelSpeed;
			}
			return result;
		}

		private IEnumerator TravelPart()
		{
			return new _003CTravelPart_003Ed__15(0)
			{
				_003C_003E4__this = this
			};
		}

		private IEnumerator DoAnimation()
		{
			return new _003CDoAnimation_003Ed__16(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.deltaTime = deltaTime;
			lockedTime += deltaTime;
			if (animationEnum == null)
			{
				animationEnum = DoAnimation();
			}
			animationEnum.MoveNext();
			bool flag = lockedTime > settings.timeToLockSlot;
			if (!isUnlocked && flag)
			{
				isUnlocked = true;
				globalLock.UnlockAll();
			}
		}
	}
}
