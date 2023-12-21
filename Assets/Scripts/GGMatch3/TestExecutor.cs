using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace GGMatch3
{
	public class TestExecutor : Match3GameListener
	{
		public struct ExecuteArguments
		{
			public int repeatTimes;

			public LevelEditorVisualizer visualizer;
		}

		public struct ResultData
		{
			public int moveCount;

			public int count;
		}

		[Serializable]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static Comparison<ResultData> _003C_003E9__13_0;

			internal int _003CDoExecution_003Eb__13_0(ResultData x, ResultData y)
			{
				return x.moveCount.CompareTo(y.moveCount);
			}
		}

		private sealed class _003CDoExecution_003Ed__13 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public TestExecutor _003C_003E4__this;

			private LevelEditorVisualizer _003Cvisualizer_003E5__2;

			private DateTime _003CstartTime_003E5__3;

			private int _003CrepeatTimes_003E5__4;

			private int _003Ci_003E5__5;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			[DebuggerHidden]
			public _003CDoExecution_003Ed__13(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				int num = _003C_003E1__state;
				TestExecutor testExecutor = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					goto IL_0140;
				}
				_003C_003E1__state = -1;
				_003Cvisualizer_003E5__2 = testExecutor.arguments.visualizer;
				_003CstartTime_003E5__3 = DateTime.Now;
				_003CrepeatTimes_003E5__4 = testExecutor.arguments.repeatTimes;
				_003Ci_003E5__5 = 0;
				goto IL_0183;
				IL_0140:
				if (testExecutor.gameResults.Count <= _003Ci_003E5__5)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Ci_003E5__5++;
				goto IL_0183;
				IL_0183:
				if (_003Ci_003E5__5 < _003CrepeatTimes_003E5__4)
				{
					Match3GameParams match3GameParams = new Match3GameParams();
					match3GameParams.level = _003Cvisualizer_003E5__2.level;
					match3GameParams.listener = testExecutor;
					match3GameParams.isAIPlayer = true;
					match3GameParams.iterations = _003Cvisualizer_003E5__2.stepsPerFrame;
					match3GameParams.timeScale = 10000f;
					match3GameParams.isHudDissabled = _003Cvisualizer_003E5__2.isHudDissabled;
					match3GameParams.disableParticles = true;
					if (_003Cvisualizer_003E5__2.humanVisibleDebug)
					{
						match3GameParams.iterations = 1;
						match3GameParams.timeScale = 1f;
						match3GameParams.isHudDissabled = false;
						match3GameParams.isAIDebug = true;
					}
					match3GameParams.setRandomSeed = true;
					int randomSeed = (int)DateTime.Now.Ticks % 1000;
					if (_003Cvisualizer_003E5__2.setRandomSeed)
					{
						randomSeed = _003Cvisualizer_003E5__2.randomSeed;
					}
					match3GameParams.randomSeed = randomSeed;
					testExecutor.arguments.visualizer.PlayGame(match3GameParams);
					goto IL_0140;
				}
				testExecutor.arguments.visualizer.StopGame();
				for (int i = 0; i < testExecutor.gameResults.Count; i++)
				{
					GameResults.GameResult gameResult = testExecutor.gameResults[i];
					if (!testExecutor.countGamesPerMoves.ContainsKey(gameResult.numberOfMoves))
					{
						testExecutor.countGamesPerMoves.Add(gameResult.numberOfMoves, 0);
					}
					testExecutor.countGamesPerMoves[gameResult.numberOfMoves] = testExecutor.countGamesPerMoves[gameResult.numberOfMoves] + 1;
				}
				List<ResultData> list = new List<ResultData>();
				foreach (KeyValuePair<int, int> countGamesPerMove in testExecutor.countGamesPerMoves)
				{
					ResultData item = default(ResultData);
					item.moveCount = countGamesPerMove.Key;
					item.count = countGamesPerMove.Value;
					list.Add(item);
				}
				list.Sort(_003C_003Ec._003C_003E9._003CDoExecution_003Eb__13_0);
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < list.Count; j++)
				{
					ResultData resultData = list[j];
					stringBuilder.AppendLine(resultData.moveCount + "," + resultData.count);
				}
				UnityEngine.Debug.Log("DURATION " + (DateTime.Now - _003CstartTime_003E5__3).TotalSeconds + "sec");
				string text = stringBuilder.ToString();
				UnityEngine.Debug.Log(text);
				testExecutor.arguments.visualizer.resultString = text;
				testExecutor.arguments.visualizer.lastResult = testExecutor.results;
				testExecutor.Clear();
				return false;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}
		}

		private GameResults results = new GameResults();

		private Dictionary<int, int> countGamesPerMoves = new Dictionary<int, int>();

		private ExecuteArguments arguments;

		private IEnumerator executionEnum;

		private List<GameResults.GameResult> gameResults => results.gameResults;

		public void OnGameComplete(GameCompleteParams completeParams)
		{
			GameResults.GameResult gameResult = new GameResults.GameResult();
			gameResult.isComplete = true;
			Match3Game game = completeParams.game;
			gameResult.randomSeed = game.initParams.randomSeed;
			gameResult.numberOfMoves = completeParams.stageState.userMovesCount;
			gameResult.gameStats = game.board.gameStats;
			gameResults.Add(gameResult);
			NavigationManager instance = NavigationManager.instance;
			if (!(instance == null))
			{
				instance.Pop();
				string resultString = "COMPLETE: " + Mathf.RoundToInt((float)gameResults.Count / (float)arguments.repeatTimes * 100f) + "%";
				arguments.visualizer.resultString = resultString;
			}
		}

		public void OnGameStarted(GameStartedParams startedParams)
		{
		}

		private void Clear()
		{
			results = new GameResults();
			countGamesPerMoves.Clear();
		}

		public void StopTesting()
		{
			Clear();
			executionEnum = null;
		}

		public void StartTesting(ExecuteArguments arguments)
		{
			this.arguments = arguments;
			Clear();
			results.repeats = arguments.repeatTimes;
			results.levelName = arguments.visualizer.levelName;
			executionEnum = DoExecution();
		}

		private IEnumerator DoExecution()
		{
			return new _003CDoExecution_003Ed__13(0)
			{
				_003C_003E4__this = this
			};
		}

		public void Update()
		{
			if (executionEnum != null && !executionEnum.MoveNext())
			{
				executionEnum = null;
			}
		}
	}
}
