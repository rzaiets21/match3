using System.Collections.Generic;
using GGMatch3;

namespace Extensions
{
    public static class StageExtension
    {
        public static Match3GameParams GetInitParams(this Match3StagesDB.Stage stage)
        {
            var initParams = new Match3GameParams();
            initParams.level = stage.levelReference.level;
            if (stage.multiLevelReference.Count > 0)
            {
                List<Match3StagesDB.LevelReference> multiLevelReference = stage.multiLevelReference;
                for (int i = 0; i < multiLevelReference.Count; i++)
                {
                    LevelDefinition level = multiLevelReference[i].level;
                    if (level != null)
                    {
                        initParams.levelsList.Add(level);
                    }
                }
            }
            initParams.stage = stage;
            initParams.levelIndex = stage.index;
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

            return initParams;
        }
        
        public static Match3GameParams GetInitParams(this Match3StagesDB.Stage stage, Match3GameListener listener)
        {
            var initParams = stage.GetInitParams();
            initParams.listener = listener;

            return initParams;
        }
    }
}