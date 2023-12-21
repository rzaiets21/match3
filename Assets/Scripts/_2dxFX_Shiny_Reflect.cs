using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class _2dxFX_Shiny_Reflect : MonoBehaviour
{
	public Material ForceMaterial;

	public bool ActiveChange = true;

	public Texture2D __MainTex2;

	private string shader = "2DxFX/Standard/Shiny_Reflect";

	public float _Alpha = 1f;

	public float Light = 1f;

	public float LightSize = 0.5f;

	public bool UseShinyCurve = true;

	public AnimationCurve ShinyLightCurve;

	public float AnimationSpeedReduction = 3f;

	public float Intensity = 1f;

	public float OnlyLight;

	public float LightBump = 0.05f;

	private float ShinyLightCurveTime;

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
		__MainTex2 = (Resources.Load("_2dxFX_Gradient") as Texture2D);
		ShaderChange = 0;
		if (CanvasSpriteRenderer != null)
		{
			CanvasSpriteRenderer.sharedMaterial.SetTexture("_MainTex2", __MainTex2);
		}
		else if (CanvasImage != null)
		{
			CanvasImage.material.SetTexture("_MainTex2", __MainTex2);
		}
		if (ShinyLightCurve == null)
		{
			ShinyLightCurve = new AnimationCurve();
		}
		if (ShinyLightCurve.length == 0)
		{
			ShinyLightCurve.AddKey(7.780734E-06f, -0.4416301f);
			ShinyLightCurve.keys[0].inTangent = 0f;
			ShinyLightCurve.keys[0].outTangent = 0f;
			ShinyLightCurve.AddKey(0.4310643f, 1.113406f);
			ShinyLightCurve.keys[1].inTangent = 0.2280953f;
			ShinyLightCurve.keys[1].outTangent = 0.2280953f;
			ShinyLightCurve.AddKey(0.5258899f, 1.229086f);
			ShinyLightCurve.keys[2].inTangent = -0.1474274f;
			ShinyLightCurve.keys[2].outTangent = -0.1474274f;
			ShinyLightCurve.AddKey(0.6136486f, 1.113075f);
			ShinyLightCurve.keys[3].inTangent = 0.005268873f;
			ShinyLightCurve.keys[3].outTangent = 0.005268873f;
			ShinyLightCurve.AddKey(0.9367767f, -0.4775873f);
			ShinyLightCurve.keys[4].inTangent = -3.890693f;
			ShinyLightCurve.keys[4].outTangent = -3.890693f;
			ShinyLightCurve.AddKey(1.144408f, -0.4976555f);
			ShinyLightCurve.keys[5].inTangent = 0f;
			ShinyLightCurve.keys[5].outTangent = 0f;
			ShinyLightCurve.postWrapMode = WrapMode.Loop;
			ShinyLightCurve.preWrapMode = WrapMode.Loop;
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
		if (!ActiveChange)
		{
			return;
		}
		if (CanvasSpriteRenderer != null)
		{
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_Alpha", 1f - _Alpha);
			if (UseShinyCurve)
			{
				if (ShinyLightCurve != null)
				{
					CanvasSpriteRenderer.sharedMaterial.SetFloat("_Distortion", ShinyLightCurve.Evaluate(ShinyLightCurveTime));
				}
				ShinyLightCurveTime += Time.deltaTime / 8f * AnimationSpeedReduction;
			}
			else
			{
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_Distortion", Light);
			}
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_Value2", LightSize);
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_Value3", Intensity);
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_Value4", OnlyLight);
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_Value5", LightBump);
		}
		else if (CanvasImage != null)
		{
			CanvasImage.material.SetFloat("_Alpha", 1f - _Alpha);
			if (UseShinyCurve)
			{
				CanvasImage.material.SetFloat("_Distortion", ShinyLightCurve.Evaluate(ShinyLightCurveTime));
				ShinyLightCurveTime += Time.deltaTime / 8f * AnimationSpeedReduction;
			}
			else
			{
				CanvasImage.material.SetFloat("_Distortion", Light);
			}
			CanvasImage.material.SetFloat("_Value2", LightSize);
			CanvasImage.material.SetFloat("_Value3", Intensity);
			CanvasImage.material.SetFloat("_Value4", OnlyLight);
			CanvasImage.material.SetFloat("_Value5", LightBump);
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
			__MainTex2 = (Resources.Load("_2dxFX_Gradient") as Texture2D);
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
			__MainTex2 = (Resources.Load("_2dxFX_Gradient") as Texture2D);
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
