using ProtoModels;
using UnityEngine;

public class PlayerPositionRequestTest : MonoBehaviour
{
	public delegate void OnComplete();

	public int startPid;

	public int endPid;

	private OnComplete onPopulationComplete;

	public void GetPlayerPositionList(OnComplete onComplete)
	{
		onPopulationComplete = onComplete;
		GGServerRequestsBackend.GetPlayerPositionsRequest getPlayerPositionsRequest = new GGServerRequestsBackend.GetPlayerPositionsRequest();
		PlayerPositions playerPositions = new PlayerPositions();
		for (int i = startPid; i <= endPid; i++)
		{
			PlayerPositions.PlayerPosition playerPosition = new PlayerPositions.PlayerPosition();
			playerPosition.playerId = i.ToString();
			playerPositions.players.Add(playerPosition);
		}
		getPlayerPositionsRequest.AddData(playerPositions);
		BehaviourSingletonInit<GGServerRequestsBackend>.instance.GetPlayerPositionList(getPlayerPositionsRequest, OnRequestComplete);
	}

	public void OnRequestComplete(GGServerRequestsBackend.ServerRequest request)
	{
		if (onPopulationComplete != null)
		{
			onPopulationComplete();
			onPopulationComplete = null;
		}
	}
}
