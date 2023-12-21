using UnityEngine;

namespace GGCloth
{
	public class PointPositionConstraint : Constraint
	{
		public enum Direction
		{
			X,
			Y,
			XY
		}

		public PointMass point;

		public PointMass pointToConstrainBy;

		public float minDistance = 0.1f;

		private int initialXSign;

		private int initialYSign;

		public Direction constrainDirection;

		public void Init(PointMass point, PointMass pointToConstrainBy, Direction constrainDirection)
		{
			this.point = point;
			this.pointToConstrainBy = pointToConstrainBy;
			this.constrainDirection = constrainDirection;
			initialXSign = (int)Mathf.Sign(point.currentPosition.x - pointToConstrainBy.currentPosition.x);
			initialYSign = (int)Mathf.Sign(point.currentPosition.y - pointToConstrainBy.currentPosition.y);
		}

		public override void Solve(PointWorld fieldWorld)
		{
			Vector3 currentPosition = point.currentPosition;
			Vector3 vector = currentPosition;
			if (constrainDirection == Direction.X || constrainDirection == Direction.XY)
			{
				if (initialXSign < 0)
				{
					vector.x = Mathf.Min(pointToConstrainBy.currentPosition.x - minDistance, point.currentPosition.x);
				}
				else
				{
					vector.x = Mathf.Max(pointToConstrainBy.currentPosition.x + minDistance, point.currentPosition.x);
				}
			}
			if (constrainDirection == Direction.Y || constrainDirection == Direction.XY)
			{
				if (initialYSign < 0)
				{
					vector.y = Mathf.Min(pointToConstrainBy.currentPosition.y - minDistance, point.currentPosition.y);
				}
				else
				{
					vector.y = Mathf.Max(pointToConstrainBy.currentPosition.y + minDistance, point.currentPosition.y);
				}
			}
			if (vector != currentPosition)
			{
				point.SetRestingPostion(vector);
			}
		}
	}
}
