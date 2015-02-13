using System.Collections.Generic;
using NHibernate.DdlGen.Model;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Model
{
    [TestFixture]
    public class IndexModelFixture
    {

        [Test]
        public void CanGetColumnsList_WithOneColumn()
        {
            var model = new IndexModel
            {
                Columns = new[] { "Tim" }
            };
            var lst = model.GetColumnList(new GenericDialect());
            Assert.That(lst, Is.EqualTo("(Tim)"));
        }

        [Test]
        public void CanGetColumnsList_WithQuotedNames()
        {
            var model = new IndexModel
            {
                Columns = new[] { "`Tim`" }
            };
            var lst = model.GetColumnList(new GenericDialect());
            Assert.That(lst, Is.EqualTo("(\"Tim\")"));
        }

        [Test]
        public void CanGetColumnsList_WithManyColumn()
        {
            var model = new IndexModel
            {
                Columns = new[]{ "Tim", "Larry" }
            };
            var lst = model.GetColumnList(new GenericDialect());
            Assert.That(lst, Is.EqualTo("(Tim, Larry)"));
        }
    }
}