﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Cfg;
using NHibernate.Event;
using NUnit.Framework;

namespace NHibernate.Test.Events
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class MyDisposableListener : IPostUpdateEventListener, IDisposable
	{
		public Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}

	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class DisposableListenersTest
	{
		[Test]
		public async Task WhenCloseSessionFactoryThenCallDisposeOfListenerAsync()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var myDisposableListener = new MyDisposableListener();
			cfg.AppendListeners(ListenerType.PostUpdate, new[]{myDisposableListener});
			var sf = cfg.BuildSessionFactory();
			await (sf.CloseAsync());
			Assert.That(myDisposableListener.DisposeCalled, Is.True);
		}
	}
}