using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class HeuristicAIPlayer
	{
		public struct PotentialMove
		{
			public ActionScore moveScore;

			public PotentialMatches.CompoundSlotsSet swapMatch;

			public PowerupActivations.PowerupActivation powerupActivation;

			public PowerupCombines.PowerupCombine powerupCombine;

			public ActionScore powerupScore;

			public PotentialMove(ActionScore moveScore, PotentialMatches.CompoundSlotsSet swapMatch, ActionScore powerupScore)
			{
				this.moveScore = moveScore;
				this.swapMatch = swapMatch;
				powerupActivation = null;
				powerupCombine = null;
				this.powerupScore = powerupScore;
			}

			public PotentialMove(ActionScore moveScore, PowerupActivations.PowerupActivation powerupActivation)
			{
				this.moveScore = moveScore;
				swapMatch = null;
				this.powerupActivation = powerupActivation;
				powerupCombine = null;
				powerupScore = default(ActionScore);
			}

			public PotentialMove(ActionScore moveScore, PowerupCombines.PowerupCombine powerupCombine)
			{
				this.moveScore = moveScore;
				swapMatch = null;
				powerupActivation = null;
				this.powerupCombine = powerupCombine;
				powerupScore = default(ActionScore);
			}
		}

		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Comparison<PotentialMove> _003C_003E9__5_0;

			internal int _003CFindBestMove_003Eb__5_0(PotentialMove x, PotentialMove y)
			{
				ActionScore moveScore = y.moveScore;
				ActionScore moveScore2 = x.moveScore;
				ActionScore powerupScore = y.powerupScore;
				ActionScore powerupScore2 = x.powerupScore;
				int num = moveScore.goalsCount.CompareTo(moveScore2.goalsCount);
				if (num == 0)
				{
					num = moveScore.obstaclesDestroyed.CompareTo(moveScore2.obstaclesDestroyed);
				}
				if (num == 0)
				{
					num = moveScore.powerupsCreated.CompareTo(moveScore2.powerupsCreated);
				}
				if (num == 0)
				{
					num = powerupScore.goalsCount.CompareTo(powerupScore2.goalsCount);
				}
				if (num == 0)
				{
					num = powerupScore.obstaclesDestroyed.CompareTo(powerupScore2.obstaclesDestroyed);
				}
				return num;
			}
		}

		private Match3Game game;

		private List<PotentialMove> potentialMoves = new List<PotentialMove>();

		private int lastMoveCount;

		public void Init(Match3Game game)
		{
			this.game = game;
			lastMoveCount = 0;
		}

		public void FindBestMove()
		{
			potentialMoves.Clear();
			game.goals.FillSlotData(game);
			game.board.potentialMatches.FindPotentialMatches(game);
			game.board.powerupActivations.Fill(game);
			game.board.powerupCombines.Fill(game);
			PotentialMatches potentialMatches = game.board.potentialMatches;
			Match3Goals goals = game.goals;
			List<PotentialMatches.CompoundSlotsSet> matchesList = potentialMatches.matchesList;
			for (int i = 0; i < matchesList.Count; i++)
			{
				PotentialMatches.CompoundSlotsSet compoundSlotsSet = matchesList[i];
				ActionScore actionScore = compoundSlotsSet.GetActionScore(game, goals);
				ActionScore actionScore2 = default(ActionScore);
				if (actionScore.powerupsCreated > 0)
				{
					ChipType createdPowerup = compoundSlotsSet.createdPowerup;
					if (createdPowerup != ChipType.DiscoBall)
					{
						List<PowerupActivations.PowerupActivation> list = game.board.powerupActivations.CreatePotentialActivations(createdPowerup, game.GetSlot(compoundSlotsSet.positionOfSlotMissingForMatch));
						for (int j = 0; j < list.Count; j++)
						{
							ActionScore actionScore3 = list[j].GetActionScore(game, goals);
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
				potentialMoves.Add(new PotentialMove(actionScore, compoundSlotsSet, actionScore2));
			}
			List<PowerupActivations.PowerupActivation> powerups = game.board.powerupActivations.powerups;
			for (int k = 0; k < powerups.Count; k++)
			{
				PowerupActivations.PowerupActivation powerupActivation = powerups[k];
				ActionScore actionScore4 = powerupActivation.GetActionScore(game, goals);
				potentialMoves.Add(new PotentialMove(actionScore4, powerupActivation));
			}
			List<PowerupCombines.PowerupCombine> combines = game.board.powerupCombines.combines;
			for (int l = 0; l < combines.Count; l++)
			{
				PowerupCombines.PowerupCombine powerupCombine = combines[l];
				ActionScore actionScore5 = powerupCombine.GetActionScore(game, goals);
				potentialMoves.Add(new PotentialMove(actionScore5, powerupCombine));
			}
			potentialMoves.Sort(_003C_003Ec._003C_003E9._003CFindBestMove_003Eb__5_0);
			if (potentialMoves.Count == 0)
			{
				UnityEngine.Debug.Log("NO MOVES");
				return;
			}
			bool instant = true;
			PotentialMove potentialMove = potentialMoves[0];
			ActionScore moveScore = potentialMove.moveScore;
			if (moveScore.isZero && potentialMove.powerupScore.isZero)
			{
				potentialMove = potentialMoves[game.RandomRange(0, potentialMoves.Count)];
				game.board.gameStats.noUsefulMovesTurns++;
			}
			if (potentialMove.swapMatch != null)
			{
				PotentialMatches.CompoundSlotsSet swapMatch = potentialMove.swapMatch;
				game.TrySwitchSlots(swapMatch.positionOfSlotMissingForMatch, swapMatch.swipeSlot.position, instant);
			}
			else if (potentialMove.powerupCombine != null)
			{
				PowerupCombines.PowerupCombine powerupCombine2 = potentialMove.powerupCombine;
				game.TrySwitchSlots(powerupCombine2.powerupSlot.position, powerupCombine2.exchangeSlot.position, instant);
			}
			else if (potentialMove.powerupActivation != null)
			{
				PowerupActivations.PowerupActivation powerupActivation2 = potentialMove.powerupActivation;
				if (powerupActivation2.isTap)
				{
					game.TapOnSlot(powerupActivation2.powerupSlot.position);
				}
				else
				{
					game.TrySwitchSlots(powerupActivation2.powerupSlot.position, powerupActivation2.exchangeSlot.position, instant);
				}
			}
			bool isAIDebug = game.initParams.isAIDebug;
			int lastMoveCount2 = lastMoveCount;
			int userMovesCount = game.board.userMovesCount;
			lastMoveCount++;
			if (!isAIDebug)
			{
				return;
			}
			UnityEngine.Debug.Log("MOVE " + game.board.userMovesCount);
			for (int m = 0; m < potentialMoves.Count; m++)
			{
				PotentialMove potentialMove2 = potentialMoves[m];
				if (potentialMove2.swapMatch != null)
				{
					UnityEngine.Debug.LogFormat("{0} Score: {1} {2} -> {3}", m, potentialMove2.moveScore.ToDebugString(), potentialMove2.swapMatch.swipeSlot.position, potentialMove2.swapMatch.positionOfSlotMissingForMatch);
				}
				else if (potentialMove2.powerupCombine != null)
				{
					UnityEngine.Debug.LogFormat("{0} Score: {1} {2} -> {3}", m, potentialMove2.moveScore.ToDebugString(), potentialMove2.powerupCombine.exchangeSlot.position, potentialMove2.powerupCombine.powerupSlot.position);
				}
				else if (potentialMove2.powerupActivation != null)
				{
					if (potentialMove2.powerupActivation.isTap)
					{
						UnityEngine.Debug.LogFormat("{0} Score: {1} {2} -> TAP", m, potentialMove2.moveScore.ToDebugString(), potentialMove2.powerupActivation.powerupSlot.position);
					}
					else
					{
						UnityEngine.Debug.LogFormat("{0} Score: {1} {2} -> {3}", m, potentialMove2.moveScore.ToDebugString(), potentialMove2.powerupActivation.exchangeSlot.position, potentialMove2.powerupActivation.powerupSlot.position);
					}
				}
			}
			UnityEngine.Debug.LogError("DEBUG END " + game.board.userMovesCount);
		}
	}
}
