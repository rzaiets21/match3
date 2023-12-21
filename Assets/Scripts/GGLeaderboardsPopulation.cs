using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GGLeaderboardsPopulation : MonoBehaviour
{
	public delegate void OnComplete();

	private sealed class _003CDoUpdateUser_003Ed__10 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GGLeaderboardsPopulation _003C_003E4__this;

		public GGServerRequestsBackend.UpdateRequest update;

		public GGServerRequestsBackend.OnComplete onComplete;

		private GGServerRequestsBackend.NonceRequest _003CnonceReq_003E5__2;

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
		public _003CDoUpdateUser_003Ed__10(int _003C_003E1__state)
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
			GGLeaderboardsPopulation gGLeaderboardsPopulation = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003CnonceReq_003E5__2 = new GGServerRequestsBackend.NonceRequest();
				_003C_003E2__current = gGLeaderboardsPopulation.StartCoroutine(_003CnonceReq_003E5__2.RequestCoroutine());
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_003CnonceReq_003E5__2.status == GGServerRequestsBackend.ServerRequest.RequestStatus.Success)
				{
					update.nonce = _003CnonceReq_003E5__2.GetResponse<string>();
					UnityEngine.Debug.Log("Nonce: " + update.nonce);
					_003C_003E2__current = gGLeaderboardsPopulation.StartCoroutine(update.RequestCoroutine());
					_003C_003E1__state = 2;
					return true;
				}
				break;
			case 2:
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

	public int level;

	public int startPid;

	public int endPid;

	public string nameBase;

	public int startScore;

	public string updateCountry = "czech";

	private int currentPid = -1;

	private OnComplete onPopulationComplete;

	public void UpdateLeaderboards(OnComplete onComplete)
	{
		onPopulationComplete = onComplete;
		currentPid = ((currentPid >= 0) ? (currentPid + 1) : startPid);
		GGServerRequestsBackend.UpdateRequest updateRequest = new GGServerRequestsBackend.UpdateRequest(level, nameBase + currentPid, updateCountry.ToLower());
		int num = startScore;
		updateRequest.SetScore(num);
		updateRequest.SetPid(currentPid.ToString());
		UnityEngine.Debug.Log("pid: " + currentPid + ", name: " + nameBase + ", level: " + level + ", score: " + num);
		StartCoroutine(DoUpdateUser(updateRequest, onRequestComplete));
	}

	private IEnumerator DoUpdateUser(GGServerRequestsBackend.UpdateRequest update, GGServerRequestsBackend.OnComplete onComplete)
	{
		return new _003CDoUpdateUser_003Ed__10(0)
		{
			_003C_003E4__this = this,
			update = update,
			onComplete = onComplete
		};
	}

	public void onRequestComplete(GGServerRequestsBackend.ServerRequest request)
	{
		if (currentPid < endPid)
		{
			UpdateLeaderboards(onPopulationComplete);
			return;
		}
		currentPid = -1;
		if (onPopulationComplete != null)
		{
			onPopulationComplete();
			onPopulationComplete = null;
		}
	}
}
