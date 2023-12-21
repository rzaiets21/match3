using UnityEngine;

public class GGFixIslandEdges
{
	private static Material material_;

	public static Material sharedMaterial
	{
		get
		{
			if (material_ == null)
			{
				material_ = GGPaintableShader.Build(GGPaintableShader.Load("GGPaintableTexture/FixIslandEdges"));
			}
			return material_;
		}
	}
}
