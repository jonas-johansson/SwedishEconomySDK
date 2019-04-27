using System;

namespace SwedishEconomySDK
{
	public class IncomeTaxCalculator
	{
		readonly double skattesats_nedreGräns = 0.20;
		readonly double skattesats_övreGräns = 0.05;
		readonly IncomeTaxConfig config;

		public IncomeTaxCalculator(IncomeTaxConfig config)
		{
			this.config = config;
		}

		public IncomeTaxResult Calculate(double arbetsinkomst)
		{
			double fastställdFörvärvsinkomst = arbetsinkomst;
			double grundavdrag = CalculateGrundavdrag(fastställdFörvärvsinkomst);
			double beskattningsbarFörvärvsinkomst = fastställdFörvärvsinkomst - grundavdrag;
			double allmänPensionsavgift = CalculateAllmänPensionsavgift(fastställdFörvärvsinkomst);
			double kommunalSkatt = CalculateKommunalSkatt(beskattningsbarFörvärvsinkomst);

			var r = new IncomeTaxResult()
			{
				kommunalskatt = kommunalSkatt,
				statligskatt = CalculateStatligSkatt(beskattningsbarFörvärvsinkomst),
				begravningsavgift = CalculateBegravningsavgift(beskattningsbarFörvärvsinkomst),
				grundavdrag = grundavdrag,
				beskattningsbarForvärvsinkomst = beskattningsbarFörvärvsinkomst,
				allmänPensionsavgift = allmänPensionsavgift,
				skattereduktionFörAllmänPensionsavgift = CalculateSkattereduktionFörAllmänPensionsavgift(fastställdFörvärvsinkomst),
				jobbskatteavdrag = CalculateJobbskatteavdrag(arbetsinkomst, grundavdrag, config.AgeAtTaxYearStart, allmänPensionsavgift, kommunalSkatt),
			};

			r.slutligSkatt = r.kommunalskatt + r.statligskatt + r.begravningsavgift - r.jobbskatteavdrag + r.allmänPensionsavgift - r.skattereduktionFörAllmänPensionsavgift;
			r.effektivTotalSkattesats = r.slutligSkatt / arbetsinkomst;

			return r;
		}

		double CalculateSkattereduktionFörAllmänPensionsavgift(double fastställdFörvärvsinkomst)
		{
			// In most cases the tax reduction for "allmän pensionsavgift" is exactly "allmän pensionsavgift".
			return CalculateAllmänPensionsavgift(fastställdFörvärvsinkomst);
		}

		double CalculateAllmänPensionsavgift(double fastställdFörvärvsinkomst)
		{
			// Reference: https://www4.skatteverket.se/rattsligvagledning/edition/2017.7/1348.html
			// Reference: https://www4.skatteverket.se/rattsligvagledning/2787.html?date=2015-01-01#section1-3

			if (fastställdFörvärvsinkomst < 0.423 * config.PBB)
				return 0;

			double cappedFastställdFörvärvsinkomst = Math.Min(fastställdFörvärvsinkomst, 8.07 * config.IBB);

			// Inkomst av anställning och inkomst av annat förvärvsarbete ska därvid var
			// för sig avrundas till närmast lägre hundratal kronor.
			double roundedDownFFI = Round.DownToNearest100(cappedFastställdFörvärvsinkomst);

			double avgift = 0.07 * roundedDownFFI;

			// Avgiften avrundas till närmast hela hundratal kronor. Avgift som slutar
			// på 50 kronor avrundas till närmast lägre hundratal kronor. Lag (2010:1261).
			bool endsWith50 = (((int)avgift) % 50) == 0;
			if (endsWith50)
				avgift = Round.DownToNearest100(avgift);
			else
				avgift = Round.ToNearest100(avgift);

			return avgift;
		}

		double CalculateBegravningsavgift(double beskattningsbarFörvärvsinkomst)
		{
			double avgift = Math.Floor(beskattningsbarFörvärvsinkomst * config.skattesats_begravning);
			return avgift;
		}

		double CalculateKommunalSkatt(double beskattningsbarFörvärvsinkomst)
		{
			// Reference: https://www4.skatteverket.se/rattsligvagledning/2477.html?date=2017-01-01#section22-1;
			return Math.Floor((config.skattesats_kommunalskatt + config.skattesats_landstingsskatt) * beskattningsbarFörvärvsinkomst);
		}

