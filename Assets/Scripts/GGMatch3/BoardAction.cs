namespace GGMatch3
{
	public class BoardAction
	{
		public LockContainer lockContainer = new LockContainer();

		private bool _003CisAlive_003Ek__BackingField;

		private bool _003CisStarted_003Ek__BackingField;

		public virtual bool isAlive
		{
			get
			{
				return _003CisAlive_003Ek__BackingField;
			}
			protected set
			{
				_003CisAlive_003Ek__BackingField = value;
			}
		}

		public virtual bool isStarted
		{
			get
			{
				return _003CisStarted_003Ek__BackingField;
			}
			protected set
			{
				_003CisStarted_003Ek__BackingField = value;
			}
		}

		public virtual void Reset()
		{
			isStarted = false;
		}

		public virtual void OnStart(ActionManager manager)
		{
			isAlive = true;
			isStarted = true;
		}

		public virtual void Stop()
		{
		}

		public virtual void OnUpdate(float deltaTime)
		{
		}
	}
}
