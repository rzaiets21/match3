using System;

namespace Expressive
{
	[Flags]
	public enum ExpressiveOptions
	{
		None = 0x1,
		IgnoreCase = 0x2,
		NoCache = 0x4,
		RoundAwayFromZero = 0x8,
		All = 0xE
	}
}
