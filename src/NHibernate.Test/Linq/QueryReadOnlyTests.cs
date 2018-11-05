using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class QueryReadOnlyTests : LinqTestCase
	{
		[Test]
		public void CanSetReadOnlyOnLinqQueries()
		{
			var result = (from e in db.Customers
			              where e.CompanyName == "Bon app'"
			              select e).WithOptions(o => o.SetReadOnly(true)).ToList();

			Assert.That(result.All(x => session.IsReadOnly(x)), Is.True);
		}

		[Test]
		public void CanSetReadOnlyOnLinqPagingQuery()
		{
			var result = (from e in db.Customers
			              select e).Skip(1).Take(1).WithOptions(o => o.SetReadOnly(true)).ToList();

			Assert.That(result.All(x => session.IsReadOnly(x)), Is.True);
		}

		[Test]
		public void CanSetReadOnlyBeforeSkipOnLinqOrderedPageQuery()
		{
			var result = (from e in db.Customers
			              orderby e.CompanyName
			              select e).WithOptions(o => o.SetReadOnly(true)).Skip(5).Take(5).ToList();

			Assert.That(result.All(x => session.IsReadOnly(x)), Is.True);
		}
	}
}
