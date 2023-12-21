using ProtoModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class GGServerRequestsBackend : BehaviourSingletonInit<GGServerRequestsBackend>
{
	public delegate void OnComplete(ServerRequest request);

	public class UrlBuilder
	{
		private string urlBase;

		private Dictionary<string, string> paramPairs = new Dictionary<string, string>();

		private Dictionary<string, List<string>> paramListPairs = new Dictionary<string, List<string>>();

		private string data = "";

		public static string GetTimestampAttributeName()
		{
			return "timestamp";
		}

		public UrlBuilder(string hostName)
		{
			urlBase = hostName;
			addParams(GetTimestampAttributeName(), DateTime.UtcNow.ToString());
			addParams("run_platform", "android");
		}

		public UrlBuilder addPath(string path)
		{
			urlBase += path;
			return this;
		}

		public UrlBuilder addData(string newData)
		{
			data = newData;
			return this;
		}

		public UrlBuilder addParams(string paramName, string paramVal)
		{
			paramPairs.Add(paramName, paramVal);
			return this;
		}

		public UrlBuilder addParams(string paramName, int paramVal)
		{
			paramPairs.Add(paramName, paramVal.ToString());
			return this;
		}

		public UrlBuilder addParams(string paramName, double paramVal)
		{
			paramPairs.Add(paramName, paramVal.ToString());
			return this;
		}

		public UrlBuilder addParams(string paramName, List<string> paramVal)
		{
			paramListPairs.Add(paramName, paramVal);
			return this;
		}

		public string SignAndToString(string publicKey, string privateKey)
		{
			string text = urlBase;
			paramPairs.Add("ap", publicKey);
			foreach (KeyValuePair<string, string> paramPair in paramPairs)
			{
				try
				{
					text = text + ((text == urlBase) ? "?" : "&") + paramPair.Key + "=" + Uri.EscapeDataString(paramPair.Value);
				}
				catch
				{
					UnityEngine.Debug.Log("Problem with key = " + paramPair.Key + " value: \"" + paramPair.Value + "\"");
				}
			}
			foreach (KeyValuePair<string, List<string>> paramListPair in paramListPairs)
			{
				foreach (string item in paramListPair.Value)
				{
					text = text + ((text == urlBase) ? "?" : "&") + paramListPair.Key + "=" + Uri.EscapeDataString(item);
				}
			}
			string hashSha = Hash.getHashSha256(Regex.Replace(text, "http(s)?://[^/]+", "") + data + privateKey);
			return text + "&sig=" + Uri.EscapeDataString(hashSha);
		}

		public override string ToString()
		{
			string text = urlBase;
			foreach (KeyValuePair<string, string> paramPair in paramPairs)
			{
				if (paramPair.Value != null)
				{
					text = text + ((text == urlBase) ? "?" : "&") + paramPair.Key + "=" + Uri.EscapeDataString(paramPair.Value);
				}
			}
			foreach (KeyValuePair<string, List<string>> paramListPair in paramListPairs)
			{
				foreach (string item in paramListPair.Value)
				{
					if (item != null)
					{
						text = text + ((text == urlBase) ? "?" : "&") + paramListPair.Key + "=" + Uri.EscapeDataString(item);
					}
				}
			}
			return text;
		}
	}

	public class ServerRequest
	{
		public enum RequestStatus
		{
			Success,
			Error,
			NotSent
		}

		public delegate void OnComplete(ServerRequest request);

		private sealed class _003CRequestCoroutine_003Ed__30 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ServerRequest _003C_003E4__this;

			private WWW _003Cquery_003E5__2;

			private float _003Ctime_003E5__3;

			private bool _003Ctimeout_003E5__4;

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
			public _003CRequestCoroutine_003Ed__30(int _003C_003E1__state)
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
				ServerRequest serverRequest = _003C_003E4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					serverRequest.progress = _003Cquery_003E5__2.progress;
					_003Ctime_003E5__3 += RealTime.deltaTime;
					if (_003Ctime_003E5__3 >= serverRequest.timeoutSec)
					{
						UnityEngine.Debug.Log("TIMEOUT1");
						_003Ctimeout_003E5__4 = true;
						_003Cquery_003E5__2.Dispose();
						goto IL_010d;
					}
				}
				else
				{
					_003C_003E1__state = -1;
					serverRequest.progress = 0f;
					if (!GGSupportMenu.instance.isNetworkConnected())
					{
						if (serverRequest.TryGetFromCache())
						{
							return false;
						}
						serverRequest.status = RequestStatus.Error;
						serverRequest.errorMessage = "Not Connected to internet! Please connect and try again";
						return false;
					}
					if (serverRequest.cacheGetStrategy != CacheGetStrategy.GetFromCacheOnlyIfRequestFails && serverRequest.TryGetFromCache())
					{
						return false;
					}
					_003Cquery_003E5__2 = serverRequest.CreateQuery();
					serverRequest.progress = _003Cquery_003E5__2.progress;
					_003Ctime_003E5__3 = 0f;
					_003Ctimeout_003E5__4 = false;
				}
				if (!_003Cquery_003E5__2.isDone)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_010d;
				IL_010d:
				if (!_003Ctimeout_003E5__4 && string.IsNullOrEmpty(_003Cquery_003E5__2.error) && _003Cquery_003E5__2.bytes != null)
				{
					serverRequest.status = RequestStatus.Success;
					serverRequest.ParseQueryResponse(_003Cquery_003E5__2);
					if (serverRequest.cache == CacheStategy.GetFromCache)
					{
						serverRequest.CacheResults(_003Cquery_003E5__2);
					}
					else if (serverRequest.cache == CacheStategy.CacheToFile)
					{
						serverRequest.CacheToFile(_003Cquery_003E5__2);
					}
				}
				else
				{
					UnityEngine.Debug.Log("Request Failed");
					if (serverRequest.cacheGetStrategy == CacheGetStrategy.GetFromCacheOnlyIfRequestFails && serverRequest.TryGetFromCache())
					{
						return false;
					}
					if (!_003Ctimeout_003E5__4)
					{
						serverRequest.errorMessage = _003Cquery_003E5__2.text;
						if (_003Cquery_003E5__2 != null && Application.isEditor)
						{
							UnityEngine.Debug.Log("URL: " + _003Cquery_003E5__2.url);
						}
						UnityEngine.Debug.Log("ERROR: " + serverRequest.errorMessage);
					}
					else
					{
						UnityEngine.Debug.Log("TIMEOUT");
					}
					serverRequest.status = RequestStatus.Error;
				}
				return false;
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

		public OnComplete onComplete;

		public int groupId;

		public float progress;

		public float timeoutSec = 15f;

		public CacheGetStrategy cacheGetStrategy;

		public CacheStategy cache;

		public TimeSpan cacheTimeToLive = TimeSpan.FromHours(6.0);

		public RequestStatus status = RequestStatus.NotSent;

		private string _003CerrorMessage_003Ek__BackingField;

		protected object responseObj;

		public string errorMessage
		{
			get
			{
				return _003CerrorMessage_003Ek__BackingField;
			}
			protected set
			{
				_003CerrorMessage_003Ek__BackingField = value;
			}
		}

		public T GetResponse<T>() where T : class
		{
			return responseObj as T;
		}

		protected virtual string GetUrl()
		{
			return "";
		}

		protected virtual WWW CreateQuery()
		{
			return null;
		}

		protected virtual void ParseQueryResponse(WWW query)
		{
			ParseResponse(query.bytes);
		}

		protected virtual void ParseResponse(byte[] bytes)
		{
			responseObj = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}

		public virtual string GetCacheKey(string url)
		{
			return RemoveChangingParams(url);
		}

		private string StripParamFromUrl(string url, string paramName)
		{
			int num = url.IndexOf(paramName);
			if (num == -1)
			{
				return url;
			}
			int num2 = num + paramName.Length;
			int num3 = url.IndexOf("&", num2);
			if (num3 == -1)
			{
				return url.Remove(num2);
			}
			return url.Remove(num2, num3 - num2);
		}

		private string RemoveChangingParams(string url)
		{
			url = StripParamFromUrl(url, UrlBuilder.GetTimestampAttributeName());
			url = StripParamFromUrl(url, "sig");
			return url;
		}

		protected virtual bool TryGetFromMemoryCache(string url)
		{
			byte[] array = Singleton<GGRequestCache>.Instance.Get<byte[]>(GetCacheKey(url));
			if (array != null)
			{
				status = RequestStatus.Success;
				ParseResponse(array);
			}
			return array != null;
		}

		protected virtual bool TryGetFromFileCache(string url)
		{
			byte[] array = SingletonInit<GGRequestFileCache>.Instance.Get<byte[]>(GetCacheKey(url));
			if (array != null)
			{
				status = RequestStatus.Success;
				ParseResponse(array);
			}
			return array != null;
		}

		protected virtual void CacheResults(WWW query)
		{
			Singleton<GGRequestCache>.Instance.Put(GetCacheKey(query.url), query.bytes, cacheTimeToLive);
		}

		protected virtual void CacheToFile(WWW query)
		{
			SingletonInit<GGRequestFileCache>.Instance.Put(GetCacheKey(query.url), query.bytes, cacheTimeToLive);
		}

		public virtual bool TryGetFromCache()
		{
			string url = GetUrl();
			if (cache == CacheStategy.GetFromCache)
			{
				if (TryGetFromMemoryCache(url))
				{
					status = RequestStatus.Success;
					return true;
				}
			}
			else if (cache == CacheStategy.CacheToFile && TryGetFromFileCache(url))
			{
				status = RequestStatus.Success;
				return true;
			}
			return false;
		}

		public virtual IEnumerator RequestCoroutine()
		{
			return new _003CRequestCoroutine_003Ed__30(0)
			{
				_003C_003E4__this = this
			};
		}
	}

	public class ProtoRequest<T> : ServerRequest where T : class
	{
		public T response
		{
			get
			{
				return GetResponse<T>();
			}
			set
			{
				responseObj = value;
			}
		}

		protected override WWW CreateQuery()
		{
			return null;
		}

		protected override void ParseResponse(byte[] bytes)
		{
			T model = null;
			if (ProtoIO.LoadFromByteStream(bytes, out model))
			{
				responseObj = model;
				return;
			}
			UnityEngine.Debug.Log("failed to load");
			responseObj = null;
		}
	}

	private interface NonceSetRequest
	{
		void SetNonce(string nonce);
	}

	private interface PidSetRequest
	{
		void SetPid(string pid);
	}

	public class ProtoRequestPid<T> : ProtoRequest<T>, PidSetRequest where T : class
	{
		public virtual void SetPid(string pid)
		{
		}
	}

	public class UploadLeadDataRequest : ProtoRequestPid<StatusMessage>
	{
		private string app;

		private string pid;

		private string data;

		public string eventId;

		public string dataId;

		public string nonce;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.uploadLeadDataPath).addParams("gameName", app).addParams("eventId", eventId)
				.addParams("playerId", pid)
				.addParams("DataId", dataId)
				.addParams("nonce", nonce)
				.addData(data)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Content-Type"] = "text/plain";
			return new WWW(GetUrl(), Encoding.UTF8.GetBytes(data), dictionary);
		}
	}

	public class EventLeadRequest : ProtoRequestPid<EventLeads>
	{
		public string appName;

		public string eventId;

		public string pid;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getOnlineEventsLeaderboardsPath).addParams("app", appName).addParams("res", "proto")
				.addParams("PlayerId", pid)
				.addParams("Eventid", eventId)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class EventScoreUpdateRequest : ProtoRequestPid<UpdateLeadResponse>
	{
		private string pid;

		private string app;

		private int rank;

		private string name;

		private string countryFlag;

		private int s;

		private double s2;

		private string imageUrl;

		public string nonce;

		public string eventId;

		public int score1 => s;

		public double score2 => s2;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.updateOnlineEventsPath).addParams("app", app).addParams("res", "proto")
				.addParams("PlayerId", pid)
				.addParams("Eventid", eventId)
				.addParams("Score1", score1)
				.addParams("Score2", score2)
				.addParams("player_name", name)
				.addParams("player_rank", rank)
				.addParams("player_flag", countryFlag)
				.addParams("imageUrl", imageUrl)
				.addParams("nonce", nonce)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class NonceRequest : ServerRequest
	{
		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.nonceUrlPath).ToString();
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class LeaderboardsRequest : ProtoRequestPid<Lead>
	{
		private string app;

		private string pid;

		private int comp;

		private int e;

		private int sR;

		private string country;

		private string distAroundPlayer;

		private int topEntries;

		private string leadTotalEntries;

		private int version;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			UrlBuilder urlBuilder = new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.leadUrlPath).addParams("app", app).addParams("pid", pid)
				.addParams("sR", sR.ToString())
				.addParams("country", country)
				.addParams("res", "protobuf")
				.addParams("distAroundPlayer", distAroundPlayer)
				.addParams("topEntries", topEntries.ToString())
				.addParams("leadTotalEntries", leadTotalEntries)
				.addParams("lv", version.ToString());
			if (comp >= 0)
			{
				urlBuilder.addParams("comp", comp.ToString());
			}
			if (e >= 0)
			{
				urlBuilder.addParams("e", e.ToString());
			}
			return urlBuilder.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class UpdateAppMessageRead : ProtoRequestPid<StatusMessage>
	{
		private string messageId;

		private string pid;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.updateAppMessagesPath).addParams("message_id", messageId).addParams("pid", pid)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class AppMessagesRequest : ProtoRequestPid<MessageList>
	{
		private string app;

		private string pid;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getAppMessagesPath).addParams("app", app).addParams("playerID", pid)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class SegmentedLeaderboardsRequest : ProtoRequestPid<CombinationLeads>
	{
		private string app;

		private string pid;

		private int comp;

		private int e;

		private int sR;

		private string country;

		private string distAroundPlayer;

		private int topEntries;

		private string leadTotalEntries;

		private int version;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			UrlBuilder urlBuilder = new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getSegmentedLeaderboards).addParams("app", app).addParams("pid", pid)
				.addParams("sR", sR.ToString())
				.addParams("country", country)
				.addParams("res", "protobuf")
				.addParams("distAroundPlayer", distAroundPlayer)
				.addParams("topEntries", topEntries.ToString())
				.addParams("maxEntriesPerLead", GGServerConstants.instance.maxEntriesPerLead.ToString())
				.addParams("leadTotalEntries", leadTotalEntries)
				.addParams("lv", version.ToString());
			if (comp >= 0)
			{
				urlBuilder.addParams("comp", comp.ToString());
			}
			if (e >= 0)
			{
				urlBuilder.addParams("e", e.ToString());
			}
			return urlBuilder.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class ActiveCompetitionRequest : ProtoRequest<CompetitionMessage>
	{
		private DateTime endTime;

		public TimeSpan timeSpanTillEndOfCompetition => endTime - DateTime.Now;

		protected override bool TryGetFromMemoryCache(string url)
		{
			ActiveCompetitionRequest activeCompetitionRequest = Singleton<GGRequestCache>.Instance.Get<ActiveCompetitionRequest>(GetCacheKey(url));
			if (activeCompetitionRequest != null)
			{
				endTime = activeCompetitionRequest.endTime;
				base.response = activeCompetitionRequest.response;
			}
			return activeCompetitionRequest != null;
		}

		protected override void CacheResults(WWW query)
		{
			Singleton<GGRequestCache>.Instance.Put(GetCacheKey(query.url), this, timeSpanTillEndOfCompetition);
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getActiveCompetitionUrlPath).addParams("app", GGServerConstants.instance.appName).SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override void ParseResponse(byte[] bytes)
		{
			base.ParseResponse(bytes);
			endTime = GetDateTimeCompEnd(base.response.compEndTimestamp);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class UpdateRequest : ProtoRequestPid<StatusMessage>
	{
		private string pid;

		private string app;

		private int sR;

		private string n;

		private string c;

		private int s;

		private string imageUrl;

		private int version;

		public string nonce;

		public int rank2;

		public string room;

		public UpdateRequest(int sR, string n, string c)
		{
			app = GGServerConstants.instance.appName;
			pid = "";
			this.sR = sR;
			this.n = n;
			this.c = c;
			s = 0;
			imageUrl = "";
			version = GGServerConstants.instance.leadVersion;
		}

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		public void SetScore(int s)
		{
			this.s = s;
		}

		protected override string GetUrl()
		{
			UrlBuilder urlBuilder = new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.updateUrlPath).addParams("app", app).addParams("pid", pid)
				.addParams("sR", sR.ToString())
				.addParams("c", c)
				.addParams("nonce", nonce)
				.addParams("rank2", rank2)
				.addParams("lv", version.ToString());
			if (n != "")
			{
				urlBuilder.addParams("n", n);
			}
			if (s >= 0)
			{
				urlBuilder.addParams("s", s.ToString());
			}
			if (imageUrl != "")
			{
				urlBuilder.addParams("image", imageUrl);
			}
			if (!string.IsNullOrEmpty(room))
			{
				urlBuilder.addParams("room", room);
			}
			return urlBuilder.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class IdRequest : ProtoRequest<Pid>
	{
		private string fbId;

		private string gId;

		private string installId;

		private string app;

		private string fbIdForApp;

		public IdRequest()
		{
			installId = GGUID.InstallId();
			fbId = "";
			gId = "";
			fbIdForApp = "";
			GGPlayerSettings instance = GGPlayerSettings.instance;
			string facebookPlayerId = instance.Model.facebookPlayerId;
			string applePlayerId = instance.Model.applePlayerId;
			bool flag = GGUtil.HasText(applePlayerId);
			if (GGUtil.HasText(facebookPlayerId))
			{
				fbIdForApp = facebookPlayerId + ConfigBase.instance.facebookAppPlayerSuffix;
			}
			else if (flag)
			{
				fbIdForApp = applePlayerId + "-apl-" + ConfigBase.instance.facebookAppPlayerSuffix;
			}
			app = GGServerConstants.instance.appName;
		}

		protected override string GetUrl()
		{
			UrlBuilder urlBuilder = new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getIdUrlPath).addParams("installId", installId).addParams("app", app);
			if (fbId != "")
			{
				urlBuilder.addParams("fbId", fbId);
			}
			if (fbIdForApp != "")
			{
				urlBuilder.addParams("fbIdForApp", fbIdForApp);
			}
			if (gId != "")
			{
				urlBuilder.addParams("gId", gId);
			}
			return urlBuilder.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class GetPrizesRequest : ProtoRequestPid<Lead>
	{
		private string app;

		private string pid;

		private int sR;

		private string country;

		private string distAroundPlayer;

		private int topEntries;

		private string leadTotalEntries;

		private string leadType;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getPrizesUrlPath).addParams("app", app).addParams("pid", pid)
				.addParams("sR", sR.ToString())
				.addParams("country", country)
				.addParams("distAroundPlayer", distAroundPlayer)
				.addParams("topEntries", topEntries.ToString())
				.addParams("leadTotalEntries", leadTotalEntries)
				.addParams("leadType", leadType)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class GetPrizesRequestCombinationLead : ProtoRequestPid<CombinationLeads>
	{
		private string app;

		private string pid;

		private int sR;

		private string country;

		private string distAroundPlayer;

		private int topEntries;

		private string leadTotalEntries;

		private string leadType;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getPrizesUrlPath).addParams("app", app).addParams("pid", pid)
				.addParams("sR", sR.ToString())
				.addParams("country", country)
				.addParams("distAroundPlayer", distAroundPlayer)
				.addParams("topEntries", topEntries.ToString())
				.addParams("leadTotalEntries", leadTotalEntries)
				.addParams("maxEntriesPerLead", GGServerConstants.instance.maxEntriesPerLead.ToString())
				.addParams("leadType", leadType)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class AckPrizesRequest : ProtoRequestPid<StatusMessage>
	{
		private string app;

		private string pid;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.ackPrizesUrlPath).addParams("app", app).addParams("pid", pid)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class CloudSyncRequest : ProtoRequestPid<CloudSyncData>
	{
		public string nonce;

		private int _003CsnapshotId_003Ek__BackingField;

		private string _003CsnapshotGUID_003Ek__BackingField;

		public int snapshotId
		{
			get
			{
				return _003CsnapshotId_003Ek__BackingField;
			}
			protected set
			{
				_003CsnapshotId_003Ek__BackingField = value;
			}
		}

		public string snapshotGUID
		{
			get
			{
				return _003CsnapshotGUID_003Ek__BackingField;
			}
			protected set
			{
				_003CsnapshotGUID_003Ek__BackingField = value;
			}
		}

		public override void SetPid(string pid)
		{
		}

		public void SetVersionInfo(int snapshotId, string snapshotGUID)
		{
			this.snapshotId = snapshotId;
			this.snapshotGUID = snapshotGUID;
		}

		public virtual CloudSyncData GetRequestData()
		{
			return null;
		}
	}

	public class GetCloudSyncDataRequest : CloudSyncRequest
	{
		private string app;

		private string pid;

		public GetCloudSyncDataRequest()
		{
			app = GGServerConstants.instance.appName;
			pid = "";
			base.snapshotId = -1;
			base.snapshotGUID = "";
		}

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		public override CloudSyncData GetRequestData()
		{
			return null;
		}

		protected override string GetUrl()
		{
			UrlBuilder urlBuilder = new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getCloudSyncUrlPath).addParams("app", app).addParams("pid", pid)
				.addParams("nonce", nonce);
			if (ConfigBase.instance.cloudSyncType == ConfigBase.GGFileIOCloudSyncTypes.GGSnapshotCloudSync)
			{
				urlBuilder = urlBuilder.addParams("addSnapshotInfo", "true");
			}
			return urlBuilder.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class UpdateCloudSyncDataRequest : CloudSyncRequest
	{
		private string app;

		private string pid;

		private string data;

		public UpdateCloudSyncDataRequest()
		{
			app = GGServerConstants.instance.appName;
			pid = "";
			base.snapshotId = -1;
			base.snapshotGUID = "";
		}

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		public void AddData(CloudSyncData syncData)
		{
			data = ProtoIO.SerializeToByte64(syncData);
		}

		public override CloudSyncData GetRequestData()
		{
			CloudSyncData model = null;
			if (!ProtoIO.LoadFromBase64String(data, out model))
			{
				model = new CloudSyncData();
			}
			return model;
		}

		protected override string GetUrl()
		{
			UrlBuilder urlBuilder = new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.updateCloudSyncUrlPath).addParams("app", app).addParams("pid", pid)
				.addParams("nonce", nonce)
				.addData(data);
			if (base.snapshotId >= 0)
			{
				urlBuilder = urlBuilder.addParams("snapshotId", base.snapshotId.ToString()).addParams("snapshotGUID", base.snapshotGUID);
			}
			return urlBuilder.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Content-Type"] = "text/plain";
			return new WWW(GetUrl(), Encoding.UTF8.GetBytes(data), dictionary);
		}
	}

	public class FacebookLoginRequest : ProtoRequest<FBLogin>
	{
	}

	public class FacebookInviteFriends : ProtoRequestPid<InvitableFriends>
	{
		private string app;

		private string pid;

		private int pagesToFetch;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.FBInvitableFriendsPath).addParams("app", app).addParams("pid", pid)
				.addParams("pages", pagesToFetch.ToString())
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class FacebookPlayingFriends : ProtoRequestPid<InvitableFriends>
	{
		private string app;

		private string pid;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.FBPlayingFriendsPath).addParams("app", app).addParams("pid", pid)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class GGServerPlayerMessages : ProtoRequestPid<ServerMessages>
	{
		private string app;

		private string pid;

		public GGServerPlayerMessages()
		{
			app = GGServerConstants.instance.appName;
			pid = "";
		}

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.PlayerGetMessagesPath).addParams("app", app).addParams("pid", pid)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class GGServerMarkMessagesAsRead : ProtoRequestPid<StatusMessage>
	{
		private string app;

		private string pid;

		private string requestIds;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.MarkMessageReadPath).addParams("app", app).addParams("pid", pid)
				.addParams("requestIds", requestIds)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class GGServerGetFriendProfiles : ProtoRequestPid<FriendsProfiles>
	{
		private string app;

		private string pid;

		private List<string> files;

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getFriendProfilesPath).addParams("app", app).addParams("pid", pid)
				.addParams("filename", files)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			return new WWW(GetUrl());
		}
	}

	public class GetPlayerPositionsRequest : ProtoRequestPid<PlayerPositions>
	{
		private string app;

		private string pid;

		private string data;

		public GetPlayerPositionsRequest()
		{
			app = GGServerConstants.instance.appName;
			pid = "";
		}

		public override void SetPid(string pid)
		{
			this.pid = pid;
		}

		public void AddData(PlayerPositions players)
		{
			data = ProtoIO.SerializeToByte64(players);
		}

		protected override string GetUrl()
		{
			return new UrlBuilder(GGServerConstants.instance.urlBase).addPath(GGServerConstants.instance.getPlayerPositionList).addParams("app", app).addParams("pid", pid)
				.addParams("res", "protobuf")
				.addData(data)
				.SignAndToString(GGServerConstants.instance.publicKey, GGServerConstants.instance.privateKey);
		}

		protected override WWW CreateQuery()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Content-Type"] = "text/plain";
			return new WWW(GetUrl(), Encoding.UTF8.GetBytes(data), dictionary);
		}
	}

	private sealed class _003CDoCallWhenRequestComplete_003Ed__61 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ServerRequest request;

		public Action<ServerRequest> onComplete;

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
		public _003CDoCallWhenRequestComplete_003Ed__61(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (request == null)
				{
					if (onComplete != null)
					{
						onComplete(request);
					}
					return false;
				}
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (request.status != ServerRequest.RequestStatus.NotSent)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if (onComplete != null)
			{
				onComplete(request);
			}
			return false;
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

	private sealed class _003C_003Ec__DisplayClass64_0
	{
		public ServerRequest playerId;

		internal void _003CDoUpdateEventsLeaderboards_003Eb__0(ServerRequest pid)
		{
			playerId = pid;
		}
	}

	private sealed class _003CDoUpdateEventsLeaderboards_003Ed__64 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGServerRequestsBackend _003C_003E4__this;

		private _003C_003Ec__DisplayClass64_0 _003C_003E8__1;

		public EventScoreUpdateRequest updateRequest;

		public OnComplete onComplete;

		private NonceRequest _003CnonceReq_003E5__2;

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
		public _003CDoUpdateEventsLeaderboards_003Ed__64(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass64_0();
				_003C_003E8__1.playerId = null;
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(gGServerRequestsBackend.GetPlayerId(_003C_003E8__1._003CDoUpdateEventsLeaderboards_003Eb__0));
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003CnonceReq_003E5__2 = new NonceRequest();
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003CnonceReq_003E5__2.RequestCoroutine());
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				if (_003C_003E8__1.playerId.status == ServerRequest.RequestStatus.Success && _003CnonceReq_003E5__2.status == ServerRequest.RequestStatus.Success)
				{
					updateRequest.SetPid(_003C_003E8__1.playerId.GetResponse<Pid>().pid);
					updateRequest.nonce = _003CnonceReq_003E5__2.GetResponse<string>();
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(updateRequest.RequestCoroutine());
					_003C_003E1__state = 3;
					return true;
				}
				break;
			case 3:
				_003C_003E1__state = -1;
				break;
			}
			if (onComplete != null)
			{
				onComplete(updateRequest);
			}
			return false;
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

	private sealed class _003C_003Ec__DisplayClass69_0
	{
		public ServerRequest playerId;

		internal void _003CDoUpdateUser_003Eb__0(ServerRequest pid)
		{
			playerId = pid;
		}
	}

	private sealed class _003CDoUpdateUser_003Ed__69 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGServerRequestsBackend _003C_003E4__this;

		private _003C_003Ec__DisplayClass69_0 _003C_003E8__1;

		public UpdateRequest update;

		public OnComplete onComplete;

		private NonceRequest _003CnonceReq_003E5__2;

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
		public _003CDoUpdateUser_003Ed__69(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass69_0();
				_003C_003E8__1.playerId = null;
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(gGServerRequestsBackend.GetPlayerId(_003C_003E8__1._003CDoUpdateUser_003Eb__0));
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003CnonceReq_003E5__2 = new NonceRequest();
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003CnonceReq_003E5__2.RequestCoroutine());
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				if (_003C_003E8__1.playerId.status == ServerRequest.RequestStatus.Success && _003CnonceReq_003E5__2.status == ServerRequest.RequestStatus.Success)
				{
					update.SetPid(_003C_003E8__1.playerId.GetResponse<Pid>().pid);
					update.nonce = _003CnonceReq_003E5__2.GetResponse<string>();
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(update.RequestCoroutine());
					_003C_003E1__state = 3;
					return true;
				}
				break;
			case 3:
				_003C_003E1__state = -1;
				break;
			}
			UnityEngine.Debug.Log("Update success " + update.status);
			if (onComplete != null)
			{
				onComplete(update);
			}
			return false;
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

	private sealed class _003CGetPlayerId_003Ed__71 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGServerRequestsBackend _003C_003E4__this;

		public OnComplete onComplete;

		private IdRequest _003CidRequest_003E5__2;

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
		public _003CGetPlayerId_003Ed__71(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CidRequest_003E5__2 = new IdRequest();
				if (ConfigBase.instance.isFakePlayerIdOn)
				{
					UnityEngine.Debug.Log("USING FAKE ID:" + ConfigBase.instance.playerId);
					gGServerRequestsBackend.memoryCachedPlayerId = ConfigBase.instance.playerId;
				}
				if (!string.IsNullOrEmpty(gGServerRequestsBackend.memoryCachedPlayerId))
				{
					_003CidRequest_003E5__2.response = new Pid();
					_003CidRequest_003E5__2.response.pid = gGServerRequestsBackend.memoryCachedPlayerId;
					_003CidRequest_003E5__2.status = ServerRequest.RequestStatus.Success;
					onComplete(_003CidRequest_003E5__2);
					return false;
				}
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003CidRequest_003E5__2.RequestCoroutine());
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_003CidRequest_003E5__2.status == ServerRequest.RequestStatus.Success)
				{
					if (string.IsNullOrEmpty(gGServerRequestsBackend.memoryCachedPlayerId))
					{
						gGServerRequestsBackend.cachedPlayerId = _003CidRequest_003E5__2.GetResponse<Pid>().pid;
					}
				}
				else if (!string.IsNullOrEmpty(gGServerRequestsBackend.storage.lastKnownPid))
				{
					_003CidRequest_003E5__2.response = new Pid();
					_003CidRequest_003E5__2.response.pid = gGServerRequestsBackend.cachedPlayerId;
					_003CidRequest_003E5__2.status = ServerRequest.RequestStatus.Success;
					onComplete(_003CidRequest_003E5__2);
					return false;
				}
				if (onComplete != null)
				{
					onComplete(_003CidRequest_003E5__2);
				}
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

	private sealed class _003CGetFacebookLogin_003Ed__72 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGServerRequestsBackend _003C_003E4__this;

		public OnComplete onComplete;

		private FacebookLoginRequest _003CloginRequest_003E5__2;

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
		public _003CGetFacebookLogin_003Ed__72(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				UnityEngine.Debug.Log("GetFacebookLogin");
				_003CloginRequest_003E5__2 = new FacebookLoginRequest();
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003CloginRequest_003E5__2.RequestCoroutine());
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_003CloginRequest_003E5__2.status == ServerRequest.RequestStatus.Success)
				{
					UnityEngine.Debug.Log("Caching player id");
					gGServerRequestsBackend.cachedPlayerId = _003CloginRequest_003E5__2.GetResponse<FBLogin>().pid;
				}
				if (onComplete != null)
				{
					onComplete(_003CloginRequest_003E5__2);
				}
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

	private sealed class _003CDoRequest_003Ed__74 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGServerRequestsBackend _003C_003E4__this;

		public ActiveCompetitionRequest req;

		public OnComplete onComplete;

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
		public _003CDoRequest_003Ed__74(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(req.RequestCoroutine());
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (onComplete != null)
				{
					onComplete(req);
				}
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

	private sealed class _003C_003Ec__DisplayClass80_0
	{
		public ServerRequest playerId;

		internal void _003CDoSyncCsData_003Eb__0(ServerRequest pid)
		{
			playerId = pid;
		}
	}

	private sealed class _003CDoSyncCsData_003Ed__80 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGServerRequestsBackend _003C_003E4__this;

		private _003C_003Ec__DisplayClass80_0 _003C_003E8__1;

		public CloudSyncRequest dataRequest;

		public OnComplete onComplete;

		private NonceRequest _003CnonceReq_003E5__2;

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
		public _003CDoSyncCsData_003Ed__80(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass80_0();
				_003C_003E8__1.playerId = null;
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(gGServerRequestsBackend.GetPlayerId(_003C_003E8__1._003CDoSyncCsData_003Eb__0));
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003CnonceReq_003E5__2 = new NonceRequest();
				UnityEngine.Debug.Log("nonce cache policy: " + _003CnonceReq_003E5__2.cache);
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003CnonceReq_003E5__2.RequestCoroutine());
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				if (_003C_003E8__1.playerId.status == ServerRequest.RequestStatus.Success && _003CnonceReq_003E5__2.status == ServerRequest.RequestStatus.Success)
				{
					dataRequest.SetPid(_003C_003E8__1.playerId.GetResponse<Pid>().pid);
					dataRequest.nonce = _003CnonceReq_003E5__2.GetResponse<string>();
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(dataRequest.RequestCoroutine());
					_003C_003E1__state = 3;
					return true;
				}
				break;
			case 3:
				_003C_003E1__state = -1;
				break;
			}
			if (onComplete != null)
			{
				onComplete(dataRequest);
			}
			return false;
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

	private sealed class _003C_003Ec__DisplayClass82_0
	{
		public ServerRequest playerId;

		internal void _003CDoUploadLeadData_003Eb__0(ServerRequest pid)
		{
			playerId = pid;
		}
	}

	private sealed class _003CDoUploadLeadData_003Ed__82 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGServerRequestsBackend _003C_003E4__this;

		private _003C_003Ec__DisplayClass82_0 _003C_003E8__1;

		public UploadLeadDataRequest dataRequest;

		public OnComplete onComplete;

		private NonceRequest _003CnonceReq_003E5__2;

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
		public _003CDoUploadLeadData_003Ed__82(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass82_0();
				_003C_003E8__1.playerId = null;
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(gGServerRequestsBackend.GetPlayerId(_003C_003E8__1._003CDoUploadLeadData_003Eb__0));
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				_003CnonceReq_003E5__2 = new NonceRequest();
				UnityEngine.Debug.Log("nonce cache policy: " + _003CnonceReq_003E5__2.cache);
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003CnonceReq_003E5__2.RequestCoroutine());
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				if (_003C_003E8__1.playerId.status == ServerRequest.RequestStatus.Success && _003CnonceReq_003E5__2.status == ServerRequest.RequestStatus.Success)
				{
					dataRequest.SetPid(_003C_003E8__1.playerId.GetResponse<Pid>().pid);
					dataRequest.nonce = _003CnonceReq_003E5__2.GetResponse<string>();
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(dataRequest.RequestCoroutine());
					_003C_003E1__state = 3;
					return true;
				}
				break;
			case 3:
				_003C_003E1__state = -1;
				break;
			}
			if (onComplete != null)
			{
				onComplete(dataRequest);
			}
			return false;
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

	private sealed class _003C_003Ec__DisplayClass93_0
	{
		public PidSetRequest setPid;

		internal void _003CDoExecuteAllInterfacesRequest_003Eb__0(ServerRequest pid)
		{
			if (pid.status == ServerRequest.RequestStatus.Success)
			{
				setPid.SetPid(pid.GetResponse<Pid>().pid);
			}
		}
	}

	private sealed class _003CDoExecuteAllInterfacesRequest_003Ed__93 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ServerRequest request;

		public GGServerRequestsBackend _003C_003E4__this;

		public OnComplete onComplete;

		private NonceSetRequest _003CsetNonce_003E5__2;

		private NonceRequest _003CnonceReq_003E5__3;

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
		public _003CDoExecuteAllInterfacesRequest_003Ed__93(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003C_003Ec__DisplayClass93_0 _003C_003Ec__DisplayClass93_ = new _003C_003Ec__DisplayClass93_0();
				_003C_003Ec__DisplayClass93_.setPid = (request as PidSetRequest);
				if (_003C_003Ec__DisplayClass93_.setPid != null)
				{
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(gGServerRequestsBackend.GetPlayerId(_003C_003Ec__DisplayClass93_._003CDoExecuteAllInterfacesRequest_003Eb__0));
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_007a;
			}
			case 1:
				_003C_003E1__state = -1;
				goto IL_007a;
			case 2:
				_003C_003E1__state = -1;
				if (_003CnonceReq_003E5__3.status == ServerRequest.RequestStatus.Success)
				{
					_003CsetNonce_003E5__2.SetNonce(_003CnonceReq_003E5__3.GetResponse<string>());
				}
				_003CnonceReq_003E5__3 = null;
				goto IL_00ef;
			case 3:
				{
					_003C_003E1__state = -1;
					if (onComplete != null)
					{
						onComplete(request);
					}
					return false;
				}
				IL_00ef:
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(request.RequestCoroutine());
				_003C_003E1__state = 3;
				return true;
				IL_007a:
				_003CsetNonce_003E5__2 = (request as NonceSetRequest);
				if (_003CsetNonce_003E5__2 != null)
				{
					_003CnonceReq_003E5__3 = new NonceRequest();
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003CnonceReq_003E5__3.RequestCoroutine());
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_00ef;
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

	private sealed class _003CDoExecuteNonceRequest_003Ed__94 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ServerRequest request;

		public GGServerRequestsBackend _003C_003E4__this;

		public OnComplete onComplete;

		private NonceSetRequest _003CsetNonce_003E5__2;

		private NonceRequest _003CnonceReq_003E5__3;

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
		public _003CDoExecuteNonceRequest_003Ed__94(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CsetNonce_003E5__2 = (request as NonceSetRequest);
				if (_003CsetNonce_003E5__2 != null)
				{
					_003CnonceReq_003E5__3 = new NonceRequest();
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003CnonceReq_003E5__3.RequestCoroutine());
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(request.RequestCoroutine());
				_003C_003E1__state = 3;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_003CnonceReq_003E5__3.status == ServerRequest.RequestStatus.Success)
				{
					_003CsetNonce_003E5__2.SetNonce(_003CnonceReq_003E5__3.GetResponse<string>());
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(request.RequestCoroutine());
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_00c5;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00c5;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_00c5:
				_003CnonceReq_003E5__3 = null;
				break;
			}
			if (onComplete != null)
			{
				onComplete(request);
			}
			return false;
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

	private sealed class _003CDoExecuteRequest_003Ed__95 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGServerRequestsBackend _003C_003E4__this;

		public ServerRequest request;

		public OnComplete onComplete;

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
		public _003CDoExecuteRequest_003Ed__95(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(request.RequestCoroutine());
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (onComplete != null)
				{
					onComplete(request);
				}
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

	private sealed class _003C_003Ec__DisplayClass96_0<T> where T : class
	{
		public ServerRequest playerId;

		public ProtoRequestPid<T> request;

		internal void _003CDoExecuteRequestWithPid_003Eb__0(ServerRequest pid)
		{
			playerId = pid;
			if (pid.status == ServerRequest.RequestStatus.Success)
			{
				request.SetPid(pid.GetResponse<Pid>().pid);
			}
		}
	}

	private sealed class _003CDoExecuteRequestWithPid_003Ed__96<T> : IEnumerator<object>, IEnumerator, IDisposable where T : class
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ProtoRequestPid<T> request;

		public GGServerRequestsBackend _003C_003E4__this;

		private _003C_003Ec__DisplayClass96_0<T> _003C_003E8__1;

		public OnComplete onComplete;

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
		public _003CDoExecuteRequestWithPid_003Ed__96(int _003C_003E1__state)
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
			GGServerRequestsBackend gGServerRequestsBackend = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass96_0<T>();
				_003C_003E8__1.request = request;
				_003C_003E8__1.playerId = null;
				_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(gGServerRequestsBackend.GetPlayerId(_003C_003E8__1._003CDoExecuteRequestWithPid_003Eb__0));
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_003C_003E8__1.playerId.status == ServerRequest.RequestStatus.Success)
				{
					_003C_003E2__current = gGServerRequestsBackend.StartCoroutine(_003C_003E8__1.request.RequestCoroutine());
					_003C_003E1__state = 2;
					return true;
				}
				break;
			case 2:
				_003C_003E1__state = -1;
				break;
			}
			if (onComplete != null)
			{
				onComplete(_003C_003E8__1.request);
			}
			return false;
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

	protected RequestsBackendStorage storage;

	protected string storageFilename = "backendStorage.bytes";

	private string memoryCachedPlayerId = "";

	public string cachedPlayerId
	{
		get
		{
			if (!string.IsNullOrEmpty(memoryCachedPlayerId))
			{
				return memoryCachedPlayerId;
			}
			if (!string.IsNullOrEmpty(storage.lastKnownPid))
			{
				return storage.lastKnownPid;
			}
			return "";
		}
		protected set
		{
			memoryCachedPlayerId = value;
			storage.lastKnownPid = value;
			Save();
		}
	}

	public void ResetCache()
	{
		cachedPlayerId = "";
		Singleton<GGRequestCache>.instance.Clear();
		SingletonInit<GGRequestFileCache>.instance.Clear();
	}

	public void Save()
	{
		ProtoIO.SaveToFile(storageFilename, storage);
	}

	public override void Init()
	{
		base.Init();
		if (!ProtoIO.LoadFromFile(storageFilename, out storage))
		{
			storage = new RequestsBackendStorage();
			Save();
		}
	}

	public void GetCompetitionLeaderboards(LeaderboardsRequest lead, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("GetLeaderboards");
		StartCoroutine(DoExecuteRequestWithPid(lead, onComplete));
	}

	public Coroutine CallWhenRequestComplete(ServerRequest request, Action<ServerRequest> onComplete)
	{
		return StartCoroutine(DoCallWhenRequestComplete(request, onComplete));
	}

	private IEnumerator DoCallWhenRequestComplete(ServerRequest request, Action<ServerRequest> onComplete)
	{
		return new _003CDoCallWhenRequestComplete_003Ed__61(0)
		{
			request = request,
			onComplete = onComplete
		};
	}

	public void GetEventsLeaderboards(EventLeadRequest lead, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("Get Events Leaderboards");
		StartCoroutine(DoExecuteRequestWithPid(lead, onComplete));
	}

	public void UpdateEventsLeaderboards(EventScoreUpdateRequest updateRequest, OnComplete onComplete)
	{
		StartCoroutine(DoUpdateEventsLeaderboards(updateRequest, onComplete));
	}

	private IEnumerator DoUpdateEventsLeaderboards(EventScoreUpdateRequest updateRequest, OnComplete onComplete)
	{
		return new _003CDoUpdateEventsLeaderboards_003Ed__64(0)
		{
			_003C_003E4__this = this,
			updateRequest = updateRequest,
			onComplete = onComplete
		};
	}

	public void GetAppMessagesRequest(AppMessagesRequest messageRequest, OnComplete onComplete)
	{
		StartCoroutine(DoExecuteRequestWithPid(messageRequest, onComplete));
	}

	public void UpdateAppMessagesRequest(UpdateAppMessageRead messageRequest, OnComplete onComplete)
	{
		StartCoroutine(DoExecuteRequestWithPid(messageRequest, onComplete));
	}

	public void GetSegmentedCompetitionLeaderboards(SegmentedLeaderboardsRequest lead, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("GetLeaderboards");
		StartCoroutine(DoExecuteRequestWithPid(lead, onComplete));
	}

	public void UpdateUser(UpdateRequest update, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("UpdateUser");
		StartCoroutine(DoUpdateUser(update, onComplete));
	}

	private IEnumerator DoUpdateUser(UpdateRequest update, OnComplete onComplete)
	{
		return new _003CDoUpdateUser_003Ed__69(0)
		{
			_003C_003E4__this = this,
			update = update,
			onComplete = onComplete
		};
	}

	public void ExecuteGetPlayerId(OnComplete onComplete)
	{
		StartCoroutine(GetPlayerId(onComplete));
	}

	public IEnumerator GetPlayerId(OnComplete onComplete)
	{
		return new _003CGetPlayerId_003Ed__71(0)
		{
			_003C_003E4__this = this,
			onComplete = onComplete
		};
	}

	public IEnumerator GetFacebookLogin(OnComplete onComplete)
	{
		return new _003CGetFacebookLogin_003Ed__72(0)
		{
			_003C_003E4__this = this,
			onComplete = onComplete
		};
	}

	public void GetActiveCompetition(ActiveCompetitionRequest req, OnComplete onComplete)
	{
		StartCoroutine(DoRequest(req, onComplete));
	}

	private IEnumerator DoRequest(ActiveCompetitionRequest req, OnComplete onComplete)
	{
		return new _003CDoRequest_003Ed__74(0)
		{
			_003C_003E4__this = this,
			req = req,
			onComplete = onComplete
		};
	}

	public void GetPrizes(GetPrizesRequest getPrizesRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("GetPrizes");
		StartCoroutine(DoExecuteRequestWithPid(getPrizesRequest, onComplete));
	}

	public void GetCombinationPrizes(GetPrizesRequestCombinationLead getPrizesRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("GetPrizes");
		StartCoroutine(DoExecuteRequestWithPid(getPrizesRequest, onComplete));
	}

	public void AckPrizes(AckPrizesRequest ackPrizesRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("AckPrizes");
		StartCoroutine(DoExecuteRequestWithPid(ackPrizesRequest, onComplete));
	}

	public Coroutine GetCSData(CloudSyncRequest dataRequest, OnComplete onComplete)
	{
		return StartCoroutine(DoSyncCsData(dataRequest, onComplete));
	}

	public void UpdateCSData(UpdateCloudSyncDataRequest dataRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("UpdateCSData");
		StartCoroutine(DoSyncCsData(dataRequest, onComplete));
	}

	private IEnumerator DoSyncCsData(CloudSyncRequest dataRequest, OnComplete onComplete)
	{
		return new _003CDoSyncCsData_003Ed__80(0)
		{
			_003C_003E4__this = this,
			dataRequest = dataRequest,
			onComplete = onComplete
		};
	}

	public void UploadLeadData(UploadLeadDataRequest dataRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("UpdateLeadData");
		StartCoroutine(DoUploadLeadData(dataRequest, onComplete));
	}

	private IEnumerator DoUploadLeadData(UploadLeadDataRequest dataRequest, OnComplete onComplete)
	{
		return new _003CDoUploadLeadData_003Ed__82(0)
		{
			_003C_003E4__this = this,
			dataRequest = dataRequest,
			onComplete = onComplete
		};
	}

	public void GetFacebookInvites(FacebookInviteFriends inviteRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("inviteRequest");
		inviteRequest.cache = CacheStategy.GetFromCache;
		inviteRequest.cacheTimeToLive = TimeSpan.FromSeconds(30.0);
		StartCoroutine(DoExecuteRequestWithPid(inviteRequest, onComplete));
	}

	public void GetFacebookPlayers(FacebookPlayingFriends playersRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("playersRequest");
		playersRequest.cache = CacheStategy.GetFromCache;
		playersRequest.cacheTimeToLive = TimeSpan.FromSeconds(30.0);
		StartCoroutine(DoExecuteRequestWithPid(playersRequest, onComplete));
	}

	public void GetMessagesForPlayer(GGServerPlayerMessages messagesRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("messagesRequest");
		messagesRequest.cache = CacheStategy.GetFromCache;
		messagesRequest.cacheTimeToLive = TimeSpan.FromMinutes(10.0);
		StartCoroutine(DoExecuteRequestWithPid(messagesRequest, onComplete));
	}

	public void MarkMessagesAsRead(GGServerMarkMessagesAsRead markAsReadRequest, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("markAsReadRequest");
		StartCoroutine(DoExecuteRequestWithPid(markAsReadRequest, onComplete));
	}

	public void GetFriendProfiles(GGServerGetFriendProfiles request, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("GetFriendProfiles");
		StartCoroutine(DoExecuteRequestWithPid(request, onComplete));
	}

	public void GetPlayerPositionList(GetPlayerPositionsRequest request, OnComplete onComplete)
	{
		UnityEngine.Debug.Log("GetFriendProfiles");
		StartCoroutine(DoExecuteRequestWithPid(request, onComplete));
	}

	public void ExecuteRequest(ServerRequest request, OnComplete onComplete)
	{
		StartCoroutine(DoExecuteRequest(request, onComplete));
	}

	public void ExecuteRequestWithPid<T>(ProtoRequestPid<T> request, OnComplete onComplete) where T : class
	{
		StartCoroutine(DoExecuteRequestWithPid(request, onComplete));
	}

	public void ExecuteRequestWithNonce(ServerRequest request, OnComplete onComplete)
	{
		StartCoroutine(DoExecuteNonceRequest(request, onComplete));
	}

	public void ExecuteRequestAllInterfacesRequest(ServerRequest request, OnComplete onComplete)
	{
		StartCoroutine(DoExecuteAllInterfacesRequest(request, onComplete));
	}

	private IEnumerator DoExecuteAllInterfacesRequest(ServerRequest request, OnComplete onComplete)
	{
		return new _003CDoExecuteAllInterfacesRequest_003Ed__93(0)
		{
			_003C_003E4__this = this,
			request = request,
			onComplete = onComplete
		};
	}

	private IEnumerator DoExecuteNonceRequest(ServerRequest request, OnComplete onComplete)
	{
		return new _003CDoExecuteNonceRequest_003Ed__94(0)
		{
			_003C_003E4__this = this,
			request = request,
			onComplete = onComplete
		};
	}

	private IEnumerator DoExecuteRequest(ServerRequest request, OnComplete onComplete)
	{
		return new _003CDoExecuteRequest_003Ed__95(0)
		{
			_003C_003E4__this = this,
			request = request,
			onComplete = onComplete
		};
	}

	private IEnumerator DoExecuteRequestWithPid<T>(ProtoRequestPid<T> request, OnComplete onComplete) where T : class
	{
		return new _003CDoExecuteRequestWithPid_003Ed__96<T>(0)
		{
			_003C_003E4__this = this,
			request = request,
			onComplete = onComplete
		};
	}

	public static DateTime GetDateTimeCompEnd(long seconds)
	{
		return DateTime.Now.AddSeconds(seconds);
	}
}
