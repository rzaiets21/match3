using System;
using UnityEngine;

namespace GGMatch3
{
	[Serializable]
	public class SpriteSortingSettings
	{
		public int sortingLayerId;

		public int sortingOrder;

		public void Set(SpriteRenderer spriteRenderer)
		{
			if (!(spriteRenderer == null))
			{
				spriteRenderer.sortingLayerID = sortingLayerId;
				spriteRenderer.sortingOrder = sortingOrder;
			}
		}

		public void Set(SkinnedMeshRenderer meshRenderer)
		{
			meshRenderer.sortingLayerID = sortingLayerId;
			meshRenderer.sortingOrder = sortingOrder;
		}

		public void Set(MeshRenderer meshRenderer)
		{
			meshRenderer.sortingLayerID = sortingLayerId;
			meshRenderer.sortingOrder = sortingOrder;
		}
	}
}
