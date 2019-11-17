using System.Linq;
using System.Threading.Tasks;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	[TestFixture]
	public class AsAsyncEnumerableTests : LinqTestCase
	{
		[Test]
		public async Task MultipleEnumerationTestAsync()
		{
			var asyncEnumerable = db.Customers.Where(c => c.Address.City == "London").OrderBy(c => c.CustomerId).AsAsyncEnumerable();
			for (var i = 0; i < 3; i++)
			{
				await AssertByPropertyValueAsync(asyncEnumerable, new[]
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

		[Test]
		public async Task PolymorphismTestAsync()
		{
			var asyncEnumerable = session.Query<INamedEntity>().OrderBy(o => o.Name).AsAsyncEnumerable();
			for (var i = 0; i < 3; i++)
			{
				await AssertByPropertyValueAsync(asyncEnumerable, new[]
							   {
								   "Admin",
								   "User",
								   "ayende",
								   "nhibernate",
								   "rahien"
							   }, x => x.Name);
			}
		}
	}
}
