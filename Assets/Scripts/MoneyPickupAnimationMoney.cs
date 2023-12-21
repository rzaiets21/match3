using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyPickupAnimationMoney : MonoBehaviour
{
	public enum Style
	{
		Coin,
		Star
	}

	[Serializable]
	public class StylePool
	{
		public Style style;

		public ComponentPool pool = new ComponentPool();
	}

	[SerializeField]
	private RectTransform starStyle;

	[SerializeField]
	private RectTransform coinStyle;

	[SerializeField]
	private List<StylePool> stylePools = new List<StylePool>();

	[NonSerialized]
	public int index;

	[NonSerialized]
	public Style style;

	[SerializeField]
	private TextMeshProUGUI bottomLabel;

	[NonSerialized]
	public Vector3 startLocalPosition;

	[NonSerialized]
	public Vector3 startTravelScale;

	private StylePool GetStylePool(Style style)
	{
		for (int i = 0; i < stylePools.Count; i++)
		{
			StylePool stylePool = stylePools[i];
			if (stylePool.style == style)
			{
				return stylePool;
			}
		}
		return null;
	}

	public void SetStyle(Style style)
	{
		GGUtil.SetActive(starStyle, style == Style.Star);
		GGUtil.SetActive(coinStyle, style == Style.Coin);
		this.style = style;
	}

	public void Init(int index, Vector3 startLocalPosition)
	{
		this.index = index;
		this.startLocalPosition = startLocalPosition;
		base.transform.localPosition = startLocalPosition;
	}

	public void Init(int index, Vector3 startLocalPosition, Style style, int count)
	{
		this.index = index;
		this.startLocalPosition = startLocalPosition;
		base.transform.localPosition = startLocalPosition;
		if (style == Style.Star)
		{
			bottomLabel.text = "Design Star";
		}
		else
		{
			bottomLabel.text = count.ToString();
		}
		SetStyle(style);
		ComponentPool pool = GetStylePool(style).pool;
		RectTransform component = pool.parent.GetComponent<RectTransform>();
		pool.Clear();
		for (int i = 0; i < count; i++)
		{
			RectTransform component2 = pool.Next(activate: true).GetComponent<RectTransform>();
			if (count == 1)
			{
				component2.localPosition = Vector3.zero;
			}
			else
			{
				component2.localPosition = new Vector3(UnityEngine.Random.Range((0f - component.sizeDelta.x) * 0.5f, component.sizeDelta.x * 0.5f), UnityEngine.Random.Range((0f - component.sizeDelta.y) * 0.5f, component.sizeDelta.y * 0.5f), 0f);
			}
		}
		pool.HideNotUsed();
	}
}
