using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationBeats
{
	[Serializable]
	public class Animation
	{
		public enum AnimationType
		{
			PlayAnimation,
			Say,
			LookAtRoom,
			LookAtCharacter,
			Animate,
			StopLooking,
			SetTransform,
			LookAtMarker,
			SetTransformToMarker,
			LookAtPosition
		}

		public enum Character
		{
			C0,
			C1
		}

		public Character character;

		public AnimationType type;

		public string stringValue;

		public Vector3 position;

		public Vector3 rotation;

		public float duration;

		public float transitionDuration;

		public float startOffset;
	}

	[Serializable]
	public class Beat
	{
		[SerializeField]
		private string name;

		public List<Animation> animations = new List<Animation>();
	}

	[Serializable]
	public class TestSetup
	{
		public List<string> objectsToOwn = new List<string>();

		public bool ShouldBeOwned(string name)
		{
			return objectsToOwn.Contains(name);
		}
	}

	[Serializable]
	public class Sequence
	{
		[Serializable]
		public class MarkerTransform
		{
			public string name;

			public Vector3 position;

			public Vector3 rotationEuler;
		}

		public string groupName;

		public string animationName;

		public List<Beat> beats = new List<Beat>();

		public bool isExpressionSet;

		public string expressionString;

		public string openAnimationFor;

		public TestSetup testSetup = new TestSetup();

		[SerializeField]
		public List<MarkerTransform> markers = new List<MarkerTransform>();
	}
}
