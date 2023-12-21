using System;
using System.Collections.Generic;
using UnityEngine;

public class PluginCallbackManager : BehaviourSingletonInit<PluginCallbackManager>
{
	public class Response
	{
		[Serializable]
		public class BaseParameters
		{
			public string callback_id;
		}

		public string jsonResponse;

		public BaseParameters baseParameters;

		public Response(BaseParameters baseParameters, string jsonResponse)
		{
			this.baseParameters = baseParameters;
			this.jsonResponse = jsonResponse;
		}
	}

	public delegate void CallbackDelegate(Response msg);

	private Dictionary<string, CallbackDelegate> callbacks = new Dictionary<string, CallbackDelegate>();

	private static int nextCallbackId;

	public string RegisterCallback(CallbackDelegate callback)
	{
		nextCallbackId++;
		string text = nextCallbackId.ToString();
		callbacks.Add(text, callback);
		return text;
	}

	public void OnCallCallback(string jsonMessage)
	{
		try
		{
			UnityEngine.Debug.Log("RECEIEVED: \"" + jsonMessage + "\"");
			Response.BaseParameters baseParameters = JsonUtility.FromJson<Response.BaseParameters>(jsonMessage);
			if (baseParameters != null)
			{
				string callback_id = baseParameters.callback_id;
				CallbackDelegate value = null;
				if (callbacks.TryGetValue(callback_id, out value))
				{
					callbacks.Remove(callback_id);
					value?.Invoke(new Response(baseParameters, jsonMessage));
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
