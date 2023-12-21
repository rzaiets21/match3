using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualObjectVariation : MonoBehaviour
{
	[SerializeField]
	public CharacterVisualObjectBehaviour visualObjectBehaviour;

	[SerializeField]
	private VisualObjectVariation variation;

	[SerializeField]
	private List<CharacterVisualSprite> sprites = new List<CharacterVisualSprite>();

	public void SetStencilIndex(int stencilIndex)
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].SetStencilIndex(stencilIndex);
		}
	}

	public void SetActive(bool isActive)
	{
		GGUtil.SetActive(this, isActive);
	}

	public void Init(CharacterVisualObjectBehaviour visualObjectBehaviour, VisualObjectVariation variation)
	{
		this.visualObjectBehaviour = visualObjectBehaviour;
		this.variation = variation;
		for (int i = 0; i < variation.sprites.Count; i++)
		{
			VisualSprite visualSprite = variation.sprites[i];
			if (!visualSprite.visualSprite.isShadow)
			{
				CharacterVisualSprite item = CreateSprite(visualSprite);
				sprites.Add(item);
			}
		}
	}

	public CharacterVisualSprite CreateSprite(VisualSprite vSprite)
	{
		GameObject gameObject = new GameObject(vSprite.visualSprite.spriteName);
		gameObject.layer = base.gameObject.layer;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		CharacterVisualSprite characterVisualSprite = gameObject.AddComponent<CharacterVisualSprite>();
		characterVisualSprite.Init(vSprite, this);
		return characterVisualSprite;
	}
}
