﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NHibernate.Test.NHSpecificTest.GH3516
{
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{

		private readonly HashSet<string> _unsupportedProperties = new();

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.FirstChar);
				rc.Property(x => x.CharacterEnum, m => m.Type<EnumCharType<CharEnum>>());
				rc.Property(x => x.UriProperty);

				rc.Property(x => x.ByteProperty);
				rc.Property(x => x.DecimalProperty);
				rc.Property(x => x.DoubleProperty);
				rc.Property(x => x.FloatProperty);
				rc.Property(x => x.ShortProperty);
				rc.Property(x => x.IntProperty);
				rc.Property(x => x.LongProperty);

				if (TestDialect.SupportsSqlType(SqlTypeFactory.SByte))
					rc.Property(x => x.SByteProperty);
				else
					_unsupportedProperties.Add(nameof(Entity.SByteProperty));

				if (TestDialect.SupportsSqlType(SqlTypeFactory.UInt16))
					rc.Property(x => x.UShortProperty);
				else
					_unsupportedProperties.Add(nameof(Entity.UShortProperty));

				if (TestDialect.SupportsSqlType(SqlTypeFactory.UInt32))
					rc.Property(x => x.UIntProperty);
				else
					_unsupportedProperties.Add(nameof(Entity.UIntProperty));

				if (TestDialect.SupportsSqlType(SqlTypeFactory.UInt64))
					rc.Property(x => x.ULongProperty);
				else
					_unsupportedProperties.Add(nameof(Entity.ULongProperty));

				rc.Property(x => x.DateTimeProperty);
				rc.Property(x => x.DateProperty, m => m.Type(NHibernateUtil.Date));
				if (TestDialect.SupportsSqlType(SqlTypeFactory.DateTimeOffSet))
					rc.Property(x => x.DateTimeOffsetProperty);
				else
					_unsupportedProperties.Add(nameof(Entity.DateTimeOffsetProperty));
				rc.Property(x => x.TimeProperty, m => m.Type(NHibernateUtil.Time));

				rc.Property(x => x.GuidProperty);
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

			mapper.Import<CharEnum>();

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		private CultureInfo _backupCulture;
		private CultureInfo _backupUICulture;

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.Save(
				new Entity
				{
					Name = Entity.NameWithSingleQuote,
					FirstChar = Entity.QuoteInitial,
					CharacterEnum = CharEnum.SingleQuote,
					UriProperty = Entity.UriWithSingleQuote
				});
			session.Save(
				new Entity
				{
					Name = Entity.NameWithEscapedSingleQuote,
					FirstChar = Entity.BackslashInitial,
					CharacterEnum = CharEnum.Backslash,
					UriProperty = Entity.UriWithEscapedSingleQuote
				});

			transaction.Commit();

			_backupCulture = CultureInfo.CurrentCulture;
			_backupUICulture = CultureInfo.CurrentUICulture;
		}

		protected override void OnTearDown()
		{
			if (_backupCulture != null)
			{
				CultureInfo.CurrentCulture = _backupCulture;
				CultureInfo.CurrentUICulture = _backupUICulture;
			}

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

		private static readonly string[] CharEnumInjections =
		{
			nameof(CharEnum.SingleQuote), nameof(CharEnum.Backslash)
		};

		[TestCaseSource(nameof(CharEnumInjections))]
		public void SqlInjectionWithCharEnum(string enumName)
		{
			using var session = OpenSession();

			var query = session.CreateQuery($"from Entity e where e.CharacterEnum = CharEnum.{enumName}");
			IList<Entity> list = null;
			Assert.That(() => list = query.List<Entity>(), Throws.Nothing);
			Assert.That(list, Has.Count.EqualTo(1), $"Unable to find entity with CharacterEnum {enumName}");
		}

		private static readonly string[] UriInjections =
		{
			nameof(Entity.UriWithSingleQuote), nameof(Entity.UriWithEscapedSingleQuote)
		};

		[TestCaseSource(nameof(UriInjections))]
		public void SqlInjectionWithUri(string propertyName)
		{
			using var session = OpenSession();

			var query = session.CreateQuery($"from Entity e where e.UriProperty = Entity.{propertyName}");
			IList<Entity> list = null;
			Assert.That(() => list = query.List<Entity>(), Throws.Nothing);
			Assert.That(list, Has.Count.EqualTo(1), $"Unable to find entity with UriProperty {propertyName}");
		}

		private static readonly string[] NumericalTypesInjections =
		{
			nameof(Entity.ByteProperty),
			nameof(Entity.DecimalProperty),
			nameof(Entity.DoubleProperty),
			nameof(Entity.FloatProperty),
			nameof(Entity.ShortProperty),
			nameof(Entity.IntProperty),
			nameof(Entity.LongProperty),
			nameof(Entity.SByteProperty),
			nameof(Entity.UShortProperty),
			nameof(Entity.UIntProperty),
			nameof(Entity.ULongProperty)
		};

		[TestCaseSource(nameof(NumericalTypesInjections))]
		public void SqlInjectionInNumericalType(string propertyName)
		{
			Assume.That(_unsupportedProperties, Does.Not.Contains((object) propertyName), $"The {propertyName} property is unsupported by the dialect");

			Entity.ArbitraryStringValue = "0; drop table Entity; --";
			using (var session = OpenSession())
			{
				IQuery query;
				// Defining that query is invalid and should throw.
				try
				{
					query = session.CreateQuery($"from Entity e where e.{propertyName} = Entity.{nameof(Entity.ArbitraryStringValue)}");
				}
				catch (Exception ex)
				{
					// All good.
					Assert.Pass($"The wicked query creation has been rejected, as it should: {ex}");
					// Needed for the compiler who does not know "Pass" always throw.
					return;
				}

				// The query definition has been accepted, run it.
				try
				{
					query.List<Entity>();
				}
				catch (Exception ex)
				{
					// Expecting no exception at that point, but the test is to check if the injection succeeded.
					Assert.Warn($"The wicked query execution has failed: {ex}");
				}
			}

			// Check if we can still query Entity. If it succeeds, at least it means the injection failed.
			using (var session = OpenSession())
			{
				IList<Entity> list = null;
				Assert.That(() => list = session.CreateQuery("from Entity e").List<Entity>(), Throws.Nothing);
				Assert.That(list, Has.Count.GreaterThan(0));
			}
		}

		private static readonly string[] DateTypesInjections =
		{
			nameof(Entity.DateTimeProperty),
			nameof(Entity.DateProperty),
			nameof(Entity.DateTimeOffsetProperty),
			nameof(Entity.TimeProperty)
		};

		[TestCaseSource(nameof(DateTypesInjections))]
		public void SqlInjectionWithDatetime(string propertyName)
		{
			Assume.That(_unsupportedProperties, Does.Not.Contains((object) propertyName), $"The {propertyName} property is unsupported by the dialect");

			var wickedCulture = new CultureInfo("en-US");
			if (propertyName == nameof(Entity.TimeProperty))
				wickedCulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss\\'\"; drop table Entity; --\"";
			else
				wickedCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-ddTHH:mm:ss\\'\"; drop table Entity; --\"";
			CultureInfo.CurrentCulture = wickedCulture;
			CultureInfo.CurrentUICulture = wickedCulture;

			using var session = OpenSession();

			var staticPropertyName = propertyName == nameof(Entity.DateTimeOffsetProperty) ?
				nameof(Entity.StaticDateTimeOffsetProperty) : nameof(Entity.StaticDateProperty);
			var query = session.CreateQuery($"from Entity e where e.{propertyName} = Entity.{staticPropertyName}");
			IList<Entity> list = null;
			Assume.That(() => list = query.List<Entity>(), Throws.Nothing,
				"The first execution of the query failed, the injection has likely failed");
			// Execute again to check the table is still here.
			Assert.That(() => list = query.List<Entity>(), Throws.Nothing,
				"The second execution of the query failed although the first one did not: the injection has succeeded");
		}

		private static readonly string[] GuidInjections =
		{
			Entity.NameWithSingleQuote, Entity.NameWithEscapedSingleQuote
		};

		[TestCaseSource(nameof(GuidInjections))]
		public void SqlInjectionWithGuid(string injection)
		{
			Entity.ArbitraryStringValue = $"{Guid.NewGuid()}{injection}";
			using var session = OpenSession();

			var query = session.CreateQuery($"from Entity e where e.GuidProperty = Entity.{nameof(Entity.ArbitraryStringValue)}");
			IList<Entity> list = null;
			Assume.That(() => list = query.List<Entity>(), Throws.Nothing,
				"The first execution of the query failed, the injection has likely failed");
			// Execute again to check the table is still here.
			Assert.That(() => list = query.List<Entity>(), Throws.Nothing,
				"The second execution of the query failed although the first one did not: the injection has succeeded");
		}
	}
}
