using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3666
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = this.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entity1 = new Entity { Id = 1, Property = "Test1" };
				var entity2 = new Entity { Id = 2, Property = "Test2" };
				var entity3 = new Entity { Id = 3, Property = "Test3" };

				session.Save(entity1);
				session.Save(entity2);
				session.Save(entity3);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = this.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from Entity");
				transaction.Commit();
			}
		}

		[Test]
		public void CacheableDoesNotThrowExceptionWithNativeSQLQuery()
		{
			using (var session = this.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = session.CreateSQLQuery("SELECT * FROM Entity WHERE Property = 'Test2'")
				                    .AddEntity(typeof(Entity))
				                    .SetCacheable(true)
				                    .List<Entity>();

				CollectionAssert.IsNotEmpty(result);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(2, result[0].Id);
			}
		}

		[Test]
		public void CacheableDoesNotThrowExceptionWithNamedQuery()
		{
			using (var session = this.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = session.GetNamedQuery("QueryName")
				                    .SetCacheable(true)
				                    .SetString("prop", "Test2")
				                    .List<Entity>();

				CollectionAssert.IsNotEmpty(result);

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(2, result[0].Id);
			}
		}
	}
}
