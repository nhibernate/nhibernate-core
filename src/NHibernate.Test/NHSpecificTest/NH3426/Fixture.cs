using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH3426
{
	/// <summary>
	/// Verify that we can convert a GUID column to a string in the standard GUID format inside
	/// the database engine.
	/// </summary>
	[TestFixture(true)]
	[TestFixture(false)]
	public class Fixture : TestCaseMappingByCode
	{
		private readonly bool _useBinaryGuid;

		public Fixture(bool useBinaryGuid)
		{
			_useBinaryGuid = useBinaryGuid;
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// For SQLite, we run the tests for both storage modes (SQLite specific setting).
			if (dialect is SQLiteDialect)
				return true;

			// For all other dialects, run the tests only once since the storage mode
			// is not relevant. (We use the case of _useBinaryGuid==true since this is probably
			// what most engines do internally.)
			return _useBinaryGuid;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			if (Dialect is SQLiteDialect)
			{
				var connStr = configuration.Properties[Environment.ConnectionString];

				if (_useBinaryGuid)
					connStr += "BinaryGuid=True;";
				else
					connStr += "BinaryGuid=False;";

				configuration.Properties[Environment.ConnectionString] = connStr;
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		private const string id = "9FF2D288-56E6-F349-9CFC-48902132D65B";

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			{
				session.Save(new Entity { Id = Guid.Parse(id), Name = "Name 1" });

				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Delete("from System.Object");

					session.Flush();
					transaction.Commit();
				}
			}
		}

		[Test]
		public void SelectGuidToString()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
					.Select(x => new { Id = x.Id.ToString() })
					.ToList();

				Assert.That(list[0].Id.ToUpper(), Is.EqualTo(id.ToUpper()));
			}
		}

		[Test]
		public void WhereGuidToString()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
								  .Where(x => x.Id.ToString().ToUpper() == id)
								  .ToList();

				Assert.That(list, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void CompareStringColumnWithGuidToString()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
								  .Where(x => x.Id.ToString() == x.Name)
								  .ToList();

				Assert.That(list, Has.Count.EqualTo(0));
			}
		}

		[Test]
		public void CompareStringColumnWithNullableGuidToString()
		{
			using (var session = OpenSession())
			{
				var list = session.Query<Entity>()
								  .Where(x => ((Guid?) x.Id).ToString() == x.Id.ToString())
								  .ToList();

				Assert.That(list, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void SelectGuidToStringImplicit()
		{
			if (Dialect is SQLiteDialect && _useBinaryGuid)
				Assert.Ignore("Fails with BinaryGuid=True due to GH-2109. (2019-04-09).");

			if (Dialect is FirebirdDialect || Dialect is MySQLDialect || Dialect is Oracle8iDialect)
				Assert.Ignore("Since strguid() is not applied, it fails on Firebird, MySQL and Oracle " +
							"because a simple cast cannot be used for GUID to string conversion on " +
							"these dialects. See GH-2109.");

			using (var session = OpenSession())
			{
				// Verify in-db GUID to string conversion when ToString() is applied to the entity that has
				// a GUID id column (that is, we deliberately avoid mentioning the Id property). This
				// exposes bug GH-2109.
				var list = session.Query<Entity>()
								.Select(x => new { Id = x.ToString() })
								.ToList();

				Assert.That(list[0].Id.ToUpper(), Is.EqualTo(id.ToUpper()));
			}
		}

		[Test]
		public void WhereGuidToStringImplicit()
		{
			if (Dialect is SQLiteDialect && _useBinaryGuid)
				Assert.Ignore("Fails with BinaryGuid=True due to GH-2109. (2019-04-09).");

			if (Dialect is FirebirdDialect || Dialect is MySQLDialect || Dialect is Oracle8iDialect)
				Assert.Ignore("Since strguid() is not applied, it fails on Firebird, MySQL and Oracle " +
							"because a simple cast cannot be used for GUID to string conversion on " +
							"these dialects. See GH-2109.");

			using (var session = OpenSession())
			{
				// Verify in-db GUID to string conversion when ToString() is applied to the entity that has
				// a GUID id column (that is, we deliberately avoid mentioning the Id property). This
				// exposes bug GH-2109.
				var list = session.Query<Entity>()
								.Where(x => x.ToString().ToUpper() == id)
								.ToList();

				Assert.That(list, Has.Count.EqualTo(1));
			}
		}
	}
}
