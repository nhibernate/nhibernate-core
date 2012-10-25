using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3304
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void NonDbGeneratedPoidEntityShouldRollback()
		{
			var rollbackEntity = new Entity {Id=13, Name = "rollback"};
			var committedEntity = new Entity {Id = 15, Name = "commited"};
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.Save(rollbackEntity);
					tx.Rollback();
				}
				using (var tx = s.BeginTransaction())
				{
					s.Save(committedEntity);
					tx.Commit();
				}
			}
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.CreateQuery("select count(*) from Entity e").UniqueResult<long>()
						.Should().Be.EqualTo(1);					
				}
			}
		}
	}
}