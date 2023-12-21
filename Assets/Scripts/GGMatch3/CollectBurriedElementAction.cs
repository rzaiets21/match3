using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class CollectBurriedElementAction : BoardAction
	{
		public struct CollectGoalParams
		{
			public Match3Goals.GoalBase goal;

			public Match3Game game;

			public BurriedElementPiece burriedElement;

			public SlotBurriedElement slotBurriedElement;

			public SlotDestroyParams destroyParams;

			public Slot slotToLock;

			public IntVector2 explosionCentre;

			public LevelDefinition.BurriedElement burriedElementDefinition
			{
				get
				{
					if (burriedElement != null)
					{
						return burriedElement.elementDefinition;
					}
					if (slotBurriedElement != null)
					{
						return slotBurriedElement.elementDefinition;
					}
					return null;
				}
			}

			public BurriedElementBehaviour burriedElementBehaviour
			{
				get
				{
					if (burriedElement != null)
					{
						return burriedElement.burriedElementBehaviour;
					}
					if (slotBurriedElement != null)
					{
						return slotBurriedElement.burriedElementBehaviour;
					}
					return null;
				}
			}

			public Quaternion localRotation
			{
				get
				{
					if (burriedElementBehaviour == null)
					{
						return Quaternion.identity;
					}
					return burriedElementBehaviour.rotationTransform.localRotation;
				}
				set
				{
					if (!(burriedElementBehaviour == null))
					{
						burriedElementBehaviour.rotationTransform.localRotation = value;
					}
				}
			}

			public TransformBehaviour transformBehaviour
			{
				get
				{
					if (slotBurriedElement != null)
					{
						return slotBurriedElement.GetComponentBehaviour<TransformBehaviour>();
					}
					if (burriedElement != null)
					{
						return burriedElement.transformBehaviour;
					}
					return null;
				}
			}

			public void RemoveFromGame()
			{
				if (burriedElement != null)
				{
					burriedElement.RemoveFromGame();
				}
				if (slotBurriedElement != null)
				{
					slotBurriedElement.RemoveFromGame();
				}
			}
		}

		[Serializable]
		public class Settings
		{
			public SpriteSortingSettings sortingLayer = new SpriteSortingSettings();

			public SpriteSortingSettings sortingLayerFly = new SpriteSortingSettings();

			public float travelDuration = 1f;

			public float additionalParticlesDuration = 0.5f;

			public float travelSpeed;

			public float maxTime = 10f;

			public float minTime;

			public AnimationCurve travelCurve;

			public float scaleUpScale = 1f;

			public float scaleUpScalEndRange = 1f;

			public float scaleUpDuration = 1f;

			public AnimationCurve scaleUpCurve;

			public AnimationCurve rotationCurve;

			public float timeToLockSlot;

			public float rotationAngle;

			public float scaleUpAngle;

			public float scaleUpAngleRangeEnd;

			public AnimationCurve bombCurve;

			public float distance;

			public bool useColor;

			public Color color;

			public float brightness;

			public float ortoDistance;

			public AnimationCurve ortoCurve;

			public bool useTravelScaleCurve;

			public AnimationCurve travelScaleCurve;

			public bool useJump;

			public float jumpDuration;

			public Vector3 jumpOffset;

			public AnimationCurve jumpTravelCurve;

			public float angleSpeed;

			public float jumpScale1;

			public AnimationCurve jumpScale1Curve;

			public float jumpScale2;

			public AnimationCurve jumpScale2Curve;
		}

		private sealed class _003CScalePart_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectBurriedElementAction _003C_003E4__this;

			private TransformBehaviour _003CtransformBehaviour_003E5__2;

			private Quaternion _003CstartRotation_003E5__3;

			private Quaternion _003CendRotation_003E5__4;

			private Settings _003Csettings_003E5__5;

			private Vector3 _003CstartScale_003E5__6;

			private Vector3 _003CendScale_003E5__7;

			private Vector3 _003ClocalPosition_003E5__8;

			private Vector3 _003Cdirection_003E5__9;

			private float _003CstartAngle_003E5__10;

			private float _003CendAngle_003E5__11;

			private float _003Ctime_003E5__12;

			private float _003Cduration_003E5__13;

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
			public _003CScalePart_003Ed__13(int _003C_003E1__state)
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
				CollectBurriedElementAction collectBurriedElementAction = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					if (!(_003Ctime_003E5__12 < _003Cduration_003E5__13))
					{
						return false;
					}
				}
				else
				{
					_003C_003E1__state = -1;
					_003CtransformBehaviour_003E5__2 = collectBurriedElementAction.collectParams.transformBehaviour;
					_003CstartRotation_003E5__3 = Quaternion.identity;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CstartRotation_003E5__3 = collectBurriedElementAction.collectParams.localRotation;
					}
					_003CendRotation_003E5__4 = Quaternion.identity;
					_003Csettings_003E5__5 = collectBurriedElementAction.settings;
					_003CstartScale_003E5__6 = Vector3.one;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CstartScale_003E5__6 = _003CtransformBehaviour_003E5__2.localScale;
						if (_003Csettings_003E5__5.useColor)
						{
							_003CtransformBehaviour_003E5__2.SetColor(_003Csettings_003E5__5.color);
							_003CtransformBehaviour_003E5__2.SetBrightness(_003Csettings_003E5__5.brightness);
						}
					}
					float num2 = UnityEngine.Random.Range(_003Csettings_003E5__5.scaleUpScale, _003Csettings_003E5__5.scaleUpScalEndRange);
					_003CendScale_003E5__7 = new Vector3(num2, num2, 1f);
					_003ClocalPosition_003E5__8 = Vector3.zero;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003ClocalPosition_003E5__8 = _003CtransformBehaviour_003E5__2.localPosition;
					}
					_003Cdirection_003E5__9 = (_003ClocalPosition_003E5__8 - collectBurriedElementAction.collectParams.game.LocalPositionOfCenter(collectBurriedElementAction.collectParams.explosionCentre)).normalized;
					_003CstartAngle_003E5__10 = 0f;
					_003CendAngle_003E5__11 = UnityEngine.Random.Range(_003Csettings_003E5__5.scaleUpAngle, _003Csettings_003E5__5.scaleUpAngleRangeEnd);
					collectBurriedElementAction.scaleUpAngle = _003CendAngle_003E5__11;
					_003Ctime_003E5__12 = 0f;
					_003Cduration_003E5__13 = _003Csettings_003E5__5.scaleUpDuration;
				}
				_003Ctime_003E5__12 += collectBurriedElementAction.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Cduration_003E5__13, _003Ctime_003E5__12);
				time = _003Csettings_003E5__5.scaleUpCurve.Evaluate(time);
				float t = _003Csettings_003E5__5.rotationCurve.Evaluate(time);
				float t2 = _003Csettings_003E5__5.bombCurve.Evaluate(time);
				Vector3 localScale = Vector3.LerpUnclamped(_003CstartScale_003E5__6, _003CendScale_003E5__7, time);
				float angle = Mathf.Lerp(_003CstartAngle_003E5__10, _003CendAngle_003E5__11, time);
				if (_003CtransformBehaviour_003E5__2 != null)
				{
					_003CtransformBehaviour_003E5__2.localScale = localScale;
					_003CtransformBehaviour_003E5__2.localRotationOffset = Quaternion.AngleAxis(angle, Vector3.forward);
					_003CtransformBehaviour_003E5__2.localPosition = Vector3.LerpUnclamped(_003ClocalPosition_003E5__8, _003ClocalPosition_003E5__8 + _003Cdirection_003E5__9 * _003Csettings_003E5__5.distance, t2);
				}
				collectBurriedElementAction.collectParams.localRotation = Quaternion.LerpUnclamped(_003CstartRotation_003E5__3, _003CendRotation_003E5__4, t);
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

		private sealed class _003CTravelJumpPart_003Ed__15 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectBurriedElementAction _003C_003E4__this;

			private TransformBehaviour _003CtransformBehaviour_003E5__2;

			private Settings _003Csettings_003E5__3;

			private Vector3 _003CstartScale_003E5__4;

			private float _003Cangle_003E5__5;

			private Vector3 _003CstartLocalPos_003E5__6;

			private Vector3 _003CendLocalPos_003E5__7;

			private Vector3 _003CuiEndLocalPos_003E5__8;

			private float _003Ctime_003E5__9;

			private float _003Cduration_003E5__10;

			private Vector3 _003CendScale_003E5__11;

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
			public _003CTravelJumpPart_003Ed__15(int _003C_003E1__state)
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
				CollectBurriedElementAction collectBurriedElementAction = _003C_003E4__this;
				float time;
				float t;
				Vector3 localPosition;
				float t2;
				Vector3 localScale;
				float time2;
				float t3;
				Vector3 localPosition2;
				float t4;
				Vector3 localScale2;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003CtransformBehaviour_003E5__2 = collectBurriedElementAction.collectParams.transformBehaviour;
					_003Csettings_003E5__3 = collectBurriedElementAction.settings;
					Vector3 vector = Vector3.zero;
					_003CstartScale_003E5__4 = Vector3.one;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						vector = _003CtransformBehaviour_003E5__2.localPosition;
						_003CstartScale_003E5__4 = _003CtransformBehaviour_003E5__2.localScale;
					}
					_003Cangle_003E5__5 = _003Csettings_003E5__3.scaleUpAngle;
					_003CstartLocalPos_003E5__6 = vector;
					Match3Game game = collectBurriedElementAction.collectParams.game;
					GoalsPanelGoal goal = game.gameScreen.goalsPanel.GetGoal(collectBurriedElementAction.collectParams.goal);
					_003CendLocalPos_003E5__7 = _003Csettings_003E5__3.jumpOffset + _003CstartLocalPos_003E5__6;
					_003CendLocalPos_003E5__7.z = 0f;
					_003CuiEndLocalPos_003E5__8 = Vector3.zero;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CuiEndLocalPos_003E5__8 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(game.gameScreen.goalsPanel.transform.position);
						if (goal != null)
						{
							_003CuiEndLocalPos_003E5__8 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(goal.transform.position);
						}
					}
					_003CuiEndLocalPos_003E5__8.z = 0f;
					_003Ctime_003E5__9 = 0f;
					_003Cduration_003E5__10 = _003Csettings_003E5__3.jumpDuration;
					float scaleUpAngle = collectBurriedElementAction.scaleUpAngle;
					float rotationAngle = _003Csettings_003E5__3.rotationAngle;
					_003CendScale_003E5__11 = _003Csettings_003E5__3.jumpScale1 * Vector3.one;
					goto IL_01af;
				}
				case 1:
					_003C_003E1__state = -1;
					if (_003Ctime_003E5__9 < _003Cduration_003E5__10)
					{
						goto IL_01af;
					}
					_003CstartScale_003E5__4 = _003CendScale_003E5__11;
					_003CendScale_003E5__11 = _003Csettings_003E5__3.jumpScale2 * Vector3.one;
					_003Ctime_003E5__9 = 0f;
					_003Cduration_003E5__10 = collectBurriedElementAction.TravelDuration(_003CendLocalPos_003E5__7, _003CuiEndLocalPos_003E5__8);
					goto IL_032c;
				case 2:
					{
						_003C_003E1__state = -1;
						if (!(_003Ctime_003E5__9 < _003Cduration_003E5__10))
						{
							return false;
						}
						goto IL_032c;
					}
					IL_01af:
					_003Ctime_003E5__9 += collectBurriedElementAction.deltaTime;
					time = Mathf.InverseLerp(0f, _003Cduration_003E5__10, _003Ctime_003E5__9);
					t = _003Csettings_003E5__3.jumpTravelCurve.Evaluate(time);
					localPosition = Vector3Ex.CatmullRomLerp(_003CstartLocalPos_003E5__6, _003CstartLocalPos_003E5__6, _003CendLocalPos_003E5__7, _003CuiEndLocalPos_003E5__8, t);
					_003Cangle_003E5__5 += _003Csettings_003E5__3.angleSpeed * collectBurriedElementAction.deltaTime;
					t2 = _003Csettings_003E5__3.jumpScale1Curve.Evaluate(time);
					localScale = Vector3.LerpUnclamped(_003CstartScale_003E5__4, _003CendScale_003E5__11, t2);
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CtransformBehaviour_003E5__2.localPosition = localPosition;
						_003CtransformBehaviour_003E5__2.localRotationOffset = Quaternion.AngleAxis(_003Cangle_003E5__5, Vector3.forward);
						_003CtransformBehaviour_003E5__2.localScale = localScale;
					}
					if (collectBurriedElementAction.travelParticles != null)
					{
						collectBurriedElementAction.travelParticles.transform.localPosition = localPosition;
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
					IL_032c:
					_003Ctime_003E5__9 += collectBurriedElementAction.deltaTime;
					time2 = Mathf.InverseLerp(0f, _003Cduration_003E5__10, _003Ctime_003E5__9);
					t3 = _003Csettings_003E5__3.travelCurve.Evaluate(time2);
					_003Cangle_003E5__5 += _003Csettings_003E5__3.angleSpeed * collectBurriedElementAction.deltaTime;
					localPosition2 = Vector3Ex.CatmullRomLerp(_003CstartLocalPos_003E5__6, _003CendLocalPos_003E5__7, _003CuiEndLocalPos_003E5__8, _003CuiEndLocalPos_003E5__8, t3);
					t4 = _003Csettings_003E5__3.jumpScale1Curve.Evaluate(time2);
					localScale2 = Vector3.LerpUnclamped(_003CstartScale_003E5__4, _003CendScale_003E5__11, t4);
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CtransformBehaviour_003E5__2.localPosition = localPosition2;
						_003CtransformBehaviour_003E5__2.localRotationOffset = Quaternion.AngleAxis(_003Cangle_003E5__5, Vector3.forward);
						_003CtransformBehaviour_003E5__2.localScale = localScale2;
					}
					if (collectBurriedElementAction.travelParticles != null)
					{
						collectBurriedElementAction.travelParticles.transform.localPosition = localPosition2;
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

		private sealed class _003CTravelPart_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectBurriedElementAction _003C_003E4__this;

			private TransformBehaviour _003CtransformBehaviour_003E5__2;

			private Settings _003Csettings_003E5__3;

			private Vector3 _003CstartScale_003E5__4;

			private Vector3 _003CstartLocalPos_003E5__5;

			private Vector3 _003CendLocalPos_003E5__6;

			private float _003Ctime_003E5__7;

			private float _003Cduration_003E5__8;

			private float _003CstartAngle_003E5__9;

			private float _003CendAngle_003E5__10;

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
			public _003CTravelPart_003Ed__16(int _003C_003E1__state)
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
				CollectBurriedElementAction collectBurriedElementAction = _003C_003E4__this;
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
					_003CtransformBehaviour_003E5__2 = collectBurriedElementAction.collectParams.transformBehaviour;
					_003Csettings_003E5__3 = collectBurriedElementAction.settings;
					Vector3 vector = Vector3.zero;
					_003CstartScale_003E5__4 = Vector3.one;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						vector = _003CtransformBehaviour_003E5__2.localPosition;
						_003CstartScale_003E5__4 = _003CtransformBehaviour_003E5__2.localScale;
					}
					_003CstartLocalPos_003E5__5 = vector;
					Match3Game game = collectBurriedElementAction.collectParams.game;
					GoalsPanelGoal goal = game.gameScreen.goalsPanel.GetGoal(collectBurriedElementAction.collectParams.goal);
					_003CendLocalPos_003E5__6 = _003CstartLocalPos_003E5__5;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CendLocalPos_003E5__6 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(game.gameScreen.goalsPanel.transform.position);
						if (goal != null)
						{
							_003CendLocalPos_003E5__6 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(goal.transform.position);
						}
					}
					_003CendLocalPos_003E5__6.z = 0f;
					_003Ctime_003E5__7 = 0f;
					_003Cduration_003E5__8 = collectBurriedElementAction.TravelDuration(_003CstartLocalPos_003E5__5, _003CendLocalPos_003E5__6);
					_003CstartAngle_003E5__9 = collectBurriedElementAction.scaleUpAngle;
					_003CendAngle_003E5__10 = _003Csettings_003E5__3.rotationAngle;
				}
				_003Ctime_003E5__7 += collectBurriedElementAction.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Cduration_003E5__8, _003Ctime_003E5__7);
				float t = _003Csettings_003E5__3.travelCurve.Evaluate(time);
				float angle = Mathf.Lerp(_003CstartAngle_003E5__9, _003CendAngle_003E5__10, t);
				Vector3 localPosition = Vector3.LerpUnclamped(_003CstartLocalPos_003E5__5, _003CendLocalPos_003E5__6, t);
				if (_003Csettings_003E5__3.ortoDistance != 0f)
				{
					float t2 = _003Csettings_003E5__3.ortoCurve.Evaluate(time);
					localPosition.y += Mathf.LerpUnclamped(0f, _003Csettings_003E5__3.ortoDistance, t2);
				}
				if (_003Csettings_003E5__3.useTravelScaleCurve)
				{
					float d = _003Csettings_003E5__3.travelScaleCurve.Evaluate(time);
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						_003CtransformBehaviour_003E5__2.localScale = _003CstartScale_003E5__4 * d;
					}
				}
				if (_003CtransformBehaviour_003E5__2 != null)
				{
					_003CtransformBehaviour_003E5__2.localPosition = localPosition;
					_003CtransformBehaviour_003E5__2.localRotationOffset = Quaternion.AngleAxis(angle, Vector3.forward);
				}
				if (collectBurriedElementAction.travelParticles != null)
				{
					collectBurriedElementAction.travelParticles.transform.localPosition = localPosition;
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

		private sealed class _003CDoAnimation_003Ed__17 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectBurriedElementAction _003C_003E4__this;

			private Match3Game _003Cgame_003E5__2;

			private TransformBehaviour _003CtransformBehaviour_003E5__3;

			private IEnumerator _003Canimation_003E5__4;

			private EnumeratorsList _003CenumList_003E5__5;

			private float _003Ctime_003E5__6;

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
			public _003CDoAnimation_003Ed__17(int _003C_003E1__state)
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
				CollectBurriedElementAction collectBurriedElementAction = _003C_003E4__this;
				ParticleSystem component;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					LevelDefinition.BurriedElement burriedElementDefinition = collectBurriedElementAction.collectParams.burriedElementDefinition;
					_003Cgame_003E5__2 = collectBurriedElementAction.collectParams.game;
					collectBurriedElementAction.collectParams.game.Play(GGSoundSystem.SFXType.CollectGoalStart);
					_003CtransformBehaviour_003E5__3 = collectBurriedElementAction.collectParams.transformBehaviour;
					if (_003CtransformBehaviour_003E5__3 != null)
					{
						_003CtransformBehaviour_003E5__3.SetSortingLayer(collectBurriedElementAction.settings.sortingLayer);
						_003CtransformBehaviour_003E5__3.SetAlpha(1f);
						_003Cgame_003E5__2.particles.CreateParticles(_003CtransformBehaviour_003E5__3.localPosition, Match3Particles.PositionType.BurriedElementBreak);
						GameObject gameObject = collectBurriedElementAction.travelParticles = _003Cgame_003E5__2.particles.CreateParticles(_003CtransformBehaviour_003E5__3.localPosition, Match3Particles.PositionType.BurriedElementTravelParticle);
					}
					_003Canimation_003E5__4 = null;
					_003CenumList_003E5__5 = new EnumeratorsList();
					if (collectBurriedElementAction.settings.scaleUpDuration >= 0f)
					{
						_003CenumList_003E5__5.Add(collectBurriedElementAction.ScalePart());
						goto IL_013f;
					}
					goto IL_014c;
				}
				case 1:
					_003C_003E1__state = -1;
					goto IL_013f;
				case 2:
					_003C_003E1__state = -1;
					goto IL_01a2;
				case 3:
					_003C_003E1__state = -1;
					goto IL_01d6;
				case 4:
					{
						_003C_003E1__state = -1;
						goto IL_0289;
					}
					IL_01e3:
					collectBurriedElementAction.collectParams.RemoveFromGame();
					_003Cgame_003E5__2.OnPickupGoal(new GoalCollectParams(collectBurriedElementAction.collectParams.goal, collectBurriedElementAction.collectParams.destroyParams));
					collectBurriedElementAction.globalLock.UnlockAll();
					if (!(collectBurriedElementAction.travelParticles != null))
					{
						break;
					}
					component = collectBurriedElementAction.travelParticles.GetComponent<ParticleSystem>();
					if (component != null)
					{
						var componentEmission = component.emission;
						componentEmission.enabled = false;
					}
					_003Ctime_003E5__6 = 0f;
					goto IL_0289;
					IL_0289:
					if (_003Ctime_003E5__6 < collectBurriedElementAction.settings.additionalParticlesDuration)
					{
						_003Ctime_003E5__6 += collectBurriedElementAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 4;
						return true;
					}
					UnityEngine.Object.Destroy(collectBurriedElementAction.travelParticles);
					break;
					IL_01a2:
					if (_003Canimation_003E5__4.MoveNext())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_01e3;
					IL_013f:
					if (_003CenumList_003E5__5.Update())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_014c;
					IL_014c:
					if (_003CtransformBehaviour_003E5__3 != null)
					{
						_003CtransformBehaviour_003E5__3.SetSortingLayer(collectBurriedElementAction.settings.sortingLayerFly);
					}
					if (collectBurriedElementAction.settings.useJump)
					{
						_003Canimation_003E5__4 = collectBurriedElementAction.TravelJumpPart();
						goto IL_01a2;
					}
					_003Canimation_003E5__4 = collectBurriedElementAction.TravelPart();
					goto IL_01d6;
					IL_01d6:
					if (_003Canimation_003E5__4.MoveNext())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 3;
						return true;
					}
					goto IL_01e3;
				}
				collectBurriedElementAction.isAlive = false;
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

		private GameObject travelParticles;

		private CollectGoalParams collectParams;

		private float deltaTime;

		private IEnumerator animationEnum;

		private Lock globalLock;

		private float lockedTime;

		private bool isUnlocked;

		private float scaleUpAngle;

		private Settings settings => Match3Settings.instance.collectBurriedEelementSettings;

		public void Init(CollectGoalParams collectParams)
		{
			this.collectParams = collectParams;
			globalLock = lockContainer.NewLock();
			globalLock.isSlotGravitySuspended = true;
			globalLock.isChipGeneratorSuspended = true;
			globalLock.LockSlot(collectParams.slotToLock);
		}

		private IEnumerator ScalePart()
		{
			return new _003CScalePart_003Ed__13(0)
			{
				_003C_003E4__this = this
			};
		}

		private float TravelDuration(Vector3 startPos, Vector3 endPos)
		{
			float result = settings.travelDuration;
			if (settings.travelSpeed > 0f)
			{
				result = Vector3.Distance(startPos, endPos) / settings.travelSpeed;
				result = Mathf.Clamp(result, settings.minTime, settings.maxTime);
			}
			return result;
		}

		private IEnumerator TravelJumpPart()
		{
			return new _003CTravelJumpPart_003Ed__15(0)
			{
				_003C_003E4__this = this
			};
		}

		private IEnumerator TravelPart()
		{
			return new _003CTravelPart_003Ed__16(0)
			{
				_003C_003E4__this = this
			};
		}

		private IEnumerator DoAnimation()
		{
			return new _003CDoAnimation_003Ed__17(0)
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
