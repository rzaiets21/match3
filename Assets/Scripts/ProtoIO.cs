using ProtoBuf.Meta;
using System;
using System.IO;

public class ProtoIO
{
	public static bool LoadFromFile<T>(string filename, out T model) where T : class
	{
		return LoadFromFile<ProtoSerializer, T>(filename, GGFileIO.instance, out model);
	}

	public static bool LoadFromFileLocal<T>(string filename, out T model) where T : class
	{
		return LoadFromFile<ProtoSerializer, T>(filename, GGFileIO.instance, out model);
	}

	public static bool LoadFromFileLocal<S, T>(string filename, out T model) where S : TypeModel, new()where T : class
	{
		return LoadFromFile<S, T>(filename, GGFileIO.instance, out model);
	}

	public static bool LoadFromFile<S, T>(string filename, GGFileIO fileIO, out T model) where S : TypeModel, new()where T : class
	{
		model = null;
		if (!GGFileIO.instance.FileExists(filename))
		{
			return false;
		}
		try
		{
			S val = new S();
			using (Stream source = fileIO.FileReadStream(filename))
			{
				model = (val.Deserialize(source, null, typeof(T)) as T);
			}
			return true;
		}
		catch
		{
		}
		return false;
	}

	public static bool SaveToFile<T>(string filename, T model) where T : new()
	{
		return SaveToFile<ProtoSerializer, T>(filename, GGFileIO.instance, model);
	}

	public static bool SaveToFileCS<T>(string filename, T model) where T : new()
	{
		return SaveToFile<ProtoSerializer, T>(filename, GGFileIOCloudSync.instance.GetDefaultFileIO(), model);
	}

	public static bool SaveToFile<S, T>(string filename, GGFileIO fileIO, T model) where S : TypeModel, new()where T : new()
	{
		S val = new S();
		if (model == null)
		{
			model = new T();
		}
		try
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				val.Serialize(memoryStream, model);
				memoryStream.Flush();
				fileIO.Write(filename, memoryStream.ToArray());
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static byte[] SerializeToByteArray<S, T>(T model) where S : TypeModel, new()where T : new()
	{
		S val = new S();
		if (model == null)
		{
			model = new T();
		}
		try
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				val.Serialize(memoryStream, model);
				memoryStream.Flush();
				return memoryStream.ToArray();
			}
		}
		catch
		{
			return null;
		}
	}

	public static bool LoadFromByteStream<T>(byte[] encoded, out T model) where T : class
	{
		return LoadFromByteStream<ProtoSerializer, T>(encoded, out model);
	}

	private static bool LoadFromByteStream<S, T>(byte[] encoded, out T model) where S : TypeModel, new()where T : class
	{
		model = null;
		MemoryStream memoryStream = new MemoryStream(encoded);
		memoryStream.SetLength(encoded.Length);
		memoryStream.Capacity = encoded.Length;
		S val = new S();
		try
		{
			model = (val.Deserialize(memoryStream, null, typeof(T)) as T);
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static string SerializeToByte64<T>(T Model) where T : class
	{
		MemoryStream memoryStream = new MemoryStream();
		new ProtoSerializer().Serialize(memoryStream, Model);
		return Convert.ToBase64String(memoryStream.ToArray(), 0, (int)memoryStream.Length);
	}

	public static bool LoadFromBase64String<T>(string base64String, out T model) where T : class
	{
		return LoadFromBase64String<ProtoSerializer, T>(base64String, out model);
	}

	private static bool LoadFromBase64String<S, T>(string base64String, out T model) where S : TypeModel, new()where T : class
	{
		byte[] array = null;
		model = null;
		try
		{
			array = Convert.FromBase64String(base64String);
		}
		catch
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream(array);
		memoryStream.SetLength(array.Length);
		memoryStream.Capacity = array.Length;
		S val = new S();
		try
		{
			model = (val.Deserialize(memoryStream, null, typeof(T)) as T);
		}
		catch
		{
			return false;
		}
		return true;
	}

	public static T Clone<T>(T model) where T : class, new()
	{
		if (model == null)
		{
			return null;
		}
		byte[] encoded = SerializeToByteArray<ProtoSerializer, T>(model);
		T model2 = new T();
		LoadFromByteStream(encoded, out model2);
		return model2;
	}
}
