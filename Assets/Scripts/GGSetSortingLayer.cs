using GGMatch3;
using UnityEngine;

public class GGSetSortingLayer : MonoBehaviour
{
	[SerializeField]
	public SpriteSortingSettings sortingLayer = new SpriteSortingSettings();

	private void TryGetSkinnedMeshRenderer()
	{
		SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
		if (!(component == null))
		{
			sortingLayer.Set(component);
		}
	}

	private void OnEnable()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component == null)
		{
			TryGetSkinnedMeshRenderer();
		}
		else
		{
			sortingLayer.Set(component);
		}
	}
}
