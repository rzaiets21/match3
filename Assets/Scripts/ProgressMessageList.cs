using System.Collections.Generic;

public class ProgressMessageList
{
	public class ProgressChange
	{
		public float fromProgress;

		public float toProgress;
	}

	public ProgressChange progressChange;

	public List<string> messageList;
}
