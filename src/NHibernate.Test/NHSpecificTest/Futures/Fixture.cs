using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Futures
{
    using System.Collections;

    [TestFixture]
    public class Fixture : TestCase
    {

        protected override IList Mappings
        {
            get { return new string[] { "NHSpecificTest.Futures.Mappings.hbm.xml" }; }
        }

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        [Test]
        public void CanUseFutureCriteria()
        {
            using (var s = sessions.OpenSession())
            {
                if(((SessionFactoryImpl)sessions)
                    .ConnectionProvider.Driver.SupportsMultipleQueries == false)
                {
                    Assert.Ignore("Not applicable for dialects that do not support multiple queries");
                }

                var persons10 = s.CreateCriteria(typeof(Person))
                    .SetMaxResults(10)
                    .Future<Person>();
                var persons5 = s.CreateCriteria(typeof(Person))
                    .SetMaxResults(5)
                    .Future<int>();

                using (var logSpy = new SqlLogSpy())
                {
                    foreach (var person in persons5)
                    {

                    }

                    foreach (var person in persons10)
                    {

                    }

                    var events = logSpy.Appender.GetEvents();
                    Assert.AreEqual(1, events.Length);
                }
            }
        }
    }
}
