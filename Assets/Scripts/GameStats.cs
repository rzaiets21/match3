using GGMatch3;
using System;
using System.Collections.Generic;

[Serializable]
public class GameStats
{
	public enum MoveType
	{
		Unknown,
		Match,
		PowerupActivation,
		PowerupMix,
		PowerupTap
	}

	[Serializable]
	public class Move
	{
		public IntVector2 fromPosition;

		public IntVector2 toPosition;

		public MoveType moveType;

		public long frameWhenActivated;
	}

	public List<Move> moves = new List<Move>();

	public int initialSeed;

	public int goalsFromPowerups;

	public int goalsFromUserMatches;

	public int goalsFromInertion;

	public int matchesFromUser;

	public int matchesFromInertion;

	public int powerupsCreatedFromUser;

	public int powerupsCreatedFromInertion;

	public int powerupsUsedBySwipe;

	public int powerupsMixed;

	public int powerupsUsedByTap;

	public int noUsefulMovesTurns;
}
