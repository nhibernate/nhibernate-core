using System.Linq;
using NHibernate.Impl;
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
			base.OnSetUp();
			using (var session = this.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = new TestEntity();
				entity.Id = 1;
				entity.Name = "Test Entity";
				session.Save(entity);
				
				var entity1 = new TestEntity();
				entity1.Id = 2;
				entity1.Name = "Test Entity";
				session.Save(entity1);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = this.OpenSession())
			{
				string hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}
	
		[Test]
		public void ProjectionsShouldWorkWithLinqProviderAndFutures()
		{
			using (ISession session = this.OpenSession())
			{
				if (((SessionFactoryImpl)sessions).ConnectionProvider.Driver.SupportsMultipleQueries == false)
				{
					Assert.Ignore("Not applicable for dialects that do not support multiple queries");
				}

				var query1 = (
				             	from entity in session.Query<TestEntity>()
				             	select new TestEntityDto {EntityId = entity.Id, EntityName = entity.Name}
				             ).ToList();

				Assert.AreEqual(2, query1.Count());

				var query2 = (
							from entity in session.Query<TestEntity>()
							select new TestEntityDto { EntityId = entity.Id, EntityName = entity.Name }
						).ToFuture();

				Assert.AreEqual(2, query2.Count());
			}
		}

		[Test]
		public void ProjectionsShouldWorkWithHqlAndFutures()
		{
			using (ISession session = this.OpenSession())
			{
				if (((SessionFactoryImpl)sessions).ConnectionProvider.Driver.SupportsMultipleQueries == false)
				{
					Assert.Ignore("Not applicable for dialects that do not support multiple queries");
				}

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
