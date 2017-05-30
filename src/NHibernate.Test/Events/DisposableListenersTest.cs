using System;
using NHibernate.Cfg;
using NHibernate.Event;
using NUnit.Framework;

namespace NHibernate.Test.Events
{
	public partial class MyDisposableListener : IPostUpdateEventListener, IDisposable
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

	public partial class DisposableListenersTest
	{
		[Test]
		public void WhenCloseSessionFactoryThenCallDisposeOfListener()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var myDisposableListener = new MyDisposableListener();
			cfg.AppendListeners(ListenerType.PostUpdate, new[]{myDisposableListener});
			var sf = cfg.BuildSessionFactory();
			sf.Close();
			Assert.That(myDisposableListener.DisposeCalled, Is.True);
		}
	}
}