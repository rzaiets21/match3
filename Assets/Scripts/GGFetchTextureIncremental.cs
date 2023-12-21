using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GGFetchTextureIncremental : BehaviourSingletonInit<GGFetchTextureIncremental>
{
	public class TextureResult
	{
		public int textureSetId;

		public Texture2D texture;
	}

	public class TextureRequest
	{
		public int textureSetId;

		public string url;
	}

	public delegate void OnIncrementComplete(List<TextureResult> textureIncrement, bool isFinished);

	private class Request
	{
		public List<TextureRequest> request;

		public OnIncrementComplete incrementCallback;

		public List<TextureResult> result;

		public int currentIncrement;
	}

	private sealed class _003CQueryTexture_003Ed__17 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public TextureRequest request;

		public GGFetchTextureIncremental _003C_003E4__this;

		private TextureResult _003CtextureResult_003E5__2;

		private WWW _003Cwww_003E5__3;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CQueryTexture_003Ed__17(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			GGFetchTextureIncremental gGFetchTextureIncremental = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CtextureResult_003E5__2 = new TextureResult
				{
					textureSetId = request.textureSetId
				};
				_003CtextureResult_003E5__2.texture = gGFetchTextureIncremental.TryGetFromCache(request.url);
				if (_003CtextureResult_003E5__2.texture != null)
				{
					gGFetchTextureIncremental.ReceiveTexture(_003CtextureResult_003E5__2);
					return false;
				}
				_003Cwww_003E5__3 = new WWW(request.url);
				_003C_003E2__current = _003Cwww_003E5__3;
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				gGFetchTextureIncremental.TryCacheTexture(_003Cwww_003E5__3);
				_003CtextureResult_003E5__2.texture = (string.IsNullOrEmpty(_003Cwww_003E5__3.error) ? _003Cwww_003E5__3.texture : null);
				gGFetchTextureIncremental.ReceiveTexture(_003CtextureResult_003E5__2);
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	public int texturesInOneIncrement = 4;

	public TimeSpan cacheTimeToLive = TimeSpan.FromHours(6.0);

	private int nextUnusedTicket;

	private Dictionary<int, Request> requests = new Dictionary<int, Request>();

	private Queue idTickets = new Queue();

	private int currentTicket = -1;

	public override void Init()
	{
		GGDebug.DebugLog("clear cache");
		Caching.ClearCache();
	}

	public int GetTicketForFetchTexturesQueue(List<TextureRequest> request, OnIncrementComplete incrementCallback)
	{
		GGDebug.DebugLog(request.Count);
		int num = nextUnusedTicket++;
		QueueNewRequestRecord(request, incrementCallback, num);
		if (currentTicket < 0)
		{
			BeginProcessingNewRequest();
		}
		return num;
	}

	private void QueueNewRequestRecord(List<TextureRequest> request, OnIncrementComplete incrementCallback, int requestId)
	{
		Request value = new Request
		{
			request = request,
			incrementCallback = incrementCallback,
			currentIncrement = 0
		};
		requests.Add(requestId, value);
		idTickets.Enqueue(requestId);
	}

	private void BeginProcessingNewRequest()
	{
		currentTicket = GetNextTicket();
		if (currentTicket >= 0)
		{
			TryFetchIncrement();
		}
	}

	private int GetNextTicket()
	{
		int num;
		do
		{
			if (idTickets.Count <= 0)
			{
				return -1;
			}
			num = (int)idTickets.Dequeue();
		}
		while (!requests.ContainsKey(num));
		return num;
	}

	private void TryFetchIncrement()
	{
		requests.TryGetValue(currentTicket, out Request value);
		if (value.request.Count > 0)
		{
			FetchIncrement();
		}
		else
		{
			OnIncrementGetFinished();
		}
	}

	private void FetchIncrement()
	{
		requests.TryGetValue(currentTicket, out Request value);
		value.result = new List<TextureResult>();
		int num = value.currentIncrement * texturesInOneIncrement;
		for (int i = num; i < num + texturesInOneIncrement; i++)
		{
			if (i < value.request.Count)
			{
				StartCoroutine(QueryTexture(value.request[i]));
			}
		}
	}

	private IEnumerator QueryTexture(TextureRequest request)
	{
		return new _003CQueryTexture_003Ed__17(0)
		{
			_003C_003E4__this = this,
			request = request
		};
	}

	private Texture2D TryGetFromCache(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		return Singleton<GGRequestCache>.Instance.Get<Texture2D>(key);
	}

	private void TryCacheTexture(WWW query)
	{
		if (string.IsNullOrEmpty(query.error) && !string.IsNullOrEmpty(query.url) && query.texture != null)
		{
			Singleton<GGRequestCache>.Instance.Put(query.url, query.texture, cacheTimeToLive);
		}
	}

	private void ReceiveTexture(TextureResult result)
	{
		requests.TryGetValue(currentTicket, out Request value);
		value.result.Add(result);
		if (IsIncrementProcessingFinished())
		{
			OnIncrementGetFinished();
		}
	}

	private bool IsIncrementProcessingFinished()
	{
		requests.TryGetValue(currentTicket, out Request value);
		int b = value.request.Count - value.currentIncrement * texturesInOneIncrement;
		return value.result.Count >= Mathf.Min(texturesInOneIncrement, b);
	}

	private void OnIncrementGetFinished()
	{
		requests.TryGetValue(currentTicket, out Request value);
		value.currentIncrement++;
		if (value.incrementCallback != null)
		{
			value.incrementCallback(value.result, IsCurrentRequestProcessed());
		}
		AdvanceRequestsToNextIncrement();
	}

	private void AdvanceRequestsToNextIncrement()
	{
		if (IsCurrentRequestProcessed())
		{
			RemoveRequest(currentTicket);
			BeginProcessingNewRequest();
		}
		else
		{
			FetchIncrement();
		}
	}

	private bool IsCurrentRequestProcessed()
	{
		requests.TryGetValue(currentTicket, out Request value);
		return value.currentIncrement * texturesInOneIncrement >= value.request.Count;
	}

	public void StopRequest(int requestTicket)
	{
		StopCoroutinesForTicket(requestTicket);
		RemoveRequest(requestTicket);
		BeginProcessingNewRequest();
	}

	private void StopCoroutinesForTicket(int requestTicket)
	{
		if (requestTicket == currentTicket)
		{
			StopAllCoroutines();
		}
	}

	private void RemoveRequest(int requestTicket)
	{
		if (requests.ContainsKey(requestTicket))
		{
			requests.Remove(requestTicket);
		}
	}
}
