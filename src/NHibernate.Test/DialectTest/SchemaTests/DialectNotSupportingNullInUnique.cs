﻿using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest.SchemaTests
{
	public class DialectNotSupportingNullInUnique : GenericDialect
	{
		public override bool SupportsNullInUnique => false;
	}

	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	[TestFixture]
	public class DialectNotSupportingNullInUniqueFixture
	{
		protected HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name, m => m.Unique(true));
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void ScriptGenerationForDialectNotSupportingNullInUnique()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			configuration.AddMapping(GetMappings());

			var script = configuration.GenerateSchemaCreationScript(new DialectNotSupportingNullInUnique());

			Assert.That(script, Has.None.Contains("unique"));
		}
	}
}