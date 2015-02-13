using System;
using System.Linq;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class DropIndexDdlOperationFixture
    {
        private class FixtureDialect : Dialect.Dialect
        {
            private readonly bool _supportsUniqueConstraintInAlterTable;
            private readonly bool _supportsIfExistDrop;

            public FixtureDialect(bool supportsUniqueConstraintInAlterTable, bool supportsIfExistDrop)
            {
                _supportsUniqueConstraintInAlterTable = supportsUniqueConstraintInAlterTable;
                _supportsIfExistDrop = supportsIfExistDrop;
            }

            public override bool SupportsUniqueConstraintInAlterTable
            {
                get { return _supportsUniqueConstraintInAlterTable; }
            }

            public override string GetIfExistsDropConstraint(string constraintName, string tableName)
            {
                if (!_supportsIfExistDrop)
                {
                    return base.GetIfExistsDropConstraint(constraintName, tableName);
                }
                return String.Format("if constraint_exists('{0}','{1}')", constraintName, tableName);
            }
        }

        [Test]
        public void CanDropUnique()
        {
            var model = new IndexModel
            {
                TableName = new DbName("Larry"),
                Name = "Bob",
                Unique = true
            };
            var dialect = new FixtureDialect(true, false);
            var operation = new DropIndexDdlOperation(model, false);
            var actual = operation.GetStatements(dialect).First();
            var expected = "alter table Larry drop constraint Bob";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanDropUnique_WithIfExists()
        {
            var model = new IndexModel
            {
                TableName = new DbName("Larry"),
                Name = "Bob",
                Unique = true
            };
            var dialect = new FixtureDialect(true, true);
            var operation = new DropIndexDdlOperation(model, true);
            var actual = operation.GetStatements(dialect).First();
            
            var expected = "if constraint_exists('Bob','Larry') \r\nalter table Larry drop constraint Bob\r\n";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanDropIndex()
        {
            var model = new IndexModel
            {
                TableName = new DbName("Larry"),
                Name = "Bob",
                Unique = false
            };
            var dialect = new FixtureDialect(true, false);
            var operation = new DropIndexDdlOperation(model, false);
            var actual = operation.GetStatements(dialect).First();
            var expected = "drop index Larry.Bob";
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}