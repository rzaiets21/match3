using JSONData;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GraphicsSceneConfig
{
	[Serializable]
	public class VisualSprite
	{
		public string spriteName;

		public string spritePath;

		[NonSerialized]
		public Sprite sprite;

		public bool isShadow;

		public int totalWidth;

		public int totalHeight;

		public int left;

		public int right;

		public int top;

		public int bottom;

		public Vector3 pivotPosition;

		public int depth;

		public int initialDepth;

		public Vector3 spritePosition
		{
			get
			{
				if (sprite == null)
				{
					return Vector3.right * left + Vector3.down * top;
				}
				Vector2 pivot = sprite.pivot;
				pivot.x /= sprite.rect.width;
				pivot.y /= sprite.rect.height;
				return Vector3.right * Mathf.Lerp(left, right, pivot.x) + Vector3.down * Mathf.Lerp(bottom, top, pivot.y);
			}
		}

		public int width => right - left;

		public int height => top - bottom;
	}

	[Serializable]
	public class Variation
	{
		public string name;

		public List<VisualSprite> sprites = new List<VisualSprite>();

		public string thumbnailName;

		public Sprite thumbnailSprite;

		public List<Triangle> hitboxTriangles = new List<Triangle>();
	}

	[Serializable]
	public class DrawnShape
	{
		public int depth;

		public ShapeGraphShape shape;
	}

	[Serializable]
	public class Triangle
	{
		[Serializable]
		public struct PlaneZSetup
		{
			public float dc;

			public float ac;

			public float bc;

			public float GetZ(Vector2 pos)
			{
				return dc - ac * pos.x - bc * pos.y;
			}
		}

		public int depth;

		public PlaneZSetup planeZSetup;

		public Vector2 p1;

		public Vector2 p2;

		public Vector2 p3;

		private static Vector2[] pointsCache = new Vector2[3];

		public float GetZ(Vector2 pos)
		{
			return planeZSetup.GetZ(pos);
		}

		public bool IsInside(Vector2 pos)
		{
			int num = 0;
			pointsCache[0] = p1;
			pointsCache[1] = p2;
			pointsCache[2] = p3;
			Vector2[] array = pointsCache;
			for (int i = 0; i < array.Length; i++)
			{
				Vector2 b = array[i];
				Vector2 v = ((i == array.Length - 1) ? array[0] : array[i + 1]) - b;
				int num2 = Mathf.RoundToInt(Mathf.Sign(Vector3.Cross(rhs: pos - b, lhs: v).z));
				if (num != 0 && num != num2)
				{
					return false;
				}
				num = num2;
			}
			return true;
		}
	}

	[Serializable]
	public class VisualObject
	{
		public struct HitResult
		{
			public bool isHit;

			public int hitDepth;
		}

		public string name;

		public List<Variation> variations = new List<Variation>();

		private List<Variation> allVariations_ = new List<Variation>();

		public int index;

		public SceneObjectsDB.SceneObjectInfo sceneObjectInfo = new SceneObjectsDB.SceneObjectInfo();

		public List<ShapeGraphShape> hitboxes = new List<ShapeGraphShape>();

		public List<Triangle> hitboxTriangles = new List<Triangle>();

		public List<DrawnShape> hitboxesList = new List<DrawnShape>();

		public List<ShapeGraphShape> dashLines = new List<ShapeGraphShape>();

		public Vector3 iconHandlePosition;

		public bool hasDefaultVariation;

		public Variation defaultVariation;

		private RoomsBackend.RoomAccessor roomAccessor_;

		private RoomsBackend.VisualObjectAccessor visualObjectModel_;

		public string animationSettingsName
		{
			get
			{
				if (string.IsNullOrEmpty(sceneObjectInfo.animationSettingsName))
				{
					return name;
				}
				return sceneObjectInfo.animationSettingsName;
			}
		}

		public string displayName
		{
			get
			{
				if (string.IsNullOrEmpty(sceneObjectInfo.displayName))
				{
					return GGUtil.FirstCharToUpper(name);
				}
				return sceneObjectInfo.displayName;
			}
		}

		private RoomsBackend.RoomAccessor roomAccessor
		{
			get
			{
				if (roomAccessor_.needsToBeRenewed)
				{
					roomAccessor_ = roomAccessor_.CreateRenewedAccessor();
				}
				return roomAccessor_;
			}
		}

		private RoomsBackend.VisualObjectAccessor visualObjectModel
		{
			get
			{
				if (visualObjectModel_ == null)
				{
					visualObjectModel_ = roomAccessor.GetVisualObject(name);
				}
				if (visualObjectModel_.needsToBeRenewed)
				{
					visualObjectModel_ = roomAccessor.GetVisualObject(name);
				}
				return visualObjectModel_;
			}
		}

		public Variation ownedVariation
		{
			get
			{
				if (ownedVariationIndex < 0 || ownedVariationIndex >= variations.Count)
				{
					return null;
				}
				return variations[ownedVariationIndex];
			}
		}

		public bool isOwned
		{
			get
			{
				return visualObjectModel.visualObject.isOwned;
			}
			set
			{
				visualObjectModel.visualObject.isOwned = value;
				visualObjectModel.Save();
			}
		}

		private bool isUnlocked
		{
			get
			{
				for (int i = 0; i < sceneObjectInfo.backwardDependencies.Count; i++)
				{
					string visualObjectName = sceneObjectInfo.backwardDependencies[i];
					if (!roomAccessor.IsOwned(visualObjectName))
					{
						return false;
					}
				}
				return true;
			}
		}

		public int ownedVariationIndex
		{
			get
			{
				return visualObjectModel.visualObject.selectedVariationIndex;
			}
			set
			{
				visualObjectModel.visualObject.selectedVariationIndex = value;
				visualObjectModel.Save();
			}
		}

		public int depthForMarkerLines
		{
			get
			{
				if (sceneObjectInfo.isMarkersAbove)
				{
					return maxDepth + 1;
				}
				return startingDepth - 1;
			}
		}

		public int maxDepth
		{
			get
			{
				bool flag = false;
				int num = 0;
				for (int i = 0; i < variations.Count; i++)
				{
					Variation variation = variations[i];
					for (int j = 0; j < variation.sprites.Count; j++)
					{
						VisualSprite visualSprite = variation.sprites[j];
						if (!flag)
						{
							num = visualSprite.depth;
							flag = true;
						}
						else
						{
							num = Mathf.Max(num, visualSprite.depth);
						}
					}
				}
				if (!flag)
				{
					return 0;
				}
				return num;
			}
		}

		public int startingDepth
		{
			get
			{
				bool flag = false;
				int num = 0;
				if (hasDefaultVariation)
				{
					Variation variation = defaultVariation;
					for (int i = 0; i < variation.sprites.Count; i++)
					{
						VisualSprite visualSprite = variation.sprites[i];
						if (!flag)
						{
							num = visualSprite.depth;
							flag = true;
						}
						else
						{
							num = Mathf.Min(num, visualSprite.depth);
						}
					}
				}
				for (int j = 0; j < variations.Count; j++)
				{
					Variation variation2 = variations[j];
					for (int k = 0; k < variation2.sprites.Count; k++)
					{
						VisualSprite visualSprite2 = variation2.sprites[k];
						if (!flag)
						{
							num = visualSprite2.depth;
							flag = true;
						}
						else
						{
							num = Mathf.Min(num, visualSprite2.depth);
						}
					}
				}
				if (!flag)
				{
					return 0;
				}
				return num;
			}
		}

		public void Init(RoomsBackend.RoomAccessor roomAccessor)
		{
			roomAccessor_ = roomAccessor;
		}

		public HitResult GetHitResult(Vector2 psdPosition)
		{
			HitResult hitResult = default(HitResult);
			List<Triangle> list = hitboxTriangles;
			Variation ownedVariation = this.ownedVariation;
			if (ownedVariation != null && ownedVariation.hitboxTriangles.Count > 0)
			{
				list = ownedVariation.hitboxTriangles;
			}
			for (int i = 0; i < list.Count; i++)
			{
				Triangle triangle = list[i];
				if (triangle.IsInside(psdPosition))
				{
					int num = -Mathf.RoundToInt(triangle.GetZ(psdPosition) * 1000f);
					if (!hitResult.isHit || hitResult.hitDepth < num)
					{
						hitResult.isHit = true;
						hitResult.hitDepth = num;
					}
				}
			}
			if (list.Count > 0)
			{
				return hitResult;
			}
			for (int j = 0; j < hitboxesList.Count; j++)
			{
				DrawnShape drawnShape = hitboxesList[j];
				if (drawnShape.shape.IsInside(psdPosition) && (!hitResult.isHit || hitResult.hitDepth < drawnShape.depth))
				{
					hitResult.isHit = true;
					hitResult.hitDepth = drawnShape.depth;
				}
			}
			if (hitboxesList.Count > 0)
			{
				return hitResult;
			}
			hitResult.isHit = IsHit(psdPosition);
			hitResult.hitDepth = maxDepth;
			return hitResult;
		}

		private bool IsHit(Vector2 psdPostion)
		{
			for (int i = 0; i < hitboxes.Count; i++)
			{
				if (hitboxes[i].IsInside(psdPostion))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsUnlocked(DecoratingScene scene)
		{
			if (sceneObjectInfo.groupIndex > 0 && !scene.IsAllElementsPickedUpInGroup(sceneObjectInfo.groupIndex - 1))
			{
				return false;
			}
			return isUnlocked;
		}
	}

	public class ValidName
	{
		public List<string> validNames;
	}

	public int width;

	public int height;

	public List<VisualObject> objects = new List<VisualObject>();

	public List<ValidName> validVisualObjectNames = new List<ValidName>();

	public override string ToString()
	{
		return JsonUtility.ToJson(this);
	}
}
