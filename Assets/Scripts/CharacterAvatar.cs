using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CharacterAvatar : MonoBehaviour
{
	public struct ChangeAnimationArguments
	{
		public DecoratingSceneConfig.CharacterAnimation animation;

		public bool useFadeInOut;

		public float initialDelay;

		public RoomSceneRenderObject roomRenderer;

		public float additionalTime;

		public Action onComplete;

		public VisualObjectBehaviour lookAt;

		public static ChangeAnimationArguments Create(string roomName, string sceneObjectName, int animationVariantIndex = 0)
		{
			DecoratingSceneConfig.AnimationsList animationsForSceneObject = ScriptableObjectSingleton<DecoratingSceneConfig>.instance.GetAnimationsForSceneObject(roomName, sceneObjectName);
			ChangeAnimationArguments result = default(ChangeAnimationArguments);
			if (animationsForSceneObject != null && animationsForSceneObject.animations.Count > animationVariantIndex)
			{
				result.animation = animationsForSceneObject.animations[animationVariantIndex];
			}
			return result;
		}
	}

	private struct LookAtState
	{
		public bool isActive;

		public Vector3 lookAtPosition;

		public float duration;

		public Vector3 startLookAtPosition;

		public float weightAtStart;

		public float weight;

		public float time;
	}

	private struct AnimationState
	{
		public bool isActive;

		public DecoratingSceneConfig.CharacterAnimation characterAnimation;

		public float additionalTime;

		public ChangeAnimationArguments changeAnimationArguments;

		public void Complete()
		{
			isActive = false;
			if (changeAnimationArguments.onComplete != null)
			{
				changeAnimationArguments.onComplete();
			}
		}
	}

	private sealed class _003CDoPopIn_003Ed__52 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public CharacterAvatar _003C_003E4__this;

		private float _003Ctime_003E5__2;

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
		public _003CDoPopIn_003Ed__52(int _003C_003E1__state)
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
			CharacterAvatar characterAvatar = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Ctime_003E5__2 = 0f;
				break;
			case 1:
				_003C_003E1__state = -1;
				break;
			}
			if (_003Ctime_003E5__2 <= characterAvatar.popInDuration)
			{
				_003Ctime_003E5__2 += Time.deltaTime;
				float num2 = Mathf.InverseLerp(0f, characterAvatar.popInDuration, _003Ctime_003E5__2);
				if (characterAvatar.popInCurve != null)
				{
					num2 = characterAvatar.popInCurve.Evaluate(num2);
				}
				Vector3 localScale = Vector3.LerpUnclamped(characterAvatar.startScale, characterAvatar.endScale, num2);
				characterAvatar.transform.localScale = localScale;
				_003C_003E2__current = null;
				_003C_003E1__state = 1;
				return true;
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

	[SerializeField]
	private float popInDuration;

	[SerializeField]
	private AnimationCurve popInCurve;

	[SerializeField]
	private Vector3 startScale;

	[SerializeField]
	private Vector3 endScale;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Transform shadowTransform;

	[SerializeField]
	private float shadowy = 0.001f;

	[SerializeField]
	private string idleState;

	[SerializeField]
	public string roomName;

	[SerializeField]
	public string sceneObjectName;

	[SerializeField]
	public int animationIndex;

	[SerializeField]
	private float lookAtChangeSpeed = 1f;

	[SerializeField]
	public Transform headLookAt;

	[SerializeField]
	private float maxHeadAngleDeg = 30f;

	[SerializeField]
	private float maxHorizontalHeadAngleDeg = 30f;

	[SerializeField]
	public float lookAtWeight;

	[NonSerialized]
	public DecoratingScene decoratingScene;

	private AnimatorControllerParameter[] allParameters_;

	[NonSerialized]
	public float timeNotAnimating = 10000f;

	private LookAtState lookAtState;

	private AnimationState animationState;

	private IEnumerator animationEnum;

	[NonSerialized]
	public DecoratingSceneConfig.CharacterAnimation lastAnimation;

	private AnimatorControllerParameter[] allParameters
	{
		get
		{
			if (allParameters_ == null)
			{
				allParameters_ = animator.parameters;
			}
			return allParameters_;
		}
	}

	public bool isRunningDefaultAnimation
	{
		get
		{
			if (lastAnimation == null)
			{
				return false;
			}
			if (lastAnimation.animationList == null)
			{
				return false;
			}
			return lastAnimation.animationList.isDefaultAnimation;
		}
	}

	public bool isAnimating => animationState.isActive;

	public Vector3 headPosition => animator.GetBoneTransform(HumanBodyBones.Head).position;

	public Vector3 bubblePosition => animator.GetBoneTransform(HumanBodyBones.Head).position + ScriptableObjectSingleton<DecoratingSceneConfig>.instance.speachBubbleHeadOffset;

	private CharacterSpeachBubble speachBubble
	{
		get
		{
			if (decoratingScene == null)
			{
				return null;
			}
			return decoratingScene.roomScreen.GetSpeachBubble(this);
		}
	}

	public void InitWithDecoratingScene(DecoratingScene decoratingScene)
	{
		this.decoratingScene = decoratingScene;
	}

	public void ShowSpeachBubble(string text)
	{
		CharacterSpeachBubble speachBubble = this.speachBubble;
		if (!(speachBubble == null))
		{
			speachBubble.SetActive(active: true);
			speachBubble.SetText(text);
		}
	}

	public void HideSpeachBubble()
	{
		if (!(speachBubble == null))
		{
			speachBubble.SetActive(active: false);
		}
	}

	public void StopLookAt()
	{
		lookAtWeight = 0f;
	}

	public void UpdateLookAtInAnimation(Vector3 position)
	{
		Vector3 acceptableHeadLookAtPosition = GetAcceptableHeadLookAtPosition(position);
		lookAtState.lookAtPosition = acceptableHeadLookAtPosition;
	}

	public void ChangeLookAtWeight(float weight, float animateDuration)
	{
		if (!(headLookAt == null))
		{
			lookAtState = default(LookAtState);
			Vector3 position = headLookAt.position;
			headLookAt.position = position;
			lookAtState.startLookAtPosition = position;
			lookAtState.duration = animateDuration;
			lookAtState.weightAtStart = lookAtWeight;
			lookAtState.weight = weight;
			lookAtState.lookAtPosition = position;
			lookAtState.isActive = true;
		}
	}

	public void LookAt(Vector3 position, float weight, float animateDuration)
	{
		if (!(headLookAt == null))
		{
			lookAtState = default(LookAtState);
			Vector3 acceptableHeadLookAtPosition = GetAcceptableHeadLookAtPosition(position);
			Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.Head);
			float d = Vector3.Distance(acceptableHeadLookAtPosition, boneTransform.position);
			Vector3 vector3 = boneTransform.position + boneTransform.forward * d;
			Vector3 vector = acceptableHeadLookAtPosition;
			Vector3 vector2 = Vector3.Lerp(vector, headLookAt.position, lookAtWeight);
			vector2 = vector;
			lookAtState.startLookAtPosition = vector2;
			if (lookAtWeight <= 0f)
			{
				lookAtState.startLookAtPosition = acceptableHeadLookAtPosition;
				headLookAt.position = acceptableHeadLookAtPosition;
			}
			else
			{
				lookAtState.startLookAtPosition = headLookAt.position;
			}
			lookAtState.duration = animateDuration;
			lookAtState.weightAtStart = lookAtWeight;
			lookAtState.weight = weight;
			lookAtState.lookAtPosition = acceptableHeadLookAtPosition;
			lookAtState.isActive = true;
			if (animateDuration <= 0f)
			{
				lookAtWeight = lookAtState.weight;
				headLookAt.position = lookAtState.lookAtPosition;
				lookAtState.isActive = false;
			}
		}
	}

	public void StopAnimation()
	{
		lastAnimation = null;
		animationState = default(AnimationState);
		HideSpeachBubble();
		AnimatorControllerParameter[] allParameters = this.allParameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in allParameters)
		{
			animator.SetBool(animatorControllerParameter.name, value: false);
		}
		animator.Play(idleState, 0);
	}

	public void RunAnimation(ChangeAnimationArguments animationArguments)
	{
		DecoratingSceneConfig.CharacterAnimation animation = animationArguments.animation;
		timeNotAnimating = 0f;
		StopAnimation();
		if (animation == null)
		{
			if (animationArguments.onComplete != null)
			{
				animationArguments.onComplete();
			}
			return;
		}
		lastAnimation = animation;
		animationState.isActive = true;
		animationState.additionalTime = animationArguments.additionalTime;
		animationState.characterAnimation = animation;
		animationState.changeAnimationArguments = animationArguments;
		animationState.characterAnimation.StartAnimation(this);
	}

	public void RunAnimation(string sceneObjectName, float additionalTime)
	{
		ChangeAnimationArguments animationArguments = ChangeAnimationArguments.Create(roomName, sceneObjectName);
		animationArguments.additionalTime = additionalTime;
		RunAnimation(animationArguments);
	}

	public void SetProperty(string name, bool value)
	{
		animator.SetBool(name, value);
	}

	public void SetPosition(Vector3 position)
	{
		base.transform.position = position;
	}

	public void SetRotation(Vector3 eulerRotation)
	{
		base.transform.rotation = Quaternion.Euler(eulerRotation);
	}

	public void PopIn()
	{
		animationEnum = DoPopIn();
	}

	private IEnumerator DoPopIn()
	{
		return new _003CDoPopIn_003Ed__52(0)
		{
			_003C_003E4__this = this
		};
	}

	private void LateUpdate()
	{
		if (shadowTransform != null)
		{
			Vector3 position = shadowTransform.position;
			position.y = shadowy;
			shadowTransform.position = position;
		}
	}

	private void Update()
	{
		if (animationEnum != null && !animationEnum.MoveNext())
		{
			animationEnum = null;
		}
		if (lookAtState.isActive)
		{
			lookAtState.time += Time.deltaTime;
			float num = Mathf.InverseLerp(0f, lookAtState.duration, lookAtState.time);
			AnimationCurve headAnimationCurve = ScriptableObjectSingleton<DecoratingSceneConfig>.instance.headAnimationCurve;
			if (headAnimationCurve != null)
			{
				num = headAnimationCurve.Evaluate(num);
			}
			Vector3 position = Vector3.Lerp(lookAtState.startLookAtPosition, lookAtState.lookAtPosition, num);
			float num2 = lookAtWeight = Mathf.Lerp(lookAtState.weightAtStart, lookAtState.weight, num);
			headLookAt.position = position;
			if (lookAtState.time >= lookAtState.duration)
			{
				lookAtState.isActive = false;
			}
		}
		if (!animationState.isActive)
		{
			timeNotAnimating += Time.deltaTime;
			return;
		}
		DecoratingSceneConfig.CharacterAnimation characterAnimation = animationState.characterAnimation;
		if (!characterAnimation.isAnimationActive)
		{
			animationState.additionalTime -= Time.deltaTime;
			if (animationState.additionalTime <= 0f)
			{
				animationState.Complete();
			}
		}
		characterAnimation.UpdateAnimation(Time.deltaTime, this);
		if (!characterAnimation.isAnimationActive && animationState.additionalTime <= 0f)
		{
			animationState.Complete();
		}
	}

	private Vector3 GetAcceptableHeadLookAtPosition(Vector3 initialPosition)
	{
		Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.Head);
		if (boneTransform == null)
		{
			return initialPosition;
		}
		Vector3 vector = initialPosition;
		Vector3 rhs = vector - boneTransform.position;
		Vector3 normalized = Vector3Ex.OnGround(base.transform.forward).normalized;
		Vector3 vector2 = Vector3.Cross(normalized, Vector3.up);
		float num = Mathf.Abs(Vector3.Dot(normalized, rhs));
		float f = Vector3.Dot(vector2, rhs);
		float num2 = Mathf.Tan((float)Math.PI / 180f * maxHorizontalHeadAngleDeg) * num;
		if (Mathf.Abs(f) > num2)
		{
			Vector3 vector3 = boneTransform.position + num * normalized + Mathf.Sign(f) * num2 * vector2;
			vector3.y = vector.y;
			vector = vector3;
		}
		rhs = vector - boneTransform.position;
		float magnitude = Vector3Ex.OnGround(rhs).magnitude;
		float y = rhs.y;
		float num3 = Mathf.Tan((float)Math.PI / 180f * maxHeadAngleDeg) * magnitude;
		if (Mathf.Abs(y) > num3)
		{
			vector.y = boneTransform.position.y + Mathf.Sign(y) * num3;
		}
		return vector;
	}

	private void OnAnimatorIK(int layerIndex)
	{
		if (!(animator == null) && !(headLookAt == null))
		{
			animator.SetLookAtWeight(lookAtWeight);
			if (!(lookAtWeight <= 0f))
			{
				Vector3 position = headLookAt.position;
				animator.SetLookAtPosition(position);
			}
		}
	}
}
