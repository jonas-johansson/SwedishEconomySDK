using System;
using System.Collections.Generic;
using System.Text;

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

		/// <summary>
		/// Skattesats för kommunal inkomst.
		/// </summary>
		public double skattesats_kommlands { get { return skattesats_kommunalskatt + skattesats_landstingsskatt; } }

		public int AgeAtTaxYearStart { get { return taxYear - birthYear; } }

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
}
