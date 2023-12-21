using System;
using UnityEngine;

public class GGPaintableMesh : MonoBehaviour
{
	public enum UVIndex
	{
		UV0,
		UV1
	}

	[SerializeField]
	private UVIndex uvIndex_;

	[SerializeField]
	public bool ignoreForRendering;

	[NonSerialized]
	private Mesh cachedMesh_;

	[NonSerialized]
	private MeshRenderer meshRenderer_;

	public int uvIndex => (int)uvIndex_;

	public Mesh cachedMesh
	{
		get
		{
			if (cachedMesh_ == null)
			{
				MeshFilter component = GetComponent<MeshFilter>();
				if (component == null)
				{
					return null;
				}
				cachedMesh_ = component.sharedMesh;
			}
			return cachedMesh_;
		}
	}

	public MeshRenderer meshRenderer
	{
		get
		{
			if (meshRenderer_ == null)
			{
				meshRenderer_ = GetComponent<MeshRenderer>();
			}
			return meshRenderer_;
		}
	}
}
