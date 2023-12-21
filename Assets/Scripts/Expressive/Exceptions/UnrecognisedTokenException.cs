using System;
using System.Runtime.Serialization;

namespace Expressive.Exceptions
{
	[Serializable]
	public sealed class UnrecognisedTokenException : Exception
	{
		private string _003CToken_003Ek__BackingField;

		public string Token
		{
			get
			{
				return _003CToken_003Ek__BackingField;
			}
			private set
			{
				_003CToken_003Ek__BackingField = value;
			}
		}

		internal UnrecognisedTokenException(string token)
			: base("Unrecognised token '" + token + "'")
		{
			Token = token;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Token", Token);
		}
	}
}
