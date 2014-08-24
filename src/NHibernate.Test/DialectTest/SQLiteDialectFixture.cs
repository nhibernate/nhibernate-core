using NHibernate.Mapping;

namespace NHibernate.Test.DialectTest
{
    using Dialect;
    using NUnit.Framework;
    using SqlCommand;

    [TestFixture]
    public class SQLiteDialectFixture
    {
        private SQLiteDialect dialect;

        [SetUp]
        public void SetUp()
        {
            dialect = new SQLiteDialect();
        }

        [Test]
        public void SupportsSubSelect()
        {
            Assert.IsTrue(dialect.SupportsSubSelects);
        }

        [Test]
        public void UseLimit()
        {
            SqlString sql = new SqlString("SELECT id, name, email FROM Users");
            SqlString s = dialect.GetLimitString(sql, new SqlString("5"), new SqlString("10"));
            Assert.AreEqual("SELECT id, name, email FROM Users limit 10 offset 5", s.ToString());
        }

        [Test]
        public void QuotedSchemaNameWithSqlLite()
        {
            Table tbl = new Table();
            tbl.Schema = "`schema`";
            tbl.Name = "`name`";

            Assert.AreEqual("\"schema_name\"", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("\"schema_table\"", dialect.Qualify("", "\"schema\"", "\"table\""));
        }

        [Test]
        public void QuotedTableNameWithoutSchemaWithSqlLite()
        {
            Table tbl = new Table();
            tbl.Name = "`name`";

            Assert.AreEqual("\"name\"", tbl.GetQualifiedName(dialect));
        }

        [Test]
        public void QuotedSchemaNameWithUnqoutedTableInSqlLite()
        {
            Table tbl = new Table();
            tbl.Schema = "`schema`";
            tbl.Name = "name";

            Assert.AreEqual("\"schema_name\"", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("\"schema_table\"", dialect.Qualify("", "\"schema\"", "table"));
        }

        [Test]
        public void QuotedCatalogSchemaNameWithSqlLite()
        {
            Table tbl = new Table();
            tbl.Catalog = "dbo";
            tbl.Schema = "`schema`";
            tbl.Name = "`name`";

            Assert.AreEqual("\"dbo_schema_name\"", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("\"dbo_schema_table\"", dialect.Qualify("dbo", "\"schema\"", "\"table\""));
        }

        [Test]
        public void QuotedTableNameWithSqlLite()
        {
            Table tbl = new Table();
            tbl.Name = "`Group`";

            Assert.AreEqual("\"Group\"", tbl.GetQualifiedName(dialect));
        }


        [Test]
        public void SchemaNameWithSqlLite()
        {
            Table tbl = new Table();
            tbl.Schema = "schema";
            tbl.Name = "name";

            Assert.AreEqual("schema_name", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("schema_table", dialect.Qualify("", "schema", "table"));
        }
    }
}
