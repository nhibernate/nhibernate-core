using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public class QueryReadOnlyTests : LinqTestCase
	{
		[Test]
		public void CanSetReadOnlyOnLinqQueries()
		{
			var result = (from e in db.Customers
						  where e.CompanyName == "Bon app'"
						  select e).AsReadOnly().ToList();

			Assert.That(result.All(x => this.session.IsReadOnly(x)), Is.True);
		}


		[Test]
		public void CanSetReadOnlyOnLinqPagingQuery()
		{
			var result = (from e in db.Customers
						  select e).Skip(1).Take(1).AsReadOnly().ToList();

			Assert.That(result.All(x => this.session.IsReadOnly(x)), Is.True);
		}


		[Test]
		public void CanSetReadOnlyBeforeSkipOnLinqOrderedPageQuery()
		{
			var result = (from e in db.Customers
						  orderby e.CompanyName
						  select e)
				.AsReadOnly().Skip(5).Take(5).ToList();

			Assert.That(result.All(x => this.session.IsReadOnly(x)), Is.True);
		}
		
		[Test]
		public void CanSetReadOnlyOnLinqGroupPageQuery()
		{
			var subQuery = db.Customers.Where(e2 => e2.CompanyName.Contains("a")).Select(e2 => e2.CustomerId)
							 .AsReadOnly(); // This AsReadOnly() should not cause trouble, and be ignored.

			var result = (from e in db.Customers
						  where subQuery.Contains(e.CustomerId)
						  group e by e.CompanyName
							  into g
							  select new { g.Key, Count = g.Count() })
				.Skip(5).Take(5)
				.AsReadOnly().ToList();

			Assert.That(result.All(x => this.session.IsReadOnly(x)), Is.True);
		}
	}
}
