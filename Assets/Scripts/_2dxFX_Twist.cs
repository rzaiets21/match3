using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class _2dxFX_Twist : MonoBehaviour
{
	public Material ForceMaterial;

	public bool ActiveChange = true;

	private string shader = "2DxFX/Standard/Twist";

	public float _Alpha = 1f;

	public float Distortion = 1.6f;

	public float _PosX = 0.5f;

	public float _PosY = 0.5f;

	public Color _ColorX = new Color(1f, 1f, 1f, 1f);

	public int ShaderChange;

	private Material tempMaterial;

	private Material defaultMaterial;

	private Image CanvasImage;

	private SpriteRenderer CanvasSpriteRenderer;

	public bool ActiveUpdate = true;

	private void Awake()
	{
		if (base.gameObject.GetComponent<Image>() != null)
		{
			CanvasImage = base.gameObject.GetComponent<Image>();
		}
		if (base.gameObject.GetComponent<SpriteRenderer>() != null)
		{
			CanvasSpriteRenderer = base.gameObject.GetComponent<SpriteRenderer>();
		}
	}

	private void Start()
	{
		ShaderChange = 0;
		XUpdate();
	}

	public void CallUpdate()
	{
		XUpdate();
	}

	private void Update()
	{
		if (ActiveUpdate)
		{
			XUpdate();
		}
	}

	private void XUpdate()
	{
		if (CanvasImage == null && base.gameObject.GetComponent<Image>() != null)
		{
			CanvasImage = base.gameObject.GetComponent<Image>();
		}
		if (CanvasSpriteRenderer == null && base.gameObject.GetComponent<SpriteRenderer>() != null)
		{
			CanvasSpriteRenderer = base.gameObject.GetComponent<SpriteRenderer>();
		}
		if (ShaderChange == 0 && ForceMaterial != null)
		{
			ShaderChange = 1;
			if (tempMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(tempMaterial);
			}
			if (CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial = ForceMaterial;
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material = ForceMaterial;
			}
			ForceMaterial.hideFlags = HideFlags.None;
			ForceMaterial.shader = Shader.Find(shader);
		}
		if (ForceMaterial == null && ShaderChange == 1)
		{
			if (tempMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(tempMaterial);
			}
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			if (CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial = tempMaterial;
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material = tempMaterial;
			}
			ShaderChange = 0;
		}
		if (ActiveChange)
		{
			if (CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_Alpha", 1f - _Alpha);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_Distortion", Distortion);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_PosX", _PosX);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_PosY", _PosY);
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material.SetFloat("_Alpha", 1f - _Alpha);
				CanvasImage.material.SetFloat("_Distortion", Distortion);
				CanvasImage.material.SetFloat("_PosX", _PosX);
				CanvasImage.material.SetFloat("_PosY", _PosY);
			}
		}
	}

	private void OnDestroy()
	{
		if (Application.isPlaying || !Application.isEditor)
		{
			return;
		}
		if (tempMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(tempMaterial);
		}
		if (base.gameObject.activeSelf && defaultMaterial != null)
		{
			if (CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial = defaultMaterial;
				CanvasSpriteRenderer.sharedMaterial.hideFlags = HideFlags.None;
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material = defaultMaterial;
				CanvasImage.material.hideFlags = HideFlags.None;
			}
		}
	}

	private void OnDisable()
	{
		if (base.gameObject.activeSelf && defaultMaterial != null)
		{
			if (CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial = defaultMaterial;
				CanvasSpriteRenderer.sharedMaterial.hideFlags = HideFlags.None;
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material = defaultMaterial;
				CanvasImage.material.hideFlags = HideFlags.None;
			}
		}
	}

	private void OnEnable()
	{
		if (defaultMaterial == null)
		{
			defaultMaterial = new Material(Shader.Find("Sprites/Default"));
		}
		if (ForceMaterial == null)
		{
			ActiveChange = true;
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			if (CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial = tempMaterial;
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material = tempMaterial;
			}
		}
		else
		{
			ForceMaterial.shader = Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			if (CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial = ForceMaterial;
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material = ForceMaterial;
			}
		}
	}
}
