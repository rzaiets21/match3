using System;
using System.Collections.Generic;
using UnityEngine;

public class ChipFeatherBehaviour : MonoBehaviour
{
	[Serializable]
	public class ColorSetup
	{
		public ItemColor itemColor;

		public SpriteRenderer spriteRenderer;

		public Color color = Color.white;

		public void Apply()
		{
			if (spriteRenderer != null)
			{
				spriteRenderer.color = color;
			}
		}
	}

	[SerializeField]
	private List<ColorSetup> colors = new List<ColorSetup>();

	public void Init(ItemColor itemColor)
	{
		for (int i = 0; i < colors.Count; i++)
		{
			ColorSetup colorSetup = colors[i];
			if (colorSetup.itemColor == itemColor)
			{
				colorSetup.Apply();
			}
		}
	}
}
