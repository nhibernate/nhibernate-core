using System.Linq;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class DropForeignKeyDdlOperationFixture
    {
        private class FixtureDialect : Dialect.Dialect
        {
            public override string GetIfExistsDropConstraint(string constraintName, string tableName)
            {
                return "if exists";
            }

            public override string GetIfExistsDropConstraintEnd(string constraintName, string tableName)
            {
                return "stsixe fi";
            }
        }

        [Test]
        public void CanDropForeignKey()
        {
            var model = new ForeignKeyModel
            {
                DependentTable = new DbName("Tim"),
                Name = "Bob"
            };
            var dialect = new FixtureDialect();
            var operation = new DropForeignKeyDdlOperation(model);
            var expected = "if exists\r\nalter table Tim  drop constraint Bob\r\nstsixe fi\r\n";
            var actual = operation.GetStatements(dialect).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }


        [Test]
        public void QuotesAllItems()
        {
            var model = new ForeignKeyModel
            {
                DependentTable = new DbName("`Tim`"),
                Name = "`Bob`"
            };
            var dialect = new FixtureDialect();
            var operation = new DropForeignKeyDdlOperation(model);
            var expected = "if exists\r\nalter table \"Tim\"  drop constraint \"Bob\"\r\nstsixe fi\r\n";
            var actual = operation.GetStatements(dialect).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}