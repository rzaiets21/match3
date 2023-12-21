using System;

public class GGAppleSignIn : BehaviourSingletonInit<GGAppleSignIn>
{
	private IAppleSignInProvider signInProvider;

	public bool isAvailable => signInProvider.isAvailable;

	public override void Init()
	{
		signInProvider = new IAppleSignInProvider();
	}

	public void SignIn(Action<IAppleSignInProvider.SignInResponse> onComplete)
	{
		signInProvider.SignIn(onComplete);
	}
}
