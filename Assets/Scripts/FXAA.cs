using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class FXAA : MonoBehaviour
{
	public Material material;

	public float Sharpness = 4f;

	public float Threshold = 0.2f;

	private static readonly int sharpnessString = Shader.PropertyToID("_Sharpness");

	private static readonly int thresholdString = Shader.PropertyToID("_Threshold");

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetFloat(sharpnessString, Sharpness);
		material.SetFloat(thresholdString, Threshold);
		Graphics.Blit(source, destination, material);
	}
}
