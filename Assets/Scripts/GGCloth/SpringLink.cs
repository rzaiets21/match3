using UnityEngine;

namespace GGCloth
{
	public class SpringLink : Constraint
	{
		public PointMass p1;

		public PointMass p2;

		public float stiffness = 0.5f;

		public float restingDistance = 1f;

		public void InitWithPointsAtRest(PointMass p1, PointMass p2, float stiffness)
		{
			this.p1 = p1;
			this.p2 = p2;
			restingDistance = Vector3.Distance(p1.currentPosition, p2.currentPosition);
			this.stiffness = stiffness;
		}

		public override void Solve(PointWorld fieldWorld)
		{
			Vector3 a = p2.currentPosition - p1.currentPosition;
			float magnitude = a.magnitude;
			float num = (magnitude - restingDistance) / magnitude;
			Vector3 vector = stiffness * num * 0.5f * a;
			p1.currentPosition += vector;
			p2.currentPosition -= vector;
		}
	}
}
