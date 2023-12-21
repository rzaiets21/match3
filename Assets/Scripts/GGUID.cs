using System;

public class GGUID
{
	protected static string uid;

	protected static string filePath => "gguid.txt";

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

	public static string NewGuid()
	{
		return Guid.NewGuid().ToString();
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
