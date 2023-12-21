using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnergyGoals : MonoBehaviour
{
    private const string PlayerPrefsDay = "GoalEnergyDay";
    private const string PlayerPrefsScore = "CurrentGoalScore";
    private const string PlayerPrefsIndex = "CurrentGoalIndex";
    
    [SerializeField] private List<EnergyGoal> goals = new List<EnergyGoal>();

    private int _goalIndex = -1;
    private int _day = -1;

    private int _currentScore = -1;

    public static EnergyGoals instance;

    public event Action OnScoreUpdate; 
    public event Action OnGoalsReset; 
    public event Action<EnergyGoal> OnGoalComplete; 

    public EnergyGoal CurrentGoal
    {
        get
        {
            if (_goalIndex < 0)
            {
                if(_day < 0)
                    _day = PlayerPrefs.GetInt(PlayerPrefsDay, 0);
                var today = DateTime.Now;
                if (_day != today.Day)
                {
                    ResetGoals(today.Day);
                }
                else
                    _goalIndex = PlayerPrefs.GetInt(PlayerPrefsIndex, 0);
            }
            else
            {
                if(_day < 0)
                    _day = PlayerPrefs.GetInt(PlayerPrefsDay, 0);
                var today = DateTime.Now;
                if (_day != today.Day)
                {
                    ResetGoals(today.Day);
                }
            }

            if (_goalIndex >= goals.Count)
                return null;
            
            return goals[_goalIndex];
        }
    }

    public int CurrentScore
    {
        get
        {
            if (_currentScore < 0)
            {
                _currentScore = PlayerPrefs.GetInt(PlayerPrefsScore, 0);
            }
            if (_currentScore < 0)
            {
                _currentScore = 0;
            }

            return _currentScore;
        }

        set
        {
            _currentScore = value;
            OnScoreUpdate?.Invoke();
            var currentGoal = CurrentGoal;
            if (currentGoal != null && _currentScore >= currentGoal.TargetScore)
            {
                var diff = _currentScore - currentGoal.TargetScore;
                _goalIndex++;
                _currentScore = diff;
                GoalComplete(currentGoal);
                
                PlayerPrefs.SetInt(PlayerPrefsScore, _currentScore);
            }
            
            PlayerPrefs.SetInt(PlayerPrefsScore, _currentScore);
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        _day = PlayerPrefs.GetInt(PlayerPrefsDay, 0);
    }

    private void Update()
    {
        var date = DateTime.Now;
        if (_day != date.Day)
        {
            ResetGoals(date.Day);
        }
    }

    private void ResetGoals(int day)
    {
        _day = day;
        _goalIndex = 0;
        CurrentScore = 0;
        PlayerPrefs.SetInt(PlayerPrefsDay, _day);
        PlayerPrefs.SetInt(PlayerPrefsIndex, _goalIndex);
        PlayerPrefs.SetInt(PlayerPrefsScore, _currentScore);
        OnGoalsReset?.Invoke();
    }
    
    private void OnDisable()
    {
        PlayerPrefs.SetInt(PlayerPrefsScore, _currentScore);
    }

    private void GoalComplete(EnergyGoal goal)
    {
        OnGoalComplete?.Invoke(goal);
    }
}

[Serializable]
public class EnergyGoal
{
    public int TargetScore;
    public int RewardCount;
}