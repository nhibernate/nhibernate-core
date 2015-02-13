using System.Linq;
using System.Resources;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class AggregateDdlOperationFixture
    {

        [Test]
        public void CanAggregateOperations()
        {
            var operation = new AggregateDdlOperation(new SqlDdlOperation("A"), new SqlDdlOperation("B"));
            var expected = new[] {"A", "B"};
            var actual = operation.GetStatements(new GenericDialect()).ToArray();
            Assert.That(actual, Is.EquivalentTo(expected));
        }

    }
}