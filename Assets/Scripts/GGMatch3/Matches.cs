using System.Collections.Generic;

namespace GGMatch3
{
	public class Matches
	{
		private Island[] islandsMap;

		private Match3Board board;

		public List<Connection> connectionsList = new List<Connection>();

		private List<Connection> connectionPool = new List<Connection>();

		private List<Island> islandPool = new List<Island>();

		public List<Island> islands = new List<Island>();

		public int MatchesCount => islands.Count;

		public void Init(Match3Board board)
		{
			this.board = board;
			islandsMap = new Island[board.size.x * board.size.y];
		}

		public void Clear()
		{
			for (int i = 0; i < islandsMap.Length; i++)
			{
				islandsMap[i] = null;
			}
			for (int j = 0; j < connectionsList.Count; j++)
			{
				Connection item = connectionsList[j];
				connectionPool.Add(item);
			}
			connectionsList.Clear();
			for (int k = 0; k < islands.Count; k++)
			{
				Island item2 = islands[k];
				islandPool.Add(item2);
			}
			islands.Clear();
		}

		public Island GetIsland(IntVector2 position)
		{
			int num = board.Index(position);
			if (num < 0 || num >= islandsMap.Length)
			{
				return null;
			}
			return islandsMap[num];
		}

		public void SetIsland(IntVector2 position, Island island)
		{
			int num = board.Index(position);
			if (num >= 0 && num < islandsMap.Length)
			{
				islandsMap[num] = island;
			}
		}

		private Connection NextConnectionFromPool()
		{
			if (connectionPool.Count > 0)
			{
				int index = connectionPool.Count - 1;
				Connection result = connectionPool[index];
				connectionPool.RemoveAt(index);
				return result;
			}
			return new Connection();
		}

		public Island NextIslandFromPool()
		{
			if (islandPool.Count > 0)
			{
				int index = islandPool.Count - 1;
				Island island = islandPool[index];
				islandPool.RemoveAt(index);
				island.Clear();
				return island;
			}
			return new Island();
		}

		public void AddCopyOfConnection(Connection c)
		{
			Connection connection = NextConnectionFromPool();
			connection.CopyFrom(c);
			connectionsList.Add(connection);
		}

		public void UpdateIslandOnMap(Island island)
		{
			for (int i = 0; i < island.connectionsList.Count; i++)
			{
				Connection connection = island.connectionsList[i];
				for (int j = 0; j < connection.slotsList.Count; j++)
				{
					Slot slot = connection.slotsList[j];
					SetIsland(slot.position, island);
				}
			}
		}

		public void RemoveIslandOnMap(Island island)
		{
			for (int i = 0; i < island.connectionsList.Count; i++)
			{
				Connection connection = island.connectionsList[i];
				for (int j = 0; j < connection.slotsList.Count; j++)
				{
					Slot slot = connection.slotsList[j];
					if (GetIsland(slot.position) == island)
					{
						SetIsland(slot.position, null);
					}
				}
			}
		}

		public void RemoveIsland(Island island)
		{
			islandPool.Add(island);
			islands.Remove(island);
		}

		public void AddIsland(Island island)
		{
			islands.Add(island);
		}
	}
}
