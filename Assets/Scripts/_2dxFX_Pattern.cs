using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[ExecuteInEditMode]
public class _2dxFX_Pattern : MonoBehaviour
{
	public Material ForceMaterial;

	public bool ActiveChange = true;

	private string shader = "2DxFX/Standard/Pattern";

	public float _Alpha = 1f;

	public Texture2D __MainTex2;

	public float _OffsetX;

	public float _OffsetY;

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
			ActiveChange = false;
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
			if (!_AutoScrollX && !_AutoScrollY)
			{
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetX", _OffsetX);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetY", _OffsetY);
			}
			if (_AutoScrollX && !_AutoScrollY)
			{
				_AutoScrollCountX += _AutoScrollSpeedX * Time.deltaTime;
				CanvasSpriteRenderer.material.SetFloat("_OffsetX", _AutoScrollCountX);
				CanvasSpriteRenderer.material.SetFloat("_OffsetY", _OffsetY);
			}
			if (!_AutoScrollX && _AutoScrollY)
			{
				_AutoScrollCountY += _AutoScrollSpeedY * Time.deltaTime;
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetX", _OffsetX);
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetY", _AutoScrollCountY);
			}
			if (_AutoScrollX && _AutoScrollY)
			{
				_AutoScrollCountX += _AutoScrollSpeedX * Time.deltaTime;
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetX", _AutoScrollCountX);
				_AutoScrollCountY += _AutoScrollSpeedY * Time.deltaTime;
				CanvasSpriteRenderer.sharedMaterial.SetFloat("_OffsetY", _AutoScrollCountY);
			}
		}
		else if (CanvasImage != null)
		{
			CanvasImage.material.SetFloat("_Alpha", 1f - _Alpha);
			if (!_AutoScrollX && !_AutoScrollY)
			{
				CanvasImage.material.SetFloat("_OffsetX", _OffsetX);
				CanvasImage.material.SetFloat("_OffsetY", _OffsetY);
			}
			if (_AutoScrollX && !_AutoScrollY)
			{
				_AutoScrollCountX += _AutoScrollSpeedX * Time.deltaTime;
				CanvasImage.material.SetFloat("_OffsetX", _AutoScrollCountX);
				CanvasImage.material.SetFloat("_OffsetY", _OffsetY);
			}
			if (!_AutoScrollX && _AutoScrollY)
			{
				_AutoScrollCountY += _AutoScrollSpeedY * Time.deltaTime;
				CanvasImage.material.SetFloat("_OffsetX", _OffsetX);
				CanvasImage.material.SetFloat("_OffsetY", _AutoScrollCountY);
			}
			if (_AutoScrollX && _AutoScrollY)
			{
				_AutoScrollCountX += _AutoScrollSpeedX * Time.deltaTime;
				CanvasImage.material.SetFloat("_OffsetX", _AutoScrollCountX);
				_AutoScrollCountY += _AutoScrollSpeedY * Time.deltaTime;
				CanvasImage.material.SetFloat("_OffsetY", _AutoScrollCountY);
			}
		}
		if (_AutoScrollCountX > 1f)
		{
			_AutoScrollCountX = 0f;
		}
		if (_AutoScrollCountX < -1f)
		{
			_AutoScrollCountX = 0f;
		}
		if (_AutoScrollCountY > 1f)
		{
			_AutoScrollCountY = 0f;
		}
		if (_AutoScrollCountY < -1f)
		{
			_AutoScrollCountY = 0f;
		}
	}

	private void OnDestroy()
	{
		if (Application.isPlaying || !Application.isEditor)
		{
			return;
		}
		if (ForceMaterial != null && tempMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(tempMaterial);
		}
		if (base.gameObject.activeSelf)
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
		if (ForceMaterial != null && tempMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(tempMaterial);
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
		defaultMaterial = new Material(Shader.Find("Sprites/Default"));
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
		if ((bool)__MainTex2)
		{
			__MainTex2.wrapMode = TextureWrapMode.Repeat;
			CanvasSpriteRenderer.sharedMaterial.SetTexture("_MainTex2", __MainTex2);
		}
	}
}
