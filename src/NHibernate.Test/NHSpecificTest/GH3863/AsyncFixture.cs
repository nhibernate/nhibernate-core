using System;
using System.Data.Common;
using System.Threading.Tasks;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3863
{
	// Excluded from the async generation: this fixture checks that the async API does not perform synchronous reads.
	[TestFixture]
	public class AsyncFixture : TestCaseMappingByCode
	{
		private Guid _parentId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Parent>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Set(
					x => x.Children,
					m =>
					{
						m.Inverse(true);
						m.Lazy(CollectionLazy.Lazy);
						m.Cascade(Mapping.ByCode.Cascade.All);
					},
					r => r.OneToMany());
			});
			mapper.Class<Child>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Parent);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(
				Cfg.Environment.BatchStrategy,
				typeof(SyncReadsCountingBatcherFactory).AssemblyQualifiedName);
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var parent = new Parent { Name = "Parent" };
			parent.Children.Add(new Child { Name = "Child1", Parent = parent });
			parent.Children.Add(new Child { Name = "Child2", Parent = parent });
			session.Save(parent);

			transaction.Commit();
			_parentId = parent.Id;
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.CreateQuery("delete from Child").ExecuteUpdate();
			session.CreateQuery("delete from Parent").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public async Task CascadeDeleteLoadsUninitializedCollectionAsynchronouslyAsync()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var parent = await session.GetAsync<Parent>(_parentId);
				Assert.That(NHibernateUtil.IsInitialized(parent.Children), Is.False, "Children should not be loaded yet");

				var batcher = (SyncReadsCountingBatcher) session.GetSessionImplementation().Batcher;
				batcher.SyncReadsCount = 0;

				await session.DeleteAsync(parent);

				Assert.That(NHibernateUtil.IsInitialized(parent.Children), Is.True, "Children should have been loaded");
				Assert.That(batcher.SyncReadsCount, Is.Zero, "Children have been loaded synchronously");

				await transaction.CommitAsync();
			}

			using (var session = OpenSession())
			{
				Assert.That(await session.Query<Child>().CountAsync(), Is.Zero);
			}
		}

		public class SyncReadsCountingBatcher : NonBatchingBatcher
		{
			public SyncReadsCountingBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
				: base(connectionManager, interceptor)
			{
			}

			public int SyncReadsCount { get; set; }

			public override DbDataReader ExecuteReader(DbCommand cmd)
			{
				SyncReadsCount++;
				return base.ExecuteReader(cmd);
			}
		}

		public class SyncReadsCountingBatcherFactory : IBatcherFactory
		{
			public IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
			{
				return new SyncReadsCountingBatcher(connectionManager, interceptor);
			}
		}
	}
}
