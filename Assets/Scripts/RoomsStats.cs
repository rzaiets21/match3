using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RoomsStats : MonoBehaviour
{
	private sealed class _003CDoCheckStats_003Ed__2 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public RoomsStats _003C_003E4__this;

		private int _003CallRoomsStars_003E5__2;

		private RoomsDB _003CroomsDB_003E5__3;

		private List<RoomsDB.Room> _003Crooms_003E5__4;

		private int _003Ci_003E5__5;

		private RoomsDB.Room _003Croom_003E5__6;

		private RoomsDB.LoadRoomRequest _003CroomRequest_003E5__7;

		private IEnumerator _003CupdateEnum_003E5__8;

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
		public _003CDoCheckStats_003Ed__2(int _003C_003E1__state)
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
			RoomsStats roomsStats = _003C_003E4__this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				_003C_003E1__state = -1;
				goto IL_00b4;
			}
			_003C_003E1__state = -1;
			_003CallRoomsStars_003E5__2 = 0;
			_003CroomsDB_003E5__3 = ScriptableObjectSingleton<RoomsDB>.instance;
			_003Crooms_003E5__4 = _003CroomsDB_003E5__3.rooms;
			_003Ci_003E5__5 = 0;
			goto IL_0163;
			IL_00b4:
			if (_003CupdateEnum_003E5__8.MoveNext())
			{
				float progress = _003CroomRequest_003E5__7.progress;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if (_003Croom_003E5__6.sceneBehaviour == null)
			{
				UnityEngine.Debug.Log("CANT LOAD ROOM " + _003Croom_003E5__6.name);
			}
			else
			{
				int num2 = roomsStats.TotalStarsCount(_003Croom_003E5__6);
				_003Croom_003E5__6.totalStarsInRoom = num2;
				UnityEngine.Debug.LogFormat("{0}: {1}", _003Croom_003E5__6.name, num2);
				_003CallRoomsStars_003E5__2 += num2;
				_003Croom_003E5__6 = null;
				_003CroomRequest_003E5__7 = null;
				_003CupdateEnum_003E5__8 = null;
			}
			_003Ci_003E5__5++;
			goto IL_0163;
			IL_0163:
			if (_003Ci_003E5__5 < _003Crooms_003E5__4.Count)
			{
				_003Croom_003E5__6 = _003Crooms_003E5__4[_003Ci_003E5__5];
				_003CroomRequest_003E5__7 = new RoomsDB.LoadRoomRequest(_003Croom_003E5__6);
				_003CupdateEnum_003E5__8 = _003CroomsDB_003E5__3.LoadRoom(_003CroomRequest_003E5__7);
				goto IL_00b4;
			}
			UnityEngine.Debug.LogFormat("Total: {0}", _003CallRoomsStars_003E5__2);
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

	private IEnumerator action;

	public void CheckStats()
	{
		action = DoCheckStats();
	}

	private IEnumerator DoCheckStats()
	{
		return new _003CDoCheckStats_003Ed__2(0)
		{
			_003C_003E4__this = this
		};
	}

	private int TotalStarsCount(RoomsDB.Room room)
	{
		int num = 0;
		List<VisualObjectBehaviour> visualObjectBehaviours = room.sceneBehaviour.visualObjectBehaviours;
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			if (visualObjectBehaviour.isPlayerControlledObject)
			{
				num += visualObjectBehaviour.visualObject.sceneObjectInfo.price.cost;
			}
		}
		return num;
	}

	private void Update()
	{
		if (action != null)
		{
			action.MoveNext();
		}
	}
}
