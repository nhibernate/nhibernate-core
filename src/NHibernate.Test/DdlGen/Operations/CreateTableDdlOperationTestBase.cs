using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public abstract class CreateTableDdlOperationTestBase
    {
        protected abstract string Expected { get; }

        [Test]
        public virtual void Test()
        {
            var operation = new CreateTableDdlOperation(Model);
            var actual = operation.GetStatements(FixtureDialect).Single();
            Assert.That(actual, Is.EqualTo(Expected));

        }
        protected virtual CreateTableModel Model
        {
            get
            {
                var columns = new List<ColumnModel>()
                {
                    new ColumnModel
                    {
                        Name = "Id",
                        Nullable = false,
                        SqlTypeCode = SqlTypeFactory.Guid,
                    },
                    new ColumnModel
                    {
                        Name = "Name",
                        SqlTypeCode = SqlTypeFactory.GetString(255)
                    }
                };
                return new CreateTableModel
                {
                    Name = new DbName("Larry"),
                    Columns = columns,
                    PrimaryKey = new PrimaryKeyModel
                    {
                        Columns = new List<ColumnModel>{columns[0]},
                        Clustered = true,
                        Identity = false
                    }

                };
            }
        }

        protected virtual CreateTableDdlOperationFixtureDialect FixtureDialect
        {
            get
            {
                var fixtureDialect = new CreateTableDdlOperationFixtureDialect();
                return fixtureDialect;
            }
        }       
    }
}