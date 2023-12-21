using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class Match3Settings : ScriptableObject
	{
		[Serializable]
		public class MonsterColorSettings
		{
			public ItemColor itemColor;

			public Material material;
		}

		[Serializable]
		public class ChipColorSettings
		{
			public string name;

			public List<ChipChange> chipChanges = new List<ChipChange>();

			public bool hasBoxes;

			public ChipChange GetChipChange(ItemColor itemColor)
			{
				for (int i = 0; i < chipChanges.Count; i++)
				{
					ChipChange chipChange = chipChanges[i];
					if (chipChange.itemColor == itemColor)
					{
						return chipChange;
					}
				}
				return null;
			}
		}

		[Serializable]
		public class ChipChange
		{
			public ItemColor itemColor;

			public bool change;

			public float hue;

			public float saturation = 1f;

			public float brightness = 1f;

			public Texture2D textureReplace;

			public float scale = 1f;

			public float boxHue;

			public float boxSaturation = 1f;

			public float boxBrightness = 1f;
		}

		private static bool applicationIsQuitting;

		protected static Match3Settings instance_;

		protected static string loadedSingletonName;

		[SerializeField]
		private int selectedColorSettings;

		[SerializeField]
		private List<ChipColorSettings> colorSettings = new List<ChipColorSettings>();

		[SerializeField]
		private List<MonsterColorSettings> monsterColorSettings = new List<MonsterColorSettings>();

		[SerializeField]
		private List<ChipChange> chipChanges = new List<ChipChange>();

		[SerializeField]
		private List<ChipDisplaySettings> chipDisplays = new List<ChipDisplaySettings>();

		public int maxPotentialMatchesAtStart = 8;

		public float boosterPlaceDelay = 0.25f;

		public float exitScrollSpeed = 0.5f;

		public GeneralSettings generalSettings = new GeneralSettings();

		public GravitySettings gravitySettings;

		public PipeSettings pipeSettings = new PipeSettings();

		public DestroyChipAction.Settings destroyActionSettings = new DestroyChipAction.Settings();

		public DestroyChipAction.Settings destroyActionSettingsRocket = new DestroyChipAction.Settings();

		public DestroyChipActionGrow.Settings destroyActionGrowSettings = new DestroyChipActionGrow.Settings();

		public DestroyBoxAction.Settings destroyBoxActionSettings = new DestroyBoxAction.Settings();

		public DestroyChipActionExplosion.Settings destroyChipActionExplosionSettings = new DestroyChipActionExplosion.Settings();

		public ExplosionAction.Settings explosionSettings = new ExplosionAction.Settings();

		public CrossExplosionAction.Settings crossExplosionSettings = new CrossExplosionAction.Settings();

		public FlyRocketPieceAction.Settings flyRocketPieceSettings = new FlyRocketPieceAction.Settings();

		public SwapChipsAction.Settings swapChipsActionSettings = new SwapChipsAction.Settings();

		public ShowSwapNotPossibleAction.Settings showSwapNotPossibleActionSettings = new ShowSwapNotPossibleAction.Settings();

		public DestroyFromGravityAction.Settings destroyFromGravityAction = new DestroyFromGravityAction.Settings();

		public CreatePowerupAction.Settings createPowerupActionSettings = new CreatePowerupAction.Settings();

		public DiscoBallDestroyAction.Settings discoBallDestroyActionSettings = new DiscoBallDestroyAction.Settings();

		public FlyCrossRocketAction.Settings flyCrossRocketActionSettings = new FlyCrossRocketAction.Settings();

		public LightSlotComponent.Settings lightSlotSettings = new LightSlotComponent.Settings();

		public BasketBlocker.Settings basketBlockerSettings = new BasketBlocker.Settings();

		public ConveyorBeltBoardComponent.Settings conveyorBeltSettings = new ConveyorBeltBoardComponent.Settings();

		public CollectGoalAction.Settings collectGoalSettings = new CollectGoalAction.Settings();

		public CollectBurriedElementAction.Settings collectBurriedEelementSettings = new CollectBurriedElementAction.Settings();

		public CollectPointsAction.Settings collectPointsActionSettings = new CollectPointsAction.Settings();

		public SeekingMissileAction.Settings seekingMissileSettings = new SeekingMissileAction.Settings();

		public ComboSeekingMissileAction.Settings seekingMissileComboSettings = new ComboSeekingMissileAction.Settings();

		public ShowPotentialMatchAction.Settings showPotentialMatchesSettings = new ShowPotentialMatchAction.Settings();

		public SwapToMatchAction.Settings swapToMatchActionSettings = new SwapToMatchAction.Settings();

		public PlayerInput.Settings playerInputSettings = new PlayerInput.Settings();

		public PlayerInput.MoveFromLineAffector.Settings playerInputMoveFromLineSettings = new PlayerInput.MoveFromLineAffector.Settings();

		public PlayerInput.ExplosionAffector.Settings explosionAffectorSettings = new PlayerInput.ExplosionAffector.Settings();

		public PlayerInputCrossAffector.Settings playerInputCrossAffectorSettings = new PlayerInputCrossAffector.Settings();

		public DiscoBallAffector.Settings discoBallAffectorSettings = new DiscoBallAffector.Settings();

		public ChipJumpBehaviour.Settings chipJumpSettings = new ChipJumpBehaviour.Settings();

		public WobbleAnimation.Settings chipWobbleSettings = new WobbleAnimation.Settings();

		public ChipJumpBehaviour.Settings powerupJumpSettings = new ChipJumpBehaviour.Settings();

		public ChipJumpBehaviour.Settings horizontalRocketJumpSettings = new ChipJumpBehaviour.Settings();

		public ChipJumpBehaviour.Settings verticalRocketJumpSettings = new ChipJumpBehaviour.Settings();

		public ChipJumpBehaviour.Settings bombJumpSettings = new ChipJumpBehaviour.Settings();

		public ChipJumpBehaviour.Settings discoBallJumpSettings = new ChipJumpBehaviour.Settings();

		public ChipJumpBehaviour.Settings seekingMissleJumpSettings = new ChipJumpBehaviour.Settings();

		public ConfirmPurchasePanel.Settings confirmPurchasePanelSettings = new ConfirmPurchasePanel.Settings();

		public DecorateRoomSceneVisualItem.Settings visualItemAnimationSettings = new DecorateRoomSceneVisualItem.Settings();

		public WinScreenBoardOutAnimation winScreenBoardOutAnimation = new WinScreenBoardOutAnimation();

		public WinScreenBoardInAnimation winScreenInAnimation = new WinScreenBoardInAnimation();

		public MoneyPickupAnimation.Settings moneyPickupAnimationSettings = new MoneyPickupAnimation.Settings();

		public MagicHat.Settings magicHatSettings = new MagicHat.Settings();

		public MagicHat.Settings magicHatSettingsBomb = new MagicHat.Settings();

		public DragButton.Settings dragButtonSettings = new DragButton.Settings();

		public StarConsumeAnimation.Settings starConsumeSettings = new StarConsumeAnimation.Settings();

		public GameScreen.MultiLevelAanimationSettings multiLevelAnimationSettings = new GameScreen.MultiLevelAanimationSettings();

		public BurriedElementPiece.Settings burriedElementPieceSettings = new BurriedElementPiece.Settings();

		public AnimateGrowingElementOnChip.Settings animateGrowingElementOnChipSettings = new AnimateGrowingElementOnChip.Settings();

		public AnimateCarryPiece.Settings animateCarryPieceSettings = new AnimateCarryPiece.Settings();

		public GrowingElementPot.Settings growingElementPotSettings = new GrowingElementPot.Settings();

		public WinScreen.Settings winScreenSettings = new WinScreen.Settings();

		public SwipeAffector.Settings swipeAffectorSettings = new SwipeAffector.Settings();

		public SeekingMissleAffector.Settings seekingMissleAffectorSettings = new SeekingMissleAffector.Settings();

		public PowerLineAffector.Settings powerLineAffectorSettings = new PowerLineAffector.Settings();

		public PowerCrossAffector.Settings powerCrossAffectorSettings = new PowerCrossAffector.Settings();

		public CombineChipAffectors.Settings combineChipAffectorSettings = new CombineChipAffectors.Settings();

		public PlacePowerupAction.Settings placePowerupActionSettings = new PlacePowerupAction.Settings();

		public DestroyMatchingIslandBlinkAction.Settings destroyIslandBlinkSettings = new DestroyMatchingIslandBlinkAction.Settings();

		public DestroyMatchingIslandBlinkAction.Settings destroyAfterSettings = new DestroyMatchingIslandBlinkAction.Settings();

		public TutorialHandController.Settings tutorialHandSettings = new TutorialHandController.Settings();

		public TutorialHandController.Settings tutorialSwipeSettings = new TutorialHandController.Settings();

		public TutorialHandController.Settings tutorialTouchSettings = new TutorialHandController.Settings();

		protected static string FilenameToLoad
		{
			get
			{
				string text = GGTest.settingsDBName;
				if (string.IsNullOrWhiteSpace(text))
				{
					text = typeof(Match3Settings).ToString();
				}
				return text;
			}
		}

		public static Match3Settings instance
		{
			get
			{
				if (NeedsToLoadSingleton())
				{
					if (applicationIsQuitting)
					{
						return null;
					}
					string filenameToLoad = FilenameToLoad;
					loadedSingletonName = FilenameToLoad;
					UnityEngine.Debug.Log("Loading singleton from " + filenameToLoad);
					instance_ = Resources.Load<Match3Settings>(filenameToLoad);
				}
				return instance_;
			}
		}

		public void OnDestroy()
		{
			applicationIsQuitting = true;
		}

		protected static bool NeedsToLoadSingleton()
		{
			if (instance_ == null)
			{
				return true;
			}
			return loadedSingletonName != FilenameToLoad;
		}

		public MonsterColorSettings GeMonsterColorSettings(ItemColor itemColor)
		{
			for (int i = 0; i < this.monsterColorSettings.Count; i++)
			{
				MonsterColorSettings monsterColorSettings = this.monsterColorSettings[i];
				if (monsterColorSettings.itemColor == itemColor)
				{
					return monsterColorSettings;
				}
			}
			return null;
		}

		public ChipColorSettings GetColorSettings()
		{
			if (selectedColorSettings >= colorSettings.Count || selectedColorSettings < 0)
			{
				return null;
			}
			return colorSettings[selectedColorSettings];
		}

		public ChipChange GetChipChange(ItemColor itemColor)
		{
			return GetColorSettings()?.GetChipChange(itemColor);
		}

		public ChipDisplaySettings GetChipDisplaySettings(ChipType chipType, ItemColor itemColor)
		{
			bool flag = chipType == ChipType.Chip || chipType == ChipType.MonsterChip;
			for (int i = 0; i < chipDisplays.Count; i++)
			{
				ChipDisplaySettings chipDisplaySettings = chipDisplays[i];
				if (chipDisplaySettings.chipType == chipType && (!flag || chipDisplaySettings.itemColor == itemColor))
				{
					return chipDisplaySettings;
				}
			}
			return null;
		}
	}
}
