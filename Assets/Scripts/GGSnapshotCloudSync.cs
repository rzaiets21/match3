using ProtoModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class GGSnapshotCloudSync : GGFileIOCloudSync
{
	public delegate void OnConflict(CloudSyncConflict conflict);

	public delegate bool CanUseCloudSyncDelegate();

	private class PendingSynchronizeNowRequests
	{
		public GGServerRequestsBackend.OnComplete onComplete;
	}

	public class CloudSyncConflict
	{
		private GGServerRequestsBackend.CloudSyncRequest conflictRequest;

		private bool _003CisResolved_003Ek__BackingField;

		public CloudSyncData serverData => conflictRequest.GetResponse<CloudSyncData>();

		public bool isResolved
		{
			get
			{
				return _003CisResolved_003Ek__BackingField;
			}
			protected set
			{
				_003CisResolved_003Ek__BackingField = value;
			}
		}

		public CloudSyncConflict(GGServerRequestsBackend.CloudSyncRequest conflictRequest)
		{
			this.conflictRequest = conflictRequest;
			isResolved = false;
		}

		public void CancelConflict()
		{
			isResolved = true;
		}

		public void ResolveConflictUsingServerVersion()
		{
			if (!isResolved)
			{
				CloudSyncData response = conflictRequest.GetResponse<CloudSyncData>();
				(GGFileIOCloudSync.instance as GGSnapshotCloudSync).ResolveConflictTakeTheirs(response);
				isResolved = true;
			}
		}

		public void ResolveConflictUsingLocalVersion()
		{
			if (!isResolved)
			{
				CloudSyncData response = conflictRequest.GetResponse<CloudSyncData>();
				(GGFileIOCloudSync.instance as GGSnapshotCloudSync).ResolveConflictTakeMine(response);
				isResolved = true;
			}
		}
	}

	private sealed class _003CDoPeriodicallyTryToSync_003Ed__32 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGSnapshotCloudSync _003C_003E4__this;

		private float _003CellapsedSecondsSinceHaveLocalChanges_003E5__2;

		private float _003CcloudSyncTimeDelaySeconds_003E5__3;

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
		public _003CDoPeriodicallyTryToSync_003Ed__32(int _003C_003E1__state)
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
			GGSnapshotCloudSync gGSnapshotCloudSync = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				goto IL_0029;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00ad;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_00ed;
				}
				IL_0029:
				_003CellapsedSecondsSinceHaveLocalChanges_003E5__2 = 0f;
				_003CcloudSyncTimeDelaySeconds_003E5__3 = ((gGSnapshotCloudSync.lastFinishedRequest != null && gGSnapshotCloudSync.lastFinishedRequest.status != 0) ? ConfigBase.instance.cloudSyncTimeDelayWhenRequestFails : ConfigBase.instance.cloudSyncTimeDelay);
				goto IL_00ad;
				IL_00ed:
				if (gGSnapshotCloudSync.isSynchronizationInProgress || gGSnapshotCloudSync.isInConflict)
				{
					break;
				}
				goto IL_0029;
				IL_00ad:
				if (!gGSnapshotCloudSync.haveLocalChanges || _003CellapsedSecondsSinceHaveLocalChanges_003E5__2 < _003CcloudSyncTimeDelaySeconds_003E5__3)
				{
					if (gGSnapshotCloudSync.haveLocalChanges)
					{
						_003CellapsedSecondsSinceHaveLocalChanges_003E5__2 += RealTime.deltaTime;
					}
					else
					{
						_003CellapsedSecondsSinceHaveLocalChanges_003E5__2 = 0f;
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003CellapsedSecondsSinceHaveLocalChanges_003E5__2 = 0f;
				gGSnapshotCloudSync.SynchronizeIfPossible();
				goto IL_00ed;
			}
			_003C_003E2__current = null;
			_003C_003E1__state = 2;
			return true;
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

	private sealed class _003C_003Ec__DisplayClass39_0
	{
		public GGSnapshotCloudSync _003C_003E4__this;

		public GGServerRequestsBackend.OnComplete onComplete;

		internal void _003CDoSynchronize_003Eb__0(GGServerRequestsBackend.ServerRequest request)
		{
			_003C_003E4__this.lastFinishedRequest = request;
			_003C_003E4__this.inProgressRequest = null;
			onComplete(request);
		}
	}

	public const string MESSAGE_CONFLICT_RESOLVED = "MessageConflictResolved";

	private DateTime lastSyncTime;

	private int applicationPausedTimes;

	private int applicationPausedTimesWhenLastSync;

	public CanUseCloudSyncDelegate canUseCloudSyncDelegate;

	private List<PendingSynchronizeNowRequests> pendingSyncRequests = new List<PendingSynchronizeNowRequests>();

	private GGServerRequestsBackend.ServerRequest lastFinishedRequest;

	private GGServerRequestsBackend.CloudSyncRequest inProgressRequest;

	private CloudSyncConflict conflict;

	private string snapshotSyncFileName = "snapshotSync.bytes";

	private SnapshotSyncInfo activeSnapshot;

	private GGSnapshotFileIO syncFileIO;

	public static bool syncNeeded
	{
		get
		{
			GGSnapshotCloudSync gGSnapshotCloudSync = GGFileIOCloudSync.instance as GGSnapshotCloudSync;
			if (gGSnapshotCloudSync == null)
			{
				return false;
			}
			return gGSnapshotCloudSync.shouldTryToSyncFromMain;
		}
	}

	public bool shouldTryToSyncFromMain
	{
		get
		{
			if (!GGPlayerSettings.instance.canCloudSync)
			{
				return false;
			}
			if (applicationPausedTimes == applicationPausedTimesWhenLastSync)
			{
				return DateTime.Now - lastSyncTime > TimeSpan.FromMinutes(30.0);
			}
			return true;
		}
	}

	private bool isSynchronizationInProgress => inProgressRequest != null;

	private bool haveLocalChanges => activeSnapshot.serverAcceptedGuid != activeSnapshot.localGuid;

	private bool isInConflict
	{
		get
		{
			if (conflict != null)
			{
				return !conflict.isResolved;
			}
			return false;
		}
	}

	public event OnConflict onConflict;

	public static void StopSyncNeeded()
	{
		GGSnapshotCloudSync gGSnapshotCloudSync = GGFileIOCloudSync.instance as GGSnapshotCloudSync;
		if (!(gGSnapshotCloudSync == null))
		{
			gGSnapshotCloudSync.SyncReceieved();
		}
	}

	public static void CallOnFocus(bool pause)
	{
		GGSnapshotCloudSync gGSnapshotCloudSync = GGFileIOCloudSync.instance as GGSnapshotCloudSync;
		if (!(gGSnapshotCloudSync == null))
		{
			gGSnapshotCloudSync.OnApplicationFocus(pause);
		}
	}

	private void OnApplicationFocus(bool pause)
	{
		if (!pause)
		{
			applicationPausedTimes++;
		}
	}

	public void SaveActiveSnapshot()
	{
		ProtoIO.SaveToFile(snapshotSyncFileName, activeSnapshot);
	}

	public void Init()
	{
		syncFileIO = new GGSnapshotFileIO(this);
		if (!ProtoIO.LoadFromFile(snapshotSyncFileName, out activeSnapshot))
		{
			activeSnapshot = new SnapshotSyncInfo();
			activeSnapshot.localSnapVersion = -1;
			activeSnapshot.serverSnapSentVersion = -1;
			activeSnapshot.serverSnapAcceptedVersion = -1;
			SaveActiveSnapshot();
		}
		StartCoroutine(DoPeriodicallyTryToSync());
	}

	public override GGFileIO GetDefaultFileIO()
	{
		if (GGPlayerSettings.instance.canCloudSync)
		{
			return syncFileIO;
		}
		return GGFileIO.instance;
	}

	public override GGFileIO GetCloudFileIO()
	{
		return syncFileIO;
	}

	public void UpdateSnapshot()
	{
		activeSnapshot.localGuid = Guid.NewGuid().ToString();
		SaveActiveSnapshot();
	}

	private IEnumerator DoPeriodicallyTryToSync()
	{
		return new _003CDoPeriodicallyTryToSync_003Ed__32(0)
		{
			_003C_003E4__this = this
		};
	}

	private void SynchronizeIfPossible()
	{
		if (!isSynchronizationInProgress && !isInConflict)
		{
			SynchronizeNow(HandleSyncRequestResult);
		}
	}

	public void SynchronizeNow(GGServerRequestsBackend.OnComplete onComplete)
	{
		UnityEngine.Debug.Log("Sync now");
		if (isSynchronizationInProgress)
		{
			PendingSynchronizeNowRequests item = new PendingSynchronizeNowRequests
			{
				onComplete = onComplete
			};
			pendingSyncRequests.Add(item);
		}
		else
		{
			DoSynchronize(onComplete);
		}
	}

	private void DoSynchronize(GGServerRequestsBackend.OnComplete onComplete)
	{
		_003C_003Ec__DisplayClass39_0 _003C_003Ec__DisplayClass39_ = new _003C_003Ec__DisplayClass39_0();
		_003C_003Ec__DisplayClass39_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass39_.onComplete = onComplete;
		inProgressRequest = CreateRequest();
		BehaviourSingletonInit<GGServerRequestsBackend>.instance.GetCSData(inProgressRequest, _003C_003Ec__DisplayClass39_._003CDoSynchronize_003Eb__0);
	}

	private GGServerRequestsBackend.CloudSyncRequest CreateRequest()
	{
		if (isInConflict)
		{
			conflict.CancelConflict();
		}
		if (haveLocalChanges)
		{
			UnityEngine.Debug.Log("cloud sync update");
			List<string> files = ConflictResolverBase.instance.FilesToSync();
			CloudSyncData snapshotForSync = GetSnapshotForSync(files);
			GGServerRequestsBackend.UpdateCloudSyncDataRequest updateCloudSyncDataRequest = new GGServerRequestsBackend.UpdateCloudSyncDataRequest();
			updateCloudSyncDataRequest.AddData(snapshotForSync);
			UnityEngine.Debug.Log("Sending local changes " + activeSnapshot.serverSnapAcceptedVersion + 1);
			updateCloudSyncDataRequest.SetVersionInfo(activeSnapshot.serverSnapAcceptedVersion + 1, activeSnapshot.localGuid);
			return updateCloudSyncDataRequest;
		}
		UnityEngine.Debug.Log("cloud sync get");
		GGServerRequestsBackend.GetCloudSyncDataRequest getCloudSyncDataRequest = new GGServerRequestsBackend.GetCloudSyncDataRequest();
		getCloudSyncDataRequest.SetVersionInfo(activeSnapshot.serverSnapAcceptedVersion, activeSnapshot.localGuid);
		return getCloudSyncDataRequest;
	}

	private void SyncReceieved()
	{
		lastSyncTime = DateTime.Now;
		applicationPausedTimesWhenLastSync = applicationPausedTimes;
	}

	public void HandleSyncRequestResult(GGServerRequestsBackend.ServerRequest request)
	{
		UnityEngine.Debug.Log("on cloud sync finished");
		GGServerRequestsBackend.CloudSyncRequest cloudSyncRequest = request as GGServerRequestsBackend.CloudSyncRequest;
		SyncReceieved();
		if (request.status != 0)
		{
			HandlePendingRequests();
			return;
		}
		CloudSyncData response = request.GetResponse<CloudSyncData>();
		UnityEngine.Debug.Log(activeSnapshot.serverSnapAcceptedVersion + " " + cloudSyncRequest.snapshotId + " " + response.snapshotId + " server GUID " + response.snapshotGUID);
		bool flag = response.snapshotGUID == cloudSyncRequest.snapshotGUID;
		bool num = cloudSyncRequest is GGServerRequestsBackend.UpdateCloudSyncDataRequest;
		bool flag2 = cloudSyncRequest.snapshotGUID != activeSnapshot.localGuid;
		bool flag3 = response.snapshotId > cloudSyncRequest.snapshotId;
		bool flag4 = !string.IsNullOrEmpty(response.snapshotGUID);
		if ((!num && !flag2) & flag3)
		{
			UnityEngine.Debug.Log("shouldTakeNewServerVesion");
			ResolveConflictTakeTheirs(response);
		}
		else if (!flag && flag4)
		{
			UnityEngine.Debug.Log("is in conflict");
			SetConflictedState(cloudSyncRequest);
		}
		else
		{
			UnityEngine.Debug.Log("not in conflict");
			UpdateLastKnownServerValues(response.snapshotId, response.snapshotGUID, activeSnapshot.localGuid);
		}
		HandlePendingRequests();
		if (isInConflict && !isSynchronizationInProgress && !ConflictResolverBase.instance.ResolveConflict(conflict))
		{
			this.onConflict?.Invoke(conflict);
		}
	}

	public void HandlePendingRequests()
	{
		if (pendingSyncRequests.Count > 0)
		{
			PendingSynchronizeNowRequests pendingSynchronizeNowRequests = pendingSyncRequests[0];
			pendingSyncRequests.RemoveAt(0);
			DoSynchronize(pendingSynchronizeNowRequests.onComplete);
		}
	}

	private void UpdateLastKnownServerValues(int serverSnapshotId, string serverSnapshotGUID, string localGUID)
	{
		activeSnapshot.serverSnapAcceptedVersion = serverSnapshotId;
		activeSnapshot.serverAcceptedGuid = serverSnapshotGUID;
		activeSnapshot.localGuid = localGUID;
		SaveActiveSnapshot();
	}

	private void SetConflictedState(GGServerRequestsBackend.CloudSyncRequest conflictedRequest)
	{
		if (conflict != null)
		{
			conflict.CancelConflict();
		}
		conflict = new CloudSyncConflict(conflictedRequest);
	}

	public void ResolveConflictTakeTheirs(CloudSyncData serverResponseData)
	{
		foreach (CloudSyncData.CloudSyncFile file in serverResponseData.files)
		{
			byte[] bytes = Convert.FromBase64String(file.data);
			GGFileIO.instance.Write(file.key, bytes);
		}
		UpdateLastKnownServerValues(serverResponseData.snapshotId, serverResponseData.snapshotGUID, serverResponseData.snapshotGUID);
		ConflictResolverBase.instance.OnConflict();
		BehaviourSingletonInit<GGNotificationCenter>.instance.Broadcast("MessageConflictResolved");
	}

	public void ResolveConflictTakeMine(CloudSyncData serverResponseData)
	{
		UpdateLastKnownServerValues(serverResponseData.snapshotId, serverResponseData.snapshotGUID, serverResponseData.snapshotGUID);
		SynchronizeNow(HandleSyncRequestResult);
		synchronize();
		BehaviourSingletonInit<GGNotificationCenter>.instance.Broadcast("MessageConflictResolved");
	}

	private CloudSyncData GetSnapshotForSync(List<string> files)
	{
		CloudSyncData cloudSyncData = new CloudSyncData();
		cloudSyncData.snapshotId = activeSnapshot.localSnapVersion;
		cloudSyncData.snapshotGUID = activeSnapshot.localGuid;
		foreach (string file in files)
		{
			CloudSyncData.CloudSyncFile cloudSyncFile = new CloudSyncData.CloudSyncFile();
			cloudSyncFile.key = file;
			cloudSyncFile.guid = "";
			cloudSyncFile.revision = 0;
			byte[] array = syncFileIO.Read(file);
			cloudSyncFile.data = ((array != null) ? Convert.ToBase64String(array, 0, array.Length) : "");
			cloudSyncData.files.Add(cloudSyncFile);
		}
		return cloudSyncData;
	}
}
