using System;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceTexturesHelper : MonoBehaviour
{
	[Serializable]
	public class ReplaceTexture
	{
		public Texture findTexture;

		public Texture replaceTexture;
	}

	public GameObject rootGameObject;

	public List<ReplaceTexture> toReplace = new List<ReplaceTexture>();
}
