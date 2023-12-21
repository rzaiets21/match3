using ProtoModels;

public static class ProtoModelExtensions
{
	public static CloudSyncData.CloudSyncFile GetFile(CloudSyncData request, string key)
	{
		if (request == null)
		{
			return null;
		}
		foreach (CloudSyncData.CloudSyncFile file in request.files)
		{
			if (file.key == key)
			{
				return file;
			}
		}
		return null;
	}
}
