using System;

namespace SwedishEconomySDK
{
	public class IncomeTaxConfig
	{
		public double skattesats_kommunalskatt;
		public double skattesats_landstingsskatt;
		public double skattesats_begravning;
		public int birthYear;
		public int taxYear;

		/// <summary>
		/// Prisbasbelopp.
		/// Reference: http://www.scb.se/sv_/Hitta-statistik/Statistik-efter-amne/Priser-och-konsumtion/Konsumentprisindex/Konsumentprisindex-KPI/33772/33779/Behallare-for-Press/406706/
		/// </summary>
		public double PBB;

		/// <summary>
		/// Inkomstbasbeloppet.
		/// Reference: http://www.regeringen.se/artiklar/2015/08/prisbasbeloppet-faststallt/
		/// </summary>
		public double IBB;

		// Could be calculated.
		// Reference: https://www4.skatteverket.se/rattsligvagledning/27071.html?date=2017-01-01#section65-5
		public double nedreSkiktgräns = 438900;
		public double övreSkiktgräns = 638500;

		public double maxPensionsgrundandeInkomst => 8.07 * IBB;
		public double gränsbeloppUtdelningFörenklingsregeln => 2.75 * IBB;
		public double maxSjukpenninggrundandeInkomstFörSjukdom => 7.5 * PBB;

		public double SjukpenningGrundandeInkomst(double årsinkomst)
		{
			return årsinkomst * 0.97;
		}

		public double ErsättningFörSjukdom(double årsinkomst)
		{
			double cappedÅrsinkomst = Math.Min(årsinkomst, maxSjukpenninggrundandeInkomstFörSjukdom);
			double ersättning = SjukpenningGrundandeInkomst(cappedÅrsinkomst) * 0.8;
			return ersättning;
		}
	}

	public struct IncomeTaxResult
	{
		public double kommunalskatt;
		public double statligskatt;
		public double begravningsavgift;
		public double grundavdrag;
		public double beskattningsbarForvärvsinkomst;
		public double jobbskatteavdrag;
		public double allmänPensionsavgift;
		public double skattereduktionFörAllmänPensionsavgift;
		public double slutligSkatt;
		public double effektivTotalSkattesats;
	}

	public class IncomeTaxCalculator
	{
		// Reference: http://www.scb.se/sv_/Hitta-statistik/Statistik-efter-amne/Priser-och-konsumtion/Konsumentprisindex/Konsumentprisindex-KPI/33772/33779/Behallare-for-Press/406706/
		/// <summary>
		/// Prisbasbelopp.
		/// </summary>
		double PBB;

		// Reference: http://www.regeringen.se/artiklar/2015/08/prisbasbeloppet-faststallt/
		/// <summary>
		/// Inkomstbasbeloppet.
		/// </summary>
		double IBB;

		double skattesats_kommunalskatt;
		double skattesats_landstingsskatt;
		double skattesats_begravning;

		/// <summary>
		/// Skattesats för kommunal inkomst.
		/// </summary>
		double skattesats_kommlands { get { return skattesats_kommunalskatt + skattesats_landstingsskatt; } }

		public int ageAtTaxYearStart { get { return taxYear - birthYear; } }

		// Could be calculated. Reference: https://www4.skatteverket.se/rattsligvagledning/27071.html?date=2017-01-01#section65-5
		double nedreSkiktgräns;
		double övreSkiktgräns;
		double skattesats_nedreGräns = 0.20;
		double skattesats_övreGräns = 0.05;

		int birthYear;
		int taxYear;

		public IncomeTaxCalculator(IncomeTaxConfig config)
		{
			this.skattesats_kommunalskatt = config.skattesats_kommunalskatt;
			this.skattesats_landstingsskatt = config.skattesats_landstingsskatt;
			this.skattesats_begravning = config.skattesats_begravning;
			this.birthYear = config.birthYear;
			this.taxYear = config.taxYear;
			this.PBB = config.PBB;
			this.IBB = config.IBB;
			this.nedreSkiktgräns = config.nedreSkiktgräns;
			this.övreSkiktgräns = config.övreSkiktgräns;
		}

