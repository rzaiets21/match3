using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GGMatch3
{
	public class ShowPotentialMatchAction : BoardAction
	{
		public struct InitParams
		{
			public Match3Game game;

			public bool stayInfiniteTime;

			public LevelDefinition.TutorialMatch tutorialMatch;

			public PowerupCombines.PowerupCombine powerupCombine;

			public PowerupActivations.PowerupActivation powerupActivation;

			public PotentialMatches.CompoundSlotsSet potentialMatch;

			public int userMoveWhenShow;

			public int movesCountWhenConveyorTookAction;

			public bool dontStopWhenInvalid;
		}

		[Serializable]
		public class Settings
		{
			public struct ShowPotentialMatchTimes
			{
				public float idleTimeBeforeShowMatch;

				public float boardIdleTimeBeforeShowMatch;
			}

			[Serializable]
			public class ShowSettingsModifier
			{
				[SerializeField]
				public ShowPotentialMatchSetting setting;

				[SerializeField]
				private float idleTimeBeforeShowMatch;

				[SerializeField]
				private float boardIdleTimeBeforeShowMatch;

				[SerializeField]
				private float idleTimeBeforePowerupCombine;

				[SerializeField]
				private float boardIdleTimeBeforePowerupCombine;

				[SerializeField]
				private bool useMaxTime;

				[SerializeField]
				private int userMovesBeforeMaxTime;

				[SerializeField]
				private float idleTimeBeforeShowMatchMax;

				[SerializeField]
				private float boardIdleTimeBeforeShowMatchMax;

				public ShowPotentialMatchTimes GetPotentialTimesAction(bool hasPowerupCombines, Match3Game game)
				{
					float num = idleTimeBeforeShowMatch;
					float num2 = boardIdleTimeBeforeShowMatch;
					if (hasPowerupCombines)
					{
						num = idleTimeBeforePowerupCombine;
						num2 = boardIdleTimeBeforePowerupCombine;
					}
					ShowPotentialMatchTimes showPotentialMatchTimes = default(ShowPotentialMatchTimes);
					showPotentialMatchTimes.idleTimeBeforeShowMatch = num;
					showPotentialMatchTimes.boardIdleTimeBeforeShowMatch = num2;
					if (useMaxTime)
					{
						float t = Mathf.InverseLerp(0f, userMovesBeforeMaxTime, game.board.userMovesCount);
						showPotentialMatchTimes.idleTimeBeforeShowMatch = Mathf.Lerp(showPotentialMatchTimes.idleTimeBeforeShowMatch, idleTimeBeforeShowMatchMax, t);
						showPotentialMatchTimes.boardIdleTimeBeforeShowMatch = Mathf.Lerp(showPotentialMatchTimes.boardIdleTimeBeforeShowMatch, boardIdleTimeBeforeShowMatchMax, t);
					}
					return showPotentialMatchTimes;
				}
			}

			public float duration = 1f;

			public float scaleDownDuration = 0.5f;

			public AnimationCurve scaleDownAnimation;

			public float waitBetweenFlashes = 2f;

			public AnimationCurve scaleAnimation;

			public bool useBrightness;

			public float brightnessMax = 1f;

			public AnimationCurve brightnessCurve;

			public float maxScale = 1.2f;

			public int maxFlashes = 10;

			public float stayBigDuration = 0.5f;

			public float idleTimeBeforeShowMatch = 2f;

			public float boardIdleTimeBeforeShowMatch = 2f;

			public float idleTimeBeforePowerupCombine = 1f;

			public float boardIdleTimeBeforePowerupCombine = 0.5f;

			public float moveDuration = 1f;

			public float moveDistance = 0.5f;

			public AnimationCurve moveCurve;

			public float moveDelay;

			[SerializeField]
			private List<ShowSettingsModifier> showSettingsModifiers = new List<ShowSettingsModifier>();

			public ShowPotentialMatchTimes GetPotentialTimesAction(ShowPotentialMatchSetting setting, bool hasPowerupCombines, Match3Game game)
			{
				float num = idleTimeBeforeShowMatch;
				float num2 = boardIdleTimeBeforeShowMatch;
				if (hasPowerupCombines)
				{
					num = idleTimeBeforePowerupCombine;
					num2 = boardIdleTimeBeforePowerupCombine;
				}
				ShowPotentialMatchTimes result = default(ShowPotentialMatchTimes);
				result.idleTimeBeforeShowMatch = num;
				result.boardIdleTimeBeforeShowMatch = num2;
				ShowSettingsModifier settingsModifier = GetSettingsModifier(setting);
				if (settingsModifier != null)
				{
					result = settingsModifier.GetPotentialTimesAction(hasPowerupCombines, game);
					return result;
				}
				return result;
			}

			private ShowSettingsModifier GetSettingsModifier(ShowPotentialMatchSetting setting)
			{
				for (int i = 0; i < showSettingsModifiers.Count; i++)
				{
					ShowSettingsModifier showSettingsModifier = showSettingsModifiers[i];
					if (showSettingsModifier.setting == setting)
					{
						return showSettingsModifier;
					}
				}
				return null;
			}
		}

		private struct SlotChipPair
		{
			public Slot slot;

			public Chip chip;

			public Vector3 moveDirection;

			public TransformBehaviour transform
			{
				get
				{
					if (chip == null)
					{
						return null;
					}
					return chip.GetComponentBehaviour<TransformBehaviour>();
				}
			}

			public Vector3 showMatchOffset
			{
				set
				{
					TransformBehaviour transform = this.transform;
					if (!(transform == null))
					{
						transform.localPotentialMatchOffsetPosition = value;
					}
				}
			}

			public float brightness
			{
				set
				{
					TransformBehaviour transform = this.transform;
					if (!(transform == null))
					{
						transform.SetBrightness(value);
					}
				}
			}

			public Vector3 showMatchActionLocalScale
			{
				set
				{
					TransformBehaviour transform = this.transform;
					if (!(transform == null))
					{
						transform.showMatchActionLocalScale = value;
					}
				}
			}

			public bool isChipChangedSlot
			{
				get
				{
					if (chip == null)
					{
						return false;
					}
					return chip.slot != slot;
				}
			}

			public SlotChipPair(Slot slot, Chip chip)
			{
				this.slot = slot;
				this.chip = chip;
				moveDirection = Vector3.zero;
			}
		}

		private sealed class _003CDoMoveAnimation_003Ed__22 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ShowPotentialMatchAction _003C_003E4__this;

			private Settings _003Csettings_003E5__2;

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
			public _003CDoMoveAnimation_003Ed__22(int _003C_003E1__state)
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
				ShowPotentialMatchAction showPotentialMatchAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Csettings_003E5__2 = showPotentialMatchAction.settings;
					goto IL_0035;
				case 1:
					_003C_003E1__state = -1;
					goto IL_00f6;
				case 2:
					{
						_003C_003E1__state = -1;
						goto IL_015e;
					}
					IL_0035:
					_003Ctime_003E5__3 = 0f;
					goto IL_00f6;
					IL_00f6:
					if (_003Ctime_003E5__3 <= _003Csettings_003E5__2.moveDuration * 2f)
					{
						_003Ctime_003E5__3 += showPotentialMatchAction.deltaTime;
						float num2 = Mathf.PingPong(_003Ctime_003E5__3, _003Csettings_003E5__2.moveDuration);
						if (_003Csettings_003E5__2.moveCurve != null)
						{
							num2 = _003Csettings_003E5__2.moveCurve.Evaluate(num2);
						}
						for (int i = 0; i < showPotentialMatchAction.matchingSlots.Count; i++)
						{
							SlotChipPair slotChipPair = showPotentialMatchAction.matchingSlots[i];
							Vector3 vector2 = slotChipPair.showMatchOffset = Vector3.LerpUnclamped(Vector3.zero, slotChipPair.moveDirection * _003Csettings_003E5__2.moveDistance, num2);
						}
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					_003Ctime_003E5__3 = 0f;
					if (!(_003Csettings_003E5__2.moveDelay > 0f))
					{
						goto IL_0035;
					}
					goto IL_015e;
					IL_015e:
					if (_003Ctime_003E5__3 <= _003Csettings_003E5__2.moveDelay)
					{
						_003Ctime_003E5__3 += showPotentialMatchAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_0035;
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

		private sealed class _003CDoSingleAnimation_003Ed__23 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ShowPotentialMatchAction _003C_003E4__this;

			private float _003Ctime_003E5__2;

			private Settings _003Csettings_003E5__3;

			private float _003Cduration_003E5__4;

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
			public _003CDoSingleAnimation_003Ed__23(int _003C_003E1__state)
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
				ShowPotentialMatchAction showPotentialMatchAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Ctime_003E5__2 = 0f;
					_003Csettings_003E5__3 = showPotentialMatchAction.settings;
					_003Cduration_003E5__4 = _003Csettings_003E5__3.duration;
					goto IL_01a2;
				case 1:
					_003C_003E1__state = -1;
					goto IL_01a2;
				case 2:
					_003C_003E1__state = -1;
					goto IL_01fb;
				case 3:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_01a2:
					if (_003Ctime_003E5__2 <= _003Cduration_003E5__4)
					{
						_003Ctime_003E5__2 += showPotentialMatchAction.deltaTime;
						float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__4, _003Ctime_003E5__2);
						float num3 = num2;
						if (_003Csettings_003E5__3.scaleAnimation != null)
						{
							num3 = _003Csettings_003E5__3.scaleAnimation.Evaluate(num3);
						}
						Vector3 showMatchActionLocalScale = Vector3.LerpUnclamped(Vector3.one, new Vector3(_003Csettings_003E5__3.maxScale, _003Csettings_003E5__3.maxScale, 1f), num3);
						float t = num2;
						if (_003Csettings_003E5__3.brightnessCurve != null)
						{
							t = _003Csettings_003E5__3.brightnessCurve.Evaluate(num2);
						}
						float brightness = Mathf.LerpUnclamped(1f, _003Csettings_003E5__3.brightnessMax, t);
						for (int i = 0; i < showPotentialMatchAction.matchingSlots.Count; i++)
						{
							SlotChipPair slotChipPair = showPotentialMatchAction.matchingSlots[i];
							if (slotChipPair.slot.isSlotGravitySuspended)
							{
								slotChipPair.showMatchActionLocalScale = Vector3.one;
								if (_003Csettings_003E5__3.useBrightness)
								{
									slotChipPair.brightness = 1f;
								}
							}
							else
							{
								slotChipPair.showMatchActionLocalScale = showMatchActionLocalScale;
								if (_003Csettings_003E5__3.useBrightness)
								{
									slotChipPair.brightness = brightness;
								}
							}
						}
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__4 = _003Csettings_003E5__3.stayBigDuration;
					goto IL_01fb;
					IL_01fb:
					if (_003Ctime_003E5__2 <= _003Cduration_003E5__4)
					{
						_003Ctime_003E5__2 += showPotentialMatchAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					_003Ctime_003E5__2 = 0f;
					_003Cduration_003E5__4 = _003Csettings_003E5__3.scaleDownDuration;
					break;
				}
				if (_003Ctime_003E5__2 <= _003Cduration_003E5__4)
				{
					_003Ctime_003E5__2 += showPotentialMatchAction.deltaTime;
					float num4 = Mathf.InverseLerp(0f, _003Cduration_003E5__4, _003Ctime_003E5__2);
					if (_003Csettings_003E5__3.scaleDownAnimation != null)
					{
						num4 = _003Csettings_003E5__3.scaleDownAnimation.Evaluate(num4);
					}
					Vector3 showMatchActionLocalScale2 = Vector3.LerpUnclamped(new Vector3(_003Csettings_003E5__3.maxScale, _003Csettings_003E5__3.maxScale, 1f), Vector3.one, num4);
					for (int j = 0; j < showPotentialMatchAction.matchingSlots.Count; j++)
					{
						SlotChipPair slotChipPair2 = showPotentialMatchAction.matchingSlots[j];
						if (slotChipPair2.slot.isSlotGravitySuspended)
						{
							slotChipPair2.showMatchActionLocalScale = Vector3.one;
						}
						else
						{
							slotChipPair2.showMatchActionLocalScale = showMatchActionLocalScale2;
						}
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 3;
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

		private sealed class _003CDoAnimation_003Ed__24 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ShowPotentialMatchAction _003C_003E4__this;

			private IEnumerator _003CsingleAnimation_003E5__2;

			private IEnumerator _003CmoveAnimation_003E5__3;

			private int _003Cflashes_003E5__4;

			private float _003Ctime_003E5__5;

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
			public _003CDoAnimation_003Ed__24(int _003C_003E1__state)
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
				ShowPotentialMatchAction showPotentialMatchAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003CsingleAnimation_003E5__2 = null;
					_003CmoveAnimation_003E5__3 = showPotentialMatchAction.DoMoveAnimation();
					_003Cflashes_003E5__4 = 0;
					goto IL_0043;
				case 1:
					_003C_003E1__state = -1;
					_003CmoveAnimation_003E5__3.MoveNext();
					goto IL_00bd;
				case 2:
					{
						_003C_003E1__state = -1;
						goto IL_0043;
					}
					IL_0043:
					if (_003CsingleAnimation_003E5__2 == null)
					{
						_003CsingleAnimation_003E5__2 = showPotentialMatchAction.DoSingleAnimation();
					}
					_003CmoveAnimation_003E5__3.MoveNext();
					if (!_003CsingleAnimation_003E5__2.MoveNext())
					{
						_003CsingleAnimation_003E5__2 = null;
						_003Ctime_003E5__5 = 0f;
						goto IL_00bd;
					}
					goto IL_0100;
					IL_00bd:
					if (_003Ctime_003E5__5 <= showPotentialMatchAction.settings.waitBetweenFlashes)
					{
						_003Ctime_003E5__5 += showPotentialMatchAction.deltaTime;
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					_003Cflashes_003E5__4++;
					if (_003Cflashes_003E5__4 > showPotentialMatchAction.settings.maxFlashes && !showPotentialMatchAction.initParams.stayInfiniteTime)
					{
						showPotentialMatchAction.StopAction();
						return false;
					}
					goto IL_0100;
					IL_0100:
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

		private InitParams initParams;

		private List<SlotChipPair> matchingSlots = new List<SlotChipPair>();

		private List<SlotChipPair> swipeSlots = new List<SlotChipPair>();

		private IEnumerator animation;

		private float deltaTime;

		private TilesBorderRenderer borderRenderer;

		private TilesBorderRenderer maskRenderer;

		private ListSlotsProvider listSlotsProvider = new ListSlotsProvider();

		private List<Slot> helperList = new List<Slot>();

		private Settings settings => Match3Settings.instance.showPotentialMatchesSettings;

		private bool isMatchNoLongerValid => !isMatchStillValid;

		private bool isMatchStillValid
		{
			get
			{
				if (initParams.game.board.isGameEnded)
				{
					return false;
				}
				if (!initParams.stayInfiniteTime && (initParams.userMoveWhenShow < initParams.game.board.userMovesCount || initParams.movesCountWhenConveyorTookAction < initParams.game.board.moveCountWhenConveyorTookAction || initParams.game.isConveyorMoving || !initParams.game.isBoardFullySettled))
				{
					return false;
				}
				for (int i = 0; i < matchingSlots.Count; i++)
				{
					SlotChipPair slotChipPair = matchingSlots[i];
					if (slotChipPair.chip == null)
					{
						return false;
					}
					if (slotChipPair.slot == null)
					{
						return false;
					}
					if (slotChipPair.isChipChangedSlot)
					{
						return false;
					}
					if (slotChipPair.chip.isRemovedFromGame)
					{
						return false;
					}
					if (slotChipPair.slot.isSlotMatchingSuspended)
					{
						return false;
					}
					if (slotChipPair.slot.isLockedForDiscoBomb)
					{
						return false;
					}
				}
				for (int j = 0; j < swipeSlots.Count; j++)
				{
					SlotChipPair slotChipPair2 = swipeSlots[j];
					if (slotChipPair2.isChipChangedSlot)
					{
						return false;
					}
					if (slotChipPair2.slot == null)
					{
						return false;
					}
					if (slotChipPair2.slot.isSlotMatchingSuspended)
					{
						return false;
					}
					if (slotChipPair2.slot.isLockedForDiscoBomb)
					{
						return false;
					}
					if (slotChipPair2.slot.isSlotGravitySuspended)
					{
						return false;
					}
					if (slotChipPair2.slot.isSlotSwapSuspended)
					{
						return false;
					}
				}
				return true;
			}
		}

		private void Clear()
		{
			if (borderRenderer != null && initParams.game != null)
			{
				initParams.game.slotsRendererPool.ReturnRenderer(borderRenderer);
				borderRenderer = null;
			}
			matchingSlots.Clear();
			swipeSlots.Clear();
			animation = null;
		}

		public void Init(InitParams initParams)
		{
			Clear();
			this.initParams = initParams;
			Match3Game game = initParams.game;
			borderRenderer = game.slotsRendererPool.Next();
			listSlotsProvider.Init(game);
			listSlotsProvider.allSlots.Clear();
			if (initParams.tutorialMatch != null)
			{
				LevelDefinition.TutorialMatch tutorialMatch = initParams.tutorialMatch;
				for (int i = 0; i < tutorialMatch.matchingSlots.Count; i++)
				{
					IntVector2 position = tutorialMatch.matchingSlots[i];
					Slot slot = game.GetSlot(position);
					if (slot == null)
					{
						UnityEngine.Debug.LogError("(SHOULD NOT HAPPEN!) GETTING EMPTY SLOT FOR SAME COLOR");
						continue;
					}
					Chip slotComponent = slot.GetSlotComponent<Chip>();
					if (slotComponent == null)
					{
						UnityEngine.Debug.LogError("(SHOULD NOT HAPPEN!) GETTING EMPTY CHIP FOR SAME COLOR");
						continue;
					}
					SlotChipPair item = new SlotChipPair(slot, slotComponent);
					matchingSlots.Add(item);
					TilesSlotsProvider.Slot item2 = default(TilesSlotsProvider.Slot);
					item2.isOccupied = true;
					item2.position = slot.position;
					listSlotsProvider.allSlots.Add(item2);
				}
				if (!tutorialMatch.matchingSlots.Contains(tutorialMatch.exchangeSlot))
				{
					listSlotsProvider.allSlots.Add(new TilesSlotsProvider.Slot(tutorialMatch.exchangeSlot, isOccupied: true));
				}
				if (!tutorialMatch.matchingSlots.Contains(tutorialMatch.slotToSwipe))
				{
					listSlotsProvider.allSlots.Add(new TilesSlotsProvider.Slot(tutorialMatch.slotToSwipe, isOccupied: true));
				}
				Slot slot2 = game.GetSlot(tutorialMatch.slotToSwipe);
				Chip slotComponent2 = slot2.GetSlotComponent<Chip>();
				SlotChipPair item3 = new SlotChipPair(slot2, slotComponent2);
				item3.moveDirection = (tutorialMatch.exchangeSlot - tutorialMatch.slotToSwipe).ToVector3().normalized;
				matchingSlots.Add(item3);
				swipeSlots.Add(new SlotChipPair(slot2, slotComponent2));
				Slot slot3 = game.GetSlot(tutorialMatch.exchangeSlot);
				Chip slotComponent3 = slot3.GetSlotComponent<Chip>();
				swipeSlots.Add(new SlotChipPair(slot3, slotComponent3));
				borderRenderer.ShowBorderOnLevel(listSlotsProvider);
			}
			else if (initParams.powerupCombine != null)
			{
				List<Slot> list = helperList;
				list.Clear();
				list.Add(initParams.powerupCombine.powerupSlot);
				list.Add(initParams.powerupCombine.exchangeSlot);
				for (int j = 0; j < list.Count; j++)
				{
					Slot slot4 = list[j];
					Chip slotComponent4 = slot4.GetSlotComponent<Chip>();
					if (slotComponent4 != null)
					{
						SlotChipPair item4 = new SlotChipPair(slot4, slotComponent4);
						if (j == 0 && list.Count > 1)
						{
							item4.moveDirection = (list[1].position - slot4.position).ToVector3();
						}
						matchingSlots.Add(item4);
						swipeSlots.Add(item4);
						TilesSlotsProvider.Slot item5 = default(TilesSlotsProvider.Slot);
						item5.isOccupied = true;
						item5.position = slot4.position;
						listSlotsProvider.allSlots.Add(item5);
					}
				}
				borderRenderer.ShowBorderOnLevel(listSlotsProvider);
			}
			else if (initParams.powerupActivation != null)
			{
				List<Slot> list2 = helperList;
				list2.Clear();
				Slot powerupSlot = initParams.powerupActivation.powerupSlot;
				Slot exchangeSlot = initParams.powerupActivation.exchangeSlot;
				list2.Add(powerupSlot);
				if (initParams.powerupActivation.isSwipe)
				{
					list2.Add(exchangeSlot);
				}
				for (int k = 0; k < list2.Count; k++)
				{
					Slot slot5 = list2[k];
					Chip slotComponent5 = slot5.GetSlotComponent<Chip>();
					if (slotComponent5 != null)
					{
						SlotChipPair item6 = new SlotChipPair(slot5, slotComponent5);
						if (k == 0 && list2.Count > 1)
						{
							item6.moveDirection = (list2[1].position - slot5.position).ToVector3();
						}
						matchingSlots.Add(item6);
						swipeSlots.Add(item6);
						TilesSlotsProvider.Slot item7 = default(TilesSlotsProvider.Slot);
						item7.isOccupied = true;
						item7.position = slot5.position;
						listSlotsProvider.allSlots.Add(item7);
					}
				}
				borderRenderer.ShowBorderOnLevel(listSlotsProvider);
			}
			else if (initParams.potentialMatch != null)
			{
				PotentialMatches.CompoundSlotsSet potentialMatch = initParams.potentialMatch;
				List<PotentialMatches.SlotsSet> slotsSets = potentialMatch.slotsSets;
				for (int l = 0; l < slotsSets.Count; l++)
				{
					List<BoardRepresentation.RepresentationSlot> sameColorSlots = slotsSets[l].sameColorSlots;
					for (int m = 0; m < sameColorSlots.Count; m++)
					{
						BoardRepresentation.RepresentationSlot representationSlot = sameColorSlots[m];
						Slot slot6 = game.GetSlot(representationSlot.position);
						if (slot6 == null)
						{
							continue;
						}
						Chip slotComponent6 = slot6.GetSlotComponent<Chip>();
						if (slotComponent6 == null)
						{
							continue;
						}
						bool flag = false;
						for (int n = 0; n < matchingSlots.Count; n++)
						{
							if (matchingSlots[n].slot == slot6)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							SlotChipPair item8 = new SlotChipPair(slot6, slotComponent6);
							matchingSlots.Add(item8);
							TilesSlotsProvider.Slot item9 = default(TilesSlotsProvider.Slot);
							item9.isOccupied = true;
							item9.position = slot6.position;
							listSlotsProvider.allSlots.Add(item9);
						}
					}
				}
				listSlotsProvider.allSlots.Add(new TilesSlotsProvider.Slot(potentialMatch.positionOfSlotMissingForMatch, isOccupied: true));
				BoardRepresentation.RepresentationSlot swipeSlot = potentialMatch.swipeSlot;
				if (!listSlotsProvider.ContainsPosition(swipeSlot.position))
				{
					listSlotsProvider.AddSlot(new TilesSlotsProvider.Slot(swipeSlot.position, isOccupied: true));
				}
				Slot slot7 = game.GetSlot(potentialMatch.swipeSlot.position);
				Chip slotComponent7 = slot7.GetSlotComponent<Chip>();
				SlotChipPair item10 = new SlotChipPair(slot7, slotComponent7);
				item10.moveDirection = (potentialMatch.positionOfSlotMissingForMatch - slot7.position).ToVector3().normalized;
				matchingSlots.Add(item10);
				swipeSlots.Add(new SlotChipPair(slot7, slotComponent7));
				Slot slot8 = game.GetSlot(potentialMatch.positionOfSlotMissingForMatch);
				Chip slotComponent8 = slot8.GetSlotComponent<Chip>();
				swipeSlots.Add(new SlotChipPair(slot8, slotComponent8));
				borderRenderer.ShowBorderOnLevel(listSlotsProvider);
			}
			base.Reset();
		}

		private IEnumerator DoMoveAnimation()
		{
			return new _003CDoMoveAnimation_003Ed__22(0)
			{
				_003C_003E4__this = this
			};
		}

		private IEnumerator DoSingleAnimation()
		{
			return new _003CDoSingleAnimation_003Ed__23(0)
			{
				_003C_003E4__this = this
			};
		}

		private IEnumerator DoAnimation()
		{
			return new _003CDoAnimation_003Ed__24(0)
			{
				_003C_003E4__this = this
			};
		}

		private void StopAction()
		{
			ReturnChipsToPositions();
			isAlive = false;
		}

		private void ReturnChipsToPositions()
		{
			for (int i = 0; i < matchingSlots.Count; i++)
			{
				SlotChipPair slotChipPair = matchingSlots[i];
				slotChipPair.showMatchActionLocalScale = Vector3.one;
				slotChipPair.showMatchOffset = Vector3.zero;
				slotChipPair.brightness = 1f;
			}
			Clear();
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			this.deltaTime = deltaTime;
			if (!initParams.dontStopWhenInvalid && isMatchNoLongerValid)
			{
				StopAction();
				return;
			}
			if (animation == null)
			{
				animation = DoAnimation();
			}
			animation.MoveNext();
		}

		public override void Stop()
		{
			if (isAlive)
			{
				StopAction();
			}
		}
	}
}
