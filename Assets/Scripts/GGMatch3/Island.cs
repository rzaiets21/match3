using System.Collections.Generic;
using UnityEngine;

namespace GGMatch3
{
	public class Island
	{
		public List<Connection> connectionsList = new List<Connection>();

		private List<Slot> slotsList = new List<Slot>();

		public bool isFromSwap;

		public bool isCreatingPowerup
		{
			get
			{
				if (!isDiscoBall && !isBomb && !isRocket)
				{
					return isSeakingMissle;
				}
				return true;
			}
		}

		public ChipType powerupToCreate
		{
			get
			{
				if (isDiscoBall)
				{
					return ChipType.DiscoBall;
				}
				if (isBomb)
				{
					return ChipType.Bomb;
				}
				if (isRocket)
				{
					if (isHorizontalRocket)
					{
						return ChipType.HorizontalRocket;
					}
					return ChipType.VerticalRocket;
				}
				if (isSeakingMissle)
				{
					return ChipType.SeekingMissle;
				}
				return ChipType.Unknown;
			}
		}

		public bool isDiscoBall
		{
			get
			{
				for (int i = 0; i < connectionsList.Count; i++)
				{
					Connection connection = connectionsList[i];
					if (connection.type != Connection.ConnectionType.Square && connection.slotsList.Count >= 5)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isBomb
		{
			get
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < connectionsList.Count; i++)
				{
					Connection connection = connectionsList[i];
					if (connection.type != Connection.ConnectionType.Square)
					{
						if (connection.type == Connection.ConnectionType.Horizontal)
						{
							num = Mathf.Max(num, connection.slotsList.Count);
						}
						if (connection.type == Connection.ConnectionType.Vertical)
						{
							num2 = Mathf.Max(num2, connection.slotsList.Count);
						}
					}
				}
				if (num >= 3)
				{
					return num2 >= 3;
				}
				return false;
			}
		}

		public bool isRocket
		{
			get
			{
				for (int i = 0; i < connectionsList.Count; i++)
				{
					Connection connection = connectionsList[i];
					if (connection.type != Connection.ConnectionType.Square && connection.slotsList.Count >= 4)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isHorizontalRocket
		{
			get
			{
				for (int i = 0; i < connectionsList.Count; i++)
				{
					Connection connection = connectionsList[i];
					if (connection.type == Connection.ConnectionType.Vertical && connection.slotsList.Count >= 4)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool isSeakingMissle
		{
			get
			{
				for (int i = 0; i < connectionsList.Count; i++)
				{
					if (connectionsList[i].type == Connection.ConnectionType.Square)
					{
						return true;
					}
				}
				return false;
			}
		}

		public List<Slot> allSlots
		{
			get
			{
				slotsList.Clear();
				for (int i = 0; i < connectionsList.Count; i++)
				{
					Connection connection = connectionsList[i];
					for (int j = 0; j < connection.slotsList.Count; j++)
					{
						Slot item = connection.slotsList[j];
						if (!slotsList.Contains(item))
						{
							slotsList.Add(item);
						}
					}
				}
				return slotsList;
			}
		}

		public Connection squareConnection
		{
			get
			{
				for (int i = 0; i < connectionsList.Count; i++)
				{
					Connection connection = connectionsList[i];
					if (connection.type == Connection.ConnectionType.Square)
					{
						return connection;
					}
				}
				return null;
			}
		}

		public bool ContainsPosition(IntVector2 position)
		{
			for (int i = 0; i < connectionsList.Count; i++)
			{
				if (connectionsList[i].ContainsPosition(position))
				{
					return true;
				}
			}
			return false;
		}

		public void Clear()
		{
			connectionsList.Clear();
			isFromSwap = false;
		}

		public void AddConnection(List<Connection> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Connection c = list[i];
				AddConnection(c);
			}
		}

		public void AddConnection(Connection c)
		{
			if (c.type != Connection.ConnectionType.Square)
			{
				connectionsList.Add(c);
				return;
			}
			Connection squareConnection = this.squareConnection;
			if (squareConnection == null)
			{
				connectionsList.Add(c);
			}
			else if (!squareConnection.IsIntersecting(c))
			{
				connectionsList.Add(c);
			}
		}
	}
}
