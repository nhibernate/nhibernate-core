using System.Collections;

using NHibernate.Impl;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Logs
{
	using System;
	using System.IO;
	using System.Text;
	using log4net;
	using log4net.Appender;
	using log4net.Core;
	using log4net.Layout;
	using log4net.Repository.Hierarchy;

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
			ThreadContext.Properties["sessionId"] = new SessionIdCapturer();

			using (var spy = new TextLogSpy("NHibernate.SQL", "%message | SessionId: %property{sessionId}"))
			using (var s = sessions.OpenSession())
			{
				var sessionId = ((SessionImpl)s).SessionId;

				s.Get<Person>(1);//will execute some sql

				var loggingEvent = spy.Events[0];
				Assert.That(loggingEvent.Contains(sessionId.ToString()), Is.True);
			}
		}

		public class SessionIdCapturer
		{
			public override string ToString()
			{
				return SessionIdLoggingContext.SessionId.ToString();
			}
		}

		public class TextLogSpy : IDisposable
		{
			private readonly TextWriterAppender appender;
			private readonly Logger loggerImpl;
			private readonly StringBuilder stringBuilder;

			public TextLogSpy(string loggerName, string pattern)
			{
				stringBuilder = new StringBuilder();
				appender = new TextWriterAppender
				{
					Layout = new PatternLayout(pattern),
					Threshold = Level.All,
					Writer = new StringWriter(stringBuilder)
				};
				loggerImpl = (Logger)LogManager.GetLogger(loggerName).Logger;
				loggerImpl.AddAppender(appender);
				loggerImpl.Level = Level.All;
			}

			public string[] Events
			{
				get
				{
					return stringBuilder.ToString().Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
				}
			}

			public void Dispose()
			{
				loggerImpl.RemoveAppender(appender);
			}
		}
	}


}