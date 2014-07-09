using System;

namespace NHibernate.Test.DialectTest
{
    using Dialect;
    using NUnit.Framework;
    [TestFixture]
    public class AlterColumn_DialectFixture
    {
        [Test]
        public void GenericDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new GenericDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(false));
        }
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void GenericDialect_Supports_AlterColumnString()
        {
            var dialect = new GenericDialect();
            var result = dialect.AlterColumnString;
          
        }


        [Test]
        public void DB2Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new DB2Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void DB2Dialect_Supports_AlterColumnString()
        {
            var dialect = new DB2Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void DB2400Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new DB2400Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void DB2400Dialect_Supports_AlterColumnString()
        {
            var dialect = new DB2400Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void FirebirdDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new FirebirdDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(false));
        }
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void FirebirdDialect_Supports_AlterColumnString()
        {
            var dialect = new FirebirdDialect();
            var result = dialect.AlterColumnString;
            
        }

        [Test]
        public void InformixDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new InformixDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }


        [Test]
        public void InformixDialect_Supports_AlterColumnString()
        {
            var dialect = new InformixDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void InformixDialect0940_Supports_SupportsAlterColumn()
        {
            var dialect = new InformixDialect0940();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void InformixDialect0940_Supports_AlterColumnString()
        {
            var dialect = new InformixDialect0940();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void InformixDialect1000_Supports_SupportsAlterColumn()
        {
            var dialect = new InformixDialect1000();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void InformixDialect1000_Supports_AlterColumnString()
        {
            var dialect = new InformixDialect1000();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void IngresDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new IngresDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void IngresDialect_Supports_AlterColumnString()
        {
            var dialect = new IngresDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MsSql2000Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MsSql2000Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MsSql2000Dialect_Supports_AlterColumnString()
        {
            var dialect = new MsSql2000Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MsSql2005Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MsSql2005Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MsSql2005Dialect_Supports_AlterColumnString()
        {
            var dialect = new MsSql2005Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MsSql2008Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MsSql2008Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MsSql2008Dialect_Supports_AlterColumnString()
        {
            var dialect = new MsSql2008Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MsSql2012Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MsSql2012Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MsSql2012Dialect_Supports_AlterColumnString()
        {
            var dialect = new MsSql2012Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MsSql7Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MsSql7Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MsSql7Dialect_Supports_AlterColumnString()
        {
            var dialect = new MsSql7Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MsSqlAzure2008Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MsSqlAzure2008Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MsSqlAzure2008Dialect_Supports_AlterColumnString()
        {
            var dialect = new MsSqlAzure2008Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MsSqlCeDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MsSqlCeDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MsSqlCeDialect_Supports_AlterColumnString()
        {
            var dialect = new MsSqlCeDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MsSqlCe40Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MsSqlCe40Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MsSqlCe40Dialect_Supports_AlterColumnString()
        {
            var dialect = new MsSqlCe40Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MySQLDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MySQLDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MySQLDialect_Supports_AlterColumnString()
        {
            var dialect = new MySQLDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void MySQL5Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new MySQL5Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void MySQL5Dialect_Supports_AlterColumnString()
        {
            var dialect = new MySQL5Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void Oracle8iDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new Oracle8iDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void Oracle8iDialect_Supports_AlterColumnString()
        {
            var dialect = new Oracle8iDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void Oracle9iDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new Oracle9iDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void Oracle9iDialect_Supports_AlterColumnString()
        {
            var dialect = new Oracle9iDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void Oracle10gDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new Oracle10gDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void Oracle10gDialect_Supports_AlterColumnString()
        {
            var dialect = new Oracle10gDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void OracleLiteDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new OracleLiteDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void OracleLiteDialect_Supports_AlterColumnString()
        {
            var dialect = new OracleLiteDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void PostgreSQLDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new PostgreSQLDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void PostgreSQLDialect_Supports_AlterColumnString()
        {
            var dialect = new PostgreSQLDialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void PostgreSQL81Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new PostgreSQL81Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void PostgreSQL81Dialect_Supports_AlterColumnString()
        {
            var dialect = new PostgreSQL81Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void PostgreSQL82Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new PostgreSQL82Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void PostgreSQL82Dialect_Supports_AlterColumnString()
        {
            var dialect = new PostgreSQL82Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("alter column "));
        }
        [Test]
        public void SQLiteDialect_Supports_SupportsAlterColumn()
        {
            var dialect = new SQLiteDialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(false));
        }
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void SQLiteDialect_Supports_AlterColumnString()
        {
            var dialect = new SQLiteDialect();
            var result = dialect.AlterColumnString;
            
        }
        [Test]
        public void SybaseASA9Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new SybaseASA9Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void SybaseASA9Dialect_Supports_AlterColumnString()
        {
            var dialect = new SybaseASA9Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }

        [Test]
        public void SybaseASE15Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new SybaseASE15Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void SybaseASE15Dialect_Supports_AlterColumnString()
        {
            var dialect = new SybaseASE15Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void SybaseSQLAnywhere10Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new SybaseSQLAnywhere10Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void SybaseSQLAnywhere10Dialect_Supports_AlterColumnString()
        {
            var dialect = new SybaseSQLAnywhere10Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void SybaseSQLAnywhere11Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new SybaseSQLAnywhere11Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void SybaseSQLAnywhere11Dialect_Supports_AlterColumnString()
        {
            var dialect = new SybaseSQLAnywhere11Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
        [Test]
        public void SybaseSQLAnywhere12Dialect_Supports_SupportsAlterColumn()
        {
            var dialect = new SybaseSQLAnywhere12Dialect();
            var result = dialect.SupportsAlterColumn;
            Assert.That(result, Is.EqualTo(true));
        }
        [Test]
        public void SybaseSQLAnywhere12Dialect_Supports_AlterColumnString()
        {
            var dialect = new SybaseSQLAnywhere12Dialect();
            var result = dialect.AlterColumnString;
            Assert.That(result, Is.EqualTo("modify "));
        }
    }
}
