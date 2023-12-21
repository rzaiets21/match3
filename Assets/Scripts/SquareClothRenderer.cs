using GGCloth;
using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SquareClothRenderer : MonoBehaviour
{
	[Serializable]
	public class ClothTexture
	{
		public ItemColor itemColor;

		public Texture2D texture;
	}

	[Serializable]
	public class ShadowSprite
	{
		public ItemColor itemColor;

		public Sprite texture;
	}

	[SerializeField]
	private List<SpriteRenderer> boxSprites = new List<SpriteRenderer>();

	[SerializeField]
	private List<ClothTexture> clothTextures = new List<ClothTexture>();

	[SerializeField]
	private List<ShadowSprite> shadowTextures = new List<ShadowSprite>();

	[SerializeField]
	private SpriteRenderer shadowSprite;

	[SerializeField]
	public int sortingLayerID;

	[SerializeField]
	public int sortingLayerOrder;

	private MeshFilter meshFilter;

	private Mesh mesh;

	[SerializeField]
	private bool invertNormals;

	private List<Vector3> vertexBuffer;

	private List<int> trisBuffer;

	private List<Vector2> uvBuffer;

	[NonSerialized]
	private SquareCloth cloth;

	[NonSerialized]
	public ItemColor itemColor;

	private bool isLocalScaleSaved;

	private Vector3 localScale;

	public void SetCloth(SquareCloth cloth)
	{
		this.cloth = cloth;
		Init();
	}

	public ClothTexture GetClothTexture(ItemColor itemColor)
	{
		this.itemColor = itemColor;
		for (int i = 0; i < clothTextures.Count; i++)
		{
			ClothTexture clothTexture = clothTextures[i];
			if (clothTexture.itemColor == itemColor)
			{
				return clothTexture;
			}
		}
		return null;
	}

	public ShadowSprite GetShadowClothTexture(ItemColor itemColor)
	{
		this.itemColor = itemColor;
		for (int i = 0; i < shadowTextures.Count; i++)
		{
			ShadowSprite shadowSprite = shadowTextures[i];
			if (shadowSprite.itemColor == itemColor)
			{
				return shadowSprite;
			}
		}
		return null;
	}

	public void UpdateMaterialSettings()
	{
		if (!isLocalScaleSaved)
		{
			isLocalScaleSaved = true;
			localScale = base.transform.localScale;
		}
		if (!Application.isEditor && !ConfigBase.instance.changeChipOnDevice)
		{
			return;
		}
		Match3Settings.ChipChange chipChange = Match3Settings.instance.GetChipChange(itemColor);
		if (chipChange == null)
		{
			return;
		}
		Match3Settings.ChipColorSettings colorSettings = Match3Settings.instance.GetColorSettings();
		for (int i = 0; i < boxSprites.Count; i++)
		{
			SpriteRenderer spriteRenderer = boxSprites[i];
			Material material = spriteRenderer.material;
			material.SetFloat("_ColorHSV_Hue_1", chipChange.boxHue);
			material.SetFloat("_ColorHSV_Saturation_1", chipChange.boxSaturation);
			material.SetFloat("_ColorHSV_Brightness_1", chipChange.boxBrightness);
			spriteRenderer.gameObject.SetActive(colorSettings.hasBoxes);
		}
		Material material2 = GetMaterial();
		if (material2 == null)
		{
			return;
		}
		if (!chipChange.change)
		{
			material2.SetFloat("_ColorHSV_Hue_1", 0f);
			material2.SetFloat("_ColorHSV_Saturation_1", 1f);
			material2.SetFloat("_ColorHSV_Brightness_1", 1f);
			return;
		}
		material2.SetFloat("_ColorHSV_Hue_1", chipChange.hue);
		material2.SetFloat("_ColorHSV_Saturation_1", chipChange.saturation);
		material2.SetFloat("_ColorHSV_Brightness_1", chipChange.brightness);
		if (chipChange.textureReplace != null)
		{
			material2.mainTexture = chipChange.textureReplace;
		}
		base.transform.localScale = localScale * chipChange.scale;
	}

	public void SetBrightness(float brightness)
	{
		Material material = GetMaterial();
		if (!(material == null))
		{
			material.SetFloat("_ColorHSV_Brightness_1", brightness);
		}
	}

	public Material GetMaterial()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component == null)
		{
			return null;
		}
		return component.material;
	}

	public void SetColor(Color color)
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (!(component == null))
		{
			component.material.color = color;
		}
	}

	public void SetAlpha(float alpha)
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (!(component == null))
		{
			Material material = component.material;
			Color color = material.color;
			color.a = alpha;
			material.color = color;
		}
	}

	public void SetShadowTexture(ItemColor itemColor)
	{
		ShadowSprite shadowClothTexture = GetShadowClothTexture(itemColor);
		if (shadowClothTexture != null && !(shadowSprite == null))
		{
			shadowSprite.sprite = shadowClothTexture.texture;
		}
	}

	public void SetClothTexture(ItemColor itemColor)
	{
		SetShadowTexture(itemColor);
		ClothTexture clothTexture = GetClothTexture(itemColor);
		if (clothTexture != null)
		{
			MeshRenderer component = GetComponent<MeshRenderer>();
			if (!(component == null))
			{
				component.material.mainTexture = clothTexture.texture;
			}
		}
	}

	public void SetSortingLayers(int sortingLayerId, int sortingLayerOrder)
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (!(component == null))
		{
			component.sortingLayerID = sortingLayerId;
			component.sortingOrder = sortingLayerOrder;
		}
	}

	public void ReinitSortingLayers()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (!(component == null))
		{
			component.sortingLayerID = sortingLayerID;
			component.sortingOrder = sortingLayerOrder;
		}
	}

	private void Init()
	{
		if (meshFilter == null)
		{
			meshFilter = base.gameObject.GetComponent<MeshFilter>();
			ReinitSortingLayers();
		}
		if (meshFilter == null)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		if (mesh == null)
		{
			mesh = new Mesh();
			meshFilter.mesh = mesh;
		}
		int rowCount = cloth.rowCount;
		int columnCount = cloth.columnCount;
		int capacity = rowCount * columnCount * 2;
		int capacity2 = (rowCount + 1) * (columnCount + 1);
		vertexBuffer = new List<Vector3>(capacity2);
		trisBuffer = new List<int>(capacity);
		uvBuffer = new List<Vector2>(capacity2);
	}

	private Vector3 Min(Vector3 a, Vector3 b)
	{
		return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
	}

	private Vector3 Max(Vector3 a, Vector3 b)
	{
		return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
	}

	public void DoUpdateMesh()
	{
		if (cloth == null || mesh == null)
		{
			return;
		}
		vertexBuffer.Clear();
		trisBuffer.Clear();
		uvBuffer.Clear();
		int rowCount = cloth.rowCount;
		int columnCount = cloth.columnCount;
		PointWorld pointWorld = cloth.pointWorld;
		PointMass point = pointWorld.GetPoint(0);
		Vector3 vector = point.currentPosition;
		Vector3 vector2 = point.currentPosition;
		for (int i = 0; i <= rowCount; i++)
		{
			for (int j = 0; j <= columnCount; j++)
			{
				Vector3 vector3 = pointWorld.GetPoint(cloth.GetPointIndex(j, i)).currentPosition;
				if (cloth.isWorldPosition)
				{
					vector3 -= base.transform.position;
				}
				else if (cloth.localPositionTransform != null)
				{
					vector3 -= cloth.localPositionTransform.localPosition;
				}
				vector = Min(vector, vector3);
				vector2 = Max(vector2, vector3);
				vertexBuffer.Add(vector3);
				Vector2 item = new Vector2(Mathf.InverseLerp(0f, columnCount, j), Mathf.InverseLerp(0f, rowCount, i));
				uvBuffer.Add(item);
				if (j < columnCount && i < rowCount)
				{
					if (!invertNormals)
					{
						trisBuffer.Add(cloth.GetPointIndex(j, i));
						trisBuffer.Add(cloth.GetPointIndex(j, i + 1));
						trisBuffer.Add(cloth.GetPointIndex(j + 1, i));
						trisBuffer.Add(cloth.GetPointIndex(j + 1, i));
						trisBuffer.Add(cloth.GetPointIndex(j, i + 1));
						trisBuffer.Add(cloth.GetPointIndex(j + 1, i + 1));
					}
					else
					{
						trisBuffer.Add(cloth.GetPointIndex(j, i));
						trisBuffer.Add(cloth.GetPointIndex(j + 1, i));
						trisBuffer.Add(cloth.GetPointIndex(j, i + 1));
						trisBuffer.Add(cloth.GetPointIndex(j + 1, i));
						trisBuffer.Add(cloth.GetPointIndex(j + 1, i + 1));
						trisBuffer.Add(cloth.GetPointIndex(j, i + 1));
					}
				}
			}
		}
		mesh.bounds.SetMinMax(vector, vector2);
		mesh.Clear();
		mesh.SetVertices(vertexBuffer);
		mesh.SetUVs(0, uvBuffer);
		mesh.SetTriangles(trisBuffer, 0, calculateBounds: false);
	}
}
