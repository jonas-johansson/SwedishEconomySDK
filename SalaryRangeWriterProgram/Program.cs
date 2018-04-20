using SwedishEconomySDK;
using System.Collections.Generic;
using System.IO;

namespace SalaryRangeWriterProgram
{
	class Program
	{
		static void Main(string[] args)
		{
			var output = new List<string>();

			// Fill in your data here:
			var calc = new IncomeTaxCalculator(new IncomeTaxConfig()
			{
				// Stockholm 2018.
				skattesats_kommunalskatt = 17.90 / 100,
				skattesats_landstingsskatt = 12.08 / 100,
				skattesats_begravning = 0.075 / 100,

				// Correct for 2018.
				taxYear = 2018,
				PBB = 45500,
				IBB = 62500,
				nedreSkiktgräns = 455300,
				övreSkiktgräns = 662300,

				birthYear = 1980,
			};

			output.Add("Gross salary (year), Net salary (year), Total tax (year), Effective tax rate");

			for (int salary = 1000 * 12; salary <= 100000 * 12; salary += 1000 * 12)
			{
				var r = calc.Calculate(salary);
				output.Add(string.Format("{0}, {1}, {2}, {3}", salary, salary - r.slutligSkatt, r.slutligSkatt, r.effektivTotalSkattesats));
			}

			File.WriteAllLines("SalaryRangeOutput.csv", output);
		}
	}
}
