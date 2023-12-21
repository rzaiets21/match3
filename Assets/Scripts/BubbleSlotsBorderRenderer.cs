using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSlotsBorderRenderer : MonoBehaviour
{
	protected class BubbleSlotsProvider : TilesSlotsProvider
	{
		public Match3Game game;

		private List<Slot> allSlots = new List<Slot>();

		public int MaxSlots => game.board.size.x * game.board.size.y;

		public void Init(Match3Game game)
		{
			this.game = game;
		}

		public int CountSoapSlates()
		{
			List<GGMatch3.Slot> sortedSlotsUpdateList = game.board.sortedSlotsUpdateList;
			int num = 0;
			for (int i = 0; i < sortedSlotsUpdateList.Count; i++)
			{
				GGMatch3.Slot levelSlot = sortedSlotsUpdateList[i];
				if (IsOccupied(levelSlot))
				{
					num++;
				}
			}
			return num;
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
			if (levelSlot.GetSlotComponent<BubblesPieceBlocker>() == null)
			{
				return false;
			}
			return true;
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

	[SerializeField]
	private BorderTilemapRenderer tilemapRenderer;

	protected BubbleSlotsProvider soapSlotsProvider = new BubbleSlotsProvider();

	private int lastBubbleCount;

	public void Render(Match3Game game)
	{
		soapSlotsProvider.Init(game);
		int num = soapSlotsProvider.CountSoapSlates();
		if (num != lastBubbleCount)
		{
			lastBubbleCount = num;
			tilemapRenderer.ShowBorder(soapSlotsProvider);
		}
	}
}
