using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class Match3Board
{
	public class MatchesInBoard
	{
		public int currentMove;

		public int matchesCount;

		public void AddMatch(int currentMoveIndex)
		{
			if (currentMove != currentMoveIndex)
			{
				currentMove = currentMoveIndex;
				matchesCount = 0;
			}
			matchesCount++;
		}
	}

	public class TimedCounter
	{
		public float timeLeft;

		public int eventCount;

		public void OnUserMadeMove()
		{
			eventCount = 0;
			timeLeft = 15f;
		}

		public void Update(float deltaTime)
		{
			timeLeft -= deltaTime;
			if (timeLeft <= 0f)
			{
				eventCount = 0;
				timeLeft = 0f;
			}
		}
	}

	public struct ChipCreateParams
	{
		public ChipType chipType;

		public ItemColor itemColor;

		public bool hasIce;

		public int iceLevel;
	}

	public MatchesInBoard matchesInBoard = new MatchesInBoard();

	public TimedCounter matchCounter = new TimedCounter();

	public GameStats gameStats = new GameStats();

	public FindMatches findMatches;

	public FindMatches findMatchesOutside;

	public PopulateBoard populateBoard = new PopulateBoard();

	public PotentialMatches potentialMatches = new PotentialMatches();

	public PowerupCombines powerupCombines = new PowerupCombines();

	public PowerupActivations powerupActivations = new PowerupActivations();

	public List<ItemColor> availableColors = new List<ItemColor>();

	public RandomProvider randomProvider = new RandomProvider();

	private List<ItemColor> selectedColors = new List<ItemColor>();

	private int maxSelectedColorCount = 10;

	private int randomNumber;

	public LevelDefinition.ChipGenerationSettings generationSettings = new LevelDefinition.ChipGenerationSettings();

	public Slot[] slots;

	public List<Slot> sortedSlotsUpdateList = new List<Slot>();

	public List<BoardComponent> boardComponents = new List<BoardComponent>();

	public BubblesBoardComponent bubblesBoardComponent;

	public IntVector2 size;

	public bool isGameEnded;

	public bool isBoardSettled;

	public bool isShufflingBoard;

	public int additionalMoves;

	public int collectedAdditionalMoves;

	public bool ignoreEndConditions;

	public bool isInteractionSuspended;

	public int powerupAnimationsInProgress;

	public bool isPowerupSelectionActive;

	public bool isEndConditionReached;

	public bool isUpdateSuspended;

	public bool isGameSoundsSuspended;

	public bool isAnyConveyorMoveSuspended;

	public int moveCountWhenConveyorTookAction;

	public ActionManager actionManager = new ActionManager();

	public BurriedElements burriedElements = new BurriedElements();

	public MonsterElements monsterElements = new MonsterElements();

	public CarpetSpread carpet = new CarpetSpread();

	public int generatedChipsCount;

	public bool hasMoreMoves;

	public ActionManager nonChangeStateActionMenager = new ActionManager();

	public long currentFrameIndex;

	public float currentTime;

	public int userMovesCount;

	public int userScore;

	public int lastSettledMove;

	public long currentCoins;

	public float lastTimeWhenUserMadeMove;

	public long lastFrameWhenUserMadeMove;

	public int currentMatchesCount;

	public float currentDeltaTime;

	public bool isDirtyInCurrentFrame;

	public int currentMoveMatches => matchesInBoard.matchesCount;

	public int totalAdditionalMoves => additionalMoves + collectedAdditionalMoves;

	public bool isInteractionSuspendedBecausePowerupAnimation => powerupAnimationsInProgress > 0;

	public void AddMatch()
	{
		matchesInBoard.AddMatch(userMovesCount);
	}

	public void ResetMatchesInBoard()
	{
		matchesInBoard.currentMove = userMovesCount;
		matchesInBoard.matchesCount = 1;
	}

	public int RandomRange(int min, int max)
	{
		return randomProvider.Range(min, max);
	}

	public float RandomRange(float min, float max)
	{
		return randomProvider.Range(min, max);
	}

	public ChipCreateParams RandomChip(GeneratorSlotSettings generationSettings)
	{
		ChipCreateParams result = default(ChipCreateParams);
		result.chipType = ChipType.Unknown;
		if (generationSettings == null || generationSettings.chipSettings.Count == 0)
		{
			return result;
		}
		if (!((float)RandomRange(0, 100) <= generationSettings.weight))
		{
			return result;
		}
		float num = 0f;
		for (int i = 0; i < generationSettings.chipSettings.Count; i++)
		{
			LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting = generationSettings.chipSettings[i];
			num += chipSetting.weight;
		}
		float num2 = RandomRange(0f, num);
		result.hasIce = generationSettings.hasIce;
		result.iceLevel = generationSettings.iceLevel;
		for (int j = 0; j < generationSettings.chipSettings.Count; j++)
		{
			LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting2 = generationSettings.chipSettings[j];
			num2 -= chipSetting2.weight;
			bool flag = j == generationSettings.chipSettings.Count - 1;
			if ((num2 <= 0f) | flag)
			{
				result.chipType = chipSetting2.chipType;
				result.itemColor = chipSetting2.itemColor;
				return result;
			}
		}
		return result;
	}

	public ChipCreateParams RandomChip(ItemColor colorToIgnore = ItemColor.Unknown)
	{
		ChipCreateParams result = default(ChipCreateParams);
		result.chipType = ChipType.Chip;
		if (generationSettings.isConfigured)
		{
			float num = 0f;
			for (int i = 0; i < generationSettings.chipSettings.Count; i++)
			{
				LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting = generationSettings.chipSettings[i];
				num += chipSetting.weight;
			}
			float num2 = RandomRange(0f, num);
			for (int j = 0; j < generationSettings.chipSettings.Count; j++)
			{
				LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting2 = generationSettings.chipSettings[j];
				num2 -= chipSetting2.weight;
				bool flag = j == generationSettings.chipSettings.Count - 1;
				if ((num2 <= 0f) | flag)
				{
					result.chipType = chipSetting2.chipType;
					result.itemColor = chipSetting2.itemColor;
					return result;
				}
			}
		}
		ItemColor itemColor = ItemColor.Blue;
		if (availableColors.Count == 0)
		{
			result.itemColor = (ItemColor)RandomRange(0, 5);
			return result;
		}
		int num3 = RandomRange(0, availableColors.Count);
		if (num3 < 0)
		{
			num3 = availableColors.Count + num3;
		}
		num3 %= availableColors.Count;
		itemColor = (result.itemColor = availableColors[num3]);
		return result;
	}

	public ItemColor RandomColor(ItemColor colorToIgnore = ItemColor.Unknown)
	{
		if (generationSettings.isConfigured)
		{
			float num = 0f;
			for (int i = 0; i < generationSettings.chipSettings.Count; i++)
			{
				LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting = generationSettings.chipSettings[i];
				num += chipSetting.weight;
			}
			float num2 = RandomRange(0f, num);
			for (int j = 0; j < generationSettings.chipSettings.Count; j++)
			{
				LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting2 = generationSettings.chipSettings[j];
				num2 -= chipSetting2.weight;
				bool flag = j == generationSettings.chipSettings.Count - 1;
				if ((num2 <= 0f) | flag)
				{
					return chipSetting2.itemColor;
				}
			}
		}
		if (availableColors.Count == 0)
		{
			return (ItemColor)RandomRange(0, 5);
		}
		int num3 = RandomRange(0, availableColors.Count);
		if (num3 < 0)
		{
			num3 = availableColors.Count + num3;
		}
		num3 %= availableColors.Count;
		return availableColors[num3];
	}

	public void Add(BoardComponent boardComponent)
	{
		boardComponents.Add(boardComponent);
	}

	public bool IsOutOfBoard(IntVector2 position)
	{
		return !IsInBoard(position);
	}

	public bool IsInBoard(IntVector2 position)
	{
		if (position.x >= 0 && position.y >= 0 && position.x < size.x)
		{
			return position.y < size.y;
		}
		return false;
	}

	public int DistanceOutsideBoard(IntVector2 position)
	{
		IntVector2 intVector = default(IntVector2);
		if (position.x < 0)
		{
			intVector.x = Mathf.Abs(position.x);
		}
		if (position.x >= size.x)
		{
			intVector.x = position.x - size.x + 1;
		}
		if (position.y < 0)
		{
			intVector.y = Mathf.Abs(position.y);
		}
		if (position.y >= size.y)
		{
			intVector.y = position.y - size.y + 1;
		}
		return Mathf.Max(intVector.x, intVector.y);
	}

	public void Init(IntVector2 size)
	{
		slots = new Slot[size.x * size.y];
		this.size = size;
		findMatches = new FindMatches();
		findMatchesOutside = new FindMatches();
		findMatchesOutside.Init(this);
		findMatches.Init(this);
	}

	public int Index(IntVector2 position)
	{
		return position.x + position.y * size.x;
	}

	public IntVector2 PositionFromIndex(int index)
	{
		return new IntVector2(index % size.x, index / size.x);
	}

	public void SetSlot(IntVector2 position, Slot slot)
	{
		slots[Index(position)] = slot;
	}

	public Slot GetSlot(IntVector2 position)
	{
		if (position.x >= size.x || position.x < 0 || position.y >= size.y || position.y < 0)
		{
			return null;
		}
		return slots[Index(position)];
	}
}
