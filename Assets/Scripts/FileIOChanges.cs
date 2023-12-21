using System;
using System.Threading;
using UnityEngine;

public class FileIOChanges : SingletonInit<FileIOChanges>
{
	public delegate void OnDataChangedDelegate();

	private event OnDataChangedDelegate onDataChanged;

	public void OnChange(OnDataChangedDelegate dataChangeDelegate)
	{
		onDataChanged -= dataChangeDelegate;
		onDataChanged += dataChangeDelegate;
	}

	public override void Init()
	{
		BehaviourSingletonInit<GGNotificationCenter>.instance.onMessage += OnMessage;
	}

	private void OnMessage(string message)
	{
		if (message == "MessageConflictResolved")
		{
			ReloadModels();
		}
	}

	private void ReloadModels()
	{
		UnityEngine.Debug.Log("Reload Models");
		if (this.onDataChanged != null)
		{
			this.onDataChanged();
		}
	}
}
