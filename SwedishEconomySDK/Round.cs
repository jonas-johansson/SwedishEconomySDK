using System;

namespace SwedishEconomySDK
{
	public static class Round
	{
		public static double DownToNearest100(double value)
		{
			return Math.Floor(value / 100) * 100;
		}

		public static double UpToNearest100(double value)
		{
			return Math.Ceiling(value / 100) * 100;
		}

		public static double ToNearest100(double value)
		{
			return Math.Round(value / 100) * 100;
		}
	}
}
