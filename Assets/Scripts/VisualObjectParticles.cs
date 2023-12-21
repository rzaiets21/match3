using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualObjectParticles : MonoBehaviour
{
	public enum PositionType
	{
		ChangeSuccess,
		BuySuccess
	}

	[Serializable]
	public class PieceCreatorPool
	{
		public PositionType type;

		public GameObject prefab;
	}

	[SerializeField]
	private SpriteSortingSettings sortingLayer = new SpriteSortingSettings();

	[SerializeField]
	private List<PieceCreatorPool> pieceCreatorPools = new List<PieceCreatorPool>();

	private PieceCreatorPool GetPool(PositionType type)
	{
		for (int i = 0; i < pieceCreatorPools.Count; i++)
		{
			PieceCreatorPool pieceCreatorPool = pieceCreatorPools[i];
			if (pieceCreatorPool.type == type)
			{
				return pieceCreatorPool;
			}
		}
		return null;
	}

	public void CreateParticles(PositionType positionType, GameObject parent, VisualObjectBehaviour visualObjectBehaviour)
	{
		VisualObjectVariation activeVariation = visualObjectBehaviour.activeVariation;
		for (int i = 0; i < activeVariation.sprites.Count; i++)
		{
			VisualSprite visualSprite = activeVariation.sprites[i];
			if (visualSprite.visualSprite.isShadow)
			{
				continue;
			}
			SpriteRenderer spriteRenderer = visualSprite.spriteRenderer;
			GameObject gameObject = CreateParticles(positionType, parent);
			GGUtil.Show(gameObject);
			if (gameObject == null)
			{
				continue;
			}
			Match3ParticleSystem component = gameObject.GetComponent<Match3ParticleSystem>();
			if (!(component == null))
			{
				List<ParticleSystem> allParticleSystems = component.GetAllParticleSystems();
				for (int j = 0; j < allParticleSystems.Count; j++)
				{
					ParticleSystem particleSystem = allParticleSystems[j];
					var particleSystemShape = particleSystem.shape;
					particleSystemShape.spriteRenderer = spriteRenderer;
					ParticleSystemRenderer component2 = particleSystem.GetComponent<ParticleSystemRenderer>();
					component2.sortingLayerID = sortingLayer.sortingLayerId;
					component2.sortingOrder = spriteRenderer.sortingOrder + 1;
				}
				component.StartParticleSystems();
			}
		}
	}

	public GameObject CreateParticles(PositionType positionType, GameObject parent)
	{
		PieceCreatorPool pool = GetPool(positionType);
		if (pool == null)
		{
			return null;
		}
		GameObject prefab = pool.prefab;
		if (prefab == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		gameObject.transform.parent = parent.transform;
		return gameObject;
	}
}
