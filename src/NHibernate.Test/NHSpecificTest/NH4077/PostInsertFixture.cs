using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH4077
{
	[TestFixture]
	public partial class PostInsertFixture : TestCaseMappingByCode
	{
		[Test]
		public void AutoflushInPostInsertListener_CausesDuplicateInserts_WithPrimaryKeyViolations()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// using FlushMode.Commit prevents the issue; using the default FlushMode.Auto breaks.
				//session.FlushMode = FlushMode.Commit;
				session.Save(new Entity { Code = "one" });
				session.Save(new Entity { Code = "two" });

				// committing the transaction causes a primary key violation by saving the entities multiple times
				transaction.Commit();
				session.Flush();
			}
		}

		[Test]
		public void Autoflush_MayTriggerAdditionalAutoFlushFromEvents()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// using FlushMode.Commit prevents the issue; using the default FlushMode.Auto breaks.
				//session.FlushMode = FlushMode.Commit;
				session.Save(new Entity { Code = "one" });
				session.Save(new Entity { Code = "two" });

				// Querying the entity triggers an auto-flush
				var count = session.CreateQuery("select count(o) from Entity o").UniqueResult<long>();
				Assert.That(count, Is.GreaterThan(0));
				transaction.Commit();
				session.Flush();
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Code);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			var existingListeners = (configuration.EventListeners.PostInsertEventListeners ?? Array.Empty<IPostInsertEventListener>()).ToList();
			// this evil listener uses the session to perform a few queries and causes an auto-flush to happen
			existingListeners.Add(new CausesAutoflushListener());
			configuration.EventListeners.PostInsertEventListeners = existingListeners.ToArray();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from Entity");

				session.Flush();
				transaction.Commit();
			}
		}

		private sealed partial class CausesAutoflushListener : IPostInsertEventListener
		{
			private bool _currentlyLogging;

			public void OnPostInsert(PostInsertEvent @event)
			{
				if (!(@event.Entity is Entity))
					return;
				// This guard is necessary to avoid multiple inserts of the original objects.
				// Commenting this out is likely to cause one PK violation per run, which seems to be capped to at most 10 attempts.
				// With the guard, only one PK violation is reported.
				if (_currentlyLogging)
					return;

				try
				{
					_currentlyLogging = true;
					var session = @event.Session;
					// this causes an Autoflush
					long count = session.CreateQuery("select count(o) from Entity o").UniqueResult<long>();
					Console.WriteLine("Total entity count: {0}", count);
				}
				finally
				{
					_currentlyLogging = false;
				}
			}
		}
	}
}
