using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class MovingElement : SlotComponent
	{
		private ChipPhysics physics = new ChipPhysics();

		private TeleporterAnimation teleportAnimation = new TeleporterAnimation();

		public override int sortingOrder => 10;

		public override bool isMovingWithConveyor => true;

		public override bool isMovedByGravity => true;

		public override bool isPreventingOtherChipsToFallIntoSlot => true;

		public override long lastMoveFrameIndex => Math.Max(physics.lastMoveFrameIndex, teleportAnimation.lastMoveFrame);

		public override float lastMoveTime => Mathf.Max(physics.lastMoveTime, teleportAnimation.lastMoveTime);

		public override bool isSlotSwapSuspended
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isSlotMatchingSuspended
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isMoving
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isMoveByConveyorSuspended
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override bool isSlotGravitySuspended
		{
			get
			{
				if (!physics.isActive)
				{
					return teleportAnimation.isActive;
				}
				return true;
			}
		}

		public override void OnCreatedBySlot(Slot toSlot)
		{
			Match3Board board = toSlot.game.board;
			IntVector2 intVector = new IntVector2(0, 0);
			List<IntVector2> forceDirections = toSlot.gravity.forceDirections;
			for (int i = 0; i < forceDirections.Count; i++)
			{
				IntVector2 b = forceDirections[i];
				intVector += b;
			}
			physics.StartMove(toSlot.game.LocalPositionOfCenter(toSlot.position - intVector), toSlot.localPositionOfCenter, board.currentFrameIndex, board.currentTime);
			slot.OnSlotComponentMadeATransformChange(this);
		}

		public override void OnMovedBySlotGravity(Slot fromSlot, Slot toSlot, MoveContentsToSlotParams moveParams)
		{
			base.OnMovedBySlotGravity(fromSlot, toSlot, moveParams);
			long currentFrameIndex = fromSlot.game.board.currentFrameIndex;
			Match3Board board = fromSlot.game.board;
			float num = 0f;
			long num2 = currentFrameIndex - teleportAnimation.lastMoveFrame;
			bool flag = true;
			if (num2 <= 1)
			{
				num = Mathf.Min(teleportAnimation.currentSpeed, Match3Settings.instance.pipeSettings.maxContinueVelocity);
				flag = false;
			}
			if (currentFrameIndex - physics.lastMoveFrameIndex <= 1)
			{
				num = physics.speed;
				flag = false;
			}
			if (moveParams.isFromPortal)
			{
				IntVector2 intVector = toSlot.gravity.forceDirections[0];
				TeleporterAnimation.MoveParams mp = default(TeleporterAnimation.MoveParams);
				mp.chip = this;
				mp.game = lastConnectedSlot.game;
				mp.positionToMoveFrom = fromSlot.position;
				mp.directionToMoveFrom = fromSlot.gravity.forceDirections[0];
				mp.entrancePipe = fromSlot.entrancePipe;
				mp.exitPipe = toSlot.exitPipe;
				mp.positionToMoveTo = toSlot.position;
				mp.directionToMoveTo = toSlot.gravity.forceDirections[0];
				mp.initialSpeed = num;
				mp.currentFrameIndex = currentFrameIndex;
				mp.currentTime = board.currentTime;
				teleportAnimation.StartMove(mp);
			}
			else
			{
				physics.speed = num;
				physics.StartMove(fromSlot.localPositionOfCenter, toSlot.localPositionOfCenter, currentFrameIndex, board.currentTime);
			}
			if (flag)
			{
				SlotStartMoveParams startMoveParams = default(SlotStartMoveParams);
				startMoveParams.fromSlot = fromSlot;
				startMoveParams.toSlot = toSlot;
				startMoveParams.slotComponent = this;
				slot.OnSlotComponentMadeAStartMove(startMoveParams);
			}
			slot.OnSlotComponentMadeATransformChange(this);
		}

		public override void OnUpdate(float deltaTime)
		{
			if (slot == null)
			{
				return;
			}
			TransformBehaviour componentBehaviour = GetComponentBehaviour<TransformBehaviour>();
			if (componentBehaviour != null)
			{
				componentBehaviour.slotOffsetPosition = slot.offsetPosition;
				Vector3 vector = componentBehaviour.slotLocalScale = slot.offsetScale;
			}
			UpdateParams updateParams = default(UpdateParams);
			updateParams.deltaTime = deltaTime;
			while (true)
			{
				UpdateResult updateResult = default(UpdateResult);
				if (teleportAnimation.isActive)
				{
					updateParams.udpateIteration = 0f;
					do
					{
						updateResult = teleportAnimation.OnUpdate(updateParams);
						updateParams.udpateIteration += 1f;
						if (updateResult.wasTraveling && !teleportAnimation.isActive)
						{
							slot.ApplySlotGravity();
						}
					}
					while (!(updateResult.leftOverDeltaTime <= 0f) && teleportAnimation.isActive);
					if (updateResult.leftOverDeltaTime <= 0f)
					{
						break;
					}
					updateParams.deltaTime = updateResult.leftOverDeltaTime;
					updateParams.udpateIteration = 0f;
				}
				if (slot.isChipGravitySuspended)
				{
					return;
				}
				updateResult = physics.OnUpdate(updateParams);
				if (!physics.isActive && updateResult.wasTraveling)
				{
					slot.ApplySlotGravity();
				}
				if (updateResult.leftOverDeltaTime <= 0f)
				{
					break;
				}
				updateParams.udpateIteration += 1f;
				updateParams.deltaTime = updateResult.leftOverDeltaTime;
			}
			slot.OnSlotComponentMadeATransformChange(this);
			if (!slot.isMovingElementRequired)
			{
				RemoveFromSlot();
				RemoveFromGame();
			}
		}

		public override void OnAddedToSlot(Slot slot)
		{
			base.OnAddedToSlot(slot);
			physics.chip = this;
		}
	}
}
