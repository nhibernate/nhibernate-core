using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3247
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob", Initial = 'B' };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally", Initial = 'S' };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CharParameterValueShouldNotBeCached()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
					.Where(e => e.Initial == 'B')
					.Single();

				Assert.AreEqual('B', result.Initial);

				result = session.Query<Entity>()
					.Where(e => e.Initial == 'S')
					.Single();

				Assert.AreEqual('S', result.Initial);
			}
		}
	}
}
