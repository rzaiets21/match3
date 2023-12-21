using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GGPaintableTexture : MonoBehaviour
{
	[SerializeField]
	private IntVector2 imageSize = new IntVector2(128, 128);

	[SerializeField]
	private IntVector2 countImageSize = new IntVector2(64, 64);

	[SerializeField]
	private RenderTextureFormat format;

	[SerializeField]
	private List<GGPaintableMesh> meshes = new List<GGPaintableMesh>();

	[NonSerialized]
	private RenderTexture renderTexture_;

	[NonSerialized]
	private RenderTexture islandMapTexture_;

	[NonSerialized]
	private BufferTexture bufferTexture = new BufferTexture();

	private RenderTexture renderTexture
	{
		get
		{
			if (renderTexture_ == null)
			{
				RenderTextureDescriptor desc = new RenderTextureDescriptor(imageSize.x, imageSize.y, format, 0);
				renderTexture_ = new RenderTexture(desc);
			}
			return renderTexture_;
		}
	}

	private RenderTexture islandMapTexture
	{
		get
		{
			if (islandMapTexture_ == null)
			{
				RenderTextureDescriptor desc = new RenderTextureDescriptor(imageSize.x, imageSize.y, format, 0);
				islandMapTexture_ = new RenderTexture(desc);
			}
			return islandMapTexture_;
		}
	}

	public void ReleaseRenderTexture()
	{
		if (islandMapTexture_ != null)
		{
			islandMapTexture_.Release();
			islandMapTexture_ = null;
		}
		if (renderTexture_ != null)
		{
			renderTexture_.Release();
			renderTexture_ = null;
		}
	}

	public void RemoveRenderTextureFromMaterials()
	{
		for (int i = 0; i < meshes.Count; i++)
		{
			Material[] materials = meshes[i].meshRenderer.materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].SetTexture("_LayerLerp", null);
			}
		}
	}

	public void ApplyRenderTextureToMaterials()
	{
		RenderTexture renderTexture = this.renderTexture;
		for (int i = 0; i < meshes.Count; i++)
		{
			Material[] materials = meshes[i].meshRenderer.materials;
			foreach (Material obj in materials)
			{
				obj.SetTexture("_LayerLerp", renderTexture);
				obj.SetFloat("_LayerLerpSlider", 1f);
			}
		}
	}

	private void OnDestroy()
	{
		ReleaseRenderTexture();
	}

	public void ClearToColor(Color color)
	{
		RenderTexture renderTexture = this.renderTexture;
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		GL.Clear(clearDepth: true, clearColor: true, color);
		RenderTexture.active = active;
		CreateIslandMap();
	}

	public void CreateIslandMap()
	{
		RenderTexture islandMapTexture = this.islandMapTexture;
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = islandMapTexture;
		GL.Clear(clearDepth: true, clearColor: true, new Color(0f, 0f, 0f, 0f));
		Material sharedMaterial = GGMarkIslands.sharedMaterial;
		sharedMaterial.SetColor(GGPaintableShader._Color, Color.red);
		for (int i = 0; i < meshes.Count; i++)
		{
			GGPaintableMesh gGPaintableMesh = meshes[i];
			if (!gGPaintableMesh.ignoreForRendering)
			{
				Mesh cachedMesh = gGPaintableMesh.cachedMesh;
				Matrix4x4 localToWorldMatrix = gGPaintableMesh.transform.localToWorldMatrix;
				sharedMaterial.SetVector(GGPaintableShader._Channel, GGPaintableShader.IndexToVector(gGPaintableMesh.uvIndex));
				for (int j = 0; j < cachedMesh.subMeshCount; j++)
				{
					int materialIndex = j;
					sharedMaterial.SetPass(0);
					Graphics.DrawMeshNow(cachedMesh, localToWorldMatrix, materialIndex);
				}
			}
		}
		RenderTexture.active = active;
	}

	public float PaintInPercentage()
	{
		RenderTexture renderTexture = this.renderTexture;
		RenderTextureDescriptor descriptor = renderTexture.descriptor;
		descriptor.width = countImageSize.x;
		descriptor.height = countImageSize.y;
		RenderTexture temporary = RenderTexture.GetTemporary(descriptor);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		GL.Clear(clearDepth: true, clearColor: true, Color.black);
		Material sharedMaterial = GGCountCommand.sharedMaterial;
		sharedMaterial.SetTexture(GGPaintableShader._Texture, renderTexture);
		for (int i = 0; i < meshes.Count; i++)
		{
			GGPaintableMesh gGPaintableMesh = meshes[i];
			if (!gGPaintableMesh.ignoreForRendering)
			{
				Mesh cachedMesh = gGPaintableMesh.cachedMesh;
				Matrix4x4 localToWorldMatrix = gGPaintableMesh.transform.localToWorldMatrix;
				sharedMaterial.SetVector(GGPaintableShader._Channel, GGPaintableShader.IndexToVector(gGPaintableMesh.uvIndex));
				for (int j = 0; j < cachedMesh.subMeshCount; j++)
				{
					int materialIndex = j;
					sharedMaterial.SetPass(0);
					Graphics.DrawMeshNow(cachedMesh, localToWorldMatrix, materialIndex);
				}
			}
		}
		RenderTexture.active = active;
		Texture2D readableCopy = GGCountCommand.GetReadableCopy(temporary);
		Color32[] pixels = readableCopy.GetPixels32();
		UnityEngine.Object.Destroy(readableCopy);
		RenderTexture.ReleaseTemporary(temporary);
		int num = 0;
		int num2 = 0;
		foreach (Color32 color in pixels)
		{
			if (color.g >= 128)
			{
				num++;
				if (color.r > 8)
				{
					num2++;
				}
			}
		}
		return Mathf.InverseLerp(0f, num, num2);
	}

	public void RenderSphere(GGPSphereCommand.Params sphereParams)
	{
		RenderTexture renderTexture = this.renderTexture;
		RenderTexture active = RenderTexture.active;
		RenderTexture renderTexture2 = bufferTexture.CopyToBuffer(renderTexture);
		RenderTexture.active = renderTexture;
		Material sharedMaterial = GGPSphereCommand.sharedMaterial;
		sphereParams.SetToMaterial(sharedMaterial);
		sharedMaterial.SetTexture(GGPaintableShader._Texture, renderTexture2);
		renderTexture.DiscardContents();
		for (int i = 0; i < meshes.Count; i++)
		{
			GGPaintableMesh gGPaintableMesh = meshes[i];
			if (!gGPaintableMesh.ignoreForRendering)
			{
				Mesh cachedMesh = gGPaintableMesh.cachedMesh;
				Matrix4x4 localToWorldMatrix = gGPaintableMesh.transform.localToWorldMatrix;
				sharedMaterial.SetVector(GGPaintableShader._Channel, GGPaintableShader.IndexToVector(gGPaintableMesh.uvIndex));
				for (int j = 0; j < cachedMesh.subMeshCount; j++)
				{
					int materialIndex = j;
					sharedMaterial.SetPass(0);
					Graphics.DrawMeshNow(cachedMesh, localToWorldMatrix, materialIndex);
				}
				Material[] materials = gGPaintableMesh.meshRenderer.materials;
				foreach (Material obj in materials)
				{
					obj.SetTexture("_LayerLerp", renderTexture);
					obj.SetFloat("_LayerLerpSlider", 1f);
				}
			}
		}
		GGGraphics.CopyTexture(renderTexture, renderTexture2);
		Material sharedMaterial2 = GGFixIslandEdges.sharedMaterial;
		sharedMaterial2.SetTexture(GGPaintableShader._MainTex, renderTexture2);
		sharedMaterial2.SetTexture(GGPaintableShader._IlsandMap, islandMapTexture);
		sharedMaterial2.SetVector(GGPaintableShader._TexelSize, new Vector4(1f / (float)renderTexture2.width, 1f / (float)renderTexture2.height, renderTexture2.width, renderTexture2.height));
		sharedMaterial2.SetPass(0);
		Graphics.Blit(null, renderTexture, sharedMaterial2);
		RenderTexture.active = active;
		bufferTexture.ReleaseBuffer(renderTexture2);
	}
}
