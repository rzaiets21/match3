using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	[RequireComponent(typeof(MeshFilter))]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(MeshRenderer))]
	public class TilesRenderer : MonoBehaviour
	{
		[SerializeField]
		private float slotSize = 1f;

		[SerializeField]
		private float borderSize = 0.25f;

		public int sortingLayerId;

		public int sortingOrder;

		private Mesh mesh;

		private MeshFilter _meshFilter;

		private MeshRenderer _meshRenderer;

		private List<Vector3> vertexBuffer;

		private List<int> trisBuffer;

		private List<Vector2> uvBuffer;

		private List<Vector2> uv1Buffer;

		private List<Vector2> uv2Buffer;

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

		private void InitBuffers(int maxSlots)
		{
			int capacity = maxSlots * 9 * 4;
			int capacity2 = maxSlots * 9 * 6;
			if (vertexBuffer == null)
			{
				vertexBuffer = new List<Vector3>(capacity);
				uvBuffer = new List<Vector2>(capacity);
				uv1Buffer = new List<Vector2>(capacity);
				uv2Buffer = new List<Vector2>(capacity);
				trisBuffer = new List<int>(capacity2);
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

		private Rect SolidRectMask()
		{
			Rect result = default(Rect);
			result.center = new Vector2(0.21f, 0.21f);
			result.size = new Vector2(0.125f, 0.125f) * 0.05f;
			return result;
		}

		public static bool IsEmptySlot(LevelDefinition level, IntVector2 position)
		{
			LevelDefinition.SlotDefinition slot = level.GetSlot(position);
			if (slot == null)
			{
				return true;
			}
			return slot.slotType == SlotType.Empty;
		}

		public static bool IsOccupiedSlot(LevelDefinition level, IntVector2 position)
		{
			LevelDefinition.SlotDefinition slot = level.GetSlot(position);
			if (slot == null)
			{
				return false;
			}
			return slot.slotType == SlotType.PlayingSpace;
		}

		public void ShowLevel()
		{
			LevelDefinition levelDefinition = ScriptableObjectSingleton<LevelDB>.instance.levels[0];
			Init();
			InitBuffers(levelDefinition.size.width * levelDefinition.size.height);
			vertexBuffer.Clear();
			trisBuffer.Clear();
			uvBuffer.Clear();
			uv1Buffer.Clear();
			uv2Buffer.Clear();
			Vector2 a = new Vector2((float)(-levelDefinition.size.width) * slotSize * 0.5f, (float)(-levelDefinition.size.height) * slotSize * 0.5f);
			Rect rect = SolidRectMask();
			Vector2 size = new Vector2(slotSize, slotSize);
			Rect uv = new Rect(0f, 0f, 0.5f, 0.5f);
			Rect uv2 = new Rect(0.05f, 0.05f, 0.1f, 0.1f);
			List<LevelDefinition.SlotDefinition> slots = levelDefinition.slots;
			for (int i = 0; i < slots.Count; i++)
			{
				LevelDefinition.SlotDefinition slotDefinition = slots[i];
				if (slotDefinition.slotType == SlotType.Empty)
				{
					continue;
				}
				IntVector2 position = slotDefinition.position;
				Vector2 vector = a + new Vector2((float)position.x * slotSize, (float)position.y * slotSize);
				Rect pos = new Rect(vector, size);
				DrawRectangle(pos, uv, rect, rect);
				bool flag = IsEmptySlot(levelDefinition, position + IntVector2.up + IntVector2.left);
				bool flag2 = IsEmptySlot(levelDefinition, position + IntVector2.up + IntVector2.right);
				bool flag3 = !flag;
				bool flag4 = !flag2;
				bool flag5 = IsEmptySlot(levelDefinition, position + IntVector2.down + IntVector2.left);
				bool flag6 = IsEmptySlot(levelDefinition, position + IntVector2.down + IntVector2.right);
				bool flag7 = IsEmptySlot(levelDefinition, position + IntVector2.left);
				bool num = IsEmptySlot(levelDefinition, position + IntVector2.down);
				bool flag8 = IsEmptySlot(levelDefinition, position + IntVector2.right);
				bool flag9 = IsEmptySlot(levelDefinition, position + IntVector2.up);
				if (flag7)
				{
					Rect pos2 = new Rect(vector.x - borderSize, vector.y + borderSize, borderSize, slotSize - 2f * borderSize);
					Rect rect2 = new Rect(0f, 0.25f, 0.25f, 0.5f);
					if (flag5)
					{
						pos2.yMin = vector.y;
					}
					if (flag)
					{
						pos2.yMax += borderSize;
					}
					DrawRectangle(pos2, uv2, rect2, rect2);
				}
				if (flag8)
				{
					Rect pos3 = new Rect(vector.x + slotSize, vector.y + borderSize, borderSize, slotSize - 2f * borderSize);
					Rect rect3 = new Rect(0.75f, 0.25f, 0.25f, 0.5f);
					if (flag6)
					{
						pos3.yMin = vector.y;
					}
					if (flag2)
					{
						pos3.yMax += borderSize;
					}
					DrawRectangle(pos3, uv2, rect3, rect3);
				}
				if (num)
				{
					Rect pos4 = new Rect(vector.x + borderSize, vector.y - borderSize, slotSize - 2f * borderSize, borderSize);
					if (flag5)
					{
						pos4.xMin = vector.x;
					}
					if (flag6)
					{
						pos4.xMax += borderSize;
					}
					Rect rect4 = new Rect(0.25f, 0f, 0.5f, 0.25f);
					DrawRectangle(pos4, uv2, rect4, rect4);
				}
				if (flag9)
				{
					Rect pos5 = new Rect(vector.x + borderSize, vector.y + slotSize, slotSize - 2f * borderSize, borderSize);
					if (flag)
					{
						pos5.xMin = vector.x;
					}
					if (flag2)
					{
						pos5.xMax += borderSize;
					}
					Rect rect5 = new Rect(0.25f, 0.75f, 0.5f, 0.25f);
					DrawRectangle(pos5, uv2, rect5, rect5);
				}
				if (flag9 && flag3)
				{
					Rect pos6 = new Rect(vector.x, vector.y + slotSize, borderSize, borderSize);
					Rect rect6 = new Rect(0.25f, 0.25f, 0.25f, 0.25f);
					DrawRectangle(pos6, uv2, rect6, rect6);
				}
				if (flag7 && flag3)
				{
					Rect pos7 = new Rect(vector.x - borderSize, vector.y + slotSize - borderSize, borderSize, borderSize);
					Rect rect7 = new Rect(0.5f, 0.5f, 0.25f, 0.25f);
					DrawRectangle(pos7, uv2, rect7, rect7);
				}
				if (flag8 && flag4)
				{
					Rect pos8 = new Rect(vector.x + slotSize, vector.y + slotSize - borderSize, borderSize, borderSize);
					Rect rect8 = new Rect(0.25f, 0.5f, 0.25f, 0.25f);
					DrawRectangle(pos8, uv2, rect8, rect8);
				}
				if (flag9 && flag4)
				{
					Rect pos9 = new Rect(vector.x + slotSize - borderSize, vector.y + slotSize, borderSize, borderSize);
					Rect rect9 = new Rect(0.5f, 0.25f, 0.25f, 0.25f);
					DrawRectangle(pos9, uv2, rect9, rect9);
				}
				if ((flag9 && flag) & flag7)
				{
					Rect pos10 = new Rect(vector.x - borderSize, vector.y + slotSize, borderSize, borderSize);
					Rect rect10 = new Rect(0f, 0.75f, 0.25f, 0.25f);
					DrawRectangle(pos10, uv2, rect10, rect10);
				}
				if ((flag9 && flag2) & flag8)
				{
					Rect pos11 = new Rect(vector.x + slotSize, vector.y + slotSize, borderSize, borderSize);
					Rect rect11 = new Rect(0.75f, 0.75f, 0.25f, 0.25f);
					DrawRectangle(pos11, uv2, rect11, rect11);
				}
				if (num & flag5 & flag7)
				{
					Rect pos12 = new Rect(vector.x - borderSize, vector.y - borderSize, borderSize, borderSize);
					Rect rect12 = new Rect(0f, 0f, 0.25f, 0.25f);
					DrawRectangle(pos12, uv2, rect12, rect12);
				}
				if (num & flag6 & flag8)
				{
					Rect pos13 = new Rect(vector.x + slotSize, vector.y - borderSize, borderSize, borderSize);
					Rect rect13 = new Rect(0.75f, 0f, 0.25f, 0.25f);
					DrawRectangle(pos13, uv2, rect13, rect13);
				}
			}
			mesh.Clear();
			mesh.SetVertices(vertexBuffer);
			mesh.SetUVs(0, uvBuffer);
			mesh.SetUVs(1, uv1Buffer);
			mesh.SetTriangles(trisBuffer, 0, calculateBounds: true);
		}

		private void DrawRectangle(Rect pos, Rect uv, Rect uv1, Rect uv2)
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
			uv1Buffer.Add(new Vector2(uv1.xMin, uv1.yMin));
			uv1Buffer.Add(new Vector2(uv1.xMax, uv1.yMin));
			uv1Buffer.Add(new Vector2(uv1.xMax, uv1.yMax));
			uv1Buffer.Add(new Vector2(uv1.xMin, uv1.yMax));
			uv2Buffer.Add(new Vector2(uv2.xMin, uv2.yMin));
			uv2Buffer.Add(new Vector2(uv2.xMax, uv2.yMin));
			uv2Buffer.Add(new Vector2(uv2.xMax, uv2.yMax));
			uv2Buffer.Add(new Vector2(uv2.xMin, uv2.yMax));
			trisBuffer.Add(count);
			trisBuffer.Add(count + 2);
			trisBuffer.Add(count + 1);
			trisBuffer.Add(count + 2);
			trisBuffer.Add(count);
			trisBuffer.Add(count + 3);
		}
	}
}
