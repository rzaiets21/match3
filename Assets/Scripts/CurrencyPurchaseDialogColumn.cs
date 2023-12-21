using System.Collections.Generic;
using UnityEngine;

public class CurrencyPurchaseDialogColumn : MonoBehaviour
{
	[SerializeField]
	private RectTransform parent;

	[SerializeField]
	private RectTransform containingRectangle;

	public Vector2 GetSize()
	{
		return containingRectangle.rect.size;
	}

	public void Init(List<RectTransform> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			items[i].parent = parent;
		}
	}
}
