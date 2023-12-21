using Expressive;
using Expressive.Expressions;
using Expressive.Functions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DecoratingSceneConfig : ScriptableObjectSingleton<DecoratingSceneConfig>
{
	[Serializable]
	public class CharacterAnimationLine
	{
		public enum LineType
		{
			NamedAnimation,
			Pause
		}

		[SerializeField]
		public LineType lineType;

		[SerializeField]
		public bool isCharacterVisible;

		[SerializeField]
		public string namedAnimationRoom;

		[SerializeField]
		public string namedAnimationName;

		[SerializeField]
		public int namedAnimationIndex;

		[SerializeField]
		public float pauseDuration;

		public CharacterAnimation GetCharacterAnimation(DecoratingSceneConfig config)
		{
			RoomAnimationList roomAnimationList = config.GetRoomAnimationList(namedAnimationRoom);
			if (roomAnimationList == null)
			{
				return null;
			}
			AnimationsList animationsForSceneObject = roomAnimationList.GetAnimationsForSceneObject(namedAnimationName);
			if (animationsForSceneObject == null || namedAnimationIndex >= animationsForSceneObject.animations.Count)
			{
				return null;
			}
			return animationsForSceneObject.animations[namedAnimationIndex];
		}
	}

	[Serializable]
	public class CharacterAnimationSequence
	{
		[SerializeField]
		public string characterName;

		[SerializeField]
		public List<CharacterAnimationLine> animationLines = new List<CharacterAnimationLine>();
	}

	[Serializable]
	public class AnimationSequence
	{
		private class ExpressionFunctions
		{
			internal class UnlockedFunction : IFunction
			{
				public ExpressionFunctions functions;

				private IDictionary<string, object> _003CVariables_003Ek__BackingField;

				public IDictionary<string, object> Variables
				{
					get
					{
						return _003CVariables_003Ek__BackingField;
					}
					set
					{
						_003CVariables_003Ek__BackingField = value;
					}
				}

				public string Name => "u";

				public object Evaluate(IExpression[] parameters, ExpressiveOptions options)
				{
					return functions.IsUnlocked(parameters[0].Evaluate(Variables) as string);
				}
			}

			internal class OwnedFunction : IFunction
			{
				public ExpressionFunctions functions;

				private IDictionary<string, object> _003CVariables_003Ek__BackingField;

				public IDictionary<string, object> Variables
				{
					get
					{
						return _003CVariables_003Ek__BackingField;
					}
					set
					{
						_003CVariables_003Ek__BackingField = value;
					}
				}

				public string Name => "o";

				public object Evaluate(IExpression[] parameters, ExpressiveOptions options)
				{
					return functions.IsOwned(parameters[0].Evaluate(Variables) as string);
				}
			}

			private UnlockedFunction unlockedFunction = new UnlockedFunction();

			private OwnedFunction ownedFunction = new OwnedFunction();

			private DecoratingScene scene;

			public void Start(DecoratingScene scene)
			{
				this.scene = scene;
			}

			public void End()
			{
				scene = null;
			}

			public void RegisterFunctions(Expression expression)
			{
				unlockedFunction.functions = this;
				ownedFunction.functions = this;
				expression.RegisterFunction(unlockedFunction);
				expression.RegisterFunction(ownedFunction);
			}

			public bool IsUnlocked(string name)
			{
				if (scene == null)
				{
					return false;
				}
				VisualObjectBehaviour behaviour = scene.GetBehaviour(name);
				if (behaviour == null)
				{
					return false;
				}
				if (behaviour.visualObject.IsUnlocked(scene))
				{
					return !behaviour.visualObject.isOwned;
				}
				return false;
			}

			public bool IsOwned(string name)
			{
				if (scene == null)
				{
					return false;
				}
				VisualObjectBehaviour behaviour = scene.GetBehaviour(name);
				if (behaviour == null)
				{
					return false;
				}
				return behaviour.visualObject.isOwned;
			}
		}

		private static ExpressionFunctions expressionFunctions = new ExpressionFunctions();

		[SerializeField]
		public string name;

		[SerializeField]
		public string expressionString;

		[SerializeField]
		public string openAnimationFor;

		[SerializeField]
		public bool leaveAfterInit;

		[SerializeField]
		public AnimationBeats.TestSetup testSetup = new AnimationBeats.TestSetup();

		[SerializeField]
		public List<CharacterAnimationSequence> characters = new List<CharacterAnimationSequence>();

		private Expression availableExpression
		{
			get
			{
				if (string.IsNullOrEmpty(expressionString))
				{
					return null;
				}
				Expression expression = new Expression(expressionString);
				expressionFunctions.RegisterFunctions(expression);
				return expression;
			}
		}

		public bool IsForOpenAnimation(string objectName)
		{
			return openAnimationFor == objectName;
		}

		public bool isAvailable(DecoratingScene scene)
		{
			Expression availableExpression = this.availableExpression;
			if (availableExpression == null)
			{
				return false;
			}
			expressionFunctions.Start(scene);
			bool flag = false;
			try
			{
				flag = Convert.ToBoolean(availableExpression.Evaluate());
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("Problem with expression " + expressionString + "\n" + ex.ToString());
				flag = false;
			}
			expressionFunctions.End();
			return flag;
		}
	}

	[Serializable]
	public class AnimationSequenceGroup
	{
		[SerializeField]
		public string groupName;

		[SerializeField]
		public List<AnimationSequence> animations = new List<AnimationSequence>();

		private List<AnimationSequence> availableSequences = new List<AnimationSequence>();

		public List<AnimationSequence> AvailableSequences(DecoratingScene scene)
		{
			availableSequences.Clear();
			for (int i = 0; i < animations.Count; i++)
			{
				AnimationSequence animationSequence = animations[i];
				if (animationSequence.isAvailable(scene))
				{
					availableSequences.Add(animationSequence);
				}
			}
			return availableSequences;
		}

		public AnimationSequence SequencForOpenAnimation(string objectName)
		{
			for (int i = 0; i < animations.Count; i++)
			{
				AnimationSequence animationSequence = animations[i];
				if (animationSequence.IsForOpenAnimation(objectName))
				{
					return animationSequence;
				}
			}
			return null;
		}
	}

	[Serializable]
	public class ScaleAnimationSettings
	{
		public string name;

		public Vector3 scaleFrom;

		public AnimationCurve scaleCurve;

		public float duration;

		public Vector3 localPositionFrom;

		public AnimationCurve localPositionCurve;
	}

	public interface IAnimationPart
	{
		bool isAnimationActive
		{
			get;
		}

		void ResetAnimationState();

		void StartAnimation(CharacterAvatar avatar);

		float Update(float deltaTime, CharacterAvatar avatar);
	}

	public interface IAnimationPartPlayer
	{
		bool isAnimationActive
		{
			get;
		}

		void StartAnimation(CharacterAvatar avatar);

		void UpdateAnimation(float deltaTime, CharacterAvatar avatar);

		void Init(List<CharacterAnimation.AnimationPart> parts);

		void Init(List<CharacterAnimation.TextAnimationPart> parts);

		void Init(List<CharacterAnimation.LookAnimationPart> parts);
	}

	public class AnimationPartPlayer : IAnimationPartPlayer
	{
		private struct AnimationState
		{
			public bool isActive;

			public int currentPartIndex;
		}

		private List<IAnimationPart> animationParts = new List<IAnimationPart>();

		private AnimationState animationState;

		public bool isAnimationActive => animationState.isActive;

		public void Init(List<CharacterAnimation.AnimationPart> parts)
		{
			animationParts.Clear();
			for (int i = 0; i < parts.Count; i++)
			{
				CharacterAnimation.AnimationPart item = parts[i];
				animationParts.Add(item);
			}
		}

		public void Init(List<CharacterAnimation.TextAnimationPart> parts)
		{
			animationParts.Clear();
			for (int i = 0; i < parts.Count; i++)
			{
				CharacterAnimation.TextAnimationPart item = parts[i];
				animationParts.Add(item);
			}
		}

		public void Init(List<CharacterAnimation.LookAnimationPart> parts)
		{
			animationParts.Clear();
			for (int i = 0; i < parts.Count; i++)
			{
				CharacterAnimation.LookAnimationPart item = parts[i];
				animationParts.Add(item);
			}
		}

		public void StartAnimation(CharacterAvatar avatar)
		{
			animationState = default(AnimationState);
			animationState.isActive = true;
			if (animationParts.Count == 0)
			{
				animationState.isActive = false;
				return;
			}
			for (int i = 0; i < animationParts.Count; i++)
			{
				animationParts[i].ResetAnimationState();
			}
			animationParts[0].StartAnimation(avatar);
		}

		public void UpdateAnimation(float deltaTime, CharacterAvatar avatar)
		{
			if (!animationState.isActive)
			{
				return;
			}
			while (animationState.currentPartIndex < animationParts.Count)
			{
				IAnimationPart animationPart = animationParts[animationState.currentPartIndex];
				if (!animationPart.isAnimationActive)
				{
					animationPart.StartAnimation(avatar);
				}
				deltaTime = animationPart.Update(deltaTime, avatar);
				if (!animationPart.isAnimationActive)
				{
					animationState.currentPartIndex++;
				}
				if (deltaTime <= 0f)
				{
					return;
				}
			}
			animationState.isActive = false;
		}
	}

	[Serializable]
	public class CharacterAnimation
	{
		[Serializable]
		public class TextAnimationPart : IAnimationPart
		{
			private struct AnimationState
			{
				public bool isAnimating;

				public float time;
			}

			public string text;

			public float duration;

			[NonSerialized]
			private AnimationState animationState;

			public bool isAnimationActive => animationState.isAnimating;

			public void ResetAnimationState()
			{
				animationState = default(AnimationState);
			}

			public void StartAnimation(CharacterAvatar avatar)
			{
				animationState = default(AnimationState);
				animationState.isAnimating = true;
				if (string.IsNullOrEmpty(text))
				{
					avatar.HideSpeachBubble();
				}
				else
				{
					avatar.ShowSpeachBubble(text);
				}
			}

			public float Update(float deltaTime, CharacterAvatar avatar)
			{
				if (!animationState.isAnimating)
				{
					return deltaTime;
				}
				float num = duration - animationState.time;
				animationState.time += deltaTime;
				if (animationState.time >= duration)
				{
					avatar.HideSpeachBubble();
					animationState.isAnimating = false;
				}
				return Mathf.Max(0f, deltaTime - num);
			}
		}

		[Serializable]
		public class LookAnimationPart : IAnimationPart
		{
			public enum LookAt
			{
				Position,
				LocalOffset,
				Character,
				ObjectInRoom,
				Pause,
				ChangeWeight,
				Marker
			}

			private struct AnimationState
			{
				public bool isAnimating;

				public float time;
			}

			public LookAt lookAtType;

			public string lookAtName;

			public float weight;

			public Vector3 offset;

			public float transitionDuration = 0.5f;

			public float duration;

			[NonSerialized]
			private AnimationState animationState;

			public bool isAnimationActive => animationState.isAnimating;

			public void ResetAnimationState()
			{
				animationState = default(AnimationState);
			}

			public void StartAnimation(CharacterAvatar avatar)
			{
				animationState = default(AnimationState);
				animationState.isAnimating = true;
				if (lookAtType == LookAt.ChangeWeight)
				{
					avatar.ChangeLookAtWeight(weight, transitionDuration);
				}
				else if (lookAtType != LookAt.Pause)
				{
					Vector3 position = LookAtPosition(avatar);
					avatar.LookAt(position, weight, transitionDuration);
				}
			}

			public float Update(float deltaTime, CharacterAvatar avatar)
			{
				if (!animationState.isAnimating)
				{
					return deltaTime;
				}
				float num = duration - animationState.time;
				animationState.time += deltaTime;
				if (animationState.time >= duration)
				{
					animationState.isAnimating = false;
				}
				return Mathf.Max(0f, deltaTime - num);
			}

			private Vector3 LookAtPosition(CharacterAvatar avatar)
			{
				if (lookAtType == LookAt.Position)
				{
					return offset;
				}
				if (lookAtType == LookAt.LocalOffset)
				{
					return avatar.transform.forward * offset.z + avatar.transform.right * offset.x + avatar.transform.up * offset.y;
				}
				if (lookAtType == LookAt.Character)
				{
					if (avatar.decoratingScene == null)
					{
						return offset;
					}
					CharacterAvatar avatar2 = avatar.decoratingScene.animationPlayer.GetAvatar(lookAtName);
					if (avatar2 == null)
					{
						return offset;
					}
					return avatar2.headPosition + offset;
				}
				if (lookAtType == LookAt.Marker)
				{
					Transform marker = avatar.decoratingScene.animationPlayer.GetMarker(lookAtName);
					if (marker == null)
					{
						return offset;
					}
					return marker.position + offset;
				}
				if (lookAtType == LookAt.ObjectInRoom)
				{
					VisualObjectBehaviour behaviour = avatar.decoratingScene.GetBehaviour(lookAtName);
					if (behaviour == null)
					{
						return offset;
					}
					return behaviour.characterBehaviour.sceneItem.lookAtPosition + offset;
				}
				return offset;
			}
		}

		[Serializable]
		public class AnimationPart : IAnimationPart
		{
			private struct AnimationState
			{
				public bool isAnimating;

				public float time;
			}

			public bool setTransform;

			public Vector3 position;

			public Vector3 rotationEuler;

			public float duration;

			public string property;

			[NonSerialized]
			private AnimationState animationState;

			public bool isAnimationActive => animationState.isAnimating;

			public void ResetAnimationState()
			{
				animationState = default(AnimationState);
			}

			public void StartAnimation(CharacterAvatar avatar)
			{
				animationState = default(AnimationState);
				animationState.isAnimating = true;
				if (setTransform)
				{
					Vector3 vector = position;
					Vector3 rotation = rotationEuler;
					avatar.SetPosition(vector);
					avatar.SetRotation(rotation);
				}
				avatar.SetProperty(property, value: true);
			}

			public float Update(float deltaTime, CharacterAvatar avatar)
			{
				if (!animationState.isAnimating)
				{
					return deltaTime;
				}
				float num = duration - animationState.time;
				animationState.time += deltaTime;
				if (animationState.time >= duration)
				{
					avatar.SetProperty(property, value: false);
					animationState.isAnimating = false;
				}
				return Mathf.Max(0f, deltaTime - num);
			}
		}

		[SerializeField]
		public bool playAtInit;

		[SerializeField]
		public bool leaveAfterInit;

		[SerializeField]
		public List<AnimationPart> animationParts = new List<AnimationPart>();

		[SerializeField]
		public List<TextAnimationPart> textAnimations = new List<TextAnimationPart>();

		[SerializeField]
		public List<LookAnimationPart> lookAnimations = new List<LookAnimationPart>();

		[SerializeField]
		private List<string> unlockedThatEnable = new List<string>();

		[SerializeField]
		private List<string> ownedThatDisable = new List<string>();

		[SerializeField]
		private List<string> ownedThaEnable = new List<string>();

		private IAnimationPartPlayer partPlayer = new AnimationPartPlayer();

		private IAnimationPartPlayer textPartPlayer = new AnimationPartPlayer();

		private IAnimationPartPlayer lookPartPlayer = new AnimationPartPlayer();

		private List<IAnimationPartPlayer> players = new List<IAnimationPartPlayer>();

		[NonSerialized]
		public AnimationsList animationList;

		public bool isAnimationActive
		{
			get
			{
				for (int i = 0; i < players.Count; i++)
				{
					if (players[i].isAnimationActive)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void Init(AnimationsList animationList)
		{
			this.animationList = animationList;
		}

		public void StartAnimation(CharacterAvatar avatar)
		{
			partPlayer.Init(animationParts);
			textPartPlayer.Init(textAnimations);
			lookPartPlayer.Init(lookAnimations);
			players.Clear();
			players.Add(partPlayer);
			players.Add(textPartPlayer);
			players.Add(lookPartPlayer);
			for (int i = 0; i < players.Count; i++)
			{
				players[i].StartAnimation(avatar);
			}
		}

		public void UpdateAnimation(float deltaTime, CharacterAvatar avatar)
		{
			if (isAnimationActive)
			{
				for (int i = 0; i < players.Count; i++)
				{
					players[i].UpdateAnimation(deltaTime, avatar);
				}
			}
		}
	}

	[Serializable]
	public class RoomAnimationList
	{
		[SerializeField]
		public string roomName;

		[SerializeField]
		public List<AnimationsList> sceneObjectAnimations = new List<AnimationsList>();

		public void Init()
		{
			for (int i = 0; i < sceneObjectAnimations.Count; i++)
			{
				sceneObjectAnimations[i].Init();
			}
		}

		public AnimationsList GetAnimationsForSceneObject(string sceneObjectName)
		{
			for (int i = 0; i < sceneObjectAnimations.Count; i++)
			{
				AnimationsList animationsList = sceneObjectAnimations[i];
				if (animationsList.sceneObjectName == sceneObjectName)
				{
					return animationsList;
				}
			}
			return null;
		}
	}

	[Serializable]
	public class AnimationsList
	{
		public string sceneObjectName;

		public bool isDefaultAnimation;

		public List<CharacterAnimation> animations = new List<CharacterAnimation>();

		private List<CharacterAnimation> availableAnimations_ = new List<CharacterAnimation>();

		public void Init()
		{
			for (int i = 0; i < animations.Count; i++)
			{
				animations[i].Init(this);
			}
		}
	}

	[Serializable]
	public class VisualObjectOverride
	{
		public bool isSettingSaved;

		public string visualObjectName;

		public Vector3 iconHandlePositionOffset;

		public Vector3 iconHandlePositionScale = Vector3.one;

		public Vector3 iconHandleRotation = Vector3.zero;
	}

	[Serializable]
	public class RoomConfig
	{
		public string roomName;

		[SerializeField]
		private List<VisualObjectOverride> objectOverrides = new List<VisualObjectOverride>();

		public VisualObjectOverride GetObjectOverride(string objectName)
		{
			for (int i = 0; i < objectOverrides.Count; i++)
			{
				VisualObjectOverride visualObjectOverride = objectOverrides[i];
				if (visualObjectOverride.visualObjectName == objectName)
				{
					return visualObjectOverride;
				}
			}
			return null;
		}
	}

	[SerializeField]
	private bool animationsEnabled;

	public float additionalTimeForRandomAnimation = 7f;

	public float additionalTimeForNewObjectAnimation = 7f;

	public float additionalInitialTime = 2f;

	public AnimationCurve headAnimationCurve;

	public Vector3 speachBubbleHeadOffset;

	public List<AnimationBeats.Sequence> beatsSequences = new List<AnimationBeats.Sequence>();

	[SerializeField]
	public List<AnimationSequenceGroup> animations = new List<AnimationSequenceGroup>();

	[SerializeField]
	public List<RoomAnimationList> roomCharacterAnimations = new List<RoomAnimationList>();

	public ScaleAnimationSettings scaleAnimationSettings = new ScaleAnimationSettings();

	public List<ScaleAnimationSettings> scaleAnimationSettingsList = new List<ScaleAnimationSettings>();

	[SerializeField]
	public List<RoomConfig> roomConfigs = new List<RoomConfig>();

	public AnimationBeats.Sequence GetBeatsSequence(string groupName, string animationName)
	{
		for (int i = 0; i < beatsSequences.Count; i++)
		{
			AnimationBeats.Sequence sequence = beatsSequences[i];
			if (sequence.groupName == groupName && sequence.animationName == animationName)
			{
				return sequence;
			}
		}
		return null;
	}

	public AnimationSequenceGroup GetAnimationSequenceGroup(string name)
	{
		if (!animationsEnabled)
		{
			return null;
		}
		for (int i = 0; i < animations.Count; i++)
		{
			AnimationSequenceGroup animationSequenceGroup = animations[i];
			if (animationSequenceGroup.groupName == name)
			{
				return animationSequenceGroup;
			}
		}
		return null;
	}

	public RoomAnimationList GetRoomAnimationList(string roomName)
	{
		for (int i = 0; i < roomCharacterAnimations.Count; i++)
		{
			RoomAnimationList roomAnimationList = roomCharacterAnimations[i];
			if (roomAnimationList.roomName == roomName)
			{
				return roomAnimationList;
			}
		}
		return null;
	}

	public AnimationsList GetAnimationsForSceneObject(string roomName, string sceneObjectName)
	{
		return GetRoomAnimationList(roomName)?.GetAnimationsForSceneObject(sceneObjectName);
	}

	public RoomConfig GetRoomConfig(string roomName)
	{
		for (int i = 0; i < roomConfigs.Count; i++)
		{
			RoomConfig roomConfig = roomConfigs[i];
			if (roomConfig.roomName == roomName)
			{
				return roomConfig;
			}
		}
		return null;
	}

	public ScaleAnimationSettings GetScaleAnimationSettingsOrDefault(string name)
	{
		for (int i = 0; i < scaleAnimationSettingsList.Count; i++)
		{
			ScaleAnimationSettings scaleAnimationSettings = scaleAnimationSettingsList[i];
			if (scaleAnimationSettings.name == name)
			{
				return scaleAnimationSettings;
			}
		}
		return this.scaleAnimationSettings;
	}

	protected override void UpdateData()
	{
		base.UpdateData();
		for (int i = 0; i < roomCharacterAnimations.Count; i++)
		{
			roomCharacterAnimations[i].Init();
		}
	}
}
