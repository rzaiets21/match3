using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Match3Particles : MonoBehaviour
{
	public enum PositionType
	{
		OnDestroyChip,
		OnExplosion,
		OnSeekingMissleExplosion,
		BoxDestroy,
		OnRocketStart,
		MagicHatCreate,
		PipeExitParticle,
		PipeEnterParticle,
		OnHammerHit,
		OnHammerPowerHit,
		BombCombine,
		ChipSwipeHorizontal,
		ChipSwipeVertical,
		MissleHitTarget,
		OnDestroyChipDiscoBomb,
		OnDestroyChipLightStart,
		OnDestroyChipPowerupCreate,
		BombCreate,
		BubblesDestroy,
		BubblesCreate,
		OnUIGoalCollected,
		BurriedElementBreak,
		PlacePowerupParticles,
		OnDestroyChipCollect,
		OnDestroyChipExplosion,
		OnDestroyChipRocket,
		BurriedElementTravelParticle,
		ChipTap,
		GoalComplete
	}

	[Serializable]
	public class ItemColorModifier
	{
		public ItemColor ItemColor;

		public bool modifyColor;

		public Color colorMin;

		public Color colorMax;

		public void Apply(GameObject go)
		{
			if (!modifyColor || go == null)
			{
				return;
			}
			Match3ParticleSystem component = go.GetComponent<Match3ParticleSystem>();
			if (component == null)
			{
				return;
			}
			List<ParticleSystem> allParticleSystems = component.GetAllParticleSystems();
			for (int i = 0; i < allParticleSystems.Count; i++)
			{
				ParticleSystem particleSystem = allParticleSystems[i];
				string text = particleSystem.gameObject.name.ToLower();
				if (text.Contains("_itemcolor_"))
				{
					GGUtil.SetActive(particleSystem.gameObject, text.Contains("_itemcolor_" + ItemColor.ToString().ToLower()));
				}
				if (!text.EndsWith("ignore") && modifyColor)
				{
					ParticleSystem.MainModule main = particleSystem.main;
					ParticleSystem.MinMaxGradient startColor = main.startColor;
					startColor.colorMin = colorMin;
					startColor.colorMax = colorMax;
					main.startColor = startColor;
				}
			}
		}
	}

	[Serializable]
	public class PieceCreatorPool
	{
		public enum AbTestVariant
		{
			Normal,
			CandyType,
			CandyAnimType,
			BurriedTest,
			BarryChipTest
		}

		public PositionType type;

		[SerializeField]
		public AbTestVariant abTestVariant;

		[SerializeField]
		private List<AbTestVariant> abTestVariantsList = new List<AbTestVariant>();

		public List<ChipType> chipTypeList = new List<ChipType>();

		public List<int> levelList = new List<int>();

		public List<ItemColorModifier> itemColorModifiers = new List<ItemColorModifier>();

		public ComponentPool pool;

		public bool IsAcceptableForVariant(int variant)
		{
			if (abTestVariant == (AbTestVariant)variant)
			{
				return true;
			}
			for (int i = 0; i < abTestVariantsList.Count; i++)
			{
				if (abTestVariantsList[i] == (AbTestVariant)variant)
				{
					return true;
				}
			}
			return false;
		}

		public ItemColorModifier GetModifier(ItemColor itemColor)
		{
			for (int i = 0; i < itemColorModifiers.Count; i++)
			{
				ItemColorModifier itemColorModifier = itemColorModifiers[i];
				if (itemColorModifier.ItemColor == itemColor)
				{
					return itemColorModifier;
				}
			}
			return null;
		}
	}

	[NonSerialized]
	public bool disableParticles;

	[SerializeField]
	private List<PieceCreatorPool> pieceCreatorPools = new List<PieceCreatorPool>();

	private List<PieceCreatorPool> positionTypeFilteredPools_ = new List<PieceCreatorPool>();

	private List<PieceCreatorPool> GetPoolsForPositionType(PositionType type)
	{
		int match3ParticlesVariant = GGTest.match3ParticlesVariant;
		positionTypeFilteredPools_.Clear();
		for (int i = 0; i < pieceCreatorPools.Count; i++)
		{
			PieceCreatorPool pieceCreatorPool = pieceCreatorPools[i];
			if (pieceCreatorPool.type == type)
			{
				if (pieceCreatorPool.IsAcceptableForVariant(match3ParticlesVariant))
				{
					positionTypeFilteredPools_.Insert(0, pieceCreatorPool);
				}
				else if (pieceCreatorPool.abTestVariant == PieceCreatorPool.AbTestVariant.Normal)
				{
					positionTypeFilteredPools_.Add(pieceCreatorPool);
				}
			}
		}
		return positionTypeFilteredPools_;
	}

	private PieceCreatorPool GetPool(PositionType type)
	{
		List<PieceCreatorPool> poolsForPositionType = GetPoolsForPositionType(type);
		for (int i = 0; i < poolsForPositionType.Count; i++)
		{
			PieceCreatorPool pieceCreatorPool = poolsForPositionType[i];
			if (pieceCreatorPool.type == type)
			{
				return pieceCreatorPool;
			}
		}
		return null;
	}

	private PieceCreatorPool GetPool(PositionType type, ChipType chipType)
	{
		List<PieceCreatorPool> poolsForPositionType = GetPoolsForPositionType(type);
		for (int i = 0; i < poolsForPositionType.Count; i++)
		{
			PieceCreatorPool pieceCreatorPool = poolsForPositionType[i];
			if (pieceCreatorPool.type == type && pieceCreatorPool.chipTypeList.Contains(chipType))
			{
				return pieceCreatorPool;
			}
		}
		return null;
	}

	private PieceCreatorPool GetPool(PositionType type, ChipType chipType, int level)
	{
		List<PieceCreatorPool> poolsForPositionType = GetPoolsForPositionType(type);
		for (int i = 0; i < poolsForPositionType.Count; i++)
		{
			PieceCreatorPool pieceCreatorPool = poolsForPositionType[i];
			if (pieceCreatorPool.type == type && pieceCreatorPool.chipTypeList.Contains(chipType) && pieceCreatorPool.levelList.Contains(level))
			{
				return pieceCreatorPool;
			}
		}
		return null;
	}

	public GameObject CreateParticles(Vector3 localPositionOfCenter, PositionType positionType, Quaternion rotation)
	{
		GameObject gameObject = CreateParticles(localPositionOfCenter, positionType);
		if (gameObject == null)
		{
			return gameObject;
		}
		gameObject.transform.rotation = rotation;
		return gameObject;
	}

	public GameObject CreateParticles(Slot slot, PositionType positionType)
	{
		return CreateParticles(slot.localPositionOfCenter, positionType);
	}

	public GameObject CreateParticles(Vector3 localPosition, PositionType positionType)
	{
		if (disableParticles)
		{
			return null;
		}
		PieceCreatorPool pool = GetPool(positionType);
		if (pool == null)
		{
			return null;
		}
		GameObject gameObject = pool.pool.Instantiate();
		if (gameObject == null)
		{
			return null;
		}
		gameObject.transform.localPosition = localPosition;
		Match3ParticleSystem component = gameObject.GetComponent<Match3ParticleSystem>();
		if (component != null)
		{
			component.StartParticleSystems();
		}
		GGUtil.SetActive(gameObject, active: true);
		return gameObject;
	}

	public GameObject CreateParticles(Chip chip, PositionType positionType, ChipType chipType, ItemColor itemColor)
	{
		TransformBehaviour componentBehaviour = chip.GetComponentBehaviour<TransformBehaviour>();
		if (componentBehaviour == null)
		{
			return null;
		}
		return CreateParticles(componentBehaviour.localPosition, positionType, chipType, itemColor);
	}

	public GameObject CreateParticlesWorld(Vector3 worldPosition, PositionType positionType, ChipType chipType, ItemColor itemColor)
	{
		PieceCreatorPool pool = GetPool(positionType, chipType);
		if (pool == null)
		{
			return null;
		}
		Vector3 localPosition = pool.pool.parent.InverseTransformPoint(worldPosition);
		localPosition.z = 0f;
		return CreateParticles(localPosition, positionType, chipType, itemColor);
	}

	public GameObject CreateParticles(Vector3 localPosition, PositionType positionType, ChipType chipType, int level)
	{
		if (disableParticles)
		{
			return null;
		}
		PieceCreatorPool pool = GetPool(positionType, chipType, level);
		if (pool == null)
		{
			pool = GetPool(positionType, chipType);
		}
		if (pool == null || pool.pool == null)
		{
			return null;
		}
		GameObject gameObject = pool.pool.Instantiate();
		if (gameObject == null)
		{
			return null;
		}
		gameObject.transform.localPosition = localPosition;
		Match3ParticleSystem component = gameObject.GetComponent<Match3ParticleSystem>();
		if (component != null)
		{
			component.StartParticleSystems();
		}
		GGUtil.SetActive(gameObject, active: true);
		return gameObject;
	}

	public GameObject CreateParticles(Vector3 localPosition, PositionType positionType, ChipType chipType, ItemColor itemColor)
	{
		if (disableParticles)
		{
			return null;
		}
		PieceCreatorPool pool = GetPool(positionType, chipType);
		if (pool == null || pool.pool == null)
		{
			return null;
		}
		GameObject gameObject = pool.pool.Instantiate();
		if (gameObject == null)
		{
			return null;
		}
		gameObject.transform.localPosition = localPosition;
		Match3ParticleSystem component = gameObject.GetComponent<Match3ParticleSystem>();
		if (component != null)
		{
			component.StartParticleSystems();
		}
		pool.GetModifier(itemColor)?.Apply(gameObject);
		GGUtil.SetActive(gameObject, active: true);
		return gameObject;
	}
}
