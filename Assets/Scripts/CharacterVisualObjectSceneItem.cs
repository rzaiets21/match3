using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualObjectSceneItem : MonoBehaviour
{
	[SerializeField]
	private List<MeshRenderer> meshes = new List<MeshRenderer>();

	[SerializeField]
	public Vector3 lookAtPosition;

	[SerializeField]
	public bool isLookAtPositionDefined;

	[SerializeField]
	public int stencilIndex;

	public void InitForRuntime()
	{
		if (meshes.Count != 0)
		{
			Material material = meshes[0].material;
			material.SetInt("_Stencil", stencilIndex);
			for (int i = 0; i < meshes.Count; i++)
			{
				meshes[i].material = material;
			}
		}
	}

	public void SetActive(bool isActive)
	{
		GGUtil.SetActive(this, isActive);
	}

	public void Init(DecoratingScene3DSetup.VisualObject visualObject, Material materialToReplace, SpriteSortingSettings sortingSettings)
	{
		meshes.Clear();
		InitWithTransform(base.transform, visualObject.collisionRoot.name);
		lookAtPosition = Vector3.zero;
		for (int i = 0; i < meshes.Count; i++)
		{
			MeshRenderer meshRenderer = meshes[i];
			meshRenderer.sharedMaterial = materialToReplace;
			if (sortingSettings != null)
			{
				meshRenderer.gameObject.AddComponent<GGSetSortingLayer>().sortingLayer = sortingSettings;
			}
			lookAtPosition += meshRenderer.bounds.center;
		}
		if (meshes.Count > 0)
		{
			lookAtPosition /= (float)meshes.Count;
			isLookAtPositionDefined = true;
		}
	}

	private void InitWithTransform(Transform root, string nameToSearchFor)
	{
		string name = root.name;
		if (name == nameToSearchFor)
		{
			FillMeshes(root);
			return;
		}
		if (!name.ToLower().StartsWith("data") && !(root == base.transform))
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(root.gameObject);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(root.gameObject);
			}
			return;
		}
		List<Transform> list = new List<Transform>();
		foreach (Transform item in root)
		{
			list.Add(item);
		}
		for (int i = 0; i < list.Count; i++)
		{
			Transform root2 = list[i];
			InitWithTransform(root2, nameToSearchFor);
		}
	}

	private void FillMeshes(Transform root)
	{
		MeshRenderer component = root.GetComponent<MeshRenderer>();
		if (component != null)
		{
			meshes.Add(component);
		}
		foreach (Transform item in root)
		{
			FillMeshes(item);
		}
	}
}
