using GGMatch3;
using UnityEngine;

public class DailyGiftsScreen : MonoBehaviour
{
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public NavigationManager nav;

		internal void _003CButtonCallback_OnBackgroundClick_003Eb__0()
		{
			nav.Pop();
		}
	}

	[SerializeField]
	private ComponentPool smallGiftPanels = new ComponentPool();

	[SerializeField]
	private ComponentPool bigGiftPanels = new ComponentPool();

	[SerializeField]
	private Transform scalingTransform;

	[SerializeField]
	private float screenSizePercent = 0.9f;

	[SerializeField]
	private int padding = 1;

	private void OnEnable()
	{
		Init();
	}

	private void Init()
	{
		Vector2 prefabSizeDelta = smallGiftPanels.prefabSizeDelta;
		Vector2 prefabSizeDelta2 = bigGiftPanels.prefabSizeDelta;
		float num = prefabSizeDelta.x * 3f + prefabSizeDelta2.x + (float)(3 * padding);
		float y = prefabSizeDelta2.y;
		smallGiftPanels.Clear();
		bigGiftPanels.Clear();
		GiftsDefinitionDB.DailyGifts dailyGifts = ScriptableObjectSingleton<GiftsDefinitionDB>.instance.dailyGifts;
		RectTransform component = GetComponent<RectTransform>();
		Vector2 vector = new Vector2(component.rect.width, component.rect.height) * screenSizePercent;
		float num2 = vector.x / num;
		float num3 = vector.y / y;
		float d = Mathf.Min(num2, num3, 1f);
		scalingTransform.localScale = Vector3.one * d;
		Vector2 a = new Vector2((0f - num) * 0.5f, (0f - y) * 0.5f);
		for (int i = 0; i < 6; i++)
		{
			ComponentPool componentPool = smallGiftPanels;
			int num4 = i / 3;
			int num5 = i % 3;
			DailyGiftsScreenGiftCard dailyGiftsScreenGiftCard = componentPool.Next<DailyGiftsScreenGiftCard>();
			dailyGiftsScreenGiftCard.Init(i, 1f, dailyGifts.IsSelected(i));
			GGUtil.Show(dailyGiftsScreenGiftCard);
			Vector2 b = new Vector2(((float)num5 + 0.5f) * prefabSizeDelta.x + (float)((num5 - 1) * padding), ((float)(-num4) + 0.5f) * prefabSizeDelta.y - (float)((num4 - 1) * padding));
			dailyGiftsScreenGiftCard.transform.localPosition = a + Vector2.up * y * 0.5f + b;
		}
		ComponentPool componentPool2 = bigGiftPanels;
		int num6 = 3;
		int num7 = 6;
		DailyGiftsScreenGiftCard dailyGiftsScreenGiftCard2 = componentPool2.Next<DailyGiftsScreenGiftCard>();
		dailyGiftsScreenGiftCard2.Init(num7, 1f, dailyGifts.IsSelected(num7));
		GGUtil.Show(dailyGiftsScreenGiftCard2);
		Vector2 a2 = new Vector2((float)num6 * prefabSizeDelta.x + (float)((num6 - 1) * padding), 0f);
		a2 += prefabSizeDelta2 * 0.5f;
		dailyGiftsScreenGiftCard2.transform.localPosition = a + a2;
		smallGiftPanels.HideNotUsed();
		bigGiftPanels.HideNotUsed();
	}

	public void ButtonCallback_OnBackgroundClick()
	{
		_003C_003Ec__DisplayClass7_0 _003C_003Ec__DisplayClass7_ = new _003C_003Ec__DisplayClass7_0();
		_003C_003Ec__DisplayClass7_.nav = NavigationManager.instance;
		GiftBoxScreen @object = _003C_003Ec__DisplayClass7_.nav.GetObject<GiftBoxScreen>();
		GiftsDefinitionDB.DailyGifts dailyGifts = ScriptableObjectSingleton<GiftsDefinitionDB>.instance.dailyGifts;
		GiftsDefinitionDB.DailyGifts.DailyGift currentDailyGift = dailyGifts.currentDailyGift;
		if (currentDailyGift == null)
		{
			_003C_003Ec__DisplayClass7_.nav.Pop();
			return;
		}
		_003C_003Ec__DisplayClass7_.nav.Pop(activateNextScreen: false);
		GiftBoxScreen.ShowArguments showArguments = default(GiftBoxScreen.ShowArguments);
		showArguments.giftsDefinition = currentDailyGift.gifts;
		showArguments.title = $"Day {currentDailyGift.index + 1} gift";
		showArguments.onComplete = _003C_003Ec__DisplayClass7_._003CButtonCallback_OnBackgroundClick_003Eb__0;
		@object.Show(showArguments);
		dailyGifts.OnClaimedDailyCoins();
	}
}
