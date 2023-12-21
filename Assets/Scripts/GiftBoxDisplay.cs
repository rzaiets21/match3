using GGMatch3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxDisplay : MonoBehaviour
{
	[SerializeField]
	private Transform mainContainer;

	[SerializeField]
	private TextMeshProUGUI countLabel;

	[SerializeField]
	private Image fillSprite;

	[SerializeField]
	private Transform buttonContainer;

	private void Init()
	{
		GiftsDefinitionDB.GiftDefinition currentGift = ScriptableObjectSingleton<GiftsDefinitionDB>.instance.currentGift;
		if (currentGift == null)
		{
			GGUtil.Hide(mainContainer);
			return;
		}
		GGUtil.Show(mainContainer);
		GGUtil.SetFill(fillSprite, currentGift.progress);
		GiftsDefinitionDB.GiftDefinition.StagesPassedDescriptor stagesPassedDescriptor = currentGift.stagesPassedDescriptor;
		GGUtil.ChangeText(countLabel, $"{stagesPassedDescriptor.currentStagesPassed}/{stagesPassedDescriptor.stagesNeededToPass}");
		GGUtil.SetActive(buttonContainer, currentGift.isAvailableToCollect);
	}

	public void ButtonCallback_OnClick()
	{
		GiftsDefinitionDB.GiftDefinition currentGift = ScriptableObjectSingleton<GiftsDefinitionDB>.instance.currentGift;
		if (currentGift == null)
		{
			Init();
			return;
		}
		if (!currentGift.isAvailableToCollect)
		{
			Init();
			return;
		}
		GiftBoxScreen @object = NavigationManager.instance.GetObject<GiftBoxScreen>();
		GiftBoxScreen.ShowArguments showArguments = new GiftBoxScreen.ShowArguments
		{
			giftsDefinition = currentGift.gifts,
			title = "Gift Box"
		};
		currentGift.ClaimGifts();
		@object.Show(showArguments);
	}

	private void OnEnable()
	{
		Init();
	}
}
