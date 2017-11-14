using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Logging
{
	[TestFixture]
	public class LoggerProviderTest
	{
		class DummyInternalLogger : IInternalLogger
		{

			public bool IsErrorEnabled
			{
				get { throw new System.NotImplementedException(); }
			}

			public bool IsFatalEnabled
			{
				get { throw new System.NotImplementedException(); }
			}

			public bool IsDebugEnabled
			{
				get { throw new System.NotImplementedException(); }
			}

			public bool IsInfoEnabled
			{
				get { throw new System.NotImplementedException(); }
			}

			public bool IsWarnEnabled
			{
				get { throw new System.NotImplementedException(); }
			}

			public void Error(object message)
			{
				throw new System.NotImplementedException();
			}

			public void Error(object message, System.Exception exception)
			{
				throw new System.NotImplementedException();
			}

			public void ErrorFormat(string format, params object[] args)
			{
				throw new System.NotImplementedException();
			}

			public void Fatal(object message)
			{
				throw new System.NotImplementedException();
			}

			public void Fatal(object message, System.Exception exception)
			{
				throw new System.NotImplementedException();
			}

			public void Debug(object message)
			{
				throw new System.NotImplementedException();
			}

			public void Debug(object message, System.Exception exception)
			{
				throw new System.NotImplementedException();
			}

			public void DebugFormat(string format, params object[] args)
			{
				throw new System.NotImplementedException();
			}

			public void Info(object message)
			{
				throw new System.NotImplementedException();
			}

			public void Info(object message, System.Exception exception)
			{
				throw new System.NotImplementedException();
			}

			public void InfoFormat(string format, params object[] args)
			{
				throw new System.NotImplementedException();
			}

			public void Warn(object message)
			{
				throw new System.NotImplementedException();
			}

			public void Warn(object message, System.Exception exception)
			{
				throw new System.NotImplementedException();
			}

			public void WarnFormat(string format, params object[] args)
			{
				throw new System.NotImplementedException();
			}
		}

		class DummyLoggerFactory : ILoggerFactory
		{
			public IInternalLogger LoggerFor(string keyName)
			{
				return new DummyInternalLogger();
			}

			public IInternalLogger LoggerFor(System.Type type)
			{
				return this.LoggerFor(type.FullName);
			}
		}

		[Test]
		public void TestFluentConfiguration()
		{
			var cfg = new Configuration().LoggerFactory<DummyLoggerFactory>();

			Assert.That(LoggerProvider.LoggerFor("dummy"), Is.InstanceOf<DummyInternalLogger>());
		}

		[Test]
		public void LoggerProviderCanCreateLoggers()
		{
			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.Not.Null);
			Assert.That(LoggerProvider.LoggerFor(typeof (LoggerProviderTest)), Is.Not.Null);
		}

		[Test]
		public void WhenNotConfiguredAndLog4NetExistsThenUseLog4NetFactory()
		{
			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.InstanceOf<Log4NetLogger>());
		}
	}
}
