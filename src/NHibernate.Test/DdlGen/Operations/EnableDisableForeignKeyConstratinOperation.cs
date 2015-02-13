using System.Linq;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class EnableDisableForeignKeyConstratinOperation
    {

        private class FixtureDialect : GenericDialect
        {
            public override string DisableForeignKeyConstraintsString
            {
                get { return "disable fks"; }
            }

            public override string EnableForeignKeyConstraintsString
            {
                get { return "enable fks"; }
            }
        }
        [Test]
        public void CanDisableFks()
        {
            var operation = new DisableForeignKeyConstraintDdlOperation();
            var expected = "disable fks";
            var actual = operation.GetStatements(new FixtureDialect()).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanEnableFks()
        {
            var operation = new EnableForeignKeyConstratintDdlOperation();
            var expected = "enable fks";
            var actual = operation.GetStatements(new FixtureDialect()).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}