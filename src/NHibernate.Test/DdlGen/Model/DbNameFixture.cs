using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.DdlGen.Model;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Model
{
    [TestFixture]
    public class DbNameFixture
    {

        [Test]
        public void QuotesAndQualifiesInFullName()
        {
            var model = new DbName("`nhibernate`", "`dbo`", "`MyTable`");
            var actual = model.QuoteAndQualify(new MsSql2000Dialect());
            var expected = "[nhibernate].[dbo].[MyTable]";
            Assert.That(actual,Is.EqualTo(expected));
        }

        [Test]
        public void DoesntQuoteUnquotedPartInFullName()
        {
            var model = new DbName("`nhibernate`", "dbo", "`MyTable`");
            var actual = model.QuoteAndQualify(new MsSql2000Dialect());
            var expected = "[nhibernate].dbo.[MyTable]";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void QuotesAndQualifiesInTwoPartName()
        {
            var model = new DbName(null, "`dbo`", "`MyTable`");
            
            var actual = model.QuoteAndQualify(new MsSql2000Dialect());
            var expected = "[dbo].[MyTable]";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void DoesntQuoteUnquotedPartInTwoPartName()
        {
            var model = new DbName(null, "dbo", "`MyTable`");
            var actual = model.QuoteAndQualify(new MsSql2000Dialect());
            var expected = "dbo.[MyTable]";
            Assert.That(actual, Is.EqualTo(expected));
        }


        [Test]
        public void QuotesAndQualifiesInOnePartName()
        {
            var model = new DbName(null, null, "`MyTable`");
            var actual = model.QuoteAndQualify(new MsSql2000Dialect());
            var expected = "[MyTable]";
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void DoesntQuoteUnquotedPartInOnePartName()
        {
            var model = new DbName(null, null, "MyTable");
            var actual = model.QuoteAndQualify(new MsSql2000Dialect());
            var expected = "MyTable";
            Assert.That(actual, Is.EqualTo(expected));
        }

        
    }
}
