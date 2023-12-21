using GGMatch3;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VariationPanel : MonoBehaviour
{
	public struct InitParams
	{
		public bool isPurchased;
	}

	[SerializeField]
	private ComponentPool buttonPool = new ComponentPool();

	[SerializeField]
	private float buttonPadding;

	[NonSerialized]
	private DecorateRoomScreen screen;

	[NonSerialized]
	public InitParams initParams;

	[NonSerialized]
	public DecorateRoomSceneVisualItem uiItem;

	private int _003CvariationIndexAtStart_003Ek__BackingField;

	private bool _003CisVariationChanged_003Ek__BackingField;

	public int variationIndexAtStart
	{
		get
		{
			return _003CvariationIndexAtStart_003Ek__BackingField;
		}
		protected set
		{
			_003CvariationIndexAtStart_003Ek__BackingField = value;
		}
	}

	public bool isVariationChanged
	{
		get
		{
			return _003CisVariationChanged_003Ek__BackingField;
		}
		protected set
		{
			_003CisVariationChanged_003Ek__BackingField = value;
		}
	}

	public void Show(DecorateRoomScreen screen, DecorateRoomSceneVisualItem uiItem, InitParams initParams)
	{
		isVariationChanged = false;
		this.screen = screen;
		this.uiItem = uiItem;
		this.initParams = initParams;
		GGUtil.SetActive(uiItem, active: true);
		uiItem.HideButton();
		uiItem.ShowMarkers();
		buttonPool.Clear();
		VisualObjectBehaviour visualObjectBehaviour = uiItem.visualObjectBehaviour;
		List<VisualObjectVariation> variations = visualObjectBehaviour.variations;
		buttonPool.parent.GetComponent<RectTransform>();
		float x = buttonPool.prefabSizeDelta.x;
		float num = (float)variations.Count * x + (float)(variations.Count - 1) * buttonPadding;
		for (int i = 0; i < variations.Count; i++)
		{
			VariationButton variationButton = buttonPool.Next<VariationButton>();
			Vector3 zero = Vector3.zero;
			zero.x = (0f - num) * 0.5f + ((float)i + 0.5f) * x + (float)i * buttonPadding;
			GGUtil.SetActive(variationButton, active: true);
			variationButton.transform.localPosition = zero;
			variationButton.Init(this, visualObjectBehaviour, i);
		}
		buttonPool.HideNotUsed();
		GGUtil.SetActive(this, active: true);
		visualObjectBehaviour.activeVariation.ScaleAnimation(0f);
		variationIndexAtStart = uiItem.visualObjectBehaviour.visualObject.ownedVariationIndex;
	}

	public void OnBackgroundClick()
	{
		GGUtil.SetActive(this, active: false);
		screen.VariationPanelCallback_OnClosed(this);
		int ownedVariationIndex = uiItem.visualObjectBehaviour.visualObject.ownedVariationIndex;
		if (variationIndexAtStart != ownedVariationIndex)
		{
			Analytics.RoomItemChangedEvent obj = new Analytics.RoomItemChangedEvent
			{
				variation = uiItem.visualObjectBehaviour.visualObject.variations[ownedVariationIndex],
				visualObject = uiItem.visualObjectBehaviour.visualObject
			};
			DecorateRoomScreen decorateRoomScreen = obj.screen = NavigationManager.instance.GetObject<DecorateRoomScreen>();
			obj.Send();
		}
	}

	public void ButtonCallback_OnChange()
	{
		isVariationChanged = true;
	}
}
