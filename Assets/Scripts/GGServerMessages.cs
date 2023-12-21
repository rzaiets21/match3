using ProtoModels;
using System;
using System.Collections.Generic;
using System.Threading;

public class GGServerMessages : BehaviourSingletonInit<GGServerMessages>
{
	public delegate void OnMessageExecuted();

	private Dictionary<string, GGMessageHandlerConfig.GGServerMessageHandlerBase> messageHandlers;

	private GGMessageHandlerConfig.GGServerMessageHandlerBase defaultHandler = new GGMessageHandlerConfig.GGServerMessageHandlerBase();

	public event OnMessageExecuted onMessageExecuted;

	public override void Init()
	{
		messageHandlers = GGMessageHandlerConfig.instance.GetHandlers();
	}

	public void GetMessages(GGServerRequestsBackend.OnComplete onComplete)
	{
		GGServerRequestsBackend.GGServerPlayerMessages messagesRequest = new GGServerRequestsBackend.GGServerPlayerMessages();
		BehaviourSingletonInit<GGServerRequestsBackend>.instance.GetMessagesForPlayer(messagesRequest, onComplete);
	}

	public void ExecuteMessageAttachment(string key, ServerMessages.GGServerMessage.Attachment attachment)
	{
		GGDebug.DebugLog(key);
		messageHandlers.TryGetValue(key, out GGMessageHandlerConfig.GGServerMessageHandlerBase value);
		if (value != null)
		{
			value.Execute(attachment);
		}
		else
		{
			defaultHandler.Execute(attachment);
		}
		if (this.onMessageExecuted != null)
		{
			this.onMessageExecuted();
		}
	}
}
