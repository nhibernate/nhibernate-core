using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.MultiTenancy;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.MultiTenancy
{
	[TestFixture]
	public class DatabaseStrategyNoDbSpecificFixture : TestCaseMappingByCode
	{
		private Guid _id;

		protected override void Configure(Configuration configuration)
		{
			configuration.Properties[Cfg.Environment.MultiTenant] = MultiTenancyStrategy.Database.ToString();
			configuration.Properties[Cfg.Environment.GenerateStatistics] = true.ToString();
			base.Configure(configuration);
		}

		[Test]
		public void ShouldThrowWithNoTenantIdentifier()
		{
			var sessionBuilder = Sfi.WithOptions().TenantConfiguration(new TenantConfiguration(new MockConnectionProvider(null, null)));

			Assert.That(() => sessionBuilder.OpenSession(), Throws.ArgumentException);
		}
		
		[Test]
		public void ShouldThrowWithNoConnectionAccess()
		{
			var sessionBuilder = Sfi.WithOptions().TenantConfiguration(new TenantConfiguration(new MockConnectionProvider("tenant1", null)));	

			Assert.That(() => sessionBuilder.OpenSession(), Throws.ArgumentException);
		}

		[Test]
		public void DifferentConnectionStringForDifferentTenants()
		{
			if (!IsSqlServerDialect)
				Assert.Ignore("MSSqlServer specific test");

			using(var session1 = OpenTenantSession("tenant1"))
			using (var session2 = OpenTenantSession("tenant2"))
			{
				Assert.That(session1.Connection.ConnectionString, Is.Not.EqualTo(session2.Connection.ConnectionString));
				ValidateSqlServerConnectionAppName(session1, "tenant1");
				ValidateSqlServerConnectionAppName(session2, "tenant2");
			}
		}

		private static void ValidateSqlServerConnectionAppName(ISession s, string tenantId)
		{
			var builder = new SqlConnectionStringBuilder(s.Connection.ConnectionString);
			Assert.That(builder.ApplicationName, Is.EqualTo(tenantId));
		}

		[Test]
		public void SecondLevelCacheReusedForSameTenant()
		{
			using (var sesTen1 = OpenTenantSession("tenant1"))
			{
				var entity = sesTen1.Get<Entity>(_id);
			}

			Sfi.Statistics.Clear();
			using (var sesTen2 = OpenTenantSession("tenant1"))
			{
				var entity = sesTen2.Get<Entity>(_id);
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
			Assert.That(Sfi.Statistics.SecondLevelCacheHitCount, Is.EqualTo(1));
		}

		[Test]
		public void SecondLevelCacheSeparationPerTenant()
		{
			using (var sesTen1 = OpenTenantSession("tenant1"))
			{
				var entity = sesTen1.Get<Entity>(_id);
			}

			Sfi.Statistics.Clear();
			using (var sesTen2 = OpenTenantSession("tenant2"))
			{
				var entity = sesTen2.Get<Entity>(_id);
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Assert.That(Sfi.Statistics.SecondLevelCacheHitCount, Is.EqualTo(0));
		}

		[Test]
		public void QueryCacheReusedForSameTenant()
		{
			using (var sesTen1 = OpenTenantSession("tenant1"))
			{
				var entity = sesTen1.Query<Entity>().WithOptions(x => x.SetCacheable(true)).Where(e => e.Id == _id).SingleOrDefault();
			}

			Sfi.Statistics.Clear();
			using (var sesTen2 = OpenTenantSession("tenant1"))
			{
				var entity = sesTen2.Query<Entity>().WithOptions(x => x.SetCacheable(true)).Where(e => e.Id == _id).SingleOrDefault();
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));
		}

		[Test]
		public void QueryCacheSeparationPerTenant()
		{
			using (var sesTen1 = OpenTenantSession("tenant1"))
			{
				var entity = sesTen1.Query<Entity>().WithOptions(x => x.SetCacheable(true)).Where(e => e.Id == _id).SingleOrDefault();
			}

			Sfi.Statistics.Clear();
			using (var sesTen2 = OpenTenantSession("tenant2"))
			{
				var entity = sesTen2.Query<Entity>().WithOptions(x => x.SetCacheable(true)).Where(e => e.Id == _id).SingleOrDefault();
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));
		}
		
		[Test]
		public void TenantSessionIsSerializableAndCanBeReconnected()
		{
			ISession deserializedSession = null; 
			using (var sesTen1 = OpenTenantSession("tenant1"))
			{
				var entity = sesTen1.Query<Entity>().WithOptions(x => x.SetCacheable(true)).Where(e => e.Id == _id).SingleOrDefault();
				sesTen1.Disconnect();
				deserializedSession = SpoofSerialization(sesTen1);
			}

			Sfi.Statistics.Clear();
			using (deserializedSession)
			{
				deserializedSession.Reconnect();
				var entity = deserializedSession.Get<Entity>(_id);
				if (IsSqlServerDialect)
					ValidateSqlServerConnectionAppName(deserializedSession, "tenant1");
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0));
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));
		}

		private ISession SpoofSerialization(ISession session)
		{
			var formatter = new BinaryFormatter
			{
#if !NETFX
				SurrogateSelector = new SerializationHelper.SurrogateSelector()	
#endif
			};
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, session);

			stream.Position = 0;

			return (ISession) formatter.Deserialize(stream);
		}

		private ISession OpenTenantSession(string tenantId)
		{
			return Sfi.WithOptions().TenantConfiguration(GetTenantConfig(tenantId)).OpenSession();
		}

		private TenantConfiguration GetTenantConfig(string tenantId)
		{
			return new TenantConfiguration(new TestTenantConnectionProvider(Sfi, tenantId));
		}

		private bool IsSqlServerDialect => Sfi.Dialect is MsSql2005Dialect;

		#region Test Setup

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity>(
				rc =>
				{
					rc.Cache(m => m.Usage(CacheUsage.NonstrictReadWrite));
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override ISession OpenSession()
		{
			return OpenTenantSession("defaultTenant");
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity {Name = "Bob"};
				session.Save(e1);

				var e2 = new Entity {Name = "Sally"};
				session.Save(e2);

				session.Flush();
				transaction.Commit();
				_id = e1.Id;
			}
		}

		#endregion Test Setup
	}

	[Serializable]
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
	
	public class MockConnectionProvider : IMultiTenantConnectionProvider
	{
		private readonly IConnectionAccess _connectionAccess;

		public MockConnectionProvider(string tenantIdentifier, IConnectionAccess connectionAccess)
		{
			_connectionAccess = connectionAccess;
			TenantIdentifier = tenantIdentifier;
		}

		public string TenantIdentifier { get; }
		public IConnectionAccess GetConnectionAccess()
		{
			return _connectionAccess;
		}
	}

	[Serializable]
	public class TestTenantConnectionProvider : AbstractMultiTenantConnectionProvider
	{
		public TestTenantConnectionProvider(ISessionFactoryImplementor sfi, string tenantId)
		{
			TenantIdentifier = tenantId;
			SessionFactory = sfi;
			TenantConnectionString = sfi.ConnectionProvider.GetConnectionString();
			if (sfi.Dialect is MsSql2005Dialect)
			{
				var stringBuilder = new SqlConnectionStringBuilder(sfi.ConnectionProvider.GetConnectionString());
				stringBuilder.ApplicationName = tenantId;
				TenantConnectionString = stringBuilder.ToString();
			}
		}

		protected override string TenantConnectionString { get; }

		public override string TenantIdentifier { get; }

		protected override ISessionFactoryImplementor SessionFactory { get; }
	}
}
