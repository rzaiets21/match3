using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class BorderTilemap : ScriptableObject
	{
		[Serializable]
		public class BorderDefinition
		{
			public int leftOffset;

			public int leftWidth;

			public int rightOffset;

			public int rightWidth;

			public int topOffset;

			public int topWidth;

			public int bottomOffset;

			public int bottomWidth;
		}

		[Serializable]
		public class TilePiece
		{
			public enum ConditionEnum
			{
				Full,
				Empty
			}

			[Serializable]
			public class Condition
			{
				public IntVector2 offset;

				public ConditionEnum conditionEnum;
			}

			[Serializable]
			public class DrawableRect
			{
				public Rect localImageRect;

				public Rect uvRect;
			}

			[Serializable]
			public class PositionDefinition
			{
				public TilePosition position;

				public List<Condition> conditions = new List<Condition>();

				public List<DrawableRect> drawableRects = new List<DrawableRect>();
			}

			public IntVector2 positionInOriginal;

			public List<PositionDefinition> positions = new List<PositionDefinition>();

			public IntVector2 rowColumnInTileMap;

			public Rect UvRect(BorderTilemap tilemap)
			{
				return new Rect((float)rowColumnInTileMap.x * tilemap.uvSize.x, (float)rowColumnInTileMap.y * tilemap.uvSize.y, tilemap.uvSize.x, tilemap.uvSize.y);
			}
		}

		public struct SortedTilePiece
		{
			public TilePiece tilePiece;

			public TilePiece.PositionDefinition positionDefinition;
		}

		public int maxTextureWidth = 1024;

		public BorderDefinition border = new BorderDefinition();

		public int spritePadding;

		public List<TilePiece> tiles = new List<TilePiece>();

		public Vector2 uvSize;

		public Texture2D texture;

		public Material material;

		private List<SortedTilePiece> sortedTiles = new List<SortedTilePiece>();

		public List<SortedTilePiece> SortedTilesForTilePosition(TilePosition tilePosition)
		{
			sortedTiles.Clear();
			for (int i = 0; i < tiles.Count; i++)
			{
				TilePiece tilePiece = tiles[i];
				for (int j = 0; j < tilePiece.positions.Count; j++)
				{
					TilePiece.PositionDefinition positionDefinition = tilePiece.positions[j];
					if (positionDefinition.position == tilePosition)
					{
						SortedTilePiece item = default(SortedTilePiece);
						item.positionDefinition = positionDefinition;
						item.tilePiece = tilePiece;
						sortedTiles.Add(item);
					}
				}
			}
			return sortedTiles;
		}

		public TilePosition TilePositionFromOffset(IntVector2 direction)
		{
			if (direction.x == 0 && direction.y == 0)
			{
				return TilePosition.BottomLeft;
			}
			if (direction.x == 1 && direction.y == 0)
			{
				return TilePosition.BottomRight;
			}
			if (direction.x == 0 && direction.y == 1)
			{
				return TilePosition.TopLeft;
			}
			if (direction.x == 1 && direction.y == 1)
			{
				return TilePosition.TopRight;
			}
			return TilePosition.TopLeft;
		}
	}
}
