using UnityEngine;

public class ScreenMessagePanel : MonoBehaviour
{
	[SerializeField]
	private ScreenMessage message;

	public void Play(string text)
	{
		GGUtil.SetActive(this, active: true);
		message.Init(this, text);
		message.Play();
	}

	public void OnAnimationEnd()
	{
		GGUtil.SetActive(this, active: false);
	}
}
