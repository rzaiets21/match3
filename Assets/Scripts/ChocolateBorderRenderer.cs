using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class ChocolateBorderRenderer : MonoBehaviour
{
	public class ChocolateTilesSlotsProvider : TilesSlotsProvider
	{
		public Match3Game game;

		public ChocolateBorderRenderer borderRenderer;

		private List<Slot> allSlots = new List<Slot>();

		public int MaxSlots => game.board.size.x * game.board.size.y;

		public void Init(Match3Game game, ChocolateBorderRenderer borderRenderer)
		{
			this.game = game;
			this.borderRenderer = borderRenderer;
		}

		public Vector2 StartPosition(float slotSize)
		{
			return new Vector2((float)(-game.board.size.x) * slotSize * 0.5f, (float)(-game.board.size.y) * slotSize * 0.5f);
		}

		public Slot GetSlot(IntVector2 position)
		{
			Slot result = default(Slot);
			GGMatch3.Slot slot = game.GetSlot(position);
			result.position = position;
			if (slot != null)
			{
				result.isOccupied = IsOccupied(slot);
			}
			return result;
		}

		private bool IsOccupied(GGMatch3.Slot levelSlot)
		{
			return borderRenderer.ShouldBeRenderedForChocolate(levelSlot);
		}

		public List<Slot> GetAllSlots()
		{
			allSlots.Clear();
			List<GGMatch3.Slot> sortedSlotsUpdateList = game.board.sortedSlotsUpdateList;
			for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
			{
				GGMatch3.Slot slot = sortedSlotsUpdateList[i];
				Slot item = default(Slot);
				item.position = slot.position;
				item.isOccupied = IsOccupied(slot);
				allSlots.Add(item);
			}
			return allSlots;
		}
	}

	private ChocolateTilesSlotsProvider tilesSlotsProvider = new ChocolateTilesSlotsProvider();

	[SerializeField]
	private TilesBorderRenderer borderRenderer;

	[SerializeField]
	private TilesBorderRenderer slotsRenderer;

	private bool ShouldBeRenderedForChocolate(Slot slot)
	{
		if (slot.GetSlotComponent<BasketBlocker>() != null)
		{
			return !slot.isMoving;
		}
		return false;
	}

	public void DisplayChocolate(Match3Game game)
	{
		List<Slot> sortedSlotsUpdateList = game.board.sortedSlotsUpdateList;
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
		{
			Slot slot = sortedSlotsUpdateList[i];
			bool flag3 = ShouldBeRenderedForChocolate(slot);
			if (slot.wasRenderedForChocolateLastFrame != flag3)
			{
				flag = true;
			}
			if (flag3)
			{
				flag2 = true;
			}
		}
		if (flag)
		{
			GGUtil.SetActive(borderRenderer, flag2);
			GGUtil.SetActive(slotsRenderer, flag2);
			if (flag2)
			{
				tilesSlotsProvider.Init(game, this);
				borderRenderer.ShowBorderOnLevel(tilesSlotsProvider);
				slotsRenderer.ShowSlotsOnLevel(tilesSlotsProvider);
			}
		}
	}
}
