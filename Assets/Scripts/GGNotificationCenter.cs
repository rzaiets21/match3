using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GGNotificationCenter : BehaviourSingletonInit<GGNotificationCenter>
{
	public delegate void GGNotificationCenterDelegate(string message);

	public class EventDispatcher
	{
		public delegate void EventDelegateListener(object data);

		public class EventDelegate
		{
			public EventDelegateListener onMessageCall;
		}

		public class EventDelegateList
		{
			public List<EventDelegate> eventList = new List<EventDelegate>();

			public void NotifyListeners(object data)
			{
				for (int i = 0; i < eventList.Count; i++)
				{
					EventDelegate eventDelegate = eventList[i];
					if (eventDelegate.onMessageCall != null)
					{
						try
						{
							eventDelegate.onMessageCall(data);
						}
						catch
						{
							UnityEngine.Debug.Log("ERROR IN DELEGATE");
						}
					}
				}
			}
		}

		protected Dictionary<Type, EventDelegateList> eventMap = new Dictionary<Type, EventDelegateList>();

		public void AssignListener(Type type, EventDelegateListener listener)
		{
			EventDelegateList eventDelegateList = null;
			if (eventMap.ContainsKey(type))
			{
				eventDelegateList = eventMap[type];
			}
			else
			{
				eventDelegateList = new EventDelegateList();
				eventMap.Add(type, eventDelegateList);
			}
			bool flag = false;
			for (int i = 0; i < eventDelegateList.eventList.Count; i++)
			{
				if (eventDelegateList.eventList[i].onMessageCall == listener)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				EventDelegate eventDelegate = new EventDelegate();
				eventDelegate.onMessageCall = listener;
				eventDelegateList.eventList.Add(eventDelegate);
			}
		}

		public void NotifyListeners(Type type, object data)
		{
			EventDelegateList eventDelegateList = null;
			if (eventMap.ContainsKey(type))
			{
				eventDelegateList = eventMap[type];
			}
			eventDelegateList?.NotifyListeners(data);
		}
	}

	public const string PurchaseIAPSuccess = "Purchase.IAP.Success";

	protected List<EventDispatcher> eventDispatchers = new List<EventDispatcher>();

	private EventDispatcher _003CdefaultEventDispatcher_003Ek__BackingField;

	public EventDispatcher defaultEventDispatcher
	{
		get
		{
			return _003CdefaultEventDispatcher_003Ek__BackingField;
		}
		protected set
		{
			_003CdefaultEventDispatcher_003Ek__BackingField = value;
		}
	}

	public event GGNotificationCenterDelegate onMessage;

	public override void Init()
	{
		base.Init();
		defaultEventDispatcher = new EventDispatcher();
		eventDispatchers.Add(defaultEventDispatcher);
	}

	public void AddEventDispatcher(EventDispatcher ed)
	{
		eventDispatchers.Add(ed);
	}

	public void RemoveEventDispatcher(EventDispatcher ed)
	{
		eventDispatchers.Remove(ed);
	}

	protected void NotifyEventDispatchers(Type type, object data)
	{
		for (int i = 0; i < eventDispatchers.Count; i++)
		{
			eventDispatchers[i]?.NotifyListeners(type, data);
		}
	}

	public void Broadcast(string message)
	{
		UnityEngine.Debug.Log("GGNotificationCenter.Broadcast('" + message + "')");
		this.onMessage(message);
	}

	public void BroadcastEvent(object e)
	{
		NotifyEventDispatchers(e.GetType(), e);
	}
}
