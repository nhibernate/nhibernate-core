using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1920
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Guid entityId;
		private Guid someOtherEntityId;
	
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				entityId = (Guid) session.Save(new EntityWithBatchSize {Name = "some name"});
				someOtherEntityId = (Guid) session.Save(new EntityWithBatchSize());

				transaction.Commit();
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void CanLoadEntity(bool loadProxyOfOtherEntity)
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				if(loadProxyOfOtherEntity)
					session.Load<EntityWithBatchSize>(someOtherEntityId);
				
				var result = session.Get<EntityWithBatchSize>(entityId);

				Assert.That(result.Name, Is.Not.Null);
			}
		}
		
		
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from EntityWithBatchSize").ExecuteUpdate();
				transaction.Commit();
			}
		}
	}
}
