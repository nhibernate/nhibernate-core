﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.GH3113
{
	using System.Threading.Tasks;
	[TestFixture]
	public class ByCodeFixtureAsync : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Oracle9iDialect;
		}

		protected override void Configure(Configuration configuration)
		{
			var dialect = NHibernate.Dialect.Dialect.GetDialect(configuration.Properties);
			if (dialect is Oracle8iDialect)
				configuration.SetProperty(Environment.Dialect, typeof(Oracle9iDialect).FullName);
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
		public async Task JoinFailsOnOracle9DialectAsync()
		{
			using (var session = OpenSession())
			{
				var result = from e in session.Query<Entity>()
							 join e2 in session.Query<Entity>() on e.Id equals e2.Id
							 where e.Name == "Bob"
							 select e.Name;

				Assert.That(await (result.ToListAsync()), Has.Count.EqualTo(1));
			}
		}
	}
}
