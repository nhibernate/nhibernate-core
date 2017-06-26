using System;
using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.SessionBuilder
{
	public class Fixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new [] { "SessionBuilder.Mappings.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			// Do not use .Instance here, we want another instance.
			configuration.Interceptor = new EmptyInterceptor();
		}

		[Test]
		public void CanSetAutoClose()
		{
			var sb = Sfi.WithOptions();
			CanSetAutoClose(sb);
			using (var s = sb.OpenSession())
			{
				CanSetAutoClose(s.SessionWithOptions());
			}
		}

		private void CanSetAutoClose<T>(T sb) where T : ISessionBuilder<T>
		{
			var options = DebugSessionFactory.GetCreationOptions(sb);
			CanSet(sb, sb.AutoClose, () => options.ShouldAutoClose,
				sb is ISharedSessionBuilder ssb ? ssb.AutoClose : default(Func<ISharedSessionBuilder>),
				// initial values
				false,
				// values
				true, false);
		}

		[Test]
		public void CanSetConnection()
		{
			var sb = Sfi.WithOptions();
			CanSetConnection(sb);
			using (var s = sb.OpenSession())
			{
				CanSetConnection(s.SessionWithOptions());
			}
		}

		private void CanSetConnection<T>(T sb) where T : ISessionBuilder<T>
		{
			var sbType = sb.GetType().Name;
			var conn = Sfi.ConnectionProvider.GetConnection();
			try
			{
				var options = DebugSessionFactory.GetCreationOptions(sb);
				Assert.IsNull(options.UserSuppliedConnection, $"{sbType}: Initial value");
				var fsb = sb.Connection(conn);
				Assert.AreEqual(conn, options.UserSuppliedConnection, $"{sbType}: After call with a connection");
				Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after call with a connection");

				if (sb is ISharedSessionBuilder ssb)
				{
					var sharedOptions = (ISharedSessionCreationOptions)options;
					Assert.IsFalse(sharedOptions.IsTransactionCoordinatorShared, $"{sbType}: Transaction coordinator shared before sharing");
					Assert.IsNull(sharedOptions.ConnectionManager, $"{sbType}: Connection manager shared before sharing");

					var fssb = ssb.Connection();
					// Sharing connection shares the connection manager, not the connection.
					Assert.IsNull(options.UserSuppliedConnection, $"{sbType}: After call with previous session connection");
					Assert.IsTrue(sharedOptions.IsTransactionCoordinatorShared, $"{sbType}: Transaction coordinator not shared after sharing");
					Assert.IsNotNull(sharedOptions.ConnectionManager, $"{sbType}: Connection manager not shared after sharing");
					Assert.AreEqual(sb, fssb, $"{sbType}: Unexpected fluent return on shared");

					fsb = sb.Connection(null);
					Assert.IsNull(options.UserSuppliedConnection, $"{sbType}: After call with null");
					Assert.IsFalse(sharedOptions.IsTransactionCoordinatorShared, $"{sbType}: Transaction coordinator shared after un-sharing");
					Assert.IsNull(sharedOptions.ConnectionManager, $"{sbType}: Connection manager shared after un-sharing");
					Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after un-sharing");
				}
				else
				{
					fsb = sb.Connection(null);
					Assert.IsNull(options.UserSuppliedConnection, $"{sbType}: After call with null");
					Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after call with null");
				}
			}
			finally
			{
				Sfi.ConnectionProvider.CloseConnection(conn);
			}
		}

		[Test]
		public void CanSetConnectionOnStateless()
		{
			var sb = Sfi.WithStatelessOptions();
			var sbType = sb.GetType().Name;
			var conn = Sfi.ConnectionProvider.GetConnection();
			try
			{
				var options = DebugSessionFactory.GetCreationOptions(sb);
				Assert.IsNull(options.UserSuppliedConnection, $"{sbType}: Initial value");
				var fsb = sb.Connection(conn);
				Assert.AreEqual(conn, options.UserSuppliedConnection, $"{sbType}: After call with a connection");
				Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after call with a connection");

				fsb = sb.Connection(null);
				Assert.IsNull(options.UserSuppliedConnection, $"{sbType}: After call with null");
				Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after call with null");
			}
			finally
			{
				Sfi.ConnectionProvider.CloseConnection(conn);
			}
		}

		[Test]
		public void CanSetConnectionReleaseMode()
		{
			var sb = Sfi.WithOptions();
			CanSetConnectionReleaseMode(sb);
			using (var s = sb.OpenSession())
			{
				CanSetConnectionReleaseMode(s.SessionWithOptions());
			}
		}

		private void CanSetConnectionReleaseMode<T>(T sb) where T : ISessionBuilder<T>
		{
			var options = DebugSessionFactory.GetCreationOptions(sb);
			CanSet(sb, sb.ConnectionReleaseMode, () => options.SessionConnectionReleaseMode,
				sb is ISharedSessionBuilder ssb ? ssb.ConnectionReleaseMode : default(Func<ISharedSessionBuilder>),
				// initial values
				Sfi.Settings.ConnectionReleaseMode,
				// values
				ConnectionReleaseMode.OnClose, ConnectionReleaseMode.AfterStatement, ConnectionReleaseMode.AfterTransaction);
		}

		[Test]
		public void CanSetFlushMode()
		{
			var sb = Sfi.WithOptions();
			CanSetFlushMode(sb);
			using (var s = sb.OpenSession())
			{
				CanSetFlushMode(s.SessionWithOptions());
			}
		}

		private void CanSetFlushMode<T>(T sb) where T : ISessionBuilder<T>
		{
			var options = DebugSessionFactory.GetCreationOptions(sb);
			CanSet(sb, sb.FlushMode, () => options.InitialSessionFlushMode,
				sb is ISharedSessionBuilder ssb ? ssb.FlushMode : default(Func<ISharedSessionBuilder>),
				// initial values
				Sfi.Settings.DefaultFlushMode,
				// values
				FlushMode.Always, FlushMode.Auto, FlushMode.Commit, FlushMode.Manual);
		}

		[Test]
		public void CanSetInterceptor()
		{
			var sb = Sfi.WithOptions();
			CanSetInterceptor(sb);
			using (var s = sb.OpenSession())
			{
				CanSetInterceptor(s.SessionWithOptions());
			}
		}

		private void CanSetInterceptor<T>(T sb) where T : ISessionBuilder<T>
		{
			var sbType = sb.GetType().Name;
			// Do not use .Instance here, we want another instance.
			var interceptor = new EmptyInterceptor();
			var options = DebugSessionFactory.GetCreationOptions(sb);

			Assert.AreEqual(Sfi.Interceptor, options.SessionInterceptor, $"{sbType}: Initial value");
			var fsb = sb.Interceptor(interceptor);
			Assert.AreEqual(interceptor, options.SessionInterceptor, $"{sbType}: After call with an interceptor");
			Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after call with an interceptor");

			if (sb is ISharedSessionBuilder ssb)
			{
				var fssb = ssb.Interceptor();
				Assert.AreEqual(EmptyInterceptor.Instance, options.SessionInterceptor, $"{sbType}: After call with shared interceptor");
				Assert.AreEqual(sb, fssb, $"{sbType}: Unexpected fluent return on shared");
			}

			Assert.Throws<ArgumentNullException>(() => sb.Interceptor(null), $"{sbType}: After call with null");

			fsb = sb.NoInterceptor();
			Assert.AreEqual(EmptyInterceptor.Instance, options.SessionInterceptor, $"{sbType}: After no call");
			Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after no call");
		}

		private void CanSet<T, V>(T sb, Func<V, T> setter, Func<V> getter, Func<ISharedSessionBuilder> shared, V initialValue,
			params V[] values) where T : ISessionBuilder<T>
		{
			var sbType = sb.GetType().Name;
			Assert.AreEqual(initialValue, getter(), $"{sbType}: Initial value");
			if (shared != null)
			{
				var fssb = shared();
				Assert.AreEqual(values.Last(), getter(), $"{sbType}: After call with shared setting");
				Assert.AreEqual(sb, fssb, $"{sbType}: Unexpected fluent return on shared");
			}
			foreach (var value in values)
			{
				var fsb = setter(value);
				Assert.AreEqual(value, getter(), $"{sbType}: After call with {value}");
				Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after call with {value}");
			}
		}
	}
}
