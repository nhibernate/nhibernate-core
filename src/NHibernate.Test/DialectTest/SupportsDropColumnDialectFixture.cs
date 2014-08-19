using System;

namespace NHibernate.Test.DialectTest
{
    using Dialect;
    using NUnit.Framework;

    [TestFixture]
    public class SupportsDropColumnDialectFixture
    {
        [Test]
        public void GenericDialect_SupportsSupportsDropColumn()
        {
            var dialect = new GenericDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.False);
        }

        [Test]
        public void DB2Dialect_SupportsSupportsDropColumn()
        {
            //http://www-01.ibm.com/support/knowledgecenter/SSEPGG_9.7.0/com.ibm.db2.luw.admin.dbobj.doc/doc/t0020132.html?lang=en
            //http://razorsql.com/features/db2_drop_column.html
            var dialect = new DB2Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void DB2400Dialect_SupportsSupportsDropColumn()
        {
            // Inherits DB2Dialect
            var dialect = new DB2400Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void FirebirdDialect_SupportsSupportsDropColumn()
        {
            //http://razorsql.com/features/firebird_drop_column.html
            //http://stackoverflow.com/questions/20423627/how-do-you-drop-a-column-in-firebird
            var dialect = new FirebirdDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void InformixDialect_SupportsSupportsDropColumn()
        {
            //http://razorsql.com/features/informix_drop_column.html
            //http://publib.boulder.ibm.com/infocenter/idshelp/v10/index.jsp?topic=/com.ibm.sqls.doc/sqls107.htm
            var dialect = new InformixDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void InformixDialect0940_SupportsSupportsDropColumn()
        {
            //Inherits InformixDialect
            var dialect = new InformixDialect0940();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void InformixDialect1000_SupportsSupportsDropColumn()
        {
            //Inherits Informix0940Dialect
            var dialect = new InformixDialect1000();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void IngresDialect_SupportsSupportsDropColumn()
        {
            //http://www.dtsql.com/db/db_ingres.htm#column_drop
            var dialect = new IngresDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);

        }

        [Test]
        public void MsSql2000Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new MsSql2000Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MsSql2005Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new MsSql2005Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MsSql2008Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new MsSql2008Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MsSql2012Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new MsSql2012Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MsSql7Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new MsSql7Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MsSqlAzure2008Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new MsSqlAzure2008Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MsSqlCeDialect_SupportsSupportsDropColumn()
        {
            //http://technet.microsoft.com/en-us/library/ms174123(v=sql.110).aspx
            var dialect = new MsSqlCeDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MsSqlCe40Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new MsSqlCe40Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MySQLDialect_SupportsSupportsDropColumn()
        {
            //http://dev.mysql.com/doc/refman/5.1/en/alter-table.html
            var dialect = new MySQLDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void MySQL5Dialect_SupportsSupportsDropColumn()
        {
            //Inherits MySQLDialect
            var dialect = new MySQL5Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void Oracle8iDialect_SupportsSupportsDropColumn()
        {
            //http://www.dba-oracle.com/t_alter_table_drop_column_syntax_example.htm
            var dialect = new Oracle8iDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void Oracle9iDialect_SupportsSupportsDropColumn()
        {
            var dialect = new Oracle9iDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void Oracle10gDialect_SupportsSupportsDropColumn()
        {
            var dialect = new Oracle10gDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void OracleLiteDialect_SupportsSupportsDropColumn()
        {
            var dialect = new OracleLiteDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void PostgreSQLDialect_SupportsSupportsDropColumn()
        {
            //http://www.postgresql.org/docs/9.0/static/sql-altertable.html
            var dialect = new PostgreSQLDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void PostgreSQL81Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new PostgreSQL81Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void PostgreSQL82Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new PostgreSQL82Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void SQLiteDialect_SupportsSupportsDropColumn()
        {
            //SQLite does not support dropping columns!
            var dialect = new SQLiteDialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.False);
        }

        [Test]
        public void SybaseASA9Dialect_SupportsSupportsDropColumn()
        {
            //http://infocenter.sybase.com/help/index.jsp?topic=/com.sybase.infocenter.dc32300.1550/html/sqlug/X27996.htm
            var dialect = new SybaseASA9Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void SybaseASE15Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new SybaseASE15Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void SybaseSQLAnywhere10Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new SybaseSQLAnywhere10Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void SybaseSQLAnywhere11Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new SybaseSQLAnywhere11Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }

        [Test]
        public void SybaseSQLAnywhere12Dialect_SupportsSupportsDropColumn()
        {
            var dialect = new SybaseSQLAnywhere12Dialect();
            var result = dialect.SupportsDropColumn;
            Assert.That(result, Is.True);
        }
    }
}
