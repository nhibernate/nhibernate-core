namespace NHibernate.Test.DialectTest
{
	using Dialect;
	using Mapping;
	using NUnit.Framework;

	[TestFixture]
	public class SqlCEDialectFixture
	{
        private MsSqlCeDialect dialect;

        [SetUp]
        public void SetUp()
        {
            dialect = new MsSqlCeDialect();
        }
        
        [Test]
		public void BinaryBlob_mapping_to_SqlCe_types()
		{
			SimpleValue sv = new SimpleValue();
			sv.TypeName = NHibernateUtil.BinaryBlob.Name;
			Column column = new Column();
			column.Value = sv;

			// no length, should produce maximum
			Assert.AreEqual("VARBINARY(8000)", column.GetSqlType(dialect, null));

			// maximum varbinary length is 8000
			column.Length = 8000;
			Assert.AreEqual("VARBINARY(8000)", column.GetSqlType(dialect,null));

			column.Length = 8001;
			Assert.AreEqual("IMAGE", column.GetSqlType(dialect, null));
		}
    
        [Test]
        public void QuotedSchemaNameWithSqlCE()
        {
            Table tbl = new Table();
            tbl.Schema = "`schema`";
            tbl.Name = "`name`";

            Assert.AreEqual("\"schema_name\"", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("\"schema_table\"", dialect.Qualify("", "\"schema\"", "\"table\""));
        }

        [Test]
        public void QuotedTableNameWithoutSchemaWithSqlCE()
        {
            Table tbl = new Table();
            tbl.Name = "`name`";

            Assert.AreEqual("\"name\"", tbl.GetQualifiedName(dialect));
        }

        [Test]
        public void QuotedSchemaNameWithUnqoutedTableInSqlCE()
        {
            Table tbl = new Table();
            tbl.Schema = "`schema`";
            tbl.Name = "name";

            Assert.AreEqual("\"schema_name\"", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("\"schema_table\"", dialect.Qualify("", "\"schema\"", "table"));
        }

        [Test]
        public void QuotedCatalogSchemaNameWithSqlCE()
        {
            Table tbl = new Table();
            tbl.Catalog = "dbo";
            tbl.Schema = "`schema`";
            tbl.Name = "`name`";

            Assert.AreEqual("dbo.\"schema_name\"", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("dbo.\"schema_table\"", dialect.Qualify("dbo", "\"schema\"", "\"table\""));
        }

        [Test]
        public void QuotedTableNameWithSqlCE()
        {
            Table tbl = new Table();
            tbl.Name = "`Group`";

            Assert.AreEqual("\"Group\"", tbl.GetQualifiedName(dialect));
        }

        [Test]
        public void SchemaNameWithSqlCE()
        {
            Table tbl = new Table();
            tbl.Schema = "schema";
            tbl.Name = "name";

            Assert.AreEqual("schema_name", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("schema_table", dialect.Qualify("", "schema", "table"));
        }
    }
}