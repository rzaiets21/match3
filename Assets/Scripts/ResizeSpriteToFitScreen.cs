using UnityEngine;

public class ResizeSpriteToFitScreen : MonoBehaviour
{
	private void OnEnable()
	{
		ResizeSpriteToScreen();
	}

	private void ResizeSpriteToScreen()
	{
		SpriteRenderer component = GetComponent<SpriteRenderer>();
		if (!(component == null))
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			float x = component.sprite.bounds.size.x;
			float y = component.sprite.bounds.size.y;
			if (!(Camera.main == null))
			{
				float num = Camera.main.orthographicSize * 2f;
				float num2 = num / (float)Screen.height * (float)Screen.width;
				base.transform.localScale = new Vector3(num2 / x, num / y);
			}
		}
	}
}
