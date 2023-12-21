using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class AnimateCarryPiece : BoardAction
	{
		public struct InitArguments
		{
			public Match3Game game;

			public ChipType chipType;

			public IntVector2 originPosition;

			public Slot destinationSlot;
		}

		[Serializable]
		public class Settings
		{
			public SpriteSortingSettings sortingLayer = new SpriteSortingSettings();

			public SpriteSortingSettings sortingLayerFly = new SpriteSortingSettings();

			public float travelDuration = 1f;

			public AnimationCurve travelCurve;

			public float startScale = 0.5f;

			public float scaleUpScale = 1f;

			public float scaleUpDuration = 1f;

			public AnimationCurve scaleUpCurve;

			public AnimationCurve rotationCurve;

			public float bombDuration;

			public AnimationCurve bombCurve;

			public float distanceWithMagicHat;
		}

		private sealed class _003CBombAffectPart_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public AnimateCarryPiece _003C_003E4__this;

			public TransformBehaviour transformBehaviour;

			public float distance;

			private Settings _003Csettings_003E5__2;

			private Vector3 _003Cdirection_003E5__3;

			private Vector3 _003CstartPosition_003E5__4;

			private float _003Ctime_003E5__5;

			private float _003Cduration_003E5__6;

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
			public _003CBombAffectPart_003Ed__9(int _003C_003E1__state)
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
				AnimateCarryPiece animateCarryPiece = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					if (!(_003Ctime_003E5__5 < _003Cduration_003E5__6))
					{
						return false;
					}
				}
				else
				{
					_003C_003E1__state = -1;
					_003Csettings_003E5__2 = animateCarryPiece.settings;
					Vector3 vector = animateCarryPiece.initArguments.game.LocalPositionOfCenter(animateCarryPiece.initArguments.originPosition);
					_003Cdirection_003E5__3 = Vector3.up;
					_003CstartPosition_003E5__4 = vector;
					_003Ctime_003E5__5 = 0f;
					_003Cduration_003E5__6 = _003Csettings_003E5__2.bombDuration;
				}
				_003Ctime_003E5__5 += animateCarryPiece.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__5);
				time = _003Csettings_003E5__2.bombCurve.Evaluate(time);
				if (transformBehaviour != null)
				{
					transformBehaviour.localPosition = Vector3.LerpUnclamped(_003CstartPosition_003E5__4, _003CstartPosition_003E5__4 + _003Cdirection_003E5__3 * distance, time);
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

		private sealed class _003CScalePart_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public AnimateCarryPiece _003C_003E4__this;

			public TransformBehaviour transformBehaviour;

			private Settings _003Csettings_003E5__2;

			private Vector3 _003CstartScale_003E5__3;

			private Vector3 _003CendScale_003E5__4;

			private float _003Ctime_003E5__5;

			private float _003Cduration_003E5__6;

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
			public _003CScalePart_003Ed__10(int _003C_003E1__state)
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
				AnimateCarryPiece animateCarryPiece = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					if (!(_003Ctime_003E5__5 < _003Cduration_003E5__6))
					{
						return false;
					}
				}
				else
				{
					_003C_003E1__state = -1;
					Quaternion identity = Quaternion.identity;
					_003Csettings_003E5__2 = animateCarryPiece.settings;
					_003CstartScale_003E5__3 = new Vector3(_003Csettings_003E5__2.startScale, _003Csettings_003E5__2.startScale, 1f);
					_003CendScale_003E5__4 = new Vector3(_003Csettings_003E5__2.scaleUpScale, _003Csettings_003E5__2.scaleUpScale, 1f);
					_003Ctime_003E5__5 = 0f;
					_003Cduration_003E5__6 = _003Csettings_003E5__2.scaleUpDuration;
				}
				_003Ctime_003E5__5 += animateCarryPiece.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__5);
				time = _003Csettings_003E5__2.scaleUpCurve.Evaluate(time);
				_003Csettings_003E5__2.rotationCurve.Evaluate(time);
				Vector3 localScale = Vector3.LerpUnclamped(_003CstartScale_003E5__3, _003CendScale_003E5__4, time);
				if (transformBehaviour != null)
				{
					transformBehaviour.localScale = localScale;
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

		private sealed class _003CTravelPart_003Ed__11 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public AnimateCarryPiece _003C_003E4__this;

			public TransformBehaviour transformBehaviour;

			public Vector3 endLocalPos;

			private Settings _003Csettings_003E5__2;

			private Vector3 _003CstartLocalPos_003E5__3;

			private float _003Ctime_003E5__4;

			private float _003Cduration_003E5__5;

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
			public _003CTravelPart_003Ed__11(int _003C_003E1__state)
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
				AnimateCarryPiece animateCarryPiece = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					if (!(_003Ctime_003E5__4 < _003Cduration_003E5__5))
					{
						return false;
					}
				}
				else
				{
					_003C_003E1__state = -1;
					_003Csettings_003E5__2 = animateCarryPiece.settings;
					Vector3 vector = Vector3.zero;
					if (transformBehaviour != null)
					{
						vector = transformBehaviour.localPosition;
					}
					_003CstartLocalPos_003E5__3 = vector;
					_003Ctime_003E5__4 = 0f;
					_003Cduration_003E5__5 = _003Csettings_003E5__2.travelDuration;
				}
				_003Ctime_003E5__4 += animateCarryPiece.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Cduration_003E5__5, _003Ctime_003E5__4);
				time = _003Csettings_003E5__2.travelCurve.Evaluate(time);
				Vector3 localPosition = Vector3.LerpUnclamped(_003CstartLocalPos_003E5__3, endLocalPos, time);
				if (transformBehaviour != null)
				{
					transformBehaviour.localPosition = localPosition;
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

		private sealed class _003CDoAnimation_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public AnimateCarryPiece _003C_003E4__this;

			private Match3Game _003Cgame_003E5__2;

			private TransformBehaviour _003CtransformBehaviour_003E5__3;

			private IEnumerator _003Canimation_003E5__4;

			private EnumeratorsList _003CenumList_003E5__5;

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
			public _003CDoAnimation_003Ed__12(int _003C_003E1__state)
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
				AnimateCarryPiece animateCarryPiece = _003C_003E4__this;
				Slot destinationSlot;
				switch (num)
				{
				default:
					return false;
				case 0:
				{
					_003C_003E1__state = -1;
					_003Cgame_003E5__2 = animateCarryPiece.initArguments.game;
					_003CtransformBehaviour_003E5__3 = _003Cgame_003E5__2.CreatePieceTypeBehaviour(animateCarryPiece.initArguments.chipType);
					Vector3 localPosition = _003Cgame_003E5__2.LocalPositionOfCenter(animateCarryPiece.initArguments.originPosition);
					if (_003CtransformBehaviour_003E5__3 != null)
					{
						GGUtil.SetActive(_003CtransformBehaviour_003E5__3, active: true);
						_003CtransformBehaviour_003E5__3.SetSortingLayer(animateCarryPiece.settings.sortingLayer);
						_003CtransformBehaviour_003E5__3.SetAlpha(1f);
						_003CtransformBehaviour_003E5__3.localPosition = localPosition;
					}
					_003Canimation_003E5__4 = null;
					_003CenumList_003E5__5 = new EnumeratorsList();
					_003CenumList_003E5__5.Add(animateCarryPiece.ScalePart(_003CtransformBehaviour_003E5__3));
					_003CenumList_003E5__5.Add(animateCarryPiece.BombAffectPart(_003CtransformBehaviour_003E5__3, animateCarryPiece.settings.distanceWithMagicHat));
					goto IL_012f;
				}
				case 1:
					_003C_003E1__state = -1;
					goto IL_012f;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_012f:
					if (_003CenumList_003E5__5.Update())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					if (_003CtransformBehaviour_003E5__3 != null)
					{
						_003CtransformBehaviour_003E5__3.SetSortingLayer(animateCarryPiece.settings.sortingLayerFly);
					}
					destinationSlot = animateCarryPiece.initArguments.destinationSlot;
					_003Canimation_003E5__4 = animateCarryPiece.TravelPart(_003CtransformBehaviour_003E5__3, destinationSlot.localPositionOfCenter);
					break;
				}
				if (_003Canimation_003E5__4.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				animateCarryPiece.slotLock.UnlockAll();
				PlacePowerupAction placePowerupAction = new PlacePowerupAction();
				PlacePowerupAction.Parameters parameters = new PlacePowerupAction.Parameters();
				parameters.game = _003Cgame_003E5__2;
				parameters.internalAnimation = true;
				parameters.powerup = animateCarryPiece.initArguments.chipType;
				parameters.initialDelay = 0f;
				parameters.slotWhereToPlacePowerup = animateCarryPiece.initArguments.destinationSlot;
				placePowerupAction.Init(parameters);
				_003Cgame_003E5__2.board.actionManager.AddAction(placePowerupAction);
				_003Cgame_003E5__2.Play(GGSoundSystem.SFXType.CreatePowerup);
				if (_003CtransformBehaviour_003E5__3 != null)
				{
					_003CtransformBehaviour_003E5__3.ForceRemoveFromGame();
				}
				animateCarryPiece.isAlive = false;
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

		private InitArguments initArguments;

		private float deltaTime;

		private IEnumerator animationEnum;

		private Lock slotLock;

		private Settings settings => Match3Settings.instance.animateCarryPieceSettings;

		public void Init(InitArguments initArguments)
		{
			this.initArguments = initArguments;
			slotLock = lockContainer.NewLock();
			slotLock.SuspendAll();
			slotLock.LockSlot(initArguments.destinationSlot);
		}

		private IEnumerator BombAffectPart(TransformBehaviour transformBehaviour, float distance)
		{
			return new _003CBombAffectPart_003Ed__9(0)
			{
				_003C_003E4__this = this,
				transformBehaviour = transformBehaviour,
				distance = distance
			};
		}

		private IEnumerator ScalePart(TransformBehaviour transformBehaviour)
		{
			return new _003CScalePart_003Ed__10(0)
			{
				_003C_003E4__this = this,
				transformBehaviour = transformBehaviour
			};
		}

		private IEnumerator TravelPart(TransformBehaviour transformBehaviour, Vector3 endLocalPos)
		{
			return new _003CTravelPart_003Ed__11(0)
			{
				_003C_003E4__this = this,
				transformBehaviour = transformBehaviour,
				endLocalPos = endLocalPos
			};
		}

		private IEnumerator DoAnimation()
		{
			return new _003CDoAnimation_003Ed__12(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.deltaTime = deltaTime;
			if (animationEnum == null)
			{
				animationEnum = DoAnimation();
			}
			animationEnum.MoveNext();
		}
	}
}
