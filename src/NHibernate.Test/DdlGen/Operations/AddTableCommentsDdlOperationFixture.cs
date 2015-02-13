using System.Collections.Generic;
using System.Linq;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class AddTableCommentsDdlOperationFixture
    {
        private class FixtureDialect : Dialect.Dialect
        {
            public override bool SupportsCommentOn
            {
                get { return true; }
            }
        }

        [Test]
        public void CanCommentOnTable()
        {
            var dialect = new FixtureDialect();
            var model = new TableCommentsModel
            {
                TableName = new DbName("Tim"),
                Comment = "I am so smart"
            };
            var operation = new AddTableCommentsDdlOperation(model);
            var expected = "comment on table Tim is 'I am so smart'";
            var actual = operation.GetStatements(dialect).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
        [Test]
        public void CanCommentOnColumn()
        {
            var dialect = new FixtureDialect();
            var model = new TableCommentsModel
            {
                TableName = new DbName("Tim"),
                Columns = new List<ColumnCommentModel>
                {
                    new ColumnCommentModel
                    {
                        ColumnName = "Bob",
                        Comment = "this is a column"
                    }
                }
            };
            var operation = new AddTableCommentsDdlOperation(model);
            var expected = "comment on column Tim.Bob is 'this is a column'";
            var actual = operation.GetStatements(dialect).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }

    }
}