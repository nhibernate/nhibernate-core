using System.Linq;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class SqlDdlOperationFixture
    {
        [Test]
        public void DoesntModifyStuff()
        {
            var expected = Enumerable.Range(1, 10).Select(x => x.ToString()).ToArray();
            var operation = new SqlDdlOperation(expected);
            var actual = operation.GetStatements(new GenericDialect()).ToArray();
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}