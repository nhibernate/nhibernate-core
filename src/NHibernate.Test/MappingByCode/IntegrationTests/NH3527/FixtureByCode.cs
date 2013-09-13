using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3527
{
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		[Test]
		public void VerifyMapping()
		{
			var mapping = this.GetMappings();

			// Item mapping.
			mapping.UnionSubclasses[0].abstractSpecified.Should().Be.True();
			mapping.UnionSubclasses[0].@abstract.Should().Be.True();
			// InventoryItem mapping.
			mapping.UnionSubclasses[1].abstractSpecified.Should().Be.False();
			mapping.UnionSubclasses[1].@abstract.Should().Be.False();
		}

		[Test]
		public void VerifyTables()
		{
			using (var session = this.OpenSession())
			{
				var tableNameColumnName = "TABLE_NAME";
				var itemTableName = "Item";
				var inventoryItemTableName = "InventoryItem";

				var schema = this.Dialect.GetDataBaseSchema((DbConnection) session.Connection);
				var tables = schema.GetTables(null, null, null, null).AsEnumerable();
				var itemTable = tables.SingleOrDefault(
					x => string.Equals(x.Field<string>(tableNameColumnName),
					                   itemTableName,
					                   StringComparison.InvariantCultureIgnoreCase));
				var inventoryItemTable = tables.SingleOrDefault(
					x => string.Equals(x.Field<string>(tableNameColumnName),
					                   inventoryItemTableName,
					                   StringComparison.InvariantCultureIgnoreCase));

				itemTable.Should().Be.Null();
				inventoryItemTable.Should().Not.Be.Null();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2008Dialect;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityBase>(
				m => m.Id(x => x.Id, im => im.Generator(Generators.HighLow)));
			mapper.UnionSubclass<Item>(m => { });
			mapper.UnionSubclass<InventoryItem>(m => { });

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
