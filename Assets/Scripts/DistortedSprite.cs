using GGMatch3;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DistortedSprite : MonoBehaviour
{
	[SerializeField]
	private DistortedImageRenderer distortedImageRenderer = new DistortedImageRenderer();

	[SerializeField]
	public SpriteSortingSettings sortingSettings = new SpriteSortingSettings();

	private MeshFilter _meshFilter;

	private MeshRenderer _meshRenderer;

	private Mesh mesh;

	private VertexHelper vertexHelper = new VertexHelper();

	public Vector3 bl
	{
		set
		{
			distortedImageRenderer.bl = value;
		}
	}

	public Vector3 tl
	{
		set
		{
			distortedImageRenderer.tl = value;
		}
	}

	public Vector3 br
	{
		set
		{
			distortedImageRenderer.br = value;
		}
	}

	public Vector3 tr
	{
		set
		{
			distortedImageRenderer.tr = value;
		}
	}

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

	public void CreateGeometry()
	{
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.name = base.name + "_BRMesh";
		}
		distortedImageRenderer.PopulateMesh(vertexHelper);
		vertexHelper.FillMesh(mesh);
		meshFilter.mesh = mesh;
		meshRenderer.sortingLayerID = sortingSettings.sortingLayerId;
		meshRenderer.sortingOrder = sortingSettings.sortingOrder;
	}
}
