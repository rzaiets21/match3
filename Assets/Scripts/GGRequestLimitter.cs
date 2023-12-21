using System.Collections;
using System.Collections.Generic;

public class GGRequestLimitter : BehaviourSingleton<GGRequestLimitter>
{
	public class RunningRequest
	{
		public IEnumerator query;

		public GGServerRequestsBackend.ServerRequest request;
	}

	public int requestLimit = 4;

	private int nextUnusedId;

	private List<GGServerRequestsBackend.ServerRequest> pendingRequests = new List<GGServerRequestsBackend.ServerRequest>();

	private List<RunningRequest> runningRequests = new List<RunningRequest>();

	public int GetGroupId()
	{
		return nextUnusedId++;
	}

	public void Add(GGServerRequestsBackend.ServerRequest request)
	{
		pendingRequests.Add(request);
	}

	private void Update()
	{
		CheckRunningRequests();
		StartNewRequests();
	}

	private void CheckRunningRequests()
	{
		for (int num = runningRequests.Count - 1; num >= 0; num--)
		{
			RunningRequest runningRequest = runningRequests[num];
			if (!runningRequest.query.MoveNext())
			{
				if (runningRequest.request.onComplete != null)
				{
					runningRequest.request.onComplete(runningRequest.request);
				}
				runningRequests.RemoveAt(num);
			}
		}
	}

	private void StartNewRequests()
	{
		while (runningRequests.Count < requestLimit && pendingRequests.Count > 0)
		{
			RunningRequest runningRequest = new RunningRequest();
			runningRequest.request = pendingRequests[0];
			pendingRequests.RemoveAt(0);
			if (runningRequest.request != null)
			{
				runningRequest.query = runningRequest.request.RequestCoroutine();
				runningRequests.Add(runningRequest);
			}
		}
	}

	public void StopRequest(GGServerRequestsBackend.ServerRequest request)
	{
		if (request == null)
		{
			return;
		}
		for (int num = pendingRequests.Count - 1; num >= 0; num--)
		{
			if (pendingRequests[num] == request)
			{
				pendingRequests.RemoveAt(num);
				request.onComplete = null;
			}
		}
		for (int num2 = runningRequests.Count - 1; num2 >= 0; num2--)
		{
			if (runningRequests[num2].request == request)
			{
				runningRequests.RemoveAt(num2);
				request.onComplete = null;
			}
		}
	}

	public void StopRequestsWithGroup(int groupId)
	{
		for (int num = pendingRequests.Count - 1; num >= 0; num--)
		{
			if (pendingRequests[num].groupId == groupId)
			{
				pendingRequests.RemoveAt(num);
			}
		}
		for (int num2 = runningRequests.Count - 1; num2 >= 0; num2--)
		{
			if (runningRequests[num2].request.groupId == groupId)
			{
				runningRequests.RemoveAt(num2);
			}
		}
	}
}
