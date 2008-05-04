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
            SqlString s = dialect.GetLimitString(sql, 5, 10);
            Assert.AreEqual("SELECT id, name, email FROM Users limit 10, 5", s.ToString());
        }

        [Test]
        public void QuotedSchemaNameWithSqlLite()
        {
            Table tbl = new Table();
            tbl.Schema = "`schema`";
            tbl.Name = "`name`";

            Assert.AreEqual("\"schema_name\"", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("\"schema_table\"", dialect.Qualify("","\"schema\"", "\"table\""));
        }

        [Test]
        public void SchemaNameWithSqlLite()
        {
            Table tbl = new Table();
            tbl.Schema = "schema";
            tbl.Name = "name";

            Assert.AreEqual("schema_name", tbl.GetQualifiedName(dialect));
            Assert.AreEqual("schema_table", dialect.Qualify("","schema","table"));
        }
    }
}
