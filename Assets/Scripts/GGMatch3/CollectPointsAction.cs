using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class CollectPointsAction : BoardAction
	{
		public struct InitArguments
		{
			public int count;

			public int matchesCount;

			public Vector3 localPosition;

			public Match3Game game;

			public bool isBlocker;

			public ItemColor itemColor;

			public void SetCount(int desiredCount, int numMatches)
			{
				matchesCount = Mathf.Clamp(numMatches - 1, 0, 5);
				int num = matchesCount + 1;
				count = desiredCount * num;
			}
		}

		[Serializable]
		public class Settings
		{
			[Serializable]
			public class ColorForItemColor
			{
				[SerializeField]
				public ItemColor itemColor;

				[SerializeField]
				public Color color;
			}

			public bool enabled;

			public bool enableForPowerupCreate;

			public bool enableForPowerupBlow;

			public bool enableForGoals;

			public int blockerCount = 30;

			public int count = 60;

			public int powerupCount = 100;

			public float fadeFrom;

			public float fadeTo;

			public float scaleFrom;

			public float scaleTo;

			public float scaleToEnd;

			public float scaleToBlocker;

			public int maxMatches;

			public AnimationCurve scaleCurve;

			public float appearDuration;

			public Vector3 travelOffset;

			public float travelDuration;

			public AnimationCurve travelFadeCurve;

			public float finalAlpha;

			[SerializeField]
			private List<ColorForItemColor> itemColors = new List<ColorForItemColor>();

			public Color GetColor(ItemColor itemColor)
			{
				for (int i = 0; i < itemColors.Count; i++)
				{
					ColorForItemColor colorForItemColor = itemColors[i];
					if (colorForItemColor.itemColor == itemColor)
					{
						return colorForItemColor.color;
					}
				}
				return Color.white;
			}
		}

		private sealed class _003CMove_003Ed__14 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectPointsAction _003C_003E4__this;

			private Settings _003Csettings_003E5__2;

			private Vector3 _003CstartPos_003E5__3;

			private Vector3 _003CendPos_003E5__4;

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
			public _003CMove_003Ed__14(int _003C_003E1__state)
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
				CollectPointsAction collectPointsAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Csettings_003E5__2 = collectPointsAction.settings;
					_003CstartPos_003E5__3 = collectPointsAction.transformToMove.localPosition;
					_003CendPos_003E5__4 = _003CstartPos_003E5__3 + _003Csettings_003E5__2.travelOffset;
					_003Ctime_003E5__5 = 0f;
					_003Cduration_003E5__6 = _003Csettings_003E5__2.travelDuration;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__5 <= _003Cduration_003E5__6)
				{
					_003Ctime_003E5__5 += collectPointsAction.deltaTime;
					float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__5);
					float t = num2;
					float t2 = _003Csettings_003E5__2.travelFadeCurve.Evaluate(num2);
					Vector3 localPosition = Vector3.LerpUnclamped(_003CstartPos_003E5__3, _003CendPos_003E5__4, t);
					float alpha = Mathf.Lerp(_003Csettings_003E5__2.fadeTo, _003Csettings_003E5__2.finalAlpha, t2);
					collectPointsAction.transformToMove.localPosition = localPosition;
					collectPointsAction.transformToMove.SetAlpha(alpha);
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
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

		private sealed class _003CAppear_003Ed__15 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectPointsAction _003C_003E4__this;

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
			public _003CAppear_003Ed__15(int _003C_003E1__state)
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
				CollectPointsAction collectPointsAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Csettings_003E5__2 = collectPointsAction.settings;
					_003CstartScale_003E5__3 = Vector3.one * _003Csettings_003E5__2.scaleFrom;
					_003CendScale_003E5__4 = Vector3.one * Mathf.Lerp(_003Csettings_003E5__2.scaleTo, _003Csettings_003E5__2.scaleToEnd, Mathf.InverseLerp(0f, _003Csettings_003E5__2.maxMatches, collectPointsAction.initArguments.matchesCount));
					if (collectPointsAction.initArguments.isBlocker)
					{
						_003CendScale_003E5__4 = Vector3.one * _003Csettings_003E5__2.scaleToBlocker;
					}
					_003Ctime_003E5__5 = 0f;
					_003Cduration_003E5__6 = _003Csettings_003E5__2.appearDuration;
					break;
				case 1:
					_003C_003E1__state = -1;
					break;
				}
				if (_003Ctime_003E5__5 <= _003Cduration_003E5__6)
				{
					_003Ctime_003E5__5 += collectPointsAction.deltaTime;
					float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__6, _003Ctime_003E5__5);
					float t = _003Csettings_003E5__2.scaleCurve.Evaluate(num2);
					float t2 = num2;
					Vector3 localScale = Vector3.LerpUnclamped(_003CstartScale_003E5__3, _003CendScale_003E5__4, t);
					float alpha = Mathf.Lerp(_003Csettings_003E5__2.fadeFrom, _003Csettings_003E5__2.fadeTo, t2);
					collectPointsAction.transformToMove.localScale = localScale;
					collectPointsAction.transformToMove.SetAlpha(alpha);
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
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

		private sealed class _003CDoAnimation_003Ed__16 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public CollectPointsAction _003C_003E4__this;

			private IEnumerator _003Canimation_003E5__2;

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
				CollectPointsAction collectPointsAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					collectPointsAction.transformToMove = collectPointsAction.initArguments.game.CreatePointsDisplay();
					if (collectPointsAction.transformToMove == null)
					{
						collectPointsAction.isAlive = false;
						return false;
					}
					collectPointsAction.transformToMove.localPosition = collectPointsAction.initArguments.localPosition;
					collectPointsAction.transformToMove.SetText(collectPointsAction.initArguments.count.ToString());
					collectPointsAction.transformToMove.SetColor(collectPointsAction.settings.GetColor(collectPointsAction.initArguments.itemColor));
					GGUtil.Show(collectPointsAction.transformToMove);
					_003Canimation_003E5__2 = collectPointsAction.Appear();
					goto IL_00d8;
				case 1:
					_003C_003E1__state = -1;
					goto IL_00d8;
				case 2:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_00d8:
					if (_003Canimation_003E5__2.MoveNext())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					collectPointsAction.initArguments.game.OnScoreAdded(collectPointsAction.initArguments.count);
					_003Canimation_003E5__2 = collectPointsAction.Move();
					break;
				}
				if (_003Canimation_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				collectPointsAction.transformToMove.RemoveFromGame();
				collectPointsAction.isAlive = false;
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

		private IEnumerator animationEnum;

		private InitArguments initArguments;

		private static List<Slot> slotsWithGoals = new List<Slot>();

		private TransformBehaviour transformToMove;

		private Settings settings => Match3Settings.instance.collectPointsActionSettings;

		public static void OnBlockerDestroy(Slot slot, SlotDestroyParams destroyParams)
		{
			Settings collectPointsActionSettings = Match3Settings.instance.collectPointsActionSettings;
			if (collectPointsActionSettings.enabled && slot != null && destroyParams != null && destroyParams.scoreAdded <= 0)
			{
				int blockerCount = collectPointsActionSettings.blockerCount;
				InitArguments initArguments = default(InitArguments);
				Match3Game game = slot.game;
				initArguments.SetCount(blockerCount, 1);
				initArguments.localPosition = slot.localPositionOfCenter;
				initArguments.game = game;
				initArguments.isBlocker = true;
				initArguments.itemColor = ItemColor.Uncolored;
				CollectPointsAction collectPointsAction = new CollectPointsAction();
				collectPointsAction.Init(initArguments);
				game.board.actionManager.AddAction(collectPointsAction);
				destroyParams.scoreAdded++;
			}
		}

		public static void OnIslandDestroy(DestroyMatchingIslandAction.InitArguments arguments)
		{
			Settings collectPointsActionSettings = Match3Settings.instance.collectPointsActionSettings;
			if (!collectPointsActionSettings.enabled)
			{
				return;
			}
			bool flag = arguments.slotWherePowerupIsCreated != null;
			List<Slot> allSlots = arguments.allSlots;
			if (allSlots.Count == 0)
			{
				return;
			}
			slotsWithGoals.Clear();
			Match3Game game = arguments.game;
			bool flag2 = false;
			ItemColor itemColor = ItemColor.Unknown;
			Vector3 a = Vector3.zero;
			ActionScore a2 = default(ActionScore);
			for (int i = 0; i < allSlots.Count; i++)
			{
				Slot slot = allSlots[i];
				a += slot.localPositionOfCenter;
				ActionScore actionScore = game.goals.FreshActionScoreForDestroyingSlot(slot);
				a2 += actionScore;
				if (actionScore.goalsCount > 0)
				{
					slotsWithGoals.Add(slot);
				}
				Chip slotComponent = slot.GetSlotComponent<Chip>();
				if (slotComponent != null)
				{
					itemColor = slotComponent.itemColor;
				}
			}
			if (!collectPointsActionSettings.enableForGoals && flag2)
			{
				return;
			}
			a /= allSlots.Count;
			if (arguments.slotWherePowerupIsCreated != null)
			{
				a = arguments.slotWherePowerupIsCreated.localPositionOfCenter;
			}
			if (slotsWithGoals.Count > 0)
			{
				for (int j = 0; j < slotsWithGoals.Count; j++)
				{
					Slot slot2 = slotsWithGoals[j];
					InitArguments initArguments = default(InitArguments);
					initArguments.SetCount(collectPointsActionSettings.count, 1);
					initArguments.localPosition = slot2.localPositionOfCenter;
					initArguments.game = game;
					initArguments.itemColor = itemColor;
					CollectPointsAction collectPointsAction = new CollectPointsAction();
					collectPointsAction.Init(initArguments);
					game.board.actionManager.AddAction(collectPointsAction);
				}
				return;
			}
			int desiredCount = collectPointsActionSettings.count;
			if (flag)
			{
				desiredCount = collectPointsActionSettings.powerupCount * allSlots.Count;
			}
			if (collectPointsActionSettings.enableForPowerupCreate || !flag)
			{
				InitArguments initArguments2 = default(InitArguments);
				initArguments2.SetCount(desiredCount, game.board.currentMoveMatches);
				initArguments2.localPosition = a;
				initArguments2.game = game;
				initArguments2.itemColor = itemColor;
				CollectPointsAction collectPointsAction2 = new CollectPointsAction();
				collectPointsAction2.Init(initArguments2);
				game.board.actionManager.AddAction(collectPointsAction2);
			}
		}

		public static void OnChipDestroy(Chip chip, SlotDestroyParams destroyParams)
		{
			Settings collectPointsActionSettings = Match3Settings.instance.collectPointsActionSettings;
			if (!collectPointsActionSettings.enabled || chip == null || destroyParams == null || (!collectPointsActionSettings.enableForGoals && chip.isPartOfActiveGoal) || (destroyParams.isFromSwapOrTap && !collectPointsActionSettings.enableForPowerupBlow && chip.isPowerup) || (chip.chipType == ChipType.Chip && !destroyParams.isHitByBomb) || chip.isPickupElement)
			{
				return;
			}
			Slot lastConnectedSlot = chip.lastConnectedSlot;
			if (lastConnectedSlot == null)
			{
				return;
			}
			Match3Game game = lastConnectedSlot.game;
			ActionScore actionScore = game.goals.FreshActionScoreForDestroyingSlot(lastConnectedSlot);
			if (chip != null && chip.isPartOfActiveGoal)
			{
				actionScore.goalsCount++;
			}
			if (actionScore.goalsCount > 0 || destroyParams.chipBlockersDestroyed > 0 || chip.isPowerup)
			{
				InitArguments initArguments = default(InitArguments);
				initArguments.count = collectPointsActionSettings.count;
				if (chip.isPowerup)
				{
					initArguments.SetCount(collectPointsActionSettings.powerupCount, game.board.currentMoveMatches);
				}
				if (actionScore.goalsCount > 0 || actionScore.obstaclesDestroyed > 0)
				{
					initArguments.SetCount(collectPointsActionSettings.count, 1);
				}
				initArguments.localPosition = lastConnectedSlot.localPositionOfCenter;
				initArguments.game = game;
				initArguments.itemColor = chip.itemColor;
				CollectPointsAction collectPointsAction = new CollectPointsAction();
				collectPointsAction.Init(initArguments);
				game.board.actionManager.AddAction(collectPointsAction);
				destroyParams.scoreAdded++;
			}
		}

		public static void OnSlotDestroy(Slot slot, SlotDestroyParams destroyParams)
		{
			Settings collectPointsActionSettings = Match3Settings.instance.collectPointsActionSettings;
			if (collectPointsActionSettings.enabled && destroyParams != null && destroyParams.scoreAdded <= 0 && destroyParams.chipsDestroyed <= 0 && destroyParams.chipBlockersDestroyed != 0)
			{
				InitArguments initArguments = default(InitArguments);
				initArguments.count = collectPointsActionSettings.count;
				initArguments.localPosition = slot.localPositionOfCenter;
				initArguments.game = slot.game;
				initArguments.itemColor = ItemColor.Unknown;
				CollectPointsAction collectPointsAction = new CollectPointsAction();
				collectPointsAction.Init(initArguments);
				slot.game.board.actionManager.AddAction(collectPointsAction);
				destroyParams.scoreAdded++;
			}
		}

		public void Init(InitArguments initArguments)
		{
			this.initArguments = initArguments;
		}

		private IEnumerator Move()
		{
			return new _003CMove_003Ed__14(0)
			{
				_003C_003E4__this = this
			};
		}

		private IEnumerator Appear()
		{
			return new _003CAppear_003Ed__15(0)
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
			if (animationEnum == null)
			{
				animationEnum = DoAnimation();
			}
			animationEnum.MoveNext();
		}
	}
}
