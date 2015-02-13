using System.Collections.Generic;
using System.Linq;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class CreateTemporaryTableDdlOperationFixture
    {
        public class FixtureDialect : Dialect.Dialect
        {
            public override string CreateTemporaryTableString
            {
                get { return "create temp table"; }
            }

            public override string CreateTemporaryTablePostfix
            {
                get { return "for now..."; }
            }
        }

        [Test]
        public void UsesTempTableExtensions()
        {

            var dialect = new FixtureDialect();
            var model = new CreateTemporaryTableModel
            {
                Name = new DbName("Tim"),
                Columns = new List<ColumnModel>()
                {
                    new ColumnModel
                    {
                        Name = "A",
                        SqlType="varchar over 9000",
                        Nullable = false
                    }
                }
            };
            var operation = new CreateTemporaryTableDdlOperation(model);
            var expected = "create temp table Tim (A varchar over 9000 not null) for now...";
            var actual = operation.GetStatements(dialect).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void IgnoresCheckAndDefault()
        {

            var dialect = new FixtureDialect();
            var model = new CreateTemporaryTableModel
            {
                Name = new DbName("Tim"),
                Columns = new List<ColumnModel>()
                {
                    new ColumnModel
                    {
                        Name = "A",
                        SqlType="varchar over 9000",
                        Nullable = false,
                        CheckConstraint = new ColumnCheckModel{ ColumnName = "A", Expression="Not here"},
                        DefaultValue = "('Not here')"
                    }
                }
            };
            var operation = new CreateTemporaryTableDdlOperation(model);
            var expected = "create temp table Tim (A varchar over 9000 not null) for now...";
            var actual = operation.GetStatements(dialect).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}