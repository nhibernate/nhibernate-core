using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2378
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
				entity.TestPerson = new Person { Id = 1, Name = "TestUser" };
				session.Save(entity);
				
				var entity1 = new TestEntity();
				entity1.Id = 2;
				entity1.Name = "Test Entity";
				entity1.TestPerson = new Person { Id = 2, Name = "TestUser" };
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
		public void ShortEntityCanBeQueryCorrectlyUsingLinqProvider()
		{
			using (ISession session = this.OpenSession())
			{
				IQueryable<TestEntity> query = session.Query<TestEntity>();
				IQueryable<TestEntityDto> list = query.Select(o => new TestEntityDto
				                                                    	{
				                                                    		EntityId = o.Id,
				                                                    		EntityName = o.Name,
				                                                    		PersonId =
				                                                    			(o.TestPerson != null)
				                                                    				? o.TestPerson.Id
				                                                    				: (short) 0,
				                                                    		PersonName =
				                                                    			(o.TestPerson != null)
				                                                    				? o.TestPerson.Name
				                                                    				: string.Empty
				                                                    	});

				IQueryable<TestEntityDto> m = list.Where(o => o.PersonId == 2);

            	
				Assert.AreEqual(1, m.Count());
			}
		}
	}
}