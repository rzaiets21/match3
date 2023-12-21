using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class CollectCoinAction : BoardAction
	{
		public struct InitArguments
		{
			public Match3Game game;

			public Slot chipSlot;

			public float delay;
		}

		private sealed class _003CScalePart_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public TransformBehaviour transformBehaviour;

			public CollectCoinAction _003C_003E4__this;

			private CollectGoalAction.Settings _003Csettings_003E5__2;

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
				CollectCoinAction collectCoinAction = _003C_003E4__this;
				float time;
				Vector3 localScale;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					if (transformBehaviour != null)
					{
						transformBehaviour.SetAlpha(1f);
					}
					_003Csettings_003E5__2 = collectCoinAction.settings;
					_003CstartScale_003E5__3 = Vector3.one;
					if (transformBehaviour != null)
					{
						_003CstartScale_003E5__3 = transformBehaviour.localScale;
					}
					if (transformBehaviour != null)
					{
						transformBehaviour.localScale = _003CstartScale_003E5__3;
					}
					_003CendScale_003E5__4 = new Vector3(_003Csettings_003E5__2.scaleUpScale, _003Csettings_003E5__2.scaleUpScale, 1f);
					_003Ctime_003E5__5 = 0f;
					if (collectCoinAction.initArguments.delay > 0f)
					{
						goto IL_010b;
					}
					goto IL_011e;
				case 1:
					_003C_003E1__state = -1;
					goto IL_010b;
				case 2:
					{
						_003C_003E1__state = -1;
						if (!(_003Ctime_003E5__5 < _003Cduration_003E5__6))
						{
							return false;
						}
						goto IL_013a;
					}
					IL_010b:
					if (_003Ctime_003E5__5 < collectCoinAction.initArguments.delay)
					{
						_003Ctime_003E5__5 += collectCoinAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_011e;
					IL_011e:
					_003Ctime_003E5__5 = 0f;
					_003Cduration_003E5__6 = _003Csettings_003E5__2.scaleUpDuration;
					goto IL_013a;
					IL_013a:
					_003Ctime_003E5__5 += collectCoinAction.deltaTime;
					time = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__5);
					time = _003Csettings_003E5__2.scaleUpCurve.Evaluate(time);
					localScale = Vector3.LerpUnclamped(_003CstartScale_003E5__3, _003CendScale_003E5__4, time);
					if (transformBehaviour != null)
					{
						transformBehaviour.localScale = localScale;
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

		private sealed class _003CTravelPart_003Ed__11 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectCoinAction _003C_003E4__this;

			public TransformBehaviour transformBehaviour;

			private CollectGoalAction.Settings _003Csettings_003E5__2;

			private Vector3 _003CstartLocalPos_003E5__3;

			private Vector3 _003CendLocalPos_003E5__4;

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
				CollectCoinAction collectCoinAction = _003C_003E4__this;
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
					_003Csettings_003E5__2 = collectCoinAction.settings;
					Vector3 vector = Vector3.zero;
					if (collectCoinAction.initArguments.chipSlot != null)
					{
						vector = collectCoinAction.initArguments.chipSlot.localPositionOfCenter;
					}
					if (transformBehaviour != null)
					{
						vector = transformBehaviour.localPosition;
					}
					_003CstartLocalPos_003E5__3 = vector;
					Match3Game game = collectCoinAction.initArguments.game;
					RectTransform coinsTransform = game.gameScreen.goalsPanel.coinsTransform;
					_003CendLocalPos_003E5__4 = _003CstartLocalPos_003E5__3;
					if (transformBehaviour != null)
					{
						_003CendLocalPos_003E5__4 = transformBehaviour.WorldToLocalPosition(game.gameScreen.goalsPanel.transform.position);
						if (coinsTransform != null)
						{
							_003CendLocalPos_003E5__4 = transformBehaviour.WorldToLocalPosition(coinsTransform.transform.position);
						}
					}
					_003CendLocalPos_003E5__4.z = 0f;
					_003Ctime_003E5__5 = 0f;
					_003Cduration_003E5__6 = _003Csettings_003E5__2.travelDuration;
				}
				_003Ctime_003E5__5 += collectCoinAction.deltaTime;
				float time = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__5);
				time = _003Csettings_003E5__2.travelCurve.Evaluate(time);
				Vector3 localPosition = Vector3.LerpUnclamped(_003CstartLocalPos_003E5__3, _003CendLocalPos_003E5__4, time);
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

			public CollectCoinAction _003C_003E4__this;

			private Match3Game _003Cgame_003E5__2;

			private TransformBehaviour _003Ccoin_003E5__3;

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
				CollectCoinAction collectCoinAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Cgame_003E5__2 = collectCoinAction.initArguments.game;
					_003Ccoin_003E5__3 = _003Cgame_003E5__2.CreateCoin();
					_003CtransformBehaviour_003E5__4 = _003Ccoin_003E5__3;
					if (_003CtransformBehaviour_003E5__4 != null)
					{
						_003CtransformBehaviour_003E5__4.localPosition = collectCoinAction.initArguments.chipSlot.localPositionOfCenter;
						_003CtransformBehaviour_003E5__4.SetSortingLayer(collectCoinAction.settings.sortingLayer);
						GGUtil.Show(_003CtransformBehaviour_003E5__4);
					}
					_003Canimation_003E5__5 = null;
					_003Cgame_003E5__2.Play(GGSoundSystem.SFXType.CoinCollectStart);
					_003CenumList_003E5__6 = new EnumeratorsList();
					_003CenumList_003E5__6.Add(collectCoinAction.ScalePart(_003CtransformBehaviour_003E5__4));
					goto IL_00f9;
				case 1:
					_003C_003E1__state = -1;
					goto IL_00f9;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_00f9:
					if (_003CenumList_003E5__6.Update())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					if (_003CtransformBehaviour_003E5__4 != null)
					{
						_003CtransformBehaviour_003E5__4.SetSortingLayer(collectCoinAction.settings.sortingLayerFly);
					}
					_003Canimation_003E5__5 = collectCoinAction.TravelPart(_003CtransformBehaviour_003E5__4);
					break;
				}
				if (_003Canimation_003E5__5.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003Cgame_003E5__2.Play(GGSoundSystem.SFXType.CoinCollect);
				if (_003Ccoin_003E5__3 != null)
				{
					_003Ccoin_003E5__3.RemoveFromGame();
				}
				collectCoinAction.globalLock.UnlockAll();
				_003Cgame_003E5__2.OnCollectCoin();
				collectCoinAction.isAlive = false;
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

		private Lock globalLock;

		private float lockedTime;

		public bool isUnlocked;

		private CollectGoalAction.Settings settings => Match3Settings.instance.collectGoalSettings;

		public void Init(InitArguments initArguments)
		{
			this.initArguments = initArguments;
			globalLock = lockContainer.NewLock();
			globalLock.isSlotGravitySuspended = true;
			globalLock.isChipGeneratorSuspended = true;
			globalLock.LockSlot(initArguments.chipSlot);
		}

		private IEnumerator ScalePart(TransformBehaviour transformBehaviour)
		{
			return new _003CScalePart_003Ed__10(0)
			{
				_003C_003E4__this = this,
				transformBehaviour = transformBehaviour
			};
		}

		private IEnumerator TravelPart(TransformBehaviour transformBehaviour)
		{
			return new _003CTravelPart_003Ed__11(0)
			{
				_003C_003E4__this = this,
				transformBehaviour = transformBehaviour
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
