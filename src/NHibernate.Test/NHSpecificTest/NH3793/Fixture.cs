using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3793
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			// No-op
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
		public void CompositeIdWithKeyManyToOneUsesEntityName()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				session.Query<ObjectWithBothEntityNames>()
					   .SelectMany(o => o.ParentEntityOneSet.Select(p => p.Children));

				// Previously, there would have been an exception before reaching this point.
				Assert.IsTrue(true);
			}
		}
	}
}