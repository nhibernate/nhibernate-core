using System;
using NHibernate.Cfg;
using NHibernate.Event;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Events
{
	public class MyDisposableListener : IPostUpdateEventListener, IDisposable
	{
		public void OnPostUpdate(PostUpdateEvent @event)
		{
		}

		public bool DisposeCalled { get; private set; }
		public void Dispose()
		{
			DisposeCalled = true;
		}
	}

	public class DisposableListenersTest
	{
		[Test]
		public void WhenCloseSessionFactoryThenCallDisposeOfListener()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var myDisposableListener = new MyDisposableListener();
			cfg.AppendListeners(ListenerType.PostUpdate, new[]{myDisposableListener});
			var sf = cfg.BuildSessionFactory();
			sf.Close();
			myDisposableListener.DisposeCalled.Should().Be.True();
		}
	}
}