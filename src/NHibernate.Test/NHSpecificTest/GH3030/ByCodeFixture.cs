using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3030
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Table("Entity");
					rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void LinqShouldNotLeakEntityParameters()
		{
			WeakReference sessionReference = null;
			WeakReference firstReference = null;
			WeakReference secondReference = null;

			new System.Action(
				() =>
				{
					using (var session = ((DebugSessionFactory) Sfi).ActualFactory.OpenSession())
					{
						var first = new Entity { Id = 1 };
						var second = new Entity { Id = 2 };

						_ = session.Query<Entity>().FirstOrDefault(f => f == first);
						_ = session.Query<Entity>().FirstOrDefault(f => f == second);

						sessionReference = new WeakReference(session, true);
						firstReference = new WeakReference(first, true);
						secondReference = new WeakReference(second, true);
					}
				})();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			Assert.That(sessionReference.Target, Is.Null);
			Assert.That(firstReference.Target, Is.Null);
			Assert.That(secondReference.Target, Is.Null);
		}

		public class Entity
		{
			public virtual int Id { get; set; }
		}
	}
}
