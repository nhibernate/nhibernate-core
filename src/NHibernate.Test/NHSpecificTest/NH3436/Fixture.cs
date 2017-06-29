using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3436
{
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<TestEntity>(rc =>
			{
				rc.Table("TestEntity");
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new TestEntity();
				session.Save(e1);

				var e2 = new TestEntity();
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void TestQueryWithContainsInParallel()
		{
			var ids = new List<Guid>
			{
				Guid.NewGuid(),
				Guid.NewGuid(),
			};
			const int threadsToRun = 32;
			var events = new WaitHandle[threadsToRun];
			var exceptions = new List<Exception>();
			for (var i = 0; i < threadsToRun; i++)
			{
				var @event = new ManualResetEvent(false);
				events[i] = @event;
				ThreadPool.QueueUserWorkItem(s =>
				{
					try
					{
						Run(ids);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
					finally
					{
						@event.Set();
					}
				});
			}
			WaitHandle.WaitAll(events);
			Assert.IsEmpty(exceptions);
		}
		
		[Test]
		public void TestQueryWithContains()
		{
			var ids = new List<Guid>
			{
				Guid.NewGuid(),
				Guid.NewGuid(),
			};
			Run(ids);
		}

		private void Run(ICollection<Guid> ids)
		{
			using (var session = Sfi.OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<TestEntity>()
					.Where(entity => ids.Contains(entity.Id))
					.ToList();

				Assert.That(result.Count, Is.EqualTo(0));
			}
		}

		public class TestEntity
		{
			public virtual Guid Id { get; set; }
		}
	}
}
