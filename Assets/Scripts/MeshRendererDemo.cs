using System.Collections.Generic;
using UnityEngine;

public class MeshRendererDemo : MonoBehaviour
{
	[SerializeField]
	private float distance = 0.2f;

	[SerializeField]
	private int rows = 4;

	[SerializeField]
	private int columns = 5;

	private MeshFilter meshFilter;

	private Mesh mesh;

	private List<Vector3> vertexBuffer;

	private List<int> trisBuffer;

	private List<Vector2> uvBuffer;

	private bool isInitialized;

	private void Init()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			meshFilter = base.gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (mesh == null)
			{
				mesh = new Mesh();
				meshFilter.mesh = mesh;
			}
			int num = 9;
			vertexBuffer = new List<Vector3>(num * 4);
			trisBuffer = new List<int>(num * 6);
			uvBuffer = new List<Vector2>(num * 4);
		}
	}

	private void Start()
	{
		Init();
	}

	private int GetIndex(int column, int row, int columns)
	{
		return column + row * (columns + 1);
	}

	public void DoUpdateMesh()
	{
		Init();
		vertexBuffer.Clear();
		trisBuffer.Clear();
		uvBuffer.Clear();
		Vector3 vector = new Vector3((float)(-columns) * 0.5f * distance, (float)(-rows) * 0.5f * distance, 0f);
		for (int i = 0; i <= rows; i++)
		{
			for (int j = 0; j <= columns; j++)
			{
				Vector3 a = vector + j * Vector3.right * distance + i * Vector3.up * distance;
				Vector2 vector2 = UnityEngine.Random.insideUnitCircle * distance * 0.1f;
				a += new Vector3(vector2.x, vector2.y, 0f);
				vertexBuffer.Add(a);
				Vector2 item = new Vector2(Mathf.InverseLerp(0f, columns, j), Mathf.InverseLerp(0f, rows, i));
				uvBuffer.Add(item);
				if (j < columns && i < rows)
				{
					trisBuffer.Add(GetIndex(j, i, columns));
					trisBuffer.Add(GetIndex(j + 1, i, columns));
					trisBuffer.Add(GetIndex(j, i + 1, columns));
					trisBuffer.Add(GetIndex(j + 1, i, columns));
					trisBuffer.Add(GetIndex(j + 1, i + 1, columns));
					trisBuffer.Add(GetIndex(j, i + 1, columns));
				}
			}
		}
		mesh.bounds.SetMinMax(vector, vector + Vector3.right * columns * distance + Vector3.up * rows * distance);
		mesh.Clear();
		mesh.SetVertices(vertexBuffer);
		mesh.SetUVs(0, uvBuffer);
		mesh.SetTriangles(trisBuffer, 0, calculateBounds: false);
	}
}
