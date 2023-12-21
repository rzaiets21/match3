using UnityEngine;

public class GGCountCommand
{
	private static Material material_;

	public static Material sharedMaterial
	{
		get
		{
			if (material_ == null)
			{
				material_ = GGPaintableShader.Build(GGPaintableShader.Load("GGPaintableTexture/Count"));
			}
			return material_;
		}
	}

	public static Texture2D GetReadableCopy(RenderTexture renderTexture, TextureFormat format = TextureFormat.ARGB32, bool mipMaps = false)
	{
		if (renderTexture != null)
		{
			Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, format, mipMaps, QualitySettings.activeColorSpace == ColorSpace.Linear);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
			RenderTexture.active = active;
			texture2D.Apply();
			return texture2D;
		}
		return null;
	}
}
