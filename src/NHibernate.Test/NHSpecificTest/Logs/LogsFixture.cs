using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
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
			GlobalContext.Properties["sessionId"] = new SessionIdCapturer();

			using (var spy = new TextLogSpy("NHibernate.SQL", "%message | SessionId: %property{sessionId}"))
			using (var s = Sfi.OpenSession())
			{
				var sessionId = ((SessionImpl)s).SessionId;

				s.Get<Person>(1);//will execute some sql

				var loggingEvent = spy.GetWholeLog();
				Assert.That(loggingEvent.Contains(sessionId.ToString()), Is.True);
			}
		}

		[Test]
		public void WillGetSessionIdFromSessionLogsConcurrent()
		{
			GlobalContext.Properties["sessionId"] = new SessionIdCapturer();

			var semaphore = new ManualResetEventSlim();
			var failures = new ConcurrentBag<Exception>();
			var sessionIds = new ConcurrentDictionary<int, Guid>();
			var array = Enumerable.Range(1, 10).Select(
				i => new Thread(
					() =>
					{
						try
						{
							using (var s = Sfi.OpenSession())
							{
								sessionIds.AddOrUpdate(
									i,
									s.GetSessionImplementation().SessionId,
									(ti, old) => throw new InvalidOperationException(
										$"Thread number {ti} has already session id {old}, while attempting to set it to" +
										$" {s.GetSessionImplementation().SessionId}"));
								semaphore.Wait();

								for (int j = 0; j < 10; j++)
								{
									s.Get<Person>(i * 10 + j); //will execute some sql
								}
							}
						}
						catch (Exception e)
						{
							failures.Add(e);
						}
					})).ToArray();

			using (var spy = new TextLogSpy("NHibernate.SQL", "%message | SessionId: %property{sessionId}"))
			{
				Array.ForEach(array, thread => thread.Start());
				// Give some time to threads for reaching the wait, having all of them ready to do most of their job concurrently.
				Thread.Sleep(100);
				semaphore.Set();
				Array.ForEach(array, thread => thread.Join());

				Assert.That(failures, Is.Empty, $"{failures.Count} thread(s) failed.");

				var loggingEvent = spy.GetWholeLog();
				for (var i = 1; i < 11; i++)
				for (var j = 0; j < 10; j++)
				{
					var sessionId = sessionIds[i];
					Assert.That(loggingEvent, Does.Contain($"p0 = {i * 10 + j} [Type: Int32 (0:0:0)] | SessionId: {sessionId}"));
				}
			}
		}

		// IFixingRequired interface ensures the value is evaluated at log time rather than at log buffer flush time.
		public class SessionIdCapturer : IFixingRequired
		{
			public object GetFixedObject() => ToString();

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
			private readonly Level previousLevel;

			public TextLogSpy(string loggerName, string pattern)
			{
				stringBuilder = new StringBuilder();
				appender = new TextWriterAppender
				{
					Layout = new PatternLayout(pattern),
					Threshold = Level.All,
					Writer = new StringWriter(stringBuilder)
				};
				loggerImpl = (Logger)LogManager.GetLogger(typeof(LogsFixture).Assembly, loggerName).Logger;
				loggerImpl.AddAppender(appender);
				previousLevel = loggerImpl.Level;
				loggerImpl.Level = Level.All;
			}

			public string GetWholeLog()
			{
				return stringBuilder.ToString();
			}

			public void Dispose()
			{
				loggerImpl.RemoveAppender(appender);
				loggerImpl.Level = previousLevel;
			}
		}
	}


}
