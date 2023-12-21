using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class ConveyorBeltBoardComponent : BoardComponent
	{
		[Serializable]
		public class Settings
		{
			public int slotsToExtendPastEndOfBoard = 2;

			public float delayBeforeMove;

			public float duration = 1f;

			public float pipeDuration = 1f;

			public AnimationCurve travelCurve;

			public AnimationCurve pipeTravelCurve;

			public float pipeScale = 0.95f;

			public float minTimeNotMoveBeforeCanStartConveyor = 0.4f;

			public Color colorWhenActive = Color.white;

			public Color colorWhenMoved = Color.white;

			public float colorChangeSpeed = 5f;

			public bool shouldJump;

			public float orthoDirectionUp;

			public Vector3 jumpScale;

			public SpriteSortingSettings sortingSettings;

			public AnimationCurve jumpUpCurve;

			public AnimationCurve jumpDistanceCurve;
		}

		public class LinearMove
		{
			public Vector3 startPosition;

			public Vector3 endPosition;

			public bool isStarted;

			public bool isEnded;

			public bool isJump;

			public bool resetVisuallyWhenStart;

			public PipeBehaviour pipe;

			public void Reset()
			{
				isStarted = false;
				isEnded = false;
			}

			public void OnStart(Movement m)
			{
				isStarted = true;
				if (!resetVisuallyWhenStart)
				{
					return;
				}
				for (int i = 0; i < m.moveComponents.Count; i++)
				{
					ChipBehaviour componentBehaviour = m.moveComponents[i].GetComponentBehaviour<ChipBehaviour>();
					if (!(componentBehaviour == null))
					{
						componentBehaviour.hasBounce = false;
					}
				}
			}

			public void OnEnd(Movement m)
			{
				isEnded = true;
				for (int i = 0; i < m.moveComponents.Count; i++)
				{
					ChipBehaviour componentBehaviour = m.moveComponents[i].GetComponentBehaviour<ChipBehaviour>();
					if (!(componentBehaviour == null))
					{
						componentBehaviour.hasBounce = true;
					}
				}
				if (isJump)
				{
					for (int j = 0; j < m.moveComponents.Count; j++)
					{
						TransformBehaviour componentBehaviour2 = m.moveComponents[j].GetComponentBehaviour<TransformBehaviour>();
						if (!(componentBehaviour2 == null))
						{
							componentBehaviour2.ResetSortingLayerSettings();
						}
					}
				}
				if (!(pipe == null))
				{
					pipe.SetScale(1f);
					pipe.SetOffsetPosition(Vector3.zero);
				}
			}
		}

		public class Movement
		{
			public Slot fromSlot;

			public Slot toSlot;

			public bool isEnded;

			public List<SlotComponent> moveComponents = new List<SlotComponent>();

			public List<LinearMove> linearMoves = new List<LinearMove>();

			public int currentLinearMoveIndex;

			public float durationScale = 1f;

			public void Reset()
			{
				currentLinearMoveIndex = 0;
				isEnded = false;
				for (int i = 0; i < linearMoves.Count; i++)
				{
					linearMoves[i].Reset();
				}
			}
		}

		private Match3Game game;

		private LevelDefinition.ConveyorBelt conveyorBeltDef;

		private ConveyorBeltBehaviour beh;

		private int movesCountWhenTookAction;

		private LockContainer lockContainer = new LockContainer();

		private Lock globalLock;

		private List<Movement> moveList = new List<Movement>();

		private float movingTime;

		private bool _003CisMoving_003Ek__BackingField;

		private Color colorWhenStartMove;

		private List<Slot> slotsToLock = new List<Slot>();

		private List<Slot> slotsToCheck = new List<Slot>();

		private List<Slot> allSlotsList = new List<Slot>();

		public Settings settings => Match3Settings.instance.conveyorBeltSettings;

		public bool isMoving
		{
			get
			{
				return _003CisMoving_003Ek__BackingField;
			}
			protected set
			{
				_003CisMoving_003Ek__BackingField = value;
			}
		}

		public int lastMoveConveyorTookAction => movesCountWhenTookAction;

		public bool needsToActivateConveyor => game.board.userMovesCount > movesCountWhenTookAction;

		private void StartMove()
		{
			colorWhenStartMove = beh.GetColor();
			List<Slot> slots = GetSlotsToLock();
			globalLock.LockSlots(slots);
			movingTime = 0f;
			isMoving = true;
			for (int i = 0; i < moveList.Count; i++)
			{
				Movement movement = moveList[i];
				movement.Reset();
				movement.moveComponents.Clear();
				for (int j = 0; j < movement.fromSlot.components.Count; j++)
				{
					SlotComponent slotComponent = movement.fromSlot.components[j];
					if (slotComponent.isMovingWithConveyor)
					{
						movement.moveComponents.Add(slotComponent);
					}
				}
				List<SlotComponent> moveComponents = movement.moveComponents;
				for (int k = 0; k < moveComponents.Count; k++)
				{
					moveComponents[k].RemoveFromSlot();
				}
			}
			for (int l = 0; l < moveList.Count; l++)
			{
				Movement movement2 = moveList[l];
				List<SlotComponent> moveComponents2 = movement2.moveComponents;
				for (int m = 0; m < moveComponents2.Count; m++)
				{
					SlotComponent c = moveComponents2[m];
					movement2.toSlot.AddComponent(c);
				}
			}
		}

		private void SetPipeScale(PipeBehaviour pipe, float scale)
		{
			if (!(pipe == null))
			{
				pipe.SetScale(scale);
			}
		}

		public List<Slot> GetSlotsToLock()
		{
			slotsToLock.Clear();
			slotsToLock.AddRange(allSlotsList);
			return slotsToLock;
		}

		public List<Slot> GetSlotsToCheck()
		{
			slotsToCheck.Clear();
			slotsToCheck.AddRange(allSlotsList);
			return slotsToCheck;
		}

		private void UpdateMove(float deltaTime)
		{
			Settings settings = this.settings;
			float duration = settings.duration;
			float delayBeforeMove = settings.delayBeforeMove;
			movingTime += deltaTime;
			if (movingTime < delayBeforeMove)
			{
				return;
			}
			float value = movingTime - delayBeforeMove;
			float num = Mathf.InverseLerp(0f, duration, value);
			float num2 = settings.travelCurve.Evaluate(num);
			beh.SetTile(num2);
			bool flag = false;
			for (int i = 0; i < moveList.Count; i++)
			{
				Movement movement = moveList[i];
				List<SlotComponent> moveComponents = movement.moveComponents;
				Vector3 localPositionOfCenter = movement.fromSlot.localPositionOfCenter;
				Vector3 localPositionOfCenter2 = movement.toSlot.localPositionOfCenter;
				Vector3 localScale = Vector3.one;
				Vector3 localPosition = Vector3.LerpUnclamped(localPositionOfCenter, localPositionOfCenter2, num2);
				bool flag2 = false;
				if (movement.linearMoves.Count > 0)
				{
					int currentLinearMoveIndex = movement.currentLinearMoveIndex;
					float num3 = Mathf.InverseLerp(0f, settings.pipeDuration * movement.durationScale, value);
					if (num3 < 1f)
					{
						flag = true;
						flag2 = true;
					}
					num3 = settings.pipeTravelCurve.Evaluate(num3);
					float num4 = Mathf.Lerp(0f, movement.linearMoves.Count, num3);
					int num5 = movement.currentLinearMoveIndex = Mathf.FloorToInt(Mathf.Clamp(num4, 0f, movement.linearMoves.Count - 1));
					for (int j = 0; j < num5; j++)
					{
						LinearMove linearMove = movement.linearMoves[j];
						if (linearMove.isStarted && !linearMove.isEnded)
						{
							linearMove.OnEnd(movement);
						}
					}
					float num6 = Mathf.Clamp01(num4 - (float)num5);
					PipeSettings pipeSettings = Match3Settings.instance.pipeSettings;
					LinearMove linearMove2 = movement.linearMoves[num5];
					if (!linearMove2.isStarted)
					{
						linearMove2.OnStart(movement);
						if (linearMove2.isJump)
						{
							for (int k = 0; k < moveComponents.Count; k++)
							{
								TransformBehaviour componentBehaviour = moveComponents[k].GetComponentBehaviour<TransformBehaviour>();
								if (!(componentBehaviour == null))
								{
									componentBehaviour.SaveSortingLayerSettings();
									componentBehaviour.SetSortingLayer(settings.sortingSettings);
								}
							}
						}
						SetPipeScale(linearMove2.pipe, settings.pipeScale);
						if (linearMove2.pipe != null)
						{
							linearMove2.pipe.SetScale(Match3Settings.instance.pipeSettings.pipeScale);
						}
						if (linearMove2.pipe == beh.entrancePipe)
						{
							game.particles.CreateParticlesWorld(linearMove2.pipe.exitTransform.position, Match3Particles.PositionType.PipeEnterParticle, ChipType.Chip, ItemColor.Unknown);
						}
						if (linearMove2.pipe == beh.exitPipe)
						{
							game.particles.CreateParticlesWorld(linearMove2.pipe.exitTransform.position, Match3Particles.PositionType.PipeExitParticle, ChipType.Chip, ItemColor.Unknown);
						}
					}
					localPosition = Vector3.LerpUnclamped(linearMove2.startPosition, linearMove2.endPosition, num6);
					if (linearMove2.pipe != null)
					{
						float t = pipeSettings.offsetCurve.Evaluate(Mathf.PingPong(num6, 0.5f));
						linearMove2.pipe.SetOffsetPosition(Vector3.Lerp(Vector3.zero, pipeSettings.offsetPosition, t));
					}
					Vector3 vector = linearMove2.endPosition - linearMove2.startPosition;
					float scale = Match3Settings.instance.pipeSettings.scale;
					float scale2 = Match3Settings.instance.pipeSettings.scale;
					if (Mathf.Abs(vector.x) > 0f)
					{
						localScale.y = scale;
						localScale.x = scale2;
					}
					else
					{
						localScale.y = scale2;
						localScale.x = scale;
					}
					if (linearMove2.isJump)
					{
						localPosition = Vector3.LerpUnclamped(linearMove2.startPosition, linearMove2.endPosition, settings.jumpDistanceCurve.Evaluate(num6));
						float time = Mathf.PingPong(num6, 0.5f);
						time = settings.jumpUpCurve.Evaluate(time);
						localScale = Vector3.Lerp(Vector3.one, settings.jumpScale, time);
						localPosition += Vector3.Cross(vector, Vector3.forward).normalized * Mathf.Lerp(0f, settings.orthoDirectionUp, time);
					}
				}
				else if (num < 1f)
				{
					flag = true;
					flag2 = true;
				}
				if (movement.isEnded)
				{
					continue;
				}
				for (int l = 0; l < moveComponents.Count; l++)
				{
					TransformBehaviour componentBehaviour2 = moveComponents[l].GetComponentBehaviour<TransformBehaviour>();
					if (!(componentBehaviour2 == null))
					{
						componentBehaviour2.localPosition = localPosition;
						componentBehaviour2.localScale = localScale;
					}
				}
				if (flag2 || movement.isEnded)
				{
					continue;
				}
				movement.isEnded = true;
				if (movement.linearMoves.Count > 0)
				{
					for (int m = 0; m < movement.linearMoves.Count; m++)
					{
						LinearMove linearMove3 = movement.linearMoves[m];
						if (!linearMove3.isEnded)
						{
							linearMove3.isEnded = true;
							linearMove3.OnEnd(movement);
							bool flag3 = linearMove3.pipe == beh.exitPipe;
						}
					}
				}
				globalLock.Unlock(movement.toSlot);
			}
			if (!conveyorBeltDef.isLoop)
			{
				Mathf.Max(duration, settings.pipeDuration);
			}
			Color color = Color.Lerp(colorWhenStartMove, settings.colorWhenMoved, num);
			beh.SetColor(color);
			if (flag)
			{
				return;
			}
			beh.SetColor(settings.colorWhenMoved);
			isMoving = false;
			GetSlotsToLock();
			globalLock.UnlockAll();
			for (int n = 0; n < moveList.Count; n++)
			{
				Movement movement2 = moveList[n];
				for (int num7 = 0; num7 < movement2.moveComponents.Count; num7++)
				{
					TransformBehaviour componentBehaviour3 = movement2.moveComponents[num7].GetComponentBehaviour<TransformBehaviour>();
					if (!(componentBehaviour3 == null))
					{
						componentBehaviour3.localScale = Vector3.one;
					}
				}
				for (int num8 = 0; num8 < movement2.linearMoves.Count; num8++)
				{
					LinearMove linearMove4 = movement2.linearMoves[num8];
					if (linearMove4.isStarted && !linearMove4.isEnded)
					{
						linearMove4.OnEnd(movement2);
						if (linearMove4.pipe == beh.exitPipe)
						{
							game.particles.CreateParticlesWorld(linearMove4.pipe.exitTransform.position, Match3Particles.PositionType.PipeExitParticle, ChipType.Chip, ItemColor.Unknown);
						}
					}
				}
			}
			game.ApplySlotGravityForAllSlots();
		}

		public void Init(Match3Game game, LevelDefinition.ConveyorBelt conveyorBeltDef, ConveyorBeltBehaviour beh)
		{
			this.game = game;
			this.conveyorBeltDef = conveyorBeltDef;
			this.beh = beh;
			List<LevelDefinition.ConveyorBeltLinearSegment> segmentList = conveyorBeltDef.segmentList;
			for (int i = 0; i < segmentList.Count; i++)
			{
				List<LevelDefinition.SlotDefinition> slotList = segmentList[i].slotList;
				for (int j = 0; j < slotList.Count; j++)
				{
					LevelDefinition.SlotDefinition slotDefinition = slotList[j];
					Slot slot = game.GetSlot(slotDefinition.position);
					allSlotsList.Add(slot);
				}
			}
			globalLock = lockContainer.NewLock();
			globalLock.isAvailableForDiscoBombSuspended = true;
			globalLock.isSlotMatchingSuspended = true;
			globalLock.isChipGeneratorSuspended = true;
			globalLock.isDestroySuspended = true;
			globalLock.isSlotGravitySuspended = true;
			for (int k = 0; k < allSlotsList.Count; k++)
			{
				Slot fromSlot = allSlotsList[k];
				Slot toSlot = allSlotsList[(k + 1) % allSlotsList.Count];
				Movement movement = new Movement();
				movement.fromSlot = fromSlot;
				movement.toSlot = toSlot;
				moveList.Add(movement);
				int count = allSlotsList.Count;
			}
			if (!conveyorBeltDef.isLoop)
			{
				Movement movement2 = moveList[moveList.Count - 1];
				if (settings.shouldJump)
				{
					LinearMove linearMove = new LinearMove();
					linearMove.startPosition = game.LocalPositionOfCenter(movement2.fromSlot.position);
					linearMove.isJump = true;
					linearMove.pipe = null;
					linearMove.endPosition = movement2.toSlot.localPositionOfCenter;
					movement2.linearMoves.Add(linearMove);
					movement2.durationScale = 3f;
				}
				else
				{
					LinearMove linearMove2 = new LinearMove();
					linearMove2.startPosition = movement2.fromSlot.localPositionOfCenter;
					IntVector2 direction = conveyorBeltDef.lastSegment.direction;
					linearMove2.endPosition = game.LocalPositionOfCenter(movement2.fromSlot.position + direction);
					linearMove2.pipe = null;
					linearMove2.resetVisuallyWhenStart = true;
					movement2.linearMoves.Add(linearMove2);
					linearMove2 = new LinearMove();
					direction = conveyorBeltDef.lastSegment.direction;
					int num = game.SlotsDistanceToEndOfBoard(movement2.fromSlot.position, direction) + settings.slotsToExtendPastEndOfBoard;
					num = 1;
					linearMove2.startPosition = game.LocalPositionOfCenter(movement2.fromSlot.position + direction);
					linearMove2.endPosition = game.LocalPositionOfCenter(movement2.fromSlot.position + direction * num);
					linearMove2.pipe = beh.entrancePipe;
					movement2.linearMoves.Add(linearMove2);
					linearMove2 = new LinearMove();
					IntVector2 direction2 = conveyorBeltDef.firstSegment.direction;
					num = game.SlotsDistanceToEndOfBoard(movement2.toSlot.position, -direction2) + settings.slotsToExtendPastEndOfBoard;
					num = 1;
					linearMove2.startPosition = game.LocalPositionOfCenter(movement2.toSlot.position - direction2 * num);
					linearMove2.endPosition = movement2.toSlot.localPositionOfCenter;
					linearMove2.resetVisuallyWhenStart = true;
					linearMove2.pipe = beh.exitPipe;
					movement2.linearMoves.Add(linearMove2);
					movement2.durationScale = 2f;
				}
				beh.SetColor(settings.colorWhenMoved);
			}
		}

		public bool IsMoveConveyorSuspended()
		{
			Slot[] slot3 = game.board.slots;
			float minTimeNotMoveBeforeCanStartConveyor = settings.minTimeNotMoveBeforeCanStartConveyor;
			if (game.input.isActive)
			{
				return true;
			}
			List<Slot> list = GetSlotsToCheck();
			FindMatches findMatches = game.board.findMatches;
			for (int i = 0; i < list.Count; i++)
			{
				Slot slot = list[i];
				if (findMatches.matches.GetIsland(slot.position) != null)
				{
					return true;
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Slot slot2 = list[j];
				if (slot2 == null)
				{
					continue;
				}
				if (slot2.isMoveByConveyorSuspended)
				{
					return true;
				}
				bool isEmpty;
				int num;
				if (slot2.isReacheableByGeneratorOrChip)
				{
					isEmpty = slot2.isEmpty;
				}
				else
					num = 0;
				if (slot2.isReacheableByGeneratorOrChip && slot2.isEmpty)
				{
					return true;
				}
				List<SlotComponent> components = slot2.components;
				for (int k = 0; k < components.Count; k++)
				{
					SlotComponent slotComponent = components[k];
					if (game.board.currentTime - slotComponent.lastMoveTime < minTimeNotMoveBeforeCanStartConveyor)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (isMoving)
			{
				UpdateMove(deltaTime);
				return;
			}
			lockContainer.UnlockAll();
			if (!game.isConveyorSuspended && game.board.userMovesCount > movesCountWhenTookAction)
			{
				Color color = Color.Lerp(beh.GetColor(), settings.colorWhenActive, settings.colorChangeSpeed * deltaTime);
				beh.SetColor(color);
				if (game.board.actionManager.ActionCount <= 0 && !game.board.isAnyConveyorMoveSuspended)
				{
					movesCountWhenTookAction = game.board.userMovesCount;
					StartMove();
				}
			}
		}
	}
}
