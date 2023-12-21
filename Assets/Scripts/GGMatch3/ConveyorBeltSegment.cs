using UnityEngine;

namespace GGMatch3
{
	public class ConveyorBeltSegment : MonoBehaviour
	{
		[SerializeField]
		private TiledSpriteRenderer tiledSegment;

		[SerializeField]
		private SpriteRenderer start;

		[SerializeField]
		private SpriteRenderer end;

		public void SetTile(float tile)
		{
			tiledSegment.meshRenderer.sharedMaterial.mainTextureOffset = new Vector2(0f, 0f - tile);
		}

		public void SetColor(Color color)
		{
			tiledSegment.meshRenderer.sharedMaterial.color = color;
		}

		public Color GetColor()
		{
			return tiledSegment.meshRenderer.sharedMaterial.color;
		}

		public void Init(int numSlots, Vector2 slotSize)
		{
			float num = slotSize.x * 0.5f;
			float num2 = slotSize.y * 0.5f;
			tiledSegment.ClearAndInit(1);
			Rect pos = default(Rect);
			pos.xMin = 0f - num;
			pos.xMax = num;
			pos.yMin = 0f - num2;
			pos.yMax = ((float)numSlots - 0.5f) * slotSize.y;
			Rect uv = new Rect(0f, 0f, 1f, numSlots);
			tiledSegment.DrawRectangle(pos, uv);
			tiledSegment.CopyToMesh();
			if (start != null)
			{
				SetScaleToMatch(start);
				Bounds bounds = start.bounds;
				start.transform.localPosition = new Vector3(0f, 0f - num2 + bounds.extents.y, 0f);
			}
			if (end != null)
			{
				SetScaleToMatch(end);
				Bounds bounds2 = end.bounds;
				end.transform.localPosition = new Vector3(0f, (float)numSlots * slotSize.y - bounds2.extents.y, 0f);
			}
		}

		private void SetScaleToMatch(SpriteRenderer sprite)
		{
			float num = 1f / sprite.bounds.extents.x;
			sprite.transform.localScale = Vector3.Scale(sprite.transform.localScale, new Vector3(num, num, 1f));
		}
	}
}
