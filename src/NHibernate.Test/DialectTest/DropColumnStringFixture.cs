using System;

namespace NHibernate.Test.DialectTest
{
    using Dialect;
    using NUnit.Framework;
    [TestFixture]
    public class DropColumnStringDialectFixture
    {
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void GenericDialect_SupportsDropColumnString()
        {
            var dialect = new GenericDialect();
            var result = dialect.DropColumnString;
            
        }

        [Test]
        public void DB2Dialect_SupportsDropColumnString()
        {
            var dialect = new DB2Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void DB2400Dialect_SupportsDropColumnString()
        {
            var dialect = new DB2400Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void FirebirdDialect_SupportsDropColumnString()
        {
            var dialect = new FirebirdDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }

        [Test]
        public void InformixDialect_SupportsDropColumnString()
        {
            var dialect = new InformixDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void InformixDialect0940_SupportsDropColumnString()
        {
            var dialect = new InformixDialect0940();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void InformixDialect1000_SupportsDropColumnString()
        {
            var dialect = new InformixDialect1000();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void IngresDialect_SupportsDropColumnString()
        {
            var dialect = new IngresDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void MsSql2000Dialect_SupportsDropColumnString()
        {
            var dialect = new MsSql2000Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void MsSql2005Dialect_SupportsDropColumnString()
        {
            var dialect = new MsSql2005Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void MsSql2008Dialect_SupportsDropColumnString()
        {
            var dialect = new MsSql2008Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void MsSql2012Dialect_SupportsDropColumnString()
        {
            var dialect = new MsSql2012Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void MsSql7Dialect_SupportsDropColumnString()
        {
            var dialect = new MsSql7Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void MsSqlAzure2008Dialect_SupportsDropColumnString()
        {
            var dialect = new MsSqlAzure2008Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void MsSqlCeDialect_SupportsDropColumnString()
        {
            var dialect = new MsSqlCeDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }
        [Test]
        public void MsSqlCe40Dialect_SupportsDropColumnString()
        {
            var dialect = new MsSqlCe40Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }
        [Test]
        public void MySQLDialect_SupportsDropColumnString()
        {
            var dialect = new MySQLDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }
        [Test]
        public void MySQL5Dialect_SupportsDropColumnString()
        {
            var dialect = new MySQL5Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }
        [Test]
        public void Oracle8iDialect_SupportsDropColumnString()
        {
            var dialect = new Oracle8iDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void Oracle9iDialect_SupportsDropColumnString()
        {
            var dialect = new Oracle9iDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void Oracle10gDialect_SupportsDropColumnString()
        {
            var dialect = new Oracle10gDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void OracleLiteDialect_SupportsDropColumnString()
        {
            var dialect = new OracleLiteDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void PostgreSQLDialect_SupportsDropColumnString()
        {
            var dialect = new PostgreSQLDialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void PostgreSQL81Dialect_SupportsDropColumnString()
        {
            var dialect = new PostgreSQL81Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test]
        public void PostgreSQL82Dialect_SupportsDropColumnString()
        {
            var dialect = new PostgreSQL82Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop column "));
        }
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void SQLiteDialect_SupportsDropColumnString()
        {
            var dialect = new SQLiteDialect();
            var result = dialect.DropColumnString;
            
        }
        [Test]
        public void SybaseASA9Dialect_SupportsDropColumnString()
        {
            var dialect = new SybaseASA9Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }

        [Test]
        public void SybaseASE15Dialect_SupportsDropColumnString()
        {
            var dialect = new SybaseASE15Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }
        [Test]
        public void SybaseSQLAnywhere10Dialect_SupportsDropColumnString()
        {
            var dialect = new SybaseSQLAnywhere10Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }
        [Test]
        public void SybaseSQLAnywhere11Dialect_SupportsDropColumnString()
        {
            var dialect = new SybaseSQLAnywhere11Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }
        [Test]
        public void SybaseSQLAnywhere12Dialect_SupportsDropColumnString()
        {
            var dialect = new SybaseSQLAnywhere12Dialect();
            var result = dialect.DropColumnString;
            Assert.That(result, Is.EqualTo("drop "));
        }
    }
}
