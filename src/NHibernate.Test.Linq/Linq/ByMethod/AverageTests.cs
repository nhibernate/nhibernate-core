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
			if (Dialect is Oracle8iDialect)
			{
				// The point of this test is to verify that LINQ's Average over an
				// integer columns yields a non-integer result, even on databases
				// where the corresponding avg() will yield a return type equal to
				// the input type. This means the LINQ provider must generate HQL
				// that cast the input to double inside the call to avg(). This works
				// fine on most databases, but Oracle causes trouble.
				//
				// The dialect maps double to "DOUBLE PRECISION" on Oracle, which
				// on Oracle has larger precision than C#'s double. When such
				// values are returned, ODP.NET will convert it to .Net decimal, which
				// has lower precision and thus causes an overflow exception.
				//
				// Some argue that this is a flaw in ODP.NET, others have created
				// local dialects that use e.g. BINARY_DOUBLE instead, which more
				// closely matches C#'s IEEE 745 double, see e.g. HHH-1961 and
				// serveral blogs.

				Assert.Ignore("Not supported on Oracle due to casting/overflow issues.");
			}

			//NH-2429
			var average = db.Products.Average(x => x.UnitsOnOrder);

			Assert.AreEqual(average, 10.129870d, 0.000001d);
		}
	}
}