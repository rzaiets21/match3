using ProtoModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class Match3StagesDB : ScriptableObject
	{
		[Serializable]
		public class LevelReference
		{
			public string levelDBName;

			public string levelName;

			public LevelDB levelDB
			{
				get
				{
					if (string.IsNullOrEmpty(levelDBName))
					{
						return ScriptableObjectSingleton<LevelDB>.instance;
					}
					return LevelDB.NamedInstance(levelDBName);
				}
			}

			public LevelDefinition level => levelDB.Get(levelName);
		}

		[Serializable]
		public class Stage : Match3GameListener
		{
			public enum Difficulty
			{
				Normal,
				Hard,
				Nightmare
			}

			[Serializable]
			public class Alternative
			{
				[SerializeField]
				public LevelReference levelReference = new LevelReference();
			}

			[SerializeField]
			private Difficulty difficulty_;

			[NonSerialized]
			public int index;

			[NonSerialized]
			private Match3StagesDB stages;

			[SerializeField]
			public LevelReference levelReference = new LevelReference();

			private List<LevelReference> levelReferencesToPublish = new List<LevelReference>();

			[SerializeField]
			public List<LevelReference> multiLevelReference = new List<LevelReference>();

			private List<LevelReference> allLevelReferences_;

			[SerializeField]
			public int coinsCount;

			[SerializeField]
			public List<BoosterType> forbittenBoosters = new List<BoosterType>();

			[SerializeField]
			public bool hideUIElements;

			[SerializeField]
			private bool showStarDialog;

			[SerializeField]
			public List<string> startMessages = new List<string>();

			[SerializeField]
			public List<Alternative> alternatives = new List<Alternative>();

			private bool isIntroMessageShown_;

			public Difficulty difficulty => difficulty_;

			private Match3Stages.Stage model => stages.GetModelForStage(this);

			public List<LevelReference> allLevelReferences
			{
				get
				{
					if (allLevelReferences_ == null)
					{
						allLevelReferences_ = new List<LevelReference>();
					}
					allLevelReferences_.Clear();
					if (multiLevelReference.Count > 0)
					{
						allLevelReferences_.AddRange(multiLevelReference);
					}
					else
					{
						allLevelReferences_.Add(levelReference);
					}
					return allLevelReferences_;
				}
			}

			public bool shouldUseStarDialog
			{
				get
				{
					if (showStarDialog)
					{
						return true;
					}
					LevelDefinition level = levelReference.level;
					if (level != null && level.tutorialMatches.Count > 0)
					{
						return true;
					}
					return false;
				}
			}

			public int timesPlayed => model.timesPlayed;

			public bool isIntroMessageShown
			{
				get
				{
					return model.isIntroMessageShown;
				}
				set
				{
					model.isIntroMessageShown = value;
					stages.SaveModel();
				}
			}

			public bool isPassed => model.isPassed;

			public void Init(Match3StagesDB stages, int index)
			{
				this.stages = stages;
				this.index = index;
			}

			public void CompleteStage()
			{
				model.isPassed = true;
				// stages.model.passedStages = 0;
				if (stages.model.passedStages == index)
				{
					stages.model.passedStages++;
				}
				stages.SaveModel();
			}
			
			public void OnGameComplete(GameCompleteParams completeParams)
			{
				return;
				Match3Game game = completeParams.game;
				if (completeParams.isWin)
				{
					model.isPassed = true;
					if (stages.model.passedStages == index)
					{
						stages.model.passedStages++;
					}
					stages.SaveModel();
					Analytics.StageCompletedEvent stageCompletedEvent = new Analytics.StageCompletedEvent();
					stageCompletedEvent.stageState = completeParams.stageState;
					stageCompletedEvent.Send();
					return;
				}
				if (game != null && !game.hasPlayedAnyMoves)
				{
					model.timesPlayed = Mathf.Max(model.timesPlayed - 1, 0);
					stages.SaveModel();
				}
				GameScreen.StageState stageState = completeParams.stageState;
				if (stageState.userMovesCount > 0)
				{
					Analytics.StageFailedEvent stageFailedEvent = new Analytics.StageFailedEvent();
					stageFailedEvent.stageState = stageState;
					stageFailedEvent.Send();
				}
			}

			public void OnGameStarted(GameStartedParams startedParams)
			{
				model.timesPlayed++;
				stages.SaveModel();
				Analytics.StageStartedEvent stageStartedEvent = new Analytics.StageStartedEvent();
				stageStartedEvent.stageState = startedParams.stageState;
				stageStartedEvent.Send();
			}
		}

		private static bool applicationIsQuitting;

		protected static string loadedDBName;

		protected static Match3StagesDB instance_;

		private const string Filename = "st.bytes";

		public List<BoosterConfig> defaultBoosters = new List<BoosterConfig>();

		[SerializeField]
		public int limit;

		[SerializeField]
		public List<Stage> stages = new List<Stage>();

		private Match3Stages model;

		public static Match3StagesDB instance
		{
			get
			{
				string stagesDBName = GGTest.stagesDBName;
				if (stagesDBName != loadedDBName || instance_ == null)
				{
					if (applicationIsQuitting)
					{
						return null;
					}
					loadedDBName = stagesDBName;
					UnityEngine.Debug.Log("Loading singleton from " + stagesDBName);
					instance_ = Resources.Load<Match3StagesDB>(stagesDBName);
					if (instance_ == null)
					{
						instance_ = Resources.Load<Match3StagesDB>(typeof(Match3StagesDB).ToString());
					}
					if (instance_ != null)
					{
						instance_.UpdateData();
					}
					
					UnityEngine.Debug.Log("Loading singleton from " + instance_.name);
					SingletonInit<FileIOChanges>.instance.OnChange(instance_.ReloadModel);
				}
				return instance_;
			}
		}

		public Stage currentStage => stages[Mathf.Clamp(model.passedStages, 0, stages.Count - 1)];
		public Stage nextStage => stages[Mathf.Clamp(model.passedStages + 1, 0, stages.Count - 1)];

		public Stage this[int index] => stages[Mathf.Clamp(model.passedStages + index, 0, stages.Count - 1)];

		public int passedStages => model.passedStages;

		public int stagesPassed
		{
			get
			{
				return model.passedStages;
			}
			set
			{
				model.passedStages = value;
				SaveModel();
			}
		}

		public void OnDestroy()
		{
			applicationIsQuitting = true;
		}

		public Stage GetStageForLevelName(string levelDBName, string levelName)
		{
			for (int i = 0; i < stages.Count; i++)
			{
				Stage stage = stages[i];
				if (stage.levelReference.levelDBName == levelDBName && stage.levelReference.levelName == levelName)
				{
					return stage;
				}
				for (int j = 0; j < stage.multiLevelReference.Count; j++)
				{
					LevelReference levelReference = stage.multiLevelReference[j];
					if (levelReference.levelDBName == levelDBName && levelReference.levelName == levelName)
					{
						return stage;
					}
				}
			}
			return null;
		}

		public int PassedStagesInRow(int maxStage)
		{
			int num = Mathf.Clamp(model.passedStages, 0, stages.Count - 1);
			if (stages[num].timesPlayed > 0)
			{
				return 0;
			}
			int num2 = 0;
			maxStage = Mathf.Max(0, maxStage);
			for (int num3 = num - 1; num3 >= maxStage; num3--)
			{
				Stage stage = stages[num3];
				num2++;
				if (stage.timesPlayed > 1 || num2 > 3)
				{
					break;
				}
			}
			return num2;
		}

		public Match3Stages.Stage GetModelForStage(Stage stage)
		{
			if (model.stages == null)
			{
				model.stages = new List<Match3Stages.Stage>();
			}
			for (int i = 0; i < model.stages.Count; i++)
			{
				Match3Stages.Stage stage2 = model.stages[i];
				if (stage2.stageIndex == stage.index)
				{
					return stage2;
				}
			}
			Match3Stages.Stage stage3 = new Match3Stages.Stage();
			stage3.stageIndex = stage.index;
			stage3.stageName = stage.levelReference.levelName;
			stage3.forbittenBoosters = new List<ProtoModels.BoosterType>();
			for (int j = 0; j < stage.forbittenBoosters.Count; j++)
			{
				BoosterType booster = stage.forbittenBoosters[j];
				stage3.forbittenBoosters.Add(BoosterConfig.BoosterToProtoType(booster));
			}
			model.stages.Add(stage3);
			SaveModel();
			return stage3;
		}

		protected void UpdateData()
		{
			for (int i = 0; i < stages.Count; i++)
			{
				stages[i].Init(this, i);
			}
			ReloadModel();
		}

		public void ResetAll()
		{
			model = new Match3Stages();
			SaveModel();
		}

		private void ReloadModel()
		{
			if (!ProtoIO.LoadFromFileLocal("st.bytes", out model))
			{
				model = new Match3Stages();
			}
		}

		public Match3Stages ModelFromData(CloudSyncData fileSystemData)
		{
			Match3Stages result = ProtoIO.Clone(model);
			if (fileSystemData == null)
			{
				return result;
			}
			CloudSyncData.CloudSyncFile file = ProtoModelExtensions.GetFile(fileSystemData, "st.bytes");
			if (file == null)
			{
				return result;
			}
			Match3Stages match3Stages = null;
			if (!ProtoIO.LoadFromBase64String(file.data, out match3Stages))
			{
				return result;
			}
			if (match3Stages == null)
			{
				return result;
			}
			return match3Stages;
		}

		public void SaveModel()
		{
			ProtoIO.SaveToFileCS("st.bytes", model);
		}

		public Match3Stages GetModel()
		{
			if (!ProtoIO.LoadFromFileLocal("st.bytes", out model))
			{
				model = new Match3Stages();
			}

			return model;
		}

		public void SetNewModel(Match3Stages _model)
		{
			model = _model;
			SaveModel();
		}
	}
}
