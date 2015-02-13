using System;
using System.Linq;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NUnit.Framework;


namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class DropTableDdlOperationFixture
    {
        [Test]
        public void GetStatements_ReturnsDropTableFromDialect()
        {
            var dialect = new GenericDialect();
            
            var operation = new DropTableDdlOperation(new DbName("`Larry`"));
            var expected = dialect.GetDropTableString("\"Larry\"");
            var actual = operation.GetStatements(dialect).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
