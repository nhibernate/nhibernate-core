using System;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaValidator
{
	[TestFixture]
	public class PostgresSchemaValidateFixture
	{
		[Test]
		public void ShouldBeInvalidIfTimestampTzIsExpectedAndGotTimestamp()
		{
			var actual = BuildConfiguration("timestamp");
			Assume.That(Dialect.Dialect.GetDialect(actual.Properties) is PostgreSQLDialect);

			var expected = BuildConfiguration("timestamptz");

			var export = new SchemaExport(actual);
			export.Create(true, true);

			try
			{
				var validator = new Tool.hbm2ddl.SchemaValidator(expected);

				var error = Assert.Throws<SchemaValidationException>(() => validator.Validate());
				Assert.That(error, Has.Message.EqualTo("Schema validation failed: see list of validation errors"));
				Assert.That(
					error,
					Has.Property("ValidationErrors").Some.Contains("Wrong column type").IgnoreCase
					   .And.Contains("for column CreatedAt. Found: timestamp, Expected timestamptz").IgnoreCase);
			}
			finally
			{
				export.Drop(true, true);
			}
		}

		[Test]
		public void ShouldBeInvalidIfTimestampIsExpectedAndGotTimestampTz()
		{
			var actual = BuildConfiguration("timestamptz");
			Assume.That(Dialect.Dialect.GetDialect(actual.Properties) is PostgreSQLDialect);
				
			var expected = BuildConfiguration("timestamp");

			var export = new SchemaExport(actual);
			export.Create(true, true);

			try
			{
				var validator = new Tool.hbm2ddl.SchemaValidator(expected);

				var error = Assert.Throws<SchemaValidationException>(() => validator.Validate());
				Assert.That(error, Has.Message.EqualTo("Schema validation failed: see list of validation errors"));
				Assert.That(
					error,
					Has.Property("ValidationErrors").Some.Contains("Wrong column type").IgnoreCase
					   .And.Contains("for column CreatedAt. Found: timestamptz, Expected timestamp").IgnoreCase);
			}
			finally
			{
				export.Drop(true, true);
			}
		}

		private static Configuration BuildConfiguration(string type)
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(c =>
			{
				c.Table("Entity");
				c.Id(x => x.Id);
				c.Property(x => x.CreatedAt, p => p.Column(cm => cm.SqlType(type)));
			});

			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddDeserializedMapping(mapper.CompileMappingForAllExplicitlyAddedEntities(), "Entity");
			return cfg;
		}

		public class Entity
		{
			public virtual int Id { get; set; }
			public virtual DateTime CreatedAt { get; set; }
		}
	}
}
