using GGMatch3;
using ProtoModels;
using System;

public class Analytics : BehaviourSingletonInit<Analytics>
{
	public class EventBase
	{
		public virtual void Send()
		{
			BehaviourSingletonInit<GGNotificationCenter>.instance.BroadcastEvent(this);
		}
	}

	public class StageStartedEvent : EventBase
	{
		public GameScreen.StageState stageState;
	}

	public class StageCompletedEvent : EventBase
	{
		public GameScreen.StageState stageState;
	}

	public class StageFailedEvent : EventBase
	{
		public GameScreen.StageState stageState;
	}

	public class IAPEvent : EventBase
	{
		public InAppBackend.PurchaseEventArguments purchaseArguments;

		public OffersDB.ProductDefinition inAppObject;

		public string purchaseToken;
	}

	public class MovesBoughtEvent : EventBase
	{
		public GameScreen.StageState stageState;

		public BuyMovesPricesConfig.OfferConfig offer;
	}

	public class LivesRefillBoughtEvent : EventBase
	{
		public int livesBeforeRefill;

		public int livesAfterRefill;

		public LivesPriceConfig.PriceConfig config;
	}

	public class RoomItemBoughtEvent : EventBase
	{
		public SingleCurrencyPrice price;

		public DecorateRoomScreen screen;

		public GraphicsSceneConfig.VisualObject visualObject;

		public GraphicsSceneConfig.Variation variation;

		public int numberOfItemsOwned;
	}

	public class RoomItemChangedEvent : EventBase
	{
		public DecorateRoomScreen screen;

		public GraphicsSceneConfig.VisualObject visualObject;

		public GraphicsSceneConfig.Variation variation;
	}

	public class BoosterUsedEvent : EventBase
	{
		public BoosterConfig booster;

		public GameScreen.StageState stageState;
	}

	public class RateDialog : EventBase
	{
		public int timesShown;

		public bool isLike;

		public bool isGoingToRate;
	}

	public const string Filename = "ans.bytes";

	public AnalyticsDAO model;

	public float secondsTillExited => (float)TimeSpan.FromTicks(DateTime.UtcNow.Ticks - model.lastTimeWhenExited).TotalSeconds;

	public override void Init()
	{
		if (!ProtoIO.LoadFromFile<ProtoSerializer, AnalyticsDAO>("ans.bytes", GGFileIO.instance, out model))
		{
			model = new AnalyticsDAO();
			model.version = GGPlayerSettings.instance.Model.version;
		}
	}

	public void Save()
	{
		ProtoIO.SaveToFile("ans.bytes", model);
	}

	public void SetExited()
	{
		model.lastTimeWhenExited = DateTime.UtcNow.Ticks;
		Save();
	}

	public void IncSessionNum()
	{
		model.sessionNum++;
		Save();
	}
}
