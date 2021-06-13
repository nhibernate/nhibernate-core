﻿using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2820
{
	[TestFixture]
	public class PostgresTimestampzFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.Timestamp, m => m.Column(x => x.SqlType("timestamp with time zone")));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is PostgreSQLDialect;
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity {Name = "Bob", Timestamp = DateTimeOffset.UtcNow};
				session.Save(e1);

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
		public void CanRetrieveEntityWithTimeStampColumn()
		{
			using (var session = OpenSession())
			{
				var result = from e in session.Query<Entity>()
							 where e.Name == "Bob"
							 select e;

				Assert.That(result.ToList(), Has.Count.EqualTo(1));
			}
		}
	}
}
