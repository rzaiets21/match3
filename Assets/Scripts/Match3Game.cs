using EZCameraShake;
using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Match3Game : MonoBehaviour
{
	[Serializable]
	public class DifficultyChanger
	{
		[Serializable]
		public class MaterialChange
		{
			[SerializeField]
			public Match3StagesDB.Stage.Difficulty difficulty;

			[SerializeField]
			private Material material;

			[SerializeField]
			private MeshRenderer renderer;

			public void Change()
			{
				if (!(renderer == null))
				{
					renderer.material = material;
				}
			}
		}

		[SerializeField]
		private List<MaterialChange> changes = new List<MaterialChange>();

		public void Apply(Match3StagesDB.Stage.Difficulty difficulty)
		{
			for (int i = 0; i < changes.Count; i++)
			{
				MaterialChange materialChange = changes[i];
				if (materialChange.difficulty == difficulty)
				{
					materialChange.Change();
				}
			}
		}
	}

	[Serializable]
	public class PieceCreatorPool
	{
		public PieceType type;

		public List<ChipType> chipTypeList = new List<ChipType>();

		public List<ItemColor> itemColorList = new List<ItemColor>();

		public ComponentPool pool;
	}

	public interface IAffectorExportAction
	{
		void Execute();

		void OnCancel();
	}

	public class InputAffectorExport
	{
		public class InputAffectorForSlot
		{
			public Slot slot;

			public List<LightingBolt> bolts = new List<LightingBolt>();

			public void Clear()
			{
				DiscoBallAffector.RemoveFromGame(bolts);
				bolts.Clear();
			}

			public void PutAllBoltsIn(BoltCollection boltCollection)
			{
				boltCollection.bolts.AddRange(bolts);
				bolts.Clear();
			}

			public LightingBolt GetLigtingBoltForSlots(IntVector2 startPosition, IntVector2 endPosition)
			{
				for (int i = 0; i < bolts.Count; i++)
				{
					LightingBolt lightingBolt = bolts[i];
					if (lightingBolt.isSlotPositionsSet && lightingBolt.startSlotPosition == startPosition && lightingBolt.endSlotPosition == endPosition)
					{
						bolts.Remove(lightingBolt);
						return lightingBolt;
					}
				}
				return null;
			}
		}

		private List<IAffectorExportAction> actions = new List<IAffectorExportAction>();

		private List<ChipAffectorBase> chipAffectors = new List<ChipAffectorBase>();

		public List<InputAffectorForSlot> affectorExports = new List<InputAffectorForSlot>();

		public bool hasActions => actions.Count > 0;

		public void AddChipAffector(ChipAffectorBase chipAffector)
		{
			chipAffectors.Add(chipAffector);
		}

		public void ExecuteOnAfterDestroy()
		{
			for (int i = 0; i < chipAffectors.Count; i++)
			{
				chipAffectors[i].OnAfterDestroy();
			}
		}

		public void AddAction(IAffectorExportAction action)
		{
			if (!actions.Contains(action))
			{
				actions.Add(action);
			}
		}

		public void ExecuteActions()
		{
			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].Execute();
			}
			actions.Clear();
		}

		public LightingBolt GetLigtingBoltForSlots(IntVector2 startPosition, IntVector2 endPosition)
		{
			for (int i = 0; i < affectorExports.Count; i++)
			{
				LightingBolt ligtingBoltForSlots = affectorExports[i].GetLigtingBoltForSlots(startPosition, endPosition);
				if (ligtingBoltForSlots != null)
				{
					return ligtingBoltForSlots;
				}
			}
			return null;
		}

		public InputAffectorForSlot GetInputAffectorForSlot(Slot slot)
		{
			for (int i = 0; i < affectorExports.Count; i++)
			{
				InputAffectorForSlot inputAffectorForSlot = affectorExports[i];
				if (inputAffectorForSlot.slot == slot)
				{
					return inputAffectorForSlot;
				}
			}
			return null;
		}

		public void Clear()
		{
			for (int i = 0; i < affectorExports.Count; i++)
			{
				affectorExports[i].Clear();
			}
			for (int j = 0; j < actions.Count; j++)
			{
				actions[j].OnCancel();
			}
		}
	}

	public struct SwitchSlotsArguments
	{
		public IntVector2 pos1;

		public IntVector2 pos2;

		public bool instant;

		public List<LightingBolt> bolts;

		public float affectorDuration;

		public bool isAlreadySwitched;

		private InputAffectorExport affectorExport_;

		public InputAffectorExport affectorExport
		{
			get
			{
				if (affectorExport_ == null)
				{
					affectorExport_ = new InputAffectorExport();
				}
				return affectorExport_;
			}
		}

		public void Clear()
		{
			if (affectorExport_ != null)
			{
				affectorExport_.Clear();
			}
		}
	}

	public struct StartGameArguments
	{
		public bool putBoosters;
	}

	public class TutorialMatchProgress
	{
		public LevelDefinition.TutorialMatch tutorialMatch;

		public bool isStarted;
	}

	[Serializable]
	public class TutorialSlotHighlighter
	{
		[SerializeField]
		private TilesBorderRenderer tilesMaskRenderer;

		[SerializeField]
		private TilesBorderRenderer borderMaskRenderer;

		[SerializeField]
		private TilesBorderRenderer borderRenderer;

		[SerializeField]
		private Transform tutorialBackground;

		[NonSerialized]
		private GameScreen gameScreen;

		public void Init(GameScreen gameScreen)
		{
			this.gameScreen = gameScreen;
		}

		public void SetTutorialBackgroundActive(bool active)
		{
			GGUtil.SetActive(tutorialBackground, active);
		}

		public void ShowGameScreenTutorialMask()
		{
			if (gameScreen != null)
			{
				GGUtil.SetActive(gameScreen.tutorialMask, active: true);
				SetTutorialBackgroundActive(active: false);
			}
		}

		public void Show(TilesSlotsProvider provider)
		{
			SetActive(flag: true);
			tilesMaskRenderer.ShowSlotsOnLevel(provider);
			borderMaskRenderer.ShowBorderOnLevel(provider);
			borderRenderer.ShowBorderOnLevel(provider);
		}

		public void Hide()
		{
			SetActive(flag: false);
			if (gameScreen != null)
			{
				GGUtil.SetActive(gameScreen.tutorialMask, active: false);
			}
		}

		private void SetActive(bool flag)
		{
			GGUtil.SetActive(tutorialBackground, flag);
			GGUtil.SetActive(tilesMaskRenderer, flag);
			GGUtil.SetActive(borderMaskRenderer, flag);
			GGUtil.SetActive(borderRenderer, flag);
		}
	}

	public struct CreateBoardArguments
	{
		public LevelDefinition level;

		public Vector3 offset;
	}

	protected struct PotentialMatch
	{
		public ActionScore actionScore;

		public PowerupCombines.PowerupCombine powerupCombine;

		public PowerupActivations.PowerupActivation powerupActivation;

		public PotentialMatches.CompoundSlotsSet potentialMatch;

		public ActionScore powerupCreatedScore;

		public bool isActive
		{
			get
			{
				if (powerupCombine == null && powerupActivation == null)
				{
					return potentialMatch != null;
				}
				return true;
			}
		}

		public int ScoreWithPowerupScore(int currentScoreFactor, int goalsFactor)
		{
			return currentScoreFactor * actionScore.GoalsAndObstaclesScore(goalsFactor) + powerupCreatedScore.GoalsAndObstaclesScore(goalsFactor);
		}

		public PotentialMatch(PowerupCombines.PowerupCombine powerupCombine, ActionScore actionScore)
		{
			this.actionScore = actionScore;
			this.powerupCombine = powerupCombine;
			powerupActivation = null;
			potentialMatch = null;
			powerupCreatedScore = default(ActionScore);
		}

		public PotentialMatch(PowerupActivations.PowerupActivation powerupActivation, ActionScore actionScore)
		{
			this.actionScore = actionScore;
			powerupCombine = null;
			this.powerupActivation = powerupActivation;
			potentialMatch = null;
			powerupCreatedScore = default(ActionScore);
		}

		public PotentialMatch(PotentialMatches.CompoundSlotsSet potentialMatch, ActionScore actionScore, ActionScore powerupCreatedScore)
		{
			this.actionScore = actionScore;
			powerupCombine = null;
			powerupActivation = null;
			this.potentialMatch = potentialMatch;
			this.powerupCreatedScore = powerupCreatedScore;
		}
	}

	private struct SlotDestroyNeighbour
	{
		public Slot slotBeingDestroyed;

		public Slot neighbourSlot;

		public SlotDestroyNeighbour(Slot slotBeingDestroyed, Slot neighbourSlot)
		{
			this.slotBeingDestroyed = slotBeingDestroyed;
			this.neighbourSlot = neighbourSlot;
		}
	}

	public class MaximumColorHelper
	{
		public struct Color
		{
			public ItemColor color;

			public int count;
		}

		private List<Color> colorList = new List<Color>();

		public Color MaxColor
		{
			get
			{
				Color color = default(Color);
				color.color = ItemColor.Red;
				for (int i = 0; i < colorList.Count; i++)
				{
					Color color2 = colorList[i];
					if (color2.count > color.count)
					{
						color = color2;
					}
				}
				return color;
			}
		}

		public void Clear()
		{
			colorList.Clear();
		}

		private void AddColor(ItemColor color)
		{
			for (int i = 0; i < colorList.Count; i++)
			{
				Color color2 = colorList[i];
				if (color2.color == color)
				{
					color2.count++;
					colorList[i] = color2;
					return;
				}
			}
			Color item = default(Color);
			item.color = color;
			item.count++;
			colorList.Add(item);
		}

		public void AddSlot(Slot slot, bool replaceWithBombs)
		{
			if (slot == null)
			{
				return;
			}
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			if (slotComponent != null && slotComponent.canFormColorMatches)
			{
				ItemColor itemColor = slotComponent.itemColor;
				if (slot.CanParticipateInDiscoBombAffectedArea(itemColor, replaceWithBombs))
				{
					AddColor(itemColor);
				}
			}
		}
	}

	[Serializable]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Comparison<Slot> _003C_003E9__121_0;

		internal int _003CCreateBoard_003Eb__121_0(Slot x, Slot y)
		{
			return x.maxDistanceToEnd.CompareTo(y.maxDistanceToEnd);
		}
	}

	private sealed class _003CDoShuffleBoardAnimation_003Ed__192 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public Match3Game _003C_003E4__this;

		private IEnumerator _003CshuffleInAnimation_003E5__2;

		private float _003Ctime_003E5__3;

		private float _003Cduration_003E5__4;

		private IEnumerator _003CoutAnimation_003E5__5;

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
		public _003CDoShuffleBoardAnimation_003Ed__192(int _003C_003E1__state)
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
			Match3Game match3Game = _003C_003E4__this;
			PopulateBoard.BoardRepresentation boardRepresentation;
			PopulateBoard.Params @params;
			Slot[] slots;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				match3Game.board.isShufflingBoard = true;
				match3Game.board.bubblesBoardComponent.CancelSpread();
				_003CshuffleInAnimation_003E5__2 = match3Game.gameScreen.shuffleContainer.DoShow();
				goto IL_0078;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0078;
			case 2:
				_003C_003E1__state = -1;
				goto IL_038d;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0078:
				if (_003CshuffleInAnimation_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				boardRepresentation = new PopulateBoard.BoardRepresentation();
				boardRepresentation.Init(match3Game, generateFlowerChips: false);
				match3Game.attachedElementsPerItemColor.Clear();
				@params = new PopulateBoard.Params();
				@params.randomProvider = match3Game.board.randomProvider;
				if (match3Game.level.generationSettings.isConfigured)
				{
					List<LevelDefinition.ChipGenerationSettings.ChipSetting> chipSettings = match3Game.level.generationSettings.chipSettings;
					for (int i = 0; i < chipSettings.Count; i++)
					{
						LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting = chipSettings[i];
						if (chipSetting.chipType == ChipType.Chip)
						{
							@params.availableColors.Add(chipSetting.itemColor);
						}
					}
				}
				else
				{
					for (int j = 0; j < match3Game.level.numChips; j++)
					{
						@params.availableColors.Add((ItemColor)j);
					}
				}
				@params.maxPotentialMatches = Match3Settings.instance.maxPotentialMatchesAtStart;
				if (!match3Game.board.populateBoard.RandomPopulate(boardRepresentation, @params))
				{
					for (int k = 0; k < match3Game.board.sortedSlotsUpdateList.Count; k++)
					{
						Chip slotComponent = match3Game.board.sortedSlotsUpdateList[k].GetSlotComponent<Chip>();
						if (slotComponent != null && slotComponent.hasGrowingElement)
						{
							if (!match3Game.attachedElementsPerItemColor.ContainsKey(slotComponent.itemColor))
							{
								match3Game.attachedElementsPerItemColor[slotComponent.itemColor] = 0;
							}
							Dictionary<ItemColor, int> attachedElementsPerItemColor = match3Game.attachedElementsPerItemColor;
							ItemColor itemColor = slotComponent.itemColor;
							attachedElementsPerItemColor[itemColor]++;
						}
					}
					boardRepresentation.Init(match3Game, generateFlowerChips: true);
					match3Game.board.populateBoard.RandomPopulate(boardRepresentation, @params);
				}
				slots = match3Game.board.slots;
				foreach (Slot slot in slots)
				{
					if (slot == null)
					{
						continue;
					}
					PopulateBoard.BoardRepresentation.RepresentationSlot slot2 = match3Game.board.populateBoard.board.GetSlot(slot.position);
					if (!slot2.needsToBeGenerated || !slot2.isGenerated)
					{
						continue;
					}
					Chip slotComponent2 = slot.GetSlotComponent<Chip>();
					if (slotComponent2.chipType == ChipType.Chip)
					{
						ChipBehaviour componentBehaviour = slotComponent2.GetComponentBehaviour<ChipBehaviour>();
						slotComponent2.itemColor = slot2.itemColor;
						if (componentBehaviour != null)
						{
							componentBehaviour.ChangeClothTexture(slotComponent2.itemColor);
						}
						int num2 = 0;
						if (match3Game.attachedElementsPerItemColor.ContainsKey(slotComponent2.itemColor))
						{
							num2 = match3Game.attachedElementsPerItemColor[slotComponent2.itemColor];
						}
						bool flag = num2 > 0;
						num2 = Mathf.Max(0, num2 - 1);
						match3Game.attachedElementsPerItemColor[slotComponent2.itemColor] = num2;
						if (slotComponent2.hasGrowingElement && !flag)
						{
							slotComponent2.DestroyGrowingElement();
						}
						else if (!slotComponent2.hasGrowingElement && flag)
						{
							TransformBehaviour growingElementGraphics = AnimateGrowingElementOnChip.CreateGrowingElementPieceBehaviour(match3Game);
							slotComponent2.AttachGrowingElement(growingElementGraphics);
						}
					}
				}
				_003Ctime_003E5__3 = 0f;
				_003Cduration_003E5__4 = 1f;
				goto IL_038d;
				IL_038d:
				if (_003Ctime_003E5__3 < _003Cduration_003E5__4)
				{
					_003Ctime_003E5__3 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003CoutAnimation_003E5__5 = match3Game.gameScreen.shuffleContainer.DoHide();
				break;
			}
			if (_003CoutAnimation_003E5__5.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
			match3Game.board.isShufflingBoard = false;
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

	private sealed class _003CDoStartInAnimation_003Ed__237 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public Match3Game _003C_003E4__this;

		private float _003Ctime_003E5__2;

		private WinScreenBoardInAnimation _003Csettings_003E5__3;

		private float _003Cduration_003E5__4;

		private Transform _003Ct_003E5__5;

		private Vector3 _003CendPos_003E5__6;

		private Vector3 _003CstartPos_003E5__7;

		private Vector3 _003CendScale_003E5__8;

		private Vector3 _003CstartScale_003E5__9;

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
		public _003CDoStartInAnimation_003Ed__237(int _003C_003E1__state)
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
			Match3Game match3Game = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				_003Csettings_003E5__3 = Match3Settings.instance.winScreenInAnimation;
				_003Cduration_003E5__4 = _003Csettings_003E5__3.duration;
				_003Ct_003E5__5 = match3Game.boardContainer;
				_003CendPos_003E5__6 = _003Ct_003E5__5.localPosition;
				_003CstartPos_003E5__7 = _003CendPos_003E5__6 + _003Csettings_003E5__3.offset;
				_003CendScale_003E5__8 = _003Ct_003E5__5.localScale;
				_003CstartScale_003E5__9 = _003Csettings_003E5__3.scale;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__2 <= _003Cduration_003E5__4)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__4, _003Ctime_003E5__2);
				if (_003Csettings_003E5__3.curve != null)
				{
					num2 = _003Csettings_003E5__3.curve.Evaluate(num2);
				}
				Vector3 localPosition = Vector3.LerpUnclamped(_003CstartPos_003E5__7, _003CendPos_003E5__6, num2);
				Vector3 localScale = Vector3.LerpUnclamped(_003CstartScale_003E5__9, _003CendScale_003E5__8, num2);
				_003Ct_003E5__5.localPosition = localPosition;
				_003Ct_003E5__5.localScale = localScale;
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

	private sealed class _003C_003Ec__DisplayClass240_0
	{
		public bool isWellDoneDone;

		public Match3Game _003C_003E4__this;

		public Action onComplete;

		internal void _003CDoWinAnimation_003Eb__1()
		{
			isWellDoneDone = true;
		}

		internal void _003CDoWinAnimation_003Eb__0()
		{
			_003C_003E4__this.animationEnum = null;
			_003C_003E4__this.gameScreen.tapToContinueContainer.Hide();
			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	private sealed class _003CDoWinAnimation_003Ed__240 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public Match3Game _003C_003E4__this;

		public Action onComplete;

		public WinScreen.InitArguments winScreenArguments;

		private _003C_003Ec__DisplayClass240_0 _003C_003E8__1;

		private int _003CcoinsPerMove_003E5__2;

		private int _003CadditionalCoins_003E5__3;

		private int _003CmovesRemaining_003E5__4;

		private List<Chip> _003CpowerupChips_003E5__5;

		private float _003Ctime_003E5__6;

		private float _003CmaxDuration_003E5__7;

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
		public _003CDoWinAnimation_003Ed__240(int _003C_003E1__state)
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
			Match3Game match3Game = _003C_003E4__this;
			float num3;
			int b2;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass240_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__1.onComplete = onComplete;
				_003CcoinsPerMove_003E5__2 = 1;
				_003C_003E8__1.isWellDoneDone = false;
				if (!match3Game.isWellDoneShown)
				{
					Action action = _003C_003E8__1._003CDoWinAnimation_003Eb__1;
					WellDoneContainer.InitArguments initArguments = new WellDoneContainer.InitArguments(action);
					match3Game.gameScreen.wellDoneContainer.Show(initArguments);
					match3Game.gameScreen.ShowConfetti();
					match3Game.board.currentCoins = winScreenArguments.baseStageWonCoins + 20;
					match3Game.gameScreen.goalsPanel.ShowCoins();
					match3Game.gameScreen.goalsPanel.SetCoinsCount(match3Game.board.currentCoins);
					goto IL_016b;
				}
				_003Ctime_003E5__6 = 0f;
				_003CmaxDuration_003E5__7 = 8f;
				goto IL_014f;
			case 1:
				_003C_003E1__state = -1;
				goto IL_014f;
			case 2:
				_003C_003E1__state = -1;
				goto IL_0244;
			case 3:
				_003C_003E1__state = -1;
				goto IL_03ae;
			case 4:
				_003C_003E1__state = -1;
				goto IL_03ae;
			case 5:
				_003C_003E1__state = -1;
				goto IL_04d6;
			case 6:
				{
					_003C_003E1__state = -1;
					goto IL_04d6;
				}
				IL_04d6:
				if (!match3Game.isBoardFullySettled)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 6;
					return true;
				}
				goto IL_03c1;
				IL_016b:
				_003CadditionalCoins_003E5__3 = 0;
				for (int i = 0; i < match3Game.board.sortedSlotsUpdateList.Count; i++)
				{
					Chip slotComponent = match3Game.board.sortedSlotsUpdateList[i].GetSlotComponent<Chip>();
					if (slotComponent != null && slotComponent.isPowerup)
					{
						int num2 = _003CcoinsPerMove_003E5__2;
						if (slotComponent.chipType == ChipType.Bomb)
						{
							num2 *= 2;
						}
						else if (slotComponent.chipType == ChipType.DiscoBall)
						{
							num2 *= 5;
						}
						slotComponent.carriesCoins = match3Game.CoinsPerChipType(slotComponent.chipType, _003CcoinsPerMove_003E5__2);
						_003CadditionalCoins_003E5__3 += slotComponent.carriesCoins;
					}
				}
				//_003CmovesRemaining_003E5__4 = match3Game.gameScreen.stageState.MovesRemaining;
				goto IL_0244;
				IL_03ae:
				if (!match3Game.isBoardFullySettled)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 4;
					return true;
				}
				_003CpowerupChips_003E5__5 = new List<Chip>();
				goto IL_03c1;
				IL_03c1:
				_003CpowerupChips_003E5__5.Clear();
				for (int j = 0; j < match3Game.board.sortedSlotsUpdateList.Count; j++)
				{
					Chip slotComponent2 = match3Game.board.sortedSlotsUpdateList[j].GetSlotComponent<Chip>();
					if (slotComponent2 != null && slotComponent2.isPowerup)
					{
						_003CpowerupChips_003E5__5.Add(slotComponent2);
					}
				}
				if (_003CpowerupChips_003E5__5.Count != 0 || !match3Game.isBoardFullySettled)
				{
					GGUtil.Shuffle(_003CpowerupChips_003E5__5);
					int b = 5;
					for (int k = 0; k < Mathf.Min(_003CpowerupChips_003E5__5.Count, b); k++)
					{
						Chip chip = _003CpowerupChips_003E5__5[k];
						if (chip != null && chip.slot != null)
						{
							chip.slot.OnDestroySlot(new SlotDestroyParams
							{
								activationDelay = 0.05f * (float)k
							});
						}
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 5;
					return true;
				}
				match3Game.gameScreen.tapToContinueContainer.Hide();
				if (_003C_003E8__1.onComplete != null)
				{
					_003C_003E8__1.onComplete();
				}
				return false;
				IL_0244:
				if (!_003C_003E8__1.isWellDoneDone)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				num3 = 0.1f;
				b2 = 20;
				Mathf.Max(_003CmovesRemaining_003E5__4, b2);
				for (int l = 0; l < _003CmovesRemaining_003E5__4; l++)
				{
					PlacePowerupAction placePowerupAction = new PlacePowerupAction();
					PlacePowerupAction.Parameters parameters = new PlacePowerupAction.Parameters();
					parameters.game = match3Game;
					int num4 = UnityEngine.Random.Range(0, 5);
					parameters.powerup = ChipType.HorizontalRocket;
					switch (num4)
					{
					case 0:
						parameters.powerup = ChipType.HorizontalRocket;
						break;
					case 1:
						parameters.powerup = ChipType.VerticalRocket;
						break;
					case 2:
						parameters.powerup = ChipType.Bomb;
						break;
					case 3:
						parameters.powerup = ChipType.SeekingMissle;
						break;
					default:
						parameters.powerup = ChipType.Bomb;
						break;
					}
					parameters.initialDelay = (float)l * num3;
					if (l <= _003CmovesRemaining_003E5__4)
					{
						parameters.addCoins = match3Game.CoinsPerChipType(parameters.powerup, _003CcoinsPerMove_003E5__2);
					}
					placePowerupAction.Init(parameters);
					match3Game.board.actionManager.AddAction(placePowerupAction);
					_003CadditionalCoins_003E5__3 += parameters.addCoins;
				}
				winScreenArguments.additionalCoins = _003CadditionalCoins_003E5__3;
				match3Game.gameScreen.tapToContinueContainer.Show(_003C_003E8__1._003CDoWinAnimation_003Eb__0);
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
				IL_014f:
				if (!match3Game.isBoardFullySettled || !match3Game.isWellDoneShownDone)
				{
					_003Ctime_003E5__6 += Time.deltaTime;
					if (!(_003Ctime_003E5__6 > _003CmaxDuration_003E5__7))
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
				}
				_003C_003E8__1.isWellDoneDone = true;
				goto IL_016b;
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

	private sealed class _003CDoWinScreenBoardOutAnimation_003Ed__242 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public Match3Game _003C_003E4__this;

		private float _003Ctime_003E5__2;

		private WinScreenBoardOutAnimation _003Csettings_003E5__3;

		private float _003Cduration_003E5__4;

		private Transform _003Ct_003E5__5;

		private Vector3 _003CstartPos_003E5__6;

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
		public _003CDoWinScreenBoardOutAnimation_003Ed__242(int _003C_003E1__state)
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
			Match3Game match3Game = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				_003Csettings_003E5__3 = Match3Settings.instance.winScreenBoardOutAnimation;
				_003Cduration_003E5__4 = _003Csettings_003E5__3.duration;
				_003Ct_003E5__5 = match3Game.boardContainer;
				_003CstartPos_003E5__6 = _003Ct_003E5__5.localPosition;
				Vector3 localScale = _003Ct_003E5__5.localScale;
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__2 <= _003Cduration_003E5__4)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float num2 = Mathf.InverseLerp(0f, _003Cduration_003E5__4, _003Ctime_003E5__2);
				if (_003Csettings_003E5__3.outCurve != null)
				{
					num2 = _003Csettings_003E5__3.outCurve.Evaluate(num2);
				}
				Vector3 localPosition = Vector3.LerpUnclamped(_003CstartPos_003E5__6, _003Csettings_003E5__3.offset * 2f + _003CstartPos_003E5__6, num2);
				_003Ct_003E5__5.localPosition = localPosition;
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

	public float timeScale = 1f;

	private HeuristicAIPlayer aiPlayer = new HeuristicAIPlayer();

	public string levelName;

	private long startTimestampTicks;

	private long endTimestampTicks;

	[NonSerialized]
	public LevelDefinition level;

	private bool gameStarted;

	private bool preventAutomatchesIfPossible_;

	private List<BuyMovesPricesConfig.OfferConfig> moveOffersBought = new List<BuyMovesPricesConfig.OfferConfig>();

	[NonSerialized]
	public GameScreen gameScreen;

	[SerializeField]
	private DifficultyChanger difficultyChanger = new DifficultyChanger();

	[SerializeField]
	private CameraShaker cameraShaker;

	[SerializeField]
	public PlayerInput input;

	[SerializeField]
	public SlotsRendererPool slotsRendererPool;

	[SerializeField]
	public Match3Particles particles;

	[SerializeField]
	private TilesBorderRenderer borderRenderer;

	[SerializeField]
	private HiddenElementBorderRenderer hiddenElementBorderRenderer;

	[SerializeField]
	private SnowCoverBorderRenderer snowCoverRenderer;

	[SerializeField]
	private BubbleSlotsBorderRenderer bubbleSlotsBorderRenderer;

	[SerializeField]
	private TilesBorderRenderer slotsRenderer;

	[SerializeField]
	private TilesBorderRenderer conveyorBorderRenderer;

	[SerializeField]
	private BorderTilemapRenderer conveyorHoleRenderer;

	[SerializeField]
	private TilesBorderRenderer conveyorSlotsRenderer;

	[SerializeField]
	private ChocolateBorderRenderer chocolateBorderRenderer;

	[SerializeField]
	public TutorialSlotHighlighter tutorialHighlighter = new TutorialSlotHighlighter();

	[SerializeField]
	private Transform boardContainer;

	public Vector2 slotPhysicalSize = new Vector2(1f, 1f);

	private ShowPotentialMatchAction showMatchAction = new ShowPotentialMatchAction();

	public float gravity = 9f;

	[SerializeField]
	private List<PieceCreatorPool> pieceCreatorPools = new List<PieceCreatorPool>();

	private List<Slot> affectingList = new List<Slot>();

	[NonSerialized]
	public Match3Board board = new Match3Board();

	[NonSerialized]
	public Match3Goals goals = new Match3Goals();

	[NonSerialized]
	public ExtraFallingChips extraFallingChips = new ExtraFallingChips();

	[NonSerialized]
	public Match3GameParams initParams;

	private StartGameArguments startGameArguments;

	private List<TutorialMatchProgress> tutorialMatchProgressList = new List<TutorialMatchProgress>();

	private ShowTutorialMaskAction tutorialAction;

	public CreateBoardArguments createBoardArguments;

	private List<IntVector2> wallDirections = new List<IntVector2>();

	private IEnumerator shuffleBoardAnimation;

	public Dictionary<ItemColor, int> attachedElementsPerItemColor = new Dictionary<ItemColor, int>();

	private List<PotentialMatches.CompoundSlotsSet.MatchType> matchTypeToFind = new List<PotentialMatches.CompoundSlotsSet.MatchType>();

	private List<PowerupCombines.CombineType> combineTypeToFind = new List<PowerupCombines.CombineType>();

	private List<Slot> discoBallSlots = new List<Slot>();

	protected List<PotentialMatch> potentialMatchesList = new List<PotentialMatch>();

	private bool isWellDoneShown;

	private bool isWellDoneShownDone;

	private int movesSettledCount;

	private IEnumerator animationEnum;

	private List<Slot> allNeighbourSlots = new List<Slot>();

	private List<SlotDestroyNeighbour> destroyNeighbourSlots = new List<SlotDestroyNeighbour>();

	private MaximumColorHelper maxColorHelper = new MaximumColorHelper();

	public bool preventAutomatchesIfPossible
	{
		get
		{
			if (!preventAutomatchesIfPossible_)
			{
				return Match3Settings.instance.generalSettings.preventAutomatchesIfPossible;
			}
			return true;
		}
	}

	public bool strictAsPossibleToprevent
	{
		get
		{
			if (!preventAutomatchesIfPossible_)
			{
				return Match3Settings.instance.generalSettings.strictAsPossibleToPrevent;
			}
			return true;
		}
	}

	public long totalTimePlayed => endTimestampTicks - startTimestampTicks;

	public int timesBoughtMoves => moveOffersBought.Count;

	public int totalCoinsSpent
	{
		get
		{
			int num = 0;
			for (int i = 0; i < moveOffersBought.Count; i++)
			{
				BuyMovesPricesConfig.OfferConfig offerConfig = moveOffersBought[i];
				if (offerConfig.price.currency == CurrencyType.coins)
				{
					num += offerConfig.price.cost;
				}
			}
			return num;
		}
	}

	public long timePlayed => DateTime.UtcNow.Ticks - startTimestampTicks;

	public Match3Settings settings => Match3Settings.instance;

	public float boardContainerScale => boardContainer.localScale.x;

	public Vector3 bottomLeft
	{
		get
		{
			Vector3 result = default(Vector3);
			result.x = (0f - slotPhysicalSize.x) * (float)(board.size.x - 1) * 0.5f;
			result.y = (0f - slotPhysicalSize.y) * (float)(board.size.y - 1) * 0.5f;
			return result;
		}
	}

	public int SeekingMissleCrossRadius
	{
		get
		{
			if (Match3Settings.instance.generalSettings.seekingRangeType == GeneralSettings.SeekingRangeType.Normal)
			{
				return 1;
			}
			return 0;
		}
	}

	public bool isHudEnabled
	{
		get
		{
			if (initParams == null)
			{
				return true;
			}
			return !initParams.isHudDissabled;
		}
	}

	private bool isTutorialActive => tutorialAction != null;

	private SuggestMoveType suggestMoveType => level.suggestMoveType;

	private ShowPotentialMatchSetting showPotentialMatchSetting => level.suggestMoveSetting;

	public bool isUserInteractionSuspended
	{
		get
		{
			if (!board.isInteractionSuspended && !board.isInteractionSuspendedBecausePowerupAnimation)
			{
				return board.bubblesBoardComponent.isWaitingForBubblesToBurst;
			}
			return true;
		}
	}

	public bool isBubblesSuspended
	{
		get
		{
			if (!board.isGameEnded)
			{
				return board.isInteractionSuspended;
			}
			return true;
		}
	}

	public bool isConveyorSuspended
	{
		get
		{
			if (!board.isGameEnded && !board.isInteractionSuspended)
			{
				return board.bubblesBoardComponent.isWaitingForBubblesToBurst;
			}
			return true;
		}
	}

	public bool hasPlayedAnyMoves => board.userMovesCount > 0;

	public bool isConveyorMoving
	{
		get
		{
			for (int i = 0; i < board.boardComponents.Count; i++)
			{
				ConveyorBeltBoardComponent conveyorBeltBoardComponent = board.boardComponents[i] as ConveyorBeltBoardComponent;
				if (conveyorBeltBoardComponent != null && conveyorBeltBoardComponent.isMoving)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool isBoardFullySettled
	{
		get
		{
			if (board.currentMatchesCount == 0 && board.actionManager.ActionCount == 0)
			{
				return !isAnySlotMoving;
			}
			return false;
		}
	}

	public bool isAnySlotMoving
	{
		get
		{
			List<Slot> sortedSlotsUpdateList = board.sortedSlotsUpdateList;
			for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
			{
				if (sortedSlotsUpdateList[i].isMoving)
				{
					return true;
				}
			}
			return false;
		}
	}

	public int TotalSlotsGoalsRemainingCount
	{
		get
		{
			List<Match3Goals.GoalBase> list = goals.goals;
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				Match3Goals.GoalBase goalBase = list[i];
				if (goalBase.config.chipType == ChipType.FallingGingerbreadMan)
				{
					int num2 = goalBase.RemainingCount;
					if (num2 == 0)
					{
						continue;
					}
					int num3 = Mathf.Max(board.size.x, board.size.y);
					for (int j = 0; j < board.sortedSlotsUpdateList.Count; j++)
					{
						Slot slot = board.sortedSlotsUpdateList[j];
						if (slot == null)
						{
							continue;
						}
						Chip slotComponent = slot.GetSlotComponent<Chip>();
						if (slotComponent == null || slotComponent.chipType != ChipType.FallingGingerbreadMan)
						{
							continue;
						}
						num2 = Mathf.Max(0, num2 - 1);
						int num4 = 0;
						Slot slot2 = slot;
						while (slot2 != null && slot2 != null)
						{
							slot2 = slot2.NextSlotToPushToWithoutSandflow();
							num4++;
							if (num4 >= num3)
							{
								break;
							}
						}
						num += Mathf.Max(num4 - 1, 0);
					}
					num += num2 * num3;
				}
				else
				{
					num += goalBase.RemainingCount;
				}
			}
			return num;
		}
	}

	public bool IsPreventAutomatedMachesIfPossible()
	{
		if (board.isGameEnded)
		{
			return true;
		}
		if (!preventAutomatchesIfPossible)
		{
			return false;
		}
		if (!level.useChanceToNotPreventChipMatching)
		{
			return true;
		}
		if ((float)RandomRange(0, 100) > level.chanceToNotPreventChipMatching)
		{
			return false;
		}
		return true;
	}

	public Vector3 WorldToBoardPosition(Vector3 worldPosition)
	{
		Vector3 result = boardContainer.InverseTransformPoint(worldPosition);
		result.z = 0f;
		return result;
	}

	public Vector3 LocalToWorldPosition(Vector3 localPosition)
	{
		Vector3 result = boardContainer.TransformPoint(localPosition);
		result.z = 0f;
		return result;
	}

	public ComponentPool GetPool(PieceType type)
	{
		for (int i = 0; i < pieceCreatorPools.Count; i++)
		{
			PieceCreatorPool pieceCreatorPool = pieceCreatorPools[i];
			if (pieceCreatorPool.type == type)
			{
				return pieceCreatorPool.pool;
			}
		}
		return null;
	}

	public ComponentPool GetPool(PieceType type, ChipType chipType, ItemColor itemColor)
	{
		for (int i = 0; i < pieceCreatorPools.Count; i++)
		{
			PieceCreatorPool pieceCreatorPool = pieceCreatorPools[i];
			if (pieceCreatorPool.type == type && pieceCreatorPool.chipTypeList.Contains(chipType) && pieceCreatorPool.itemColorList.Contains(itemColor))
			{
				return pieceCreatorPool.pool;
			}
		}
		return null;
	}

	public List<Slot> GetCross(IntVector2 centerPos, int radius)
	{
		List<Slot> area = GetArea(centerPos, radius);
		for (int i = centerPos.x - radius; i <= centerPos.x + radius; i++)
		{
			for (int j = 0; j <= board.size.y; j++)
			{
				if (Mathf.Abs(j - centerPos.y) > radius)
				{
					Slot slot = GetSlot(new IntVector2(i, j));
					if (slot != null)
					{
						area.Add(slot);
					}
				}
			}
		}
		for (int k = centerPos.y - radius; k <= centerPos.y + radius; k++)
		{
			for (int l = 0; l <= board.size.x; l++)
			{
				if (Mathf.Abs(l - centerPos.x) > radius)
				{
					Slot slot2 = GetSlot(new IntVector2(l, k));
					if (slot2 != null)
					{
						area.Add(slot2);
					}
				}
			}
		}
		return area;
	}

	public List<Slot> GetSeekingMissleArea(IntVector2 centerPos)
	{
		GeneralSettings.SeekingRangeType seekingRangeType = Match3Settings.instance.generalSettings.seekingRangeType;
		return GetCrossArea(centerPos, SeekingMissleCrossRadius);
	}

	public List<Slot> GetCrossArea(IntVector2 centerPos, int maxRadius)
	{
		List<Slot> list = affectingList;
		list.Clear();
		for (int i = centerPos.x - maxRadius; i <= centerPos.x + maxRadius; i++)
		{
			for (int j = centerPos.y - maxRadius; j <= centerPos.y + maxRadius; j++)
			{
				if (Mathf.Abs(i - centerPos.x) + Mathf.Abs(j - centerPos.y) <= maxRadius)
				{
					Slot slot = GetSlot(new IntVector2(i, j));
					if (slot != null)
					{
						list.Add(slot);
					}
				}
			}
		}
		return list;
	}

	public List<Slot> GetAreaOfEffect(ChipType chipType, IntVector2 centerPos)
	{
		affectingList.Clear();
		switch (chipType)
		{
		case ChipType.Bomb:
			return GetBombArea(centerPos, 2);
		case ChipType.HorizontalRocket:
			return GetHorizontalLine(centerPos);
		case ChipType.VerticalRocket:
			return GetVerticalLine(centerPos);
		case ChipType.SeekingMissle:
			return GetSeekingMissleArea(centerPos);
		default:
			return affectingList;
		}
	}

	public List<Slot> GetBombArea(IntVector2 centerPos, int maxRadius)
	{
		GeneralSettings.BombRangeType bombRangeType = Match3Settings.instance.generalSettings.bombRangeType;
		if (bombRangeType == GeneralSettings.BombRangeType.Candy)
		{
			maxRadius = Mathf.Max(0, maxRadius - 1);
		}
		List<Slot> list = affectingList;
		list.Clear();
		for (int i = centerPos.x - maxRadius; i <= centerPos.x + maxRadius; i++)
		{
			int num = Mathf.Abs(i - centerPos.x);
			int num2 = maxRadius;
			switch (bombRangeType)
			{
			case GeneralSettings.BombRangeType.Full:
				num2 = maxRadius;
				break;
			case GeneralSettings.BombRangeType.Circle:
				num2 = Mathf.Min(maxRadius - num + 1, maxRadius);
				break;
			case GeneralSettings.BombRangeType.Diamond:
				num2 = Mathf.Min(maxRadius - num, maxRadius);
				break;
			case GeneralSettings.BombRangeType.Candy:
				num2 = maxRadius;
				break;
			}
			for (int j = centerPos.y - num2; j <= centerPos.y + num2; j++)
			{
				Slot slot = GetSlot(new IntVector2(i, j));
				if (slot != null)
				{
					list.Add(slot);
				}
			}
		}
		return list;
	}

	public List<Slot> GetArea(IntVector2 centerPos, int maxRadius)
	{
		List<Slot> list = affectingList;
		list.Clear();
		for (int i = centerPos.x - maxRadius; i <= centerPos.x + maxRadius; i++)
		{
			for (int j = centerPos.y - maxRadius; j <= centerPos.y + maxRadius; j++)
			{
				Slot slot = GetSlot(new IntVector2(i, j));
				if (slot != null)
				{
					list.Add(slot);
				}
			}
		}
		return list;
	}

	public List<Slot> GetAllPlayingSlots()
	{
		List<Slot> list = affectingList;
		list.Clear();
		Slot[] slots = board.slots;
		foreach (Slot slot in slots)
		{
			if (slot != null)
			{
				list.Add(slot);
			}
		}
		return list;
	}

	public List<Slot> GetVerticalLine(IntVector2 centerPos)
	{
		List<Slot> list = affectingList;
		list.Clear();
		for (int i = 0; i <= board.size.y; i++)
		{
			Slot slot = GetSlot(new IntVector2(centerPos.x, i));
			if (slot != null)
			{
				list.Add(slot);
			}
		}
		return list;
	}

	public List<Slot> GetHorizontalLine(IntVector2 centerPos)
	{
		List<Slot> list = affectingList;
		list.Clear();
		for (int i = 0; i <= board.size.x; i++)
		{
			Slot slot = GetSlot(new IntVector2(i, centerPos.y));
			if (slot != null)
			{
				list.Add(slot);
			}
		}
		return list;
	}

	public ComponentPool GetPool(PieceType type, ChipType chipType)
	{
		for (int i = 0; i < pieceCreatorPools.Count; i++)
		{
			PieceCreatorPool pieceCreatorPool = pieceCreatorPools[i];
			if (pieceCreatorPool.type == type && pieceCreatorPool.chipTypeList.Contains(chipType))
			{
				return pieceCreatorPool.pool;
			}
		}
		return null;
	}

	public Slot GetSlot(IntVector2 position)
	{
		return board.GetSlot(position);
	}

	public void Play(GGSoundSystem.SFXType sound)
	{
		if (isHudEnabled)
		{
			GGSoundSystem.PlayParameters sound2 = default(GGSoundSystem.PlayParameters);
			sound2.soundType = sound;
			Play(sound2);
		}
	}

	public void Play(GGSoundSystem.PlayParameters sound)
	{
		if (!board.isGameSoundsSuspended && isHudEnabled)
		{
			sound.frameNumber = Time.frameCount;
			GGSoundSystem.Play(sound);
		}
	}

	public void TapOnSlot(IntVector2 pos1, SwapParams swapParams = null)
	{
		if (isUserInteractionSuspended)
		{
			return;
		}
		Slot slot = GetSlot(pos1);
		if (slot == null)
		{
			return;
		}
		Chip slotComponent = slot.GetSlotComponent<Chip>();
		if (slotComponent != null && !slot.isTapToActivateSuspended)
		{
			GameStats.Move move = new GameStats.Move();
			move.fromPosition = (move.toPosition = pos1);
			move.moveType = GameStats.MoveType.PowerupTap;
			move.frameWhenActivated = board.currentFrameIndex;
			board.gameStats.moves.Add(move);
			if (slotComponent.isPowerup)
			{
				board.gameStats.powerupsUsedByTap++;
			}
			SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
			slotDestroyParams.isFromTap = true;
			slotDestroyParams.swapParams = swapParams;
			if (slotComponent.canBeTappedToActivate && EnergyManager.instance.HasEnergyForOneLife())
			{
				slot.OnDestroySlot(slotDestroyParams);
				EnergyManager.instance.SpendEnergy(10);
			}
		}
	}

	public bool TrySwitchSlots(Slot slot1, Slot slot2, bool instant)
	{
		if (slot1 == null || slot2 == null)
		{
			return false;
		}
		return TrySwitchSlots(slot1.position, slot2.position, instant);
	}

	public bool TrySwitchSlots(IntVector2 pos1, IntVector2 pos2, bool instant)
	{
		SwitchSlotsArguments arguments = default(SwitchSlotsArguments);
		arguments.pos1 = pos1;
		arguments.pos2 = pos2;
		arguments.instant = instant;
		return TrySwitchSlots(arguments);
	}

	public void TryShowSwitchNotPossible(IntVector2 pos1, IntVector2 pos2)
	{
		SwitchSlotsArguments arguments = default(SwitchSlotsArguments);
		arguments.pos1 = pos1;
		arguments.pos2 = pos2;
		TryShowSwitchNotPossible(arguments);
	}

	private void TryShowSwitchNotPossible(SwitchSlotsArguments arguments)
	{
		IntVector2 pos = arguments.pos1;
		IntVector2 pos2 = arguments.pos2;
		Slot slot = GetSlot(pos);
		Slot slot2 = GetSlot(pos2);
		if (slot != null && !slot.isSlotMatchingSuspended && !slot.isSlotGravitySuspended && !slot.isSlotSwipingSuspended)
		{
			ShowSwapNotPossibleAction.SwapChipsParams chipParams = default(ShowSwapNotPossibleAction.SwapChipsParams);
			chipParams.slot1 = slot;
			chipParams.positionToMoveSlot1 = pos2;
			chipParams.game = this;
			if (slot2 != null && slot2.isSlotGravitySuspendedByComponent)
			{
				chipParams.wobble = true;
			}
			ShowSwapNotPossibleAction showSwapNotPossibleAction = new ShowSwapNotPossibleAction();
			showSwapNotPossibleAction.Init(chipParams);
			board.actionManager.AddAction(showSwapNotPossibleAction);
		}
	}

	public bool TrySwitchSlots(SwitchSlotsArguments arguments)
	{
		IntVector2 pos = arguments.pos1;
		IntVector2 pos2 = arguments.pos2;
		bool instant = arguments.instant;
		if (isUserInteractionSuspended)
		{
			return false;
		}
		Slot slot = GetSlot(pos);
		Slot slot2 = GetSlot(pos2);
		if (slot == null || slot2 == null)
		{
			if (slot != null)
			{
				TryShowSwitchNotPossible(arguments);
			}
			return false;
		}
		
		if (slot.isSlotMatchingSuspended || slot2.isSlotMatchingSuspended)
		{
			TryShowSwitchNotPossible(arguments);
			return false;
		}
		if (slot.isSlotGravitySuspended || slot2.isSlotGravitySuspended)
		{
			TryShowSwitchNotPossible(arguments);
			return false;
		}
		if (slot.isSlotSwipingSuspended || slot2.isSlotSwapSuspended)
		{
			TryShowSwitchNotPossible(arguments);
			return false;
		}
		if (slot.isSlotSwipingSuspendedForSlot(slot2) || slot.isSwipeSuspendedTo(slot2))
		{
			TryShowSwitchNotPossible(arguments);
			return false;
		}

		SwapToMatchAction.PowerupList powerupList = new SwapToMatchAction.PowerupList();
		powerupList.Add(slot.GetSlotComponent<Chip>());
		powerupList.Add(slot2.GetSlotComponent<Chip>());
		GameStats.Move move = new GameStats.Move();
		move.fromPosition = pos;
		move.toPosition = pos2;
		move.moveType = GameStats.MoveType.Match;
		move.frameWhenActivated = board.currentFrameIndex;
		if (powerupList.isMixingPowerups)
		{
			board.gameStats.powerupsMixed++;
			move.moveType = GameStats.MoveType.PowerupMix;
		}
		else if (powerupList.isActivatingPowerup)
		{
			board.gameStats.powerupsUsedBySwipe++;
			move.moveType = GameStats.MoveType.PowerupActivation;
		}
		board.matchCounter.OnUserMadeMove();
		board.gameStats.moves.Add(move);
		
		SwapToMatchAction swapToMatchAction = new SwapToMatchAction();
		SwapToMatchAction.SwapActionProperties swapProperties = default(SwapToMatchAction.SwapActionProperties);
		swapProperties.slot1 = slot;
		swapProperties.slot2 = slot2;
		swapProperties.isInstant = instant;
		swapProperties.bolts = arguments.bolts;
		swapProperties.switchSlotsArgument = arguments;
		swapToMatchAction.Init(swapProperties);
		board.actionManager.AddAction(swapToMatchAction);
		return true;
	}

	public int SlotsDistanceToEndOfBoard(IntVector2 pos, IntVector2 direction)
	{
		if (direction == IntVector2.zero)
		{
			return 0;
		}
		if (direction.x > 0)
		{
			return board.size.x - pos.x - 1;
		}
		if (direction.x < 0)
		{
			return pos.x;
		}
		if (direction.y > 0)
		{
			return board.size.y - pos.y - 1;
		}
		if (direction.y < 0)
		{
			return pos.y;
		}
		return 0;
	}

	public Vector3 LocalPositionOfCenter(IntVector2 position)
	{
		return bottomLeft + new Vector3((float)position.x * slotPhysicalSize.x, (float)position.y * slotPhysicalSize.y, 0f);
	}

	public IntVector2 BoardPositionFromLocalPosition(Vector3 position)
	{
		position -= bottomLeft;
		int x = Mathf.FloorToInt(position.x / slotPhysicalSize.x);
		int y = Mathf.FloorToInt(position.y / slotPhysicalSize.y);
		return new IntVector2(x, y);
	}

	public IntVector2 BoardPositionFromLocalPositionRound(Vector3 position)
	{
		position -= bottomLeft;
		int x = Mathf.RoundToInt(position.x / slotPhysicalSize.x);
		int y = Mathf.RoundToInt(position.y / slotPhysicalSize.y);
		return new IntVector2(x, y);
	}

	public IntVector2 ClosestBoardPositionFromLocalPosition(Vector3 position)
	{
		position -= bottomLeft;
		int x = Mathf.FloorToInt((position.x + slotPhysicalSize.x * 0.5f) / slotPhysicalSize.x);
		int y = Mathf.FloorToInt((position.y + slotPhysicalSize.y * 0.5f) / slotPhysicalSize.y);
		return new IntVector2(x, y);
	}

	public void SetHasBounceOnAllChips(bool hasBounce)
	{
		for (int i = 0; i < board.sortedSlotsUpdateList.Count; i++)
		{
			Chip slotComponent = board.sortedSlotsUpdateList[i].GetSlotComponent<Chip>();
			if (slotComponent != null)
			{
				ChipBehaviour componentBehaviour = slotComponent.GetComponentBehaviour<ChipBehaviour>();
				if (!(componentBehaviour == null))
				{
					componentBehaviour.hasBounce = hasBounce;
				}
			}
		}
	}

	private TutorialMatchProgress GetOrCreateTutorialMatchProgress(LevelDefinition.TutorialMatch tutorialMatch)
	{
		for (int i = 0; i < tutorialMatchProgressList.Count; i++)
		{
			TutorialMatchProgress tutorialMatchProgress = tutorialMatchProgressList[i];
			if (tutorialMatchProgress.tutorialMatch == tutorialMatch)
			{
				return tutorialMatchProgress;
			}
		}
		TutorialMatchProgress tutorialMatchProgress2 = new TutorialMatchProgress();
		tutorialMatchProgress2.tutorialMatch = tutorialMatch;
		tutorialMatchProgressList.Add(tutorialMatchProgress2);
		return tutorialMatchProgress2;
	}

	public void UpdateTutorialMatches(bool isBoardSettled)
	{
		if (tutorialAction != null)
		{
			tutorialAction.isBoardSettled = isBoardSettled;
		}
		if (isUserInteractionSuspended)
		{
			return;
		}
		int userMovesCount = board.userMovesCount;
		if (userMovesCount >= level.tutorialMatches.Count || !isBoardSettled)
		{
			return;
		}
		LevelDefinition.TutorialMatch tutorialMatch = level.tutorialMatches[userMovesCount];
		TutorialMatchProgress orCreateTutorialMatchProgress = GetOrCreateTutorialMatchProgress(tutorialMatch);
		if (orCreateTutorialMatchProgress.isStarted)
		{
			return;
		}
		orCreateTutorialMatchProgress.isStarted = true;
		if (tutorialMatch.isEnabled)
		{
			ShowTutorialMaskAction.Parameters parameters = new ShowTutorialMaskAction.Parameters();
			parameters.game = this;
			parameters.match = tutorialMatch;
			if (userMovesCount != level.tutorialMatches.Count - 1)
			{
				parameters.onMiddle = OnTutorialMiddle;
				parameters.onEnd = OnTutorialEnd;
			}
			else
			{
				parameters.onEnd = OnLastTutorialEnd;
			}
			tutorialAction = new ShowTutorialMaskAction();
			tutorialAction.Init(parameters);
			board.nonChangeStateActionMenager.AddAction(tutorialAction);
		}
	}

	private void OnLastTutorialEnd()
	{
		tutorialAction = null;
		PutBoosters(startGameArguments);
	}

	private void OnTutorialMiddle()
	{
		board.isInteractionSuspended = true;
	}

	private void OnTutorialEnd()
	{
		board.isInteractionSuspended = false;
		tutorialAction = null;
	}

	public void ShakeCamera(GeneralSettings.CameraShakeSettings shakeSettings)
	{
		if (!(cameraShaker == null))
		{
			cameraShaker.enabled = true;
			float shakeScale = Match3Settings.instance.generalSettings.shakeScale;
			if (!(shakeScale <= 0f))
			{
				cameraShaker.ShakeOnce(shakeSettings.magnitude * shakeScale, shakeSettings.roughness, shakeSettings.fadeInTime, shakeSettings.fadeOutTime, shakeSettings.posInfluence, shakeSettings.rotInfluence);
			}
		}
	}

	public void ShakeCamera()
	{
		GeneralSettings.CameraShakeSettings shakeSettings = Match3Settings.instance.generalSettings.shakeSettings;
		ShakeCamera(shakeSettings);
	}

	public void StartGame(StartGameArguments arguments)
	{
		SetHasBounceOnAllChips(hasBounce: true);
		gameStarted = true;
		UpdateTutorialMatches(isBoardSettled: true);
		startGameArguments = arguments;
		if (tutorialAction == null)
		{
			PutBoosters(arguments);
		}
		startTimestampTicks = DateTime.UtcNow.Ticks;
		if(_level == null)
			_level = level.Clone();
	}

	private void PutBoosters(StartGameArguments arguments)
	{
		if (arguments.putBoosters && initParams.activeBoosters != null && initParams.activeBoosters.Count > 0)
		{
			if (initParams.giftBoosterLevel > 0)
			{
				gameScreen.rankedBoostersStartAnimation.Show(initParams.giftBoosterLevel);
			}
			List<BoosterConfig> activeBoosters = initParams.activeBoosters;
			for (int i = 0; i < activeBoosters.Count; i++)
			{
				BoosterConfig boosterConfig = activeBoosters[i];
				PlacePowerupAction placePowerupAction = new PlacePowerupAction();
				PlacePowerupAction.Parameters parameters = new PlacePowerupAction.Parameters();
				parameters.game = this;
				parameters.internalAnimation = true;
				parameters.powerup = BoosterConfig.BoosterToChipType(boosterConfig.boosterType);
				parameters.initialDelay = (float)i * Match3Settings.instance.placePowerupActionSettings.delayBetweenPowerups;
				if (initParams.giftBoosterLevel > 0)
				{
					parameters.initialDelay += gameScreen.rankedBoostersStartAnimation.boosterDelay;
				}
				if (i == activeBoosters.Count - 1)
				{
					parameters.onComplete = _003CPutBoosters_003Eb__108_0;
				}
				placePowerupAction.Init(parameters);
				board.actionManager.AddAction(placePowerupAction);
				Analytics.BoosterUsedEvent boosterUsedEvent = new Analytics.BoosterUsedEvent();
				boosterUsedEvent.booster = boosterConfig;
				boosterUsedEvent.stageState = gameScreen.stageState;
				boosterUsedEvent.Send();
			}
		}
		else
		{
			HighlightAllGoals();
		}
	}

	public void Init(Camera mainCamera, GameScreen gameScreen, Match3GameParams initParams)
	{
		board.isInteractionSuspended = false;
		input.SetCamera(mainCamera);
		this.initParams = initParams;
		this.gameScreen = gameScreen;
		tutorialHighlighter.Init(gameScreen);
		int num = (int)DateTime.Now.Ticks;
		if (initParams.setRandomSeed)
		{
			num = initParams.randomSeed;
		}
		board.randomProvider.seed = num;
		board.randomProvider.Init();
		board.gameStats.initialSeed = num;
	}

	public void Callback_ShowActivatePowerup(PowerupsDB.PowerupDefinition powerup)
	{
		if (!isTutorialActive && !board.isGameEnded && !board.isShufflingBoard && !board.isEndConditionReached && !isUserInteractionSuspended)
		{
			if (powerup.ownedCount <= 0)
			{
				BuyPowerupDialog.InitArguments initArguments = default(BuyPowerupDialog.InitArguments);
				initArguments.powerup = powerup;
				initArguments.onSuccess = Callback_OnBuyPowerupComplete;
				BuyPowerupDialog.Show(initArguments);
			}
			else
			{
				PowerupPlacementHandler.InitArguments initArguments2 = default(PowerupPlacementHandler.InitArguments);
				initArguments2.game = this;
				initArguments2.powerup = powerup;
				initArguments2.onComplete = Callback_OnPlacePowerup;
				gameScreen.powerupPlacement.Show(initArguments2);
				board.isPowerupSelectionActive = true;
				showMatchAction.Stop();
			}
		}
	}

	private void Callback_OnPlacePowerup(PowerupPlacementHandler.PlacementCompleteArguments completeArguments)
	{
		board.isPowerupSelectionActive = false;
		gameScreen.powerupsPanel.Refresh();
		if (!completeArguments.isCancel)
		{
			HammerHitAction.InitArguments initArguments = default(HammerHitAction.InitArguments);
			initArguments.game = this;
			initArguments.completeArguments = completeArguments;
			HammerHitAction hammerHitAction = new HammerHitAction();
			hammerHitAction.Init(initArguments);
			PowerupsDB.PowerupDefinition powerup = completeArguments.initArguments.powerup;
			completeArguments.initArguments.powerup.usedCount = completeArguments.initArguments.powerup.usedCount + 1;
			gameScreen.powerupsPanel.Refresh();
			board.actionManager.AddAction(hammerHitAction);
			if (powerup.type == PowerupType.Hammer)
			{
				gameScreen.stageState.hammersUsed++;
			}
			else
			{
				gameScreen.stageState.powerHammersUsed++;
			}
		}
	}

	private void Callback_OnBuyPowerupComplete(bool success)
	{
		gameScreen.powerupsPanel.Refresh();
		gameScreen.CurrencyPanel.SetLabels();
	}

	public void CreateBoard(LevelDefinition level)
	{
		CreateBoardArguments arguments = default(CreateBoardArguments);
		arguments.level = level;
		CreateBoard(arguments);
	}

	public void SetStageDifficulty(Match3StagesDB.Stage.Difficulty difficulty)
	{
		difficultyChanger.Apply(difficulty);
	}

	// private class BoardInfo
	// {
	// 	public Vector2Int Size;
	// 	public Vector3 Offset;
	// 	public LevelDefinition LevelDefinition;
	// }
	//
	// public void CreateBoard(BoardInfo boardInfo)
	// {
	// 	Vector3 offset = boardInfo.Offset;
	// 	if (initParams.isHudDissabled)
	// 	{
	// 		GGUtil.Hide(boardContainer);
	// 	}
	// 	level = boardInfo.LevelDefinition;
	// 	int width = boardInfo.Size.x;
	// 	int height = boardInfo.Size.y;
	// 	preventAutomatchesIfPossible_ = level.isPreventingGeneratorChipMatching;
	// 	RectTransform gamePlayAreaContainer = gameScreen.gamePlayAreaContainer;
	// 	Vector3[] array = new Vector3[4];
	// 	gamePlayAreaContainer.GetWorldCorners(array);
	// 	float num = 0.25f;
	// 	Vector2 vector = new Vector2(width * slotPhysicalSize.x + 2f * num, height * slotPhysicalSize.x + 2f * num);
	// 	Vector2 vector2 = new Vector2(array[2].x - array[0].x, array[2].y - array[0].y);
	// 	float num2 = Mathf.Min(vector2.x / vector.x, vector2.y / vector.y);
	// 	boardContainer.localScale = new Vector3(num2, num2, 1f);
	// 	boardContainer.position = gamePlayAreaContainer.transform.position + offset;
	// 	board = new Match3Board();
	// 	IntVector2 size = board.size;
	// 	size.x = width;
	// 	size.y = height;
	// 	board.generationSettings = level.generationSettings;
	// 	board.Init(size);
	// 	borderRenderer.slotSize = slotPhysicalSize.x;
	// 	slotsRenderer.slotSize = slotPhysicalSize.x;
	// 	borderRenderer.ShowBorderOnLevel(level);
	// 	slotsRenderer.ShowSlotsOnLevel(level);
	// 	bool hasMoreMoves = false;
	// 	LevelDefinition.ConveyorBelts conveyorBelts = level.GetConveyorBelts();
	// 	PopulateBoard.Params @params = new PopulateBoard.Params();
	// 	@params.randomProvider = board.randomProvider;
	// 	if (level.generationSettings.isConfigured)
	// 	{
	// 		List<LevelDefinition.ChipGenerationSettings.ChipSetting> chipSettings = level.generationSettings.chipSettings;
	// 		for (int i = 0; i < chipSettings.Count; i++)
	// 		{
	// 			LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting = chipSettings[i];
	// 			if (chipSetting.chipType == ChipType.Chip)
	// 			{
	// 				if (@params.availableColors.Contains(chipSetting.itemColor))
	// 				{
	// 					UnityEngine.Debug.Log("DUPLICATE AVAILABLE COLORS");
	// 					continue;
	// 				}
	// 				board.availableColors.Add(chipSetting.itemColor);
	// 				@params.availableColors.Add(chipSetting.itemColor);
	// 			}
	// 		}
	// 	}
	// 	else
	// 	{
	// 		for (int j = 0; j < level.numChips; j++)
	// 		{
	// 			@params.availableColors.Add((ItemColor)j);
	// 			board.availableColors.Add((ItemColor)j);
	// 		}
	// 	}
	// 	particles.disableParticles = initParams.disableParticles;
	// 	@params.maxPotentialMatches = Match3Settings.instance.maxPotentialMatchesAtStart;
	// 	board.populateBoard.RandomPopulate(level, @params);
	// 	for (int k = 0; k < size.y; k++)
	// 	{
	// 		for (int l = 0; l < size.x; l++)
	// 		{
	// 			IntVector2 position = new IntVector2(l, k);
	// 			LevelDefinition.SlotDefinition slot = levelDefinition.GetSlot(position);
	// 			if (slot.slotType == SlotType.Empty || (slot.chipType == ChipType.EmptyConveyorSpace && !conveyorBelts.IsPartOfConveyor(position)))
	// 			{
	// 				continue;
	// 			}
	// 			Slot slot2 = new Slot();
	// 			slot2.canGenerateChip = slot.generatorSettings.isGeneratorOn;
	// 			slot2.generatorSettings = slot.generatorSettings;
	// 			slot2.generatorSlotSettings = levelDefinition.GetGeneratorSlotSettings(slot.generatorSettings.slotGeneratorSetupIndex);
	// 			slot2.position = new IntVector2(l, k);
	// 			slot2.Init(this);
	// 			if (slot2.generatorSlotSettings != null)
	// 			{
	// 				AddSpecialGenerator(slot2);
	// 			}
	// 			if (slot.hasNet)
	// 			{
	// 				AddNetToSlot(slot2, slot);
	// 			}
	// 			if (slot.hasBox)
	// 			{
	// 				AddBoxToSlot(slot2, slot, levelDefinition);
	// 			}
	// 			if (slot.hasBubbles)
	// 			{
	// 				AddBubblesToSlot(slot2);
	// 			}
	// 			if (slot.hasSnowCover)
	// 			{
	// 				AddSnowCoverToSlot(slot2);
	// 			}
	// 			if (slot.hasBasket)
	// 			{
	// 				AddBasketToSlot(slot2, slot);
	// 			}
	// 			if (slot.hasChain)
	// 			{
	// 				AddChainToSlot(slot2, slot);
	// 			}
	// 			if (slot.wallSettings.isWallActive)
	// 			{
	// 				AddWallToSlot(slot2, slot);
	// 			}
	// 			if (slot.colorSlateLevel > 0 && (slot.colorSlateLevel > 1 || levelDefinition.burriedElements.HasElementsUnderPosition(slot2.position)))
	// 			{
	// 				AddSlotColorSlot(slot2, slot);
	// 			}
	// 			AddLightToSlot(slot2, slot);
	// 			AddBckLightToSlot(slot2);
	// 			board.SetSlot(slot2.position, slot2);
	// 			slot2.gravity.down = slot.gravitySettings.down;
	// 			slot2.gravity.up = slot.gravitySettings.up;
	// 			slot2.gravity.left = slot.gravitySettings.left;
	// 			slot2.gravity.right = slot.gravitySettings.right;
	// 			slot2.isExitForFallingChip = slot.isExitForFallingChip;
	// 			ItemColor itemColor = (ItemColor)RandomRange(0, 4);
	// 			PopulateBoard.BoardRepresentation.RepresentationSlot slot3 = board.populateBoard.board.GetSlot(slot2.position);
	// 			if (slot3 != null)
	// 			{
	// 				itemColor = slot3.itemColor;
	// 			}
	// 			if ((slot.chipType == ChipType.Chip || slot.chipType == ChipType.MonsterChip) && slot.itemColor != ItemColor.Unknown && slot.itemColor != ItemColor.RandomColor)
	// 			{
	// 				itemColor = slot.itemColor;
	// 			}
	// 			if (!slot.IsMonsterInSlot(levelDefinition))
	// 			{
	// 				if (slot.chipType == ChipType.EmptyConveyorSpace)
	// 				{
	// 					CreateEmptyConveyorSpace(slot2);
	// 				}
	// 				else if (slot.chipType == ChipType.MagicHat)
	// 				{
	// 					CreateMagicHat(slot2);
	// 				}
	// 				else if (slot.chipType == ChipType.MagicHatBomb || slot.chipType == ChipType.MagicHatSeekingMissle || slot.chipType == ChipType.MagicHatRocket)
	// 				{
	// 					CreateMagicHatBomb(slot2, slot);
	// 				}
	// 				else if (slot.chipType == ChipType.GrowingElement)
	// 				{
	// 					CreateGrowingElement(slot2, slot);
	// 				}
	// 				else if (slot.chipType == ChipType.FallingGingerbreadMan)
	// 				{
	// 					CreateFallingElement(slot2, slot.chipType);
	// 				}
	// 				else if (slot.chipType == ChipType.RockBlocker || slot.chipType == ChipType.SnowRockBlocker)
	// 				{
	// 					CreateRockBlocker(slot2, slot);
	// 				}
	// 				else if (slot.chipType == ChipType.MoreMovesChip)
	// 				{
	// 					hasMoreMoves = true;
	// 					CreateMoreMovesChip(slot2, slot);
	// 				}
	// 				else if (slot.chipType == ChipType.BunnyChip || slot.chipType == ChipType.CookiePickup)
	// 				{
	// 					CreateCharacterInSlot(slot2, slot);
	// 				}
	// 				else if (slot.chipType == ChipType.Bomb || slot.chipType == ChipType.DiscoBall || slot.chipType == ChipType.HorizontalRocket || slot.chipType == ChipType.VerticalRocket || slot.chipType == ChipType.SeekingMissle)
	// 				{
	// 					CreatePowerupInSlot(slot2, slot.chipType);
	// 				}
	// 				else if (slot.chipType == ChipType.MonsterChip)
	// 				{
	// 					CreateChipInSlot(slot2, ChipType.MonsterChip, itemColor);
	// 				}
	// 				else if (slot.chipType != ChipType.EmptyChipSlot)
	// 				{
	// 					CreateChipInSlot(slot2, itemColor);
	// 				}
	// 			}
	// 			if (slot.gravitySettings.canJumpWithGravity)
	// 			{
	// 				AddJumpRampToSlot(slot2);
	// 			}
	// 			if (slot.isExitForFallingChip)
	// 			{
	// 				AddFallingElementExitToSlot(slot2);
	// 			}
	// 			Chip slotComponent = slot2.GetSlotComponent<Chip>();
	// 			if (slotComponent != null)
	// 			{
	// 				slotComponent.chipTag = slot.chipTag;
	// 			}
	// 			if (slot.hasIce)
	// 			{
	// 				AddIceToSlot(slot2, slot);
	// 			}
	// 			if (slot.chipType != ChipType.EmptyConveyorSpace && slot.holeBlocker)
	// 			{
	// 				CreateEmptyConveyorSpace(slot2);
	// 			}
	// 		}
	// 	}
	// 	board.hasMoreMoves = hasMoreMoves;
	// 	for (int m = 0; m < level.generatorSetups.Count; m++)
	// 	{
	// 		GeneratorSetup generatorSetup = level.generatorSetups[m];
	// 		Slot slot4 = board.GetSlot(generatorSetup.position);
	// 		if (slot4 != null)
	// 		{
	// 			slot4.generatorSetup = generatorSetup;
	// 		}
	// 	}
	// 	aiPlayer.Init(this);
	// 	board.burriedElements.Init(this, levelDefinition);
	// 	board.carpet.Init(this, levelDefinition);
	// 	board.monsterElements.Init(this, levelDefinition);
	// 	List<LevelDefinition.Portal> allPortals = levelDefinition.GetAllPortals();
	// 	for (int n = 0; n < allPortals.Count; n++)
	// 	{
	// 		LevelDefinition.Portal portal = allPortals[n];
	// 		if (portal.isValid)
	// 		{
	// 			Slot slot5 = GetSlot(portal.entranceSlot.position);
	// 			Slot slot6 = GetSlot(portal.exitSlot.position);
	// 			if (slot5 != null && slot6 != null)
	// 			{
	// 				slot5.portalDestinationSlots.Add(slot6);
	// 				AddPipe(slot5, isExit: false, n);
	// 				AddPipe(slot6, isExit: true, n);
	// 			}
	// 		}
	// 	}
	// 	Slot[] slots = board.slots;
	// 	foreach (Slot slot7 in slots)
	// 	{
	// 		if (slot7 == null)
	// 		{
	// 			continue;
	// 		}
	// 		IntVector2 position2 = slot7.position;
	// 		if (level.GetSlot(position2).gravitySettings.canJumpWithGravity)
	// 		{
	// 			List<Slot> list = slot7.gravity.FindSlotsToWhichCanJumpTo(slot7, this);
	// 			for (int num4 = 0; num4 < list.Count; num4++)
	// 			{
	// 				Slot slot8 = list[num4];
	// 				slot7.jumpDestinationSlots.Add(slot8);
	// 				slot8.jumpOriginSlots.Add(slot7);
	// 			}
	// 		}
	// 	}
	// 	board.sortedSlotsUpdateList.Clear();
	// 	foreach (Slot slot9 in slots)
	// 	{
	// 		if (slot9 != null)
	// 		{
	// 			slot9.FillIncomingGravitySlots();
	// 			slot9.SetMaxDistanceToEnd();
	// 			board.sortedSlotsUpdateList.Add(slot9);
	// 		}
	// 	}
	// 	board.sortedSlotsUpdateList.Sort(_003C_003Ec._003C_003E9._003CCreateBoard_003Eb__121_0);
	// 	ListSlotsProvider listSlotsProvider = new ListSlotsProvider();
	// 	listSlotsProvider.Init(this);
	// 	for (int num6 = 0; num6 < levelDefinition.slots.Count; num6++)
	// 	{
	// 		LevelDefinition.SlotDefinition slotDefinition = levelDefinition.slots[num6];
	// 		if (slotDefinition.hasHoleInSlot && !slotDefinition.isPartOfConveyor)
	// 		{
	// 			TilesSlotsProvider.Slot slot10 = new TilesSlotsProvider.Slot(slotDefinition.position, isOccupied: true);
	// 			listSlotsProvider.AddSlot(slot10);
	// 		}
	// 	}
	// 	for (int num7 = 0; num7 < conveyorBelts.conveyorBeltList.Count; num7++)
	// 	{
	// 		LevelDefinition.ConveyorBelt conveyorBelt = conveyorBelts.conveyorBeltList[num7];
	// 		AddConveyorBelt(conveyorBelt, num7 + allPortals.Count);
	// 		List<LevelDefinition.ConveyorBeltLinearSegment> segmentList = conveyorBelt.segmentList;
	// 		for (int num8 = 0; num8 < segmentList.Count; num8++)
	// 		{
	// 			List<LevelDefinition.SlotDefinition> slotList = segmentList[num8].slotList;
	// 			for (int num9 = 0; num9 < slotList.Count; num9++)
	// 			{
	// 				LevelDefinition.SlotDefinition slotDefinition2 = slotList[num9];
	// 				TilesSlotsProvider.Slot slot11 = new TilesSlotsProvider.Slot(slotDefinition2.position, isOccupied: true);
	// 				listSlotsProvider.AddSlot(slot11);
	// 			}
	// 		}
	// 	}
	// 	bool flag = listSlotsProvider.allSlots.Count > 0;
	// 	GGUtil.SetActive(conveyorSlotsRenderer, flag);
	// 	GGUtil.SetActive(conveyorBorderRenderer, flag);
	// 	GGUtil.SetActive(conveyorHoleRenderer, flag);
	// 	if (flag)
	// 	{
	// 		if (conveyorSlotsRenderer != null)
	// 		{
	// 			conveyorSlotsRenderer.ShowSlotsOnLevel(listSlotsProvider);
	// 		}
	// 		if (conveyorBorderRenderer != null)
	// 		{
	// 			conveyorBorderRenderer.ShowBorderOnLevel(listSlotsProvider);
	// 		}
	// 		if (conveyorHoleRenderer != null)
	// 		{
	// 			conveyorHoleRenderer.ShowBorder(listSlotsProvider);
	// 		}
	// 	}
	// 	
	// 	goals.Init(levelDefinition, this);
	// 	extraFallingChips.Init(levelDefinition.extraFallingElements);
	// 	input.Init(this);
	// 	SetHasBounceOnAllChips(hasBounce: false);
	// 	if (chocolateBorderRenderer != null)
	// 	{
	// 		chocolateBorderRenderer.DisplayChocolate(this);
	// 	}
	// 	for (int num10 = 0; num10 < slots.Length; num10++)
	// 	{
	// 		slots[num10]?.GetSlotComponent<Chip>()?.UpdateFeatherShow();
	// 	}
	// 	if (hiddenElementBorderRenderer != null)
	// 	{
	// 		hiddenElementBorderRenderer.Render(this);
	// 	}
	// 	if (snowCoverRenderer != null)
	// 	{
	// 		snowCoverRenderer.Render(this);
	// 	}
	// 	if (bubbleSlotsBorderRenderer != null)
	// 	{
	// 		bubbleSlotsBorderRenderer.Render(this);
	// 	}
	// 	BubblesBoardComponent bubblesBoardComponent = new BubblesBoardComponent();
	// 	bubblesBoardComponent.Init(this);
	// 	board.bubblesBoardComponent = bubblesBoardComponent;
	// 	board.Add(bubblesBoardComponent);
	// }

	private void CreateSlotsModel(LevelDefinition levelDefinition)
	{
		var width = board.size.x;
		var height = board.size.y;
		
		LevelDefinition.ConveyorBelts conveyorBelts = levelDefinition.GetConveyorBelts();
		
		for (int k = 0; k < height; k++)
		{
			for (int l = 0; l < width; l++)
			{
				IntVector2 position = new IntVector2(l, k);
				LevelDefinition.SlotDefinition slotDefinition = levelDefinition.GetSlot(position);
				if (slotDefinition.slotType == SlotType.Empty || (slotDefinition.chipType == ChipType.EmptyConveyorSpace &&
				                                                  !conveyorBelts.IsPartOfConveyor(position)) || slotDefinition.holeBlocker)
				{
					continue;
				}

				var slot2 = board.GetSlot(position);
				
				if (slot2.TryGetSlotComponent<NetMatchOrDestroyNextToLock>(out var netMatch))
				{
					switch (netMatch.ChipType)
					{
						case ChipType.Box when netMatch.sortingOrder == 50:
							slotDefinition.netLevel = netMatch.blockerLevel;
							break;
						case ChipType.Box when netMatch.sortingOrder == 100:
							slotDefinition.boxLevel = netMatch.blockerLevel;
							break;
						case ChipType.Chain:
							slotDefinition.chainLevel = netMatch.blockerLevel;
							break;
					}
				}
		
				slotDefinition.hasBubbles = slot2.TryGetSlotComponent<BubblesPieceBlocker>(out _);
				if (slot2.TryGetSlotComponent<SnowCover>(out var snowCover))
				{
					slotDefinition.snowCoverLevel = 1;
				}
		
				if (slot2.TryGetSlotComponent<BasketBlocker>(out var basketBlocker))
				{
					slotDefinition.basketLevel = basketBlocker.blockerLevel;
				}
				if (slot2.TryGetSlotComponent<SlotColorSlate>(out var slotColorSlate))
				{
					slotDefinition.colorSlateLevel = slotColorSlate.blockerLevel;
				}
				if (slot2.TryGetSlotComponent<EmptyConveyorSpace>(out _))
				{
					slotDefinition.chipType = ChipType.EmptyConveyorSpace;
				}
				else if (slot2.TryGetSlotComponent<MagicHat>(out _))
				{
					slotDefinition.chipType = ChipType.MagicHat;
				}
				else if (slot2.TryGetSlotComponent<MagicHatBomb>(out var magicHatBomb))
				{
					slotDefinition.chipType = magicHatBomb.ChipType;
					slotDefinition.magicHatItemsCount = magicHatBomb.BombsCount;
				}
				else if (slot2.TryGetSlotComponent<GrowingElementChip>(out var growingElementChip))
				{
					slotDefinition.chipType = ChipType.GrowingElement;
					slotDefinition.itemColor = growingElementChip.ItemColor;
				}
				else if (slot2.TryGetSlotComponent<RockBlocker>(out var rockBlocker))
				{
					slotDefinition.chipType = rockBlocker.sortingOrder == 200 ? ChipType.SnowRockBlocker : ChipType.RockBlocker;
					slotDefinition.itemLevel = rockBlocker.blockerLevel - 1;
				}
				else if (slot2.TryGetSlotComponent<Chip>(out var chip))
				{
					slotDefinition.chipType = chip.chipType;
					slotDefinition.itemColor = chip.itemColor;
					slotDefinition.itemLevel = chip.itemLevel;
		
					slotDefinition.chipTag = chip.chipTag;
				}
				else
				{
					slotDefinition.chipType = ChipType.EmptyChipSlot;
				}
		
				if (slot2.TryGetSlotComponent<IceBlocker>(out var iceBlocker))
				{
					slotDefinition.iceLevel = iceBlocker.blockerLevel;
				}
			}
		}
		
		var burriedElements = board.burriedElements.elementPieces;
		List<LevelDefinition.BurriedElement> elements = _level.burriedElements.Clone().elements;
		elements.RemoveAll(x => burriedElements.All(b => b.position != x.position));
		_level.burriedElements.elements = elements;
		
		var monsterElements = board.monsterElements.pieces;
		List<LevelDefinition.MonsterElement> monsters = _level.monsterElements.Clone().elements;
		monsters.RemoveAll(x => monsterElements.All(b => b.position != x.position));
		_level.monsterElements.elements = monsters;

		var levelGoals = goals.goals;
		for (int i = 0; i < levelGoals.Count; i++)
		{
			var goalConfig = _level.goals.goals[i];
			var goal = levelGoals.FirstOrDefault(x => x.config.IsCompatible(goalConfig));

			if (goal == null)
				throw new NullReferenceException();

			goalConfig.collectCount = Mathf.Clamp(goal.RemainingCount, 0, goalConfig.collectCount);
		}
		
		_level.tutorialMatches.Clear();
	}

	public void CreateBoard(CreateBoardArguments arguments)
	{
		createBoardArguments = arguments;
		LevelDefinition levelDefinition = arguments.level;
		Vector3 offset = arguments.offset;
		if (initParams.isHudDissabled)
		{
			GGUtil.Hide(boardContainer);
		}
		level = levelDefinition;
		int width = levelDefinition.size.width;
		int height = levelDefinition.size.height;
		preventAutomatchesIfPossible_ = levelDefinition.isPreventingGeneratorChipMatching;
		RectTransform gamePlayAreaContainer = gameScreen.gamePlayAreaContainer;
		Vector3[] array = new Vector3[4];
		gamePlayAreaContainer.GetWorldCorners(array);
		float num = 0.25f;
		Vector2 vector = new Vector2((float)levelDefinition.size.width * slotPhysicalSize.x + 2f * num, (float)levelDefinition.size.height * slotPhysicalSize.x + 2f * num);
		Vector2 vector2 = new Vector2(array[2].x - array[0].x, array[2].y - array[0].y);
		float num2 = Mathf.Min(vector2.x / vector.x, vector2.y / vector.y);
		boardContainer.localScale = new Vector3(num2, num2, 1f);
		boardContainer.position = gamePlayAreaContainer.transform.position + offset;
		board = new Match3Board();
		IntVector2 size = board.size;
		size.x = width;
		size.y = height;
		board.generationSettings = levelDefinition.generationSettings;
		board.Init(size);
		borderRenderer.slotSize = slotPhysicalSize.x;
		slotsRenderer.slotSize = slotPhysicalSize.x;
		borderRenderer.ShowBorderOnLevel(levelDefinition);
		slotsRenderer.ShowSlotsOnLevel(levelDefinition);
		bool hasMoreMoves = false;
		LevelDefinition.ConveyorBelts conveyorBelts = levelDefinition.GetConveyorBelts();
		PopulateBoard.Params @params = new PopulateBoard.Params();
		@params.randomProvider = board.randomProvider;
		if (levelDefinition.generationSettings.isConfigured)
		{
			List<LevelDefinition.ChipGenerationSettings.ChipSetting> chipSettings = levelDefinition.generationSettings.chipSettings;
			for (int i = 0; i < chipSettings.Count; i++)
			{
				LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting = chipSettings[i];
				if (chipSetting.chipType == ChipType.Chip)
				{
					if (@params.availableColors.Contains(chipSetting.itemColor))
					{
						UnityEngine.Debug.Log("DUPLICATE AVAILABLE COLORS");
						continue;
					}
					board.availableColors.Add(chipSetting.itemColor);
					@params.availableColors.Add(chipSetting.itemColor);
				}
			}
		}
		else
		{
			for (int j = 0; j < levelDefinition.numChips; j++)
			{
				@params.availableColors.Add((ItemColor)j);
				board.availableColors.Add((ItemColor)j);
			}
		}
		particles.disableParticles = initParams.disableParticles;
		@params.maxPotentialMatches = Match3Settings.instance.maxPotentialMatchesAtStart;
		board.populateBoard.RandomPopulate(levelDefinition, @params);
		for (int k = 0; k < size.y; k++)
		{
			for (int l = 0; l < size.x; l++)
			{
				IntVector2 position = new IntVector2(l, k);
				LevelDefinition.SlotDefinition slot = levelDefinition.GetSlot(position);
				if (slot.slotType == SlotType.Empty || (slot.chipType == ChipType.EmptyConveyorSpace && !conveyorBelts.IsPartOfConveyor(position)))
				{
					continue;
				}
				Slot slot2 = new Slot();
				slot2.canGenerateChip = slot.generatorSettings.isGeneratorOn;
				slot2.generatorSettings = slot.generatorSettings;
				slot2.generatorSlotSettings = levelDefinition.GetGeneratorSlotSettings(slot.generatorSettings.slotGeneratorSetupIndex);
				slot2.position = new IntVector2(l, k);
				slot2.Init(this);
				if (slot2.generatorSlotSettings != null)
				{
					AddSpecialGenerator(slot2);
				}
				if (slot.hasNet)
				{
					AddNetToSlot(slot2, slot);
				}
				if (slot.hasBox)
				{
					AddBoxToSlot(slot2, slot, levelDefinition);
				}
				if (slot.hasBubbles)
				{
					AddBubblesToSlot(slot2);
				}
				if (slot.hasSnowCover)
				{
					AddSnowCoverToSlot(slot2);
				}
				if (slot.hasBasket)
				{
					AddBasketToSlot(slot2, slot);
				}
				if (slot.hasChain)
				{
					AddChainToSlot(slot2, slot);
				}
				if (slot.wallSettings.isWallActive)
				{
					AddWallToSlot(slot2, slot);
				}
				if (slot.colorSlateLevel > 0 && (slot.colorSlateLevel > 1 || levelDefinition.burriedElements.HasElementsUnderPosition(slot2.position)))
				{
					AddSlotColorSlot(slot2, slot);
				}
				AddLightToSlot(slot2, slot);
				AddBckLightToSlot(slot2);
				board.SetSlot(slot2.position, slot2);
				slot2.gravity.down = slot.gravitySettings.down;
				slot2.gravity.up = slot.gravitySettings.up;
				slot2.gravity.left = slot.gravitySettings.left;
				slot2.gravity.right = slot.gravitySettings.right;
				slot2.isExitForFallingChip = slot.isExitForFallingChip;
				ItemColor itemColor = (ItemColor)RandomRange(0, 4);
				PopulateBoard.BoardRepresentation.RepresentationSlot slot3 = board.populateBoard.board.GetSlot(slot2.position);
				if (slot3 != null)
				{
					itemColor = slot3.itemColor;
				}
				if ((slot.chipType == ChipType.Chip || slot.chipType == ChipType.MonsterChip) && slot.itemColor != ItemColor.Unknown && slot.itemColor != ItemColor.RandomColor)
				{
					itemColor = slot.itemColor;
				}
				if (!slot.IsMonsterInSlot(levelDefinition))
				{
					if (slot.chipType == ChipType.EmptyConveyorSpace)
					{
						CreateEmptyConveyorSpace(slot2);
					}
					else if (slot.chipType == ChipType.MagicHat)
					{
						CreateMagicHat(slot2);
					}
					else if (slot.chipType == ChipType.MagicHatBomb || slot.chipType == ChipType.MagicHatSeekingMissle || slot.chipType == ChipType.MagicHatRocket)
					{
						CreateMagicHatBomb(slot2, slot);
					}
					else if (slot.chipType == ChipType.GrowingElement)
					{
						CreateGrowingElement(slot2, slot);
					}
					else if (slot.chipType == ChipType.FallingGingerbreadMan)
					{
						CreateFallingElement(slot2, slot.chipType);
					}
					else if (slot.chipType == ChipType.RockBlocker || slot.chipType == ChipType.SnowRockBlocker)
					{
						CreateRockBlocker(slot2, slot);
					}
					else if (slot.chipType == ChipType.MoreMovesChip)
					{
						hasMoreMoves = true;
						CreateMoreMovesChip(slot2, slot);
					}
					else if (slot.chipType == ChipType.BunnyChip || slot.chipType == ChipType.CookiePickup)
					{
						CreateCharacterInSlot(slot2, slot);
					}
					else if (slot.chipType == ChipType.Bomb || slot.chipType == ChipType.DiscoBall || slot.chipType == ChipType.HorizontalRocket || slot.chipType == ChipType.VerticalRocket || slot.chipType == ChipType.SeekingMissle)
					{
						CreatePowerupInSlot(slot2, slot.chipType);
					}
					else if (slot.chipType == ChipType.MonsterChip)
					{
						CreateChipInSlot(slot2, ChipType.MonsterChip, itemColor);
					}
					else if (slot.chipType != ChipType.EmptyChipSlot)
					{
						CreateChipInSlot(slot2, itemColor);
					}
				}
				if (slot.gravitySettings.canJumpWithGravity)
				{
					AddJumpRampToSlot(slot2);
				}
				if (slot.isExitForFallingChip)
				{
					AddFallingElementExitToSlot(slot2);
				}
				Chip slotComponent = slot2.GetSlotComponent<Chip>();
				if (slotComponent != null)
				{
					slotComponent.chipTag = slot.chipTag;
				}
				if (slot.hasIce)
				{
					AddIceToSlot(slot2, slot);
				}
				if (slot.chipType != ChipType.EmptyConveyorSpace && slot.holeBlocker)
				{
					CreateEmptyConveyorSpace(slot2);
				}
			}
		}
		board.hasMoreMoves = hasMoreMoves;
		for (int m = 0; m < levelDefinition.generatorSetups.Count; m++)
		{
			GeneratorSetup generatorSetup = levelDefinition.generatorSetups[m];
			Slot slot4 = board.GetSlot(generatorSetup.position);
			if (slot4 != null)
			{
				slot4.generatorSetup = generatorSetup;
			}
		}
		aiPlayer.Init(this);
		board.burriedElements.Init(this, levelDefinition);
		board.carpet.Init(this, levelDefinition);
		board.monsterElements.Init(this, levelDefinition);
		List<LevelDefinition.Portal> allPortals = levelDefinition.GetAllPortals();
		for (int n = 0; n < allPortals.Count; n++)
		{
			LevelDefinition.Portal portal = allPortals[n];
			if (portal.isValid)
			{
				Slot slot5 = GetSlot(portal.entranceSlot.position);
				Slot slot6 = GetSlot(portal.exitSlot.position);
				if (slot5 != null && slot6 != null)
				{
					slot5.portalDestinationSlots.Add(slot6);
					AddPipe(slot5, isExit: false, n);
					AddPipe(slot6, isExit: true, n);
				}
			}
		}
		Slot[] slots = board.slots;
		foreach (Slot slot7 in slots)
		{
			if (slot7 == null)
			{
				continue;
			}
			IntVector2 position2 = slot7.position;
			if (levelDefinition.GetSlot(position2).gravitySettings.canJumpWithGravity)
			{
				List<Slot> list = slot7.gravity.FindSlotsToWhichCanJumpTo(slot7, this);
				for (int num4 = 0; num4 < list.Count; num4++)
				{
					Slot slot8 = list[num4];
					slot7.jumpDestinationSlots.Add(slot8);
					slot8.jumpOriginSlots.Add(slot7);
				}
			}
		}
		board.sortedSlotsUpdateList.Clear();
		foreach (Slot slot9 in slots)
		{
			if (slot9 != null)
			{
				slot9.FillIncomingGravitySlots();
				slot9.SetMaxDistanceToEnd();
				board.sortedSlotsUpdateList.Add(slot9);
			}
		}
		board.sortedSlotsUpdateList.Sort(_003C_003Ec._003C_003E9._003CCreateBoard_003Eb__121_0);
		ListSlotsProvider listSlotsProvider = new ListSlotsProvider();
		listSlotsProvider.Init(this);
		for (int num6 = 0; num6 < levelDefinition.slots.Count; num6++)
		{
			LevelDefinition.SlotDefinition slotDefinition = levelDefinition.slots[num6];
			if (slotDefinition.hasHoleInSlot && !slotDefinition.isPartOfConveyor)
			{
				TilesSlotsProvider.Slot slot10 = new TilesSlotsProvider.Slot(slotDefinition.position, isOccupied: true);
				listSlotsProvider.AddSlot(slot10);
			}
		}
		for (int num7 = 0; num7 < conveyorBelts.conveyorBeltList.Count; num7++)
		{
			LevelDefinition.ConveyorBelt conveyorBelt = conveyorBelts.conveyorBeltList[num7];
			AddConveyorBelt(conveyorBelt, num7 + allPortals.Count);
			List<LevelDefinition.ConveyorBeltLinearSegment> segmentList = conveyorBelt.segmentList;
			for (int num8 = 0; num8 < segmentList.Count; num8++)
			{
				List<LevelDefinition.SlotDefinition> slotList = segmentList[num8].slotList;
				for (int num9 = 0; num9 < slotList.Count; num9++)
				{
					LevelDefinition.SlotDefinition slotDefinition2 = slotList[num9];
					TilesSlotsProvider.Slot slot11 = new TilesSlotsProvider.Slot(slotDefinition2.position, isOccupied: true);
					listSlotsProvider.AddSlot(slot11);
				}
			}
		}
		bool flag = listSlotsProvider.allSlots.Count > 0;
		GGUtil.SetActive(conveyorSlotsRenderer, flag);
		GGUtil.SetActive(conveyorBorderRenderer, flag);
		GGUtil.SetActive(conveyorHoleRenderer, flag);
		if (flag)
		{
			if (conveyorSlotsRenderer != null)
			{
				conveyorSlotsRenderer.ShowSlotsOnLevel(listSlotsProvider);
			}
			if (conveyorBorderRenderer != null)
			{
				conveyorBorderRenderer.ShowBorderOnLevel(listSlotsProvider);
			}
			if (conveyorHoleRenderer != null)
			{
				conveyorHoleRenderer.ShowBorder(listSlotsProvider);
			}
		}
		
		goals.Init(levelDefinition, this);
		extraFallingChips.Init(levelDefinition.extraFallingElements);
		input.Init(this);
		SetHasBounceOnAllChips(hasBounce: false);
		if (chocolateBorderRenderer != null)
		{
			chocolateBorderRenderer.DisplayChocolate(this);
		}
		for (int num10 = 0; num10 < slots.Length; num10++)
		{
			slots[num10]?.GetSlotComponent<Chip>()?.UpdateFeatherShow();
		}
		if (hiddenElementBorderRenderer != null)
		{
			hiddenElementBorderRenderer.Render(this);
		}
		if (snowCoverRenderer != null)
		{
			snowCoverRenderer.Render(this);
		}
		if (bubbleSlotsBorderRenderer != null)
		{
			bubbleSlotsBorderRenderer.Render(this);
		}
		BubblesBoardComponent bubblesBoardComponent = new BubblesBoardComponent();
		bubblesBoardComponent.Init(this);
		board.bubblesBoardComponent = bubblesBoardComponent;
		board.Add(bubblesBoardComponent);
	}

	private void AddSlotColorSlot(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		SlotColorSlate slotColorSlate = new SlotColorSlate();
		if (slotDef.colorSlateLevel == 2)
		{
			slotColorSlate._sortingOrder = 9;
			var spriteName = slotDef.colorSlateSpriteName;
			var sprite = transform.Find(spriteName);
			if (sprite != null)
			{
				GGUtil.SetActive(sprite.gameObject, true);
			}
		}
		slotColorSlate.Init(slotDef.colorSlateLevel);
		if (isHudEnabled)
		{
			MultiLayerItemBehaviour multiLayerItemBehaviour = GetPool(PieceType.SlotColorSlate).Next<MultiLayerItemBehaviour>();
			multiLayerItemBehaviour.Init(ChipType.PickupGrass, slotDef.colorSlateLevel - 1);
			multiLayerItemBehaviour.SetPattern(slot);
			GGUtil.SetActive(multiLayerItemBehaviour, active: true);
			slotColorSlate.Add(multiLayerItemBehaviour);
			TransformBehaviour component = multiLayerItemBehaviour.GetComponent<TransformBehaviour>();
			if (slotDef.colorSlateLevel == 2)
			{
				var spriteName = slotDef.colorSlateSpriteName;
				var sprite = component.transform.GetChild(1).GetChild(0).GetChild(1).Find(spriteName);
				if (sprite != null)
				{
					GGUtil.SetActive(sprite.gameObject, true);
				}
			}
			component.localPosition = slot.localPositionOfCenter;
			slotColorSlate.Add(component);
		}
		slot.AddComponent(slotColorSlate);
		
	}

	public void AddBurriedElementSlot(Slot slot, LevelDefinition.BurriedElement burriedElement)
	{
		if (slot != null)
		{
			SlotBurriedElement slotBurriedElement = new SlotBurriedElement();
			slotBurriedElement.Init(burriedElement);
			if (isHudEnabled)
			{
				BurriedElementBehaviour burriedElementBehaviour = GetPool(PieceType.BurriedBunny).Next<BurriedElementBehaviour>();
				slotBurriedElement.Add(burriedElementBehaviour);
				GGUtil.SetActive(burriedElementBehaviour, active: true);
				burriedElementBehaviour.Init(burriedElement);
				TransformBehaviour component = burriedElementBehaviour.GetComponent<TransformBehaviour>();
				component.localPosition = slot.localPositionOfCenter;
				slotBurriedElement.Add(component);
			}
			slot.AddComponent(slotBurriedElement);
		}
	}

	private void AddConveyorBelt(LevelDefinition.ConveyorBelt conveyorBelt, int index)
	{
		ConveyorBeltBehaviour conveyorBeltBehaviour = GetPool(PieceType.ConveyorBelt).Next<ConveyorBeltBehaviour>();
		conveyorBeltBehaviour.Init(this, conveyorBelt, index);
		GGUtil.SetActive(conveyorBeltBehaviour, active: true);
		List<LevelDefinition.ConveyorBeltLinearSegment> segmentList = conveyorBelt.segmentList;
		for (int i = 0; i < segmentList.Count; i++)
		{
			List<LevelDefinition.SlotDefinition> slotList = segmentList[i].slotList;
			for (int j = 0; j < slotList.Count; j++)
			{
				LevelDefinition.SlotDefinition slotDefinition = slotList[j];
				Slot slot = GetSlot(slotDefinition.position);
				if (slot != null && slot.GetSlotComponent<EmptyConveyorSpace>() == null)
				{
					CreateConveyorBeltPlate(slot);
				}
			}
		}
		ConveyorBeltBoardComponent conveyorBeltBoardComponent = new ConveyorBeltBoardComponent();
		conveyorBeltBoardComponent.Init(this, conveyorBelt, conveyorBeltBehaviour);
		board.Add(conveyorBeltBoardComponent);
	}

	private void AddBckLightToSlot(Slot slot)
	{
		slot.backLight = new LightSlotComponent();
		if (isHudEnabled)
		{
			SlotLightBehaviour slotLightBehaviour = GetPool(PieceType.SlotBackLight).Next<SlotLightBehaviour>();
			slotLightBehaviour.transform.localPosition = slot.localPositionOfCenter;
			slotLightBehaviour.InitWithSlotComponent(slot.backLight);
		}
	}

	private void AddLightToSlot(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		if (isHudEnabled)
		{
			SlotLightBehaviour slotLightBehaviour = GetPool(PieceType.SlotLight).Next<SlotLightBehaviour>();
			slotLightBehaviour.transform.localPosition = slot.localPositionOfCenter;
			slotLightBehaviour.Init(slot, slot.isBackgroundPatternActive && slotDef.chipType != ChipType.EmptyConveyorSpace && !slotDef.isPartOfConveyor);
		}
		else
		{
			slot.AddComponent(new LightSlotComponent());
		}
	}

	private void AddJumpRampToSlot(Slot slot)
	{
		if (isHudEnabled)
		{
			List<IntVector2> forceDirections = slot.gravity.forceDirections;
			for (int i = 0; i < forceDirections.Count; i++)
			{
				IntVector2 direction = forceDirections[i];
				GetPool(PieceType.JumpRamp).Next<JumpRampBehaviour>().Init(slot.localPositionOfCenter, direction);
			}
		}
	}

	private void AddFallingElementExitToSlot(Slot slot)
	{
		if (isHudEnabled)
		{
			List<IntVector2> forceDirections = slot.gravity.forceDirections;
			for (int i = 0; i < forceDirections.Count; i++)
			{
				IntVector2 direction = forceDirections[i];
				GetPool(PieceType.FallingElementExit).Next<JumpRampBehaviour>().Init(slot.localPositionOfCenter, direction);
			}
		}
	}

	private void AddWallToSlot(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		wallDirections.Clear();
		LevelDefinition.WallSettings wallSettings = slotDef.wallSettings;
		if (wallSettings.left)
		{
			wallDirections.Add(IntVector2.left);
		}
		if (wallSettings.right)
		{
			wallDirections.Add(IntVector2.right);
		}
		if (wallSettings.up)
		{
			wallDirections.Add(IntVector2.up);
		}
		if (wallSettings.down)
		{
			wallDirections.Add(IntVector2.down);
		}
		for (int i = 0; i < wallDirections.Count; i++)
		{
			IntVector2 direction = wallDirections[i];
			WallBlocker wallBlocker = new WallBlocker();
			wallBlocker.Init(direction);
			if (!initParams.isHudDissabled)
			{
				WallBehaviour wallBehaviour = GetPool(PieceType.WallBlocker).Next<WallBehaviour>();
				wallBehaviour.transform.localPosition = slot.localPositionOfCenter;
				wallBehaviour.Init(direction);
				TransformBehaviour component = wallBehaviour.GetComponent<TransformBehaviour>();
				if (component != null)
				{
					wallBlocker.Add(component);
				}
			}
			slot.AddComponent(wallBlocker);
		}
	}

	private void AddChainToSlot(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		NetMatchOrDestroyNextToLock netMatchOrDestroyNextToLock = new NetMatchOrDestroyNextToLock();
		NetMatchOrDestroyNextToLock.InitProperties initProperties = default(NetMatchOrDestroyNextToLock.InitProperties);
		initProperties.sortingOrder = 50;
		initProperties.level = slotDef.chainLevel;
		initProperties.chipType = ChipType.Chain;
		initProperties.isMoveIntoSlotSuspended = true;
		initProperties.isDestroyByMatchingNeighborSuspended = true;
		initProperties.isAttachGrowingElementSuspended = true;
		initProperties.wobbleSettings = Match3Settings.instance.chipWobbleSettings;
		initProperties.useSound = true;
		initProperties.soundType = GGSoundSystem.SFXType.BreakChain;
		netMatchOrDestroyNextToLock.Init(initProperties);
		if (!initParams.isHudDissabled)
		{
			MultiLayerItemBehaviour multiLayerItemBehaviour = GetPool(PieceType.MultiLayerItem, ChipType.Chain).Next<MultiLayerItemBehaviour>();
			multiLayerItemBehaviour.transform.localPosition = slot.localPositionOfCenter;
			multiLayerItemBehaviour.Init(ChipType.Chain, initProperties.level - 1);
			TransformBehaviour component = multiLayerItemBehaviour.GetComponent<TransformBehaviour>();
			if (component != null)
			{
				netMatchOrDestroyNextToLock.Add(component);
			}
			netMatchOrDestroyNextToLock.Add(multiLayerItemBehaviour);
		}
		slot.AddComponent(netMatchOrDestroyNextToLock);
	}

	private void AddIceToSlot(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		if (slot.GetSlotComponent<Chip>() == null)
		{
			return;
		}
		IceBlocker iceBlocker = new IceBlocker();
		IceBlocker.InitProperties initProperties = default(IceBlocker.InitProperties);
		initProperties.level = slotDef.iceLevel;
		initProperties.sortingOrder = 20;
		initProperties.chip = slot.GetSlotComponent<Chip>();
		if (isHudEnabled)
		{
			TransformBehaviour transformBehaviour = GetPool(PieceType.MultiLayerItem, ChipType.IceOnChip).Next<TransformBehaviour>();
			if (transformBehaviour != null)
			{
				iceBlocker.Add(transformBehaviour);
				transformBehaviour.localPosition = slot.localPositionOfCenter;
				GGUtil.SetActive(transformBehaviour, active: true);
			}
		}
		iceBlocker.Init(initProperties);
		slot.AddComponent(iceBlocker);
	}

	public void AddIceToSlot(Slot slot, int iceLevel)
	{
		if (slot.GetSlotComponent<Chip>() == null)
		{
			return;
		}
		IceBlocker iceBlocker = new IceBlocker();
		IceBlocker.InitProperties initProperties = default(IceBlocker.InitProperties);
		initProperties.level = iceLevel;
		initProperties.sortingOrder = 20;
		initProperties.chip = slot.GetSlotComponent<Chip>();
		if (isHudEnabled)
		{
			TransformBehaviour transformBehaviour = GetPool(PieceType.MultiLayerItem, ChipType.IceOnChip).Next<TransformBehaviour>();
			if (transformBehaviour != null)
			{
				iceBlocker.Add(transformBehaviour);
				transformBehaviour.localPosition = slot.localPositionOfCenter;
				GGUtil.SetActive(transformBehaviour, active: true);
			}
		}
		iceBlocker.Init(initProperties);
		slot.AddComponent(iceBlocker);
	}

	private void AddBasketToSlot(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		if (slotDef.chipType == ChipType.EmptyChipSlot)
		{
			CreateMovingElementInSlot(slot);
		}
		BasketBlocker basketBlocker = new BasketBlocker();
		BasketBlocker.InitProperties initProperties = default(BasketBlocker.InitProperties);
		initProperties.level = slotDef.basketLevel;
		initProperties.sortingOrder = 30;
		initProperties.canFallthroughPickup = true;
		basketBlocker.Init(initProperties);
		bool hasEmptyChip = slotDef.chipType == ChipType.EmptyChipSlot;
		if (isHudEnabled)
		{
			ChipType chipType = ChipType.BasketBlocker;
			if (slotDef.chipType == ChipType.BunnyChip)
			{
				chipType = ChipType.BasetBlockerWithBunny;
			}
			MultiLayerItemBehaviour multiLayerItemBehaviour = GetPool(PieceType.MultiLayerItem, ChipType.BasketBlocker).Next<MultiLayerItemBehaviour>();
			multiLayerItemBehaviour.transform.localPosition = slot.localPositionOfCenter;
			multiLayerItemBehaviour.Init(chipType, initProperties.level - 1);
			basketBlocker.Add(multiLayerItemBehaviour);
			multiLayerItemBehaviour.SetHasEmptyChip(hasEmptyChip);
			TransformBehaviour component = multiLayerItemBehaviour.GetComponent<TransformBehaviour>();
			if (component != null)
			{
				basketBlocker.Add(component);
			}
		}
		slot.AddComponent(basketBlocker);
	}

	public void AddBubblesToSlot(Slot slot)
	{
		BubblesPieceBlocker bubblesPieceBlocker = new BubblesPieceBlocker();
		if (isHudEnabled)
		{
			TransformBehaviour transformBehaviour = GetPool(PieceType.BubblePiece).Next<TransformBehaviour>();
			transformBehaviour.transform.localPosition = slot.localPositionOfCenter;
			GGUtil.Show(transformBehaviour);
			bubblesPieceBlocker.Add(transformBehaviour);
		}
		slot.AddComponent(bubblesPieceBlocker);
	}

	public void AddSnowCoverToSlot(Slot slot)
	{
		SnowCover snowCover = new SnowCover();
		SnowCover.InitProperties initProperties = default(SnowCover.InitProperties);
		initProperties.sortingOrder = 110;
		initProperties.wobbleSettings = Match3Settings.instance.chipWobbleSettings;
		snowCover.Init(initProperties);
		slot.AddComponent(snowCover);
	}

	private void AddBoxToSlot(Slot slot, LevelDefinition.SlotDefinition slotDef, LevelDefinition level)
	{
		NetMatchOrDestroyNextToLock netMatchOrDestroyNextToLock = new NetMatchOrDestroyNextToLock();
		NetMatchOrDestroyNextToLock.InitProperties initProperties = default(NetMatchOrDestroyNextToLock.InitProperties);
		initProperties.level = slotDef.boxLevel;
		initProperties.sortingOrder = 100;
		initProperties.isMoveIntoSlotSuspended = true;
		initProperties.canFallthroughPickup = false;
		initProperties.isAttachGrowingElementSuspended = true;
		initProperties.isSlotMatchingSuspended = true;
		initProperties.isAvailableForDiscoBombSuspended = true;
		initProperties.isBlockingBurriedElement = true;
		initProperties.chipType = ChipType.Box;
		if (slotDef.chipType == ChipType.EmptyChipSlot || slotDef.chipType == ChipType.Unknown)
		{
			if (slotDef.colorSlateLevel == 1 && level.burriedElements.HasElementsUnderPosition(slotDef.position))
			{
				initProperties.SetDisplayChipType(ChipType.BoxWithBurriedElement);
			}
			else
			{
				initProperties.SetDisplayChipType(ChipType.BoxEmpty);
			}
		}
		initProperties.wobbleSettings = Match3Settings.instance.chipWobbleSettings;
		initProperties.useSound = true;
		initProperties.soundType = GGSoundSystem.SFXType.BreakBox;
		netMatchOrDestroyNextToLock.Init(initProperties);
		if (isHudEnabled)
		{
			MultiLayerItemBehaviour multiLayerItemBehaviour = GetPool(PieceType.MultiLayerItem, ChipType.Box).Next<MultiLayerItemBehaviour>();
			multiLayerItemBehaviour.transform.localPosition = slot.localPositionOfCenter;
			if (!multiLayerItemBehaviour.HasChipType(initProperties.displayChipType))
			{
				initProperties.SetDisplayChipType(ChipType.Box);
			}
			multiLayerItemBehaviour.Init(initProperties.displayChipType, initProperties.level - 1);
			netMatchOrDestroyNextToLock.Add(multiLayerItemBehaviour);
			TransformBehaviour component = multiLayerItemBehaviour.GetComponent<TransformBehaviour>();
			if (component != null)
			{
				netMatchOrDestroyNextToLock.Add(component);
			}
		}
		slot.AddComponent(netMatchOrDestroyNextToLock);
	}

	private void AddNetToSlot(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		SlotComponent slotComponent = null;
		NetMatchOrDestroyNextToLock netMatchOrDestroyNextToLock = new NetMatchOrDestroyNextToLock();
		netMatchOrDestroyNextToLock.Init(new NetMatchOrDestroyNextToLock.InitProperties
		{
			level = 1,
			sortingOrder = 50,
			isMoveIntoSlotSuspended = true,
			canFallthroughPickup = true,
			isSlotMatchingSuspended = false,
			isAvailableForDiscoBombSuspended = false,
			isAttachGrowingElementSuspended = true,
			chipType = ChipType.Box
		});
		slotComponent = netMatchOrDestroyNextToLock;
		if (isHudEnabled)
		{
			NetBehaviour netBehaviour = GetPool(PieceType.Net).Next<NetBehaviour>();
			netBehaviour.transform.localPosition = slot.localPositionOfCenter;
			slotComponent.Add(netBehaviour);
			GGUtil.Show(netBehaviour);
			TransformBehaviour component = netBehaviour.GetComponent<TransformBehaviour>();
			if (component != null)
			{
				slotComponent.Add(component);
			}
		}
		slot.AddComponent(slotComponent);
	}

	private void AddSpecialGenerator(Slot slot)
	{
		if (!isHudEnabled)
		{
			return;
		}
		ComponentPool pool = GetPool(PieceType.SpecialGenerator);
		if (pool != null)
		{
			SpecialGeneratorBehaviour specialGeneratorBehaviour = pool.Next<SpecialGeneratorBehaviour>();
			if (!(specialGeneratorBehaviour == null))
			{
				specialGeneratorBehaviour.transform.localPosition = slot.localPositionOfCenter;
				specialGeneratorBehaviour.Init(slot.generatorSlotSettings);
				GGUtil.Show(specialGeneratorBehaviour);
			}
		}
	}

	public GrowingElementChip CreateGrowingElement(Slot slot, LevelDefinition.SlotDefinition slotDefinition)
	{
		GrowingElementChip growingElementChip = new GrowingElementChip();
		growingElementChip.Init(slotDefinition.itemColor);
		if (isHudEnabled)
		{
			GrowingElementBehaviour growingElementBehaviour = GetPool(PieceType.Chip, ChipType.GrowingElement).Next<GrowingElementBehaviour>();
			growingElementBehaviour.Init();
			growingElementChip.Add(growingElementBehaviour);
			growingElementBehaviour.transform.localPosition = slot.localPositionOfCenter;
			TransformBehaviour component = growingElementBehaviour.GetComponent<TransformBehaviour>();
			growingElementChip.Add(component);
		}
		slot.AddComponent(growingElementChip);
		return growingElementChip;
	}

	public TransformBehaviour CreatePieceTypeBehaviour(ChipType chipType)
	{
		if (!isHudEnabled)
		{
			return null;
		}
		ComponentPool pool = GetPool(PieceType.PowerupChip, chipType);
		if (pool == null)
		{
			UnityEngine.Debug.LogError("NO POOL FOR " + chipType);
			return null;
		}
		TransformBehaviour transformBehaviour = pool.Next<TransformBehaviour>();
		if (transformBehaviour != null)
		{
			SolidPieceRenderer component = transformBehaviour.GetComponent<SolidPieceRenderer>();
			if (component != null)
			{
				component.Init(chipType);
			}
		}
		GGUtil.SetActive(transformBehaviour, active: true);
		return transformBehaviour;
	}

	public TransformBehaviour CreateGrowingElementPieceBehaviour()
	{
		if (!isHudEnabled)
		{
			return null;
		}
		TransformBehaviour transformBehaviour = GetPool(PieceType.Chip, ChipType.GrowingElementPiece).Next<TransformBehaviour>();
		GGUtil.SetActive(transformBehaviour, active: true);
		return transformBehaviour;
	}

	public MonsterElementBehaviour CreateMonsterElementBehaviour()
	{
		if (!isHudEnabled)
		{
			return null;
		}
		MonsterElementBehaviour monsterElementBehaviour = GetPool(PieceType.MonsterElement).Next<MonsterElementBehaviour>();
		GGUtil.SetActive(monsterElementBehaviour, active: true);
		return monsterElementBehaviour;
	}

	public FlyingSaucerBehaviour CreateFlyingSaucer()
	{
		Debug.LogError("CreateFlyingSaucer");
		if (!isHudEnabled)
		{
			return null;
		}
		FlyingSaucerBehaviour flyingSaucerBehaviour = GetPool(PieceType.FlyingSaucer).Next<FlyingSaucerBehaviour>();
		GGUtil.SetActive(flyingSaucerBehaviour, active: true);
		return flyingSaucerBehaviour;
	}

	public void CreateEmptyConveyorSpace(Slot slot)
	{
		if (slot != null)
		{
			EmptyConveyorSpace c = new EmptyConveyorSpace();
			slot.AddComponent(c);
		}
	}

	public void CreateConveyorBeltPlate(Slot slot)
	{
		ConveyorBeltPlate conveyorBeltPlate = new ConveyorBeltPlate();
		if (isHudEnabled)
		{
			TransformBehaviour transformBehaviour = GetPool(PieceType.ConveyorBeltPlate).Next<TransformBehaviour>();
			conveyorBeltPlate.Add(transformBehaviour);
			transformBehaviour.localPosition = slot.localPositionOfCenter;
			GGUtil.SetActive(transformBehaviour, active: true);
		}
		slot.AddComponent(conveyorBeltPlate);
	}

	public MagicHat CreateMagicHat(Slot slot)
	{
		MagicHat magicHat = new MagicHat();
		MagicHatBehaviour hatBehaviour = null;
		if (isHudEnabled)
		{
			TransformBehaviour transformBehaviour = GetPool(PieceType.Chip, ChipType.MagicHat).Next<TransformBehaviour>();
			hatBehaviour = transformBehaviour.GetComponent<MagicHatBehaviour>();
			magicHat.Add(transformBehaviour);
			transformBehaviour.localPosition = slot.localPositionOfCenter;
			GGUtil.SetActive(transformBehaviour, active: true);
		}
		magicHat.Init(hatBehaviour);
		slot.AddComponent(magicHat);
		return magicHat;
	}

	public MagicHatBomb CreateMagicHatBomb(Slot slot, LevelDefinition.SlotDefinition slotDefinition)
	{
		MagicHatBomb magicHatBomb = new MagicHatBomb();
		MagicHatBehaviour hatBehaviour = null;
		if (isHudEnabled)
		{
			TransformBehaviour transformBehaviour = GetPool(PieceType.Chip, ChipType.MagicHatBomb).Next<TransformBehaviour>();
			hatBehaviour = transformBehaviour.GetComponent<MagicHatBehaviour>();
			magicHatBomb.Add(transformBehaviour);
			transformBehaviour.localPosition = slot.localPositionOfCenter;
			GGUtil.SetActive(transformBehaviour, active: true);
		}
		magicHatBomb.Init(hatBehaviour, slotDefinition.magicHatItemsCount, slotDefinition.chipType);
		slot.AddComponent(magicHatBomb);
		return magicHatBomb;
	}

	public HammerAnimationBehaviour CreateHammerAnimationBehaviour()
	{
		ComponentPool pool = GetPool(PieceType.HammerAnimation);
		if (pool == null)
		{
			return null;
		}
		HammerAnimationBehaviour hammerAnimationBehaviour = pool.Next<HammerAnimationBehaviour>();
		GGUtil.SetActive(hammerAnimationBehaviour, active: true);
		return hammerAnimationBehaviour;
	}

	public CarpetSpreadBehaviour CreateCarpetSpread()
	{
		if (!isHudEnabled)
		{
			return null;
		}
		return GetPool(PieceType.CarpetSpread).Next<CarpetSpreadBehaviour>();
	}

	public BurriedElementBehaviour CreateBurriedElement()
	{
		if (!isHudEnabled)
		{
			return null;
		}
		return GetPool(PieceType.BurriedBunny).Next<BurriedElementBehaviour>();
	}

	public Chip CreateFallingElement(Slot slot, ChipType chipType)
	{
		Chip chip = new Chip();
		chip.Init(chipType, ItemColor.Unknown);
		if (isHudEnabled)
		{
			TransformBehaviour transformBehaviour = GetPool(PieceType.Chip, chipType).Next<TransformBehaviour>();
			chip.Add(transformBehaviour);
			chip.SetTransformToMove(transformBehaviour.transform);
			transformBehaviour.localPosition = slot.localPositionOfCenter;
			GGUtil.SetActive(transformBehaviour, active: true);
		}
		slot.AddComponent(chip);
		return chip;
	}

	public RockBlocker CreateRockBlocker(Slot slot, LevelDefinition.SlotDefinition slotDefinition)
	{
		RockBlocker rockBlocker = new RockBlocker();
		RockBlocker.InitArguments initArguments = default(RockBlocker.InitArguments);
		initArguments.sortingOrder = 10;
		if (slotDefinition.chipType == ChipType.SnowRockBlocker)
		{
			initArguments.sortingOrder = 200;
			initArguments.cancelsSnow = true;
		}
		initArguments.level = slotDefinition.itemLevel;
		rockBlocker.Init(initArguments);
		ChipType chipType = ChipType.RockBlocker;
		if (isHudEnabled)
		{
			MultiLayerItemBehaviour multiLayerItemBehaviour = GetPool(PieceType.MultiLayerItem, chipType).Next<MultiLayerItemBehaviour>();
			int itemLevel = slotDefinition.itemLevel;
			multiLayerItemBehaviour.Init(chipType, itemLevel);
			rockBlocker.Add(multiLayerItemBehaviour);
			TransformBehaviour component = multiLayerItemBehaviour.GetComponent<TransformBehaviour>();
			rockBlocker.Add(component);
			component.localPosition = slot.localPositionOfCenter;
			GGUtil.SetActive(multiLayerItemBehaviour, active: true);
		}
		slot.AddComponent(rockBlocker);
		return rockBlocker;
	}

	public Chip CreateCharacterInSlot(Slot slot, ChipType chipType, int itemLevel)
	{
		Chip chip = new Chip();
		chip.Init(chipType, ItemColor.Unknown);
		chip.itemLevel = itemLevel;
		if (isHudEnabled)
		{
			MultiLayerItemBehaviour multiLayerItemBehaviour = GetPool(PieceType.MultiLayerItem, chipType).Next<MultiLayerItemBehaviour>();
			multiLayerItemBehaviour.Init(chipType, itemLevel);
			chip.Add(multiLayerItemBehaviour);
			TransformBehaviour component = multiLayerItemBehaviour.GetComponent<TransformBehaviour>();
			chip.Add(component);
			chip.SetTransformToMove(component.transform);
			component.localPosition = slot.localPositionOfCenter;
			GGUtil.SetActive(multiLayerItemBehaviour, active: true);
		}
		slot.AddComponent(chip);
		return chip;
	}

	public LightingBolt CreateLightingBolt()
	{
		return GetPool(PieceType.LightingBolt).Next<LightingBolt>();
	}

	public TransformBehaviour CreateChipFeather(Slot slot, ItemColor itemColor)
	{
		TransformBehaviour transformBehaviour = GetPool(PieceType.ChipFeather).Next<TransformBehaviour>();
		transformBehaviour.localPosition = slot.localPositionOfCenter;
		ChipFeatherBehaviour component = transformBehaviour.GetComponent<ChipFeatherBehaviour>();
		if (component != null)
		{
			component.Init(itemColor);
		}
		GGUtil.SetActive(transformBehaviour, active: true);
		return transformBehaviour;
	}

	public LightingBolt CreateLightingBoltChip()
	{
		return GetPool(PieceType.LightingBoltChip).Next<LightingBolt>();
	}

	public LightingBolt CreateLightingBoltPowerup()
	{
		return GetPool(PieceType.LightingBoltPowerup).Next<LightingBolt>();
	}

	public Chip CreateMoreMovesChip(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		return CreateCharacterInSlot(slot, slotDef.chipType, slotDef.itemLevel);
	}

	public Chip CreateCharacterInSlot(Slot slot, LevelDefinition.SlotDefinition slotDef)
	{
		return CreateCharacterInSlot(slot, slotDef.chipType, slotDef.itemLevel);
	}

	public void AddPipe(Slot slot, bool isExit, int index)
	{
		PipeBehaviour pipeBehaviour = GetPool(PieceType.Pipe).Next<PipeBehaviour>();
		pipeBehaviour.Init(slot, isExit);
		pipeBehaviour.SetColor(Match3Settings.instance.pipeSettings.GetColor(index));
		if (isExit)
		{
			slot.exitPipe = pipeBehaviour;
		}
		else
		{
			slot.entrancePipe = pipeBehaviour;
		}
		GGUtil.SetActive(pipeBehaviour, active: true);
	}

	public PipeBehaviour CreatePipeDontAddToSlot()
	{
		PipeBehaviour pipeBehaviour = GetPool(PieceType.Pipe).Next<PipeBehaviour>();
		GGUtil.SetActive(pipeBehaviour, active: true);
		return pipeBehaviour;
	}

	public MovingElement CreateMovingElementInSlot(Slot slot)
	{
		MovingElement movingElement = new MovingElement();
		if (isHudEnabled)
		{
			TransformBehaviour transformBehaviour = GetPool(PieceType.MovingElement).Next<TransformBehaviour>();
			transformBehaviour.localPosition = slot.localPositionOfCenter;
			movingElement.Add(transformBehaviour);
		}
		slot.AddComponent(movingElement);
		return movingElement;
	}

	public TransformBehaviour CreateCoin()
	{
		if (!isHudEnabled)
		{
			return null;
		}
		return GetPool(PieceType.Coin)?.Next<TransformBehaviour>();
	}

	public TransformBehaviour CreatePointsDisplay()
	{
		if (!isHudEnabled)
		{
			return null;
		}
		return GetPool(PieceType.PointsDisplay)?.Next<TransformBehaviour>();
	}

	public Chip CreateChipInSlot(Slot slot, ChipType chipType, ItemColor itemColor)
	{
		Chip chip = new Chip();
		chip.Init(chipType, itemColor);
		chip.lastConnectedSlot = slot;
		if (isHudEnabled)
		{
			ChipBehaviour chipBehaviour = GetPool(PieceType.Chip, chipType).Next<ChipBehaviour>();
			chipBehaviour.Init(chip);
			chip.Add(chipBehaviour);
			TransformBehaviour component = chipBehaviour.GetComponent<TransformBehaviour>();
			chip.Add(component);
			GGUtil.SetActive(component, active: true);
			if (chipBehaviour != null)
			{
				chipBehaviour.ResetVisually();
			}
			component.localPosition = slot.localPositionOfCenter;
		}
		slot.AddComponent(chip);
		return chip;
	}

	public Chip CreateChipInSlot(Slot slot, ItemColor itemColor)
	{
		Chip chip = new Chip();
		chip.Init(ChipType.Chip, itemColor);
		chip.lastConnectedSlot = slot;
		if (isHudEnabled)
		{
			ChipBehaviour chipBehaviour = GetPool(PieceType.Chip).Next<ChipBehaviour>();
			chipBehaviour.Init(chip);
			chip.Add(chipBehaviour);
			TransformBehaviour component = chipBehaviour.GetComponent<TransformBehaviour>();
			chip.Add(component);
			ChipBehaviour componentBehaviour = chip.GetComponentBehaviour<ChipBehaviour>();
			if (componentBehaviour != null)
			{
				componentBehaviour.gameObject.SetActive(value: true);
				component.localPosition = slot.localPositionOfCenter;
				componentBehaviour.ResetVisually();
			}
		}
		slot.AddComponent(chip);
		return chip;
	}

	public Chip CreatePowerupInSlot(Slot slot, ChipType chipType)
	{
		Chip chip = new Chip();
		chip.Init(chipType, ItemColor.Unknown);
		if (isHudEnabled)
		{
			SolidPieceRenderer solidPieceRenderer = GetPool(PieceType.PowerupChip, chipType).Next<SolidPieceRenderer>();
			solidPieceRenderer.Init(chip);
			chip.Add(solidPieceRenderer);
			TransformBehaviour component = solidPieceRenderer.GetComponent<TransformBehaviour>();
			chip.Add(component);
			component.localPosition = slot.localPositionOfCenter;
			solidPieceRenderer.gameObject.SetActive(value: true);
			solidPieceRenderer.ResetVisually();
		}
		slot.AddComponent(chip);
		return chip;
	}

	public GameObject CreateParticles(Chip chip, PieceType pieceType, ChipType chipType, ItemColor itemColor)
	{
		ComponentPool pool = GetPool(pieceType, chipType, itemColor);
		if (pool == null)
		{
			return null;
		}
		GameObject gameObject = pool.Next(activate: true);
		if (gameObject == null)
		{
			return null;
		}
		TransformBehaviour componentBehaviour = chip.GetComponentBehaviour<TransformBehaviour>();
		if (componentBehaviour == null)
		{
			return gameObject;
		}
		gameObject.transform.localPosition = componentBehaviour.localPosition;
		return gameObject;
	}

	public RocketPieceBehaviour CreateRocketPiece()
	{
		if (!isHudEnabled)
		{
			return null;
		}
		RocketPieceBehaviour rocketPieceBehaviour = GetPool(PieceType.RocketPiece).Next<RocketPieceBehaviour>();
		rocketPieceBehaviour.Init();
		return rocketPieceBehaviour;
	}

	public void ApplySlotGravityForAllSlots()
	{
		List<Slot> sortedSlotsUpdateList = board.sortedSlotsUpdateList;
		for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
		{
			sortedSlotsUpdateList[i].ApplySlotGravity();
		}
	}

	private LevelDefinition _level;
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			//load
			// CreateBoard();
			if (_level == null && level != null)
			{
				_level = level.Clone();
				CreateSlotsModel(_level);
			}
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if(level != null)
			{
				CreateBoard(_level);
				Debug.LogError("load model");
			}
		}
				
		if (initParams == null)
		{
			return;
		}
		if (!gameStarted || board.isUpdateSuspended)
		{
			if (animationEnum != null)
			{
				animationEnum.MoveNext();
			}
			return;
		}
		float deltaTime = Time.deltaTime;
		deltaTime *= initParams.timeScale;
		int num = Mathf.Max(1, initParams.iterations);
		for (int i = 0; i < num; i++)
		{
			StepSymulation(deltaTime, i);
			if (board.isGameEnded)
			{
				break;
			}
		}
	}

	private bool _hasChanges;

	private void SaveProgress()
	{
		if(!_hasChanges)
			return;
		if (_level == null)
			_level = level.Clone();
		CreateSlotsModel(_level);
		_hasChanges = false;
	}

	public LevelDefinition GetLevelProgress() => _level;

	private void StepSymulation(float deltaTime, int iteration)
	{
		if (animationEnum != null)
		{
			animationEnum.MoveNext();
		}
		board.matchCounter.Update(deltaTime);
		board.isAnyConveyorMoveSuspended = true;
		if (board.isShufflingBoard)
		{
			UpdateBoardShuffle();
			return;
		}
		if (iteration == 0)
		{
			input.DoUpdate(deltaTime);
		}
		board.currentTime += deltaTime;
		board.currentDeltaTime = deltaTime;
		board.isDirtyInCurrentFrame = false;
		board.actionManager.OnUpdate(deltaTime);
		board.nonChangeStateActionMenager.OnUpdate(deltaTime);
		Slot[] slots = board.slots;
		if (slots == null)
		{
			return;
		}
		List<Slot> sortedSlotsUpdateList = board.sortedSlotsUpdateList;
		for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
		{
			sortedSlotsUpdateList[i].OnUpdate(deltaTime);
		}
		float num = 0f;
		foreach (Slot slot in slots)
		{
			if (slot != null)
			{
				num = Mathf.Max(slot.lastMoveTime, num);
			}
		}
		Matches matches = board.findMatches.FindAllMatches();
		PotentialMatches potentialMatches = board.potentialMatches;
		if (isAnySlotMoving)
		{
			_hasChanges = true;
		}
		else
		{
			SaveProgress();
		}
		if (!settings.generalSettings.waitTillBoardStopsForMatches || !isAnySlotMoving)
		{
			ProcessMatches(matches, null);
			SaveProgress();
		}
		board.currentMatchesCount = matches.islands.Count;
		potentialMatches.FindPotentialMatches(this);
		board.powerupCombines.Fill(this);
		board.powerupActivations.Fill(this);
		bool flag = board.powerupActivations.powerups.Count > 0 || board.powerupCombines.combines.Count > 0;
		board.isAnyConveyorMoveSuspended = false;
		bool flag2 = false;
		List<BoardComponent> boardComponents = board.boardComponents;
		for (int k = 0; k < boardComponents.Count; k++)
		{
			ConveyorBeltBoardComponent conveyorBeltBoardComponent = boardComponents[k] as ConveyorBeltBoardComponent;
			if (conveyorBeltBoardComponent != null)
			{
				board.moveCountWhenConveyorTookAction = Mathf.Max(board.moveCountWhenConveyorTookAction, conveyorBeltBoardComponent.lastMoveConveyorTookAction);
				if (conveyorBeltBoardComponent.IsMoveConveyorSuspended())
				{
					board.isAnyConveyorMoveSuspended = true;
					break;
				}
				if (conveyorBeltBoardComponent.needsToActivateConveyor)
				{
					flag2 = true;
				}
			}
		}
		if (input.isActive)
		{
			board.isAnyConveyorMoveSuspended = true;
		}
		bool isConveyorMoving = this.isConveyorMoving;
		for (int l = 0; l < boardComponents.Count; l++)
		{
			boardComponents[l].Update(deltaTime);
		}
		board.burriedElements.Update(deltaTime);
		float num2 = board.currentTime - num;
		float num3 = board.currentTime - board.lastTimeWhenUserMadeMove;
		bool isConveyorMoving2 = this.isConveyorMoving;
		if (isConveyorMoving && !isConveyorMoving2)
		{
			matches = board.findMatches.FindAllMatches();
			board.currentMatchesCount = matches.islands.Count;
			potentialMatches.FindPotentialMatches(this);
			board.powerupCombines.Fill(this);
			board.powerupActivations.Fill(this);
		}
		CheckEndGameConditions();
		if (board.actionManager.ActionCount == 0 && !isConveyorMoving2 && !board.isShufflingBoard && matches.MatchesCount == 0 && potentialMatches.MatchesCount == 0 && !board.isGameEnded && !input.isActive && !flag && !isAnySlotMoving && !board.isGameEnded)
		{
			ShuffleBoard();
			board.currentFrameIndex++;
			return;
		}
		bool flag3 = board.powerupCombines.combines.Count > 0;
		ShowPotentialMatchAction.Settings.ShowPotentialMatchTimes potentialTimesAction = Match3Settings.instance.showPotentialMatchesSettings.GetPotentialTimesAction(showPotentialMatchSetting, flag3, this);
		float idleTimeBeforeShowMatch = potentialTimesAction.idleTimeBeforeShowMatch;
		float boardIdleTimeBeforeShowMatch = potentialTimesAction.boardIdleTimeBeforeShowMatch;
		bool flag4 = false;
		for (int m = 0; m < board.slots.Length; m++)
		{
			Slot slot2 = board.slots[m];
			if (slot2 != null && slot2.LockCount > 0)
			{
				flag4 = true;
				break;
			}
		}
		bool flag5 = board.actionManager.ActionCount == 0 && !this.isConveyorMoving && !board.isShufflingBoard && !isAnySlotMoving && !flag2 && !flag4 && matches.MatchesCount == 0;
		if (flag5)
		{
			int lastSettledMove = board.lastSettledMove;
			board.lastSettledMove = Mathf.Max(board.lastSettledMove, board.userMovesCount);
			if (board.lastSettledMove > lastSettledMove)
			{
				OnMoveSettled();
			}
		}
		board.isBoardSettled = flag5;
		UpdateTutorialMatches(flag5 && !board.isGameEnded);
		bool flag6 = board.powerupActivations.powerups.Count > 0;
		bool flag7 = (potentialMatches.MatchesCount > 0) | flag3 | flag6;
		bool num4 = ((num2 > boardIdleTimeBeforeShowMatch && num3 > idleTimeBeforeShowMatch && matches.MatchesCount == 0) & flag7) && board.actionManager.ActionCount == 0 && !showMatchAction.isAlive && !isUserInteractionSuspended && !input.isActive && !showMatchAction.isAlive && !board.isGameEnded && !board.isPowerupSelectionActive;
		if (this.initParams.isAIPlayer && !isUserInteractionSuspended && board.actionManager.ActionCount == 0 && !this.isConveyorMoving && !board.isGameEnded && !board.isShufflingBoard && !isAnySlotMoving && !flag2 && !flag4 && matches.MatchesCount == 0)
		{
			aiPlayer.FindBestMove();
		}
		if (num4)
		{
			ShowPotentialMatchAction.InitParams initParams = default(ShowPotentialMatchAction.InitParams);
			initParams.game = this;
			initParams.userMoveWhenShow = board.userMovesCount;
			initParams.movesCountWhenConveyorTookAction = board.moveCountWhenConveyorTookAction;
			matchTypeToFind.Clear();
			goals.FillSlotData(this);
			PotentialMatch potentialMatch = default(PotentialMatch);
			if (suggestMoveType == SuggestMoveType.MatchesWithoutPowerupCreate)
			{
				potentialMatch = GetMatchingPotentialMatchAction();
			}
			else if (suggestMoveType == SuggestMoveType.Normal)
			{
				potentialMatch = GetBestPotentialMatchAction();
			}
			else if (suggestMoveType == SuggestMoveType.GoodOnFirstAndLast2)
			{
				List<Match3Goals.GoalBase> activeGoals = goals.GetActiveGoals();
				int num5 = 0;
				for (int n = 0; n < activeGoals.Count; n++)
				{
					Match3Goals.GoalBase goalBase = activeGoals[n];
					if (goalBase.config.chipType != ChipType.FallingGingerbreadMan)
					{
						num5 += goalBase.RemainingCount;
					}
				}
				if (board.userMovesCount == 1)
				{
					potentialMatch = GetBestPotentialMatchAction();
				}
				else if (num5 <= 2)
				{
					potentialMatch = GetBestPotentialMatchAction();
				}
				else
				{
					List<PotentialMatch> list = FillPotentialMatchesWithScore(addPowerupCombines: true, addPowerupActivations: true, onlyBestPowerups: true);
					if (list.Count > 0)
					{
						potentialMatch = list[UnityEngine.Random.Range(0, list.Count)];
					}
				}
			}
			else
			{
				potentialMatchesList.Clear();
				PowerupCombines.PowerupCombine powerupCombine = SelectPowerupCombine(goals, board.powerupCombines);
				if (powerupCombine != null)
				{
					potentialMatchesList.Add(new PotentialMatch(powerupCombine, powerupCombine.GetActionScore(this, goals)));
				}
				PowerupActivations.PowerupActivation powerupActivation = SelectPowerupActivation(goals, board.powerupActivations);
				if (powerupActivation != null)
				{
					potentialMatchesList.Add(new PotentialMatch(powerupActivation, powerupActivation.GetActionScore(this, goals)));
				}
				PotentialMatches.CompoundSlotsSet compoundSlotsSet = SelectPotentialMatch(goals, potentialMatches);
				if (compoundSlotsSet != null)
				{
					potentialMatchesList.Add(new PotentialMatch(compoundSlotsSet, compoundSlotsSet.GetActionScore(this, goals), default(ActionScore)));
				}
				bool flag8 = false;
				for (int num6 = 0; num6 < potentialMatchesList.Count; num6++)
				{
					PotentialMatch potentialMatch2 = potentialMatchesList[num6];
					if (!flag8 || IsBetter(potentialMatch, potentialMatch2))
					{
						flag8 = true;
						potentialMatch = potentialMatch2;
					}
				}
			}
			if (potentialMatch.isActive)
			{
				initParams.powerupCombine = potentialMatch.powerupCombine;
				initParams.powerupActivation = potentialMatch.powerupActivation;
				initParams.potentialMatch = potentialMatch.potentialMatch;
				showMatchAction.Init(initParams);
				board.nonChangeStateActionMenager.AddAction(showMatchAction);
			}
		}
		if (chocolateBorderRenderer != null)
		{
			chocolateBorderRenderer.DisplayChocolate(this);
		}
		if (snowCoverRenderer != null)
		{
			snowCoverRenderer.Render(this);
		}
		if (hiddenElementBorderRenderer != null)
		{
			hiddenElementBorderRenderer.Render(this);
		}
		if (bubbleSlotsBorderRenderer != null)
		{
			bubbleSlotsBorderRenderer.Render(this);
		}
		board.currentFrameIndex++;
	}

	private IEnumerator DoShuffleBoardAnimation()
	{
		return new _003CDoShuffleBoardAnimation_003Ed__192(0)
		{
			_003C_003E4__this = this
		};
	}

	private void ShuffleBoard()
	{
		board.isShufflingBoard = true;
		shuffleBoardAnimation = DoShuffleBoardAnimation();
	}

	private void UpdateBoardShuffle()
	{
		if (shuffleBoardAnimation != null)
		{
			shuffleBoardAnimation.MoveNext();
		}
	}

	public Slot LastSlotOnDirection(Slot origin, IntVector2 direction)
	{
		if (origin == null)
		{
			return null;
		}
		if (direction.x == 0 && direction.y == 0)
		{
			return origin;
		}
		Slot result = origin;
		IntVector2 intVector = origin.position;
		while (true)
		{
			intVector += direction;
			if (board.IsOutOfBoard(intVector))
			{
				break;
			}
			Slot slot = GetSlot(intVector);
			if (slot != null)
			{
				result = slot;
			}
		}
		return result;
	}

	public Slot FirstSlotOnDirection(IntVector2 position, IntVector2 direction)
	{
		if (direction.x == 0 && direction.y == 0)
		{
			return null;
		}
		Slot slot = GetSlot(position);
		if (slot != null)
		{
			return slot;
		}
		IntVector2 intVector = position;
		while (true)
		{
			intVector += direction;
			if (board.IsOutOfBoard(intVector))
			{
				break;
			}
			Slot slot2 = GetSlot(intVector);
			if (slot2 != null)
			{
				return slot2;
			}
		}
		return null;
	}

	public List<Slot> SlotsInDiscoBallDestroy(ItemColor itemColor, bool replaceWithBombs)
	{
		discoBallSlots.Clear();
		Slot[] slots = board.slots;
		foreach (Slot slot in slots)
		{
			if (slot != null && slot.CanParticipateInDiscoBombAffectedArea(itemColor, replaceWithBombs))
			{
				discoBallSlots.Add(slot);
			}
		}
		return discoBallSlots;
	}

	private bool IsBetter(PotentialMatch a, PotentialMatch b)
	{
		int currentScoreFactor = 3;
		int goalsFactor = 2;
		
		int num = a.ScoreWithPowerupScore(currentScoreFactor, goalsFactor);
		int num2 = b.ScoreWithPowerupScore(currentScoreFactor, goalsFactor);
		if (num != num2)
		{
			return num2 > num;
		}
		
		ActionScore actionScore = a.actionScore;
		ActionScore actionScore2 = b.actionScore;
		if (actionScore.goalsCount != actionScore2.goalsCount)
		{
			return actionScore2.goalsCount > actionScore.goalsCount;
		}
		if (actionScore2.obstaclesDestroyed != actionScore.obstaclesDestroyed)
		{
			return actionScore2.obstaclesDestroyed > actionScore.obstaclesDestroyed;
		}
		if (actionScore.powerupsCreated > 0 && actionScore2.powerupsCreated > 0)
		{
			return IsBetter(a.powerupCreatedScore, b.powerupCreatedScore);
		}
		return actionScore2.powerupsCreated > actionScore.powerupsCreated;
		
		// if (gameScreen.stageState.MovesRemaining > 1)
		// {
		// 	int num = a.ScoreWithPowerupScore(currentScoreFactor, goalsFactor);
		// 	int num2 = b.ScoreWithPowerupScore(currentScoreFactor, goalsFactor);
		// 	if (num != num2)
		// 	{
		// 		return num2 > num;
		// 	}
		// }
		// ActionScore actionScore = a.actionScore;
		// ActionScore actionScore2 = b.actionScore;
		// if (actionScore.goalsCount != actionScore2.goalsCount)
		// {
		// 	return actionScore2.goalsCount > actionScore.goalsCount;
		// }
		// if (actionScore2.obstaclesDestroyed != actionScore.obstaclesDestroyed)
		// {
		// 	return actionScore2.obstaclesDestroyed > actionScore.obstaclesDestroyed;
		// }
		// if (actionScore.powerupsCreated > 0 && actionScore2.powerupsCreated > 0)
		// {
		// 	return IsBetter(a.powerupCreatedScore, b.powerupCreatedScore);
		// }
		// return actionScore2.powerupsCreated > actionScore.powerupsCreated;
	}

	private bool IsBetter(ActionScore a, ActionScore b)
	{
		if (a.goalsCount != b.goalsCount)
		{
			return b.goalsCount > a.goalsCount;
		}
		if (b.obstaclesDestroyed != a.obstaclesDestroyed)
		{
			return b.obstaclesDestroyed > a.obstaclesDestroyed;
		}
		return b.powerupsCreated > a.powerupsCreated;
	}

	private PowerupActivations.PowerupActivation SelectPowerupActivation(Match3Goals goals, PowerupActivations powerupActivations)
	{
		List<PowerupActivations.PowerupActivation> powerups = powerupActivations.powerups;
		PowerupActivations.PowerupActivation powerupActivation = null;
		ActionScore a = default(ActionScore);
		for (int i = 0; i < powerups.Count; i++)
		{
			PowerupActivations.PowerupActivation powerupActivation2 = powerups[i];
			ActionScore actionScore = powerupActivation2.GetActionScore(this, goals);
			if (powerupActivation == null || IsBetter(a, actionScore))
			{
				a = actionScore;
				powerupActivation = powerupActivation2;
			}
		}
		return powerupActivation;
	}

	private PowerupCombines.PowerupCombine SelectPowerupCombine(Match3Goals goals, PowerupCombines powerupCombines)
	{
		List<PowerupCombines.PowerupCombine> combine = powerupCombines.combines;
		combineTypeToFind.Clear();
		combineTypeToFind.Add(PowerupCombines.CombineType.DoubleDiscoBall);
		combineTypeToFind.Add(PowerupCombines.CombineType.DiscoBallBomb);
		combineTypeToFind.Add(PowerupCombines.CombineType.DiscoBallRocket);
		combineTypeToFind.Add(PowerupCombines.CombineType.DiscoBallSeekingMissle);
		combineTypeToFind.Add(PowerupCombines.CombineType.RocketBomb);
		combineTypeToFind.Add(PowerupCombines.CombineType.DoubleBomb);
		combineTypeToFind.Add(PowerupCombines.CombineType.DoubleRocket);
		combineTypeToFind.Add(PowerupCombines.CombineType.DoubleSeekingMissle);
		combineTypeToFind.Add(PowerupCombines.CombineType.BombSeekingMissle);
		combineTypeToFind.Add(PowerupCombines.CombineType.RocketSeekingMissle);
		combineTypeToFind.Add(PowerupCombines.CombineType.DiscoBallColor);
		for (int i = 0; i < combineTypeToFind.Count; i++)
		{
			PowerupCombines.CombineType combineType = combineTypeToFind[i];
			List<PowerupCombines.PowerupCombine> list = powerupCombines.FilterCombines(combineType);
			bool flag = true;
			if (combineType == PowerupCombines.CombineType.DiscoBallColor)
			{
				flag = false;
				for (int j = 0; j < list.Count; j++)
				{
					PowerupCombines.PowerupCombine powerupCombine = list[j];
					Chip slotComponent = powerupCombine.exchangeSlot.GetSlotComponent<Chip>();
					if (!slotComponent.canFormColorMatches)
					{
						continue;
					}
					ItemColor itemColor = slotComponent.itemColor;
					List<Slot> list2 = SlotsInDiscoBallDestroy(itemColor, replaceWithBombs: false);
					for (int k = 0; k < list2.Count; k++)
					{
						Slot slot = list2[k];
						if (goals.IsDestroyingSlotCompleatingAGoal(slot, this, includeNeighbours: true))
						{
							flag = true;
							return powerupCombine;
						}
					}
				}
			}
			if (list.Count > 0 && flag)
			{
				return list[0];
			}
		}
		return null;
	}

	private PotentialMatches.CompoundSlotsSet SelectPotentialMatch(Match3Goals goals, PotentialMatches potentialMatches)
	{
		if (suggestMoveType == SuggestMoveType.BombsFirst)
		{
			matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.Bomb);
		}
		if (suggestMoveType == SuggestMoveType.HorizontalVerticalMissleFirst)
		{
			matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.Rocket);
		}
		if (suggestMoveType == SuggestMoveType.SeekingMissleFirst)
		{
			matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.SeekingMissle);
		}
		if (suggestMoveType == SuggestMoveType.PreferBombs)
		{
			matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.DiscoBall);
			matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.Bomb);
			matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.Rocket);
			matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.SeekingMissle);
		}
		matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.DiscoBall);
		matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.CompleatingGoals);
		matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.SeekingMissle);
		matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.Rocket);
		matchTypeToFind.Add(PotentialMatches.CompoundSlotsSet.MatchType.Bomb);
		for (int i = 0; i < matchTypeToFind.Count; i++)
		{
			PotentialMatches.CompoundSlotsSet.MatchType matchType = matchTypeToFind[i];
			List<PotentialMatches.CompoundSlotsSet> list = null;
			list = ((matchType != PotentialMatches.CompoundSlotsSet.MatchType.CompleatingGoals) ? potentialMatches.FilterForType(matchType) : potentialMatches.FilterForTypeCompleatingGoals(this));
			if (list == null || list.Count <= 0)
			{
				continue;
			}
			ActionScore a = default(ActionScore);
			PotentialMatches.CompoundSlotsSet compoundSlotsSet = null;
			for (int j = 0; j < list.Count; j++)
			{
				PotentialMatches.CompoundSlotsSet compoundSlotsSet2 = list[j];
				ActionScore actionScore = compoundSlotsSet2.GetActionScore(this, goals);
				if (compoundSlotsSet == null || IsBetter(a, actionScore))
				{
					a = actionScore;
					compoundSlotsSet = compoundSlotsSet2;
				}
			}
			return compoundSlotsSet;
		}
		List<PotentialMatches.CompoundSlotsSet> matchesList = potentialMatches.matchesList;
		if (matchesList.Count == 0)
		{
			return null;
		}
		return matchesList[AnimRandom.Range(0, matchesList.Count)];
	}

	private void CheckEndGameConditions()
	{
		if (board.ignoreEndConditions)
		{
			return;
		}
		bool flag = goals.isAllGoalsComplete;
		bool isOutOfMoves = false;
		bool flag2 = board.currentTime - board.lastTimeWhenUserMadeMove > 0.1f;
		bool flag3 = board.currentMatchesCount == 0 && board.actionManager.ActionCount == 0 && !isAnySlotMoving;
		flag2 = flag3;
		
		if (Application.isEditor && UnityEngine.Input.GetKey(KeyCode.W))
		{
			flag = true;
			flag2 = true;
		}
		if (flag | isOutOfMoves)
		{
			board.isInteractionSuspended = true;
			board.isEndConditionReached = true;
			if (board.isGameEnded)
			{
				return;
			}
			if (flag)
			{
				if (flag2)
				{
					board.isGameEnded = true;
					EndGame(isWin: true);
				}
			}
			else if (isOutOfMoves && flag3)
			{
				board.isGameEnded = true;
				EndGame(isWin: false);
			}
		}
		else if (board.isEndConditionReached)
		{
			board.isEndConditionReached = false;
			board.isInteractionSuspended = false;
		}
	}

	public void LateUpdate()
	{
		// if (Input.GetKeyDown(KeyCode.P))
		// {
		// 	EndGame(true);
		// }
	}

	private void EndGame(bool isWin)
	{
		if (initParams.isAIPlayer && !isWin)
		{
			board.isGameEnded = false;
			board.isInteractionSuspended = false;
			board.additionalMoves += 10;
			return;
		}
		board.isGameEnded = true;
		bool isOutOfMove = false;
		board.isInteractionSuspended = true;
		endTimestampTicks = DateTime.UtcNow.Ticks;
		if (isWin)
		{
			GameCompleteParams gameCompleteParams = new GameCompleteParams();
			gameCompleteParams.isWin = true;
			gameCompleteParams.stageState = gameScreen.stageState;
			gameCompleteParams.game = this;
			UnityEngine.Debug.Log("MOVES " + board.userMovesCount);
			gameScreen.Match3GameCallback_OnGameWon(gameCompleteParams);
		}
	}

	public void ResumeGame()
	{
		board.isGameEnded = false;
		board.isUpdateSuspended = false;
		board.isInteractionSuspended = false;
	}

	public void SuspendGameSounds()
	{
		board.isGameSoundsSuspended = true;
	}

	public void SuspendGame()
	{
		board.isUpdateSuspended = true;
	}

	public void QuitGame()
	{
		board.isGameEnded = true;
		board.isInteractionSuspended = true;
		endTimestampTicks = DateTime.UtcNow.Ticks;
	}

	public void ContinueGameAfterOffer(BuyMovesPricesConfig.OfferConfig offer)
	{
		board.isGameEnded = false;
		board.additionalMoves += offer.movesCount;
		board.isInteractionSuspended = false;
		board.isInteractionSuspended = false;
		moveOffersBought.Add(offer);
		float num = 0f;
		for (int i = 0; i < offer.powerups.Count; i++)
		{
			BuyMovesPricesConfig.OfferConfig.PowerupDefinition powerupDefinition = offer.powerups[i];
			for (int j = 0; j < powerupDefinition.count; j++)
			{
				PlacePowerupAction placePowerupAction = new PlacePowerupAction();
				PlacePowerupAction.Parameters parameters = new PlacePowerupAction.Parameters();
				parameters.game = this;
				parameters.powerup = powerupDefinition.powerupType;
				parameters.initialDelay = num;
				parameters.internalAnimation = true;
				placePowerupAction.Init(parameters);
				board.actionManager.AddAction(placePowerupAction);
				num += Match3Settings.instance.boosterPlaceDelay;
			}
		}
		gameScreen.powerupsPanel.ReinitPowerups();
	}

	public void OnSlotSetDirty(Slot slot)
	{
		board.isDirtyInCurrentFrame = true;
	}

	public void OnPickupGoal(GoalCollectParams goalCollect)
	{
		Match3Goals.GoalBase goal = goalCollect.goal;
		if (goal == null || goal.IsComplete())
		{
			return;
		}

		SlotDestroyParams destroyParams = goalCollect.destroyParams;
		GameStats gameStats = board.gameStats;
		if (destroyParams != null)
		{
			if (destroyParams.isHitByBomb)
			{
				gameStats.goalsFromPowerups++;
			}
			else if (destroyParams.isFromSwap)
			{
				gameStats.goalsFromUserMatches++;
			}
			else
			{
				gameStats.goalsFromInertion++;
			}
		}
		else
		{
			gameStats.goalsFromInertion++;
		}

		goals.OnPickupGoal(goal);
		GoalsPanelGoal goal2 = gameScreen.goalsPanel.GetGoal(goal);
		if (goal is Match3Goals.CollectItemsGoal collectItemsGoal)
		{
			if (!collectItemsGoal.IsComplete() || collectItemsGoal.collected != collectItemsGoal.config.collectCount)
				collectItemsGoal.collected++;
		}

		goal2.UpdateCollectedCount();
		Vector3 localPosition = boardContainer.InverseTransformPoint(goal2.transform.position);
		if (goal.IsComplete())
		{
			GameObject gameObject = particles.CreateParticles(localPosition, Match3Particles.PositionType.GoalComplete);
			gameObject.transform.parent = goal2.transform.parent;
			gameObject.transform.localPosition = goal2.transform.localPosition;
		}
		else
		{
			particles.CreateParticles(localPosition, Match3Particles.PositionType.OnUIGoalCollected);
		}

		Play(GGSoundSystem.SFXType.CollectGoal);
		if (goal.IsComplete())
		{
			Play(GGSoundSystem.SFXType.GoalsComplete);
			HighlightAllGoals();
			if (goal.config.chipType == ChipType.Chip)
			{
				List<Slot> sortedSlotsUpdateList = board.sortedSlotsUpdateList;
				for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
				{
					Slot slot = sortedSlotsUpdateList[i];
					if (slot != null)
					{
						Chip slotComponent = slot.GetSlotComponent<Chip>();
						if (slotComponent != null && slotComponent.chipType == goal.config.chipType &&
						    slotComponent.itemColor == goal.config.itemColor)
						{
							particles.CreateParticles(slotComponent, Match3Particles.PositionType.OnDestroyChip,
								slotComponent.chipType, slotComponent.itemColor);
						}
					}
				}
			}
		}

		if (goals.isAllGoalsComplete)
		{
			Play(GGSoundSystem.SFXType.GoalsComplete);
		}
		// Match3Goals.GoalBase goal = goalCollect.goal;
		// if (goal == null)
		// {
		// 	return;
		// }
		//
		// SlotDestroyParams destroyParams = goalCollect.destroyParams;
		// GameStats gameStats = board.gameStats;
		// if (destroyParams != null)
		// {
		// 	if (destroyParams.isHitByBomb)
		// 	{
		// 		gameStats.goalsFromPowerups++;
		// 	}
		// 	else if (destroyParams.isFromSwap)
		// 	{
		// 		gameStats.goalsFromUserMatches++;
		// 	}
		// 	else
		// 	{
		// 		gameStats.goalsFromInertion++;
		// 	}
		// }
		// else
		// {
		// 	gameStats.goalsFromInertion++;
		// }
		//
		// var chipTypeDef = Match3Goals.ChipTypeDef.Create(goal.config);
		// foreach (var tempGoal in goals.goals)
		// {
		// 	if(!tempGoal.IsPartOfGoal(chipTypeDef))
		// 		continue;
		// 	
		// 	GoalsPanelGoal goal2 = gameScreen.goalsPanel.GetGoal(tempGoal);
		// 	if (tempGoal is Match3Goals.CollectItemsGoal collectItemsGoal)
		// 	{
		// 		if(collectItemsGoal.IsComplete() && collectItemsGoal.collected == collectItemsGoal.config.collectCount)
		// 			continue;
		// 		
		// 		collectItemsGoal.collected++;
		// 	}
		// 	goal2.UpdateCollectedCount();
		// 	Vector3 localPosition = boardContainer.InverseTransformPoint(goal2.transform.position);
		// 	if (tempGoal.IsComplete())
		// 	{
		// 		GameObject gameObject = particles.CreateParticles(localPosition, Match3Particles.PositionType.GoalComplete);
		// 		gameObject.transform.parent = goal2.transform.parent;
		// 		gameObject.transform.localPosition = goal2.transform.localPosition;
		// 	}
		// 	else
		// 	{
		// 		particles.CreateParticles(localPosition, Match3Particles.PositionType.OnUIGoalCollected);
		// 	}
		// 	
		// 	Play(GGSoundSystem.SFXType.CollectGoal);
		// 	if (goal.IsComplete())
		// 	{
		// 		Play(GGSoundSystem.SFXType.GoalsComplete);
		// 		HighlightAllGoals();
		// 		if (goal.config.chipType == ChipType.Chip)
		// 		{
		// 			List<Slot> sortedSlotsUpdateList = board.sortedSlotsUpdateList;
		// 			for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
		// 			{
		// 				Slot slot = sortedSlotsUpdateList[i];
		// 				if (slot != null)
		// 				{
		// 					Chip slotComponent = slot.GetSlotComponent<Chip>();
		// 					if (slotComponent != null && slotComponent.chipType == goal.config.chipType && slotComponent.itemColor == goal.config.itemColor)
		// 					{
		// 						particles.CreateParticles(slotComponent, Match3Particles.PositionType.OnDestroyChip, slotComponent.chipType, slotComponent.itemColor);
		// 					}
		// 				}
		// 			}
		// 		}
		// 	}
		// }
		// if (goals.isAllGoalsComplete)
		// {
		// 	Play(GGSoundSystem.SFXType.GoalsComplete);
		// }
	}

	public void OnScoreAdded(int score)
	{
		// board.userScore += score;
		// gameScreen.goalsPanel.UpdateScore();
		EnergyGoals.instance.CurrentScore += score;
	}

	public void OnUserMadeMove()
	{
		_hasChanges = true;
		board.userMovesCount++;
		board.lastTimeWhenUserMadeMove = board.currentTime;
		board.lastFrameWhenUserMadeMove = board.currentFrameIndex;
		gameScreen.goalsPanel.UpdateMovesCount();
		board.ResetMatchesInBoard();
		// int movesRemaining = gameScreen.stageState.MovesRemaining;
		// if (movesRemaining == 5 || movesRemaining == 10 || movesRemaining == 15)
		// {
		// 	HighlightAllGoals();
		// }
		board.bubblesBoardComponent.OnUserMadeMove();
		
		// bool flag = movesRemaining == 5;
		// if (board.hasMoreMoves)
		// {
		// 	flag = (movesRemaining == 5 || movesRemaining == 3 || movesRemaining == 2 || movesRemaining == 1);
		// }
		// if (flag && movesRemaining > 0)
		// {
		// 	if (movesRemaining > 1)
		// 	{
		// 		gameScreen.movesContainer.Show($"{gameScreen.stageState.MovesRemaining} Moves");
		// 	}
		// 	else if (movesRemaining == 1)
		// 	{
		// 		gameScreen.movesContainer.Show("Last Move");
		// 	}
		// 	else
		// 	{
		// 		gameScreen.movesContainer.Show(gameScreen.stageState.MovesRemaining.ToString());
		// 	}
		// }
	}

	private void TryHighlightGoals()
	{
		List<Match3Goals.GoalBase> list = goals.goals;
		for (int i = 0; i < list.Count; i++)
		{
			Match3Goals.GoalBase goalBase = list[i];
			if (!goalBase.IsComplete())
			{
				int countAtStart = goalBase.CountAtStart;
				int remainingCount = goalBase.RemainingCount;
				bool flag = false;
				switch (goalBase.config.chipType)
				{
				case ChipType.FallingGingerbreadMan:
					flag = (remainingCount <= 1);
					break;
				case ChipType.BurriedElement:
					flag = (remainingCount < 5);
					break;
				case ChipType.Chip:
					flag = (remainingCount <= 6);
					break;
				}
				if (flag)
				{
					HighlightGoal(goalBase);
				}
			}
		}
	}

	private void HighlightGoal(Match3Goals.GoalBase goal)
	{
		if (goal == null)
		{
			return;
		}
		Match3Goals.ChipTypeDef chipTypeDef = default(Match3Goals.ChipTypeDef);
		chipTypeDef.chipType = goal.config.chipType;
		chipTypeDef.itemColor = goal.config.itemColor;
		if (chipTypeDef.chipType == ChipType.BurriedElement)
		{
			board.burriedElements.WobbleAll();
			return;
		}
		for (int i = 0; i < board.sortedSlotsUpdateList.Count; i++)
		{
			Slot slot = board.sortedSlotsUpdateList[i];
			if (slot != null)
			{
				Chip slotComponent = slot.GetSlotComponent<Chip>();
				if (slotComponent != null && slotComponent.IsCompatibleWithPickupGoal(chipTypeDef))
				{
					slotComponent.Wobble(Match3Settings.instance.chipWobbleSettings);
				}
			}
		}
	}

	private void HighlightAllGoals()
	{
		board.burriedElements.WobbleAll();
		for (int i = 0; i < board.sortedSlotsUpdateList.Count; i++)
		{
			Slot slot = board.sortedSlotsUpdateList[i];
			if (slot != null)
			{
				Chip slotComponent = slot.GetSlotComponent<Chip>();
				if (slotComponent != null && slotComponent.isPartOfActiveGoal)
				{
					slotComponent.Wobble(Match3Settings.instance.chipWobbleSettings);
				}
			}
		}
	}

	private PotentialMatch GetMatchingPotentialMatchAction()
	{
		goals.FillSlotData(this);
		potentialMatchesList.Clear();
		List<PotentialMatches.CompoundSlotsSet> matchesList = board.potentialMatches.matchesList;
		for (int i = 0; i < matchesList.Count; i++)
		{
			PotentialMatches.CompoundSlotsSet compoundSlotsSet = matchesList[i];
			ActionScore actionScore = compoundSlotsSet.GetActionScore(this, goals);
			ActionScore actionScore2 = default(ActionScore);
			if (actionScore.powerupsCreated > 0)
			{
				ChipType createdPowerup = compoundSlotsSet.createdPowerup;
				if (createdPowerup != ChipType.DiscoBall)
				{
					List<PowerupActivations.PowerupActivation> list = board.powerupActivations.CreatePotentialActivations(createdPowerup, GetSlot(compoundSlotsSet.positionOfSlotMissingForMatch));
					for (int j = 0; j < list.Count; j++)
					{
						ActionScore actionScore3 = list[j].GetActionScore(this, goals);
						actionScore2.goalsCount = Mathf.Max(actionScore2.goalsCount, actionScore3.goalsCount);
						actionScore2.obstaclesDestroyed = Mathf.Max(actionScore2.obstaclesDestroyed, actionScore3.obstaclesDestroyed);
					}
				}
				else
				{
					actionScore2.goalsCount = 20;
					actionScore2.obstaclesDestroyed = 20;
				}
			}
			potentialMatchesList.Add(new PotentialMatch(compoundSlotsSet, actionScore, actionScore2));
		}
		for (int k = 0; k < potentialMatchesList.Count; k++)
		{
			PotentialMatch potentialMatch = potentialMatchesList[k];
			if (potentialMatch.actionScore.powerupsCreated <= 0 && potentialMatch.actionScore.goalsCount > 0)
			{
				return potentialMatch;
			}
		}
		List<PowerupCombines.PowerupCombine> combines = board.powerupCombines.combines;
		for (int l = 0; l < combines.Count; l++)
		{
			PowerupCombines.PowerupCombine powerupCombine = combines[l];
			potentialMatchesList.Add(new PotentialMatch(powerupCombine, powerupCombine.GetActionScore(this, goals)));
		}
		List<PowerupActivations.PowerupActivation> powerups = board.powerupActivations.powerups;
		for (int m = 0; m < powerups.Count; m++)
		{
			PowerupActivations.PowerupActivation powerupActivation = powerups[m];
			potentialMatchesList.Add(new PotentialMatch(powerupActivation, powerupActivation.GetActionScore(this, goals)));
		}
		PotentialMatch potentialMatch2 = default(PotentialMatch);
		bool flag = false;
		for (int n = 0; n < potentialMatchesList.Count; n++)
		{
			PotentialMatch potentialMatch3 = potentialMatchesList[n];
			if (!flag || IsBetter(potentialMatch2.actionScore, potentialMatch3.actionScore))
			{
				flag = true;
				potentialMatch2 = potentialMatch3;
			}
		}
		return potentialMatch2;
	}

	private List<PotentialMatch> FillPotentialMatchesWithScore(bool addPowerupCombines, bool addPowerupActivations, bool onlyBestPowerups)
	{
		goals.FillSlotData(this);
		potentialMatchesList.Clear();
		if (addPowerupCombines)
		{
			List<PowerupCombines.PowerupCombine> combines = board.powerupCombines.combines;
			for (int i = 0; i < combines.Count; i++)
			{
				PowerupCombines.PowerupCombine powerupCombine = combines[i];
				potentialMatchesList.Add(new PotentialMatch(powerupCombine, powerupCombine.GetActionScore(this, goals)));
			}
		}
		if (addPowerupActivations)
		{
			List<PowerupActivations.PowerupActivation> powerups = board.powerupActivations.powerups;
			for (int j = 0; j < powerups.Count; j++)
			{
				PowerupActivations.PowerupActivation powerupActivation = powerups[j];
				potentialMatchesList.Add(new PotentialMatch(powerupActivation, powerupActivation.GetActionScore(this, goals)));
			}
		}
		if (onlyBestPowerups)
		{
			PotentialMatch bestPotentialMatch = GetBestPotentialMatch(potentialMatchesList);
			potentialMatchesList.Clear();
			if (bestPotentialMatch.isActive)
			{
				potentialMatchesList.Add(bestPotentialMatch);
			}
		}
		List<PotentialMatches.CompoundSlotsSet> matchesList = board.potentialMatches.matchesList;
		for (int k = 0; k < matchesList.Count; k++)
		{
			PotentialMatches.CompoundSlotsSet compoundSlotsSet = matchesList[k];
			ActionScore actionScore = compoundSlotsSet.GetActionScore(this, goals);
			ActionScore actionScore2 = default(ActionScore);
			if (actionScore.powerupsCreated > 0)
			{
				ChipType createdPowerup = compoundSlotsSet.createdPowerup;
				if (createdPowerup != ChipType.DiscoBall)
				{
					List<PowerupActivations.PowerupActivation> list = board.powerupActivations.CreatePotentialActivations(createdPowerup, GetSlot(compoundSlotsSet.positionOfSlotMissingForMatch));
					for (int l = 0; l < list.Count; l++)
					{
						ActionScore actionScore3 = list[l].GetActionScore(this, goals);
						if (actionScore3.goalsCount > actionScore2.goalsCount)
						{
							actionScore2 = actionScore3;
						}
						if (actionScore3.obstaclesDestroyed > actionScore2.obstaclesDestroyed)
						{
							actionScore2 = actionScore3;
						}
					}
				}
				else
				{
					actionScore2.goalsCount = 20;
					actionScore2.obstaclesDestroyed = 20;
				}
			}
			potentialMatchesList.Add(new PotentialMatch(compoundSlotsSet, actionScore, actionScore2));
		}
		return potentialMatchesList;
	}

	private PotentialMatch GetBestPotentialMatch(List<PotentialMatch> potentialMatchesList)
	{
		PotentialMatch potentialMatch = default(PotentialMatch);
		bool flag = false;
		for (int i = 0; i < potentialMatchesList.Count; i++)
		{
			PotentialMatch potentialMatch2 = potentialMatchesList[i];
			if (!flag || IsBetter(potentialMatch, potentialMatch2))
			{
				flag = true;
				potentialMatch = potentialMatch2;
			}
		}
		return potentialMatch;
	}

	private PotentialMatch GetBestPotentialMatchAction()
	{
		List<PotentialMatch> list = FillPotentialMatchesWithScore(addPowerupCombines: true, addPowerupActivations: true, onlyBestPowerups: false);
		return GetBestPotentialMatch(list);
	}

	private void OnMoveSettled()
	{
		return;
		movesSettledCount++;
		if (movesSettledCount % 2 == 0)
		{
			TryHighlightGoals();
		}
		
		gameScreen.powerupsPanel.ReinitPowerups();
		// if (gameScreen.stageState.MovesRemaining <= 3)
		// {
		// 	PotentialMatch bestPotentialMatchAction = GetBestPotentialMatchAction();
		// 	int totalSlotsGoalsRemainingCount = TotalSlotsGoalsRemainingCount;
		// 	if (!bestPotentialMatchAction.isActive)
		// 	{
		// 		gameScreen.powerupsPanel.ShowArrowsOnAvailablePowerups();
		// 	}
		// 	else if (bestPotentialMatchAction.isActive && bestPotentialMatchAction.actionScore.goalsCount < totalSlotsGoalsRemainingCount)
		// 	{
		// 		gameScreen.powerupsPanel.ShowArrowsOnAvailablePowerups();
		// 	}
		// 	else
		// 	{
		// 		gameScreen.powerupsPanel.ReinitPowerups();
		// 	}
		// }
		// else
		// {
		// 	gameScreen.powerupsPanel.ReinitPowerups();
		// }
	}

	public void OnCollectCoin()
	{
		board.currentCoins++;
		gameScreen.goalsPanel.SetCoinsCount(board.currentCoins);
	}

	public void OnCollectedMoreMoves(int count)
	{
		board.collectedAdditionalMoves += count;
		gameScreen.goalsPanel.UpdateMovesCount();
		if (count > 0)
		{
			Play(GGSoundSystem.SFXType.CollectMoreMoves);
		}
	}

	public void ProcessMatches(Matches m, SwapParams swapParams)
	{
		bool flag = false;
		for (int i = 0; i < m.islands.Count; i++)
		{
			Island island = m.islands[i];
			bool flag2 = ProcessIsland(island, swapParams);
			flag |= flag2;
		}
		if (flag)
		{
			ApplySlotGravityForAllSlots();
			board.matchCounter.timeLeft = 15f;
			board.matchCounter.eventCount++;
			_hasChanges = true;
		}
	}

	public void StartInAnimation()
	{
		animationEnum = DoStartInAnimation();
		animationEnum.MoveNext();
	}

	private IEnumerator DoStartInAnimation()
	{
		return new _003CDoStartInAnimation_003Ed__237(0)
		{
			_003C_003E4__this = this
		};
	}

	public void StartWinAnimation(WinScreen.InitArguments winScreenArguments, Action onComplete)
	{
		board.isUpdateSuspended = false;
		board.isInteractionSuspended = true;
		animationEnum = DoWinAnimation(winScreenArguments, onComplete);
		animationEnum.MoveNext();
	}

	private int CoinsPerChipType(ChipType chipType, int coinsPerPowerup)
	{
		int num = coinsPerPowerup;
		switch (chipType)
		{
		case ChipType.Bomb:
			num *= 2;
			break;
		case ChipType.DiscoBall:
			num *= 5;
			break;
		}
		return num;
	}

	private IEnumerator DoWinAnimation(WinScreen.InitArguments winScreenArguments, Action onComplete)
	{
		return new _003CDoWinAnimation_003Ed__240(0)
		{
			_003C_003E4__this = this,
			winScreenArguments = winScreenArguments,
			onComplete = onComplete
		};
	}

	public void StartWinScreenBoardAnimation()
	{
		animationEnum = DoWinScreenBoardOutAnimation();
	}

	private IEnumerator DoWinScreenBoardOutAnimation()
	{
		return new _003CDoWinScreenBoardOutAnimation_003Ed__242(0)
		{
			_003C_003E4__this = this
		};
	}

	private bool ProcessIsland(Island island, SwapParams swapParams)
	{
		List<Slot> allSlots = island.allSlots;
		SlotDestroyParams slotDestroyParams = new SlotDestroyParams();
		slotDestroyParams.matchIsland = island;
		slotDestroyParams.isFromSwap = island.isFromSwap;
		slotDestroyParams.isCreatingPowerupFromThisMatch = island.isCreatingPowerup;
		for (int i = 0; i < allSlots.Count; i++)
		{
			if (allSlots[i].canCarpetSpreadFromHere)
			{
				slotDestroyParams.isHavingCarpet = true;
				break;
			}
		}
		if (island.isCreatingPowerup)
		{
			for (int j = 0; j < allSlots.Count; j++)
			{
				if (allSlots[j].isCreatePowerupWithThisSlotSuspended)
				{
					slotDestroyParams.isCreatingPowerupFromThisMatch = false;
					break;
				}
			}
		}
		Slot slot = null;
		bool flag = swapParams != null;
		if (slotDestroyParams.isCreatingPowerupFromThisMatch)
		{
			Slot slot2 = null;
			for (int k = 0; k < allSlots.Count; k++)
			{
				Slot slot3 = allSlots[k];
				if (slot2 == null || slot2.lastMoveFrameIndex < slot3.lastMoveFrameIndex)
				{
					slot2 = slot3;
				}
			}
			slot = ((swapParams == null) ? slot2 : ((!island.ContainsPosition(swapParams.swipeToPosition)) ? GetSlot(swapParams.startPosition) : GetSlot(swapParams.swipeToPosition)));
			if (slot != null && slot.isPowerupReplacementSuspended)
			{
				slot = null;
				slotDestroyParams.isCreatingPowerupFromThisMatch = false;
			}
		}
		if (slot == null)
		{
			slotDestroyParams.isCreatingPowerupFromThisMatch = false;
		}
		if (island.isFromSwap)
		{
			slotDestroyParams.swapParams = swapParams;
		}
		else
		{
			Matches matches = board.findMatches.matches;
			bool flag2 = false;
			for (int l = 0; l < allSlots.Count; l++)
			{
				Slot slot4 = allSlots[l];
				slot4.GetSlotComponent<Chip>();
				List<Slot> neigbourSlots = slot4.neigbourSlots;
				for (int m = 0; m < neigbourSlots.Count; m++)
				{
					Slot slot5 = neigbourSlots[m];
					if (matches.GetIsland(slot5.position) == null)
					{
						Chip slotComponent = slot5.GetSlotComponent<Chip>();
						if (slotComponent != null && slotComponent.isSlotMatchingSuspended)
						{
							flag2 = true;
							break;
						}
					}
				}
			}
			if (flag2)
			{
				return false;
			}
		}
		DestroyMatchingIslandAction.InitArguments initArguments = default(DestroyMatchingIslandAction.InitArguments);
		initArguments.boltCollection = new BoltCollection();
		if (swapParams != null)
		{
			InputAffectorExport affectorExport = swapParams.affectorExport;
			Slot slot6 = null;
			if (island.ContainsPosition(swapParams.startPosition))
			{
				slot6 = GetSlot(swapParams.startPosition);
			}
			else if (island.ContainsPosition(swapParams.swipeToPosition))
			{
				slot6 = GetSlot(swapParams.swipeToPosition);
			}
			affectorExport.GetInputAffectorForSlot(slot6)?.PutAllBoltsIn(initArguments.boltCollection);
		}
		bool flag3 = slot != null;
		initArguments.slotDestroyParams = slotDestroyParams;
		initArguments.slotWherePowerupIsCreated = slot;
		initArguments.powerupToCreate = island.powerupToCreate;
		initArguments.allSlots = new List<Slot>();
		initArguments.allSlots.AddRange(allSlots);
		initArguments.game = this;
		initArguments.matchComboIndex = board.matchCounter.eventCount;
		bool flag4 = Slot.HasNeighboursAffectedByMatchingSlots(allSlots, this);
		if (allSlots.Count > 0 && !flag && Match3Settings.instance.destroyIslandBlinkSettings.useBlink && !flag3 && flag4)
		{
			DestroyMatchingIslandBlinkAction destroyMatchingIslandBlinkAction = new DestroyMatchingIslandBlinkAction();
			destroyMatchingIslandBlinkAction.Init(initArguments);
			board.actionManager.AddAction(destroyMatchingIslandBlinkAction);
			return true;
		}
		if (allSlots.Count > 0 && !flag && ((flag3 | flag4) || Match3Settings.instance.swipeAffectorSettings.autoMatchesProduceLighting))
		{
			DestroyMatchingIslandAction destroyMatchingIslandAction = new DestroyMatchingIslandAction();
			destroyMatchingIslandAction.Init(initArguments);
			board.actionManager.AddAction(destroyMatchingIslandAction);
			return true;
		}
		if (allSlots.Count > 0 && !flag && Match3Settings.instance.destroyIslandBlinkSettings.useBlink)
		{
			DestroyMatchingIslandBlinkAction destroyMatchingIslandBlinkAction2 = new DestroyMatchingIslandBlinkAction();
			destroyMatchingIslandBlinkAction2.Init(initArguments);
			board.actionManager.AddAction(destroyMatchingIslandBlinkAction2);
			return true;
		}
		FinishDestroySlots(initArguments);
		return allSlots.Count > 0;
	}

	public void FinishDestroySlots(DestroyMatchingIslandAction.InitArguments arguments)
	{
		List<Slot> allSlots = arguments.allSlots;
		SlotDestroyParams slotDestroyParams = arguments.slotDestroyParams;
		SwapParams swapParams = slotDestroyParams.swapParams;
		Slot slotWherePowerupIsCreated = arguments.slotWherePowerupIsCreated;
		bool flag = false;
		for (int i = 0; i < allSlots.Count; i++)
		{
			Chip slotComponent = allSlots[i].GetSlotComponent<Chip>();
			if (slotComponent != null && slotComponent.isPartOfActiveGoal)
			{
				flag = true;
				break;
			}
		}
		if (!slotDestroyParams.isCreatingPowerupFromThisMatch && !flag)
		{
			GGSoundSystem.PlayParameters sound = default(GGSoundSystem.PlayParameters);
			sound.soundType = GGSoundSystem.SFXType.PlainMatch;
			sound.variationIndex = arguments.matchComboIndex;
			Play(sound);
		}
		CollectPointsAction.OnIslandDestroy(arguments);
		for (int j = 0; j < allSlots.Count; j++)
		{
			allSlots[j].OnDestroySlot(slotDestroyParams);
		}
		board.AddMatch();
		destroyNeighbourSlots.Clear();
		allNeighbourSlots.Clear();
		if (!slotDestroyParams.isNeigbourDestroySuspended)
		{
			for (int k = 0; k < allSlots.Count; k++)
			{
				Slot slot = allSlots[k];
				if (slotDestroyParams.IsNeigborDestraySuspended(slot))
				{
					continue;
				}
				List<Slot> neigbourSlots = slot.neigbourSlots;
				for (int l = 0; l < neigbourSlots.Count; l++)
				{
					Slot slot2 = neigbourSlots[l];
					if (!allSlots.Contains(slot2) && !allNeighbourSlots.Contains(slot2))
					{
						allNeighbourSlots.Add(slot2);
						destroyNeighbourSlots.Add(new SlotDestroyNeighbour(slot, slot2));
					}
				}
			}
		}
		for (int m = 0; m < destroyNeighbourSlots.Count; m++)
		{
			SlotDestroyNeighbour slotDestroyNeighbour = destroyNeighbourSlots[m];
			slotDestroyNeighbour.neighbourSlot.OnDestroyNeighbourSlot(slotDestroyNeighbour.slotBeingDestroyed, slotDestroyParams);
		}
		if (slotDestroyParams.isCreatingPowerupFromThisMatch)
		{
			CreatePowerupAction createPowerupAction = new CreatePowerupAction();
			if (slotDestroyParams.chipsAvailableForPowerupCreateAnimation != null && slotDestroyParams.chipsAvailableForPowerupCreateAnimation.Count > 0)
			{
				for (int n = 0; n < slotDestroyParams.chipsAvailableForPowerupCreateAnimation.Count; n++)
				{
					Chip chip = slotDestroyParams.chipsAvailableForPowerupCreateAnimation[n];
					LightingBolt boltEndingOnSlot = arguments.boltCollection.GetBoltEndingOnSlot(chip.lastConnectedSlot);
					arguments.boltCollection.AddUsedBolt(boltEndingOnSlot);
					createPowerupAction.AddChip(chip, boltEndingOnSlot);
				}
			}
			CreatePowerupAction.CreateParams createParams = default(CreatePowerupAction.CreateParams);
			createParams.game = this;
			createParams.positionWherePowerupWillBeCreated = slotWherePowerupIsCreated.position;
			createParams.powerupToCreate = arguments.powerupToCreate;
			createPowerupAction.Init(createParams);
			board.actionManager.AddAction(createPowerupAction);
		}
		bool flag2 = swapParams != null;
		GameStats gameStats = board.gameStats;
		if (flag2)
		{
			gameStats.matchesFromUser++;
		}
		else
		{
			gameStats.matchesFromInertion++;
		}
		if (slotDestroyParams.isCreatingPowerupFromThisMatch)
		{
			if (flag2)
			{
				gameStats.powerupsCreatedFromUser++;
			}
			else
			{
				gameStats.powerupsCreatedFromInertion++;
			}
		}
		arguments.boltCollection.Clear();
	}

	public List<Slot> SlotsThatCanParticipateInDiscoBallAffectedArea(ItemColor itemColor, bool replaceWithBombs)
	{
		List<Slot> list = new List<Slot>();
		for (int i = 0; i < board.sortedSlotsUpdateList.Count; i++)
		{
			Slot slot = board.sortedSlotsUpdateList[i];
			if (slot != null && slot.CanParticipateInDiscoBombAffectedArea(itemColor, replaceWithBombs))
			{
				list.Add(slot);
			}
		}
		return list;
	}

	public ItemColor BestItemColorForDiscoBomb(bool replaceWithBombs)
	{
		maxColorHelper.Clear();
		Slot[] slots = board.slots;
		foreach (Slot slot in slots)
		{
			maxColorHelper.AddSlot(slot, replaceWithBombs);
		}
		return maxColorHelper.MaxColor.color;
	}

	public void ClearSlotLocks()
	{
		Slot[] slots = board.slots;
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i]?.ClearLocks();
		}
	}

	public void AddLockToAllSlots(Lock slotLock)
	{
		if (slotLock == null)
		{
			return;
		}
		for (int i = 0; i < board.slots.Length; i++)
		{
			Slot slot = board.slots[i];
			if (slot != null)
			{
				slotLock.LockSlot(slot);
			}
		}
	}

	public int RandomRange(int min, int max)
	{
		return board.RandomRange(min, max);
	}

	public float RandomRange(float min, float max)
	{
		return board.RandomRange(min, max);
	}

	public void RemoveLockFromAllSlots(Lock slotLock)
	{
		for (int i = 0; i < board.slots.Length; i++)
		{
			board.slots[i]?.RemoveLock(slotLock);
		}
	}

	public void Leave1Move()
	{
		//board.userMovesCount += gameScreen.stageState.MovesRemaining - 1;
		gameScreen.goalsPanel.UpdateMovesCount();
	}

	private void _003CPutBoosters_003Eb__108_0()
	{
		HighlightAllGoals();
	}
}
