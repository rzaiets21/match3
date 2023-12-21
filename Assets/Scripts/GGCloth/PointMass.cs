using UnityEngine;

namespace GGCloth
{
	public class PointMass
	{
		public Vector3 currentPosition;

		public Vector3 previosPosition;

		public float mass;

		public float dampingFactor;

		public Vector3 acceleration;

		public void SetRestingPostion(Vector3 position)
		{
			currentPosition = (previosPosition = position);
		}

		public void VerletIntegrate(float deltaTimeMilliseconds, PointWorld world)
		{
			Vector3 a = currentPosition - previosPosition;
			previosPosition = currentPosition;
			Vector3 a2 = acceleration + world.gravityMS;
			currentPosition += a * (1f - dampingFactor) + a2 * (deltaTimeMilliseconds * deltaTimeMilliseconds);
		}
	}
}
