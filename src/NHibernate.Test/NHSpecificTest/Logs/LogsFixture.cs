using System.Collections;

using NHibernate.Impl;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Logs
{
    [TestFixture]
    public class LogsFixture : TestCase
	{
		protected override IList Mappings
		{
            get { return new[] { "NHSpecificTest.Logs.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
        public void WillGetSessionIdFromSessionLogs()
		{
            using(var spy = new SqlLogSpy())
		    using(var s = sessions.OpenSession())
		    {
		        var sessionId = ((SessionImpl)s).SessionId;

		        s.Get<Person>(1);//will execute some sql

		        var loggingEvent = spy.Appender.GetEvents()[0];
                Assert.AreEqual(sessionId, loggingEvent.Properties["sessionId"]);
		    }
		}
	}
}