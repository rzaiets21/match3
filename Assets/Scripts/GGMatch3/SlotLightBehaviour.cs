using UnityEngine;

namespace GGMatch3
{
	public class SlotLightBehaviour : SlotComponentBehavoiour
	{
		[SerializeField]
		private SpriteRenderer lightSprite;

		[SerializeField]
		private SpriteRenderer slotBckSprite;

		public void InitWithSlotComponent(LightSlotComponent slotComponent)
		{
			SetActive(lightSprite, active: false);
			slotComponent.Init(this);
			SetActive(this, active: true);
		}

		public void Init(Slot slot, bool isBackPatternEnabled)
		{
			LightSlotComponent lightSlotComponent = new LightSlotComponent();
			SetActive(lightSprite, active: false);
			lightSlotComponent.Init(this);
			slot.AddComponent(lightSlotComponent);
			SetActive(this, active: true);
			SetActive(slotBckSprite, isBackPatternEnabled);
		}

		public static void SetActive(SpriteRenderer beh, bool active)
		{
			if (!(beh == null))
			{
				GameObject gameObject = beh.gameObject;
				if (gameObject.activeSelf != active)
				{
					gameObject.SetActive(active);
				}
			}
		}

		public static void SetActive(MonoBehaviour beh, bool active)
		{
			if (!(beh == null))
			{
				GameObject gameObject = beh.gameObject;
				if (gameObject.activeSelf != active)
				{
					gameObject.SetActive(active);
				}
			}
		}

		public void SetLight(float intensity)
		{
			SetActive(lightSprite, intensity > 0f);
			Color color = lightSprite.color;
			color.a = intensity;
			lightSprite.color = color;
		}
	}
}
