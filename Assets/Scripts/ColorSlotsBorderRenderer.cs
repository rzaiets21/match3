using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class ColorSlotsBorderRenderer : MonoBehaviour
{
	public class ColorSlotsTilesSlotsProvider : TilesSlotsProvider
	{
		public Match3Game game;

		public ColorSlotsBorderRenderer borderRenderer;

		private List<Slot> allSlots = new List<Slot>();

		public int MaxSlots => game.board.size.x * game.board.size.y;

		public void Init(Match3Game game, ColorSlotsBorderRenderer borderRenderer)
		{
			this.game = game;
			this.borderRenderer = borderRenderer;
		}

		public Vector2 StartPosition(float size)
		{
			return new Vector2((float)(-game.board.size.x) * size * 0.5f, (float)(-game.board.size.y) * size * 0.5f);
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
			return borderRenderer.ShouldBeRendered(levelSlot);
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

	private ColorSlotsTilesSlotsProvider tilesSlotsProvider = new ColorSlotsTilesSlotsProvider();

	[SerializeField]
	private List<TilesBorderRenderer> borderRenderer = new List<TilesBorderRenderer>();

	[SerializeField]
	private List<TilesBorderRenderer> slotsRenderer = new List<TilesBorderRenderer>();

	private int lastColoredSlates;

	private bool ShouldBeRendered(Slot slot)
	{
		return slot.GetSlotComponent<SlotColorSlate>() != null;
	}

	public void DisplayChocolate(Match3Game game)
	{
		List<Slot> sortedSlotsUpdateList = game.board.sortedSlotsUpdateList;
		int num = 0;
		for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
		{
			Slot slot = sortedSlotsUpdateList[i];
			if (ShouldBeRendered(slot))
			{
				num++;
			}
		}
		bool num2 = lastColoredSlates != num;
		lastColoredSlates = num;
		if (!num2)
		{
			return;
		}
		bool flag = num > 0;
		for (int j = 0; j < borderRenderer.Count; j++)
		{
			GGUtil.SetActive(borderRenderer[j], flag);
		}
		for (int k = 0; k < slotsRenderer.Count; k++)
		{
			GGUtil.SetActive(slotsRenderer[k], flag);
		}
		if (flag)
		{
			tilesSlotsProvider.Init(game, this);
			for (int l = 0; l < borderRenderer.Count; l++)
			{
				borderRenderer[l].ShowBorderOnLevel(tilesSlotsProvider);
			}
			for (int m = 0; m < slotsRenderer.Count; m++)
			{
				slotsRenderer[m].ShowSlotsOnLevel(tilesSlotsProvider);
			}
		}
	}
}
