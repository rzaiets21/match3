using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomsDB : ScriptableObjectSingleton<RoomsDB>
{
	[Serializable]
	public class Room
	{
		public string name;

		public string sceneName;

		public string displayName;

		public string description;

		public bool getFromAssetBundleOnEditor;

		public string assetBundleURLOSX;

		public string assetBundleURLAndroid;

		public string assetBundleURLIOS;

		public Sprite cardSprite;

		public GiftBoxScreen.GiftsDefinition giftDefinition = new GiftBoxScreen.GiftsDefinition();

		public bool isOnlyForEditor;

		public bool remove;

		public int totalStarsInRoom;

		private RoomsDB rooms;

		public string editorAssetPath;

		[NonSerialized]
		public DecoratingScene sceneBehaviour;

		[NonSerialized]
		public bool isSceneLoaded;

		[NonSerialized]
		public string loadedSceneName;

		[NonSerialized]
		public AssetBundle loadedAssetBundle;

		public string assetBundleURL
		{
			get
			{
				if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
				{
					return assetBundleURLOSX;
				}
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					return assetBundleURLIOS;
				}
				RuntimePlatform platform = Application.platform;
				return assetBundleURLAndroid;
			}
		}

		public bool isPassed
		{
			get => roomAccessor.isPassed;
			set
			{
				roomAccessor.isPassed = value;
			}
		}

		public bool isLocked => !isOpen;

		public bool isOpen
		{
			get
			{
				int num = rooms.IndexOf(this);
					if (num <= 0)
				{
					return true;
				}
				int index = num - 1;
				return rooms.rooms[index].isPassed;
			}
		}

		private RoomsBackend.RoomAccessor roomAccessor => SingletonInit<RoomsBackend>.instance.GetRoom(name);

		public void Init(RoomsDB rooms)
		{
			this.rooms = rooms;
		}
	}

	public class LoadRoomRequest
	{
		public Room room;

		public float progress;

		public bool isDone;

		public bool isError;

		public LoadRoomRequest(Room room)
		{
			this.room = room;
		}
	}

	private sealed class _003CLoadRoom_003Ed__11 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public LoadRoomRequest roomRequest;

		public RoomsDB _003C_003E4__this;

		private Room _003Croom_003E5__2;

		private Scene _003Cscene_003E5__3;

		private bool _003CroomLoaded_003E5__4;

		private List<Room> _003Crooms_003E5__5;

		private int _003Ci_003E5__6;

		private Room _003Citem_003E5__7;

		private AsyncOperation _003CasyncWait_003E5__8;

		private string _003CsceneName_003E5__9;

		private WWW _003Cwww_003E5__10;

		private AssetBundle _003CassetBundle_003E5__11;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CLoadRoom_003Ed__11(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || (uint)(num - 3) <= 1u)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
		}

		private bool MoveNext()
		{
			try
			{
				int num = _003C_003E1__state;
				RoomsDB roomsDB = _003C_003E4__this;
				string[] allScenePaths;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Croom_003E5__2 = roomRequest.room;
					_003Cscene_003E5__3 = default(Scene);
					_003CroomLoaded_003E5__4 = false;
					_003Crooms_003E5__5 = roomsDB.rooms;
					_003Ci_003E5__6 = 0;
					goto IL_0149;
				case 1:
					_003C_003E1__state = -1;
					goto IL_00e0;
				case 2:
					_003C_003E1__state = -1;
					goto IL_01e2;
				case 3:
					_003C_003E1__state = -3;
					goto IL_0290;
				case 4:
					_003C_003E1__state = -3;
					goto IL_0349;
				case 5:
					{
						_003C_003E1__state = -1;
						break;
					}
					IL_0149:
					if (_003Ci_003E5__6 < _003Crooms_003E5__5.Count)
					{
						_003Citem_003E5__7 = _003Crooms_003E5__5[_003Ci_003E5__6];
						if (_003Citem_003E5__7.isSceneLoaded)
						{
							Scene sceneByName = SceneManager.GetSceneByName(_003Citem_003E5__7.loadedSceneName);
							if (sceneByName.isLoaded)
							{
								_003CasyncWait_003E5__8 = SceneManager.UnloadSceneAsync(sceneByName);
								goto IL_00e0;
							}
							goto IL_00f4;
						}
						goto IL_0100;
					}
					if (!_003CroomLoaded_003E5__4)
					{
						if (!string.IsNullOrEmpty(_003Croom_003E5__2.sceneName))
						{
							_003CsceneName_003E5__9 = _003Croom_003E5__2.sceneName;
							_003CasyncWait_003E5__8 = SceneManager.LoadSceneAsync(_003CsceneName_003E5__9, LoadSceneMode.Additive);
							_003CasyncWait_003E5__8.allowSceneActivation = true;
							goto IL_01e2;
						}
						_003Cwww_003E5__10 = WWW.LoadFromCacheOrDownload(_003Croom_003E5__2.assetBundleURL, 0);
						_003C_003E1__state = -3;
						goto IL_0290;
					}
					goto IL_03ee;
					IL_00e0:
					if (!_003CasyncWait_003E5__8.isDone)
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 1;
						return true;
					}
					_003CasyncWait_003E5__8 = null;
					goto IL_00f4;
					IL_0349:
					if (!_003CasyncWait_003E5__8.isDone)
					{
						roomRequest.progress = _003CasyncWait_003E5__8.progress;
						_003C_003E2__current = null;
						_003C_003E1__state = 4;
						return true;
					}
					_003Cscene_003E5__3 = SceneManager.GetSceneByName(_003CsceneName_003E5__9);
					_003Croom_003E5__2.isSceneLoaded = true;
					_003Croom_003E5__2.loadedSceneName = _003Cscene_003E5__3.name;
					_003Croom_003E5__2.loadedAssetBundle = _003CassetBundle_003E5__11;
					_003CroomLoaded_003E5__4 = true;
					_003CassetBundle_003E5__11.Unload(unloadAllLoadedObjects: false);
					_003Cwww_003E5__10.Dispose();
					_003CroomLoaded_003E5__4 = true;
					_003CassetBundle_003E5__11 = null;
					_003CsceneName_003E5__9 = null;
					_003CasyncWait_003E5__8 = null;
					_003C_003Em__Finally1();
					_003Cwww_003E5__10 = null;
					goto IL_03ee;
					IL_01e2:
					if (!_003CasyncWait_003E5__8.isDone)
					{
						roomRequest.progress = _003CasyncWait_003E5__8.progress;
						_003C_003E2__current = null;
						_003C_003E1__state = 2;
						return true;
					}
					_003Cscene_003E5__3 = SceneManager.GetSceneByName(_003CsceneName_003E5__9);
					_003Croom_003E5__2.isSceneLoaded = true;
					_003Croom_003E5__2.loadedSceneName = _003Cscene_003E5__3.name;
					_003CroomLoaded_003E5__4 = true;
					_003CsceneName_003E5__9 = null;
					_003CasyncWait_003E5__8 = null;
					goto IL_03ee;
					IL_03ee:
					if (!_003Cscene_003E5__3.IsValid())
					{
						_003C_003E2__current = null;
						_003C_003E1__state = 5;
						return true;
					}
					break;
					IL_00f4:
					_003Citem_003E5__7.isSceneLoaded = false;
					goto IL_0100;
					IL_0100:
					if (_003Citem_003E5__7.loadedAssetBundle != null)
					{
						_003Citem_003E5__7.loadedAssetBundle.Unload(unloadAllLoadedObjects: true);
					}
					_003Citem_003E5__7.loadedAssetBundle = null;
					_003Citem_003E5__7 = null;
					_003Ci_003E5__6++;
					goto IL_0149;
					IL_0290:
					if (!_003Cwww_003E5__10.isDone)
					{
						roomRequest.progress = _003Cwww_003E5__10.progress;
						_003C_003E2__current = null;
						_003C_003E1__state = 3;
						return true;
					}
					if (_003Cwww_003E5__10.error != null)
					{
						roomRequest.isDone = true;
						roomRequest.isError = true;
						bool result = false;
						_003C_003Em__Finally1();
						return result;
					}
					_003CassetBundle_003E5__11 = _003Cwww_003E5__10.assetBundle;
					allScenePaths = _003CassetBundle_003E5__11.GetAllScenePaths();
					_003CsceneName_003E5__9 = Path.GetFileNameWithoutExtension(allScenePaths[0]);
					_003CasyncWait_003E5__8 = SceneManager.LoadSceneAsync(_003CsceneName_003E5__9, LoadSceneMode.Additive);
					_003CasyncWait_003E5__8.allowSceneActivation = true;
					goto IL_0349;
				}
				DecoratingScene sceneBehaviour = null;
				GameObject[] rootGameObjects = _003Cscene_003E5__3.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					DecoratingScene component = rootGameObjects[i].GetComponent<DecoratingScene>();
					if (component != null)
					{
						sceneBehaviour = component;
						break;
					}
				}
				_003Croom_003E5__2.sceneBehaviour = sceneBehaviour;
				roomRequest.isDone = true;
				return false;
			}
			catch
			{
				//try-fault
				// System_002EIDisposable_002EDispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003Cwww_003E5__10 != null)
			{
				((IDisposable)_003Cwww_003E5__10).Dispose();
			}
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[SerializeField]
	private List<Room> roomsList = new List<Room>();

	[NonSerialized]
	private List<Room> rooms_ = new List<Room>();

	public List<Room> rooms
	{
		get
		{
			rooms_.Clear();
			for (int i = 0; i < roomsList.Count; i++)
			{
				Room room = roomsList[i];
				bool flag = true;
				if (room.isOnlyForEditor)
				{
					flag = false;
				}
				if (room.remove)
				{
					flag = false;
				}
				if (flag)
				{
					rooms_.Add(room);
				}
			}
			return rooms_;
		}
	}

	public Room ActiveRoom
	{
		get
		{
			RoomsBackend instance = SingletonInit<RoomsBackend>.instance;
			List<Room> rooms = this.rooms;
			int selectedRoomIndex = instance.selectedRoomIndex;
			return rooms[Mathf.Clamp(selectedRoomIndex, 0, rooms.Count - 1)];
		}
	}

	public Room NextRoom(Room room)
	{
		List<Room> rooms = this.rooms;
		int num = rooms.IndexOf(room);
		int num2 = 0;
		if (num >= 0)
		{
			num2 = num + 1;
		}
		if (num2 < 0 || num2 > rooms.Count - 1)
		{
			return null;
		}
		return rooms[num2];
	}

	public int IndexOf(Room room)
	{
		return rooms.IndexOf(room);
	}

	protected override void UpdateData()
	{
		base.UpdateData();
		List<Room> rooms = this.rooms;
		for (int i = 0; i < rooms.Count; i++)
		{
			rooms[i].Init(this);
		}
	}

	public IEnumerator LoadRoom(LoadRoomRequest roomRequest)
	{
		return new _003CLoadRoom_003Ed__11(0)
		{
			_003C_003E4__this = this,
			roomRequest = roomRequest
		};
	}
}
