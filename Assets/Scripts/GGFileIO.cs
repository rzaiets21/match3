using System.IO;

public class GGFileIO
{
	public static GGFileIO instance_;

	public static GGFileIO instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = new GGFileIOUnity();
			}
			return instance_;
		}
	}

	public virtual void Write(string path, string text)
	{
	}

	public virtual void Write(string path, byte[] bytes)
	{
	}

	public virtual string ReadText(string path)
	{
		return null;
	}

	public virtual byte[] Read(string path)
	{
		return null;
	}

	public virtual bool FileExists(string path)
	{
		return false;
	}

	public virtual Stream FileReadStream(string path)
	{
		byte[] array = Read(path);
		return new MemoryStream(array, 0, array.Length);
	}
}
