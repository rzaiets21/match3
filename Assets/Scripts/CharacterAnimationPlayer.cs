using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CharacterAnimationPlayer : MonoBehaviour
{
	private struct AnimationState
	{
		public IEnumerator animationEnumerator;

		public DecoratingSceneConfig.AnimationSequence animationSequence;

		public ChangeAnimationArguments arguments;
	}

	private sealed class _003CDoPlay_003Ed__20 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ChangeAnimationArguments arguments;

		public CharacterAnimationPlayer _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDoPlay_003Ed__20(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			CharacterAnimationPlayer characterAnimationPlayer = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				DecoratingSceneConfig.AnimationSequence animation = arguments.animation;
				characterAnimationPlayer.StopAvatarAnimations();
				characterAnimationPlayer.HideAvatars();
				List<DecoratingSceneConfig.CharacterAnimationSequence> characters = animation.characters;
				characterAnimationPlayer.enumList.Clear();
				for (int i = 0; i < characters.Count; i++)
				{
					DecoratingSceneConfig.CharacterAnimationSequence characterAnimationSequence = characters[i];
					CharacterAvatar avatar = characterAnimationPlayer.GetAvatar(characterAnimationSequence.characterName);
					if (!(avatar == null))
					{
						characterAnimationPlayer.enumList.Add(characterAnimationPlayer.DoPlaySequence(avatar, characterAnimationSequence));
					}
				}
				break;
			}
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (characterAnimationPlayer.enumList.Update())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
			}
			if (arguments.onComplete != null)
			{
				arguments.onComplete();
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private sealed class _003C_003Ec__DisplayClass21_0
	{
		public bool isAnimationRunning;

		internal void _003CDoPlaySequence_003Eb__0()
		{
			isAnimationRunning = false;
		}
	}

	private sealed class _003CDoPlaySequence_003Ed__21 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public DecoratingSceneConfig.CharacterAnimationSequence sequence;

		public CharacterAvatar avatar;

		public CharacterAnimationPlayer _003C_003E4__this;

		private _003C_003Ec__DisplayClass21_0 _003C_003E8__1;

		private List<DecoratingSceneConfig.CharacterAnimationLine> _003Clines_003E5__2;

		private int _003Ci_003E5__3;

		private DecoratingSceneConfig.CharacterAnimationLine _003Citem_003E5__4;

		private float _003Ctime_003E5__5;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDoPlaySequence_003Ed__21(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			CharacterAnimationPlayer characterAnimationPlayer = _003C_003E4__this;
			DecoratingSceneConfig.CharacterAnimation characterAnimation;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Clines_003E5__2 = sequence.animationLines;
				_003Ci_003E5__3 = 0;
				goto IL_0162;
			case 1:
				_003C_003E1__state = -1;
				goto IL_00b5;
			case 2:
				{
					_003C_003E1__state = -1;
					goto IL_0135;
				}
				IL_0162:
				if (_003Ci_003E5__3 < _003Clines_003E5__2.Count)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass21_0();
					_003Citem_003E5__4 = _003Clines_003E5__2[_003Ci_003E5__3];
					GGUtil.SetActive(avatar, _003Citem_003E5__4.isCharacterVisible);
					_003Ctime_003E5__5 = 0f;
					goto IL_00b5;
				}
				return false;
				IL_0150:
				_003Ci_003E5__3++;
				goto IL_0162;
				IL_0135:
				if (_003C_003E8__1.isAnimationRunning)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				_003C_003E8__1 = null;
				_003Citem_003E5__4 = null;
				goto IL_0150;
				IL_00b5:
				if (_003Ctime_003E5__5 <= _003Citem_003E5__4.pauseDuration)
				{
					_003Ctime_003E5__5 += characterAnimationPlayer.deltaTime;
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				characterAnimation = _003Citem_003E5__4.GetCharacterAnimation(ScriptableObjectSingleton<DecoratingSceneConfig>.instance);
				if (characterAnimation != null)
				{
					CharacterAvatar.ChangeAnimationArguments animationArguments = default(CharacterAvatar.ChangeAnimationArguments);
					animationArguments.animation = characterAnimation;
					_003C_003E8__1.isAnimationRunning = true;
					animationArguments.onComplete = _003C_003E8__1._003CDoPlaySequence_003Eb__0;
					avatar.RunAnimation(animationArguments);
					goto IL_0135;
				}
				goto IL_0150;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[SerializeField]
	public string beatToPlay;

	[SerializeField]
	private List<CharacterAvatar> avatars = new List<CharacterAvatar>();

	[SerializeField]
	private List<Transform> markers = new List<Transform>();

	[NonSerialized]
	public BeatMarkers beatMarkers = new BeatMarkers();

	[NonSerialized]
	public DecoratingScene scene;

	private EnumeratorsList enumList = new EnumeratorsList();

	private AnimationState animationState;

	public DecoratingSceneConfig.AnimationSequence lastAnimation => animationState.animationSequence;

	private float deltaTime => Time.deltaTime;

	public void Init(DecoratingScene scene)
	{
		this.scene = scene;
		for (int i = 0; i < avatars.Count; i++)
		{
			avatars[i].InitWithDecoratingScene(scene);
		}
	}

	public Transform GetMarker(string name)
	{
		for (int i = 0; i < markers.Count; i++)
		{
			Transform transform = markers[i];
			if (transform.name == name)
			{
				return transform;
			}
		}
		return null;
	}

	public void Stop()
	{
		animationState = default(AnimationState);
		StopAvatarAnimations();
	}

	public void PlayWithSetup(ChangeAnimationArguments arguments)
	{
		DecoratingSceneConfig.AnimationSequence animation = arguments.animation;
		List<VisualObjectBehaviour> visualObjectBehaviours = scene.visualObjectBehaviours;
		for (int i = 0; i < visualObjectBehaviours.Count; i++)
		{
			VisualObjectBehaviour visualObjectBehaviour = visualObjectBehaviours[i];
			visualObjectBehaviour.visualObject.isOwned = animation.testSetup.ShouldBeOwned(visualObjectBehaviour.name);
		}
		scene.roomScreen.Init();
		scene.SetCharacterAlpha(1f);
		Play(arguments);
	}

	public void HideAvatars()
	{
		for (int i = 0; i < avatars.Count; i++)
		{
			GGUtil.SetActive(avatars[i], active: false);
		}
	}

	public void Play(ChangeAnimationArguments arguments)
	{
		if (arguments.isNoAnimation)
		{
			Stop();
			return;
		}
		animationState = default(AnimationState);
		animationState.arguments = arguments;
		animationState.animationSequence = arguments.animation;
		animationState.animationEnumerator = DoPlay(animationState.arguments);
		animationState.animationEnumerator.MoveNext();
	}

	public CharacterAvatar GetAvatar(string characterName)
	{
		for (int i = 0; i < avatars.Count; i++)
		{
			CharacterAvatar characterAvatar = avatars[i];
			if (characterAvatar.name == characterName)
			{
				return characterAvatar;
			}
		}
		return null;
	}

	private void StopAvatarAnimations()
	{
		for (int i = 0; i < avatars.Count; i++)
		{
			CharacterAvatar characterAvatar = avatars[i];
			characterAvatar.StopAnimation();
			characterAvatar.StopLookAt();
		}
	}

	private IEnumerator DoPlay(ChangeAnimationArguments arguments)
	{
		return new _003CDoPlay_003Ed__20(0)
		{
			_003C_003E4__this = this,
			arguments = arguments
		};
	}

	private IEnumerator DoPlaySequence(CharacterAvatar avatar, DecoratingSceneConfig.CharacterAnimationSequence sequence)
	{
		return new _003CDoPlaySequence_003Ed__21(0)
		{
			_003C_003E4__this = this,
			avatar = avatar,
			sequence = sequence
		};
	}

	public void Update()
	{
		IEnumerator animationEnumerator = animationState.animationEnumerator;
		if (animationEnumerator != null && !animationEnumerator.MoveNext())
		{
			animationState.animationEnumerator = null;
		}
	}
}
