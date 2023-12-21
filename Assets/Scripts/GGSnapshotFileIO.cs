using System.IO;

public class GGSnapshotFileIO : GGFileIO
{
	private GGSnapshotCloudSync snapshotSync;

	public GGSnapshotFileIO(GGSnapshotCloudSync snapshotSync)
	{
		this.snapshotSync = snapshotSync;
	}

	public override void Write(string path, string text)
	{
		GGFileIO.instance.Write(path, text);
		snapshotSync.UpdateSnapshot();
	}

	public override void Write(string path, byte[] bytes)
	{
		GGFileIO.instance.Write(path, bytes);
		snapshotSync.UpdateSnapshot();
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

	public override Stream FileReadStream(string path)
	{
		return GGFileIO.instance.FileReadStream(path);
	}
}
