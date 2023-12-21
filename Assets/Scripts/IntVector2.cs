using System;
using Newtonsoft.Json;
using UnityEngine;

namespace GGMatch3
{
	[Serializable]
	public struct IntVector2
	{
		public int x;

		public int y;

		[JsonIgnore] public static IntVector2[] upDownLeftRight = new IntVector2[4]
		{
			up,
			down,
			left,
			right
		};

		[JsonIgnore] public static IntVector2[] upDown = new IntVector2[2]
		{
			up,
			down
		};

		[JsonIgnore] public static IntVector2[] leftRight = new IntVector2[2]
		{
			left,
			right
		};

		[JsonIgnore] public IntVector2 orthogonal => new IntVector2(y, x);

		[JsonIgnore] public static IntVector2 up => new IntVector2(0, 1);

		[JsonIgnore] public static IntVector2 down => new IntVector2(0, -1);

		[JsonIgnore] public static IntVector2 zero => new IntVector2(0, 0);

		[JsonIgnore] public static IntVector2 left => new IntVector2(-1, 0);

		[JsonIgnore] public static IntVector2 right => new IntVector2(1, 0);

		public IntVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x, y, 0f);
		}

		public static IntVector2 operator +(IntVector2 a, IntVector2 b)
		{
			IntVector2 result = default(IntVector2);
			result.x = a.x + b.x;
			result.y = a.y + b.y;
			return result;
		}

		public static IntVector2 operator -(IntVector2 a, IntVector2 b)
		{
			IntVector2 result = default(IntVector2);
			result.x = a.x - b.x;
			result.y = a.y - b.y;
			return result;
		}

		public static bool operator ==(IntVector2 a, IntVector2 b)
		{
			if (a.x == b.x)
			{
				return a.y == b.y;
			}
			return false;
		}

		public static IntVector2 operator *(IntVector2 a, int b)
		{
			IntVector2 result = default(IntVector2);
			result.x = a.x * b;
			result.y = a.y * b;
			return result;
		}

		public static IntVector2 operator *(int b, IntVector2 a)
		{
			IntVector2 result = default(IntVector2);
			result.x = a.x * b;
			result.y = a.y * b;
			return result;
		}

		public static bool operator !=(IntVector2 a, IntVector2 b)
		{
			if (a.x == b.x)
			{
				return a.y != b.y;
			}
			return true;
		}

		public static IntVector2 operator -(IntVector2 a)
		{
			IntVector2 result = default(IntVector2);
			result.x = -a.x;
			result.y = -a.y;
			return result;
		}

		public override string ToString()
		{
			return $"({x},{y})";
		}
	}
}
