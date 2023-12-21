using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class _2dxFX_Offset : MonoBehaviour
{
	public Material ForceMaterial;

	public bool ActiveChange = true;

	private string shader = "2DxFX/Standard/Offset";

	public float _Alpha = 1f;

	public float _OffsetX;

	public float _OffsetY;

	public float _ZoomX = 1f;

	public float _ZoomY = 1f;

	public float _ZoomXY = 1f;

	public bool _AutoScrollX;

	public float _AutoScrollSpeedX;

	public bool _AutoScrollY;

	public float _AutoScrollSpeedY;

	private float _AutoScrollCountX;

	private float _AutoScrollCountY;

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
		if (!ActiveChange)
		{
			return;
		}
		if (CanvasSpriteRenderer != null)
		{
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_Alpha", 1f - _Alpha);
			if (_AutoScrollX)
			{
				_AutoScrollCountX += _AutoScrollSpeedX * 0.01f * Time.deltaTime;
				if (_AutoScrollCountX < 0f)
				{
					_AutoScrollCountX = 1f;
				}
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetX", 1f + _AutoScrollCountX);
			}
			else
			{
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetX", 1f + _OffsetX);
			}
			if (_AutoScrollY)
			{
				_AutoScrollCountY += _AutoScrollSpeedY * 0.01f * Time.deltaTime;
				if (_AutoScrollCountY < 0f)
				{
					_AutoScrollCountY = 1f;
				}
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetY", 1f + _AutoScrollCountY);
			}
			else
			{
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetY", 1f + _OffsetY);
			}
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_ZoomX", _ZoomX * _ZoomXY);
			CanvasSpriteRenderer.sharedMaterial.SetFloat("_ZoomY", _ZoomY * _ZoomXY);
		}
		else
		{
			if (!(CanvasImage != null))
			{
				return;
			}
			CanvasImage.material.SetFloat("_Alpha", 1f - _Alpha);
			if (_AutoScrollX)
			{
				_AutoScrollCountX += _AutoScrollSpeedX * 0.01f * Time.deltaTime;
				if (_AutoScrollCountX < 0f)
				{
					_AutoScrollCountX = 1f;
				}
				CanvasImage.material.SetFloat("_OffsetX", 1f + _AutoScrollCountX);
			}
			else
			{
				CanvasImage.material.SetFloat("_OffsetX", 1f + _OffsetX);
			}
			if (_AutoScrollY)
			{
				_AutoScrollCountY += _AutoScrollSpeedY * 0.01f * Time.deltaTime;
				if (_AutoScrollCountY < 0f)
				{
					_AutoScrollCountY = 1f;
				}
				CanvasImage.material.SetFloat("_OffsetY", 1f + _AutoScrollCountY);
			}
			else
			{
				CanvasImage.material.SetFloat("_OffsetY", 1f + _OffsetY);
			}
			CanvasImage.material.SetFloat("_ZoomX", _ZoomX * _ZoomXY);
			CanvasImage.material.SetFloat("_ZoomY", _ZoomY * _ZoomXY);
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
