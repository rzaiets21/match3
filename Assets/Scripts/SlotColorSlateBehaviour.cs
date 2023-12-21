using System;
using System.Collections.Generic;
using UnityEngine;

public class SlotColorSlateBehaviour : MonoBehaviour
{
	[Serializable]
	public class SpriteDescriptor
	{
		public string spriteName;

		public SpriteRenderer sprite;
	}

	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	private List<SpriteDescriptor> sprites = new List<SpriteDescriptor>();

	public void Init(string spriteName)
	{
		for (int i = 0; i < sprites.Count; i++)
		{
			SpriteDescriptor spriteDescriptor = sprites[i];
			spriteDescriptor.sprite.gameObject.SetActive(spriteDescriptor.spriteName == spriteName);
		}
	}
}
