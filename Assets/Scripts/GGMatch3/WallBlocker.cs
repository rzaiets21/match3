namespace GGMatch3
{
	public class WallBlocker : SlotComponent
	{
		private IntVector2 blockDirection;

		public override bool isMovingWithConveyor => true;

		public void Init(IntVector2 direction)
		{
			blockDirection = direction;
		}

		public override bool isBlockingDirection(IntVector2 direction)
		{
			if (slot == null)
			{
				return false;
			}
			if (blockDirection.y != 0 && direction.y == blockDirection.y)
			{
				return true;
			}
			if (blockDirection.x != 0 && direction.x == blockDirection.x)
			{
				return true;
			}
			return false;
		}
	}
}
