using System.Collections.Generic;
using UnityEngine;

public class CurrencyPurchaseDialogMultyPrefab : MonoBehaviour
{
	[SerializeField]
	private List<RectTransform> parents = new List<RectTransform>();

	public void Init(List<RectTransform> buttons)
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			RectTransform parent = parents[i % parents.Count];
			RectTransform rectTransform = buttons[i];
			rectTransform.transform.SetParent(parent);
			rectTransform.transform.localPosition = Vector3.zero;
			RectTransform rectTransform2 = rectTransform.transform as RectTransform;
			if (rectTransform2 != null)
			{
				rectTransform2.anchoredPosition = Vector2.zero;
			}
		}
	}
}
