using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class _2dxFX_Wave : MonoBehaviour
{
	public Material ForceMaterial;

	public bool ActiveChange = true;

	private string shader = "2DxFX/Standard/Wave";

	public float _Alpha = 1f;

	public float _OffsetX = 10f;

	public float _OffsetY = 10f;

	public float _DistanceX = 0.03f;

	public float _DistanceY = 0.03f;

	public float _WaveTimeX = 0.16f;

	public float _WaveTimeY = 0.12f;

	public bool AutoPlayWaveX;

	public float AutoPlaySpeedX = 5f;

	public bool AutoPlayWaveY;

	public float AutoPlaySpeedY = 5f;

	public bool AutoRandom;

	public float AutoRandomRange = 10f;

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
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetX", _OffsetX);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetY", _OffsetY);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_DistanceX", _DistanceX);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_DistanceY", _DistanceY);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_WaveTimeX", _WaveTimeX);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_WaveTimeY", _WaveTimeY);
			}
			else if (CanvasImage != null)
			{
				CanvasImage.material.SetFloat("_Alpha", 1f - _Alpha);
				CanvasImage.material.SetFloat("_OffsetX", _OffsetX);
				CanvasImage.material.SetFloat("_OffsetY", _OffsetY);
				CanvasImage.material.SetFloat("_DistanceX", _DistanceX);
				CanvasImage.material.SetFloat("_DistanceY", _DistanceY);
				CanvasImage.material.SetFloat("_WaveTimeX", _WaveTimeX);
				CanvasImage.material.SetFloat("_WaveTimeY", _WaveTimeY);
			}
			float num = (!AutoRandom) ? Time.deltaTime : (UnityEngine.Random.Range(1f, AutoRandomRange) / 5f * Time.deltaTime);
			if (AutoPlayWaveX)
			{
				_WaveTimeX += AutoPlaySpeedX * num;
			}
			if (AutoPlayWaveY)
			{
				_WaveTimeY += AutoPlaySpeedY * num;
			}
			if (_WaveTimeX > 6.28f)
			{
				_WaveTimeX = 0f;
			}
			if (_WaveTimeY > 6.28f)
			{
				_WaveTimeY = 0f;
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
