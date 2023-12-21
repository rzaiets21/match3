using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBoltPrefabScript : LightningBoltPrefabScriptBase
	{
		public GameObject Source;

		public GameObject Destination;

		public Vector3 StartVariance;

		public Vector3 EndVariance;

		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			parameters.Start = ((Source == null) ? parameters.Start : Source.transform.position);
			parameters.End = ((Destination == null) ? parameters.End : Destination.transform.position);
			parameters.StartVariance = StartVariance;
			parameters.EndVariance = EndVariance;
			base.CreateLightningBolt(parameters);
			transform.localPosition = new Vector3(0, 0, -5);
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
			
			transform.localPosition = new Vector3(0, 0, -5);
		}
	}
}
