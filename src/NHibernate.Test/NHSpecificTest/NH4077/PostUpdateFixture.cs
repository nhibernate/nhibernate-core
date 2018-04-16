using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH4077
{
	[TestFixture]
	public partial class PostUpdateFixture : TestCaseMappingByCode
	{
		[Test]
		public void AutoflushInPostUpdateListener_CausesArgumentOutOfRangeException_in_ActionQueueExecuteActions()
		{
			// load a few (more than one) entities and process them. we let NHibernate figure out if they need saving or not.
			Entity entityOne;
			Entity entityTwo;
			using (var session = OpenSession())
			{
				entityOne = session.CreateCriteria<Entity>().Add(Restrictions.Eq(nameof(Entity.Code), "one")).List<Entity>().First();
				entityTwo = session.CreateCriteria<Entity>().Add(Restrictions.Eq(nameof(Entity.Code), "two")).List<Entity>().First();
			}

			// processing omitted (not necessary to illustrate the problem)

			// resave them, but all-or-nothing inside a transaction
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// using FlushMode.Commit prevents the issue; using the default FlushMode.Auto breaks.
				//session.FlushMode = FlushMode.Commit;
				session.SaveOrUpdate(entityOne);
				session.SaveOrUpdate(entityTwo);

				// committing the transaction causes an ArgumentOutOfRange exception inside ActionQueue.ExecuteActions
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
			var existingListeners = (configuration.EventListeners.PostUpdateEventListeners ?? Array.Empty<IPostUpdateEventListener>()).ToList();
			// this evil listener uses the session to perform a few queries and causes an auto-flush to happen
			existingListeners.Add(new CausesAutoflushListener());
			configuration.EventListeners.PostUpdateEventListeners = existingListeners.ToArray();
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

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// objects must exist before doing the processing; the issue does not occur during 
				session.Save(new Entity { Code = "one" });
				session.Save(new Entity { Code = "two" });

				session.Flush();
				transaction.Commit();
			}
		}

		private sealed partial class CausesAutoflushListener : IPostUpdateEventListener
		{
			private bool _currentlyLogging;

			public void OnPostUpdate(PostUpdateEvent @event)
			{
				if (!(@event.Entity is Entity))
					return;
				// this guard is necessary to avoid a StackOverflowException due to the Query below triggering this event again.
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
