using GGMatch3;
using System.Collections.Generic;
using UnityEngine;

public class BubblesBoxFloorRenderer : MonoBehaviour
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

		public int minLevel;

		private List<Slot> allSlots = new List<Slot>();

		public int MaxSlots => game.board.size.x * game.board.size.y;

		public void Init(Match3Game game, int minLevel)
		{
			this.game = game;
			this.minLevel = minLevel;
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
			SlotColorSlate slotComponent = levelSlot.GetSlotComponent<SlotColorSlate>();
			if (slotComponent == null)
			{
				return false;
			}
			if (slotComponent.blockerLevel < minLevel)
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

	public class HoleProvider : HiddenElementProvider
	{
		private Match3Game game;

		private List<Slot> allSlots = new List<Slot>();

		public int MaxSlots => GetAllSlots().Count;

		public int CountColoredSlates()
		{
			return GetAllSlots().Count;
		}

		public void Init(Match3Game game)
		{
			this.game = game;
		}

		public Vector2 StartPosition(float size)
		{
			return new Vector2((float)(-game.board.size.x) * size * 0.5f, (float)(-game.board.size.y) * size * 0.5f);
		}

		public Slot GetSlot(IntVector2 position)
		{
			Slot result = default(Slot);
			result.position = position;
			result.isOccupied = game.board.burriedElements.ContainsPosition(position);
			return result;
		}

		public List<Slot> GetAllSlots()
		{
			BurriedElements burriedElements = game.board.burriedElements;
			allSlots.Clear();
			List<BurriedElementPiece> elementPieces = burriedElements.elementPieces;
			for (int i = 0; i < elementPieces.Count; i++)
			{
				LevelDefinition.BurriedElement elementDefinition = elementPieces[i].elementDefinition;
				IntVector2 position = elementDefinition.position;
				IntVector2 oppositeCornerPosition = elementDefinition.oppositeCornerPosition;
				int num = Mathf.Min(position.x, oppositeCornerPosition.x);
				int num2 = Mathf.Max(position.x, oppositeCornerPosition.x);
				int num3 = Mathf.Min(position.y, oppositeCornerPosition.y);
				int num4 = Mathf.Max(position.y, oppositeCornerPosition.y);
				for (int j = num; j <= num2; j++)
				{
					for (int k = num3; k <= num4; k++)
					{
						Slot item = default(Slot);
						item.position = new IntVector2(j, k);
						item.isOccupied = true;
						allSlots.Add(item);
					}
				}
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
	private BorderTilemapRenderer holeRenderer;

	[SerializeField]
	private BorderTilemapRenderer level1Renderer;

	[SerializeField]
	private BorderTilemapRenderer level2Renderer;

	[SerializeField]
	private bool useHole;

	private HoleProvider holeProvider;

	private List<LevelRendererPair> levelRendererPairs;

	public void Render(Match3Game game)
	{
		if (levelRendererPairs == null)
		{
			levelRendererPairs = new List<LevelRendererPair>();
			if (useHole && holeRenderer != null)
			{
				holeProvider = new HoleProvider();
				holeProvider.Init(game);
				LevelRendererPair levelRendererPair = new LevelRendererPair();
				levelRendererPair.levelProvider = holeProvider;
				levelRendererPair.renderer = holeRenderer;
				levelRendererPairs.Add(levelRendererPair);
			}
			else
			{
				GGUtil.SetActive(holeRenderer, active: false);
			}
			for (int i = 0; i < 2; i++)
			{
				LevelRendererPair levelRendererPair2 = new LevelRendererPair();
				LevelSlotsProvider levelSlotsProvider = new LevelSlotsProvider();
				levelSlotsProvider.Init(game, i + 1);
				levelRendererPair2.levelProvider = levelSlotsProvider;
				levelRendererPair2.renderer = ((i == 0) ? level1Renderer : level2Renderer);
				levelRendererPairs.Add(levelRendererPair2);
			}
		}
		for (int j = 0; j < levelRendererPairs.Count; j++)
		{
			LevelRendererPair levelRendererPair3 = levelRendererPairs[j];
			if (levelRendererPair3.isHidden)
			{
				continue;
			}
			HiddenElementProvider levelProvider = levelRendererPair3.levelProvider;
			int num = levelProvider.CountColoredSlates();
			if (num != levelProvider.lastColoredSlates)
			{
				levelProvider.lastColoredSlates = num;
				levelRendererPair3.renderer.ShowBorder(levelProvider);
				GGUtil.SetActive(levelRendererPair3.renderer, num > 0);
				if (num <= 0)
				{
					levelRendererPair3.isHidden = true;
				}
			}
		}
	}
}
