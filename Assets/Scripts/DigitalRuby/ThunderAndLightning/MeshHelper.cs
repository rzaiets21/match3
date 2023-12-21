using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
	public class MeshHelper
	{
		private Mesh mesh;

		private int[] triangles;

		private Vector3[] vertices;

		private Vector3[] normals;

		private float[] normalizedAreaWeights;

		public int[] Triangles => triangles;

		public MeshHelper(Mesh mesh)
		{
			this.mesh = mesh;
			triangles = mesh.triangles;
			vertices = mesh.vertices;
			normals = mesh.normals;
			CalculateNormalizedAreaWeights();
		}

		public void GenerateRandomPoint(ref RaycastHit hit, out int triangleIndex)
		{
			triangleIndex = SelectRandomTriangle();
			GetRaycastFromTriangleIndex(triangleIndex, ref hit);
		}

		public void GetRaycastFromTriangleIndex(int triangleIndex, ref RaycastHit hit)
		{
			Vector3 vector = GenerateRandomBarycentricCoordinates();
			Vector3 a = vertices[triangles[triangleIndex]];
			Vector3 vector2 = vertices[triangles[triangleIndex + 1]];
			Vector3 a2 = vertices[triangles[triangleIndex + 2]];
			hit.barycentricCoordinate = vector;
			hit.point = a * vector.x + vector2 * vector.y + a2 * vector.z;
			if (normals == null)
			{
				hit.normal = Vector3.Cross(a2 - vector2, a - vector2).normalized;
				return;
			}
			a = normals[triangles[triangleIndex]];
			vector2 = normals[triangles[triangleIndex + 1]];
			a2 = normals[triangles[triangleIndex + 2]];
			hit.normal = a * vector.x + vector2 * vector.y + a2 * vector.z;
		}

		private float[] CalculateSurfaceAreas(out float totalSurfaceArea)
		{
			int num = 0;
			totalSurfaceArea = 0f;
			float[] array = new float[triangles.Length / 3];
			for (int i = 0; i < triangles.Length; i += 3)
			{
				Vector3 a = vertices[triangles[i]];
				Vector3 vector = vertices[triangles[i + 1]];
				Vector3 b = vertices[triangles[i + 2]];
				float sqrMagnitude = (a - vector).sqrMagnitude;
				float sqrMagnitude2 = (a - b).sqrMagnitude;
				float sqrMagnitude3 = (vector - b).sqrMagnitude;
				float num2 = PathGenerator.SquareRoot((2f * sqrMagnitude * sqrMagnitude2 + 2f * sqrMagnitude2 * sqrMagnitude3 + 2f * sqrMagnitude3 * sqrMagnitude - sqrMagnitude * sqrMagnitude - sqrMagnitude2 * sqrMagnitude2 - sqrMagnitude3 * sqrMagnitude3) / 16f);
				array[num++] = num2;
				totalSurfaceArea += num2;
			}
			return array;
		}

		private void CalculateNormalizedAreaWeights()
		{
			normalizedAreaWeights = CalculateSurfaceAreas(out float totalSurfaceArea);
			if (normalizedAreaWeights.Length != 0)
			{
				float num = 0f;
				for (int i = 0; i < normalizedAreaWeights.Length; i++)
				{
					float num2 = normalizedAreaWeights[i] / totalSurfaceArea;
					normalizedAreaWeights[i] = num;
					num += num2;
				}
			}
		}

		private int SelectRandomTriangle()
		{
			float value = Random.value;
			int num = 0;
			int num2 = normalizedAreaWeights.Length - 1;
			while (num < num2)
			{
				int num3 = (num + num2) / 2;
				if (normalizedAreaWeights[num3] < value)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3;
				}
			}
			return num * 3;
		}

		private Vector3 GenerateRandomBarycentricCoordinates()
		{
			Vector3 vector = new Vector3(Random.Range(Mathf.Epsilon, 1f), Random.Range(Mathf.Epsilon, 1f), Random.Range(Mathf.Epsilon, 1f));
			return vector / (vector.x + vector.y + vector.z);
		}
	}
}
