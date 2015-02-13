using System.Linq;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class DropSequenceDdlOperationFixture
    {
        [Test]
        public void GetStatements_ReturnsDropSequenceFromDialect()
        {
            var dialect = new MsSql2012Dialect();
            const string sequenceName = "Larry";
            var operation = new DropSequenceDdlOperation(sequenceName);
            var expected = dialect.GetDropSequenceString(sequenceName);
            var actual = operation.GetStatements(dialect).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}