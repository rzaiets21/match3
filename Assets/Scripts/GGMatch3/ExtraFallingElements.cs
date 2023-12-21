using System;
using System.Collections.Generic;

namespace GGMatch3
{
	[Serializable]
	public class ExtraFallingElements
	{
		[Serializable]
		public class ExtraFallingElement
		{
			public int minMovesBeforeActive;

			public int minCreatedChipsBeforeReactivate;

			public ExtraFallingElement Clone()
			{
				return new ExtraFallingElement
				{
					minMovesBeforeActive = minMovesBeforeActive
				};
			}
		}

		public List<ExtraFallingElement> fallingElementsList = new List<ExtraFallingElement>();

		public ExtraFallingElements Clone()
		{
			ExtraFallingElements extraFallingElements = new ExtraFallingElements();
			for (int i = 0; i < fallingElementsList.Count; i++)
			{
				ExtraFallingElement extraFallingElement = fallingElementsList[i];
				extraFallingElements.fallingElementsList.Add(extraFallingElement.Clone());
			}
			return extraFallingElements;
		}
	}
}
