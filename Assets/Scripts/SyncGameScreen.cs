using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class SyncGameScreen : MonoBehaviour
{
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		public bool synchronizingToServer;

		public GGSnapshotCloudSync snapshotSync;

		internal void _003CDoSyncNow_003Eb__0(GGServerRequestsBackend.ServerRequest request)
		{
			synchronizingToServer = false;
			snapshotSync.HandleSyncRequestResult(request);
		}
	}

	private sealed class _003CDoSyncNow_003Ed__5 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public SyncGameScreen _003C_003E4__this;

		private _003C_003Ec__DisplayClass5_0 _003C_003E8__1;

		private float _003CstartTime_003E5__2;

		private float _003CmaxDurationSec_003E5__3;

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
		public _003CDoSyncNow_003Ed__5(int _003C_003E1__state)
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
			SyncGameScreen syncGameScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass5_0();
				GGUtil.SetFill(syncGameScreen.fillImage, 0f);
				GGPlayerSettings instance = GGPlayerSettings.instance;
				GGPlayerSettings.instance.canCloudSync = true;
				_003C_003E8__1.snapshotSync = (GGFileIOCloudSync.instance as GGSnapshotCloudSync);
				_003C_003E8__1.synchronizingToServer = true;
				_003C_003E8__1.snapshotSync.SynchronizeNow(_003C_003E8__1._003CDoSyncNow_003Eb__0);
				_003CstartTime_003E5__2 = Time.unscaledTime;
				_003CmaxDurationSec_003E5__3 = 4f;
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003C_003E8__1.synchronizingToServer)
			{
				float num2 = Time.unscaledTime - _003CstartTime_003E5__2;
				float fillAmount = Mathf.InverseLerp(0f, _003CmaxDurationSec_003E5__3, num2);
				GGUtil.SetFill(syncGameScreen.fillImage, fillAmount);
				if (!(num2 >= _003CmaxDurationSec_003E5__3))
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
			}
			GGSnapshotCloudSync.StopSyncNeeded();
			GGUtil.SetFill(syncGameScreen.fillImage, 1f);
			NavigationManager.instance.Pop();
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

	private sealed class _003C_003Ec__DisplayClass6_0
	{
		public bool requestRunning;

		public bool synchronizingToServer;

		public GGSnapshotCloudSync snapshotSync;

		public NavigationManager nav;

		internal void _003CDoLogin_003Eb__0(GGServerRequestsBackend.ServerRequest _003Cp0_003E)
		{
			requestRunning = false;
		}

		internal void _003CDoLogin_003Eb__1(GGServerRequestsBackend.ServerRequest request)
		{
			synchronizingToServer = false;
			snapshotSync.HandleSyncRequestResult(request);
		}

		internal void _003CDoLogin_003Eb__2(bool _003Cp0_003E)
		{
			nav.PopMultiple(2);
		}
	}

	private sealed class _003CDoLogin_003Ed__6 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public SyncGameScreen _003C_003E4__this;

		private _003C_003Ec__DisplayClass6_0 _003C_003E8__1;

		private GGServerRequestsBackend.IdRequest _003CidRequest_003E5__2;

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
		public _003CDoLogin_003Ed__6(int _003C_003E1__state)
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
			SyncGameScreen syncGameScreen = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass6_0();
				GGUtil.SetFill(syncGameScreen.fillImage, 0f);
				BehaviourSingletonInit<GGServerRequestsBackend>.instance.ResetCache();
				_003CidRequest_003E5__2 = new GGServerRequestsBackend.IdRequest();
				_003CidRequest_003E5__2.cache = CacheStategy.DontCache;
				_003C_003E8__1.requestRunning = true;
				BehaviourSingletonInit<GGServerRequestsBackend>.instance.ExecuteRequest(_003CidRequest_003E5__2, _003C_003E8__1._003CDoLogin_003Eb__0);
				goto IL_00c1;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00c1;
			case 2:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_00c1:
				if (_003C_003E8__1.requestRunning)
				{
					GGUtil.SetFill(syncGameScreen.fillImage, _003CidRequest_003E5__2.progress);
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				GGUtil.SetFill(syncGameScreen.fillImage, 1f);
				GGPlayerSettings.instance.canCloudSync = true;
				_003C_003E8__1.snapshotSync = (GGFileIOCloudSync.instance as GGSnapshotCloudSync);
				_003C_003E8__1.synchronizingToServer = true;
				_003C_003E8__1.snapshotSync.SynchronizeNow(_003C_003E8__1._003CDoLogin_003Eb__1);
				break;
			}
			if (_003C_003E8__1.synchronizingToServer)
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 2;
				return true;
			}
			_003C_003E8__1.nav = NavigationManager.instance;
			_003C_003E8__1.nav.GetObject<Dialog>().Show("Login success", "OK", _003C_003E8__1._003CDoLogin_003Eb__2);
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

	[SerializeField]
	private Image fillImage;

	private IEnumerator action;

	public void LoginToFacebook(string userId)
	{
		GGPlayerSettings instance = GGPlayerSettings.instance;
		instance.Model.applePlayerId = "";
		instance.Model.facebookPlayerId = userId;
		instance.Save();
		action = DoLogin();
		NavigationManager.instance.Push(base.gameObject);
	}

	public void LoginToApple(string userId)
	{
		GGPlayerSettings instance = GGPlayerSettings.instance;
		instance.Model.applePlayerId = userId;
		instance.Model.facebookPlayerId = "";
		instance.Save();
		action = DoLogin();
		NavigationManager.instance.Push(base.gameObject);
	}

	public void SynchronizeNow()
	{
		action = DoSyncNow();
		NavigationManager.instance.Push(base.gameObject);
	}

	private IEnumerator DoSyncNow()
	{
		return new _003CDoSyncNow_003Ed__5(0)
		{
			_003C_003E4__this = this
		};
	}

	private IEnumerator DoLogin()
	{
		return new _003CDoLogin_003Ed__6(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		if (action != null && !action.MoveNext())
		{
			action = null;
		}
	}
}
