using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergyGoalTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private DateTime targetTime;

    private void Update()
    {
        var today = DateTime.Today.AddDays(1);
        var date = today - DateTime.Now;
        timerText.SetText(date.ToString(@"hh\:mm\:ss"));
    }
}
