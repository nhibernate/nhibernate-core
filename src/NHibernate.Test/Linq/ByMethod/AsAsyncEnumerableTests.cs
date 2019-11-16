using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class AsAsyncEnumerableTests : LinqTestCase
	{
		[Test]
		public void MultipleEnumerationTest()
		{
			var enumerable = db.Customers.Where(c => c.Address.City == "London").OrderBy(c => c.CustomerId).AsEnumerable();
			for (var i = 0; i < 3; i++)
			{
				AssertByIds(enumerable, new[]
							   {
								   "AROUT",
								   "BSBEV",
								   "CONSH",
								   "EASTC",
								   "NORTS",
								   "SEVES"
							   }, x => x.CustomerId);
			}
		}
	}
}
