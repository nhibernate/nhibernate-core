using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3516
{
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.FirstChar);
			});

			mapper.Class<BaseClass>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Discriminator(x => x.Column("StringDiscriminator"));
				rc.Property(x => x.Name);
				rc.Abstract(true);
			});
			mapper.Subclass<Subclass1>(rc => rc.DiscriminatorValue(Entity.NameWithSingleQuote));
			mapper.Subclass<Subclass2>(rc => rc.DiscriminatorValue(Entity.NameWithEscapedSingleQuote));

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.Save(new Entity { Name = Entity.NameWithSingleQuote, FirstChar = Entity.QuoteInitial });
			session.Save(new Entity { Name = Entity.NameWithEscapedSingleQuote, FirstChar = Entity.BackslashInitial });

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		private static readonly string[] StringInjectionsProperties =
		{
			nameof(Entity.NameWithSingleQuote), nameof(Entity.NameWithEscapedSingleQuote)
		};

		[TestCaseSource(nameof(StringInjectionsProperties))]
		public void SqlInjectionInStrings(string propertyName)
		{
			using var session = OpenSession();

			var query = session.CreateQuery($"from Entity e where e.Name = Entity.{propertyName}");
			IList<Entity> list = null;
			Assert.That(() => list = query.List<Entity>(), Throws.Nothing);
			Assert.That(list, Has.Count.EqualTo(1), $"Unable to find entity with name {propertyName}");
		}

		private static readonly string[] SpecialNames =
		{
			"\0; drop table Entity; --",
			"\b; drop table Entity; --",
			"\n; drop table Entity; --",
			"\r; drop table Entity; --",
			"\t; drop table Entity; --",
			"\x1A; drop table Entity; --",
			"\"; drop table Entity; --",
			"\\; drop table Entity; --"
		};

		[TestCaseSource(nameof(SpecialNames))]
		public void StringsWithSpecialCharacters(string name)
		{
			// We may not even be able to insert the entity.
			var wasInserted = false;
			try
			{
				using var s = OpenSession();
				using var t = s.BeginTransaction();
				var e = new Entity { Name = name };
				s.Save(e);
				t.Commit();

				wasInserted = true;
			}
			catch (Exception e)
			{
				Assert.Warn($"The entity insertion failed with message {e}");
			}

			try
			{
				using var session = OpenSession();
				Entity.ArbitraryStringValue = name;
				var list = session.CreateQuery($"from Entity e where e.Name = Entity.{nameof(Entity.ArbitraryStringValue)}").List<Entity>();
				if (wasInserted && list.Count != 1)
					Assert.Warn($"Unable to find entity with name {nameof(Entity.ArbitraryStringValue)}");
			}
			catch (Exception e)
			{
				Assert.Warn($"The query has failed with message {e}");
			}

			// Check the db is not wrecked.
			if (wasInserted)
			{
				using var session = OpenSession();
				var list = session
					.CreateQuery("from Entity e where e.Name = :name")
					.SetString("name", name)
					.List<Entity>();
				Assert.That(list, Has.Count.EqualTo(1));
			}
			else
			{
				using var session = OpenSession();
				var all = session.CreateQuery("from Entity e").List<Entity>();
				Assert.That(all, Has.Count.GreaterThan(0));
			}
		}

		[Test]
		public void SqlInjectionInStringDiscriminator()
		{
			using var session = OpenSession();

			session.Save(new Subclass1 { Name = "Subclass1" });
			session.Save(new Subclass2 { Name = "Subclass2" });

			// ObjectToSQLString is used for generating the inserts.
			Assert.That(session.Flush, Throws.Nothing, "Unable to flush the subclasses");

			foreach (var entityName in new[] { nameof(Subclass1), nameof(Subclass2) })
			{
				var query = session.CreateQuery($"from {entityName}");
				IList list = null;
				Assert.That(() => list = query.List(), Throws.Nothing, $"Unable to list entities of {entityName}");
				Assert.That(list, Has.Count.EqualTo(1), $"Unable to find the {entityName} entity");
			}
		}

		private static readonly string[] CharInjectionsProperties =
		{
			nameof(Entity.QuoteInitial), nameof(Entity.BackslashInitial)
		};

		[TestCaseSource(nameof(CharInjectionsProperties))]
		public void SqlInjectionInChar(string propertyName)
		{
			using var session = OpenSession();
			var query = session.CreateQuery($"from Entity e where e.FirstChar = Entity.{propertyName}");
			IList<Entity> list = null;
			Assert.That(() => list = query.List<Entity>(), Throws.Nothing);
			Assert.That(list, Is.Not.Null.And.Count.EqualTo(1), $"Unable to find entity with initial {propertyName}");
		}
	}
}
