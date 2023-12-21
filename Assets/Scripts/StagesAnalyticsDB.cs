using System;
using System.Collections.Generic;
using UnityEngine;

public class StagesAnalyticsDB : ScriptableObject
{
	[Serializable]
	public class StageData
	{
		[Serializable]
		public class MovesData
		{
			public enum PassedType
			{
				Passed,
				NotPassed
			}

			[SerializeField]
			public PassedType passed;

			[SerializeField]
			public int moves;

			[SerializeField]
			public int games;
		}

		[Serializable]
		public class PlayedData
		{
			[SerializeField]
			public int timesPlayed;

			[SerializeField]
			public int games;
		}

		public int stageIndex;

		[SerializeField]
		public List<MovesData> moves = new List<MovesData>();

		[SerializeField]
		public List<PlayedData> playedData = new List<PlayedData>();

		[SerializeField]
		public int usersStarted;
	}

	[SerializeField]
	public List<StageData> stages = new List<StageData>();

	public StageData GetOrCreateStageData(int index)
	{
		StageData stageData = GetStageData(index);
		if (stageData != null)
		{
			return stageData;
		}
		stageData = new StageData();
		stageData.stageIndex = index;
		stages.Add(stageData);
		return stageData;
	}

	public StageData GetStageData(int index)
	{
		for (int i = 0; i < stages.Count; i++)
		{
			StageData stageData = stages[i];
			if (stageData.stageIndex == index)
			{
				return stageData;
			}
		}
		return null;
	}
}
