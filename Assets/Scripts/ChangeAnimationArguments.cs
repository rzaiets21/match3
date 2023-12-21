using System;

public struct ChangeAnimationArguments
{
	public DecoratingSceneConfig.AnimationSequence animation;

	public Action onComplete;

	public bool showWideBars;

	public bool isNoAnimation => !isAnimationAvailable;

	public bool isAnimationAvailable => animation != null;

	public static ChangeAnimationArguments NoAnimation => default(ChangeAnimationArguments);

	public static ChangeAnimationArguments Create(string roomName, string sceneObjectName)
	{
		DecoratingSceneConfig.AnimationSequenceGroup animationSequenceGroup = ScriptableObjectSingleton<DecoratingSceneConfig>.instance.GetAnimationSequenceGroup(roomName);
		ChangeAnimationArguments result = default(ChangeAnimationArguments);
		if (animationSequenceGroup != null)
		{
			result.animation = animationSequenceGroup.SequencForOpenAnimation(sceneObjectName);
		}
		return result;
	}
}