		public IncomeTaxResult Calculate(double arbetsinkomst)
		{
			double fastställdFörvärvsinkomst = arbetsinkomst;
			double grundavdrag = Grundavdrag(fastställdFörvärvsinkomst);
			double beskattningsbarFörvärvsinkomst = fastställdFörvärvsinkomst - grundavdrag;
			double allmänPensionsavgift = AllmänPensionsavgift(fastställdFörvärvsinkomst);
			double kommunalSkatt = KommunalSkatt(beskattningsbarFörvärvsinkomst);

			var r = new IncomeTaxResult()
			{
				kommunalskatt = kommunalSkatt,
				statligskatt = StatligSkatt(beskattningsbarFörvärvsinkomst),
				begravningsavgift = Begravningsavgift(beskattningsbarFörvärvsinkomst),
				grundavdrag = grundavdrag,
				beskattningsbarForvärvsinkomst = beskattningsbarFörvärvsinkomst,
				allmänPensionsavgift = allmänPensionsavgift,
				skattereduktionFörAllmänPensionsavgift = SkattereduktionFoerAllmaenPensionsavgift(fastställdFörvärvsinkomst),
				jobbskatteavdrag = Jobbskatteavdrag(arbetsinkomst, grundavdrag, ageAtTaxYearStart, allmänPensionsavgift, kommunalSkatt),
			};

			r.slutligSkatt = r.kommunalskatt + r.statligskatt + r.begravningsavgift - r.jobbskatteavdrag + r.allmänPensionsavgift - r.skattereduktionFörAllmänPensionsavgift;
			r.effektivTotalSkattesats = r.slutligSkatt / arbetsinkomst;

			return r;
		}

		double SkattereduktionFoerAllmaenPensionsavgift(double fastställdFörvärvsinkomst)
		{
			// In most cases the tax reduction for "allmän pensionsavgift" is exactly "allmän pensionsavgift".
			// There are a few cases where the 
			return AllmänPensionsavgift(fastställdFörvärvsinkomst);
		}

		double AllmänPensionsavgift(double fastställdFörvärvsinkomst)
		{
			// Reference: https://www4.skatteverket.se/rattsligvagledning/edition/2017.7/1348.html
			// Reference: https://www4.skatteverket.se/rattsligvagledning/2787.html?date=2015-01-01#section1-3

			if (fastställdFörvärvsinkomst < 0.423 * PBB)
				return 0;

			double cappedFastställdFörvärvsinkomst = Math.Min(fastställdFörvärvsinkomst, 8.07 * IBB);

			// Inkomst av anställning och inkomst av annat förvärvsarbete ska därvid var
			// för sig avrundas till närmast lägre hundratal kronor.
			double roundedDownFFI = RoundDownToNearest100(cappedFastställdFörvärvsinkomst);

			double avgift = 0.07 * roundedDownFFI;

			// Avgiften avrundas till närmast hela hundratal kronor. Avgift som slutar
			// på 50 kronor avrundas till närmast lägre hundratal kronor. Lag (2010:1261).
			bool endsWith50 = (((int)avgift) % 50) == 0;
			if (endsWith50)
				avgift = RoundDownToNearest100(avgift);
			else
				avgift = RoundToNearest100(avgift);

			return avgift;
		}

		double Begravningsavgift(double beskattningsbarFörvärvsinkomst)
		{
			double avgift = Math.Floor(beskattningsbarFörvärvsinkomst * skattesats_begravning);
			return avgift;
		}

		double KommunalSkatt(double beskattningsbarFörvärvsinkomst)
		{
			// Reference: https://www4.skatteverket.se/rattsligvagledning/2477.html?date=2017-01-01#section22-1;
			return Math.Floor((skattesats_kommunalskatt + skattesats_landstingsskatt) * beskattningsbarFörvärvsinkomst);
		}

