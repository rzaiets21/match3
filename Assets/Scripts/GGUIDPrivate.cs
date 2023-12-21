using System;

public class GGUIDPrivate
{
	protected static string uid;

	protected static string filePath => "gguid_private.txt";

	public static void Reset()
	{
		uid = Guid.NewGuid().ToString();
		Save();
	}

	protected static void Save()
	{
		try
		{
			GGFileIO.instance.Write(filePath, uid);
		}
		catch
		{
		}
	}

	public static string InstallId()
	{
		if (!string.IsNullOrEmpty(uid))
		{
			return uid;
		}
		if (!GGFileIO.instance.FileExists(filePath))
		{
			uid = Guid.NewGuid().ToString();
			Save();
		}
		else
		{
			try
			{
				uid = GGFileIO.instance.ReadText(filePath);
			}
			catch
			{
			}
		}
		return uid;
	}
}
