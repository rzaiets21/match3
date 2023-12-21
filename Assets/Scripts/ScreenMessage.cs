using UnityEngine;
using UnityEngine.UI;

public class ScreenMessage : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Text text;

	private ScreenMessagePanel panel;

	public void Init(ScreenMessagePanel panel, string text)
	{
		this.panel = panel;
		this.text.text = text;
	}

	public void Play()
	{
		animator.SetTrigger("Play");
	}

	public void OnAnimationEnd()
	{
		panel.OnAnimationEnd();
	}
}
