using UnityEngine;

public class VisualSprite : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	public GraphicsSceneConfig.VisualSprite visualSprite;

	public Transform pivotTransform;

	public Transform spriteTransform;

	public void ResetVisually()
	{
		pivotTransform.localScale = Vector3.one;
		pivotTransform.position = pivotTransform.parent.TransformPoint(visualSprite.pivotPosition);
	}

	public void Init(GraphicsSceneConfig.VisualSprite visualSprite)
	{
		this.visualSprite = visualSprite;
		Sprite sprite = visualSprite.sprite;
		GameObject gameObject = new GameObject("pivot");
		pivotTransform = gameObject.transform;
		pivotTransform.parent = base.transform;
		pivotTransform.localScale = Vector3.one;
		GameObject gameObject2 = new GameObject("sprite");
		spriteTransform = gameObject2.transform;
		spriteTransform.parent = pivotTransform;
		spriteTransform.localScale = Vector3.one;
		if (visualSprite.sprite != null)
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
		Vector3 spritePosition = visualSprite.spritePosition;
		Vector3 pivotPosition = visualSprite.pivotPosition;
		pivotTransform.position = base.transform.parent.TransformPoint(pivotPosition);
		base.transform.parent.TransformPoint(spritePosition);
		spriteTransform.position = spritePosition;
	}

	public void ResetPositions()
	{
		Vector3 spritePosition = visualSprite.spritePosition;
		Vector3 pivotPosition = visualSprite.pivotPosition;
		pivotTransform.position = base.transform.parent.TransformPoint(pivotPosition);
		base.transform.parent.TransformPoint(spritePosition);
		spriteTransform.position = spritePosition;
	}
}
