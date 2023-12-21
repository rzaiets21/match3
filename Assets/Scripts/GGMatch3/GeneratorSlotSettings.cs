using System;
using System.Collections.Generic;

namespace GGMatch3
{
	[Serializable]
	public class GeneratorSlotSettings
	{
		public float weight;

		public bool hasIce;

		public int iceLevel;

		public List<LevelDefinition.ChipGenerationSettings.ChipSetting> chipSettings = new List<LevelDefinition.ChipGenerationSettings.ChipSetting>();

		public GeneratorSlotSettings Clone()
		{
			GeneratorSlotSettings generatorSlotSettings = new GeneratorSlotSettings();
			generatorSlotSettings.weight = weight;
			generatorSlotSettings.hasIce = hasIce;
			generatorSlotSettings.iceLevel = iceLevel;
			for (int i = 0; i < chipSettings.Count; i++)
			{
				LevelDefinition.ChipGenerationSettings.ChipSetting chipSetting = chipSettings[i];
				generatorSlotSettings.chipSettings.Add(chipSetting.Clone());
			}
			return generatorSlotSettings;
		}
	}
}
