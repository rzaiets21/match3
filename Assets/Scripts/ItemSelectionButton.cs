using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelectionButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler
{
	private DecorateRoomScreen roomScreen;

	public void Init(DecorateRoomScreen roomScreen)
	{
		this.roomScreen = roomScreen;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!(roomScreen == null))
		{
			roomScreen.ButtonCallback_OnSceneClick();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
	}
}
