using System;
using System.Collections.Generic;
using System.Text;

namespace SwedishEconomySDK
{
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
		public double skattereduktionFörFörvärvsinkomst;
		public double slutligSkatt;
		public double effektivTotalSkattesats;
		public double publicServiceAvgift;
	}
}
