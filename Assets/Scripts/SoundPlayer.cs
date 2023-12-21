using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
	[SerializeField]
	private GGSoundSystem.SFXType soundType;

	public void ButtonCallback_OnClick()
	{
		GGSoundSystem.Play(soundType);
	}
}
