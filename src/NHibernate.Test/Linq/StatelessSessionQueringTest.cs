using System.Linq;
using System.Text;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using NHibernate.Linq;
using SharpTestsEx;

namespace NHibernate.Test.Linq
{
	public class StatelessSessionQueringTest : LinqTestCase
	{
		[Test]
		public void CanQueryChildStatelessSession()
		{
			//NH-3606
			using (var session = this.OpenSession())
			{
				Assert.AreEqual(0, session.GetSessionImplementation().PersistenceContext.EntityEntries.Count);

				using (var statelessSession = session.GetStatelessSession())
				{
					var results = statelessSession.Query<Customer>().ToList();

					Assert.IsNotEmpty(results);
				}

				Assert.AreEqual(0, session.GetSessionImplementation().PersistenceContext.EntityEntries.Count);
			}
		}

		[Test]
		public void CanCreateStatelessSessions()
		{
			//NH-3606
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				Assert.IsTrue(session.Transaction.IsActive);

				using (var statelessSession = session.GetStatelessSession())
				{
					Assert.IsTrue(statelessSession.Transaction.IsActive);

					statelessSession.Transaction.Rollback();
				}

				Assert.IsFalse(session.Transaction.IsActive);
			}
		}

		[Test]
		public void CanApplyAsStatelessExtensionMethod()
		{
			//NH-3606
			using (var session = OpenSession())
			{
				Assert.AreEqual(0, session.GetSessionImplementation().PersistenceContext.EntityEntries.Count);

				var results = session.Query<Customer>().AsStateless().ToList();

				Assert.IsNotEmpty(results);

				Assert.AreEqual(0, session.GetSessionImplementation().PersistenceContext.EntityEntries.Count);
			}
		}

		[Test]
		public void ExplicitlyFetchedLazyMembersAreNotCached()
		{
			//NH-3606
			using (var session = OpenSession())
			{
				Assert.AreEqual(0, session.GetSessionImplementation().PersistenceContext.EntityEntries.Count);

				var results = session.Query<Customer>().AsStateless().Fetch(x => x.Orders).ToList();

				Assert.IsNotEmpty(results);

				Assert.AreEqual(0, session.GetSessionImplementation().PersistenceContext.EntityEntries.Count);
			}
		}

		[Test]
		public void WhenQueryThroughStatelessSessionThenDoesNotThrows()
		{
			using (var statelessSession = Sfi.OpenStatelessSession())
			{
				var query = statelessSession.Query<Customer>();
				query.Executing(q => q.ToList()).NotThrows();
			}
		}

		[Test]
		public void AggregateWithStartsWith()
		{
			using (IStatelessSession statelessSession = Sfi.OpenStatelessSession())
			{
				StringBuilder query = (from c in statelessSession.Query<Customer>() where c.CustomerId.StartsWith("A") select c.CustomerId)
					.Aggregate(new StringBuilder(), (sb, id) => sb.Append(id).Append(","));
				query.ToString().Should().Be("ALFKI,ANATR,ANTON,AROUT,");
			}
		}
	}
}