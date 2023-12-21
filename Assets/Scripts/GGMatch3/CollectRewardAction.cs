using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace GGMatch3
{
    public class CollectRewardAction : BoardAction
    {
        private sealed class _003CDoAnimation_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectRewardAction _003C_003E4__this;

			private GameScreen _003Cgame_003E5__3;

			private TransformBehaviour _003CtransformBehaviour_003E5__4;

			private IEnumerator _003Canimation_003E5__5;

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
				CollectRewardAction collectGoalAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Cgame_003E5__3 = collectGoalAction.game;
					_003CtransformBehaviour_003E5__4 = collectGoalAction.transformToMove;
					if (_003CtransformBehaviour_003E5__4 != null)
					{
						_003CtransformBehaviour_003E5__4.SetSortingLayer(collectGoalAction.settings.sortingLayer);
						_003CtransformBehaviour_003E5__4.gameObject.SetActive(true);
					}
					_003Canimation_003E5__5 = null;
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
				if (_003CtransformBehaviour_003E5__4 != null)
				{
					_003CtransformBehaviour_003E5__4.gameObject.SetActive(false);
					_003CtransformBehaviour_003E5__4.localPosition = collectGoalAction.startPosition;
				}
				_003Cgame_003E5__3.OnPickUpReward();
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
        
        private sealed class _003CTravelPart_003Ed__15 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectRewardAction _003C_003E4__this;

			private TransformBehaviour _003CtransformBehaviour_003E5__2;

			private CollectGoalAction.Settings _003Csettings_003E5__3;

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
				CollectRewardAction collectGoalAction = _003C_003E4__this;
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
					_003CtransformBehaviour_003E5__2 = collectGoalAction.transformToMove;
					_003Csettings_003E5__3 = collectGoalAction.settings;
					Vector3 vector = Vector3.zero;
					_003CstartScale_003E5__4 = Vector3.one;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						vector = _003CtransformBehaviour_003E5__2.localPosition;
						_003CstartScale_003E5__4 = _003CtransformBehaviour_003E5__2.localScale;
					}
					_003CstartLocalPos_003E5__5 = vector;
					GameScreen game = collectGoalAction.game;
					CurrencyType currencyType = collectGoalAction.currencyType;
					CurrencyDisplay currencyDisplay = game.CurrencyPanel.DisplayForCurrency(currencyType);
					_003CendLocalPos_003E5__6 = _003CstartLocalPos_003E5__5;
					if (_003CtransformBehaviour_003E5__2 != null)
					{
						//_003CtransformBehaviour_003E5__2.Disable();
						_003CendLocalPos_003E5__6 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(currencyDisplay.GetTransform().position);
						if (currencyDisplay != null)
						{
							_003CendLocalPos_003E5__6 = _003CtransformBehaviour_003E5__2.WorldToLocalPosition(currencyDisplay.GetTransform().position);
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

        private TransformBehaviour transformToMove;
        private GameScreen game;
        private CurrencyType currencyType;
        private Vector3 startPosition;
        
        private float deltaTime;

        private IEnumerator animationEnum;

        private SlotComponentLock chipLock;

        private float lockedTime;

        public bool isUnlocked;

        private CollectGoalAction.Settings settings => Match3Settings.instance.collectGoalSettings;
        
        private float TravelDuration(Vector3 startPos, Vector3 endPos)
        {
	        return 0.9f;
	        float result = settings.travelDuration;
	        if (settings.travelSpeed > 0f)
	        {
		        result = Vector3.Distance(startPos, endPos) / 150f;
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

        public void Init(TransformBehaviour transformBehaviour, CurrencyType currencyType, GameScreen game)
        {
	        transformToMove = transformBehaviour;
	        this.currencyType = currencyType;
	        this.game = game;
	        
	        startPosition = transformToMove.localPosition;
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
	        }
        }
    }
}