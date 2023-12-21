using UnityEngine;

public class GGFileIOCloudSync : MonoBehaviour
{
	public const string NOTIFICATION_NEW_DATA = "CloudSync.NewData";

	public const string NOTIFICATION_DATA_UPLOADED = "CloudSync.DataUploaded";

	public static GGFileIOCloudSync instance_;

	public static GGFileIOCloudSync instance
	{
		get
		{
			if (instance_ == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "GGFileIOCloudSync";
				switch (ConfigBase.instance.cloudSyncType)
				{
				case ConfigBase.GGFileIOCloudSyncTypes.WhisperSync:
					instance_ = gameObject.AddComponent<GGFileIOCloudSync>();
					break;
				case ConfigBase.GGFileIOCloudSyncTypes.GGCloudSync:
				{
					GGCloudSyncFileIOSync gGCloudSyncFileIOSync = gameObject.AddComponent<GGCloudSyncFileIOSync>();
					gGCloudSyncFileIOSync.Init();
					instance_ = gGCloudSyncFileIOSync;
					DestroyUtil.DontDestroyOnLoad(instance_.gameObject);
					break;
				}
				case ConfigBase.GGFileIOCloudSyncTypes.GGSaveLocalOnly:
					instance_ = gameObject.AddComponent<GGFileIOCloudSync>();
					break;
				case ConfigBase.GGFileIOCloudSyncTypes.GGSnapshotCloudSync:
				{
					GGSnapshotCloudSync gGSnapshotCloudSync = gameObject.AddComponent<GGSnapshotCloudSync>();
					gGSnapshotCloudSync.Init();
					instance_ = gGSnapshotCloudSync;
					DestroyUtil.DontDestroyOnLoad(instance_.gameObject);
					break;
				}
				default:
					instance_ = gameObject.AddComponent<GGFileIOCloudSync>();
					break;
				}
			}
			return instance_;
		}
	}

	public static bool isCloudSyncNotification(string message)
	{
		if (!("CloudSync.NewData" == message))
		{
			return "CloudSync.DataUploaded" == message;
		}
		return true;
	}

	public virtual GGFileIO GetDefaultFileIO()
	{
		return GGFileIO.instance;
	}

	public virtual GGFileIO GetCloudFileIO()
	{
		return GGFileIO.instance;
	}

	public virtual bool IsInConflict(string name)
	{
		return false;
	}

	public virtual void synchronize()
	{
	}
}
