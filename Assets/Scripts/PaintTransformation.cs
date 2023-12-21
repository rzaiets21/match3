using System.Collections.Generic;
using UnityEngine;

public class PaintTransformation : MonoBehaviour
{
	[SerializeField]
	public List<GGPaintableTexture> paintableTextures = new List<GGPaintableTexture>();

	public void Init()
	{
	}

	public void ClearTexturesToColor(Color color)
	{
		for (int i = 0; i < paintableTextures.Count; i++)
		{
			GGPaintableTexture gGPaintableTexture = paintableTextures[i];
			gGPaintableTexture.ClearToColor(color);
			gGPaintableTexture.ApplyRenderTextureToMaterials();
		}
	}

	public void RenderSphere(GGPSphereCommand.Params sphereParams)
	{
		for (int i = 0; i < paintableTextures.Count; i++)
		{
			paintableTextures[i].RenderSphere(sphereParams);
		}
	}

	public float FillPercent()
	{
		float num = 0f;
		if (paintableTextures.Count == 0)
		{
			return 1f;
		}
		for (int i = 0; i < paintableTextures.Count; i++)
		{
			GGPaintableTexture gGPaintableTexture = paintableTextures[i];
			num += gGPaintableTexture.PaintInPercentage();
		}
		return num / (float)paintableTextures.Count;
	}

	public void ReleaseAll()
	{
		for (int i = 0; i < paintableTextures.Count; i++)
		{
			GGPaintableTexture gGPaintableTexture = paintableTextures[i];
			gGPaintableTexture.RemoveRenderTextureFromMaterials();
			gGPaintableTexture.ReleaseRenderTexture();
		}
	}
}
