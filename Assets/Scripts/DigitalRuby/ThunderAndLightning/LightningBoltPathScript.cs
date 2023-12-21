using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningBoltPathScript : LightningBoltPathScriptBase
	{
		public float Speed = 1f;

		public RangeOfFloats SpeedIntervalRange = new RangeOfFloats
		{
			Minimum = 1f,
			Maximum = 1f
		};

		public bool Repeat = true;

		private float nextInterval = 1f;

		private int nextIndex;

		private Vector3? lastPoint;

		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			Vector3? vector = null;
			List<GameObject> currentPathObjects = GetCurrentPathObjects();
			if (currentPathObjects.Count < 2)
			{
				return;
			}
			if (nextIndex >= currentPathObjects.Count)
			{
				if (!Repeat)
				{
					return;
				}
				if (currentPathObjects[currentPathObjects.Count - 1] == currentPathObjects[0])
				{
					nextIndex = 1;
				}
				else
				{
					nextIndex = 0;
					lastPoint = null;
				}
			}
			try
			{
				if (!lastPoint.HasValue)
				{
					lastPoint = currentPathObjects[nextIndex++].transform.position;
				}
				vector = currentPathObjects[nextIndex].transform.position;
				if (lastPoint.HasValue && vector.HasValue)
				{
					parameters.Start = lastPoint.Value;
					parameters.End = vector.Value;
					base.CreateLightningBolt(parameters);
					if ((nextInterval -= Speed) <= 0f)
					{
						float num = UnityEngine.Random.Range(SpeedIntervalRange.Minimum, SpeedIntervalRange.Maximum);
						nextInterval = num + nextInterval;
						lastPoint = vector;
						nextIndex++;
					}
				}
			}
			catch (NullReferenceException)
			{
			}
		}

		public void Reset()
		{
			lastPoint = null;
			nextIndex = 0;
			nextInterval = 1f;
		}
	}
}
