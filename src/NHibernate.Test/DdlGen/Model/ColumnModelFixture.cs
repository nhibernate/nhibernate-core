using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Dialect;
using NHibernate.SqlTypes;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Model
{
    [TestFixture]
    public class ColumnModelFixture
    {
        [Test]
        public void GetSqlType_ReturnsRawType_WhenSpecified()
        {
            var expected = "Yoyo";
            var model = new ColumnModel() {SqlType = expected};
            var actual = model.GetSqlType(new GenericDialect());
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetSqlType_ResolvesSqlTypeCode_ThroughDialect_WhenNoTypeExplicitlySpecifiedd()
        {
            var model = new ColumnModel {SqlTypeCode = SqlTypeFactory.DateTime};
            var expected = "DATETIME";
            var actual = model.GetSqlType(new GenericDialect());
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void AppendTemporaryTableDefinition_RespectsNotNull()
        {
            var model = new ColumnModel
            {
                Name = "Tim",
                SqlTypeCode = SqlTypeFactory.DateTime,
                Nullable = false
            };
            var actual = new StringBuilder();
            model.AppendTemporaryTableColumnDefinition(actual, new GenericDialect());
            Assert.That(actual.ToString(), Is.EqualTo("Tim DATETIME not null"));
        }

        [Test]
        public void AppendTemporaryTableDefinition_RespectsNull()
        {
            var model = new ColumnModel
            {
                Name = "Tim",
                SqlTypeCode = SqlTypeFactory.DateTime,
                Nullable = true
            };
            var actual = new StringBuilder();
            model.AppendTemporaryTableColumnDefinition(actual, new GenericDialect());
            Assert.That(actual.ToString(), Is.EqualTo("Tim DATETIME "));
        }

        [Test]
        public void AppendTemporaryTableDefinition_QuotesName()
        {
            var model = new ColumnModel
            {
                Name = "`Tim`",
                SqlTypeCode = SqlTypeFactory.DateTime,
                Nullable = true
            };
            var actual = new StringBuilder();
            model.AppendTemporaryTableColumnDefinition(actual, new GenericDialect());
            Assert.That(actual.ToString(), Is.EqualTo("\"Tim\" DATETIME "));
        }

        [Test]
        public void AppendDefinition_QuotesName()
        {
            var model = new ColumnModel
            {
                Name = "`Tim`",
                SqlTypeCode = SqlTypeFactory.DateTime,
                Nullable = false
            };
            var actual = new StringBuilder();
            model.AppendColumnDefinitinon(actual, new GenericDialect(), false);
            Assert.That(actual.ToString(), Is.EqualTo("\"Tim\" DATETIME not null"));
        }

        [Test]
        public void AppendDefinition_RespectsNotNull()
        {
            var model = new ColumnModel
            {
                Name = "Tim",
                SqlTypeCode = SqlTypeFactory.DateTime,
                Nullable = false
            };
            var actual = new StringBuilder();
            model.AppendColumnDefinitinon(actual, new GenericDialect(), false);
            Assert.That(actual.ToString(), Is.EqualTo("Tim DATETIME not null"));
        }

        [Test]
        public void AppendDefinition_RespectsNull()
        {
            var model = new ColumnModel
            {
                Name = "Tim",
                SqlTypeCode = SqlTypeFactory.DateTime,
                Nullable = true
            };
            var actual = new StringBuilder();
            model.AppendColumnDefinitinon(actual, new GenericDialect(), false);
            Assert.That(actual.ToString(), Is.EqualTo("Tim DATETIME"));
        }


        [Test]
        public void AppendDefinition_ouputs_DefaultValues()
        {
            var model = new ColumnModel
            {
                Name = "Tim",
                SqlTypeCode = SqlTypeFactory.DateTime,
                Nullable = true,
                DefaultValue = "(getdate())"
            };
            var actual = new StringBuilder();
            model.AppendColumnDefinitinon(actual, new GenericDialect(), false);
            Assert.That(actual.ToString(), Is.EqualTo("Tim DATETIME default (getdate()) "));
        }


        [Test]
        public void AppendDefinition_ouputs_CheckConstraints()
        {
            var model = new ColumnModel
            {
                Name = "Tim",
                SqlTypeCode = SqlTypeFactory.DateTime,
                Nullable = true,
                CheckConstraint = new ColumnCheckModel
                {
                    Expression = "larry"
                }
            };
            var actual = new StringBuilder();
            model.AppendColumnDefinitinon(actual, new GenericDialect(), false);
            Assert.That(actual.ToString(), Is.EqualTo("Tim DATETIME check( larry) "));
        }
    }
}