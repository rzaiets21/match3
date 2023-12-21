using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteGroup : MonoBehaviour
{
	[SerializeField]
	private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

	[SerializeField]
	private float alphaValue;

	private void OnDidApplyAnimationProperties()
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			SpriteRenderer spriteRenderer = sprites[i];
			Color color = spriteRenderer.color;
			color.a = alphaValue;
			spriteRenderer.color = color;
		}
	}
}
