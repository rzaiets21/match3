using ProtoModels;
using System.Collections.Generic;
using UnityEngine;

public class GGMessageHandlerConfig : ScriptableObject
{
	public class GGServerMessageHandlerBase
	{
		public virtual void Execute(ServerMessages.GGServerMessage.Attachment attachment)
		{
			GGDebug.DebugLog("Default message");
		}
	}

	private static GGMessageHandlerConfig instance_;

	public static GGMessageHandlerConfig instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = (Resources.Load("GGServerAssets/GGMessageHandlerConfig", typeof(GGMessageHandlerConfig)) as GGMessageHandlerConfig);
			}
			if (instance_ == null)
			{
				UnityEngine.Debug.LogError("No message config defined");
				instance_ = new GGMessageHandlerConfig();
			}
			return instance_;
		}
	}

	public virtual Dictionary<string, GGServerMessageHandlerBase> GetHandlers()
	{
		return new Dictionary<string, GGServerMessageHandlerBase>();
	}
}
