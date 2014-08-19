namespace NHibernate.Test.DialectTest
{
    using Dialect;
    using NUnit.Framework;
    [TestFixture]
    public class BatchTerminator_DialectFixture
    {
        [Test]
        public void GenericDialect_Supports_BatchTerminator()
        {
            var dialect = new GenericDialect();
            var result = dialect.BatchTerminator;
            Assert.That(result, Is.EqualTo(""));
        }
        [Test]
        public void MsSql2000Dialect_Supports_BatchTerminator()
        {
            var dialect = new MsSql2000Dialect();
            var result = dialect.BatchTerminator;
            Assert.That(result, Is.EqualTo("\r\nGO\r\n"));
        }
    }
}
