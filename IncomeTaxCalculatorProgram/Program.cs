using SwedishEconomySDK;
using System;

namespace IncomeTaxCalculatorProgram
{
	public class TaxYearVariables
	{
		public int TaxYear { set; get; }
		public int PBB { set; get; }
		public int IBB { set; get; }
	}

	public class SkattesatsConstants
	{
	}

	public class TaxYearConstants_2021
	{
		public const int PBB = 47_600;
		public const int IBB = 68_200;
		public const int TaxYear = 2021;
	}

	public class Program
	{
		static void Main()
		{
			//// Fill in your data here:
			//var taxConfig = new IncomeTaxConfig()
			//{
			//	// Stockholm 2018.
			//	skattesats_kommunalskatt = 17.90 / 100,
			//	skattesats_landstingsskatt = 12.08 / 100,
			//	skattesats_begravning = 0.075 / 100,

			//	// Correct for 2018.
			//	taxYear = 2018,
			//	PBB = 45500,
			//	IBB = 62500,
			//	nedreSkiktgräns = 455300,
			//	övreSkiktgräns = 662300,

			//	birthYear = 1980,
			//};

			// Fill in your data here:
			var taxConfig = new IncomeTaxConfig()
			{
				// Stockholm 2021.
				skattesats_kommunalskatt = 17.74 / 100,
				skattesats_landstingsskatt = 12.08 / 100,
				skattesats_begravning = 0.065 / 100,

				// Correct for 2021.
				taxYear = 2021,
				PBB = TaxYearConstants_2021.PBB,
				IBB = TaxYearConstants_2021.IBB,
				nedreSkiktgräns = 523_200,

				birthYear = 1980,
			};

			double arbetsinkomstFörHelaÅret = 537_200;

			// Calculate and present the result.
			var incomeTaxCalculator = new IncomeTaxCalculator(taxConfig);
			var r = incomeTaxCalculator.Calculate(arbetsinkomstFörHelaÅret);

			Console.WriteLine();
			Console.WriteLine("Arbetsinkomst: {0}", arbetsinkomstFörHelaÅret);
			Console.WriteLine("Beskattningsbar förvärvsinkomst: {0}", r.beskattningsbarForvärvsinkomst);
			Console.WriteLine("Kommunalskatt: {0}", r.kommunalskatt);
			Console.WriteLine("Statligskatt: {0}", r.statligskatt);
			Console.WriteLine("Begravningsavgift: {0}", r.begravningsavgift);
			Console.WriteLine("Grundavdrag: {0}", r.grundavdrag);
			Console.WriteLine("Allmän pensionsavgift: {0}", r.allmänPensionsavgift);
			Console.WriteLine("Jobbskatteavdrag: {0}", r.jobbskatteavdrag);
			Console.WriteLine("Totalskatt: {0}", r.slutligSkatt);
			Console.WriteLine("Effektiv total skattesats: {0}%", r.effektivTotalSkattesats * 100);
			Console.WriteLine();
		}
	}
}
