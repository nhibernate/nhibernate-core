using System.Linq;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    public class CreateIndexDdlOperationFixture
    {

        private class FixtureDialect : GenericDialect
        {
            private readonly bool _supportsUniqueConstraintInAlterTable;

            public FixtureDialect(bool supportsUniqueConstraintInAlterTable)
            {
                _supportsUniqueConstraintInAlterTable = supportsUniqueConstraintInAlterTable;
            }

            public override bool SupportsUniqueConstraintInAlterTable
            {
                get { return _supportsUniqueConstraintInAlterTable; }
            }
        }

        [Test]
        public void CanCreateIndex()
        {
            var model = new IndexModel
            {
                TableName = new DbName("Larry"),
                Name = "IX_Larry_Name",
                Unique = false,
                Columns = new[]
                {
                    "Name"
                }
            };
            var operation = new CreateIndexDdlOperation(model);
            var expected = "create index IX_Larry_Name on Larry (Name)";
            var actual = operation.GetStatements(new FixtureDialect(false)).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void CreateIndexOperationQuotesNames()
        {
            var model = new IndexModel
            {
                TableName = new DbName("Larry"),
                Name = "`IX_Larry_Name`",
                Unique = false,
                Columns = new[]
                {
                    "`Name`"
                }
            };
            var operation = new CreateIndexDdlOperation(model);
            var expected = "create index \"IX_Larry_Name\" on Larry (\"Name\")";
            var actual = operation.GetStatements(new FixtureDialect(false)).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanCreateClusteredIndex()
        {
            var model = new IndexModel
            {
                TableName = new DbName("Larry"),
                Name = "IX_Larry_Name",
                Unique = false,
                Clustered = true,
                Columns = new[]
                {
                     "Name"
                }
            };
            var operation = new CreateIndexDdlOperation(model);
            var expected = "create clustered index IX_Larry_Name on Larry (Name)";
            var actual = operation.GetStatements(new FixtureDialect(false)).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }


        [Test]
        public void CanCreateUniqeIndex_WhenSupportsUniqueConstratintInAlterTable_IsFalse()
        {
            var model = new IndexModel
            {
                TableName = new DbName("Larry"),
                Name = "IX_Larry_Name",
                Unique = true,
                
                Columns = new[]
                {
                    "Name"
                }
            };
            var operation = new CreateIndexDdlOperation(model);
            var expected = "create unique index IX_Larry_Name on Larry (Name)";
            var actual = operation.GetStatements(new FixtureDialect(false)).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanCreateUniqeIndex_WhenSupportsUniqueConstratintInAlterTable_IsTrue()
        {
            var model = new IndexModel
            {
                TableName = new DbName("Larry"),
                Name = "IX_Larry_Name",
                Unique = true,

                Columns = new[]
                {
                    "Name"
                }
            };
            var operation = new CreateIndexDdlOperation(model);
            var expected = "alter table Larry add constraint IX_Larry_Name unique (Name)";
            var actual = operation.GetStatements(new FixtureDialect(true)).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}