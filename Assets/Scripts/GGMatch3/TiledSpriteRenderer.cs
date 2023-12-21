using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[DisallowMultipleComponent]
	public class TiledSpriteRenderer : MonoBehaviour
	{
		[SerializeField]
		private SpriteSortingSettings sortingSettings = new SpriteSortingSettings();

		private Mesh mesh;

		private MeshFilter _meshFilter;

		private MeshRenderer _meshRenderer;

		private List<Vector3> vertexBuffer;

		private List<int> trisBuffer;

		private List<Vector2> uvBuffer;

		public MeshFilter meshFilter
		{
			get
			{
				if (!_meshFilter)
				{
					_meshFilter = GetComponent<MeshFilter>();
				}
				return _meshFilter;
			}
		}

		public MeshRenderer meshRenderer
		{
			get
			{
				if (!_meshRenderer)
				{
					_meshRenderer = GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		private void InitBuffers(int maxRectangles)
		{
			int capacity = maxRectangles * 4;
			int capacity2 = maxRectangles * 6;
			if (vertexBuffer == null)
			{
				vertexBuffer = new List<Vector3>(capacity);
				uvBuffer = new List<Vector2>(capacity);
				trisBuffer = new List<int>(capacity2);
			}
		}

		private void Init()
		{
			if (mesh == null)
			{
				mesh = meshFilter.mesh;
			}
			if (mesh == null)
			{
				mesh = new Mesh();
				meshFilter.mesh = mesh;
			}
			sortingSettings.Set(meshRenderer);
		}

		public void ClearAndInit(int numRectangles)
		{
			Init();
			InitBuffers(numRectangles);
		}

		public void DrawRectangle(Rect pos, Rect uv)
		{
			Vector2 min = pos.min;
			Vector2 max = pos.max;
			int count = vertexBuffer.Count;
			vertexBuffer.Add(new Vector3(min.x, min.y, 0f));
			vertexBuffer.Add(new Vector3(max.x, min.y, 0f));
			vertexBuffer.Add(new Vector3(max.x, max.y, 0f));
			vertexBuffer.Add(new Vector3(min.x, max.y, 0f));
			uvBuffer.Add(new Vector2(uv.xMin, uv.yMin));
			uvBuffer.Add(new Vector2(uv.xMax, uv.yMin));
			uvBuffer.Add(new Vector2(uv.xMax, uv.yMax));
			uvBuffer.Add(new Vector2(uv.xMin, uv.yMax));
			trisBuffer.Add(count);
			trisBuffer.Add(count + 2);
			trisBuffer.Add(count + 1);
			trisBuffer.Add(count + 2);
			trisBuffer.Add(count);
			trisBuffer.Add(count + 3);
		}

		public void CopyToMesh()
		{
			mesh.Clear();
			mesh.SetVertices(vertexBuffer);
			mesh.SetUVs(0, uvBuffer);
			mesh.SetTriangles(trisBuffer, 0, calculateBounds: true);
		}
	}
}
