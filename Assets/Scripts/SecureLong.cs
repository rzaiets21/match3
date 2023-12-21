using ProtoModels;
using System;
using UnityEngine;

public struct SecureLong
{
	private long key1;

	private long key2;

	private long key3;

	private long key4;

	private long value1;

	private long value2;

	private long value3;

	private long value4;

	private long value5;

	private long value6;

	private long value7;

	private long value8;

	private long value9;

	private long value10;

	public bool isAssigned;

	public bool isTamperedWith;

	public long valueLong => Evaluate();

	public SecureLong(ProtoModels.SecureLong model, long defaultValue)
	{
		if (model == null || !model.isAssigned)
		{
			key1 = 0L;
			key2 = 0L;
			key3 = 0L;
			key4 = 0L;
			value1 = 0L;
			value2 = 0L;
			value3 = 0L;
			value4 = 0L;
			value5 = 0L;
			value6 = 0L;
			value7 = 0L;
			value8 = 0L;
			value9 = 0L;
			value10 = 0L;
			isAssigned = false;
			isTamperedWith = false;
			Assign(defaultValue);
		}
		else
		{
			key1 = model.key1;
			key2 = model.key2;
			key3 = model.key3;
			key4 = model.key4;
			value1 = model.value1;
			value2 = model.value2;
			value3 = model.value3;
			value4 = model.value4;
			value5 = model.value5;
			value6 = model.value6;
			value7 = model.value7;
			value8 = model.value8;
			value9 = model.value9;
			value10 = model.value10;
			isAssigned = model.isAssigned;
			isTamperedWith = model.isTamperedWith;
		}
	}

	public ProtoModels.SecureLong ToModel()
	{
		return new ProtoModels.SecureLong
		{
			key1 = key1,
			key2 = key2,
			key3 = key3,
			key4 = key4,
			value1 = value1,
			value2 = value2,
			value3 = value3,
			value4 = value4,
			value5 = value5,
			value6 = value6,
			value7 = value7,
			value8 = value8,
			value9 = value9,
			value10 = value10,
			isAssigned = isAssigned,
			isTamperedWith = isTamperedWith
		};
	}

	public void ShuffleKeys()
	{
		key1 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		key2 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		key3 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		key4 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
	}

	private long Encrypt1(long value)
	{
		value += 199;
		return value;
	}

	private long Decrypt1(long value)
	{
		value -= 199;
		return value;
	}

	private long Encrypt2(long value)
	{
		value = (value ^ key1) + 88;
		return value;
	}

	private long Decrypt2(long value)
	{
		value = ((value - 88) ^ key1);
		return value;
	}

	private long Encrypt3(long value)
	{
		value = ((value + 177) ^ key2);
		return value;
	}

	private long Decrypt3(long value)
	{
		value = (value ^ key2) - 177;
		return value;
	}

	private long Encrypt4(long value)
	{
		value = (value ^ key2) + (0x17D ^ key3);
		return value;
	}

	private long Decrypt4(long value)
	{
		value = ((value - (0x17D ^ key3)) ^ key2);
		return value;
	}

	private long Encrypt5(long value)
	{
		value = UnityEngine.Random.Range(-100, 100);
		return value;
	}

	private long Encrypt6(long value)
	{
		value -= 300;
		return value;
	}

	private long Decrypt6(long value)
	{
		value += 300;
		return value;
	}

	private long Encrypt7(long value)
	{
		value = ((value + 261) ^ key4);
		return value;
	}

	private long Decrypt7(long value)
	{
		value = (value ^ key4) - 261;
		return value;
	}

	private long Encrypt8(long value)
	{
		value = UnityEngine.Random.Range(-100, 100);
		return value;
	}

	private long Encrypt9(long value)
	{
		value = UnityEngine.Random.Range(-100, 100);
		return value;
	}

	private long Encrypt10(long value)
	{
		value = UnityEngine.Random.Range(-100, 100);
		return value;
	}

	private void Assign(long value)
	{
		ShuffleKeys();
		isAssigned = true;
		value1 = Encrypt1(value);
		value2 = Encrypt2(value);
		value3 = Encrypt3(value);
		value4 = Encrypt4(value);
		value5 = Encrypt5(value);
		value6 = Encrypt6(value);
		value7 = Encrypt7(value);
		value8 = Encrypt8(value);
		value9 = Encrypt9(value);
		value10 = Encrypt10(value);
	}

	private long Evaluate()
	{
		long num = Decrypt1(value1);
		long num2 = num;
		num = Math.Min(num, Decrypt2(value2));
		if (num2 != num)
		{
			isTamperedWith = true;
		}
		long num3 = num;
		num = Math.Min(num, Decrypt3(value3));
		if (num3 != num)
		{
			isTamperedWith = true;
		}
		long num4 = num;
		num = Math.Min(num, Decrypt4(value4));
		if (num4 != num)
		{
			isTamperedWith = true;
		}
		long num5 = num;
		num = Math.Min(num, Decrypt6(value6));
		if (num5 != num)
		{
			isTamperedWith = true;
		}
		long num6 = num;
		num = Math.Min(num, Decrypt7(value7));
		if (num6 != num)
		{
			isTamperedWith = true;
		}
		return num;
	}

	public static long operator +(SecureLong s, long i)
	{
		return s.Evaluate() + i;
	}

	public static long operator -(SecureLong s, long i)
	{
		return s.Evaluate() - i;
	}

	public static implicit operator SecureLong(long value)
	{
		SecureLong result = default(SecureLong);
		result.Assign(value);
		return result;
	}
}
