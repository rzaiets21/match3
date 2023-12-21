using System;
using System.Collections.Generic;
using GGMatch3;
using UnityEngine;

[CreateAssetMenu(menuName = "Create GoalDB")]
public class GoalsDB : ScriptableObject
{
    public List<GoalInfo> GoalInfos = new List<GoalInfo>();
}

[Serializable]
public class GoalInfo
{
    public List<GoalConfig> goals = new List<GoalConfig>();
    public List<GoalReward> rewards = new List<GoalReward>();
}
