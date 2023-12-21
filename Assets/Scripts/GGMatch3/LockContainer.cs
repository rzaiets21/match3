using System.Collections.Generic;

namespace GGMatch3
{
	public class LockContainer
	{
		public List<Lock> locks = new List<Lock>();

		public Lock NewLock()
		{
			Lock @lock = new Lock();
			locks.Add(@lock);
			return @lock;
		}

		public void UnlockAllAndSaveToTemporaryList()
		{
			for (int i = 0; i < locks.Count; i++)
			{
				locks[i].UnlockAllAndSaveToTemporaryList();
			}
		}

		public void LockTemporaryListAndClear()
		{
			for (int i = 0; i < locks.Count; i++)
			{
				locks[i].LockTemporaryListAndClear();
			}
		}

		public void UnlockAll()
		{
			for (int i = 0; i < locks.Count; i++)
			{
				locks[i].UnlockAll();
			}
		}
	}
}
