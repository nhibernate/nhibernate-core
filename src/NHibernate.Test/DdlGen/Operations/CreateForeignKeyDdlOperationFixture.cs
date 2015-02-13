using System.Linq;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class CreateForeignKeyDdlOperationFixture
    {

        [Test]
        public void CanCreateFk_ToPK_WithoutCascade()
        {
            var model = new ForeignKeyModel
            {
                Name = "FK_Yoyo_to_Frisbee",
                CascadeDelete = false,
                DependentTable = new DbName("Yoyo"),
                ForeignKeyColumns = new[] {"FrisbeeId"},
                ReferencedTable = new DbName("Frisbee"),
                IsReferenceToPrimaryKey = true,
                PrimaryKeyColumns = new[] {"Id"}

            };
            var operation = new CreateForeignKeyOperation(model);
            var actual = operation.GetStatements(new GenericDialect()).Single();
            var expected = "alter table Yoyo  add constraint FK_Yoyo_to_Frisbee foreign key (FrisbeeId) references Frisbee";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanCreateFk_ToPK_WithCascade()
        {
            var model = new ForeignKeyModel
            {
                Name = "FK_Yoyo_to_Frisbee",
                CascadeDelete = true,
                DependentTable = new DbName("Yoyo"),
                ForeignKeyColumns = new[] { "FrisbeeId" },
                ReferencedTable = new DbName("Frisbee"),
                IsReferenceToPrimaryKey = true,
                PrimaryKeyColumns = new[] { "Id" }

            };
            var operation = new CreateForeignKeyOperation(model);
            var actual = operation.GetStatements(new GenericDialect()).Single();
            var expected = "alter table Yoyo  add constraint FK_Yoyo_to_Frisbee foreign key (FrisbeeId) references Frisbee on delete cascade ";
            Assert.That(actual, Is.EqualTo(expected));
        }


        [Test]
        public void CanCreateFk_ToUK_WithoutCascade()
        {
            var model = new ForeignKeyModel
            {
                Name = "FK_Yoyo_to_Frisbee",
                CascadeDelete = false,
                DependentTable = new DbName("Yoyo"),
                ForeignKeyColumns = new[] { "FrisbeeId" },
                ReferencedTable = new DbName("Frisbee"),
                IsReferenceToPrimaryKey = false,
                PrimaryKeyColumns = new[] { "Id2" }

            };
            var operation = new CreateForeignKeyOperation(model);
            var actual = operation.GetStatements(new GenericDialect()).Single();
            var expected = "alter table Yoyo  add constraint FK_Yoyo_to_Frisbee foreign key (FrisbeeId) references Frisbee (Id2)";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanCreateFk_ToUK_WithCascade()
        {
            var model = new ForeignKeyModel
            {
                Name = "FK_Yoyo_to_Frisbee",
                CascadeDelete = true,
                DependentTable = new DbName("Yoyo"),
                ForeignKeyColumns = new[] { "FrisbeeId" },
                ReferencedTable = new DbName("Frisbee"),
                IsReferenceToPrimaryKey = false,
                PrimaryKeyColumns = new[] { "Id2" }

            };
            var operation = new CreateForeignKeyOperation(model);
            var actual = operation.GetStatements(new GenericDialect()).Single();
            var expected = "alter table Yoyo  add constraint FK_Yoyo_to_Frisbee foreign key (FrisbeeId) references Frisbee (Id2) on delete cascade ";
            Assert.That(actual, Is.EqualTo(expected));
        }


        [Test]
        public void ReturnsNoStatementsWhenNotSupportedByDialect()
        {
            var model = new ForeignKeyModel
            {
                Name = "FK_Yoyo_to_Frisbee",
                CascadeDelete = true,
                DependentTable = new DbName("Yoyo"),
                ForeignKeyColumns = new[] { "FrisbeeId" },
                ReferencedTable = new DbName("Frisbee"),
                IsReferenceToPrimaryKey = false,
                PrimaryKeyColumns = new[] { "Id2" }

            };
            var operation = new CreateForeignKeyOperation(model);
            var actual = operation.GetStatements(new SQLiteDialect()).ToList();
            
            Assert.That(actual.Count,Is.EqualTo(0));
        }
    }
}