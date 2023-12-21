using UnityEngine;

public class GameScreenMenu : MonoBehaviour
{
	public enum State
	{
		Open,
		Closed
	}

	[SerializeField]
	private CurrencyPrefabAnimation backgroundInAnimation;

	[SerializeField]
	private CurrencyPrefabAnimation backgroundOutAnimation;

	[SerializeField]
	private CurrencyPrefabAnimation exitButtonInAnimation;

	[SerializeField]
	private CurrencyPrefabAnimation exitButtonOutAnimation;

	[SerializeField]
	private RectTransform backgroundButton;

	private State state = State.Closed;

	public void Show()
	{
		backgroundOutAnimation.Stop();
		backgroundInAnimation.Init();
		backgroundInAnimation.Play(0f);
		exitButtonOutAnimation.Stop();
		exitButtonInAnimation.Init();
		exitButtonInAnimation.Play(0.15f);
		GGUtil.SetActive(backgroundButton, active: true);
	}

	public void OnEnable()
	{
		state = State.Closed;
		exitButtonInAnimation.Stop();
		exitButtonOutAnimation.Stop();
		backgroundOutAnimation.Stop();
		backgroundInAnimation.Stop();
		backgroundInAnimation.Init();
		exitButtonInAnimation.Init();
		GGUtil.SetActive(backgroundButton, active: false);
	}

	public void Hide()
	{
		backgroundInAnimation.Stop();
		backgroundOutAnimation.Init();
		backgroundOutAnimation.Play(0.15f);
		exitButtonInAnimation.Stop();
		exitButtonOutAnimation.Init();
		exitButtonOutAnimation.Play(0f);
		GGUtil.SetActive(backgroundButton, active: false);
	}

	public void ButtonCallback_OnMenuButtonClicked()
	{
		if (state == State.Closed)
		{
			Show();
			state = State.Open;
			GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
		}
		else
		{
			Hide();
			state = State.Closed;
			GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
		}
	}
}
