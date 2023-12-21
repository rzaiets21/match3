using System;

public class GGCloudSyncFileIO : GGFileIO
{
	private GGCloudSyncFileIOSync sync;

	public GGCloudSyncFileIO(GGCloudSyncFileIOSync ggSync)
	{
		sync = ggSync;
	}

	public override void Write(string path, string text)
	{
		GGDebug.DebugLog("CloudSyncWrite");
		GGFileIO.instance.Write(path, text);
		string guid = Guid.NewGuid().ToString();
		sync.SetGuid(path, guid);
	}

	public override void Write(string path, byte[] bytes)
	{
		GGDebug.DebugLog("CloudSyncWrite");
		GGFileIO.instance.Write(path, bytes);
		string guid = Guid.NewGuid().ToString();
		sync.SetGuid(path, guid);
	}

	public override string ReadText(string path)
	{
		return GGFileIO.instance.ReadText(path);
	}

	public override byte[] Read(string path)
	{
		if (FileExists(path))
		{
			return GGFileIO.instance.Read(path);
		}
		return null;
	}

	public override bool FileExists(string path)
	{
		return GGFileIO.instance.FileExists(path);
	}
}
