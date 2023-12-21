using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ScrollableSelectRoomScreen : MonoBehaviour
{
	public class ChangeRoomArguments
	{
		public RoomsDB.Room passedRoom;

		public RoomsDB.Room unlockedRoom;
	}

	private sealed class _003CDoChangeRoomsAnimation_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ScrollableSelectRoomScreen _003C_003E4__this;

		public ChangeRoomArguments changeRoom;

		private ScrollableSelectRoomScreenButton _003CpassedRoom_003E5__2;

		private ScrollableSelectRoomScreenButton _003CunlockedRoom_003E5__3;

		private float _003Cdelay_003E5__4;

		private float _003Ctime_003E5__5;

		private IEnumerator _003Cenumerator_003E5__6;

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
		public _003CDoChangeRoomsAnimation_003Ed__9(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			ScrollableSelectRoomScreen scrollableSelectRoomScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				GGUtil.Show(scrollableSelectRoomScreen.stopInteractionWidget);
				_003CpassedRoom_003E5__2 = scrollableSelectRoomScreen.GetButton(changeRoom.passedRoom);
				_003CunlockedRoom_003E5__3 = scrollableSelectRoomScreen.GetButton(changeRoom.unlockedRoom);
				if (_003CunlockedRoom_003E5__3 != null)
				{
					_003CunlockedRoom_003E5__3.ShowLocked();
				}
				_003Cdelay_003E5__4 = 0f;
				_003Ctime_003E5__5 = 0f;
				if (_003CpassedRoom_003E5__2 != null)
				{
					_003CpassedRoom_003E5__2.ShowOpenNotPassed();
					scrollableSelectRoomScreen.CenterContainerToCenterChild(_003CpassedRoom_003E5__2.GetComponent<RectTransform>());
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_013d;
			case 1:
				_003C_003E1__state = -1;
				_003CpassedRoom_003E5__2.ShowPassedAnimation();
				_003Cdelay_003E5__4 = _003CpassedRoom_003E5__2.passAnimationDuration;
				_003Ctime_003E5__5 = 0f;
				goto IL_012f;
			case 2:
				_003C_003E1__state = -1;
				goto IL_012f;
			case 3:
				_003C_003E1__state = -1;
				goto IL_0183;
			case 4:
				{
					_003C_003E1__state = -1;
					goto IL_01e2;
				}
				IL_012f:
				if (_003Ctime_003E5__5 <= _003Cdelay_003E5__4)
				{
					_003Ctime_003E5__5 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_013d;
				IL_013d:
				if (!(_003CunlockedRoom_003E5__3 != null))
				{
					break;
				}
				_003Cenumerator_003E5__6 = scrollableSelectRoomScreen.MoveCenterContainerToCenterChild(_003CunlockedRoom_003E5__3.GetComponent<RectTransform>(), 0.5f);
				goto IL_0183;
				IL_0183:
				if (_003Cenumerator_003E5__6.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 3;
					return true;
				}
				_003CunlockedRoom_003E5__3.ShowUnlockAnimation();
				_003Cdelay_003E5__4 = _003CunlockedRoom_003E5__3.unlockAnimationDuration;
				_003Ctime_003E5__5 = 0f;
				goto IL_01e2;
				IL_01e2:
				if (_003Ctime_003E5__5 < _003Cdelay_003E5__4)
				{
					_003Ctime_003E5__5 += Time.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 4;
					return true;
				}
				_003Cenumerator_003E5__6 = null;
				break;
			}
			GGUtil.Hide(scrollableSelectRoomScreen.stopInteractionWidget);
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private sealed class _003CMoveCenterContainerToCenterChild_003Ed__12 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public RectTransform item;

		public float duration;

		private RectTransform _003Ccontainer_003E5__2;

		private Vector3 _003CstartPosition_003E5__3;

		private Vector3 _003CendPosition_003E5__4;

		private float _003Ctime_003E5__5;

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
		public _003CMoveCenterContainerToCenterChild_003Ed__12(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003Ccontainer_003E5__2 = item.parent.GetComponent<RectTransform>();
				Vector3 localPosition = _003Ccontainer_003E5__2.localPosition;
				Vector3 localPosition2 = item.localPosition;
				_003CstartPosition_003E5__3 = localPosition;
				_003CendPosition_003E5__4 = _003CstartPosition_003E5__3;
				_003CendPosition_003E5__4.x = 0f - localPosition2.x;
				_003Ctime_003E5__5 = 0f;
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__5 <= duration)
			{
				_003Ctime_003E5__5 += Time.deltaTime;
				float t = Mathf.InverseLerp(0f, duration, _003Ctime_003E5__5);
				Vector3 localPosition3 = Vector3.Lerp(_003CstartPosition_003E5__3, _003CendPosition_003E5__4, t);
				_003Ccontainer_003E5__2.localPosition = localPosition3;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[SerializeField]
	private List<Transform> widgetsToHide = new List<Transform>();

	[SerializeField]
	private Transform stopInteractionWidget;

	[SerializeField]
	private ComponentPool roomsPool = new ComponentPool();

	[SerializeField]
	private float spacing = 80f;

	private ChangeRoomArguments changeRoomArguments;

	private IEnumerator animationEnumerator;

	public void Show(ChangeRoomArguments changeRoomArguments)
	{
		this.changeRoomArguments = changeRoomArguments;
		NavigationManager.instance.Push(base.gameObject);
	}

	private void Init(ChangeRoomArguments changeRoomArguments)
	{
		GGUtil.Hide(widgetsToHide);
		roomsPool.Clear();
		RoomsDB instance = ScriptableObjectSingleton<RoomsDB>.instance;
		List<RoomsDB.Room> rooms = instance.rooms;
		RectTransform component = roomsPool.parent.GetComponent<RectTransform>();
		float x = roomsPool.prefabSizeDelta.x;
		float num = x * (float)rooms.Count + spacing * (float)(rooms.Count - 1);
		Vector2 sizeDelta = component.sizeDelta;
		sizeDelta.x = num;
		sizeDelta.y = roomsPool.prefabSizeDelta.y;
		component.sizeDelta = sizeDelta;
		float num2 = (0f - num) * 0.5f;
		for (int i = 0; i < rooms.Count; i++)
		{
			RoomsDB.Room room = rooms[i];
			ScrollableSelectRoomScreenButton scrollableSelectRoomScreenButton = roomsPool.Next<ScrollableSelectRoomScreenButton>(activate: true);
			RectTransform component2 = scrollableSelectRoomScreenButton.GetComponent<RectTransform>();
			Vector3 localPosition = component2.localPosition;
			localPosition.x = num2 + (float)i * (spacing + x) + x * 0.5f;
			localPosition.y = 0f;
			component2.localPosition = localPosition;
			scrollableSelectRoomScreenButton.Init(room);
		}
		roomsPool.HideNotUsed();
		animationEnumerator = null;
		if (changeRoomArguments == null)
		{
			ScrollableSelectRoomScreenButton button = GetButton(instance.ActiveRoom);
			CenterContainerToCenterChild(button.GetComponent<RectTransform>());
		}
		else
		{
			animationEnumerator = DoChangeRoomsAnimation(changeRoomArguments);
			animationEnumerator.MoveNext();
		}
	}

	private IEnumerator DoChangeRoomsAnimation(ChangeRoomArguments changeRoom)
	{
		return new _003CDoChangeRoomsAnimation_003Ed__9(0)
		{
			_003C_003E4__this = this,
			changeRoom = changeRoom
		};
	}

	private ScrollableSelectRoomScreenButton GetButton(RoomsDB.Room room)
	{
		for (int i = 0; i < roomsPool.usedObjects.Count; i++)
		{
			ScrollableSelectRoomScreenButton component = roomsPool.usedObjects[i].GetComponent<ScrollableSelectRoomScreenButton>();
			if (component.room == room)
			{
				return component;
			}
		}
		return null;
	}

	private void CenterContainerToCenterChild(RectTransform item)
	{
		RectTransform component = item.parent.GetComponent<RectTransform>();
		Vector3 localPosition = component.localPosition;
		Vector3 localPosition2 = item.localPosition;
		localPosition.x = 0f - localPosition2.x;
		component.localPosition = localPosition;
	}

	private IEnumerator MoveCenterContainerToCenterChild(RectTransform item, float duration)
	{
		return new _003CMoveCenterContainerToCenterChild_003Ed__12(0)
		{
			item = item,
			duration = duration
		};
	}

	private void OnEnable()
	{
		Init(changeRoomArguments);
		changeRoomArguments = null;
	}

	private void Update()
	{
		if (animationEnumerator != null)
		{
			animationEnumerator.MoveNext();
		}
	}
}
