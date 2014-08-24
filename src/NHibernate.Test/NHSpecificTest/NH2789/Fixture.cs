using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2789
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Save(new EntityWithAByteValue {ByteValue = null});
				session.Save(new EntityWithAByteValue {ByteValue = 1});
				session.Save(new EntityWithAByteValue {ByteValue = 2});
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
		public void EqualityOperator()
		{
			using (var session = OpenSession())
			{
				var query = (from e in session.Query<EntityWithAByteValue>()
							 where e.ByteValue == 1
							 select e)
							 .ToList();

				Assert.AreEqual(1, query.Count);
			}
		}

		[Test]
		public void EqualityOperatorNull()
		{
			using (var session = OpenSession())
			{
				var query = (from e in session.Query<EntityWithAByteValue>()
							 where e.ByteValue == null
							 select e)
							 .ToList();

				Assert.AreEqual(1, query.Count);
			}
		}

		[Test]
		public void EqualityOperatorNotNull()
		{
			using (var session = OpenSession())
			{
				var query = (from e in session.Query<EntityWithAByteValue>()
							 where e.ByteValue != null
							 select e)
							 .ToList();

				Assert.AreEqual(2, query.Count);
			}
		}
	}
}
