using System.Collections.Generic;
using UnityEngine;

namespace GGCloth
{
	public class MultiPointAttachConstraint : Constraint
	{
		public class PointPosition
		{
			public PointMass point;

			public Vector3 localPosition;
		}

		public Vector3 centralPosition;

		public List<PointPosition> points = new List<PointPosition>();

		public void Init(Vector3 centralPosition)
		{
			points.Clear();
			this.centralPosition = centralPosition;
		}

		public void FixPoint(PointMass point)
		{
			PointPosition pointPosition = new PointPosition();
			pointPosition.point = point;
			pointPosition.localPosition = point.currentPosition - centralPosition;
			points.Add(pointPosition);
		}

		public override void Solve(PointWorld fieldWorld)
		{
			for (int i = 0; i < points.Count; i++)
			{
				PointPosition pointPosition = points[i];
				Vector3 restingPostion = centralPosition + pointPosition.localPosition;
				pointPosition.point.SetRestingPostion(restingPostion);
			}
		}
	}
}
