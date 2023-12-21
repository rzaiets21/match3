using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyGoalsPanel : MonoBehaviour
{
    [SerializeField] private EnergyGoals energyGoals;
    [SerializeField] private GameObject energyTimer;
    [SerializeField] private EnergyCompleteScreen completeScreen;
    [SerializeField] private RectTransform energyRect;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [SerializeField] private TextMeshProUGUI currentScore;
    [SerializeField] private TextMeshProUGUI targetScore;
    [SerializeField] private TextMeshProUGUI energyReward;
    [SerializeField] private TextMeshProUGUI progress;
    [SerializeField] private Image progressBarImage;

    private EnergyGoal _currentGoal;

    private bool _inAnimation;
    private int _reward;

    private void OnEnable()
    {
        energyGoals.OnScoreUpdate += UpdateVisual;
        energyGoals.OnGoalComplete += OnCompleteGoal;
        energyGoals.OnGoalsReset += OnGoalsReset;

        _currentGoal = energyGoals.CurrentGoal;
        energyTimer.SetActive(_currentGoal == null);
        canvasGroup.alpha = _currentGoal != null ? 1 : 0;
        UpdateVisual();
    }

    private void OnDisable()
    {
        energyGoals.OnScoreUpdate -= UpdateVisual;
        energyGoals.OnGoalComplete -= OnCompleteGoal;
        energyGoals.OnGoalsReset -= OnGoalsReset;

        OnAnimationComplete();
        completeScreen.Hide();
    }

    public void Start()
    {
        _currentGoal = energyGoals.CurrentGoal;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if(_inAnimation)
            return;
        
        if (_currentGoal == null)
            return;
        
        var score = energyGoals.CurrentScore;
        var target = _currentGoal.TargetScore;
        
        currentScore.text = $"{score}";
        targetScore.text = $"{target}";
        energyReward.text = $"{_currentGoal.RewardCount}";

        var percent = (float)score / target;
        progress.text = (percent * 100).ToString("0") + "%";
        progressBarImage.fillAmount = percent;
    }

    private void OnCompleteGoal(EnergyGoal goal)
    {
        _inAnimation = true;
        canvasGroup.alpha = 0f;
        _reward = goal.RewardCount;
        completeScreen.Show(energyRect, _reward, OnAnimationComplete);
    }

    private void OnGoalsReset()
    {
        _currentGoal = energyGoals.CurrentGoal;
        canvasGroup.alpha = _currentGoal != null ? 1 : 0;
        energyTimer.SetActive(_currentGoal == null);
    }
    
    private void OnAnimationComplete()
    {
        if(!_inAnimation)
            return;

        EnergyManager.instance.SetEnergy(EnergyManager.instance.GetCurrentEnergyValue() + _reward * 10);
        _reward = 0;
        _currentGoal = energyGoals.CurrentGoal;
        canvasGroup.alpha = _currentGoal != null ? 1 : 0;
        energyTimer.SetActive(_currentGoal == null);
        UpdateVisual();
        _inAnimation = false;
    }
}
