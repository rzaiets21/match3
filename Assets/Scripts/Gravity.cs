using System.Collections.Generic;

namespace GGMatch3
{
	public struct Gravity
	{
		public struct SandflowDirection
		{
			public IntVector2 direction;

			public IntVector2 offset;

			public IntVector2 forceDirection;

			public SandflowDirection(IntVector2 direction, IntVector2 forceDirection)
			{
				this.direction = direction;
				this.forceDirection = forceDirection;
				offset = direction + forceDirection;
			}
		}

		public bool up;

		public bool down;

		public bool left;

		public bool right;

		private List<IntVector2> orthoDirections_;

		private List<IntVector2> directions_;

		private List<SandflowDirection> sandflowDirections_;

		public List<IntVector2> orthoDirections
		{
			get
			{
				if (orthoDirections_ != null)
				{
					return orthoDirections_;
				}
				orthoDirections_ = new List<IntVector2>();
				if (up || down)
				{
					orthoDirections_.Add(IntVector2.right);
					orthoDirections_.Add(IntVector2.left);
				}
				if (left || right)
				{
					orthoDirections_.Add(IntVector2.up);
					orthoDirections_.Add(IntVector2.down);
				}
				return orthoDirections_;
			}
		}

		public List<IntVector2> forceDirections
		{
			get
			{
				if (directions_ != null)
				{
					return directions_;
				}
				directions_ = new List<IntVector2>();
				if (up)
				{
					directions_.Add(IntVector2.up);
				}
				if (down)
				{
					directions_.Add(IntVector2.down);
				}
				if (left)
				{
					directions_.Add(IntVector2.left);
				}
				if (right)
				{
					directions_.Add(IntVector2.right);
				}
				return directions_;
			}
		}

		public List<SandflowDirection> sandflowDirections
		{
			get
			{
				if (sandflowDirections_ != null)
				{
					return sandflowDirections_;
				}
				sandflowDirections_ = new List<SandflowDirection>();
				if (up)
				{
					sandflowDirections_.Add(new SandflowDirection(IntVector2.left, IntVector2.up));
					sandflowDirections_.Add(new SandflowDirection(IntVector2.right, IntVector2.up));
				}
				if (down)
				{
					sandflowDirections_.Add(new SandflowDirection(IntVector2.left, IntVector2.down));
					sandflowDirections_.Add(new SandflowDirection(IntVector2.right, IntVector2.down));
				}
				if (left)
				{
					sandflowDirections_.Add(new SandflowDirection(IntVector2.up, IntVector2.left));
					sandflowDirections_.Add(new SandflowDirection(IntVector2.down, IntVector2.left));
				}
				if (right)
				{
					sandflowDirections_.Add(new SandflowDirection(IntVector2.up, IntVector2.right));
					sandflowDirections_.Add(new SandflowDirection(IntVector2.down, IntVector2.right));
				}
				return sandflowDirections_;
			}
		}

		public List<Slot> FindSlotsToWhichCanJumpTo(Slot origin, Match3Game game)
		{
			List<Slot> list = new List<Slot>();
			IntVector2 position = origin.position;
			List<IntVector2> forceDirections = this.forceDirections;
			for (int i = 0; i < forceDirections.Count; i++)
			{
				IntVector2 a = forceDirections[i];
				int num = 0;
				while (true)
				{
					num++;
					IntVector2 position2 = position + a * num;
					if (game.board.IsOutOfBoard(position2))
					{
						break;
					}
					Slot slot = game.GetSlot(position2);
					if (slot != null)
					{
						if (num != 1)
						{
							list.Add(slot);
						}
						break;
					}
				}
			}
			return list;
		}
	}
}
