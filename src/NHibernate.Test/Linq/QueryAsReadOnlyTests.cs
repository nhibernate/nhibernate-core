using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public class QueryAsReadOnlyTests : LinqTestCase
	{
		[Test]
		public void CanSetReadonlyOnLinqQueries()
		{
			//NH-3658
			var result = (from e in db.Customers
						  where e.CompanyName == "Corp"
						  select e).AsReadOnly().ToList();

			Assert.IsTrue(result.All(x => session.IsReadOnly(x)));
		}
	}
}
