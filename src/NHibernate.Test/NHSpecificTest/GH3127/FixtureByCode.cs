﻿using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3127
{
	[TestFixture]
	public class StringTypeAsGenericParamByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name, x => x.Type<StringType>());
				rc.Property(x => x.NameAnsi, x => x.Type<AnsiStringType>());
				rc.Property(x => x.Amount, x => x.Type<CurrencyType>());
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob", NameAnsi = "Bob", Amount = 10.0M };
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
		public void QueryWorks()
		{
			using (var session = OpenSession())
			{
				var result = from e in session.Query<Entity>()
							 where e.Name == "Bob" && e.NameAnsi == "Bob" && e.Amount == 10.0M
							 select e;

				Assert.That(result.ToList(), Has.Count.EqualTo(1));
			}
		}
	}
}
