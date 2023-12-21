using UnityEngine;

public class GGMarkIslands
{
	private static Material material_;

	public static Material sharedMaterial
	{
		get
		{
			if (material_ == null)
			{
				material_ = GGPaintableShader.Build(GGPaintableShader.Load("GGPaintableTexture/MarkIslands"));
			}
			return material_;
		}
	}
}
