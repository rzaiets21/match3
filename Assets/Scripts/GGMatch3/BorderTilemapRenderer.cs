using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class BorderTilemapRenderer : MonoBehaviour
	{
		[SerializeField]
		public float slotSize = 1f;

		[SerializeField]
		private bool setSortingSettings;

		[SerializeField]
		private SpriteSortingSettings sortingSettings = new SpriteSortingSettings();

		[SerializeField]
		private BorderTilemap tilemap;

		private Mesh mesh;

		private MeshFilter _meshFilter;

		private MeshRenderer _meshRenderer;

		private List<Vector3> vertexBuffer;

		private List<int> trisBuffer;

		private List<Vector2> uvBuffer;

		public MeshFilter meshFilter
		{
			get
			{
				if (!_meshFilter)
				{
					_meshFilter = GetComponent<MeshFilter>();
				}
				return _meshFilter;
			}
		}

		public MeshRenderer meshRenderer
		{
			get
			{
				if (!_meshRenderer)
				{
					_meshRenderer = GetComponent<MeshRenderer>();
				}
				return _meshRenderer;
			}
		}

		private void InitBuffers(int vertexCount, int trisCount)
		{
			if (vertexBuffer == null)
			{
				vertexBuffer = new List<Vector3>(vertexCount);
				uvBuffer = new List<Vector2>(vertexCount);
				trisBuffer = new List<int>(trisCount);
			}
		}

		private void Init()
		{
			if (mesh == null)
			{
				mesh = meshFilter.mesh;
			}
			if (mesh == null)
			{
				mesh = new Mesh();
				meshFilter.mesh = mesh;
			}
		}

		public static bool IsEmptySlot(TilesSlotsProvider slotsProvider, IntVector2 position)
		{
			return slotsProvider.GetSlot(position).isEmpty;
		}

		public static bool IsOccupiedSlot(TilesSlotsProvider slotsProvider, IntVector2 position)
		{
			return slotsProvider.GetSlot(position).isOccupied;
		}

		public void ShowBorder(TilesSlotsProvider slotsProvider)
		{
			Init();
			int maxSlots = slotsProvider.MaxSlots;
			int vertexCount = maxSlots * 8 * 4;
			int trisCount = maxSlots * 8 * 6;
			InitBuffers(vertexCount, trisCount);
			ClearBuffers();
			Vector2 a = slotsProvider.StartPosition(slotSize);
			new Rect(0f, 0f, 1f, 1f);
			List<TilesSlotsProvider.Slot> allSlots = slotsProvider.GetAllSlots();
			int num = 2;
			Vector2 size = new Vector2(slotSize / (float)num, slotSize / (float)num);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					IntVector2 intVector = new IntVector2(i, j);
					List<BorderTilemap.SortedTilePiece> list = tilemap.SortedTilesForTilePosition(tilemap.TilePositionFromOffset(intVector));
					for (int k = 0; k < allSlots.Count; k++)
					{
						TilesSlotsProvider.Slot slot = allSlots[k];
						if (slot.isEmpty)
						{
							continue;
						}
						IntVector2 position = slot.position;
						BorderTilemap.TilePiece tilePiece = null;
						BorderTilemap.TilePiece.PositionDefinition positionDefinition = null;
						for (int l = 0; l < list.Count; l++)
						{
							BorderTilemap.SortedTilePiece sortedTilePiece = list[l];
							List<BorderTilemap.TilePiece.Condition> conditions = sortedTilePiece.positionDefinition.conditions;
							bool flag = true;
							for (int m = 0; m < conditions.Count; m++)
							{
								BorderTilemap.TilePiece.Condition condition = conditions[m];
								bool isOccupied = slotsProvider.GetSlot(position + condition.offset).isOccupied;
								if (condition.conditionEnum == BorderTilemap.TilePiece.ConditionEnum.Empty && isOccupied)
								{
									flag = false;
									break;
								}
								if (condition.conditionEnum == BorderTilemap.TilePiece.ConditionEnum.Full && !isOccupied)
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								tilePiece = sortedTilePiece.tilePiece;
								positionDefinition = sortedTilePiece.positionDefinition;
								break;
							}
						}
						if (tilePiece == null || positionDefinition == null)
						{
							continue;
						}
						if (positionDefinition.drawableRects.Count == 0)
						{
							Vector2 position2 = a + new Vector2(((float)position.x + (float)intVector.x * 0.5f) * slotSize, ((float)position.y + (float)intVector.y * 0.5f) * slotSize);
							Rect pos = new Rect(position2, size);
							Rect uv = tilePiece.UvRect(tilemap);
							Vector2 vector = new Vector2(uv.width * 0.01f, uv.height * 0.01f);
							uv.min += vector;
							uv.max -= vector;
							DrawRectangle(pos, uv);
							continue;
						}
						List<BorderTilemap.TilePiece.DrawableRect> drawableRect2 = positionDefinition.drawableRects;
						for (int n = 0; n < positionDefinition.drawableRects.Count; n++)
						{
							BorderTilemap.TilePiece.DrawableRect drawableRect = positionDefinition.drawableRects[n];
							Vector2 position3 = drawableRect.localImageRect.position;
							Vector2 position4 = a + new Vector2(((float)position.x + 0.5f + position3.x) * slotSize, ((float)position.y + 0.5f + position3.y) * slotSize);
							Rect pos2 = new Rect(position4, drawableRect.localImageRect.size * slotSize);
							DrawRectangle(pos2, drawableRect.uvRect);
						}
					}
				}
			}
			SetBuffersToToMesh();
		}

		private void ClearBuffers()
		{
			vertexBuffer.Clear();
			trisBuffer.Clear();
			uvBuffer.Clear();
		}

		private void SetBuffersToToMesh()
		{
			mesh.Clear();
			mesh.SetVertices(vertexBuffer);
			mesh.SetUVs(0, uvBuffer);
			mesh.SetTriangles(trisBuffer, 0, calculateBounds: true);
			if (setSortingSettings)
			{
				sortingSettings.Set(meshRenderer);
			}
			tilemap.material.mainTexture = tilemap.texture;
			meshRenderer.sharedMaterial = tilemap.material;
		}

		private void DrawRectangle(Rect pos, Rect uv)
		{
			Vector2 min = pos.min;
			Vector2 max = pos.max;
			int count = vertexBuffer.Count;
			vertexBuffer.Add(new Vector3(min.x, min.y, 0f));
			vertexBuffer.Add(new Vector3(max.x, min.y, 0f));
			vertexBuffer.Add(new Vector3(max.x, max.y, 0f));
			vertexBuffer.Add(new Vector3(min.x, max.y, 0f));
			uvBuffer.Add(new Vector2(uv.xMin, uv.yMin));
			uvBuffer.Add(new Vector2(uv.xMax, uv.yMin));
			uvBuffer.Add(new Vector2(uv.xMax, uv.yMax));
			uvBuffer.Add(new Vector2(uv.xMin, uv.yMax));
			trisBuffer.Add(count);
			trisBuffer.Add(count + 2);
			trisBuffer.Add(count + 1);
			trisBuffer.Add(count + 2);
			trisBuffer.Add(count);
			trisBuffer.Add(count + 3);
		}
	}
}
