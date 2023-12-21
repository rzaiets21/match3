using System.Collections.Generic;
using UnityEngine;

public class ToggleSoundsButton : MonoBehaviour
{
	public enum SoundType
	{
		Music,
		SoundFx
	}

	[SerializeField]
	private SoundType soundType;

	[SerializeField]
	private List<Transform> widgetsToHide = new List<Transform>();

	[SerializeField]
	private VisualStyleSet onStyle = new VisualStyleSet();

	[SerializeField]
	private VisualStyleSet offStyle = new VisualStyleSet();

	private bool isOff
	{
		get
		{
			GGPlayerSettings instance = GGPlayerSettings.instance;
			if (soundType != 0)
			{
				return instance.isSoundFXOff;
			}
			return instance.isMusicOff;
		}
		set
		{
			GGPlayerSettings instance = GGPlayerSettings.instance;
			if (soundType == SoundType.Music)
			{
				instance.isMusicOff = value;
			}
			else
			{
				instance.isSoundFXOff = value;
			}
		}
	}

	private void OnEnable()
	{
		UpdateVisualState();
	}

	public void ButtonCallback_OnClick()
	{
		isOff = !isOff;
		UpdateVisualState();
		GGSoundSystem.Play(GGSoundSystem.SFXType.ButtonPress);
	}

	private void UpdateVisualState()
	{
		bool num = !isOff;
		GGUtil.Hide(widgetsToHide);
		if (num)
		{
			onStyle.Apply();
		}
		else
		{
			offStyle.Apply();
		}
	}
}
