using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class SelectRoomScreen : MonoBehaviour
{
	[SerializeField]
	private ComponentPool buttonsPool = new ComponentPool();

	private void OnEnable()
	{
		Init();
	}

	private void Init()
	{
		buttonsPool.Clear();
		List<RoomsDB.Room> rooms = ScriptableObjectSingleton<RoomsDB>.instance.rooms;
		for (int i = 0; i < rooms.Count; i++)
		{
			RoomsDB.Room room = rooms[i];
			buttonsPool.Next<SelectRoomScreenButton>(activate: true).Init(room, this);
		}
		buttonsPool.HideNotUsed();
	}

	public void SelectRoomScreenButtonCallback_OnRoomSelected(RoomsDB.Room room)
	{
		RoomsBackend instance = SingletonInit<RoomsBackend>.instance;
		int num = ScriptableObjectSingleton<RoomsDB>.instance.IndexOf(room);
		UnityEngine.Debug.Log("ROOM INDEX " + num);
		instance.selectedRoomIndex = num;
		NavigationManager.instance.Pop();
	}
}
