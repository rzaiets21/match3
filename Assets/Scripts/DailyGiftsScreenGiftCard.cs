using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyGiftsScreenGiftCard : MonoBehaviour
{
	[SerializeField]
	private RectTransform gift;

	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private List<Transform> widgetsToHide = new List<Transform>();

	[SerializeField]
	private VisualStyleSet selectedStyle = new VisualStyleSet();

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private float alphaWenNotSelected = 0.85f;

	public void Init(int giftIndex, float giftScale, bool isSelected)
	{
		GGUtil.Hide(widgetsToHide);
		GGUtil.ChangeText(label, $"Day {giftIndex + 1}");
		gift.localScale = Vector3.one * giftScale;
		if (isSelected)
		{
			selectedStyle.Apply();
		}
		float alpha = isSelected ? 1f : alphaWenNotSelected;
		GGUtil.SetAlpha(canvasGroup, alpha);
	}
}
