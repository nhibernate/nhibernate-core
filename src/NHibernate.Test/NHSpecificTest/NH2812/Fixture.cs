using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2812
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = new EntityWithAByteValue {ByteValue = 1};
				session.Save(entity);
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from EntityWithAByteValue");
				tx.Commit();
			}
		}

		[Test]
		public void PerformingAQueryOnAByteColumnShouldNotThrowEqualityOperator()
		{
			using (var session = sessions.OpenSession())
			{
				var query = (from e in session.Query<EntityWithAByteValue>()
							 where e.ByteValue == 1
							 select e)
							 .ToList();

				// this should not fail if fixed
				Assert.AreEqual(1, query.Count);
			}
		}

		[Test]
		public void PerformingAQueryOnAByteColumnShouldNotThrowEquals()
		{
			using (var session = sessions.OpenSession())
			{
				var query = (from e in session.Query<EntityWithAByteValue>()
							 where e.ByteValue.Equals(1)
							 select e)
							 .ToList();

				Assert.AreEqual(1, query.Count);
			}
		}
	}
}
