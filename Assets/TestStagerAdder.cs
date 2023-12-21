using System;
using System.Collections;
using System.Collections.Generic;
using GGMatch3;
using UnityEngine;

public class TestStagerAdder : MonoBehaviour
{
    public GiftBoxScreen scree;
    public int startStage = 10;
    public int maxStage = 350;
    
    [ContextMenu("AddStage")]
    public IEnumerator AddStage()
    {
        var match3DB = Match3StagesDB.instance;
        var stages = match3DB.stages;
        var delkay = new WaitForSeconds(0.05f);
        while (match3DB.stages.Count != 2000)
        {
            var stage = stages[startStage];
            match3DB.stages.Add(stage);
            startStage++;
            if (startStage > 350)
            {
                startStage = 10;
            }
            Debug.Log(match3DB.stages.Count);
            yield return delkay;
        }
        Debug.LogError("Add levels complete!");
    }

    [ContextMenu("test")]
    public void ShowGift()
    {
        scree.Show(new GiftBoxScreen.ShowArguments());
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(AddStage());
        }
    }
}