		double CalculateStatligSkatt(double beskattningsbarFörvärvsinkomst)
		{
			// Reference: https://www4.skatteverket.se/rattsligvagledning/edition/2017.7/2930.html
			// Reference: https://www4.skatteverket.se/rattsligvagledning/2477.html?date=2017-01-01#section22-1

			if (beskattningsbarFörvärvsinkomst < 200)
				return 0;

			double bfiÖverNedreSkiktgränsen = Math.Max(0, beskattningsbarFörvärvsinkomst - config.nedreSkiktgräns);
			double bfiÖverÖvreSkiktgränsen = Math.Max(0, beskattningsbarFörvärvsinkomst - config.övreSkiktgräns);

			double statligSkatt = Math.Round((skattesats_nedreGräns * bfiÖverNedreSkiktgränsen) + (skattesats_övreGräns * bfiÖverÖvreSkiktgränsen));
			return statligSkatt;
		}

		double CalculateJobbskatteavdrag(double arbetsinkomst, double grundavdrag, int ageAtTaxYearStart, double allmänPensionsavgift, double kommunalskatt)
		{
			double jobbskatteavdrag;

			if (ageAtTaxYearStart < 65)
			{
				var AI = arbetsinkomst;
				var GA = grundavdrag;
				var PBB = config.PBB;
				var SKL = config.skattesats_kommlands;

				// Reference: https://www4.skatteverket.se/rattsligvagledning/edition/2017.7/2940.html

				if (AI < 0.91 * PBB)
					jobbskatteavdrag = (AI - GA) * SKL;
				else if (AI >= 0.91 * PBB && AI < 2.94 * PBB)
					jobbskatteavdrag = ((0.91 * PBB) + (0.332 * (AI - (0.91 * PBB))) - GA) * SKL;
				else if (AI >= 2.94 * PBB && AI < 8.08 * PBB)
					jobbskatteavdrag = ((1.584 * PBB) + (0.111 * (AI - (2.94 * PBB))) - GA) * SKL;
				else if (AI >= 8.08 * PBB && AI <= 13.54 * PBB)
					jobbskatteavdrag = ((2.155 * PBB) - GA) * SKL;
				else if (AI >= 13.54 * PBB)
					jobbskatteavdrag = (((2.155 * PBB) - GA) * SKL) - (0.03 * (AI - (13.54 * PBB)));
				else
					throw new Exception("Unhandled case for jobbskatteavdrag");
			}
			else
			{
				throw new Exception("Unhandled case. Can't calculate jobbskatteavdrag for people of age 65+ yet.");
			}

			if (jobbskatteavdrag + allmänPensionsavgift > kommunalskatt)
			{
				jobbskatteavdrag = kommunalskatt - allmänPensionsavgift;
			}

			jobbskatteavdrag = Math.Max(0, Math.Floor(jobbskatteavdrag));
			return jobbskatteavdrag;
		}

		double CalculateGrundavdrag(double fastställdFörvärvsinkomst)
		{
			// Reference: https://www4.skatteverket.se/rattsligvagledning/27071.html?date=2017-01-01#section63-3
			// Reference: https://www.skatteverket.se/privat/skatter/arbeteochinkomst/skattetabeller/grundavdrag.4.6d02084411db6e252fe80009078.html

			double FFI = fastställdFörvärvsinkomst;
			double PBB = config.PBB;

			double grundavdrag;

			if (FFI <= 0.99 * PBB)
				grundavdrag = 0.423 * PBB;
			else if (FFI <= 2.72 * PBB)
				grundavdrag = (0.423 * PBB) + (0.20 * (FFI - (0.99 * PBB)));
			else if (FFI <= 3.11 * PBB)
				grundavdrag = 0.77 * PBB;
			else if (FFI <= 7.88 * PBB)
				grundavdrag = (0.77 * PBB) - (0.10 * (FFI - (3.11 * PBB)));
			else
				grundavdrag = 0.293 * PBB;

			grundavdrag = Round.UpToNearest100(grundavdrag);
			return Math.Min(grundavdrag, fastställdFörvärvsinkomst);
		}
	}
}
