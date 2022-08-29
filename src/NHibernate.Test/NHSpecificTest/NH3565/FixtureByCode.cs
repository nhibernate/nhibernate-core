using System.Data;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3565
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name, m =>
				{
					m.Type(NHibernateUtil.AnsiString);
					m.Length(10);
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return base.AppliesTo(dialect)
					//Dialects like SQL Server CE, Firebird don't distinguish AnsiString from String
					&& Dialect.GetTypeName(new SqlType(DbType.AnsiString)) != Dialect.GetTypeName(new SqlType(DbType.String));
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

				transaction.Commit();
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

		[Test]
		public void ParameterTypeForLikeIsProperlyDetected()
		{
			using (var logSpy = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var result = from e in session.Query<Entity>()
							 where NHibernate.Linq.SqlMethods.Like(e.Name, "Bob")
							 select e;

				Assert.That(result.ToList(), Has.Count.EqualTo(1));
				Assert.That(logSpy.GetWholeLog(), Does.Contain("Type: AnsiString"));
			}
		}

		[KnownBug("Not fixed yet")]
		[Test]
		public void ParameterTypeForContainsIsProperlyDetected()
		{
			using (var logSpy = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var result = from e in session.Query<Entity>()
							 where e.Name.Contains("Bob")
							 select e;

				Assert.That(result.ToList(), Has.Count.EqualTo(1));
				Assert.That(logSpy.GetWholeLog(), Does.Contain("Type: AnsiString"));
			}
		}

		[KnownBug("Not fixed yet")]
		[Test]
		public void ParameterTypeForStartsWithIsProperlyDetected()
		{
			using (var logSpy = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var result = from e in session.Query<Entity>()
							 where e.Name.StartsWith("Bob")
							 select e;

				Assert.That(result.ToList(), Has.Count.EqualTo(1));
				Assert.That(logSpy.GetWholeLog(), Does.Contain("Type: AnsiString"));
			}
		}
	}
}
