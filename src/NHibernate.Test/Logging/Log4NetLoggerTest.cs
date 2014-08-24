using System;
using log4net;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Logging
{
	public class Log4NetLoggerTest
	{
		private class LogMock: ILog
		{
			public int debug;
			public int debugException;
			public int debugFormat;
			public int info;
			public int infoException;
			public int infoFormat;
			public int warn;
			public int warnException;
			public int warnFormat;
			public int error;
			public int errorException;
			public int errorFormat;
			public int fatal;
			public int fatalException;
			public int isDebugEnabled;
			public int isInfoEnabled;
			public int isWarnEnabled;
			public int isErrorEnabled;
			public int isFatalEnabled;

			public log4net.Core.ILogger Logger
			{
				get { throw new NotImplementedException(); }
			}

			public void Debug(object message)
			{
				debug++;
			}

			public void Debug(object message, Exception exception)
			{
				debugException++;
			}

			public void DebugFormat(string format, params object[] args)
			{
				debugFormat++;
			}

			public void DebugFormat(string format, object arg0)
			{
				throw new NotImplementedException();
			}

			public void DebugFormat(string format, object arg0, object arg1)
			{
				throw new NotImplementedException();
			}

			public void DebugFormat(string format, object arg0, object arg1, object arg2)
			{
				throw new NotImplementedException();
			}

			public void DebugFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void Info(object message)
			{
				info++;
			}

			public void Info(object message, Exception exception)
			{
				infoException++;
			}

			public void InfoFormat(string format, params object[] args)
			{
				infoFormat++;
			}

			public void InfoFormat(string format, object arg0)
			{
				throw new NotImplementedException();
			}

			public void InfoFormat(string format, object arg0, object arg1)
			{
				throw new NotImplementedException();
			}

			public void InfoFormat(string format, object arg0, object arg1, object arg2)
			{
				throw new NotImplementedException();
			}

			public void InfoFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void Warn(object message)
			{
				warn++;
			}

			public void Warn(object message, Exception exception)
			{
				warnException++;
			}

			public void WarnFormat(string format, params object[] args)
			{
				warnFormat++;
			}

			public void WarnFormat(string format, object arg0)
			{
				throw new NotImplementedException();
			}

			public void WarnFormat(string format, object arg0, object arg1)
			{
				throw new NotImplementedException();
			}

			public void WarnFormat(string format, object arg0, object arg1, object arg2)
			{
				throw new NotImplementedException();
			}

			public void WarnFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void Error(object message)
			{
				error++;
			}

			public void Error(object message, Exception exception)
			{
				errorException++;
			}

			public void ErrorFormat(string format, params object[] args)
			{
				errorFormat++;
			}

			public void ErrorFormat(string format, object arg0)
			{
				throw new NotImplementedException();
			}

			public void ErrorFormat(string format, object arg0, object arg1)
			{
				throw new NotImplementedException();
			}

			public void ErrorFormat(string format, object arg0, object arg1, object arg2)
			{
				throw new NotImplementedException();
			}

			public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void Fatal(object message)
			{
				fatal++;
			}

			public void Fatal(object message, Exception exception)
			{
				fatalException++;
			}

			public void FatalFormat(string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public void FatalFormat(string format, object arg0)
			{
				throw new NotImplementedException();
			}

			public void FatalFormat(string format, object arg0, object arg1)
			{
				throw new NotImplementedException();
			}

			public void FatalFormat(string format, object arg0, object arg1, object arg2)
			{
				throw new NotImplementedException();
			}

			public void FatalFormat(IFormatProvider provider, string format, params object[] args)
			{
				throw new NotImplementedException();
			}

			public bool IsDebugEnabled
			{
				get
				{
					isDebugEnabled++;
					return true;
				}
			}

			public bool IsInfoEnabled
			{
				get
				{
					isInfoEnabled++;
					return true;
				}
			}

			public bool IsWarnEnabled
			{
				get
				{
					isWarnEnabled++;
					return true;
				}
			}

			public bool IsErrorEnabled
			{
				get
				{
					isErrorEnabled++;
					return true;
				}
			}

			public bool IsFatalEnabled
			{
				get
				{
					isFatalEnabled++;
					return true;
				}
			}
		} 
	
		[Test]
		public void CallingMethods()
		{
			var logMock = new LogMock();
			var logger = new Log4NetLogger(logMock);
			var b = logger.IsDebugEnabled;
			b = logger.IsErrorEnabled;
			b = logger.IsFatalEnabled;
			b = logger.IsInfoEnabled;
			b = logger.IsWarnEnabled;
			
			logger.Debug(null);
			logger.Debug(null, null);
			logger.DebugFormat(null, null);
			
			logger.Error(null);
			logger.Error(null, null);
			logger.ErrorFormat(null, null);
			
			logger.Warn(null);
			logger.Warn(null, null);
			logger.WarnFormat(null, null);
		
			logger.Info(null);
			logger.Info(null, null);
			logger.InfoFormat(null, null);

			logger.Fatal(null);
			logger.Fatal(null, null);

			logMock.debug.Should().Be(1);
			logMock.debugException.Should().Be(1);
			logMock.debugFormat.Should().Be(1);
			logMock.info.Should().Be(1);
			logMock.infoException.Should().Be(1);
			logMock.infoFormat.Should().Be(1);
			logMock.warn.Should().Be(1);
			logMock.warnException.Should().Be(1);
			logMock.warnFormat.Should().Be(1);
			logMock.error.Should().Be(1);
			logMock.errorException.Should().Be(1);
			logMock.errorFormat.Should().Be(1);
			logMock.fatal.Should().Be(1);
			logMock.fatalException.Should().Be(1);
			logMock.isDebugEnabled.Should().Be.GreaterThan(1);
			logMock.isInfoEnabled.Should().Be.GreaterThan(1);
			logMock.isWarnEnabled.Should().Be.GreaterThan(1);
			logMock.isErrorEnabled.Should().Be.GreaterThan(1);
			logMock.isFatalEnabled.Should().Be.GreaterThan(1);
		}
	}
}