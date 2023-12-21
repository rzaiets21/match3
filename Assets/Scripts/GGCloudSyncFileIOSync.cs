using ProtoModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class GGCloudSyncFileIOSync : GGFileIOCloudSync
{
	public delegate void OnNewCloudData(GGCloudSyncFileIOSync sync);

	private sealed class _003CDoSyncWithDelay_003Ed__29 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGCloudSyncFileIOSync _003C_003E4__this;

		private float _003CcurrentDelay_003E5__2;

		private float _003CsyncDelay_003E5__3;

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
		public _003CDoSyncWithDelay_003Ed__29(int _003C_003E1__state)
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
			GGCloudSyncFileIOSync gGCloudSyncFileIOSync = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				GGDebug.DebugLog("Starting sync");
				_003CcurrentDelay_003E5__2 = 0f;
				_003CsyncDelay_003E5__3 = ((gGCloudSyncFileIOSync.syncCountForMinute < ConfigBase.instance.maxSyncFrequency) ? ConfigBase.instance.cloudSyncTimeDelay : Mathf.Max(0f, Convert.ToSingle(gGCloudSyncFileIOSync.cloudSyncStartTime.AddMinutes(1.0).Subtract(DateTime.Now).TotalSeconds)));
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003CcurrentDelay_003E5__2 < _003CsyncDelay_003E5__3)
			{
				_003CcurrentDelay_003E5__2 += Time.deltaTime;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			gGCloudSyncFileIOSync.synchronize();
			gGCloudSyncFileIOSync.isInSync = false;
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

	private string cloudSyncInfoFileName = "cloudSyncInfo.bytes";

	private CloudSyncInfo cloudSyncInfo;

	private GGCloudSyncFileIO syncFileIO;

	private bool isInSync;

	private DateTime cloudSyncStartTime = DateTime.Now;

	private int syncCountForMinute;

	public event OnNewCloudData onNewCloudData;

	public void SaveCloudSyncInfo()
	{
		ProtoIO.SaveToFile(cloudSyncInfoFileName, cloudSyncInfo);
	}

	public void Init()
	{
		isInSync = false;
		syncFileIO = new GGCloudSyncFileIO(this);
		if (!ProtoIO.LoadFromFile(cloudSyncInfoFileName, out cloudSyncInfo))
		{
			cloudSyncInfo = new CloudSyncInfo();
			cloudSyncInfo.files = new List<CloudSyncInfo.CloudSyncFileInfo>();
			SaveCloudSyncInfo();
		}
		foreach (string item in ConflictResolverBase.instance.FilesToSync())
		{
			GGDebug.DebugLog(item);
			if (GetCloudSyncFileInfo(item) == null)
			{
				GGDebug.DebugLog("info is null");
				string guid = Guid.NewGuid().ToString();
				SetGuid(item, guid);
			}
		}
		SyncWithDelay();
	}

	private CloudSyncInfo.CloudSyncFileInfo GetCloudSyncFileInfo(string filename)
	{
		if (cloudSyncInfo.files == null)
		{
			return null;
		}
		foreach (CloudSyncInfo.CloudSyncFileInfo file in cloudSyncInfo.files)
		{
			if (file.filename == filename)
			{
				return file;
			}
		}
		return null;
	}

	public override bool IsInConflict(string filename)
	{
		return GetCloudSyncFileInfo(filename)?.isInConflict ?? false;
	}

	public override GGFileIO GetDefaultFileIO()
	{
		return syncFileIO;
	}

	public override GGFileIO GetCloudFileIO()
	{
		return null;
	}

	public override void synchronize()
	{
		GGDebug.DebugLog("Sending sync");
		if (ConflictResolverBase.instance != null)
		{
			ConflictResolverBase.instance.OnBeginSync();
		}
		if (HasStagedChanges())
		{
			CloudSyncData dataListForChangesSinceLastSync = GetDataListForChangesSinceLastSync();
			GGServerRequestsBackend.UpdateCloudSyncDataRequest updateCloudSyncDataRequest = new GGServerRequestsBackend.UpdateCloudSyncDataRequest();
			updateCloudSyncDataRequest.AddData(dataListForChangesSinceLastSync);
			BehaviourSingletonInit<GGServerRequestsBackend>.instance.UpdateCSData(updateCloudSyncDataRequest, OnCloudSyncFinished);
		}
		else
		{
			GGServerRequestsBackend.GetCloudSyncDataRequest dataRequest = new GGServerRequestsBackend.GetCloudSyncDataRequest();
			BehaviourSingletonInit<GGServerRequestsBackend>.instance.GetCSData(dataRequest, OnCloudSyncFinished);
		}
	}

	public void OnCloudSyncFinished(GGServerRequestsBackend.ServerRequest request)
	{
		GGDebug.DebugLog("on cloud sync finished");
		GGServerRequestsBackend.CloudSyncRequest cloudSyncRequest = request as GGServerRequestsBackend.CloudSyncRequest;
		if (request.status != 0)
		{
			if (this.onNewCloudData != null)
			{
				this.onNewCloudData(this);
			}
			return;
		}
		CloudSyncData requestData = cloudSyncRequest.GetRequestData();
		foreach (CloudSyncData.CloudSyncFile file2 in request.GetResponse<CloudSyncData>().files)
		{
			CloudSyncInfo.CloudSyncFileInfo cloudSyncFileInfo = GetCloudSyncFileInfo(file2.key);
			if (cloudSyncFileInfo == null)
			{
				cloudSyncFileInfo = new CloudSyncInfo.CloudSyncFileInfo();
				if (cloudSyncInfo.files == null)
				{
					cloudSyncInfo.files = new List<CloudSyncInfo.CloudSyncFileInfo>();
				}
				cloudSyncFileInfo.filename = file2.key;
				cloudSyncFileInfo.localGuid = file2.guid;
				cloudSyncInfo.files.Add(cloudSyncFileInfo);
			}
			cloudSyncFileInfo.previousServerState = cloudSyncFileInfo.serverState;
			cloudSyncFileInfo.serverState = file2;
			cloudSyncFileInfo.isInConflict = (cloudSyncFileInfo.previousServerState == null || cloudSyncFileInfo.previousServerState.guid != cloudSyncFileInfo.serverState.guid);
			CloudSyncData.CloudSyncFile file = ProtoModelExtensions.GetFile(requestData, cloudSyncFileInfo.filename);
			if (file != null && file.guid == file2.guid)
			{
				cloudSyncFileInfo.isInConflict = false;
			}
		}
		SaveCloudSyncInfo();
		if (ConflictResolverBase.instance != null)
		{
			ConflictResolverBase.instance.OnConflict(this);
		}
		if (this.onNewCloudData != null)
		{
			this.onNewCloudData(this);
		}
	}

	public byte[] GetServerBytes(string filename)
	{
		CloudSyncInfo.CloudSyncFileInfo cloudSyncFileInfo = GetCloudSyncFileInfo(filename);
		if (cloudSyncFileInfo != null && cloudSyncFileInfo.serverState != null)
		{
			return Convert.FromBase64String(cloudSyncFileInfo.serverState.data);
		}
		return null;
	}

	public byte[] GetPreviousServerBytes(string filename)
	{
		CloudSyncInfo.CloudSyncFileInfo cloudSyncFileInfo = GetCloudSyncFileInfo(filename);
		if (cloudSyncFileInfo != null && cloudSyncFileInfo.serverState != null)
		{
			return Convert.FromBase64String(cloudSyncFileInfo.previousServerState.data);
		}
		return null;
	}

	public byte[] GetLocalBytes(string filename)
	{
		return syncFileIO.Read(filename);
	}

	public string GetLocalFileGuid(string filename)
	{
		CloudSyncInfo.CloudSyncFileInfo cloudSyncFileInfo = GetCloudSyncFileInfo(filename);
		if (cloudSyncFileInfo != null)
		{
			return cloudSyncFileInfo.localGuid;
		}
		return "";
	}

	public string GetServerFileGuid(string filename)
	{
		CloudSyncInfo.CloudSyncFileInfo cloudSyncFileInfo = GetCloudSyncFileInfo(filename);
		if (cloudSyncFileInfo != null && cloudSyncFileInfo.serverState != null)
		{
			return cloudSyncFileInfo.serverState.guid;
		}
		return "";
	}

	public void MarkResolved(string filename)
	{
		GetCloudSyncFileInfo(filename).isInConflict = false;
	}

	public string GetPreviousServerFileGuid(string filename)
	{
		CloudSyncInfo.CloudSyncFileInfo cloudSyncFileInfo = GetCloudSyncFileInfo(filename);
		if (cloudSyncFileInfo != null && cloudSyncFileInfo.previousServerState != null)
		{
			return cloudSyncFileInfo.previousServerState.guid;
		}
		return "";
	}

	public void SetGuid(string filename, string guid)
	{
		CloudSyncInfo.CloudSyncFileInfo cloudSyncFileInfo = GetCloudSyncFileInfo(filename);
		if (cloudSyncFileInfo == null)
		{
			cloudSyncFileInfo = new CloudSyncInfo.CloudSyncFileInfo();
			cloudSyncFileInfo.isInConflict = false;
			if (cloudSyncInfo.files == null)
			{
				cloudSyncInfo.files = new List<CloudSyncInfo.CloudSyncFileInfo>();
			}
			cloudSyncFileInfo.filename = filename;
			cloudSyncInfo.files.Add(cloudSyncFileInfo);
		}
		cloudSyncFileInfo.localGuid = guid;
		SaveCloudSyncInfo();
		SyncWithDelay();
	}

	private bool HasStagedChanges()
	{
		if (cloudSyncInfo.files == null)
		{
			return false;
		}
		foreach (CloudSyncInfo.CloudSyncFileInfo file in cloudSyncInfo.files)
		{
			if (file.serverState == null || file.localGuid != file.serverState.guid)
			{
				return true;
			}
		}
		return false;
	}

	private CloudSyncData GetDataListForChangesSinceLastSync()
	{
		GGDebug.DebugLog("changes list");
		CloudSyncData cloudSyncData = new CloudSyncData();
		if (cloudSyncInfo.files == null)
		{
			return cloudSyncData;
		}
		foreach (CloudSyncInfo.CloudSyncFileInfo file in cloudSyncInfo.files)
		{
			if (file.serverState == null || file.localGuid != file.serverState.guid)
			{
				GGDebug.DebugLog(file.filename);
				CloudSyncData.CloudSyncFile cloudSyncFile = new CloudSyncData.CloudSyncFile();
				cloudSyncFile.key = file.filename;
				cloudSyncFile.guid = file.localGuid;
				cloudSyncFile.revision = ((file.serverState != null) ? (file.serverState.revision + 1) : 0);
				byte[] array = syncFileIO.Read(file.filename);
				cloudSyncFile.data = ((array != null) ? Convert.ToBase64String(array, 0, array.Length) : "");
				GGDebug.DebugLog(cloudSyncFile.data);
				cloudSyncData.files.Add(cloudSyncFile);
			}
		}
		return cloudSyncData;
	}

	public void SyncWithDelay()
	{
		if (!isInSync)
		{
			isInSync = true;
			syncCountForMinute++;
			if (cloudSyncStartTime.AddMinutes(1.0) < DateTime.Now)
			{
				syncCountForMinute = 0;
				cloudSyncStartTime = DateTime.Now;
			}
			StartCoroutine(DoSyncWithDelay());
		}
	}

	private IEnumerator DoSyncWithDelay()
	{
		return new _003CDoSyncWithDelay_003Ed__29(0)
		{
			_003C_003E4__this = this
		};
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			SyncWithDelay();
		}
	}
}
