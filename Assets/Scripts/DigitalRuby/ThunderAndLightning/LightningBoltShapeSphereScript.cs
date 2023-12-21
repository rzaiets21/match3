using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBoltShapeSphereScript : LightningBoltPrefabScriptBase
	{
		public float InnerRadius = 0.1f;

		public float Radius = 4f;

		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			Vector3 start = Random.insideUnitSphere * InnerRadius;
			Vector3 end = Random.onUnitSphere * Radius;
			parameters.Start = start;
			parameters.End = end;
			base.CreateLightningBolt(parameters);
		}
	}
}
