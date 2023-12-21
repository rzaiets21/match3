using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class SnowCoverBorderRenderer : MonoBehaviour
{
	public class HiddenElementProvider : TilesSlotsProvider
	{
		public int lastColoredSlates;

		public virtual int CountColoredSlates()
		{
			return 0;
		}
	}

	public class LevelSlotsProvider : HiddenElementProvider
	{
		public Match3Game game;

		private List<Slot> allSlots = new List<Slot>();

		public int MaxSlots => game.board.size.x * game.board.size.y;

		public void Init(Match3Game game)
		{
			this.game = game;
		}

		public int CountColoredSlates()
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
			return levelSlot.GetSlotComponent<SnowCover>() != null;
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

	private class LevelRendererPair
	{
		public HiddenElementProvider levelProvider;

		public BorderTilemapRenderer renderer;

		public bool isHidden;
	}

	[SerializeField]
	private BorderTilemapRenderer levelRenderer;

	private List<LevelRendererPair> levelRendererPairs;

	public void Render(Match3Game game)
	{
		if (levelRendererPairs == null)
		{
			levelRendererPairs = new List<LevelRendererPair>();
			LevelRendererPair levelRendererPair = new LevelRendererPair();
			LevelSlotsProvider levelSlotsProvider = new LevelSlotsProvider();
			levelSlotsProvider.Init(game);
			levelRendererPair.levelProvider = levelSlotsProvider;
			levelRendererPair.renderer = levelRenderer;
			levelRendererPairs.Add(levelRendererPair);
		}
		for (int i = 0; i < levelRendererPairs.Count; i++)
		{
			LevelRendererPair levelRendererPair2 = levelRendererPairs[i];
			if (levelRendererPair2.isHidden)
			{
				continue;
			}
			HiddenElementProvider levelProvider = levelRendererPair2.levelProvider;
			int num = levelProvider.CountColoredSlates();
			if (num != levelProvider.lastColoredSlates)
			{
				levelProvider.lastColoredSlates = num;
				levelRendererPair2.renderer.ShowBorder(levelProvider);
				GGUtil.SetActive(levelRendererPair2.renderer, num > 0);
				if (num <= 0)
				{
					levelRendererPair2.isHidden = true;
				}
			}
		}
	}
}
