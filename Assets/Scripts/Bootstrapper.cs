using System;
using System.Collections;
using System.Collections.Generic;
using GGMatch3;
using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private void OnEnable()
    {
        GameScreen object2 = NavigationManager.instance.GetObject<GameScreen>();
        Match3StagesDB.Stage currentStage = Match3StagesDB.instance.currentStage;
        var initParams = new Match3GameParams();
        initParams.level = Resources.Load<LevelSettings>(nameof(LevelSettings)).LevelDefinition;

        initParams.stage = currentStage;
        initParams.levelIndex = currentStage.index;
        GiftsDefinitionDB.BuildupBooster.BoosterGift boosterGift = ScriptableObjectSingleton<GiftsDefinitionDB>.instance.buildupBooster.GetBoosterGift();
        if (boosterGift != null)
        {
            initParams.giftBoosterLevel = boosterGift.level;
            List<BoosterConfig> boosterConfig = boosterGift.boosterConfig;
            for (int j = 0; j < boosterConfig.Count; j++)
            {
                BoosterConfig item = boosterConfig[j];
                initParams.activeBoosters.Add(item);
            }
        }
        object2.Show(initParams);
        GGSoundSystem.Play(GGSoundSystem.MusicType.GameMusic);
    }
}
