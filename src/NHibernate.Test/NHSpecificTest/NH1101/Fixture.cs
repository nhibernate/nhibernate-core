using NHibernate.Stat;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1101
{
	// http://nhibernate.jira.com/browse/NH-1101
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			cfg.SetProperty(Cfg.Environment.GenerateStatistics, "true");
		}

		[Test]
		public void Behavior()
		{
			object savedId;
			A a = new A();
			using (ISession s = OpenSession())
			using(ITransaction t = s.BeginTransaction())
			{
				savedId = s.Save(a);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				a = s.Get<A>(savedId);

				IStatistics statistics = Sfi.Statistics;
				statistics.Clear();

				Assert.IsNotNull(a.B); // an instance of B was created
				s.Flush();
				t.Commit();

				// since we don't change anyproperties in a.B there are no dirty entity to commit
				Assert.AreEqual(0, statistics.PrepareStatementCount);
			}

			// using proxy
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				a = s.Load<A>(savedId);

				IStatistics statistics = Sfi.Statistics;
				statistics.Clear();

				Assert.IsNotNull(a.B); // an instance of B was created
				s.Flush();
				t.Commit();

				Assert.AreEqual(0, statistics.PrepareStatementCount);
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from A");
				t.Commit();
			}
		}
	}
}