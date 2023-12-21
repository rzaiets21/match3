using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGMatch3
{
	[ExecuteInEditMode]
	public class LevelEditorVisualizer : MonoBehaviour, Match3GameListener
	{
		[Serializable]
		public class UIElementPool
		{
			public GameObject prefab;

			public Transform parent;

			public List<GameObject> usedObjects = new List<GameObject>();

			public List<GameObject> notUsedObjects = new List<GameObject>();

			public float prefabWidth
			{
				get
				{
					if (prefab == null)
					{
						return 0f;
					}
					RectTransform component = prefab.GetComponent<RectTransform>();
					if (component == null)
					{
						return 0f;
					}
					return component.sizeDelta.x;
				}
			}

			public float prefabHeight
			{
				get
				{
					if (prefab == null)
					{
						return 0f;
					}
					RectTransform component = prefab.GetComponent<RectTransform>();
					if (component == null)
					{
						return 0f;
					}
					return component.sizeDelta.y;
				}
			}

			public void DestroyObjectsFromPool()
			{
				Clear(hideNotActive: false);
				for (int i = 0; i < notUsedObjects.Count; i++)
				{
					GameObject obj = notUsedObjects[i];
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(obj);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(obj);
					}
				}
				notUsedObjects.Clear();
			}

			public void Init()
			{
				prefab.SetActive(value: false);
			}

			public void Clear(bool hideNotActive)
			{
				for (int num = usedObjects.Count - 1; num >= 0; num--)
				{
					GameObject gameObject = usedObjects[num];
					if (hideNotActive)
					{
						gameObject.SetActive(value: false);
					}
					notUsedObjects.Add(gameObject);
				}
				usedObjects.Clear();
			}

			public void HideNotActive()
			{
				for (int i = 0; i < notUsedObjects.Count; i++)
				{
					notUsedObjects[i].SetActive(value: false);
				}
			}

			public T Next<T>(bool activate = true) where T : MonoBehaviour
			{
				GameObject gameObject = null;
				if (notUsedObjects.Count > 0)
				{
					int index = notUsedObjects.Count - 1;
					gameObject = notUsedObjects[index];
					notUsedObjects.RemoveAt(index);
				}
				else
				{
					gameObject = UnityEngine.Object.Instantiate(prefab, parent);
				}
				usedObjects.Add(gameObject);
				if (activate)
				{
					gameObject.SetActive(value: true);
				}
				return gameObject.GetComponent<T>();
			}
		}

		public struct GeneratorSetupHit
		{
			public bool isHit;

			public int generatorSetupIndex;

			public int generatorSetupChipIndex;
		}

		[SerializeField]
		public StagesAnalyticsDB analyticsDB;

		[SerializeField]
		public string levelDBName;

		[SerializeField]
		public List<string> possibleLevelDB = new List<string>();

		public TestExecutor test = new TestExecutor();

		[SerializeField]
		public bool justEditMode;

		[SerializeField]
		public bool showDifficulties;

		[SerializeField]
		public bool limitStages;

		[SerializeField]
		public int minStage;

		[SerializeField]
		public int maxStage;

		[NonSerialized]
		public bool isGamePlaying;

		[SerializeField]
		private UIElementPool slotPool = new UIElementPool();

		[SerializeField]
		private UIElementPool burriedElementPool = new UIElementPool();

		[SerializeField]
		private UIElementPool monsterElementPool = new UIElementPool();

		[SerializeField]
		private UIElementPool generatorSetupPool = new UIElementPool();

		[SerializeField]
		public RectTransform container;

		[SerializeField]
		public RectTransform screenContainer;

		[SerializeField]
		private RectTransform buttonsContainer;

		[SerializeField]
		private Text buttonLabel;

		[SerializeField]
		private RectTransform innerContainer;

		[SerializeField]
		private LevelEditorSlot markerSlot;

		[SerializeField]
		public LevelDefinition.SlotDefinition markerSlotDefinition = new LevelDefinition.SlotDefinition();

		[SerializeField]
		public int repeatTimes = 10;

		[SerializeField]
		public bool isHudDissabled = true;

		[SerializeField]
		public int stepsPerFrame = 1000;

		[SerializeField]
		public bool humanVisibleDebug;

		[SerializeField]
		public bool setRandomSeed;

		[SerializeField]
		public int randomSeed;

		[SerializeField]
		public GameResults lastResult;

		[SerializeField]
		public string resultString;

		private LevelDB loadedLevelDB;

		private string loadedLevelDBName;

		[NonSerialized]
		public long lastShownLevelIndex;

		[NonSerialized]
		public string lastShownLevelName;

		private List<GeneratorSetup> generatorSetupsInSameRow = new List<GeneratorSetup>();

		public bool isShowDifficiltiesVisible
		{
			get
			{
				if (showDifficulties)
				{
					return !justEditMode;
				}
				return false;
			}
		}

		public bool isShowLevelsVisible => !justEditMode;

		public bool isShowStagesVisible => !justEditMode;

		public LevelDB levelDB
		{
			get
			{
				if (string.IsNullOrEmpty(levelDBName))
				{
					return ScriptableObjectSingleton<LevelDB>.instance;
				}
				if (loadedLevelDB != null && loadedLevelDBName == levelDBName)
				{
					return loadedLevelDB;
				}
				loadedLevelDBName = levelDBName;
				loadedLevelDB = Resources.Load<LevelDB>(levelDBName);
				if (loadedLevelDB == null)
				{
					loadedLevelDB = ScriptableObjectSingleton<LevelDB>.instance;
				}
				return loadedLevelDB;
			}
		}

		public string levelName
		{
			get
			{
				return levelDB.currentLevelName;
			}
			set
			{
				UnityEngine.Debug.Log("Changed level Name " + value);
				levelDB.currentLevelName = value;
			}
		}

		public LevelDefinition level
		{
			get
			{
				LevelDefinition levelDefinition = levelDB.Get(levelName);
				if (levelDefinition == null)
				{
					levelDefinition = levelDB.levels[0];
				}
				return levelDefinition;
			}
		}

		public bool IsWorldToPositionOnBoard(Vector3 wordPos)
		{
			LevelDefinition level = this.level;
			IntVector2 intVector = WorldToBoardPosition(wordPos);
			if (intVector.x < 0 || intVector.x >= level.size.width || intVector.y < 0 || intVector.y >= level.size.height)
			{
				return false;
			}
			return true;
		}

		public IntVector2 WorldToBoardPositionClamped(Vector3 wordPos)
		{
			LevelDefinition level = this.level;
			IntVector2 intVector = WorldToBoardPosition(wordPos);
			intVector.x = Mathf.Clamp(intVector.x, 0, level.size.width - 1);
			intVector.y = Mathf.Clamp(intVector.y, 0, level.size.height - 1);
			return intVector;
		}

		public IntVector2 WorldToBoardPosition(Vector3 wordPos)
		{
			Vector3 a = container.InverseTransformPoint(wordPos);
			LevelDefinition level = this.level;
			float num = slotPool.prefabWidth * innerContainer.localScale.x;
			float num2 = slotPool.prefabHeight * innerContainer.localScale.y;
			float num3 = num * (float)level.size.width;
			float num4 = num2 * (float)level.size.height;
			Vector3 b = new Vector3((0f - num3) * 0.5f, (0f - num4) * 0.5f, 0f);
			Vector3 vector = a - b;
			int x = Mathf.FloorToInt(vector.x / num);
			int y = Mathf.FloorToInt(vector.y / num2);
			return new IntVector2(x, y);
		}

		public void HideMarker()
		{
			if (markerSlot.gameObject.activeSelf)
			{
				markerSlot.gameObject.SetActive(value: false);
			}
		}

		public void SetSlot(IntVector2 position, LevelDefinition.SlotDefinition slotDefinition)
		{
			level.SetSlot(position, slotDefinition.Clone());
		}

		public void ShowMarker(LevelDefinition.SlotDefinition slotDefiniton)
		{
			LevelDefinition level = this.level;
			float prefabWidth = slotPool.prefabWidth;
			float prefabHeight = slotPool.prefabHeight;
			float num = prefabWidth * (float)level.size.width;
			float num2 = prefabHeight * (float)level.size.height;
			Vector3 a = new Vector3((0f - num) * 0.5f, (0f - num2) * 0.5f, 0f);
			markerSlot.Init(level, slotDefiniton);
			markerSlot.gameObject.SetActive(value: true);
			markerSlot.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
			Vector3 localPosition = a + Vector3.right * (((float)slotDefiniton.position.x + 0.5f) * prefabWidth) + Vector3.up * (((float)slotDefiniton.position.y + 0.5f) * prefabHeight);
			markerSlot.GetComponent<RectTransform>().localPosition = localPosition;
		}

		public void Refresh()
		{
			ShowLevel(level);
		}

		public Vector3 GetLocalPosition(LevelDefinition level, IntVector2 position)
		{
			float prefabWidth = slotPool.prefabWidth;
			float prefabHeight = slotPool.prefabHeight;
			float num = prefabWidth * (float)level.size.width;
			float num2 = prefabHeight * (float)level.size.height;
			return new Vector3((0f - num) * 0.5f, (0f - num2) * 0.5f, 0f) + Vector3.right * (((float)position.x + 0.5f) * prefabWidth) + Vector3.up * (((float)position.y + 0.5f) * prefabHeight);
		}

		public void ShowLevel(LevelDefinition level)
		{
			lastShownLevelIndex = level.versionIndex;
			lastShownLevelName = level.name;
			level.EnsureSizeAndInit();
			float prefabWidth = slotPool.prefabWidth;
			float prefabHeight = slotPool.prefabHeight;
			float num = prefabWidth * (float)level.size.width;
			float num2 = prefabHeight * (float)level.size.height;
			float num3 = Mathf.Max(container.sizeDelta.x / num, container.sizeDelta.y / num2);
			innerContainer.localScale = new Vector3(num3, num3, 1f);
			slotPool.Init();
			slotPool.Clear(hideNotActive: false);
			burriedElementPool.Init();
			burriedElementPool.Clear(hideNotActive: false);
			monsterElementPool.Init();
			monsterElementPool.Clear(hideNotActive: false);
			Vector3 a = new Vector3((0f - num) * 0.5f, (0f - num2) * 0.5f, 0f);
			List<LevelDefinition.SlotDefinition> slots = level.slots;
			for (int i = 0; i < slots.Count; i++)
			{
				LevelDefinition.SlotDefinition slotDefinition = slots[i];
				LevelEditorSlot levelEditorSlot = slotPool.Next<LevelEditorSlot>();
				levelEditorSlot.Init(level, slotDefinition);
				Vector3 vector2 = levelEditorSlot.GetComponent<RectTransform>().localPosition = a + Vector3.right * (((float)slotDefinition.position.x + 0.5f) * prefabWidth) + Vector3.up * (((float)slotDefinition.position.y + 0.5f) * prefabHeight);
			}
			List<LevelDefinition.BurriedElement> elements = level.burriedElements.elements;
			for (int j = 0; j < elements.Count; j++)
			{
				LevelDefinition.BurriedElement burriedElement = elements[j];
				burriedElementPool.Next<LevelEditorBurriedElement>().Init(this, level, burriedElement);
			}
			List<LevelDefinition.MonsterElement> elements2 = level.monsterElements.elements;
			for (int k = 0; k < elements2.Count; k++)
			{
				LevelDefinition.MonsterElement monsterElement = elements2[k];
				monsterElementPool.Next<LevelEditorMonster>().Init(this, level, monsterElement);
			}
			slotPool.HideNotActive();
			burriedElementPool.HideNotActive();
			monsterElementPool.HideNotActive();
			generatorSetupPool.Clear(hideNotActive: false);
			List<GeneratorSetup> generatorSetups = level.generatorSetups;
			for (int l = 0; l < generatorSetups.Count; l++)
			{
				GeneratorSetup generatorSetup = generatorSetups[l];
				LevelDefinition.SlotDefinition slot = level.GetSlot(generatorSetup.position);
				if (slot != null && slot.generatorSettings.isGeneratorOn)
				{
					LevelEditorGeneratorSetup levelEditorGeneratorSetup = generatorSetupPool.Next<LevelEditorGeneratorSetup>();
					Vector3 startPositionForGeneratorSetup = GetStartPositionForGeneratorSetup(l);
					levelEditorGeneratorSetup.Init(generatorSetup, startPositionForGeneratorSetup);
					levelEditorGeneratorSetup.transform.localPosition = Vector3.zero;
				}
			}
			generatorSetupPool.HideNotActive();
		}

		public GeneratorSetupHit GetGeneratorSetupHit(Vector3 worldPos)
		{
			GeneratorSetupHit result = default(GeneratorSetupHit);
			Vector3 vector = innerContainer.InverseTransformPoint(worldPos);
			List<GeneratorSetup> generatorSetups = level.generatorSetups;
			IntVector2 intVector = WorldToBoardPosition(worldPos);
			if (intVector.x < 0 || intVector.x >= level.size.width)
			{
				return result;
			}
			float chipHeight = generatorSetupPool.prefab.GetComponent<LevelEditorGeneratorSetup>().ChipHeight;
			for (int i = 0; i < generatorSetups.Count; i++)
			{
				GeneratorSetup generatorSetup = generatorSetups[i];
				if (generatorSetup.position.x == intVector.x)
				{
					Vector3 vector2 = GetStartPositionForGeneratorSetup(i) + Vector3.down * chipHeight * 0.5f;
					int num = Mathf.FloorToInt((vector.y - vector2.y) / chipHeight);
					if (num >= 0 && num <= generatorSetup.chips.Count)
					{
						result.isHit = true;
						result.generatorSetupIndex = i;
						result.generatorSetupChipIndex = num;
						return result;
					}
				}
			}
			return result;
		}

		public Vector3 GetStartPositionForGeneratorSetup(int generatorSetupIndex)
		{
			List<GeneratorSetup> generatorSetups = level.generatorSetups;
			GeneratorSetup generatorSetup = generatorSetups[generatorSetupIndex];
			generatorSetupsInSameRow.Clear();
			for (int i = 0; i < generatorSetups.Count; i++)
			{
				GeneratorSetup generatorSetup2 = generatorSetups[i];
				LevelDefinition.SlotDefinition slot = level.GetSlot(generatorSetup2.position);
				if (slot != null && slot.generatorSettings.isGeneratorOn && generatorSetup2.position.x == generatorSetup.position.x && generatorSetup2.position.y <= generatorSetup.position.y && (generatorSetup2.position.y != generatorSetup.position.y || generatorSetupIndex > i))
				{
					generatorSetupsInSameRow.Add(generatorSetup2);
				}
			}
			Vector3 a = GetLocalPosition(level, new IntVector2(generatorSetup.position.x, level.size.height));
			float chipHeight = generatorSetupPool.prefab.GetComponent<LevelEditorGeneratorSetup>().ChipHeight;
			float num = 50f;
			for (int j = 0; j < generatorSetupsInSameRow.Count; j++)
			{
				GeneratorSetup generatorSetup3 = generatorSetupsInSameRow[j];
				a += Vector3.up * (num + (float)(generatorSetup3.chips.Count + 1) * chipHeight);
			}
			return a + Vector3.up * num;
		}

		public void GrowLevel(int cellsToAdd)
		{
			level.size.Clone();
			List<LevelDefinition.SlotDefinition> list = new List<LevelDefinition.SlotDefinition>();
			list.AddRange(level.slots);
			level.size.width += cellsToAdd * 2;
			level.size.height += cellsToAdd * 2;
			level.slots.Clear();
			level.EnsureSizeAndInit();
			CopyWithOffset(list, level, new IntVector2(cellsToAdd, cellsToAdd));
		}

		public void GrowLevelWidth(int cellsToAdd)
		{
			level.size.Clone();
			List<LevelDefinition.SlotDefinition> list = new List<LevelDefinition.SlotDefinition>();
			list.AddRange(level.slots);
			level.size.width += cellsToAdd * 2;
			level.slots.Clear();
			level.EnsureSizeAndInit();
			CopyWithOffset(list, level, new IntVector2(cellsToAdd, 0));
		}

		public void GrowLevelHeight(int cellsToAdd)
		{
			level.size.Clone();
			List<LevelDefinition.SlotDefinition> list = new List<LevelDefinition.SlotDefinition>();
			list.AddRange(level.slots);
			level.size.height += cellsToAdd * 2;
			level.slots.Clear();
			level.EnsureSizeAndInit();
			CopyWithOffset(list, level, new IntVector2(0, cellsToAdd));
		}

		public void ClearLevel()
		{
			level.slots.Clear();
			level.burriedElements.elements.Clear();
			level.tutorialMatches.Clear();
			level.generatorSetups.Clear();
			level.EnsureSizeAndInit();
		}

		public void TrimLevel()
		{
			IntVector2 intVector = new IntVector2(level.size.width - 1, level.size.height - 1);
			IntVector2 intVector2 = new IntVector2(0, 0);
			for (int i = 0; i < level.size.width; i++)
			{
				for (int j = 0; j < level.size.height; j++)
				{
					if (level.GetSlot(new IntVector2(i, j)).slotType != 0)
					{
						intVector.x = Mathf.Min(i, intVector.x);
						intVector.y = Mathf.Min(j, intVector.y);
						intVector2.x = Mathf.Max(i, intVector2.x);
						intVector2.y = Mathf.Max(j, intVector2.y);
					}
				}
			}
			int a = intVector2.x - intVector.x + 1;
			int a2 = intVector2.y - intVector.y + 1;
			int b = 3;
			a = Mathf.Max(a, b);
			a2 = Mathf.Max(a2, b);
			if (a != level.size.width || a2 != level.size.height)
			{
				level.size.Clone();
				List<LevelDefinition.SlotDefinition> list = new List<LevelDefinition.SlotDefinition>();
				list.AddRange(level.slots);
				level.size.width = a;
				level.size.height = a2;
				level.slots.Clear();
				level.EnsureSizeAndInit();
				CopyWithOffsetBounds(offset: new IntVector2(-intVector.x, -intVector.y), copyFrom: list, level: level);
			}
		}

		private void CopyWithOffsetBounds(List<LevelDefinition.SlotDefinition> copyFrom, LevelDefinition level, IntVector2 offset)
		{
			for (int i = 0; i < copyFrom.Count; i++)
			{
				LevelDefinition.SlotDefinition slotDefinition = copyFrom[i];
				slotDefinition.position += offset;
				if (slotDefinition.position.x >= 0 && slotDefinition.position.y >= 0 && slotDefinition.position.x < level.size.width && slotDefinition.position.y < level.size.height)
				{
					level.SetSlot(slotDefinition.position, slotDefinition);
				}
			}
			List<GeneratorSetup> generatorSetups = level.generatorSetups;
			for (int j = 0; j < generatorSetups.Count; j++)
			{
				generatorSetups[j].position += offset;
			}
			List<LevelDefinition.TutorialMatch> tutorialMatches = level.tutorialMatches;
			for (int k = 0; k < tutorialMatches.Count; k++)
			{
				tutorialMatches[k].OffsetAllSlots(offset);
			}
			level.burriedElements.MoveByOffset(offset);
			level.monsterElements.MoveByOffset(offset);
		}

		private void CopyWithOffset(List<LevelDefinition.SlotDefinition> copyFrom, LevelDefinition level, IntVector2 offset)
		{
			for (int i = 0; i < copyFrom.Count; i++)
			{
				LevelDefinition.SlotDefinition slotDefinition = copyFrom[i];
				slotDefinition.position += offset;
				level.SetSlot(slotDefinition.position, slotDefinition);
			}
			List<GeneratorSetup> generatorSetups = level.generatorSetups;
			for (int j = 0; j < generatorSetups.Count; j++)
			{
				generatorSetups[j].position += offset;
			}
			List<LevelDefinition.TutorialMatch> tutorialMatches = level.tutorialMatches;
			for (int k = 0; k < tutorialMatches.Count; k++)
			{
				tutorialMatches[k].OffsetAllSlots(offset);
			}
			level.burriedElements.MoveByOffset(offset);
			level.monsterElements.MoveByOffset(offset);
		}

		public void PopulateBoardRandom()
		{
			PopulateBoard.BoardRepresentation boardRepresentation = new PopulateBoard.BoardRepresentation();
			boardRepresentation.Init(level);
			PopulateBoard.Params @params = new PopulateBoard.Params();
			@params.randomProvider = new RandomProvider();
			for (int i = 0; i < level.numChips; i++)
			{
				@params.availableColors.Add((ItemColor)i);
			}
			@params.maxPotentialMatches = Match3Settings.instance.maxPotentialMatchesAtStart;
			PopulateBoard populateBoard = new PopulateBoard();
			populateBoard.RandomPopulate(boardRepresentation, @params);
			List<LevelDefinition.SlotDefinition> slots = level.slots;
			for (int j = 0; j < slots.Count; j++)
			{
				LevelDefinition.SlotDefinition slotDefinition = slots[j];
				if (slotDefinition.slotType != 0 && slotDefinition.chipType == ChipType.RandomChip)
				{
					PopulateBoard.BoardRepresentation.RepresentationSlot slot = populateBoard.board.GetSlot(slotDefinition.position);
					if (slot != null && slot.isGenerated)
					{
						slotDefinition.chipType = ChipType.Chip;
						slotDefinition.itemColor = slot.itemColor;
					}
				}
			}
		}

		private void OnEnable()
		{
			if (Application.isPlaying)
			{
				buttonsContainer.gameObject.SetActive(value: true);
				SetLabel(buttonLabel, "Play Game");
			}
		}

		private void Update()
		{
			if (level != null)
			{
				if (lastShownLevelIndex != level.versionIndex || lastShownLevelName != level.name)
				{
					ShowLevel(level);
				}
				if (Application.isPlaying)
				{
					test.Update();
				}
			}
		}

		public void OnGameComplete(GameCompleteParams completeParams)
		{
			StopGame();
		}

		public void OnGameStarted(GameStartedParams startedParams)
		{
		}

		public void PlayGame()
		{
			if (Application.isPlaying)
			{
				NavigationManager.instance.GetObject<GameScreen>().Show(new Match3GameParams
				{
					level = level,
					listener = this,
					setRandomSeed = setRandomSeed,
					randomSeed = randomSeed
				});
				container.gameObject.SetActive(value: false);
				SetLabel(buttonLabel, "Stop Game");
				isGamePlaying = true;
			}
		}

		public void StopMultipleTests()
		{
			if (Application.isPlaying)
			{
				test.StopTesting();
				StopGame();
			}
		}

		public void PlayMultipleTests()
		{
			if (Application.isPlaying)
			{
				TestExecutor.ExecuteArguments arguments = default(TestExecutor.ExecuteArguments);
				arguments.repeatTimes = repeatTimes;
				arguments.visualizer = this;
				test.StartTesting(arguments);
			}
		}

		public void PlayTestGame()
		{
			if (Application.isPlaying)
			{
				NavigationManager.instance.GetObject<GameScreen>().Show(new Match3GameParams
				{
					level = level,
					listener = this,
					isAIPlayer = true,
					iterations = stepsPerFrame,
					isHudDissabled = isHudDissabled,
					timeScale = 10000f
				});
				container.gameObject.SetActive(value: false);
				SetLabel(buttonLabel, "Stop Game");
				isGamePlaying = true;
			}
		}

		public void PlayGame(Match3GameParams initParams)
		{
			if (Application.isPlaying)
			{
				NavigationManager.instance.GetObject<GameScreen>().Show(initParams);
				container.gameObject.SetActive(value: false);
				SetLabel(buttonLabel, "Stop Game");
				isGamePlaying = true;
			}
		}

		private void SetLabel(Text label, string text)
		{
			if (!(label == null) && !(label.text == text))
			{
				label.text = text;
			}
		}

		public void StopGame()
		{
			container.gameObject.SetActive(value: true);
			SetLabel(buttonLabel, "Play Game");
			isGamePlaying = false;
			NavigationManager instance = NavigationManager.instance;
			if (!(instance == null))
			{
				instance.Pop();
			}
		}

		public void ClearPools()
		{
			slotPool.DestroyObjectsFromPool();
			burriedElementPool.DestroyObjectsFromPool();
			generatorSetupPool.DestroyObjectsFromPool();
			monsterElementPool.DestroyObjectsFromPool();
			LevelEditorSlot levelEditorSlot = markerSlot;
			LevelEditorSlot component = UnityEngine.Object.Instantiate(slotPool.prefab, levelEditorSlot.transform.parent).GetComponent<LevelEditorSlot>();
			component.transform.localPosition = levelEditorSlot.transform.localPosition;
			component.transform.localScale = levelEditorSlot.transform.localScale;
			markerSlot = component;
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(levelEditorSlot.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(levelEditorSlot.gameObject);
			}
			ShowLevel(level);
		}

		public void Callback_TogglePlay()
		{
			if (isGamePlaying)
			{
				StopGame();
			}
			else
			{
				PlayGame();
			}
		}
	}
}
