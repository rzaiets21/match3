using UnityEngine;

namespace GGMatch3
{
	public class ChipPhysics
	{
		public SlotComponent chip;

		public Vector3 movingFrom;

		public Vector3 movingTo;

		public Vector3 velocity;

		public Vector3 currentPosition;

		public bool isActive;

		public bool isFalling;

		public long frameIndexWhenDeactivated;

		public float lastMoveTime = -2f;

		public long lastMoveFrameIndex = -2L;

		public float speed
		{
			get
			{
				return Mathf.Max(velocity.x, velocity.y);
			}
			set
			{
				velocity.x = (velocity.y = value);
			}
		}

		public bool isArrivedToDestination => currentPosition == movingTo;

		public void StartMove(Vector3 movingFrom, Vector3 movingTo, long frameIndex, float currentTime)
		{
			isActive = true;
			isFalling = true;
			this.movingTo = movingTo;
			this.movingFrom = movingFrom;
			currentPosition = movingFrom;
			lastMoveFrameIndex = frameIndex;
			lastMoveTime = currentTime;
		}

		public UpdateResult OnUpdate(UpdateParams updateParams)
		{
			float deltaTime = updateParams.deltaTime;
			UpdateResult result = default(UpdateResult);
			if (!isActive)
			{
				return result;
			}
			if (chip.slot == null)
			{
				return result;
			}
			if (isArrivedToDestination)
			{
				return result;
			}
			result.wasTraveling = true;
			Match3Game game = chip.slot.game;
			lastMoveTime = game.board.currentTime;
			lastMoveFrameIndex = game.board.currentFrameIndex;
			GravitySettings gravitySettings = chip.slot.game.settings.gravitySettings;
			IntVector2 position = chip.slot.position;
			Vector3 currentPosition2 = currentPosition;
			Vector2 normalizedPositionWithinBoard = chip.slot.normalizedPositionWithinBoard;
			float num = gravitySettings.gravityRange.Lerp(normalizedPositionWithinBoard.y);
			float a = gravitySettings.minVelocityRange.Lerp(normalizedPositionWithinBoard.y);
			velocity.x = Mathf.Max(a, velocity.x);
			velocity.y = Mathf.Max(a, velocity.y);
			if (updateParams.udpateIteration == 0f)
			{
				if (currentPosition.x != movingTo.x)
				{
					velocity.x += num * deltaTime;
				}
				if (currentPosition.y != movingTo.y)
				{
					velocity.y += num * deltaTime;
				}
			}
			float num2 = Mathf.Min(b: Mathf.Max(velocity.x, velocity.y), a: gravitySettings.maxVelocity);
			float num3 = Mathf.Max(Mathf.Abs(currentPosition.x - movingTo.x), Mathf.Abs(currentPosition.y - movingTo.y));
			if (num2 * deltaTime > num3)
			{
				result.leftOverDeltaTime = deltaTime - num3 / num2;
			}
			currentPosition.x = Mathf.MoveTowards(currentPosition.x, movingTo.x, num2 * deltaTime);
			currentPosition.y = Mathf.MoveTowards(currentPosition.y, movingTo.y, num2 * deltaTime);
			currentPosition.z = movingTo.z;
			TransformBehaviour componentBehaviour = chip.GetComponentBehaviour<TransformBehaviour>();
			if (componentBehaviour != null)
			{
				componentBehaviour.localPosition = currentPosition;
			}
			if (isArrivedToDestination)
			{
				isActive = false;
			}
			return result;
		}
	}
}
