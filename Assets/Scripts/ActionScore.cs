namespace GGMatch3
{
	public struct ActionScore
	{
		public int goalsCount;

		public int powerupsCreated;

		public int obstaclesDestroyed;

		public bool isZero
		{
			get
			{
				if (goalsCount == 0 && powerupsCreated == 0)
				{
					return obstaclesDestroyed == 0;
				}
				return false;
			}
		}

		public int GoalsAndObstaclesScore(int goalsFactor)
		{
			return goalsCount * goalsFactor + obstaclesDestroyed;
		}

		public static ActionScore operator +(ActionScore a, ActionScore b)
		{
			ActionScore result = default(ActionScore);
			result.goalsCount = a.goalsCount + b.goalsCount;
			result.powerupsCreated = a.powerupsCreated + b.powerupsCreated;
			result.obstaclesDestroyed = a.obstaclesDestroyed + b.obstaclesDestroyed;
			return result;
		}

		public string ToDebugString()
		{
			return $"(goals: {goalsCount}, powerups: {powerupsCreated}, obstacles: {obstaclesDestroyed})";
		}
	}
}
