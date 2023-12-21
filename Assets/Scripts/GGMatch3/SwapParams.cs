namespace GGMatch3
{
	public class SwapParams
	{
		public IntVector2 startPosition;

		public IntVector2 swipeToPosition;

		private Match3Game.InputAffectorExport affectorExport_;

		public Match3Game.InputAffectorExport affectorExport
		{
			get
			{
				if (affectorExport_ == null)
				{
					affectorExport_ = new Match3Game.InputAffectorExport();
				}
				return affectorExport_;
			}
			set
			{
				affectorExport_ = value;
			}
		}
	}
}
