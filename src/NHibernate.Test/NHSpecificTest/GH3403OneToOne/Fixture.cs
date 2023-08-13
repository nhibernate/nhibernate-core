using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3403OneToOne
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		private Guid _id;

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			var entity = new Entity1
			{
				Child = new Entity2()
			};

			entity.Child.Parent = entity;

			session.Save(entity);
			transaction.Commit();
			_id = entity.Id;
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void OrphanDeleteForDetachedOneToOne()
		{
			Guid childId;
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entity = session.Get<Entity1>(_id);
				childId = entity.Child.Id
				session.Evict(entity.Child);
				entity.Child = null;

				session.Flush();
				transaction.Commit();
			}

			using (var session = OpenSession())
			{
				var entity = session.Get<Entity1>(_id);
				Assert.That(entity, Is.Not.Null);
				Assert.That(entity.Child, Is.Null, "Unexpected child on parent");

				var child = session.Get<Entity2>(_id);
				Assert.That(child , Is.Null, "Child is still in database");
			}
		}
	}
}
