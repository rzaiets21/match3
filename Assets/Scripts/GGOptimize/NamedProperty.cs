using System;

namespace GGOptimize
{
	[Serializable]
	public class NamedProperty
	{
		public string name;

		public NamedPropertyDataType dataType;

		public int intVal;

		public bool boolVal;

		public string strVal;

		public float floatVal;

		public int GetInt()
		{
			return intVal;
		}

		public bool GetBool()
		{
			return boolVal;
		}

		public string GetString()
		{
			return strVal;
		}
	}
}
