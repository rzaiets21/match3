using ProtoModels;
using System;

namespace GGMatch3
{
	[Serializable]
	public class BoosterConfig
	{
		public BoosterType boosterType;

		public ChipType chipType => BoosterToChipType(boosterType);

		public static ChipType BoosterToChipType(BoosterType booster)
		{
			switch (booster)
			{
			case BoosterType.BombBooster:
				return ChipType.Bomb;
			case BoosterType.DiscoBooster:
				return ChipType.DiscoBall;
			case BoosterType.VerticalRocketBooster:
				return ChipType.VerticalRocket;
			case BoosterType.SeekingMissle:
				return ChipType.SeekingMissle;
			default:
				return ChipType.VerticalRocket;
			}
		}

		public static ProtoModels.BoosterType BoosterToProtoType(BoosterType booster)
		{
			switch (booster)
			{
			case BoosterType.BombBooster:
				return ProtoModels.BoosterType.BombBooster;
			case BoosterType.DiscoBooster:
				return ProtoModels.BoosterType.DiscoBooster;
			default:
				return ProtoModels.BoosterType.VericalRocketBooster;
			}
		}
	}
}
