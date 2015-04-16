using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public class QueryReadOnlyTests : LinqTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
		}

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
	}
}