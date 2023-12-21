using System;
using System.Runtime.Serialization;

namespace Expressive.Exceptions
{
	[Serializable]
	public sealed class FunctionNameAlreadyRegisteredException : Exception
	{
		private string _003CName_003Ek__BackingField;

		public string Name
		{
			get
			{
				return _003CName_003Ek__BackingField;
			}
			private set
			{
				_003CName_003Ek__BackingField = value;
			}
		}

		internal FunctionNameAlreadyRegisteredException(string name)
			: base("A function has already been registered '" + name + "'")
		{
			Name = name;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Name", Name);
		}
	}
}
