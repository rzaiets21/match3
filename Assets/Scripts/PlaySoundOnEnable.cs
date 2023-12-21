using UnityEngine;

public class PlaySoundOnEnable : MonoBehaviour
{
	[SerializeField]
	private GGSoundSystem.SFXType soundType;

	private void OnEnable()
	{
		GGSoundSystem.Play(soundType);
	}
}
