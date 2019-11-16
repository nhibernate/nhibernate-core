using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class AsAsyncEnumerableTests : LinqTestCase
	{
		[Test]
		public async Task MultipleEnumerationTest()
		{
			var asyncEnumerable = db.Customers.Where(c => c.Address.City == "London").OrderBy(c => c.CustomerId).AsAsyncEnumerable();
			for (var i = 0; i < 3; i++)
			{
				await AssertByIds(asyncEnumerable, new[]
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
