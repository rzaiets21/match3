using System.Collections.Generic;

namespace GGMatch3
{
	public class FindMatches
	{
		public Match3Board board;

		public Matches matches;

		private Connection currentConnection = new Connection();

		private List<Island> connectedIslands = new List<Island>();

		public IntVector2 size => board.size;

		public void Init(Match3Board board)
		{
			this.board = board;
			matches = new Matches();
			matches.Init(board);
		}

		private Slot GetSlot(IntVector2 pos)
		{
			return board.GetSlot(pos);
		}

		private bool CanParticipateInMatch(Slot slot)
		{
			if (slot == null)
			{
				return false;
			}
			Chip slotComponent = slot.GetSlotComponent<Chip>();
			if (slotComponent == null)
			{
				return false;
			}
			if (!slotComponent.canFormColorMatches)
			{
				return false;
			}
			if (slot.isSlotMatchingSuspended)
			{
				return false;
			}
			return true;
		}

		private void AddSlotToLineConnectionIfPossible(Connection currentConnection, int x, int y, bool isLast, Matches matches)
		{
			Slot slot = GetSlot(new IntVector2(x, y));
			Chip chip = null;
			if (slot != null)
			{
				chip = slot.GetSlotComponent<Chip>();
			}
			bool num = CanParticipateInMatch(slot);
			if (slot == null || chip == null || slot.isSlotMatchingSuspended || !currentConnection.IsChipAcceptable(chip))
			{
				if (currentConnection.isUsable)
				{
					matches.AddCopyOfConnection(currentConnection);
				}
				currentConnection.Clear();
			}
			if (num)
			{
				currentConnection.slotsList.Add(slot);
			}
			if (isLast && currentConnection.isUsable)
			{
				matches.AddCopyOfConnection(currentConnection);
			}
			if (isLast)
			{
				currentConnection.Clear();
			}
		}

		public Matches FindAllMatches()
		{
			matches.Clear();
			currentConnection.Clear();
			for (int i = 0; i < size.y; i++)
			{
				currentConnection.Clear();
				currentConnection.type = Connection.ConnectionType.Horizontal;
				for (int j = 0; j < size.x; j++)
				{
					bool isLast = j >= size.x - 1;
					AddSlotToLineConnectionIfPossible(currentConnection, j, i, isLast, matches);
				}
			}
			for (int k = 0; k < size.x; k++)
			{
				currentConnection.Clear();
				currentConnection.type = Connection.ConnectionType.Vertical;
				for (int l = 0; l < size.y; l++)
				{
					bool isLast2 = l >= size.y - 1;
					AddSlotToLineConnectionIfPossible(currentConnection, k, l, isLast2, matches);
				}
			}
			List<Slot> list = new List<Slot>();
			for (int m = 0; m < size.x - 1; m++)
			{
				for (int n = 0; n < size.y - 1; n++)
				{
					Slot slot = GetSlot(new IntVector2(m, n));
					Slot slot2 = GetSlot(new IntVector2(m + 1, n));
					Slot slot3 = GetSlot(new IntVector2(m, n + 1));
					Slot slot4 = GetSlot(new IntVector2(m + 1, n + 1));
					list.Clear();
					list.Add(slot);
					list.Add(slot2);
					list.Add(slot3);
					list.Add(slot4);
					bool flag = true;
					ItemColor itemColor = ItemColor.Unknown;
					for (int num = 0; num < list.Count; num++)
					{
						Slot slot5 = list[num];
						if (slot5 == null)
						{
							flag = false;
							break;
						}
						if (!CanParticipateInMatch(slot5))
						{
							flag = false;
							break;
						}
						Chip slotComponent = slot5.GetSlotComponent<Chip>();
						if (slotComponent == null)
						{
							flag = false;
							break;
						}
						if (itemColor == ItemColor.Unknown)
						{
							itemColor = slotComponent.itemColor;
						}
						if (itemColor != slotComponent.itemColor)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						Connection connection = currentConnection;
						connection.Clear();
						connection.type = Connection.ConnectionType.Square;
						connection.slotsList.AddRange(list);
						matches.AddCopyOfConnection(connection);
					}
				}
			}
			connectedIslands.Clear();
			for (int num2 = 0; num2 < matches.connectionsList.Count; num2++)
			{
				Connection connection2 = matches.connectionsList[num2];
				connectedIslands.Clear();
				for (int num3 = 0; num3 < connection2.slotsList.Count; num3++)
				{
					Slot slot6 = connection2.slotsList[num3];
					Island island = matches.GetIsland(slot6.position);
					if (island != null && !connectedIslands.Contains(island))
					{
						connectedIslands.Add(island);
					}
				}
				if (connectedIslands.Count == 0)
				{
					Island island2 = matches.NextIslandFromPool();
					island2.connectionsList.Add(connection2);
					matches.AddIsland(island2);
					matches.UpdateIslandOnMap(island2);
					continue;
				}
				Island island3 = connectedIslands[0];
				for (int num4 = 1; num4 < connectedIslands.Count; num4++)
				{
					Island island4 = connectedIslands[num4];
					island3.AddConnection(island4.connectionsList);
					matches.RemoveIsland(island4);
					matches.RemoveIslandOnMap(island4);
				}
				island3.AddConnection(connection2);
				matches.UpdateIslandOnMap(island3);
			}
			return matches;
		}
	}
}
