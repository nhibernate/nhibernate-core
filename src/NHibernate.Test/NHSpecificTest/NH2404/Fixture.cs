using System.Linq;
using NHibernate.Linq;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2404
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new TestEntity
					{
						Id = 1,
						Name = "Test Entity"
					});

				session.Save(new TestEntity
					{
						Id = 2,
						Name = "Test Entity"
					});

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
		public void ProjectionsShouldWorkWithLinqProviderAndFutures()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var query1 = (from entity in session.Query<TestEntity>()
							  select new TestEntityDto {EntityId = entity.Id, EntityName = entity.Name}).ToList();

				Assert.AreEqual(2, query1.Count());

				var query2 = (from entity in session.Query<TestEntity>()
							  select new TestEntityDto {EntityId = entity.Id, EntityName = entity.Name}).ToFuture();

				Assert.AreEqual(2, query2.Count());
			}
		}

		[Test]
		public void ProjectionsShouldWorkWithHqlAndFutures()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var query1 =
					session.CreateQuery("select e.Id as EntityId, e.Name as EntityName from TestEntity e").SetResultTransformer(
						Transformers.AliasToBean(typeof (TestEntityDto)))
						.List<TestEntityDto>();

				Assert.AreEqual(2, query1.Count());

				var query2 =
					session.CreateQuery("select e.Id as EntityId, e.Name as EntityName from TestEntity e").SetResultTransformer(
						Transformers.AliasToBean(typeof (TestEntityDto)))
						.Future<TestEntityDto>();

				Assert.AreEqual(2, query2.Count());
			}
		}
	}
}
