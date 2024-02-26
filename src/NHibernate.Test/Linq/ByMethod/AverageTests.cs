using System.Linq;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class AverageTests : LinqTestCase
	{
		[Test]
		public void CanGetAverageOfIntegersAsDouble()
		{
			//NH-2429
			var average = db.Products.Average(x => x.UnitsOnOrder);

			Assert.AreEqual(10.129870d, average, 0.000001d);
		}
	}
}
