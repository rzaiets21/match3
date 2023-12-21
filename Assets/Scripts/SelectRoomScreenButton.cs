using System;
using TMPro;
using UnityEngine;

public class SelectRoomScreenButton : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI roomNameLabel;

	[NonSerialized]
	private RoomsDB.Room room;

	[NonSerialized]
	private SelectRoomScreen roomScreen;

	public void Init(RoomsDB.Room room, SelectRoomScreen roomScreen)
	{
		this.room = room;
		this.roomScreen = roomScreen;
		GGUtil.ChangeText(roomNameLabel, room.name);
	}

	public void ButtonCallback_OnClick()
	{
		roomScreen.SelectRoomScreenButtonCallback_OnRoomSelected(room);
	}
}
