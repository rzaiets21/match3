using UnityEngine;

public class GGPSphereCommand
{
	public struct Params
	{
		public Vector3 worldPosition;

		public Color brushColor;

		public float brushSize;

		public float brushHardness;

		public void SetToMaterial(Material material)
		{
			material.SetVector(GGPaintableShader._Position, worldPosition);
			material.SetFloat(GGPaintableShader._BrushSize, brushSize);
			material.SetFloat(GGPaintableShader._BrushHardness, brushHardness);
			material.SetColor(GGPaintableShader._Color, brushColor);
		}
	}

	private static Material material_;

	public static Material sharedMaterial
	{
		get
		{
			if (material_ == null)
			{
				material_ = GGPaintableShader.Build(GGPaintableShader.Load("GGPaintableTexture/Sphere"));
			}
			return material_;
		}
	}
}
