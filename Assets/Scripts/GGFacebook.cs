public class GGFacebook : BehaviourSingletonInit<GGFacebook>
{
	private IFacebookProvider facebook;

	public override void Init()
	{
		facebook = new FacebookAndroidPlugin();
	}

	public void Login(FBLoginParams loginParams)
	{
		facebook.Login(loginParams);
	}

	public bool IsInitialized()
	{
		return facebook.IsInitialized();
	}
}
