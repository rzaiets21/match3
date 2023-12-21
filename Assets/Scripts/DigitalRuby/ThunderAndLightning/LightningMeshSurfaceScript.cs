using System.Collections.Generic;
using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class LightningMeshSurfaceScript : LightningBoltPrefabScriptBase
	{
		public MeshFilter MeshFilter;

		public Collider MeshCollider;

		public RangeOfFloats MeshOffsetRange;

		public RangeOfIntegers PathLengthCount;

		public RangeOfFloats MinimumPathDistanceRange;

		public float MaximumPathDistance;

		private float maximumPathDistanceSquared;

		public bool Spline;

		public float DistancePerSegmentHint;

		private readonly List<Vector3> sourcePoints;

		private Mesh previousMesh;

		private MeshHelper meshHelper;

		private void CheckMesh()
		{
			if (MeshFilter == null || MeshFilter.sharedMesh == null)
			{
				meshHelper = null;
			}
			else if (MeshFilter.sharedMesh != previousMesh)
			{
				previousMesh = MeshFilter.sharedMesh;
				meshHelper = new MeshHelper(previousMesh);
			}
		}

		protected override LightningBoltParameters OnCreateParameters()
		{
			LightningBoltParameters lightningBoltParameters = base.OnCreateParameters();
			lightningBoltParameters.Generator = LightningGeneratorPath.PathGeneratorInstance;
			return lightningBoltParameters;
		}

		protected virtual void PopulateSourcePoints(List<Vector3> points)
		{
			if (meshHelper != null)
			{
				CreateRandomLightningPath(sourcePoints);
			}
		}

		public void CreateRandomLightningPath(List<Vector3> points)
		{
			if (meshHelper == null)
			{
				return;
			}
			RaycastHit hit = default(RaycastHit);
			maximumPathDistanceSquared = MaximumPathDistance * MaximumPathDistance;
			meshHelper.GenerateRandomPoint(ref hit, out int triangleIndex);
			hit.distance = Random.Range(MeshOffsetRange.Minimum, MeshOffsetRange.Maximum);
			Vector3 vector = hit.point + hit.normal * hit.distance;
			float num = Random.Range(MinimumPathDistanceRange.Minimum, MinimumPathDistanceRange.Maximum);
			num *= num;
			sourcePoints.Add(MeshFilter.transform.TransformPoint(vector));
			int num2 = (Random.Range(0, 1) == 1) ? 3 : (-3);
			int num3 = Random.Range(PathLengthCount.Minimum, PathLengthCount.Maximum);
			while (num3 != 0)
			{
				triangleIndex += num2;
				if (triangleIndex >= 0 && triangleIndex < meshHelper.Triangles.Length)
				{
					meshHelper.GetRaycastFromTriangleIndex(triangleIndex, ref hit);
					hit.distance = Random.Range(MeshOffsetRange.Minimum, MeshOffsetRange.Maximum);
					Vector3 vector2 = hit.point + hit.normal * hit.distance;
					float sqrMagnitude = (vector2 - vector).sqrMagnitude;
					if (sqrMagnitude > maximumPathDistanceSquared)
					{
						break;
					}
					if (sqrMagnitude >= num)
					{
						vector = vector2;
						sourcePoints.Add(MeshFilter.transform.TransformPoint(vector2));
						num3--;
						num = Random.Range(MinimumPathDistanceRange.Minimum, MinimumPathDistanceRange.Maximum);
						num *= num;
					}
				}
				else
				{
					num2 = -num2;
					triangleIndex += num2;
					num3--;
				}
			}
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			CheckMesh();
			base.Update();
		}

		public override void CreateLightningBolt(LightningBoltParameters parameters)
		{
			if (meshHelper == null)
			{
				return;
			}
			int num2 = Generations = (parameters.Generations = Mathf.Clamp(Generations, 1, 5));
			sourcePoints.Clear();
			PopulateSourcePoints(sourcePoints);
			if (sourcePoints.Count > 1)
			{
				parameters.Points.Clear();
				if (Spline && sourcePoints.Count > 3)
				{
					LightningSplineScript.PopulateSpline(parameters.Points, sourcePoints, Generations, DistancePerSegmentHint, Camera);
					parameters.SmoothingFactor = (parameters.Points.Count - 1) / sourcePoints.Count;
				}
				else
				{
					parameters.Points.AddRange(sourcePoints);
					parameters.SmoothingFactor = 1;
				}
				base.CreateLightningBolt(parameters);
			}
		}

		public LightningMeshSurfaceScript()
		{
			RangeOfFloats rangeOfFloats = new RangeOfFloats
			{
				Minimum = 0.5f,
				Maximum = 1f
			};
			MeshOffsetRange = rangeOfFloats;
			PathLengthCount = new RangeOfIntegers
			{
				Minimum = 3,
				Maximum = 6
			};
			rangeOfFloats = new RangeOfFloats
			{
				Minimum = 0.5f,
				Maximum = 1f
			};
			MinimumPathDistanceRange = rangeOfFloats;
			MaximumPathDistance = 2f;
			sourcePoints = new List<Vector3>();
			// base._002Ector();
		}
	}
}
