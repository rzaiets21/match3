using GGMatch3;
using ProtoModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RateCallerSettings : ScriptableObjectSingleton<RateCallerSettings>
{
	[Serializable]
	public class RateIntervalConfig
	{
		[SerializeField]
		private int _levelsPassedThreshold;

		public int levelsPassedThreshold => _levelsPassedThreshold;

		public bool ShouldShow(int levelsPassed)
		{
			return levelsPassed >= levelsPassedThreshold;
		}
	}

	private AppRateDAO model;

	private static string RateFilename = "internalRB.byte";

	[SerializeField]
	private int minStagesBetweenRates = 5;

	[SerializeField]
	private List<RateIntervalConfig> checkIntervals;

	private RateIntervalConfig checkInterval
	{
		get
		{
			if (checkIntervals.Count == 0)
			{
				return null;
			}
			int num = 0;
			for (int i = 0; i < checkIntervals.Count; i++)
			{
				RateIntervalConfig result = checkIntervals[i];
				num++;
				if (timesShown < num)
				{
					return result;
				}
			}
			return null;
		}
	}

	public long lastShownTime => model.timestampWhenAskedForRate;

	public int timesShown => model.timesRateShown;

	public void OnDialogShow()
	{
		model.timestampWhenAskedForRate = DateTime.Now.Ticks;
		model.timesRateShown++;
		model.stagesPassedSinceLastRate = Match3StagesDB.instance.passedStages;
		Save();
	}

	public void OnUserRated()
	{
		model.userRatedApp = true;
		Save();
	}

	public void OnUserNotLike()
	{
		model.userDoesNotLikeApp = true;
		Save();
	}

	public bool ShouldShow(int levelsPassed)
	{
		if (model.userRatedApp)
		{
			return false;
		}
		if (levelsPassed - model.stagesPassedSinceLastRate < minStagesBetweenRates)
		{
			return false;
		}
		if (checkInterval == null)
		{
			return false;
		}
		return checkInterval.ShouldShow(levelsPassed);
	}

	protected override void UpdateData()
	{
		base.UpdateData();
		ReloadModel();
		SingletonInit<FileIOChanges>.instance.OnChange(ReloadModel);
	}

	public void ReloadModel()
	{
		if (!ProtoIO.LoadFromFileLocal(RateFilename, out model))
		{
			model = new AppRateDAO();
		}
	}

	public void Save()
	{
		ProtoIO.SaveToFileCS(RateFilename, model);
	}

	public void Reset()
	{
		model.isUserEnjoying = false;
		model.playerGamesSinceLastRate = 0;
		model.stagesPassedSinceLastRate = 0;
		model.timesRateShown = 0;
		model.timestampWhenAskedForRate = 0L;
		model.userRatedApp = false;
		model.multiplayerGamesSinceLastRate = 0;
		Save();
	}
}
