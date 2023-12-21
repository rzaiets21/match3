using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GGMatch3
{
	[DefaultExecutionOrder(-100)]
	public class NavigationManager : MonoBehaviour
	{
		[Serializable]
		public class ObjectDefinition
		{
			public GameObject gameObject;

			public bool isScreen = true;
		}

		public class HistoryObject
		{
			public GameObject gameObject;

			public void SetActive(bool active)
			{
				GGUtil.SetActive(gameObject, active);
			}

			public void Hide()
			{
				GGUtil.SetActive(gameObject, active: false);
			}

			public void Show()
			{
				GGUtil.SetActive(gameObject, active: true);
			}
		}

		[SerializeField]
		private bool startInApp;

		[SerializeField]
		private bool showTermsOfServiceOnStart;

		[SerializeField]
		private string layerToLoadAtStart;

		[SerializeField]
		private List<ObjectDefinition> objectsList = new List<ObjectDefinition>();

		private static NavigationManager instance_;

		public List<HistoryObject> history = new List<HistoryObject>();

		public static NavigationManager instance
		{
			get
			{
				if (instance_ == null)
				{
					instance_ = FindObjectOfType<NavigationManager>();
				}
				return instance_;
			}
		}

		public HistoryObject CurrentScreen
		{
			get
			{
				if (history.Count == 0)
				{
					return null;
				}
				return history[history.Count - 1];
			}
		}

		public ObjectDefinition GetObjectByName(string name)
		{
			for (int i = 0; i < objectsList.Count; i++)
			{
				ObjectDefinition objectDefinition = objectsList[i];
				if (!(objectDefinition.gameObject == null) && objectDefinition.gameObject.name == name)
				{
					return objectDefinition;
				}
			}
			return null;
		}

		public Camera GetCamera()
		{
			for (int i = 0; i < objectsList.Count; i++)
			{
				ObjectDefinition objectDefinition = objectsList[i];
				if (!(objectDefinition.gameObject == null))
				{
					Camera component = objectDefinition.gameObject.GetComponent<Camera>();
					if (!(component == null))
					{
						return component;
					}
				}
			}
			return null;
		}

		public T GetObject<T>() where T : MonoBehaviour
		{
			for (int i = 0; i < objectsList.Count; i++)
			{
				ObjectDefinition objectDefinition = objectsList[i];
				if (!(objectDefinition.gameObject == null))
				{
					T component = objectDefinition.gameObject.GetComponent<T>();
					if (!((Object)component == (Object)null))
					{
						return component;
					}
				}
			}
			return null;
		}

		private void Awake()
		{
			Application.targetFrameRate = 60;
			
			for (int i = 0; i < objectsList.Count; i++)
			{
				ObjectDefinition objectDefinition = objectsList[i];
				if (objectDefinition.isScreen && GGUtil.isPartOfHierarchy(objectDefinition.gameObject))
				{
					GGUtil.Hide(objectDefinition.gameObject);
				}
			}
			if (startInApp)
			{
				InAppBackend instance = BehaviourSingletonInit<InAppBackend>.instance;
			}
			if (showTermsOfServiceOnStart && !GGPlayerSettings.instance.Model.acceptedTermsOfService)
			{
				GetObject<TermsOfServiceDialog>().Show(_003CAwake_003Eb__15_0);
			}
			else
			{
				LoadStartLayer();
			}
			GGNotificationCenter instance2 = BehaviourSingletonInit<GGNotificationCenter>.instance;
		}

		private void LoadStartLayer()
		{
			ObjectDefinition objectByName = GetObjectByName(layerToLoadAtStart);
			if (objectByName != null)
			{
				Push(objectByName.gameObject);
			}
		}

		public void Push(MonoBehaviour behaviour, bool isModal = false)
		{
			if (!(behaviour == null))
			{
				Push(behaviour.gameObject, isModal);
			}
		}

		public void Push(GameObject screen, bool isModal = false)
		{
			HistoryObject currentScreen = CurrentScreen;
			if (!isModal)
			{
				currentScreen?.Hide();
			}
			HistoryObject historyObject = new HistoryObject();
			historyObject.gameObject = screen;
			history.Add(historyObject);
			historyObject.Show();
		}

		public void PopMultiple(int screensToPopCount)
		{
			if (screensToPopCount > 0)
			{
				screensToPopCount = Mathf.Min(screensToPopCount, history.Count);
				for (int i = 0; i < screensToPopCount; i++)
				{
					bool activateNextScreen = i == screensToPopCount - 1;
					Pop(activateNextScreen);
				}
			}
		}

		public void Pop(bool activateNextScreen = true)
		{
			HistoryObject currentScreen = CurrentScreen;
			if (currentScreen != null)
			{
				history.RemoveAt(history.Count - 1);
				GameObject gameObject = currentScreen.gameObject;
				IRemoveFromHistoryEventListener removeFromHistoryEventListener = null;
				if (gameObject != null)
				{
					removeFromHistoryEventListener = gameObject.GetComponent<IRemoveFromHistoryEventListener>();
				}
				removeFromHistoryEventListener?.OnRemovedFromNavigationHistory();
				currentScreen.Hide();
				CurrentScreen?.SetActive(activateNextScreen);
			}
		}
		
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				TryToGoBack();
			}
		}

		private void TryToGoBack()
		{
			HistoryObject currentScreen = CurrentScreen;
			if (currentScreen != null)
			{
				UILayer component = currentScreen.gameObject.GetComponent<UILayer>();
				if (!(component == null))
				{
					component.OnGoBack(this);
				}
			}
		}

		private void _003CAwake_003Eb__15_0(bool success)
		{
			if (!success)
			{
				Application.Quit();
				return;
			}
			GGPlayerSettings.instance.Model.acceptedTermsOfService = true;
			GGPlayerSettings.instance.Save();
			Pop();
			LoadStartLayer();
		}
	}
}
