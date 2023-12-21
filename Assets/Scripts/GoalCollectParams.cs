using GGMatch3;

public struct GoalCollectParams
{
	public Match3Goals.GoalBase goal;

	public SlotDestroyParams destroyParams;

	public GoalCollectParams(Match3Goals.GoalBase goal, SlotDestroyParams destroyParams)
	{
		this.goal = goal;
		this.destroyParams = destroyParams;
	}
}
