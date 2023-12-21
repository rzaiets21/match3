using GGMatch3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollableSelectRoomScreenButton : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI titleLabel;

	[SerializeField]
	private TextMeshProUGUI descriptionLabel;

	[SerializeField]
	private List<RectTransform> widgetsToHide = new List<RectTransform>();

	[SerializeField]
	private RectTransform lockedStyle;

	[SerializeField]
	private RectTransform completeStyle;

	[SerializeField]
	private RectTransform unlockAnimation;

	[SerializeField]
	private RectTransform passAnimation;

	[SerializeField]
	public float unlockAnimationDuration;

	[SerializeField]
	public float passAnimationDuration;

	[SerializeField]
	private Image mainImage;

	[NonSerialized]
	public RoomsDB.Room room;

	public void Init(RoomsDB.Room room)
	{
		this.room = room;
		GGUtil.SetActive(widgetsToHide, active: false);
		if (room.isPassed)
		{
			GGUtil.Show(completeStyle);
		}
		else if (room.isLocked)
		{
			GGUtil.Show(lockedStyle);
		}
		GGUtil.ChangeText(titleLabel, room.displayName);
		GGUtil.ChangeText(descriptionLabel, room.description);
		GGUtil.SetSprite(mainImage, room.cardSprite);
	}

	public void ShowPassedAnimation()
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		GGUtil.Show(passAnimation);
	}

	public void ShowUnlockAnimation()
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		GGUtil.Show(unlockAnimation);
	}

	public void ShowOpenNotPassed()
	{
		GGUtil.SetActive(widgetsToHide, active: false);
	}

	public void ShowLocked()
	{
		GGUtil.SetActive(widgetsToHide, active: false);
		GGUtil.Show(lockedStyle);
	}

	public void Callback_OnClick()
	{
		UnityEngine.Debug.Log("OPEN ROOM " + room.name);
		if (!ConfigBase.instance.debug && !room.isOpen)
		{
			GGSoundSystem.Play(GGSoundSystem.SFXType.CancelPress);
			return;
		}
		GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
		int num2 = SingletonInit<RoomsBackend>.instance.selectedRoomIndex = ScriptableObjectSingleton<RoomsDB>.instance.IndexOf(room);
		NavigationManager.instance.Pop();
	}
}
