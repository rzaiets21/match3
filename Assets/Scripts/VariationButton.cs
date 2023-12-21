using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariationButton : MonoBehaviour
{
	[SerializeField]
	public Image image;

	[SerializeField]
	public Image background;

	[SerializeField]
	public int topMargin;

	[SerializeField]
	public int bottomMargin;

	[SerializeField]
	public int leftMargin;

	[SerializeField]
	public int rightMargin;

	[SerializeField]
	public List<Sprite> sprites = new List<Sprite>();

	private float _imageHeight;

	private float _ratio;

	[NonSerialized]
	private int variationIndex;

	[NonSerialized]
	private VisualObjectBehaviour visualObjectBehaviour;

	private VariationPanel variationPanel;

	public float ratio => _ratio;

	public float imageWidth
	{
		get
		{
			return imageHeight * ratio;
		}
		set
		{
			imageHeight = value / ratio;
			Resize();
		}
	}

	public float imageHeight
	{
		get
		{
			return _imageHeight;
		}
		set
		{
			_imageHeight = value;
			Resize();
		}
	}

	public void Init(VariationPanel variationPanel, VisualObjectBehaviour visualObjectBehaviour, int variationIndex)
	{
		this.visualObjectBehaviour = visualObjectBehaviour;
		Sprite thumbnailSprite = visualObjectBehaviour.variations[variationIndex].thumbnailSprite;
		this.variationIndex = variationIndex;
		image.sprite = thumbnailSprite;
		_imageHeight = image.sprite.rect.height;
		_ratio = image.sprite.rect.width / image.sprite.rect.height;
		this.variationPanel = variationPanel;
		Fit();
	}

	public void Fit()
	{
		RectTransform component = background.GetComponent<RectTransform>();
		float width = component.rect.width;
		float height = component.rect.height;
		imageWidth = width;
		imageHeight = height;
		imageWidth = Mathf.Min(width - (float)leftMargin - (float)rightMargin, imageWidth);
		imageHeight = Mathf.Min(height - (float)topMargin - (float)bottomMargin, imageHeight);
		Vector3 localPosition = image.transform.localPosition;
		localPosition.x = (component.rect.xMin + (float)leftMargin) * 0.5f + (component.rect.xMax - (float)rightMargin) * 0.5f;
		localPosition.y = (component.rect.yMin + (float)bottomMargin) * 0.5f + (component.rect.yMax - (float)topMargin) * 0.5f;
		image.transform.localPosition = localPosition;
	}

	public void Resize()
	{
		RectTransform component = image.GetComponent<RectTransform>();
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, imageWidth);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, imageHeight);
	}

	public void OnClick()
	{
		int ownedVariationIndex = visualObjectBehaviour.visualObject.ownedVariationIndex;
		visualObjectBehaviour.visualObject.ownedVariationIndex = variationIndex;
		visualObjectBehaviour.ShowVariationBehaviour(variationIndex);
		visualObjectBehaviour.activeVariation.ScaleAnimation(0f);
		GGSoundSystem.Play(GGSoundSystem.SFXType.Flip);
		if (variationPanel != null)
		{
			variationPanel.ButtonCallback_OnChange();
		}
	}
}
