using System;
using System.Runtime.Serialization;

namespace Expressive.Exceptions
{
	[Serializable]
	public sealed class MissingTokenException : Exception
	{
		private char _003CMissingToken_003Ek__BackingField;

		public char MissingToken
		{
			get
			{
				return _003CMissingToken_003Ek__BackingField;
			}
			private set
			{
				_003CMissingToken_003Ek__BackingField = value;
			}
		}

		internal MissingTokenException(string message, char missingToken)
			: base(message)
		{
			MissingToken = missingToken;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("MissingToken", MissingToken);
		}
	}
}
