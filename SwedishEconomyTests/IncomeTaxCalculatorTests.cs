using NUnit.Framework;
using SwedishEconomySDK;

namespace SwedishEconomyTests
{
	[TestFixture]
	public class IncomeTaxCalculatorTests
	{
		// The expected values for the tests were calculated using Skatteverkets service https://www.skatteverket.se/webdav/files/servicetjanster/skatteutrakning2017/prelskut17ink1.html

		IncomeTaxConfig Get2017TestConfig()
		{
			return new IncomeTaxConfig()
			{
				skattesats_kommunalskatt = 17.90 / 100,
				skattesats_landstingsskatt = 12.08 / 100,
				skattesats_begravning = 0.075 / 100,
				birthYear = 1980,
				taxYear = 2017,
				PBB = 44800,
				IBB = 61500
			};
		}

		IncomeTaxResult GetResultFor2017(double arbetsinkomst)
		{
			return new IncomeTaxCalculator(Get2017TestConfig()).Calculate(arbetsinkomst);
		}

		[Test]
		public void Income_1_000()
		{
			var result = GetResultFor2017(1000);
			Assert.AreEqual(1000, result.grundavdrag);
			Assert.AreEqual(0, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(0, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(0, result.allmänPensionsavgift);
			Assert.AreEqual(0, result.begravningsavgift);
			Assert.AreEqual(0, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(0, result.jobbskatteavdrag);
			Assert.AreEqual(0, result.slutligSkatt);
		}

		[Test]
		public void Income_10_000()
		{
			var result = GetResultFor2017(10000);
			Assert.AreEqual(10000, result.grundavdrag);
			Assert.AreEqual(0, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(0, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(0, result.allmänPensionsavgift);
			Assert.AreEqual(0, result.begravningsavgift);
			Assert.AreEqual(0, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(0, result.jobbskatteavdrag);
			Assert.AreEqual(0, result.slutligSkatt);
		}

		[Test]
		public void Income_29_000()
		{
			var result = GetResultFor2017(29000);
			Assert.AreEqual(19000, result.grundavdrag);
			Assert.AreEqual(10000, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(2998, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(2000, result.allmänPensionsavgift);
			Assert.AreEqual(7, result.begravningsavgift);
			Assert.AreEqual(2000, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(998, result.jobbskatteavdrag);
			Assert.AreEqual(2007, result.slutligSkatt);
		}

		[Test]
		public void Income_80_000()
		{
			var result = GetResultFor2017(80000);
			Assert.AreEqual(26100, result.grundavdrag);
			Assert.AreEqual(53900, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(16159, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(5600, result.allmänPensionsavgift);
			Assert.AreEqual(40, result.begravningsavgift);
			Assert.AreEqual(5600, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(8302, result.jobbskatteavdrag);
			Assert.AreEqual(7897, result.slutligSkatt);
		}

		[Test]
		public void Income_150_000()
		{
			var result = GetResultFor2017(150000);
			Assert.AreEqual(33500, result.grundavdrag);
			Assert.AreEqual(116500, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(34926, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(10500, result.allmänPensionsavgift);
			Assert.AreEqual(87, result.begravningsavgift);
			Assert.AreEqual(10500, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(11840, result.jobbskatteavdrag);
			Assert.AreEqual(23173, result.slutligSkatt);
		}

		[Test]
		public void Income_134_000()
		{
			var result = GetResultFor2017(134000);
			Assert.AreEqual(34500, result.grundavdrag);
			Assert.AreEqual(99500, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(29830, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(9400, result.allmänPensionsavgift);
			Assert.AreEqual(74, result.begravningsavgift);
			Assert.AreEqual(9400, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(11007, result.jobbskatteavdrag);
			Assert.AreEqual(18897, result.slutligSkatt);
		}

		[Test]
		public void Income_300_000()
		{
			var result = GetResultFor2017(300000);
			Assert.AreEqual(18500, result.grundavdrag);
			Assert.AreEqual(281500, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(84393, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(21000, result.allmänPensionsavgift);
			Assert.AreEqual(211, result.begravningsavgift);
			Assert.AreEqual(21000, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(21328, result.jobbskatteavdrag);
			Assert.AreEqual(63276, result.slutligSkatt);
		}

		[Test]
		public void Income_500_000()
		{
			var result = GetResultFor2017(500000);
			Assert.AreEqual(13200, result.grundavdrag);
			Assert.AreEqual(486800, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(145942, result.kommunalskatt);
			Assert.AreEqual(9580, result.statligskatt);
			Assert.AreEqual(34700, result.allmänPensionsavgift);
			Assert.AreEqual(365, result.begravningsavgift);
			Assert.AreEqual(34700, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(24986, result.jobbskatteavdrag);
			Assert.AreEqual(130901, result.slutligSkatt);
		}

		[Test]
		public void Income_600_000()
		{
			var result = GetResultFor2017(600000);
			Assert.AreEqual(13200, result.grundavdrag);
			Assert.AreEqual(586800, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(175922, result.kommunalskatt);
			Assert.AreEqual(29580, result.statligskatt);
			Assert.AreEqual(34700, result.allmänPensionsavgift);
			Assert.AreEqual(440, result.begravningsavgift);
			Assert.AreEqual(34700, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(24986, result.jobbskatteavdrag);
			Assert.AreEqual(180956, result.slutligSkatt);
		}

		[Test]
		public void Income_3_000_000()
		{
			var result = GetResultFor2017(3000000);
			Assert.AreEqual(13200, result.grundavdrag);
			Assert.AreEqual(2986800, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(895442, result.kommunalskatt);
			Assert.AreEqual(626995, result.statligskatt);
			Assert.AreEqual(34700, result.allmänPensionsavgift);
			Assert.AreEqual(2240, result.begravningsavgift);
			Assert.AreEqual(34700, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(0, result.jobbskatteavdrag);
			Assert.AreEqual(1524677, result.slutligSkatt);
		}

		[Test]
		public void MaximaltUtnyttjande_PensionsgrundandeInkomst()
		{
			Assert.AreEqual(496305, Get2017TestConfig().maxPensionsgrundandeInkomst);
		}

		[Test]
		public void MaximaltUtnyttjande_SjukpenningGrundandeInkomstFörSjukdom()
		{
			Assert.AreEqual(336000, Get2017TestConfig().maxSjukpenninggrundandeInkomstFörSjukdom);
		}

		[Test]
		public void MaximaltUtnyttjande_GränsFörStatligSkatt()
		{
			var calc = new IncomeTaxCalculator(Get2017TestConfig());

			// No state tax below.
			Assert.AreEqual(0, calc.Calculate(452100).statligskatt);

			// State tax above.
			Assert.AreNotEqual(0, calc.Calculate(452100 + 100).statligskatt);
		}
	}
}
