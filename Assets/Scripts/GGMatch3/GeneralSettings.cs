using System;
using UnityEngine;

namespace GGMatch3
{
	[Serializable]
	public class GeneralSettings
	{
		public enum BombRangeType
		{
			Full,
			Circle,
			Diamond,
			Candy
		}

		public enum SeekingRangeType
		{
			Normal,
			Candy
		}

		[Serializable]
		public class CameraShakeSettings
		{
			[SerializeField]
			public float magnitude = 1f;

			[SerializeField]
			public float roughness;

			[SerializeField]
			public float fadeInTime;

			[SerializeField]
			public float fadeOutTime;

			[SerializeField]
			public Vector3 posInfluence;

			[SerializeField]
			public Vector3 rotInfluence;
		}

		public float chipScaleMult = 1f;

		public float pickupScaleMult = 1f;

		public float bombScaleMult = 1f;

		public float trapScaleMult = 0.9f;

		public float shakeScale;

		public bool preventAutomatchesIfPossible;

		public bool strictAsPossibleToPrevent;

		public bool waitTillBoardStopsForMatches;

		public bool waitIfRocketHitsPowerup;

		public float scoreSpeed = 40f;

		public float lerpSpeed = 2f;

		public CameraShakeSettings shakeSettings;

		public BombRangeType bombRangeType => BombRangeType.Diamond;

		public SeekingRangeType seekingRangeType => SeekingRangeType.Normal;
	}
}
