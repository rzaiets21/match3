namespace Expressive
{
	internal sealed class Token
	{
		private string _003CCurrentToken_003Ek__BackingField;

		private int _003CLength_003Ek__BackingField;

		private int _003CStartIndex_003Ek__BackingField;

		internal string CurrentToken
		{
			get
			{
				return _003CCurrentToken_003Ek__BackingField;
			}
			private set
			{
				_003CCurrentToken_003Ek__BackingField = value;
			}
		}

		internal int Length
		{
			get
			{
				return _003CLength_003Ek__BackingField;
			}
			private set
			{
				_003CLength_003Ek__BackingField = value;
			}
		}

		internal int StartIndex
		{
			get
			{
				return _003CStartIndex_003Ek__BackingField;
			}
			private set
			{
				_003CStartIndex_003Ek__BackingField = value;
			}
		}

		public Token(string currentToken, int startIndex)
		{
			CurrentToken = currentToken;
			StartIndex = startIndex;
			Length = (CurrentToken?.Length ?? 0);
		}
	}
}
