using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class SlotsRendererPool : MonoBehaviour
{
	[SerializeField]
	private Transform parent;

	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private bool resetScale = true;

	[SerializeField]
	private bool resetPosition = true;

	private List<TilesBorderRenderer> availableBorderRenderers = new List<TilesBorderRenderer>();

	public void ReturnRenderer(TilesBorderRenderer renderer)
	{
		if (!(renderer == null) && !availableBorderRenderers.Contains(renderer))
		{
			GGUtil.Hide(renderer);
			availableBorderRenderers.Add(renderer);
		}
	}

	private TilesBorderRenderer CreateFromPrefab()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, parent);
		if (resetScale)
		{
			gameObject.transform.localScale = Vector3.one;
		}
		if (resetPosition)
		{
			gameObject.transform.localPosition = Vector3.zero;
		}
		return gameObject.GetComponent<TilesBorderRenderer>();
	}

	public TilesBorderRenderer Next()
	{
		TilesBorderRenderer tilesBorderRenderer = null;
		if (availableBorderRenderers.Count > 0)
		{
			int index = availableBorderRenderers.Count - 1;
			TilesBorderRenderer tilesBorderRenderer2 = availableBorderRenderers[index];
			availableBorderRenderers.RemoveAt(index);
			tilesBorderRenderer = tilesBorderRenderer2;
		}
		if (tilesBorderRenderer == null)
		{
			tilesBorderRenderer = CreateFromPrefab();
		}
		GGUtil.Show(tilesBorderRenderer);
		return tilesBorderRenderer;
	}
}
