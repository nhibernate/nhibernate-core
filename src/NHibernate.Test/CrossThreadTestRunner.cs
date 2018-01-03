using System;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace NHibernate.Test
{
	/// <summary>
	/// Helper class to execute a method on another thread, catching all exceptions then rethrowing them
	/// on another thread that calls Join().
	/// </summary>
	/// TODO: Explore if this can be merged with MultiThreadRunner (probably should add exception handling to the latter).
	class CrossThreadTestRunner
	{
		private Exception _lastException;
		private readonly Thread _thread;
		private readonly ThreadStart _start;

		private const string remoteStackTraceFieldName = "_remoteStackTraceString";

		private static readonly FieldInfo RemoteStackTraceField = typeof(Exception).GetField(remoteStackTraceFieldName,
		                                                                                     BindingFlags.Instance |
		                                                                                     BindingFlags.NonPublic);

		public CrossThreadTestRunner(ThreadStart start)
		{
			_start = start;
			_thread = new Thread(Run);
			_thread.SetApartmentState(ApartmentState.STA);
		}

		public void Start()
		{
			_lastException = null;
			_thread.Start();
		}

		public void Join()
		{
			_thread.Join();

			if (_lastException != null)
			{
				ThrowExceptionPreservingStack(_lastException);
			}
		}

		private void Run()
		{
			try
			{
				_start.Invoke();
			}
			catch (Exception e)
			{
				_lastException = e;
			}
		}

#if !NETCOREAPP2_0
		[ReflectionPermission(SecurityAction.Demand)]
#endif
		private static void ThrowExceptionPreservingStack(Exception exception)
		{
			if (RemoteStackTraceField != null)
			{
				RemoteStackTraceField.SetValue(exception, exception.StackTrace + Environment.NewLine);
			}
			throw exception;
		}
	}
}
