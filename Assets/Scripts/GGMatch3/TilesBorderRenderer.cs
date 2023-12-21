using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class TilesBorderRenderer : MonoBehaviour
	{
		[SerializeField]
		private bool dontConnectDiagonalSlots;

		[SerializeField]
		public float slotSize = 1f;

		[SerializeField]
		private float borderSize = 0.25f;

		[SerializeField]
		private float borderOffset;

		public int sortingLayerId;

		public int sortingOrder;

		[SerializeField]
		private bool setSortingSettings;

		[SerializeField]
		private SpriteSortingSettings sortingSettings = new SpriteSortingSettings();

		private Mesh mesh;

		private MeshFilter _meshFilter;

		private MeshRenderer _meshRenderer;

		private List<Vector3> vertexBuffer;

		private List<int> trisBuffer;

		private List<Vector2> uvBuffer;

		public TilesSlotsProvider cachedProviderDebug;

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

		public void ShowBorderOnLevel(LevelDefinition level)
		{
			LevelDefinitionTilesSlotsProvider slotsProvider = new LevelDefinitionTilesSlotsProvider(level);
			ShowBorderOnLevel(slotsProvider);
		}

		public void ShowBorderOnLevelDontConnectDiagonalSlots(TilesSlotsProvider slotsProvider)
		{
			Init();
			int maxSlots = slotsProvider.MaxSlots;
			int vertexCount = maxSlots * 8 * 4;
			int trisCount = maxSlots * 8 * 6;
			InitBuffers(vertexCount, trisCount);
			ClearBuffers();
			Vector2 a = slotsProvider.StartPosition(slotSize);
			Vector2 size = new Vector2(slotSize, slotSize);
			new Rect(0f, 0f, 0.5f, 0.5f);
			List<TilesSlotsProvider.Slot> allSlots = slotsProvider.GetAllSlots();
			for (int i = 0; i < allSlots.Count; i++)
			{
				TilesSlotsProvider.Slot slot = allSlots[i];
				if (slot.isEmpty)
				{
					continue;
				}
				IntVector2 position = slot.position;
				Vector2 vector = a + new Vector2((float)position.x * slotSize, (float)position.y * slotSize);
				new Rect(vector, size);
				bool flag = IsEmptySlot(slotsProvider, position + IntVector2.up + IntVector2.left);
				bool flag2 = IsEmptySlot(slotsProvider, position + IntVector2.up + IntVector2.right);
				bool flag3 = !flag;
				bool flag4 = !flag2;
				bool flag5 = IsEmptySlot(slotsProvider, position + IntVector2.down + IntVector2.left);
				bool flag6 = IsEmptySlot(slotsProvider, position + IntVector2.down + IntVector2.right);
				bool flag7 = !flag5;
				bool flag8 = !flag6;
				bool flag9 = IsEmptySlot(slotsProvider, position + IntVector2.left);
				bool flag10 = !flag9;
				bool flag11 = IsEmptySlot(slotsProvider, position + IntVector2.down);
				bool flag12 = IsEmptySlot(slotsProvider, position + IntVector2.right);
				bool flag13 = !flag12;
				bool flag14 = IsEmptySlot(slotsProvider, position + IntVector2.up);
				bool flag15 = !flag14;
				if (flag9)
				{
					Rect pos = new Rect(vector.x - borderSize, vector.y + borderSize, borderSize, slotSize - 2f * borderSize);
					Rect uv = new Rect(0f, 0.25f, 0.25f, 0.5f);
					if (flag14)
					{
						pos.yMax = vector.y + slotSize - borderOffset;
					}
					else if (flag)
					{
						pos.yMax = vector.y + slotSize;
					}
					else if (flag3)
					{
						pos.yMax += borderOffset;
					}
					if (flag11)
					{
						pos.yMin = vector.y + borderOffset;
					}
					else if (flag5)
					{
						pos.yMin = vector.y;
					}
					else if (flag7)
					{
						pos.yMin -= borderOffset;
					}
					pos.position += Vector2.right * borderOffset;
					DrawRectangle(pos, uv);
				}
				if (flag12)
				{
					Rect pos2 = new Rect(vector.x + slotSize, vector.y + borderSize, borderSize, slotSize - 2f * borderSize);
					Rect uv2 = new Rect(0.75f, 0.25f, 0.25f, 0.5f);
					if (flag14)
					{
						pos2.yMax = vector.y + slotSize - borderOffset;
					}
					else if (flag2)
					{
						pos2.yMax = vector.y + slotSize;
					}
					else if (flag4)
					{
						pos2.yMax += borderOffset;
					}
					if (flag11)
					{
						pos2.yMin = vector.y + borderOffset;
					}
					else if (flag6)
					{
						pos2.yMin = vector.y;
					}
					else if (flag8)
					{
						pos2.yMin -= borderOffset;
					}
					pos2.position += Vector2.left * borderOffset;
					DrawRectangle(pos2, uv2);
				}
				if (flag11)
				{
					Rect pos3 = new Rect(vector.x + borderSize, vector.y - borderSize, slotSize - 2f * borderSize, borderSize);
					if (flag12)
					{
						pos3.xMax = vector.x + slotSize - borderOffset;
					}
					else if (flag6)
					{
						pos3.xMax = vector.x + slotSize;
					}
					else if (flag8)
					{
						pos3.xMax += borderOffset;
					}
					if (flag9)
					{
						pos3.xMin = vector.x + borderOffset;
					}
					else if (flag5)
					{
						pos3.xMin = vector.x;
					}
					else if (flag7)
					{
						pos3.xMin -= borderOffset;
					}
					pos3.position += Vector2.up * borderOffset;
					DrawRectangle(uv: new Rect(0.25f, 0f, 0.5f, 0.25f), pos: pos3);
				}
				if (flag14)
				{
					Rect pos4 = new Rect(vector.x + borderSize, vector.y + slotSize, slotSize - 2f * borderSize, borderSize);
					if (flag12)
					{
						pos4.xMax = vector.x + slotSize - borderOffset;
					}
					else if (flag2)
					{
						pos4.xMax = vector.x + slotSize;
					}
					else if (flag4)
					{
						pos4.xMax += borderOffset;
					}
					if (flag9)
					{
						pos4.xMin = vector.x + borderOffset;
					}
					else if (flag)
					{
						pos4.xMin = vector.x;
					}
					else if (flag3)
					{
						pos4.xMin -= borderOffset;
					}
					pos4.position += Vector2.down * borderOffset;
					DrawRectangle(uv: new Rect(0.25f, 0.75f, 0.5f, 0.25f), pos: pos4);
				}
				if ((flag14 && flag10) & flag3)
				{
					Rect pos5 = new Rect(vector.x, vector.y + slotSize, borderSize, borderSize);
					pos5.position += new Vector2(0f - borderOffset, 0f - borderOffset);
					DrawRectangle(uv: new Rect(0.25f, 0.25f, 0.25f, 0.25f), pos: pos5);
				}
				if ((flag9 && flag15) & flag3)
				{
					Rect pos6 = new Rect(vector.x - borderSize, vector.y + slotSize - borderSize, borderSize, borderSize);
					pos6.position += new Vector2(borderOffset, borderOffset);
					DrawRectangle(uv: new Rect(0.5f, 0.5f, 0.25f, 0.25f), pos: pos6);
				}
				if ((flag12 && flag15) & flag4)
				{
					Rect pos7 = new Rect(vector.x + slotSize, vector.y + slotSize - borderSize, borderSize, borderSize);
					pos7.position += new Vector2(0f - borderOffset, borderOffset);
					DrawRectangle(uv: new Rect(0.25f, 0.5f, 0.25f, 0.25f), pos: pos7);
				}
				if ((flag14 && flag13) & flag4)
				{
					Rect pos8 = new Rect(vector.x + slotSize - borderSize, vector.y + slotSize, borderSize, borderSize);
					pos8.position += new Vector2(borderOffset, 0f - borderOffset);
					DrawRectangle(uv: new Rect(0.5f, 0.25f, 0.25f, 0.25f), pos: pos8);
				}
				if (flag14 && flag9)
				{
					Rect pos9 = new Rect(vector.x - borderSize, vector.y + slotSize, borderSize, borderSize);
					pos9.position += new Vector2(borderOffset, 0f - borderOffset);
					DrawRectangle(uv: new Rect(0f, 0.75f, 0.25f, 0.25f), pos: pos9);
				}
				if (flag14 && flag12)
				{
					Rect pos10 = new Rect(vector.x + slotSize, vector.y + slotSize, borderSize, borderSize);
					pos10.position += new Vector2(0f - borderOffset, 0f - borderOffset);
					DrawRectangle(uv: new Rect(0.75f, 0.75f, 0.25f, 0.25f), pos: pos10);
				}
				if (flag11 && flag9)
				{
					Rect pos11 = new Rect(vector.x - borderSize, vector.y - borderSize, borderSize, borderSize);
					pos11.position += new Vector2(borderOffset, borderOffset);
					DrawRectangle(uv: new Rect(0f, 0f, 0.25f, 0.25f), pos: pos11);
				}
				if (flag11 && flag12)
				{
					Rect pos12 = new Rect(vector.x + slotSize, vector.y - borderSize, borderSize, borderSize);
					pos12.position += new Vector2(0f - borderOffset, borderOffset);
					DrawRectangle(uv: new Rect(0.75f, 0f, 0.25f, 0.25f), pos: pos12);
				}
			}
			SetBuffersToToMesh();
		}

		public void ShowBorderOnLevel(TilesSlotsProvider slotsProvider)
		{
			if (Application.isEditor)
			{
				cachedProviderDebug = slotsProvider;
			}
			if (dontConnectDiagonalSlots)
			{
				ShowBorderOnLevelDontConnectDiagonalSlots(slotsProvider);
			}
			else
			{
				ShowBorderOnLevelConnectDiagonalSlots(slotsProvider);
			}
		}

		public void ShowBorderOnLevelConnectDiagonalSlots(TilesSlotsProvider slotsProvider)
		{
			Init();
			int maxSlots = slotsProvider.MaxSlots;
			int vertexCount = maxSlots * 8 * 4;
			int trisCount = maxSlots * 8 * 6;
			InitBuffers(vertexCount, trisCount);
			ClearBuffers();
			Vector2 a = slotsProvider.StartPosition(slotSize);
			Vector2 size = new Vector2(slotSize, slotSize);
			new Rect(0f, 0f, 0.5f, 0.5f);
			List<TilesSlotsProvider.Slot> allSlots = slotsProvider.GetAllSlots();
			for (int i = 0; i < allSlots.Count; i++)
			{
				TilesSlotsProvider.Slot slot = allSlots[i];
				if (slot.isEmpty)
				{
					continue;
				}
				IntVector2 position = slot.position;
				Vector2 vector = a + new Vector2((float)position.x * slotSize, (float)position.y * slotSize);
				new Rect(vector, size);
				bool flag = IsEmptySlot(slotsProvider, position + IntVector2.up + IntVector2.left);
				bool flag2 = IsEmptySlot(slotsProvider, position + IntVector2.up + IntVector2.right);
				bool flag3 = !flag;
				bool flag4 = !flag2;
				bool flag5 = IsEmptySlot(slotsProvider, position + IntVector2.down + IntVector2.left);
				bool flag6 = IsEmptySlot(slotsProvider, position + IntVector2.down + IntVector2.right);
				bool flag7 = !flag5;
				bool flag8 = !flag6;
				bool flag9 = IsEmptySlot(slotsProvider, position + IntVector2.left);
				bool flag10 = IsEmptySlot(slotsProvider, position + IntVector2.down);
				bool flag11 = IsEmptySlot(slotsProvider, position + IntVector2.right);
				bool flag12 = IsEmptySlot(slotsProvider, position + IntVector2.up);
				if (flag9)
				{
					Rect pos = new Rect(vector.x - borderSize, vector.y + borderSize, borderSize, slotSize - 2f * borderSize);
					Rect uv = new Rect(0f, 0.25f, 0.25f, 0.5f);
					if (flag5)
					{
						pos.yMin = vector.y;
					}
					if (flag)
					{
						pos.yMax += borderSize;
					}
					if (flag3)
					{
						pos.yMax += borderOffset;
					}
					if (flag7)
					{
						pos.yMin -= borderOffset;
					}
					if (flag10 && flag5)
					{
						pos.yMin += borderOffset;
					}
					if (flag12 && flag)
					{
						pos.yMax -= borderOffset;
					}
					pos.position += Vector2.right * borderOffset;
					DrawRectangle(pos, uv);
				}
				if (flag11)
				{
					Rect pos2 = new Rect(vector.x + slotSize, vector.y + borderSize, borderSize, slotSize - 2f * borderSize);
					Rect uv2 = new Rect(0.75f, 0.25f, 0.25f, 0.5f);
					if (flag6)
					{
						pos2.yMin = vector.y;
					}
					if (flag2)
					{
						pos2.yMax += borderSize;
					}
					if (flag4)
					{
						pos2.yMax += borderOffset;
					}
					if (flag8)
					{
						pos2.yMin -= borderOffset;
					}
					if (flag10 && flag6)
					{
						pos2.yMin += borderOffset;
					}
					if (flag12 && flag2)
					{
						pos2.yMax -= borderOffset;
					}
					pos2.position += Vector2.left * borderOffset;
					DrawRectangle(pos2, uv2);
				}
				if (flag10)
				{
					Rect pos3 = new Rect(vector.x + borderSize, vector.y - borderSize, slotSize - 2f * borderSize, borderSize);
					if (flag5)
					{
						pos3.xMin = vector.x;
					}
					if (flag6)
					{
						pos3.xMax += borderSize;
					}
					if (flag7)
					{
						pos3.xMin -= borderOffset;
					}
					if (flag8)
					{
						pos3.xMax += borderOffset;
					}
					if (flag5 && flag9)
					{
						pos3.xMin += borderOffset;
					}
					if (flag6 && flag11)
					{
						pos3.xMax -= borderOffset;
					}
					pos3.position += Vector2.up * borderOffset;
					DrawRectangle(uv: new Rect(0.25f, 0f, 0.5f, 0.25f), pos: pos3);
				}
				if (flag12)
				{
					Rect pos4 = new Rect(vector.x + borderSize, vector.y + slotSize, slotSize - 2f * borderSize, borderSize);
					if (flag)
					{
						pos4.xMin = vector.x;
					}
					if (flag2)
					{
						pos4.xMax += borderSize;
					}
					if (flag3)
					{
						pos4.xMin -= borderOffset;
					}
					if (flag4)
					{
						pos4.xMax += borderOffset;
					}
					if (flag && flag9)
					{
						pos4.xMin += borderOffset;
					}
					if (flag2 && flag11)
					{
						pos4.xMax -= borderOffset;
					}
					pos4.position += Vector2.down * borderOffset;
					DrawRectangle(uv: new Rect(0.25f, 0.75f, 0.5f, 0.25f), pos: pos4);
				}
				if (flag12 && flag3)
				{
					Rect pos5 = new Rect(vector.x, vector.y + slotSize, borderSize, borderSize);
					pos5.position += new Vector2(0f - borderOffset, 0f - borderOffset);
					DrawRectangle(uv: new Rect(0.25f, 0.25f, 0.25f, 0.25f), pos: pos5);
				}
				if (flag9 && flag3)
				{
					Rect pos6 = new Rect(vector.x - borderSize, vector.y + slotSize - borderSize, borderSize, borderSize);
					pos6.position += new Vector2(borderOffset, borderOffset);
					DrawRectangle(uv: new Rect(0.5f, 0.5f, 0.25f, 0.25f), pos: pos6);
				}
				if (flag11 && flag4)
				{
					Rect pos7 = new Rect(vector.x + slotSize, vector.y + slotSize - borderSize, borderSize, borderSize);
					pos7.position += new Vector2(0f - borderOffset, borderOffset);
					DrawRectangle(uv: new Rect(0.25f, 0.5f, 0.25f, 0.25f), pos: pos7);
				}
				if (flag12 && flag4)
				{
					Rect pos8 = new Rect(vector.x + slotSize - borderSize, vector.y + slotSize, borderSize, borderSize);
					pos8.position += new Vector2(borderOffset, 0f - borderOffset);
					DrawRectangle(uv: new Rect(0.5f, 0.25f, 0.25f, 0.25f), pos: pos8);
				}
				if ((flag12 && flag) & flag9)
				{
					Rect pos9 = new Rect(vector.x - borderSize, vector.y + slotSize, borderSize, borderSize);
					pos9.position += new Vector2(borderOffset, 0f - borderOffset);
					DrawRectangle(uv: new Rect(0f, 0.75f, 0.25f, 0.25f), pos: pos9);
				}
				if ((flag12 && flag2) & flag11)
				{
					Rect pos10 = new Rect(vector.x + slotSize, vector.y + slotSize, borderSize, borderSize);
					pos10.position += new Vector2(0f - borderOffset, 0f - borderOffset);
					DrawRectangle(uv: new Rect(0.75f, 0.75f, 0.25f, 0.25f), pos: pos10);
				}
				if ((flag10 && flag5) & flag9)
				{
					Rect pos11 = new Rect(vector.x - borderSize, vector.y - borderSize, borderSize, borderSize);
					pos11.position += new Vector2(borderOffset, borderOffset);
					DrawRectangle(uv: new Rect(0f, 0f, 0.25f, 0.25f), pos: pos11);
				}
				if ((flag10 && flag6) & flag11)
				{
					Rect pos12 = new Rect(vector.x + slotSize, vector.y - borderSize, borderSize, borderSize);
					pos12.position += new Vector2(0f - borderOffset, borderOffset);
					DrawRectangle(uv: new Rect(0.75f, 0f, 0.25f, 0.25f), pos: pos12);
				}
			}
			SetBuffersToToMesh();
		}

		public void ShowSlotsOnLevel(LevelDefinition level)
		{
			LevelDefinitionTilesSlotsProvider slotsProvider = new LevelDefinitionTilesSlotsProvider(level);
			ShowSlotsOnLevel(slotsProvider);
		}

		public void ShowSlotsOnLevel(TilesSlotsProvider slotsProvider)
		{
			if (Application.isEditor)
			{
				cachedProviderDebug = slotsProvider;
			}
			if (dontConnectDiagonalSlots)
			{
				ShowSlotsOnLevelDontConnectDiagonalSlots(slotsProvider);
			}
			else
			{
				ShowSlotsOnLevelConnectDiagonalSlots(slotsProvider);
			}
		}

		private void ShowSlotsOnLevelDontConnectDiagonalSlots(TilesSlotsProvider slotsProvider)
		{
			Init();
			int maxSlots = slotsProvider.MaxSlots;
			int vertexCount = maxSlots * 4;
			int trisCount = maxSlots * 6;
			InitBuffers(vertexCount, trisCount);
			ClearBuffers();
			Vector2 a = slotsProvider.StartPosition(slotSize);
			Vector2 size = new Vector2(slotSize, slotSize);
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			float num = borderOffset / slotSize;
			List<TilesSlotsProvider.Slot> allSlots = slotsProvider.GetAllSlots();
			for (int i = 0; i < allSlots.Count; i++)
			{
				TilesSlotsProvider.Slot slot = allSlots[i];
				if (slot.isEmpty)
				{
					continue;
				}
				IntVector2 position = slot.position;
				Vector2 position2 = a + new Vector2((float)position.x * slotSize, (float)position.y * slotSize);
				Rect uv = rect;
				Rect pos = new Rect(position2, size);
				bool flag = IsEmptySlot(slotsProvider, position + IntVector2.left);
				bool flag2 = !flag;
				bool flag3 = IsEmptySlot(slotsProvider, position + IntVector2.right);
				bool flag4 = !flag3;
				bool flag5 = IsEmptySlot(slotsProvider, position + IntVector2.up);
				bool flag6 = !flag5;
				bool flag7 = IsEmptySlot(slotsProvider, position + IntVector2.down);
				bool flag8 = !flag7;
				bool num2 = IsEmptySlot(slotsProvider, position + IntVector2.up + IntVector2.left);
				bool flag9 = IsEmptySlot(slotsProvider, position + IntVector2.up + IntVector2.right);
				bool flag10 = !num2;
				bool flag11 = !flag9;
				bool flag12 = IsEmptySlot(slotsProvider, position + IntVector2.down + IntVector2.left);
				bool flag13 = IsEmptySlot(slotsProvider, position + IntVector2.down + IntVector2.right);
				bool flag14 = !flag12;
				bool flag15 = !flag13;
				if ((num2 & flag2 & flag6) || ((flag9 && flag4) & flag6) || ((flag12 && flag2) & flag8) || ((flag13 && flag4) & flag8))
				{
					if (flag2)
					{
						Rect pos2 = new Rect(pos.xMin, pos.yMin + borderOffset, borderOffset, pos.height - 2f * borderOffset);
						Rect uv2 = new Rect(rect.xMin, rect.yMin + num, num, rect.height - 2f * num);
						DrawRectangle(pos2, uv2);
					}
					if (flag4)
					{
						Rect pos3 = new Rect(pos.xMax - borderOffset, pos.yMin + borderOffset, borderOffset, pos.height - 2f * borderOffset);
						Rect uv3 = new Rect(rect.xMax - num, rect.yMin + num, num, rect.height - 2f * num);
						DrawRectangle(pos3, uv3);
					}
					if (flag6)
					{
						Rect pos4 = new Rect(pos.xMin + borderOffset, pos.yMax - borderOffset, pos.width - 2f * borderOffset, borderOffset);
						Rect uv4 = new Rect(rect.xMin + num, rect.yMax - num, rect.width - 2f * num, borderOffset);
						DrawRectangle(pos4, uv4);
					}
					if (flag8)
					{
						Rect pos5 = new Rect(pos.xMin + borderOffset, pos.yMin, pos.width - 2f * borderOffset, borderOffset);
						Rect uv5 = new Rect(rect.xMin + num, rect.yMin, rect.width - 2f * num, borderOffset);
						DrawRectangle(pos5, uv5);
					}
					if ((flag10 && flag6) & flag2)
					{
						Rect pos6 = new Rect(pos.xMin, pos.yMax - borderOffset, borderOffset, borderOffset);
						Rect uv6 = new Rect(rect.xMin, rect.yMax - num, num, num);
						DrawRectangle(pos6, uv6);
					}
					if ((flag14 && flag8) & flag2)
					{
						Rect pos7 = new Rect(pos.xMin, pos.yMin, borderOffset, borderOffset);
						Rect uv7 = new Rect(rect.xMin, rect.yMin, num, num);
						DrawRectangle(pos7, uv7);
					}
					if ((flag11 && flag6) & flag4)
					{
						Rect pos8 = new Rect(pos.xMax - borderOffset, pos.yMax - borderOffset, borderOffset, borderOffset);
						Rect uv8 = new Rect(rect.xMax - num, rect.yMax - num, num, num);
						DrawRectangle(pos8, uv8);
					}
					if ((flag15 && flag8) & flag4)
					{
						Rect pos9 = new Rect(pos.xMax - borderOffset, pos.yMin, borderOffset, borderOffset);
						Rect uv9 = new Rect(rect.xMax - num, rect.yMin, num, num);
						DrawRectangle(pos9, uv9);
					}
					Rect pos10 = new Rect(pos.xMin + borderOffset, pos.yMin + borderOffset, pos.width - 2f * borderOffset, pos.height - 2f * borderOffset);
					Rect uv10 = new Rect(rect.xMin + num, rect.yMin + num, rect.width - 2f * num, rect.height - 2f * num);
					DrawRectangle(pos10, uv10);
				}
				else
				{
					if (flag5)
					{
						pos.yMax -= borderOffset;
						uv.yMax -= num;
					}
					if (flag7)
					{
						pos.yMin += borderOffset;
						uv.yMin += num;
					}
					if (flag)
					{
						pos.xMin += borderOffset;
						uv.xMin += num;
					}
					if (flag3)
					{
						pos.xMax -= borderOffset;
						uv.xMax -= num;
					}
					DrawRectangle(pos, uv);
				}
			}
			SetBuffersToToMesh();
		}

		private void ShowSlotsOnLevelConnectDiagonalSlots(TilesSlotsProvider slotsProvider)
		{
			Init();
			int maxSlots = slotsProvider.MaxSlots;
			int vertexCount = maxSlots * 4;
			int trisCount = maxSlots * 6;
			InitBuffers(vertexCount, trisCount);
			ClearBuffers();
			Vector2 a = slotsProvider.StartPosition(slotSize);
			Vector2 size = new Vector2(slotSize, slotSize);
			Rect uv = new Rect(0f, 0f, 1f, 1f);
			List<TilesSlotsProvider.Slot> allSlots = slotsProvider.GetAllSlots();
			for (int i = 0; i < allSlots.Count; i++)
			{
				TilesSlotsProvider.Slot slot = allSlots[i];
				if (!slot.isEmpty)
				{
					IntVector2 position = slot.position;
					Vector2 position2 = a + new Vector2((float)position.x * slotSize, (float)position.y * slotSize);
					Rect pos = new Rect(position2, size);
					DrawRectangle(pos, uv);
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
