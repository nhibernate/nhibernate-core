using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.NHSpecificTest.GH2445;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2446
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MySQLDialect || dialect is SQLiteDialect;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<UnsignedEntity>(m =>
			{
				m.Id(c => c.Id, id =>
				{
					id.Generator(Generators.Native);
				});
				m.Property(o => o.Short, o => o.NotNullable(true));
				m.Property(o => o.Integer, o => o.NotNullable(true));
				m.Property(o => o.Long, o => o.NotNullable(true));
				m.Property(o => o.NullableNumber);
				m.Property(o => o.ShortNumber, o => o.NotNullable(true));
				m.Property(o => o.NullableShortNumber);
				m.Property(o => o.LargeNumber, o => o.NotNullable(true));
				m.Property(o => o.NullableLargeNumber);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var entity = new UnsignedEntity()
				{
					Short = 123,
					Integer = 12345,
					Long = 123456789,
					LargeNumber = 123456789,
					NullableLargeNumber = null,
					NullableNumber = null,
					ShortNumber = 123,
					NullableShortNumber = null
				};
				session.Save(entity);
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.CreateQuery("delete from UnsignedEntity").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void TestUInt()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var number = 10;
				session.Query<UnsignedEntity>().Where(o => o.Id == (uint) number).ToList();
				session.Query<UnsignedEntity>().Where(o => o.Id == (uint) o.Integer).ToList();
				session.Query<UnsignedEntity>().Where(o => o.Id > number).ToList();
			}
		}

		[Test]
		public void TestNullableUInt()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var number = 10;
				session.Query<UnsignedEntity>().Where(o => o.NullableNumber == (uint?) number).ToList();
				session.Query<UnsignedEntity>().Where(o => o.NullableNumber == (uint?) o.Integer).ToList();
				session.Query<UnsignedEntity>().Where(o => o.NullableNumber > number).ToList();
			}
		}


		[Test]
		public void TestUShort()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				short number = 10;
				session.Query<UnsignedEntity>().Where(o => o.ShortNumber == (ushort) number).ToList();
				session.Query<UnsignedEntity>().Where(o => o.ShortNumber == (ushort) o.Short).ToList();
				session.Query<UnsignedEntity>().Where(o => o.ShortNumber > number).ToList();
			}
		}

		[Test]
		public void TestNullableUShort()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				short number = 10;
				session.Query<UnsignedEntity>().Where(o => o.NullableShortNumber == (ushort?) number).ToList();
				session.Query<UnsignedEntity>().Where(o => o.NullableShortNumber == (ushort?) o.Short).ToList();
				session.Query<UnsignedEntity>().Where(o => o.NullableShortNumber > number).ToList();
			}
		}

		[Test]
		public void TestULong()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				long number = 10;
				session.Query<UnsignedEntity>().Where(o => o.LargeNumber == (ulong) number).ToList();
				session.Query<UnsignedEntity>().Where(o => o.LargeNumber == (ulong) o.Long).ToList();
				session.Query<UnsignedEntity>().Where(o => o.LargeNumber > (ulong) number).ToList();
			}
		}

		[Test]
		public void TestNullableULong()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				long number = 10;
				session.Query<UnsignedEntity>().Where(o => o.NullableLargeNumber == (ulong?) number).ToList();
				session.Query<UnsignedEntity>().Where(o => o.NullableLargeNumber == (ulong?) o.Long).ToList();
				session.Query<UnsignedEntity>().Where(o => o.NullableLargeNumber > (ulong?) number).ToList();
			}
		}
	}
}
