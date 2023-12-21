using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class CarpetSpreadBehaviour : MonoBehaviour
{
	public class RendererTilesSlotsProvider : TilesSlotsProvider
	{
		public CarpetSpread.SlotData[] slots;

		public Match3Game game;

		private List<Slot> innerSlots = new List<Slot>();

		public int MaxSlots => game.board.size.x * game.board.size.y;

		public void Init(Match3Game game, CarpetSpread.SlotData[] slots)
		{
			this.game = game;
			this.slots = slots;
		}

		public Vector2 StartPosition(float slotSize)
		{
			return new Vector2((float)(-game.board.size.x) * slotSize * 0.5f, (float)(-game.board.size.y) * slotSize * 0.5f);
		}

		public Slot GetSlot(IntVector2 position)
		{
			if (position.x >= game.board.size.x || position.x < 0 || position.y >= game.board.size.y || position.y < 0)
			{
				return new Slot(position, isOccupied: false);
			}
			CarpetSpread.SlotData slotData = slots[game.board.Index(position)];
			return new Slot(position, slotData.hasCarpet);
		}

		public List<Slot> GetAllSlots()
		{
			innerSlots.Clear();
			for (int i = 0; i < slots.Length; i++)
			{
				CarpetSpread.SlotData slotData = slots[i];
				if (slotData.hasCarpet)
				{
					innerSlots.Add(new Slot(slotData.position, slotData.hasCarpet));
				}
			}
			return innerSlots;
		}
	}

	[SerializeField]
	private List<TilesBorderRenderer> borderRenderers = new List<TilesBorderRenderer>();

	[SerializeField]
	private TilesBorderRenderer slotsRenderer;

	private RendererTilesSlotsProvider slotsProvider = new RendererTilesSlotsProvider();

	public void Init(Match3Game game, CarpetSpread carpetSpread)
	{
		slotsProvider.Init(game, carpetSpread.slots);
	}

	public void RefreshCarpet()
	{
		slotsRenderer.ShowSlotsOnLevel(slotsProvider);
		for (int i = 0; i < borderRenderers.Count; i++)
		{
			borderRenderers[i].ShowBorderOnLevel(slotsProvider);
		}
	}
}
