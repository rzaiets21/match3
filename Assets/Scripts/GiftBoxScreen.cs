using GGMatch3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class GiftBoxScreen : MonoBehaviour
{
	public enum GiftType
	{
		Coins,
		Powerup,
		Booster,
		Energy,
		Stars
	}

	public struct ShowArguments
	{
		public string title;

		public Action onComplete;

		public GiftsDefinition giftsDefinition;
	}

	[Serializable]
	public class Gift
	{
		public GiftType giftType;

		public PowerupType powerupType;

		public BoosterType boosterType;

		public int amount;

		public int hours;

		public TimeSpan duration => TimeSpan.FromHours(hours);

		public static Gift CreateCoins(int amount)
		{
			return new Gift
			{
				giftType = GiftType.Coins,
				amount = amount
			};
		}

		public static Gift CreatePowerup(PowerupType powerupType, int amount)
		{
			return new Gift
			{
				giftType = GiftType.Powerup,
				powerupType = powerupType,
				amount = amount
			};
		}

		public static Gift CreateBooster(BoosterType boosterType, int amount)
		{
			return new Gift
			{
				giftType = GiftType.Booster,
				boosterType = boosterType,
				amount = amount
			};
		}

		public static Gift CreateStars(int amount)
		{
			return new Gift
			{
				giftType = GiftType.Stars,
				amount = amount
			};
		}

		public static Gift CreateEnergy(int hours)
		{
			return new Gift
			{
				giftType = GiftType.Energy,
				hours = hours
			};
		}

		public void ConsumeGift()
		{
			GGPlayerSettings instance = GGPlayerSettings.instance;
			if (giftType == GiftType.Coins)
			{
				instance.walletManager.AddCurrency(CurrencyType.coins, amount);
			}
			else if (giftType == GiftType.Booster)
			{
				PlayerInventory.instance.Add(boosterType, amount);
			}
			else if (giftType == GiftType.Powerup)
			{
				PlayerInventory.instance.Add(powerupType, amount);
			}
			else if (giftType == GiftType.Energy)
			{
				PlayerInventory.instance.BuyTimedItem(PlayerInventory.Item.FreeEnergyLimited, duration);
			}
			else if (giftType == GiftType.Stars)
			{
				instance.walletManager.AddCurrency(CurrencyType.diamonds, amount);
			}
		}
	}

	[Serializable]
	public class GiftsDefinition
	{
		public List<Gift> gifts = new List<Gift>();

		public void ConsumeGift()
		{
			for (int i = 0; i < gifts.Count; i++)
			{
				gifts[i].ConsumeGift();
			}
		}

		public void Add(Gift gift)
		{
			gifts.Add(gift);
		}
	}

	private sealed class _003CInAnimation_003Ed__19 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public GiftBoxScreen _003C_003E4__this;

		private IEnumerator _003CwaitingEnum_003E5__2;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CInAnimation_003Ed__19(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			GiftBoxScreen giftBoxScreen = _003C_003E4__this;
			AnimatorStateInfo currentAnimatorStateInfo;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				GGUtil.Show(giftBoxScreen.giftBoxContainer);
				GGSoundSystem.Play(GGSoundSystem.SFXType.GiftPresented);
				goto IL_003e;
			case 1:
				_003C_003E1__state = -1;
				goto IL_003e;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00ab;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_003e:
				currentAnimatorStateInfo = giftBoxScreen.giftBoxAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName(giftBoxScreen.giftBoxInAnimationName) && !(currentAnimatorStateInfo.normalizedTime >= 1f))
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003CwaitingEnum_003E5__2 = giftBoxScreen.waitForTap.DoWaitForTap();
				goto IL_00ab;
				IL_00ab:
				if (_003CwaitingEnum_003E5__2.MoveNext())
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				}
				GGUtil.Hide(giftBoxScreen.giftBoxContainer);
				// if(PlayerPrefs.GetInt("FirstGift", 0) == 1)
				// 	AdsManager.ShowInterstitial();
				GGUtil.Show(giftBoxScreen.openContainer);
				GGSoundSystem.Play(GGSoundSystem.SFXType.GiftOpen);
				_003CwaitingEnum_003E5__2 = giftBoxScreen.waitForTap.DoWaitForTap();
				// PlayerPrefs.SetInt("FirstGift", 1);
				break;
			}
			if (_003CwaitingEnum_003E5__2.MoveNext())
			{
				_003C_003E2__current = null;
				_003C_003E1__state = 3;
				return true;
			}
			GGSoundSystem.Play(GGSoundSystem.SFXType.FlyIn);
			if (giftBoxScreen.showArguments.onComplete != null)
			{
				giftBoxScreen.showArguments.onComplete();
			}
			else
			{
				NavigationManager.instance.Pop();
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private ShowArguments showArguments;

	[SerializeField]
	private List<Transform> widgetsToHide = new List<Transform>();

	[SerializeField]
	private RectTransform giftBoxContainer;

	[SerializeField]
	private Animator giftBoxAnimator;

	[SerializeField]
	private UIWaitForTap waitForTap;

	[SerializeField]
	private Transform openContainer;

	[SerializeField]
	private ComponentPool giftItemPool;

	[SerializeField]
	private TextMeshProUGUI titleText;

	[SerializeField]
	private float padding = 10f;

	private IEnumerator animation;

	private GiftsDefinition giftsDefinition;

	public string giftBoxInAnimationName;

	public void Show(ShowArguments showArguments)
	{
		this.showArguments = showArguments;
		giftsDefinition = showArguments.giftsDefinition;
		NavigationManager.instance.Push(base.gameObject);
	}

	private void OnEnable()
	{
		giftsDefinition = showArguments.giftsDefinition;
		// giftsDefinition = null;
		if (giftsDefinition == null)
		{
			giftsDefinition = new GiftsDefinition();
			giftsDefinition.Add(Gift.CreateCoins(1000));
			// giftsDefinition.Add(Gift.CreatePowerup(PowerupType.Hammer, 2));
			// giftsDefinition.Add(Gift.CreatePowerup(PowerupType.PowerHammer, 3));
			// giftsDefinition.Add(Gift.CreateBooster(BoosterType.BombBooster, 4));
			// giftsDefinition.Add(Gift.CreateBooster(BoosterType.DiscoBooster, 5));
			// giftsDefinition.Add(Gift.CreateBooster(BoosterType.VerticalRocketBooster, 6));
			// giftsDefinition.Add(Gift.CreateStars(2));
			// giftsDefinition.Add(Gift.CreateEnergy(20));
		}
		giftsDefinition.ConsumeGift();
		showArguments.giftsDefinition = giftsDefinition;
		Init(showArguments);
	}

	private void Init(ShowArguments showArguments)
	{
		GiftsDefinition giftsDefinition = showArguments.giftsDefinition;
		GGUtil.ChangeText(titleText, showArguments.title);
		GGUtil.Hide(widgetsToHide);
		waitForTap.Hide();
		giftItemPool.Clear();
		Vector2 sizeDelta = giftItemPool.prefab.GetComponent<RectTransform>().sizeDelta;
		List<Gift> gifts = giftsDefinition.gifts;
		float x = sizeDelta.x;
		float num = (0f - ((float)gifts.Count * x + (float)(gifts.Count - 1) * padding)) * 0.5f;
		for (int i = 0; i < gifts.Count; i++)
		{
			Gift gift = gifts[i];
			GiftBoxScreenGiftItem giftBoxScreenGiftItem = giftItemPool.Next<GiftBoxScreenGiftItem>();
			giftBoxScreenGiftItem.Init(gift);
			float num2 = x * 0.5f;
			float x2 = num + (float)i * (x + padding) + num2;
			giftBoxScreenGiftItem.transform.localPosition = new Vector3(x2, 0f, 0f);
		}
		giftItemPool.HideNotUsed();
		animation = InAnimation();
		animation.MoveNext();
	}

	private IEnumerator InAnimation()
	{
		return new _003CInAnimation_003Ed__19(0)
		{
			_003C_003E4__this = this
		};
	}

	private void Update()
	{
		if (animation != null)
		{
			animation.MoveNext();
		}
	}
}
