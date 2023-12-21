using System;
using System.Collections.Generic;

namespace GGMatch3
{
	[Serializable]
	public class GeneratorSetup
	{
		[Serializable]
		public class GeneratorChipSetup
		{
			public ItemColor itemColor;

			public GeneratorChipSetup Clone()
			{
				return new GeneratorChipSetup
				{
					itemColor = itemColor
				};
			}
		}

		public IntVector2 position;

		public List<GeneratorChipSetup> chips = new List<GeneratorChipSetup>();

		public GeneratorSetup Clone()
		{
			GeneratorSetup generatorSetup = new GeneratorSetup();
			generatorSetup.position = position;
			for (int i = 0; i < chips.Count; i++)
			{
				GeneratorChipSetup generatorChipSetup = chips[i];
				generatorSetup.chips.Add(generatorChipSetup.Clone());
			}
			return generatorSetup;
		}
	}
}
