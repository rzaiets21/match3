using System;

[Serializable]
public class FBLoginResponse
{
	public string error;

	public string user_id;

	public bool isError => GGUtil.HasText(error);
}
