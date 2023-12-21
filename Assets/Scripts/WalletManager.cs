using ProtoModels;
using UnityEngine;

public class WalletManager
{
	public delegate void OnSave();

	private OnSave onSave;

	private PlayerModel model;

	public WalletManager(PlayerModel model, OnSave onSave)
	{
		this.model = model;
		this.onSave = onSave;
	}

	public void ReloadModel(PlayerModel model)
	{
		this.model = model;
	}

	private void DoOnSave()
	{
		if (onSave != null)
		{
			onSave();
		}
	}

	public void BuyItem(SingleCurrencyPrice price)
	{
		BuyItem(price.cost, price.currency);
	}

	public void BuyItem(int price, CurrencyType currencyType)
	{
		switch (currencyType)
		{
		case CurrencyType.coins:
			BuyItemCoins(price);
			break;
		case CurrencyType.diamonds:
			BuyItemDiamonds(price);
			break;
		default:
			BuyItemDollars(price);
			break;
		}
	}

	public void BuyItemDiamonds(int price)
	{
		SecureLong s = new SecureLong(model.secDiamonds, model.diamonds);
		s = MathEx.Max(0L, s - price);
		model.secDiamonds = s.ToModel();
		model.diamonds = Mathf.Max(0, model.diamonds - price);
		DoOnSave();
	}

	public void BuyItemCoins(int price)
	{
		SecureLong s = new SecureLong(model.secCoins, model.coins);
		s = MathEx.Max(0L, s - price);
		model.secCoins = s.ToModel();
		model.coins = Mathf.Max(0, model.coins - price);
		DoOnSave();
	}

	public void BuyItemDollars(int price)
	{
		SecureLong s = new SecureLong(model.secGiraffeDollars, model.giraffeDollars);
		s = MathEx.Max(0L, s - price);
		model.secGiraffeDollars = s.ToModel();
		model.giraffeDollars = Mathf.Max(0, model.giraffeDollars - price);
		DoOnSave();
	}

	public bool CurrencyHasMax(CurrencyType type)
	{
		return true;
	}

	public int MaxCurrencyCount(CurrencyType type)
	{
		switch (type)
		{
		case CurrencyType.diamonds:
			return ConfigBase.instance.tokensCap;
		case CurrencyType.coins:
			return ConfigBase.instance.coinsCap;
		case CurrencyType.ggdollars:
			return ConfigBase.instance.ggDollarsCap;
		default:
			return 0;
		}
	}

	public void AddCurrency(CurrencyType type, int ammount)
	{
		switch (type)
		{
		case CurrencyType.coins:
		{
			SecureLong s2 = new SecureLong(model.secCoins, model.coins);
			model.coins += ammount;
			s2 += ammount;
			if (CurrencyHasMax(type))
			{
				model.coins = Mathf.Min(MaxCurrencyCount(type), model.coins);
				s2 = MathEx.Min(MaxCurrencyCount(type), s2.valueLong);
			}
			model.secCoins = s2.ToModel();
			break;
		}
		case CurrencyType.diamonds:
		{
			SecureLong s3 = new SecureLong(model.secDiamonds, model.diamonds);
			model.diamonds += ammount;
			s3 += ammount;
			if (CurrencyHasMax(type))
			{
				model.diamonds = Mathf.Min(MaxCurrencyCount(type), model.diamonds);
				s3 = MathEx.Min(MaxCurrencyCount(type), s3.valueLong);
			}
			model.secDiamonds = s3.ToModel();
			break;
		}
		case CurrencyType.ggdollars:
		{
			SecureLong s = new SecureLong(model.secGiraffeDollars, model.giraffeDollars);
			model.giraffeDollars += ammount;
			s += ammount;
			if (CurrencyHasMax(type))
			{
				model.giraffeDollars = Mathf.Min(MaxCurrencyCount(type), model.giraffeDollars);
				s = MathEx.Min(MaxCurrencyCount(type), s.valueLong);
			}
			model.secGiraffeDollars = s.ToModel();
			break;
		}
		}
		DoOnSave();
	}

	public void SetCurrency(CurrencyType type, int ammount)
	{
		switch (type)
		{
		case CurrencyType.coins:
		{
			SecureLong secureLong2 = new SecureLong(model.secCoins, model.coins);
			model.coins = ammount;
			secureLong2 = ammount;
			if (CurrencyHasMax(type))
			{
				model.coins = Mathf.Min(MaxCurrencyCount(type), model.coins);
				secureLong2 = MathEx.Min(MaxCurrencyCount(type), secureLong2.valueLong);
			}
			model.secCoins = secureLong2.ToModel();
			break;
		}
		case CurrencyType.diamonds:
		{
			SecureLong secureLong3 = new SecureLong(model.secDiamonds, model.diamonds);
			model.diamonds = ammount;
			secureLong3 = ammount;
			if (CurrencyHasMax(type))
			{
				model.diamonds = Mathf.Min(MaxCurrencyCount(type), model.diamonds);
				secureLong3 = MathEx.Min(MaxCurrencyCount(type), secureLong3.valueLong);
			}
			model.secDiamonds = secureLong3.ToModel();
			break;
		}
		case CurrencyType.ggdollars:
		{
			SecureLong secureLong = new SecureLong(model.secGiraffeDollars, model.giraffeDollars);
			model.giraffeDollars = ammount;
			secureLong = ammount;
			if (CurrencyHasMax(type))
			{
				model.giraffeDollars = Mathf.Min(MaxCurrencyCount(type), model.giraffeDollars);
				secureLong = MathEx.Min(MaxCurrencyCount(type), secureLong.valueLong);
			}
			model.secGiraffeDollars = secureLong.ToModel();
			break;
		}
		}
		DoOnSave();
	}

	public long CurrencyCount(CurrencyType type)
	{
		if (ConfigBase.instance.secureCurrency)
		{
			SecureLong secureLong = 0L;
			switch (type)
			{
			case CurrencyType.coins:
				secureLong = new SecureLong(model.secCoins, model.coins);
				break;
			case CurrencyType.diamonds:
				secureLong = new SecureLong(model.secDiamonds, model.diamonds);
				break;
			default:
				secureLong = new SecureLong(model.secGiraffeDollars, model.giraffeDollars);
				break;
			}
			return (int)secureLong.valueLong;
		}
		switch (type)
		{
		case CurrencyType.coins:
			return model.coins;
		case CurrencyType.diamonds:
			return model.diamonds;
		default:
			return model.giraffeDollars;
		}
	}

	public bool CanBuyItemWithPrice(SingleCurrencyPrice price)
	{
		return CanBuyItemWithPrice(price.cost, price.currency);
	}

	public bool CanBuyItemWithPrice(int price, CurrencyType currencyType)
	{
		return CurrencyCount(currencyType) >= price;
	}
}
