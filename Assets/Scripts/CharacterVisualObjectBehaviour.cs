using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualObjectBehaviour : MonoBehaviour
{
	public RoomCharacterScene scene;

	[SerializeField]
	public VisualObjectBehaviour visualObjectBeh;

	[SerializeField]
	public CharacterVisualObjectVariation defaultVariation;

	[SerializeField]
	public List<CharacterVisualObjectVariation> variations = new List<CharacterVisualObjectVariation>();

	[SerializeField]
	public List<CharacterVisualObjectVariation> allVariations = new List<CharacterVisualObjectVariation>();

	[SerializeField]
	public CharacterVisualObjectSceneItem sceneItem;

	private bool isInitializedForRuntime;

	public Vector3 lookAtPosition
	{
		get
		{
			if (sceneItem == null)
			{
				return Vector3.zero;
			}
			return sceneItem.lookAtPosition;
		}
	}

	public bool isLookAtPositionDefined
	{
		get
		{
			if (sceneItem == null)
			{
				return false;
			}
			return sceneItem.isLookAtPositionDefined;
		}
	}

	public void InitForRuntime()
	{
		if (isInitializedForRuntime)
		{
			return;
		}
		isInitializedForRuntime = true;
		if (!(sceneItem == null))
		{
			int stencilIndex = sceneItem.stencilIndex;
			sceneItem.InitForRuntime();
			for (int i = 0; i < allVariations.Count; i++)
			{
				allVariations[i].SetStencilIndex(stencilIndex);
			}
		}
	}

	public void Init(VisualObjectBehaviour visualObjectBeh, RoomCharacterScene scene)
	{
		this.scene = scene;
		this.visualObjectBeh = visualObjectBeh;
		for (int i = 0; i < visualObjectBeh.variations.Count; i++)
		{
			VisualObjectVariation variation = visualObjectBeh.variations[i];
			CharacterVisualObjectVariation item = CreateVariation(variation);
			variations.Add(item);
			allVariations.Add(item);
		}
		if (visualObjectBeh.hasDefaultVariation)
		{
			defaultVariation = CreateVariation(visualObjectBeh.defaultVariation);
			allVariations.Add(defaultVariation);
		}
	}

	private CharacterVisualObjectVariation CreateVariation(VisualObjectVariation variation)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = base.transform;
		gameObject.name = variation.name;
		gameObject.layer = base.gameObject.layer;
		gameObject.transform.localPosition = Vector3.zero;
		CharacterVisualObjectVariation characterVisualObjectVariation = gameObject.AddComponent<CharacterVisualObjectVariation>();
		characterVisualObjectVariation.Init(this, variation);
		return characterVisualObjectVariation;
	}

	public void ShowGlobalVariation(int variationIndex)
	{
		bool flag = variationIndex >= 0 && variationIndex < allVariations.Count;
		for (int i = 0; i < allVariations.Count; i++)
		{
			CharacterVisualObjectVariation characterVisualObjectVariation = allVariations[i];
			bool active = variationIndex == i;
			characterVisualObjectVariation.SetActive(active);
		}
		if (sceneItem != null)
		{
			sceneItem.SetActive(flag);
		}
		if (flag)
		{
			InitForRuntime();
		}
	}

	public void Hide()
	{
		ShowGlobalVariation(-1);
	}
}
