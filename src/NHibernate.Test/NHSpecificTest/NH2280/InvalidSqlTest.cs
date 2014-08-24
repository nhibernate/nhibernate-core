using System;

using System.Globalization;
using System.IO;
using System.Linq;
using NHibernate.Dialect;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2280
{
    [TestFixture]
    public class InvalidSqlTest : BugTestCase
    {
        [Test]
        public void CompositeKeyTest()
        {
            using (ISession session = OpenSession())
            {
                session.Query<Organisation>().Where(o => o.Codes.Any(c => c.Key.Code == "1476")).ToList();
            }
        }
    }
}