		double StatligSkatt(double beskattningsbarFörvärvsinkomst)
		{
			// Reference: https://www4.skatteverket.se/rattsligvagledning/edition/2017.7/2930.html
			// Reference: https://www4.skatteverket.se/rattsligvagledning/2477.html?date=2017-01-01#section22-1

			if (beskattningsbarFörvärvsinkomst < 200)
				return 0;

			double bfiÖverNedreSkiktgränsen = Math.Max(0, beskattningsbarFörvärvsinkomst - nedreSkiktgräns);
			double bfiÖverÖvreSkiktgränsen = Math.Max(0, beskattningsbarFörvärvsinkomst - övreSkiktgräns);

			double statligSkatt = Math.Round(skattesats_nedreGräns * bfiÖverNedreSkiktgränsen + skattesats_övreGräns * bfiÖverÖvreSkiktgränsen);
			return statligSkatt;
		}

		double Jobbskatteavdrag(double arbetsinkomst, double grundavdrag, int ageAtTaxYearStart, double allmänPensionsavgift, double kommunalskatt)
		{
			double jobbskatteavdrag;

			if (ageAtTaxYearStart < 65)
			{
				double AI = arbetsinkomst;
				double GA = grundavdrag;

				// Reference: https://www4.skatteverket.se/rattsligvagledning/edition/2017.7/2940.html

				if (AI < 0.91 * PBB)
					jobbskatteavdrag = (AI - GA) * skattesats_kommlands;
				else if (AI >= 0.91 * PBB && AI < 2.94 * PBB)
					jobbskatteavdrag = (0.91 * PBB + 0.332 * (AI - 0.91 * PBB) - GA) * skattesats_kommlands;
				else if (AI >= 2.94 * PBB && AI < 8.08 * PBB)
					jobbskatteavdrag = (1.584 * PBB + 0.111 * (AI - 2.94 * PBB) - GA) * skattesats_kommlands;
				else if (AI >= 8.08 * PBB && AI <= 13.54 * PBB)
					jobbskatteavdrag = (2.155 * PBB - GA) * skattesats_kommlands;
				else if (AI >= 13.54 * PBB)
					jobbskatteavdrag = (2.155 * PBB - GA) * skattesats_kommlands - 0.03 * (AI - 13.54 * PBB);
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

		double Grundavdrag(double fastställdFörvärvsinkomst)
		{
			// Reference: https://www4.skatteverket.se/rattsligvagledning/27071.html?date=2017-01-01#section63-3
			// Reference: https://www.skatteverket.se/privat/skatter/arbeteochinkomst/skattetabeller/grundavdrag.4.6d02084411db6e252fe80009078.html

			double ffi = fastställdFörvärvsinkomst;

			double grundavdrag;

			if (ffi <= 0.99 * PBB)
				grundavdrag = 0.423 * PBB;
			else if (ffi <= 2.72 * PBB)
				grundavdrag = 0.423 * PBB + 0.20 * (ffi - 0.99 * PBB);
			else if (ffi <= 3.11 * PBB)
				grundavdrag = 0.77 * PBB;
			else if (ffi <= 7.88 * PBB)
				grundavdrag = 0.77 * PBB - 0.10 * (ffi - 3.11 * PBB);
			else
				grundavdrag = 0.293 * PBB;

			grundavdrag = RoundUpToNearest100(grundavdrag);
			return Math.Min(grundavdrag, fastställdFörvärvsinkomst);
		}

		double RoundDownToNearest100(double value)
		{
			return Math.Floor(value / 100) * 100;
		}

		double RoundUpToNearest100(double value)
		{
			return Math.Ceiling(value / 100) * 100;
		}

		double RoundToNearest100(double value)
		{
			return Math.Round(value / 100) * 100;
		}
	}
}
