using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Loader;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH3530
{
	//NH-3530 (GH-1316) 
	[TestFixture(BatchFetchStyle.Dynamic)]
	[TestFixture(BatchFetchStyle.Legacy)]
	public class BatchFetchStyleFixture : TestCaseMappingByCode
	{
		private readonly BatchFetchStyle _fetchStyle;
		private readonly List<object> _ids = new List<object>();

		public BatchFetchStyleFixture(BatchFetchStyle fetchStyle)
		{
			_fetchStyle = fetchStyle;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.BatchFetchStyle, _fetchStyle.ToString());
		}

		[Test]
		public void CanLoadEntity()
		{
			PrepareEntities(2);

			using (var session = OpenSession())
			{
				var proxy = session.Load<Entity>(_ids[0]);
				var result = session.Get<Entity>(_ids[1]);

				Assert.That(result.Name, Is.Not.Null);

				var childrenCount = result.Children.Count;
				//Assert.That(NHibernateUtil.IsInitialized(proxy), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(proxy.Children), Is.True);
				Assert.That(childrenCount, Is.EqualTo(4));
			}
		}

		[KnownBug("GH-2960")]
		[Test]
		public void CanLoadComponentEntity()
		{
			PrepareComponentEntities(2);

			using (var session = OpenSession())
			{
				var proxy = session.Load<EntityComponent>(_ids[0]);
				var result = session.Get<EntityComponent>(_ids[1]);

				Assert.That(result.Name, Is.Not.Null);

				var childrenCount = result.Children.Count;
				Assert.That(NHibernateUtil.IsInitialized(proxy.Children), Is.True);
				Assert.That(childrenCount, Is.EqualTo(4));
			}
		}

		[Test]
		public void CanLoadComponentIdEntity()
		{
			PrepareComponentIdEntities(2);

			using (var session = OpenSession())
			{
				var proxy = session.Load<EntityComponentId>(_ids[0]);
				var result = session.Get<EntityComponentId>(_ids[1]);

				Assert.That(result.Name, Is.Not.Null);

				var childrenCount = result.Children.Count;
				Assert.That(NHibernateUtil.IsInitialized(proxy.Children), Is.True);
				Assert.That(childrenCount, Is.EqualTo(4));
			}
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		public void CanLoadBatch(int loadCount)
		{
			PrepareEntities(5);

			using (var session = OpenSession())
			{
				foreach (var id in _ids.Take(loadCount))
				{
					session.Load<Entity>(id);
				}

				var result = session.Get<Entity>(_ids[0]);
				var result2 = session.Get<Entity>(_ids[1]);
				var last = session.Get<Entity>(_ids.Last());

				var count = result.Children.Count;
				Assert.That(result.Name, Is.Not.Null);
				Assert.That(last.Name, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(result2.Children), Is.True);
			}
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

		protected override HbmMapping GetMappings()
		{
			return EntityMappings.CreateMapping();
		}

		private void PrepareEntities(int count)
		{
			_ids.Clear();
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				for (int i = 0; i < count; i++)
				{
					var entity = new Entity { Name = "some name" + 1 };
					AddChildren(entity.Children, 4);
					_ids.Add((Guid) session.Save(entity));
				}

				transaction.Commit();
			}
		}

		private void PrepareComponentEntities(int count)
		{
			_ids.Clear();
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				for (int i = 0; i < count; i++)
				{
					var entity = new EntityComponent { Id1 = i, Id2 = i + 1, Name = "some name" + 1 };
					AddChildren(entity.Children, 4);

					_ids.Add(session.Save(entity));
				}

				transaction.Commit();
			}
		}

		private void PrepareComponentIdEntities(int count)
		{
			_ids.Clear();
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				for (int i = 0; i < count; i++)
				{
					var entity = new EntityComponentId { Id = new ComponentId { Id1 = i, Id2 = i + 1 }, Name = "some name" + 1 };
					AddChildren(entity.Children, 4);
					_ids.Add(session.Save(entity));
				}

				transaction.Commit();
			}
		}

		private static void AddChildren<T>(IList<T> list, int count) where T : new()
		{
			for (int i = 0; i < count; i++)
			{
				list.Add(new T());
			}
		}
	}
}
