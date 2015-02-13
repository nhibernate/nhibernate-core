using System;
using System.Linq;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Dialect;
using NUnit.Framework;


namespace NHibernate.Test.DdlGen.Model
{
    [TestFixture]
    public class ForeignKeyModelFixture
    {
        public class FKModelFixtureDialect : GenericDialect
        {
            private readonly bool _supportsCascadeDelete;

            public FKModelFixtureDialect(bool supportsCascadeDelete)
            {
                _supportsCascadeDelete = supportsCascadeDelete;
                SuppressDefaultFkBuilding = true;
            }

            public override string GetAddForeignKeyConstraintString(string constraintName, string[] foreignKey, string referencedTable,
                string[] primaryKey, bool referencesPrimaryKey)
            {
                if(this.SuppressDefaultFkBuilding) return "AddFK";
                return base.GetAddForeignKeyConstraintString(constraintName, foreignKey, referencedTable, primaryKey,
                                                             referencesPrimaryKey);
            }

            public override bool SupportsCascadeDelete
            {
                get { return _supportsCascadeDelete; }
            }

            public bool SuppressDefaultFkBuilding { get; set; }
        }

        [Test]
        public void ForeignKeyModel_GetConstraintString_AppendsCascadeDelete_WhenSupportedAndSet()
        {
            var dialect = new FKModelFixtureDialect(true);
            var model = new ForeignKeyModel
            {
                ReferencedTable = new DbName("ReferencedTableIsMe"),
                PrimaryKeyColumns = new string[0],
                ForeignKeyColumns = new string[0],
                CascadeDelete = true,
            };
            var str = model.GetConstraintString(dialect);
            Assert.That(str, Is.StringContaining("on delete cascade"));
        }
        [Test]
        public void ForeignKeyModel_GetConstraintString_DoesNotAppendCascadeDelete_WhenNotSupportedAndSet()
        {
            var dialect = new FKModelFixtureDialect(false);
            var model = new ForeignKeyModel
            {
                ReferencedTable = new DbName("ReferencedTableIsMe"),
                PrimaryKeyColumns = new string[0],
                ForeignKeyColumns = new string[0],
                CascadeDelete = true,
            };
            var str = model.GetConstraintString(dialect);
            Assert.That(str, Is.Not.StringContaining("on delete cascade"));
        }

        [Test]
        public void ForeignKeyModel_GetConstraintString_QuotesAndQualifiesTable_AndColumns()
        {
            var dialect = new FKModelFixtureDialect(false);
            dialect.SuppressDefaultFkBuilding = false;
            var model = new ForeignKeyModel
            {
                ReferencedTable = new DbName("`ReferencedTableIsMe`"),
                DependentTable = new DbName("`DependentTableIsMe`"),
                PrimaryKeyColumns = new []{"`Id`"},
                ForeignKeyColumns = new[] { "`ReferencedTableIsMeId`" },
                CascadeDelete = true,
                IsReferenceToPrimaryKey = false
            };
            var actual = model.GetConstraintString(dialect);
            var expected = " add foreign key (\"ReferencedTableIsMeId\") references \"ReferencedTableIsMe\" (\"Id\")";
            Assert.That(actual, Is.EqualTo(expected));

        }
        
    }
}
