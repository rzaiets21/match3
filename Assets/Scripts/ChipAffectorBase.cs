using GGMatch3;
using System.Collections.Generic;

public class ChipAffectorBase
{
	protected LockContainer lockContainer = new LockContainer();

	private Lock globalLock_;

	protected Lock globalLock
	{
		get
		{
			if (globalLock_ == null)
			{
				globalLock_ = lockContainer.NewLock();
			}
			return globalLock_;
		}
	}

	public virtual bool canFinish => true;

	public virtual void ReleaseLocks()
	{
		lockContainer.UnlockAllAndSaveToTemporaryList();
	}

	public virtual void ApplyLocks()
	{
		lockContainer.LockTemporaryListAndClear();
	}

	public virtual void Clear()
	{
		lockContainer.UnlockAll();
	}

	public virtual void Update()
	{
	}

	public virtual void GiveLightingBoltsTo(List<LightingBolt> destinationBolts)
	{
	}

	public virtual void AddToInputAffectorExport(Match3Game.InputAffectorExport inputAffector)
	{
	}

	public virtual void OnAfterDestroy()
	{
	}
}
