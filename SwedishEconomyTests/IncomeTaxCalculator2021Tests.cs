using NUnit.Framework;
using SwedishEconomySDK;

namespace SwedishEconomyTests
{
	[TestFixture]
	public class IncomeTaxCalculator2021Tests
	{
		// The expected values for the tests were calculated using Skatteverkets service https://app.skatteverket.se/rakna-skatt-client-skut-skatteutrakning/rakna-ut-skatt/resultat

		IncomeTaxConfig Get2021TestConfig()
		{
			return new IncomeTaxConfig()
			{
				skattesats_kommunalskatt = 17.74 / 100,
				skattesats_landstingsskatt = 12.08 / 100,
				skattesats_begravning = 0.065 / 100,

				taxYear = 2021,
				PBB = 47_600,
				IBB = 68_200,
				nedreSkiktgräns = 523_200,

				birthYear = 1980,
			};
		}

		IncomeTaxResult GetResultFor2021(double arbetsinkomst)
		{
			return new IncomeTaxCalculator(Get2021TestConfig()).Calculate(arbetsinkomst);
		}

		[Test]
		public void Income_100_000()
		{
			var result = GetResultFor2021(100_000);
			Assert.AreEqual(30_800, result.grundavdrag);
			Assert.AreEqual(69_200, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(20_635, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(44, result.begravningsavgift);
			Assert.AreEqual(7000, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(9487, result.jobbskatteavdrag);
			Assert.AreEqual(219, result.skattereduktionFörFörvärvsinkomst);
			Assert.AreEqual(692, result.publicServiceAvgift);
			Assert.AreEqual(11665, result.slutligSkatt);
		}

		[Test]
		public void Income_537_200()
		{
			var result = GetResultFor2021(537_200);
			Assert.AreEqual(14000, result.grundavdrag);
			Assert.AreEqual(523200, result.beskattningsbarForvärvsinkomst);
			Assert.AreEqual(156018, result.kommunalskatt);
			Assert.AreEqual(0, result.statligskatt);
			Assert.AreEqual(340, result.begravningsavgift);
			Assert.AreEqual(37600, result.skattereduktionFörAllmänPensionsavgift);
			Assert.AreEqual(28798, result.jobbskatteavdrag);
			Assert.AreEqual(1500, result.skattereduktionFörFörvärvsinkomst);
			Assert.AreEqual(1329, result.publicServiceAvgift);
			Assert.AreEqual(127389, result.slutligSkatt);
		}

		//[Test]
		//public void Income_564_000()
		//{
		//	var result = GetResultFor2021(564_000);
		//	Assert.AreEqual(14_000, result.grundavdrag);
		//	Assert.AreEqual(550_000, result.beskattningsbarForvärvsinkomst);
		//	Assert.AreEqual(164_010, result.kommunalskatt);
		//	Assert.AreEqual(5_360, result.statligskatt);
		//	Assert.AreEqual(140_758, result.slutligSkatt);
		//}

		[Test]
		public void MaximaltUtnyttjande_PensionsgrundandeInkomst()
		{
			Assert.AreEqual(550_374, Get2021TestConfig().maxPensionsgrundandeInkomst);
		}

		[Test]
		public void MaximaltUtnyttjande_GränsFörStatligSkatt()
		{
			var calc = new IncomeTaxCalculator(Get2021TestConfig());

			// No state tax below.
			Assert.AreEqual(0, calc.Calculate(537_200).statligskatt);

			// State tax above.
			Assert.AreNotEqual(0, calc.Calculate(537_200 + 100).statligskatt);
		}
	}
}
