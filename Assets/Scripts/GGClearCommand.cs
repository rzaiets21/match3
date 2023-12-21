using UnityEngine;

public class GGClearCommand
{
	private static Material material_;

	public static Material sharedMaterial
	{
		get
		{
			if (material_ == null)
			{
				material_ = GGPaintableShader.Build(GGPaintableShader.Load("GGPaintableTexture/Clear"));
			}
			return material_;
		}
	}
}
