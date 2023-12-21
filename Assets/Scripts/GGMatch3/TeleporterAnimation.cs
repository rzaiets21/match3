using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class TeleporterAnimation
	{
		public struct MoveParams
		{
			public Match3Game game;

			public SlotComponent chip;

			public float initialSpeed;

			public PipeBehaviour entrancePipe;

			public IntVector2 positionToMoveFrom;

			public IntVector2 directionToMoveFrom;

			public PipeBehaviour exitPipe;

			public IntVector2 positionToMoveTo;

			public IntVector2 directionToMoveTo;

			public long currentFrameIndex;

			public float currentTime;
		}

		public struct LinearMove
		{
			public Vector3 startPosition;

			public Vector3 endPosition;

			public bool isStarted;

			public bool resetVisuallyWhenStart;

			public PipeBehaviour pipe;

			public bool isEntrance;
		}

		public List<LinearMove> moves = new List<LinearMove>();

		private Vector3 currentPosition;

		public float currentSpeed;

		private int activeMoveIndex;

		public float lastMoveTime = -2f;

		public long lastMoveFrame = -2L;

		private MoveParams mp;

		public bool isActive => activeMoveIndex < moves.Count;

		public void StartMove(MoveParams mp)
		{
			this.mp = mp;
			Match3Game game = mp.game;
			lastMoveFrame = mp.currentFrameIndex;
			lastMoveTime = mp.currentTime;
			moves.Clear();
			LinearMove item = default(LinearMove);
			item.startPosition = game.LocalPositionOfCenter(mp.positionToMoveFrom);
			int num = game.SlotsDistanceToEndOfBoard(mp.positionToMoveFrom, mp.directionToMoveFrom) + 2;
			num = 1;
			item.endPosition = game.LocalPositionOfCenter(mp.positionToMoveFrom + mp.directionToMoveFrom * num);
			item.pipe = mp.entrancePipe;
			item.isEntrance = true;
			moves.Add(item);
			LinearMove item2 = default(LinearMove);
			int num2 = game.SlotsDistanceToEndOfBoard(mp.positionToMoveTo, -mp.directionToMoveTo) + 2;
			num2 = 1;
			item2.startPosition = game.LocalPositionOfCenter(mp.positionToMoveTo - mp.directionToMoveTo * num2);
			item2.endPosition = game.LocalPositionOfCenter(mp.positionToMoveTo);
			item2.resetVisuallyWhenStart = true;
			item2.pipe = mp.exitPipe;
			moves.Add(item2);
			currentSpeed = mp.initialSpeed;
			activeMoveIndex = 0;
		}

		private void SetPipeScale(PipeBehaviour pipe, float scale)
		{
			if (!(pipe == null))
			{
				pipe.SetScale(scale);
			}
		}

		public UpdateResult OnUpdate(UpdateParams updateParams)
		{
			float deltaTime = updateParams.deltaTime;
			UpdateResult result = default(UpdateResult);
			if (!isActive)
			{
				return result;
			}
			SlotComponent chip = mp.chip;
			Match3Game game = mp.game;
			if (activeMoveIndex >= moves.Count)
			{
				return result;
			}
			if (game == null)
			{
				return result;
			}
			lastMoveTime = game.board.currentTime;
			lastMoveFrame = game.board.currentFrameIndex;
			result.wasTraveling = true;
			LinearMove linearMove = moves[activeMoveIndex];
			PipeSettings pipeSettings = game.settings.pipeSettings;
			if (!linearMove.isStarted)
			{
				linearMove.isStarted = true;
				SetPipeScale(linearMove.pipe, Match3Settings.instance.pipeSettings.pipeScale);
				currentPosition = linearMove.startPosition;
				if (linearMove.resetVisuallyWhenStart)
				{
					ChipBehaviour componentBehaviour = chip.GetComponentBehaviour<ChipBehaviour>();
					if (componentBehaviour != null)
					{
						componentBehaviour.localPosition = currentPosition;
						componentBehaviour.ResetCloth();
					}
				}
				if (linearMove.pipe != null && linearMove.isEntrance)
				{
					game.particles.CreateParticlesWorld(linearMove.pipe.exitTransform.position, Match3Particles.PositionType.PipeEnterParticle, ChipType.Chip, ItemColor.Unknown);
				}
			}
			float gravity = pipeSettings.gravity;
			float minVelocity = pipeSettings.minVelocity;
			if (updateParams.udpateIteration == 0f)
			{
				currentSpeed += gravity * deltaTime;
			}
			currentSpeed = Mathf.Max(minVelocity, currentSpeed);
			currentSpeed = Mathf.Min(pipeSettings.maxVelocity, currentSpeed);
			Vector3 endPosition = linearMove.endPosition;
			float num = Mathf.Max(Mathf.Abs(currentPosition.x - endPosition.x), Mathf.Abs(currentPosition.y - endPosition.y));
			if (currentSpeed * deltaTime > num)
			{
				result.leftOverDeltaTime = deltaTime - num / currentSpeed;
			}
			currentPosition.x = Mathf.MoveTowards(currentPosition.x, endPosition.x, currentSpeed * deltaTime);
			currentPosition.y = Mathf.MoveTowards(currentPosition.y, endPosition.y, currentSpeed * deltaTime);
			currentPosition.z = endPosition.z;
			bool num2 = currentPosition == endPosition;
			moves[activeMoveIndex] = linearMove;
			if (num2)
			{
				if (linearMove.pipe != null && !linearMove.isEntrance)
				{
					game.particles.CreateParticlesWorld(linearMove.pipe.exitTransform.position, Match3Particles.PositionType.PipeExitParticle, ChipType.Chip, ItemColor.Unknown);
				}
				SetPipeScale(linearMove.pipe, 1f);
				activeMoveIndex++;
			}
			float num3 = game.settings.pipeSettings.scale;
			float num4 = game.settings.pipeSettings.orthoScale;
			if (!isActive)
			{
				num3 = 1f;
				num4 = 1f;
			}
			TransformBehaviour componentBehaviour2 = chip.GetComponentBehaviour<TransformBehaviour>();
			if (componentBehaviour2 != null)
			{
				componentBehaviour2.localPosition = currentPosition;
				Vector3 vector = linearMove.endPosition - linearMove.startPosition;
				Vector3 one = Vector3.one;
				if (Mathf.Abs(vector.x) > 0f)
				{
					one.y = num3;
					one.x = num4;
				}
				else
				{
					one.x = num3;
					one.y = num4;
				}
				if (componentBehaviour2.scalerTransform != null)
				{
					componentBehaviour2.scalerTransform.localScale = one;
				}
			}
			return result;
		}
	}
}
