using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest
{
	[TestFixture]
	public class SchemaMetadataUpdaterFixture
	{
		[Test]
		public void CanRetrieveReservedWords()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			var dialect = Dialect.Dialect.GetDialect(configuration.Properties);
			var connectionHelper = new ManagedProviderConnectionHelper(configuration.Properties);
			connectionHelper.Prepare();
			try
			{
				var metaData = dialect.GetDataBaseSchema(connectionHelper.Connection);
				var reserved = metaData.GetReservedWords();
				Assert.That(reserved, Is.Not.Empty);
				Assert.That(reserved, Has.Member("SELECT"));
				Assert.That(reserved, Has.Member("FROM"));
			}
			finally
			{
				connectionHelper.Release();
			}
		}

		[Test]
		public void UpdateReservedWordsInDialect()
		{
			var reservedDb = new HashSet<string>();
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			var dialect = Dialect.Dialect.GetDialect(configuration.Properties);
			var connectionHelper = new ManagedProviderConnectionHelper(configuration.Properties);
			connectionHelper.Prepare();
			try
			{
				var metaData = dialect.GetDataBaseSchema(connectionHelper.Connection);
				foreach (var rw in metaData.GetReservedWords())
				{
					reservedDb.Add(rw.ToLowerInvariant());
				}
			}
			finally
			{
				connectionHelper.Release();
			}

			var sf = (ISessionFactoryImplementor) configuration.BuildSessionFactory();
			SchemaMetadataUpdater.Update(sf);
			var match = reservedDb.Intersect(sf.Dialect.Keywords);
			Assert.That(match, Is.EquivalentTo(reservedDb));
		}

		[Test]
		public void ExplicitAutoQuote()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
																GetType().Assembly);

			SchemaMetadataUpdater.QuoteTableAndColumns(configuration);

			var cm = configuration.GetClassMapping(typeof(Order));
			Assert.That(cm.Table.IsQuoted);
			var culs = new List<Column>(cm.Table.ColumnIterator);
			Assert.That(GetColumnByName(culs, "From").IsQuoted);
			Assert.That(GetColumnByName(culs, "And").IsQuoted);
			Assert.That(GetColumnByName(culs, "Select").IsQuoted);
			Assert.That(!GetColumnByName(culs, "Name").IsQuoted);
		}

		[Test]
		public void AutoQuoteTableAndColumnsAtStratup()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
			configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
			                          GetType().Assembly);
			configuration.BuildSessionFactory();
			var cm = configuration.GetClassMapping(typeof (Order));
			Assert.That(cm.Table.IsQuoted);
			var culs = new List<Column>(cm.Table.ColumnIterator);
			Assert.That(GetColumnByName(culs, "From").IsQuoted);
			Assert.That(GetColumnByName(culs, "And").IsQuoted);
			Assert.That(GetColumnByName(culs, "Select").IsQuoted);
			Assert.That(!GetColumnByName(culs, "Name").IsQuoted);
		}

		[Test]
		public void AutoQuoteTableAndColumnsAtStratupIncludeKeyWordsImport()
		{
			var reservedDb = new HashSet<string>();
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			var dialect = Dialect.Dialect.GetDialect(configuration.Properties);
			var connectionHelper = new ManagedProviderConnectionHelper(configuration.Properties);
			connectionHelper.Prepare();
			try
			{
				var metaData = dialect.GetDataBaseSchema(connectionHelper.Connection);
				foreach (var rw in metaData.GetReservedWords())
				{
					reservedDb.Add(rw.ToLowerInvariant());
				}
			}
			finally
			{
				connectionHelper.Release();
			}

			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
			configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
																GetType().Assembly);
			var sf = (ISessionFactoryImplementor)configuration.BuildSessionFactory();
			var match = reservedDb.Intersect(sf.Dialect.Keywords);
			Assert.That(match, Is.EquivalentTo(reservedDb));
		}

		private static Column GetColumnByName(IEnumerable<Column> columns, string colName)
		{
			return columns.FirstOrDefault(column => column.Name.Equals(colName));
		}

		[Test]
		public void CanWorkWithAutoQuoteTableAndColumnsAtStratup()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
			configuration.SetProperty(Environment.Hbm2ddlAuto, "create-drop");
			configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
																GetType().Assembly);
			var sf = configuration.BuildSessionFactory();
			using (ISession s = sf.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new Order {From = "from", Column = "column", And = "order"});
				t.Commit();
			}

			using (ISession s = sf.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Order");
				t.Commit();
			}

			new SchemaExport(configuration).Drop(false, false);
		}

		[Test]
		public void WhenConfiguredOnlyExplicitAutoQuote()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			var configuredDialect = Dialect.Dialect.GetDialect();
			if(!configuredDialect.DefaultProperties.ContainsKey(Environment.ConnectionDriver))
			{
				Assert.Ignore(GetType() + " does not apply to " + configuredDialect);
			}
			configuration.Properties.Remove(Environment.ConnectionDriver);
			configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
																GetType().Assembly);

			SchemaMetadataUpdater.QuoteTableAndColumns(configuration);

			var cm = configuration.GetClassMapping(typeof(Order));
			Assert.That(cm.Table.IsQuoted);
			var culs = new List<Column>(cm.Table.ColumnIterator);
			Assert.That(GetColumnByName(culs, "From").IsQuoted);
			Assert.That(GetColumnByName(culs, "And").IsQuoted);
			Assert.That(GetColumnByName(culs, "Select").IsQuoted);
			Assert.That(!GetColumnByName(culs, "Name").IsQuoted);
		}
	}
}