using System;
using System.Collections.Generic;

namespace GGMatch3
{
	[Serializable]
	public class GameResults
	{
		[Serializable]
		public class GameResult
		{
			public int randomSeed;

			public int numberOfMoves;

			public bool isComplete;

			public GameStats gameStats;
		}

		public string levelName;

		public int repeats;

		public List<GameResult> gameResults = new List<GameResult>();
	}
}
