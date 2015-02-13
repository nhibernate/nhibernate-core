using System.Linq;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class AlterTableAddColumnDdlOperationFixture
    {

        [Test]
        public void CanAddColumn()
        {
            var model = new AddOrAlterColumnModel
            {
                Table = new DbName("Bob"),
                Column = new ColumnModel
                {
                    Name = "ImAColumn",
                    Nullable = false,
                    SqlType = "varchar(5)"
                }
            };
            var operation = new AlterTableAddColumnDdlOperation( model);
            var expected = "alter table Bob add column ImAColumn varchar(5) not null";
            var actual = operation.GetStatements(new GenericDialect()).Single();
            Assert.That(actual, Is.EqualTo(expected));

        }

    }
}