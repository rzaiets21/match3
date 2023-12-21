using System.Collections.Generic;
using UnityEngine;

namespace GGCloth
{
	public class PointWorld
	{
		private List<PointMass> points = new List<PointMass>();

		private List<Constraint> constraints = new List<Constraint>();

		public float fixedTimeStepMilliseconds = 10f;

		private float leftOverTimeMS;

		public int constraintRelaxationSteps = 1;

		public Vector3 gravityMS;

		public List<PointMass> Points => points;

		public void SetGravity(Vector3 gravity)
		{
			gravityMS = gravity / 1000000f;
		}

		public void Clear()
		{
			points.Clear();
			constraints.Clear();
		}

		public PointMass GetPoint(int index)
		{
			return points[index];
		}

		public void AddPoint(PointMass point)
		{
			points.Add(point);
		}

		public void Prepend(Constraint constraint)
		{
			constraints.Insert(0, constraint);
		}

		public void AddConstraint(Constraint constraint)
		{
			constraints.Add(constraint);
		}

		public void Step(float deltaTime)
		{
			float num = deltaTime * 1000f + leftOverTimeMS;
			int num2 = Mathf.FloorToInt(num / fixedTimeStepMilliseconds);
			leftOverTimeMS = num - (float)num2 * fixedTimeStepMilliseconds;
			for (int i = 0; i < num2; i++)
			{
				VerletIntegrate();
				SatisfyConstraints();
			}
		}

		private void VerletIntegrate()
		{
			for (int i = 0; i < points.Count; i++)
			{
				points[i].VerletIntegrate(fixedTimeStepMilliseconds, this);
			}
		}

		private void SatisfyConstraints()
		{
			for (int i = 0; i < constraintRelaxationSteps; i++)
			{
				for (int j = 0; j < constraints.Count; j++)
				{
					constraints[j].Solve(this);
				}
			}
		}
	}
}
