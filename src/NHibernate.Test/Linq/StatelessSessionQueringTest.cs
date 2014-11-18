using System.Linq;
using System.Text;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.Linq
{
	public class StatelessSessionQueringTest : LinqTestCase
	{
		[Test]
		public void WhenQueryThroughStatelessSessionThenDoesNotThrows()
		{
			using (var statelessSession = Sfi.OpenStatelessSession())
			{
				var query = statelessSession.Query<Customer>();
				Assert.That(() => query.ToList(), Throws.Nothing);
			}
		}

		[Test]
		public void AggregateWithStartsWith()
		{
			using (IStatelessSession statelessSession = Sfi.OpenStatelessSession())
			{
				StringBuilder query = (from c in statelessSession.Query<Customer>() where c.CustomerId.StartsWith("A") select c.CustomerId)
					.Aggregate(new StringBuilder(), (sb, id) => sb.Append(id).Append(","));
				Assert.That(query.ToString(), Is.EqualTo("ALFKI,ANATR,ANTON,AROUT,"));
			}
		}
	}
}