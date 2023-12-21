using GGMatch3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragTarget : MonoBehaviour
{
	private Camera uiCamera_;

	[SerializeField]
	private Image image;

	private ConfirmPurchasePanel panel;

	private Camera uiCamera
	{
		get
		{
			if (uiCamera_ == null)
			{
				uiCamera_ = NavigationManager.instance.GetCamera();
			}
			return uiCamera_;
		}
	}

	public void Init(ConfirmPurchasePanel panel, Sprite sprite)
	{
		this.panel = panel;
		image.sprite = sprite;
	}

	public void OnDrop(BaseEventData data)
	{
		PointerEventData pointerEventData = data as PointerEventData;
		Vector3 dragButtonWorldPosition = uiCamera.ScreenToWorldPoint(pointerEventData.position);
		if (panel.IsTargetIn(dragButtonWorldPosition))
		{
			panel.OnPurchaseConfirmed();
		}
	}
}
