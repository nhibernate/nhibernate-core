using System.Linq;
using System.Threading;
using System.Transactions;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1594
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(ISessionFactoryImplementor factory) =>
			factory.ConnectionProvider.Driver.SupportsSystemTransactions;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new GH1594.Entity {Name = "Bob"};
				session.Save(e1);

				var e2 = new GH1594.Entity {Name = "Sally"};
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHbernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void ExecutionContextLocalValuesLeak()
		{
			using (var session = OpenSession())
			{
				RunInTransaction(session);
				var localValuesCountAfterFirstCall = ExecutionContext.Capture().LocalValuesCount();
				if (!localValuesCountAfterFirstCall.HasValue)
					Assert.Ignore("Unable to get async local values count");
				RunInTransaction(session);
				var localValuesCountAfterSecondCall = ExecutionContext.Capture().LocalValuesCount();
				if (!localValuesCountAfterSecondCall.HasValue)
					Assert.Ignore("Unable to get async local values count");

				Assert.AreEqual(localValuesCountAfterFirstCall, localValuesCountAfterSecondCall);
			}
		}

		private void RunInTransaction(ISession session)
		{
			using (var ts = new TransactionScope())
			{
				var result = from e in session.Query<GH1594.Entity>()
							 where e.Name == "Bob"
							 select e;

				result.ToList();
				ts.Complete();
			}
		}
	}
}
