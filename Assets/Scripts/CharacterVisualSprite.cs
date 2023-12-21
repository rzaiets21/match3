using UnityEngine;

public class CharacterVisualSprite : MonoBehaviour
{
	[SerializeField]
	private CharacterVisualObjectVariation variation;

	public SpriteRenderer spriteRenderer;

	public VisualSprite visualSpriteBeh;

	public Transform pivotTransform;

	public Transform spriteTransform;

	public void SetStencilIndex(int stencilIndex)
	{
		if (!(spriteRenderer == null))
		{
			spriteRenderer.material.SetInt("_Stencil", stencilIndex);
		}
	}

	public void Init(VisualSprite visualSpriteBeh, CharacterVisualObjectVariation variation)
	{
		this.visualSpriteBeh = visualSpriteBeh;
		GraphicsSceneConfig.VisualSprite visualSprite = visualSpriteBeh.visualSprite;
		this.variation = variation;
		Sprite sprite = visualSpriteBeh.spriteRenderer.sprite;
		GameObject gameObject = new GameObject("pivot");
		pivotTransform = gameObject.transform;
		pivotTransform.parent = base.transform;
		pivotTransform.localScale = Vector3.one;
		gameObject.layer = base.gameObject.layer;
		GameObject gameObject2 = new GameObject("sprite");
		spriteTransform = gameObject2.transform;
		spriteTransform.parent = pivotTransform;
		spriteTransform.localScale = Vector3.one;
		gameObject2.layer = base.gameObject.layer;
		if (sprite != null)
		{
			spriteRenderer = gameObject2.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = sprite;
			spriteRenderer.sortingOrder = visualSprite.depth;
			float num = (float)visualSprite.width / sprite.bounds.size.x;
			float num2 = (float)visualSprite.width / (float)visualSprite.height;
			float num3 = sprite.bounds.size.x / sprite.bounds.size.y;
			if (Mathf.Abs(num3 - num2) > 0.01f || float.IsNaN(num2) || float.IsNaN(num3))
			{
				UnityEngine.Debug.LogError(visualSprite.spriteName + " WRONG ASPECT RATIO " + num3 + " " + num2);
			}
			spriteTransform.localScale = new Vector3(num, num, 1f);
		}
		pivotTransform.localPosition = visualSpriteBeh.pivotTransform.localPosition;
		spriteTransform.localPosition = visualSpriteBeh.spriteTransform.localPosition;
		Material spriteMaterial = variation.visualObjectBehaviour.scene.spriteMaterial;
		if (spriteMaterial != null && spriteRenderer != null)
		{
			spriteRenderer.sharedMaterial = spriteMaterial;
		}
	}
}
