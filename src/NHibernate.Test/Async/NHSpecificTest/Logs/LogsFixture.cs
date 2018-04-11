﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NUnit.Framework;
using NHibernate.Linq;

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
	public class LogsFixtureAsync : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "NHSpecificTest.Logs.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Cfg.Environment.UseSecondLevelCache, "false");
			configuration.SetProperty(Cfg.Environment.TrackSessionId, "true");
		}

		protected override void OnSetUp()
		{
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Person());
				s.Save(new Person());
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from Person").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public async Task WillGetSessionIdFromSessionLogsAsync()
		{
			GlobalContext.Properties["sessionId"] = new SessionIdCapturer();

			using (var spy = new TextLogSpy("NHibernate.SQL", "%message | SessionId: %property{sessionId}"))
			using (var s = Sfi.OpenSession())
			{
				var sessionId = ((SessionImpl) s).SessionId;

				await (s.GetAsync<Person>(1)); //will execute some sql

				var loggingEvent = spy.GetWholeLog();
				Assert.That(loggingEvent.Contains(sessionId.ToString()), Is.True);
			}
		}

		[Test]
		public async Task WillGetSessionIdFromConsecutiveSessionsLogsAsync()
		{
			GlobalContext.Properties["sessionId"] = new SessionIdCapturer();

			using (var spy = new TextLogSpy("NHibernate.SQL", "%message | SessionId: %property{sessionId}"))
			{
				var sessions = Enumerable.Range(1, 10).Select(i => Sfi.OpenSession()).ToArray();
				try
				{
					for (var i = 0; i < 10; i++)
					for (var j = 0; j < 10; j++)
					{
						var s = sessions[j];
						await (s.GetAsync<Person>(i * 10 + j)); //will execute some sql
					}
				}
				finally
				{
					foreach (var s in sessions)
					{
						s.Dispose();
					}
				}

				var loggingEvent = spy.GetWholeLog();
				for (var i = 0; i < 10; i++)
				for (var j = 0; j < 10; j++)
				{
					var sessionId = sessions[j].GetSessionImplementation().SessionId;
					Assert.That(loggingEvent, Does.Contain($"p0 = {i * 10 + j} [Type: Int32 (0:0:0)] | SessionId: {sessionId}"));
				}
			}
		}

		[Test]
		public async Task WillGetSessionIdFromInterlacedSessionsLogsAsync()
		{
			GlobalContext.Properties["sessionId"] = new SessionIdCapturer();
			var interceptor = new InterlacedSessionInterceptor(Sfi);
			using (var spy = new TextLogSpy("NHibernate.SQL", "%message | SessionId: %property{sessionId}"))
			using (var s = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			{
				// Trigger an operation which will fire many interceptor events, before and after s own logging.
				var persons = await (s.Query<Person>().ToListAsync());

				var loggingEvent = spy.GetWholeLog();
				for (var i = 0; i < interceptor.SessionIds.Count; i++)
				{
					var sessionId = interceptor.SessionIds[i];
					Assert.That(loggingEvent, Does.Contain($"p0 = {i + 1} [Type: Int32 (0:0:0)] | SessionId: {sessionId}"));
				}
				Assert.That(loggingEvent, Does.Contain($"Person person0_ | SessionId: {s.GetSessionImplementation().SessionId}"));
			}
		}

		[Test]
		public async Task WillGetSessionIdFromSessionLogsConcurrentAsync()
		{
			GlobalContext.Properties["sessionId"] = new SessionIdCapturer();

			// Do not use a ManualResetEventSlim, it does not support async and exhausts the task thread pool in the
			// async counterparts of this test. SemaphoreSlim has the async support and release the thread when waiting.
			var semaphore = new SemaphoreSlim(0);
			var failures = new ConcurrentBag<Exception>();
			var sessionIds = new ConcurrentDictionary<int, Guid>();
			using (var spy = new TextLogSpy("NHibernate.SQL", "%message | SessionId: %property{sessionId}"))
			{
				await (Task.WhenAll(
					Enumerable.Range(1, 12 - 1).Select(async i =>
					{
						if (i > 10)
						{
							// Give some time to threads for reaching the wait, having all of them ready to do most of their job concurrently.
							await (Task.Delay(100));
							semaphore.Release(10);
							return;
						}
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
								await (semaphore.WaitAsync());

								for (int j = 0; j < 10; j++)
								{
									await (s.GetAsync<Person>(i * 10 + j)); //will execute some sql
								}
							}
						}
						catch (Exception e)
						{
							failures.Add(e);
						}
					})));

				Assert.That(failures, Is.Empty, $"{failures.Count} task(s) failed.");

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
				loggerImpl = (Logger) LogManager.GetLogger(typeof(LogsFixtureAsync).Assembly, loggerName).Logger;
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

		public class InterlacedSessionInterceptor : EmptyInterceptor
		{
			private readonly ISessionFactory _sfi;

			public System.Collections.Generic.List<Guid> SessionIds { get; } = new System.Collections.Generic.List<Guid>();

			public InterlacedSessionInterceptor(ISessionFactory sfi)
			{
				_sfi = sfi;
			}

			public override SqlString OnPrepareStatement(SqlString sql)
			{
				using (var s = _sfi.OpenSession())
				{
					SessionIds.Add(s.GetSessionImplementation().SessionId);
					s.Get<Person>(SessionIds.Count); //will execute some sql
				}
				return base.OnPrepareStatement(sql);
			}

			public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
			{
				using (var s = _sfi.OpenSession())
				{
					SessionIds.Add(s.GetSessionImplementation().SessionId);
					s.Get<Person>(SessionIds.Count); //will execute some sql
				}
				return base.OnLoad(entity, id, state, propertyNames, types);
			}
		}
	}
}
