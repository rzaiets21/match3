using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class _2dxFX_AL_GrassFX : MonoBehaviour
{
	public Material ForceMaterial;

	public bool ActiveChange = true;

	public bool AddShadow = true;

	public bool ReceivedShadow;

	public int BlendMode;

	private string shader = "2DxFX/AL/GrassFX";

	public float _Alpha = 1f;

	public float Heat = 1f;

	public float Speed = 1f;

	private AnimationCurve Wind;

	private float WindTime1;

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
		Wind = new AnimationCurve();
		Wind.AddKey(0f, 0f);
		Wind.keys[0].inTangent = 0f;
		Wind.keys[0].outTangent = 0f;
		Wind.AddKey(0.1004994f, 0.06637689f);
		Wind.keys[1].inTangent = 0f;
		Wind.keys[1].outTangent = 0f;
		Wind.AddKey(0.2430963f, -0.06465532f);
		Wind.keys[2].inTangent = -0.07599592f;
		Wind.keys[2].outTangent = -0.07599592f;
		Wind.AddKey(0.3425266f, 0.02290122f);
		Wind.keys[3].inTangent = 0.03580004f;
		Wind.keys[3].outTangent = 0.03580004f;
		Wind.AddKey(0.4246872f, -0.02232522f);
		Wind.keys[4].inTangent = -0.006025657f;
		Wind.keys[4].outTangent = -0.006025657f;
		Wind.AddKey(0.5104106f, 0.1647801f);
		Wind.keys[5].inTangent = 0.02981164f;
		Wind.keys[5].outTangent = 0.02981164f;
		Wind.AddKey(0.6082056f, -0.04679203f);
		Wind.keys[6].inTangent = -0.3176928f;
		Wind.keys[6].outTangent = -0.3176928f;
		Wind.AddKey(0.7794942f, 0.2234365f);
		Wind.keys[7].inTangent = 0.2063811f;
		Wind.keys[7].outTangent = 0.2063811f;
		Wind.AddKey(0.8546611f, -0.003165513f);
		Wind.keys[8].inTangent = 0.02264977f;
		Wind.keys[8].outTangent = 0.02264977f;
		Wind.AddKey(1.022495f, -0.07358052f);
		Wind.keys[9].inTangent = 2.450916f;
		Wind.keys[9].outTangent = 2.450916f;
		Wind.AddKey(1.250894f, -0.1813075f);
		Wind.keys[10].inTangent = 0.02214685f;
		Wind.keys[10].outTangent = 0.02214685f;
		Wind.AddKey(1.369877f, -0.06861454f);
		Wind.keys[11].inTangent = -1.860534f;
		Wind.keys[11].outTangent = -1.860534f;
		Wind.AddKey(1.484951f, -0.1543293f);
		Wind.keys[12].inTangent = 0.0602752f;
		Wind.keys[12].outTangent = 0.0602752f;
		Wind.AddKey(1.583562f, 0.100938f);
		Wind.keys[13].inTangent = 0.08665025f;
		Wind.keys[13].outTangent = 0.08665025f;
		Wind.AddKey(1.687307f, -0.100769f);
		Wind.keys[14].inTangent = 0.01110137f;
		Wind.keys[14].outTangent = 0.01110137f;
		Wind.AddKey(1.797593f, 0.04921142f);
		Wind.keys[15].inTangent = 3.407104f;
		Wind.keys[15].outTangent = 3.407104f;
		Wind.AddKey(1.927248f, -0.1877219f);
		Wind.keys[16].inTangent = -0.001117587f;
		Wind.keys[16].outTangent = -0.001117587f;
		Wind.AddKey(2.067694f, 0.2742145f);
		Wind.keys[17].inTangent = 4.736587f;
		Wind.keys[17].outTangent = 4.736587f;
		Wind.AddKey(2.184602f, -0.06127208f);
		Wind.keys[18].inTangent = -0.1308322f;
		Wind.keys[18].outTangent = -0.1308322f;
		Wind.AddKey(2.305948f, 0.1891117f);
		Wind.keys[19].inTangent = 0.04030764f;
		Wind.keys[19].outTangent = 0.04030764f;
		Wind.AddKey(2.428946f, -0.1695723f);
		Wind.keys[20].inTangent = -0.2463162f;
		Wind.keys[20].outTangent = -0.2463162f;
		Wind.AddKey(2.55922f, 0.0359862f);
		Wind.keys[21].inTangent = 0.3967434f;
		Wind.keys[21].outTangent = 0.3967434f;
		Wind.AddKey(2.785119f, -0.08398628f);
		Wind.keys[22].inTangent = -0.2388284f;
		Wind.keys[22].outTangent = -0.2388284f;
		Wind.AddKey(3f, 0f);
		Wind.keys[23].inTangent = 0f;
		Wind.keys[23].outTangent = 0f;
		Wind.postWrapMode = WrapMode.Loop;
		Wind.preWrapMode = WrapMode.Loop;
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
			if (_2DxFX.ActiveShadow && AddShadow)
			{
				CanvasSpriteRenderer.shadowCastingMode = ShadowCastingMode.On;
				if (ReceivedShadow)
				{
					CanvasSpriteRenderer.receiveShadows = true;
					CanvasSpriteRenderer.sharedMaterial.renderQueue = 2450;
					CanvasSpriteRenderer.sharedMaterial.SetInt("_Z", 1);
				}
				else
				{
					CanvasSpriteRenderer.receiveShadows = false;
					CanvasSpriteRenderer.sharedMaterial.renderQueue = 3000;
					CanvasSpriteRenderer.sharedMaterial.SetInt("_Z", 0);
				}
			}
			else
			{
				CanvasSpriteRenderer.shadowCastingMode = ShadowCastingMode.Off;
				CanvasSpriteRenderer.receiveShadows = false;
				CanvasSpriteRenderer.sharedMaterial.renderQueue = 3000;
				CanvasSpriteRenderer.sharedMaterial.SetInt("_Z", 0);
			}
			if (BlendMode == 0)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 0);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 1);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 10);
			}
			if (BlendMode == 1)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 0);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 1);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 1);
			}
			if (BlendMode == 2)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 2);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 1);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 2);
			}
			if (BlendMode == 3)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 4);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 1);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 1);
			}
			if (BlendMode == 4)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 2);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 1);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 1);
			}
			if (BlendMode == 5)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 4);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 10);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 10);
			}
			if (BlendMode == 6)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 0);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 2);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 10);
			}
			if (BlendMode == 7)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 0);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 4);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 1);
			}
			if (BlendMode == 8)
			{
				CanvasSpriteRenderer.sharedMaterial.SetInt("_BlendOp", 2);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_SrcBlend", 7);
				CanvasSpriteRenderer.sharedMaterial.SetInt("_DstBlend", 2);
			}
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_Distortion", Heat);
			if (Wind != null)
			{
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_Wind", Wind.Evaluate(WindTime1));
			}
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_Speed", Speed);
			WindTime1 += Time.deltaTime / 8f * Speed;
		}
		else if (CanvasImage != null)
		{
			CanvasImage.material.SetFloat("_Alpha", 1f - _Alpha);
			CanvasImage.material.SetFloat("_Distortion", Heat);
			if (Wind != null)
			{
				CanvasImage.material.SetFloat("_Wind", Wind.Evaluate(WindTime1));
			}
			CanvasImage.material.SetFloat("_Speed", Speed);
			WindTime1 += Time.deltaTime / 8f * Speed;
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
		WindTime1 = 0f;
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
