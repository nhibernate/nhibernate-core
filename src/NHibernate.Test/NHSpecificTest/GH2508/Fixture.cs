using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2508
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			var listeners = configuration.EventListeners;
			listeners.PreCollectionUpdateEventListeners =
				new[] {new AuditEventListener()}
					.Concat(listeners.PreCollectionUpdateEventListeners)
					.ToArray();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new LoggerCase {Name = "Bob"};
				session.Save(e1);

				var e2 = new LoggerCase {Name = "Sally"};
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}

		[Test]
		public void TestPreCollectionUpdateEvent()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = (from e in session.Query<LoggerCase>()
							  where e.Name == "Bob"
							  select e).First();

				result.Children.Add(new Child { Logger = result, Name = "child" });
				session.SaveOrUpdate(result);
				transaction.Commit();
			}
		}
	}
}
