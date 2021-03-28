using System.Collections.Generic;
using NHibernate.Dialect.Function;
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
        private SQLFunctionRegistry registry;

        [SetUp]
        public void SetUp()
        {
            dialect = new SQLiteDialect();
            registry = new SQLFunctionRegistry(dialect, new Dictionary<string, ISQLFunction>());
        }

        [Test]
        public void SupportsSubSelect()
        {
            Assert.IsTrue(dialect.SupportsSubSelects);
        }

        [TestCase("int"), TestCase("integer"), TestCase("tinyint"), TestCase("smallint"), TestCase("bigint")]
        [TestCase("numeric"), TestCase("decimal"), TestCase("bit"), TestCase("money"), TestCase("smallmoney")]
        [TestCase("float"), TestCase("real"), TestCase("date"), TestCase("datetime"), TestCase("smalldatetime")]
        [TestCase("char"), TestCase("varchar"), TestCase("text"), TestCase("nvarchar"), TestCase("ntext")]
        [TestCase("binary"), TestCase("varbinary"), TestCase("image")]
        [TestCase("cursor"), TestCase("timestamp"), TestCase("uniqueidentifier"), TestCase("sql_variant")]
        public void WhereStringTemplate(string type)
        {
            if (!type.EndsWith(")"))
                WhereStringTemplate(type + "(1)");

            const string value = "1";
            string sql = "(CAST(" + value + " AS " + type + "))";
            Assert.AreEqual(sql, Template.RenderWhereStringTemplate(sql, dialect, registry));
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
