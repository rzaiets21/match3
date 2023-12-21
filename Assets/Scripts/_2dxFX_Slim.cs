using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class _2dxFX_Slim : MonoBehaviour
{
	public Material ForceMaterial;

	public bool ActiveChange = true;

	private string shader = "2DxFX/Standard/Slim";

	public float _Alpha = 1f;

	public Texture2D __MainTex2;

	public float TurnToLiquid = 0.052f;

	public float Heat = 1f;

	public float Speed = 1f;

	public float EValue = 1f;

	public float Light = 3f;

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
		__MainTex2 = (Resources.Load("_2dxFX_WaterTXT") as Texture2D);
		ShaderChange = 0;
		if (CanvasSpriteRenderer != null)
		{
			CanvasSpriteRenderer.sharedMaterial.SetTexture("_MainTex2", __MainTex2);
		}
		else if (CanvasImage != null)
		{
			CanvasImage.material.SetTexture("_MainTex2", __MainTex2);
		}
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
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_Distortion", Heat);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_Speed", Speed);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("EValue", EValue);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("Light", Light);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("TurnToLiquid", TurnToLiquid);
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material.SetFloat("_Alpha", 1f - _Alpha);
				CanvasImage.material.SetFloat("_Distortion", Heat);
				CanvasImage.material.SetFloat("_Speed", Speed);
				CanvasImage.material.SetFloat("EValue", EValue);
				CanvasImage.material.SetFloat("Light", Light);
				CanvasImage.material.SetFloat("TurnToLiquid", TurnToLiquid);
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
			__MainTex2 = (Resources.Load("_2dxFX_WaterTXT") as Texture2D);
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
			__MainTex2 = (Resources.Load("_2dxFX_WaterTXT") as Texture2D);
		}
		if ((bool)__MainTex2)
		{
			__MainTex2.wrapMode = TextureWrapMode.Repeat;
			if (CanvasSpriteRenderer != null)
			{
				CanvasSpriteRenderer.sharedMaterial.SetTexture("_MainTex2", __MainTex2);
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material.SetTexture("_MainTex2", __MainTex2);
			}
		}
	}
}
