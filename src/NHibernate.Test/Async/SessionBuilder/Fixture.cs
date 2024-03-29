﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.SessionBuilder
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "SessionBuilder.Mappings.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			// Do not use .Instance here, we want another instance.
			configuration.Interceptor = new EmptyInterceptor();
		}

		private void CanSetAutoClose<T>(T sb) where T : ISessionBuilder<T>
		{
			var options = DebugSessionFactory.GetCreationOptions(sb);
			CanSet(sb, sb.AutoClose, () => options.ShouldAutoClose,
				sb is ISharedSessionBuilder ssb ? ssb.AutoClose : default(Func<ISharedSessionBuilder>),
				// initial value
				false,
				// values
				true, false);
		}

		private void CanSetAutoJoinTransaction<T>(T sb) where T : ISessionBuilder<T>
		{
			var options = DebugSessionFactory.GetCreationOptions(sb);
			CanSet(sb, sb.AutoJoinTransaction, () => options.ShouldAutoJoinTransaction,
				sb is ISharedSessionBuilder ssb ? ssb.AutoJoinTransaction : default(Func<ISharedSessionBuilder>),
				// initial value
				true,
				// values
				false, true);
		}

		private void CanSetAutoJoinTransactionOnStateless<T>(T sb) where T : IStatelessSessionBuilder
		{
			var options = DebugSessionFactory.GetCreationOptions(sb);
			CanSetOnStateless(
				sb, sb.AutoJoinTransaction, () => options.ShouldAutoJoinTransaction,
				sb is ISharedStatelessSessionBuilder ssb ? ssb.AutoJoinTransaction : default(Func<ISharedStatelessSessionBuilder>),
				// initial value
				true,
				// values
				false, true);
		}

		[Test]
		public async Task CanSetConnectionAsync()
		{
			var sb = Sfi.WithOptions();
			await (CanSetConnectionAsync(sb));
			await (CanSetConnectionOnStatelessAsync(Sfi.WithStatelessOptions()));
			using (var s = sb.OpenSession())
			{
				await (CanSetConnectionAsync(s.SessionWithOptions()));
				await (CanSetConnectionOnStatelessAsync(s.StatelessSessionWithOptions()));
			}
		}

		private async Task CanSetConnectionAsync<T>(T sb, CancellationToken cancellationToken = default(CancellationToken)) where T : ISessionBuilder<T>
		{
			var sbType = sb.GetType().Name;
			var conn = await (Sfi.ConnectionProvider.GetConnectionAsync(cancellationToken));
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

		private async Task CanSetConnectionOnStatelessAsync<T>(T sb, CancellationToken cancellationToken = default(CancellationToken)) where T : IStatelessSessionBuilder
		{
			var sbType = sb.GetType().Name;
			var conn = await (Sfi.ConnectionProvider.GetConnectionAsync(cancellationToken));
			try
			{
				var options = DebugSessionFactory.GetCreationOptions(sb);
				Assert.IsNull(options.UserSuppliedConnection, $"{sbType}: Initial value");
				var fsb = sb.Connection(conn);
				Assert.AreEqual(conn, options.UserSuppliedConnection, $"{sbType}: After call with a connection");
				Assert.AreEqual(sb, fsb, $"{sbType}: Unexpected fluent return after call with a connection");

				if (sb is ISharedStatelessSessionBuilder ssb)
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

		private void CanSetConnectionReleaseMode<T>(T sb) where T : ISessionBuilder<T>
		{
			var options = DebugSessionFactory.GetCreationOptions(sb);
			CanSet(sb, sb.ConnectionReleaseMode, () => options.SessionConnectionReleaseMode,
				sb is ISharedSessionBuilder ssb ? ssb.ConnectionReleaseMode : default(Func<ISharedSessionBuilder>),
				// initial value
				Sfi.Settings.ConnectionReleaseMode,
				// values
				ConnectionReleaseMode.OnClose, ConnectionReleaseMode.AfterStatement, ConnectionReleaseMode.AfterTransaction);
		}

		private void CanSetFlushMode<T>(T sb) where T : ISessionBuilder<T>
		{
			var options = DebugSessionFactory.GetCreationOptions(sb);
			CanSet(sb, sb.FlushMode, () => options.InitialSessionFlushMode,
				sb is ISharedSessionBuilder ssb ? ssb.FlushMode : default(Func<ISharedSessionBuilder>),
				// initial value
				Sfi.Settings.DefaultFlushMode,
				// values
				FlushMode.Always, FlushMode.Auto, FlushMode.Commit, FlushMode.Manual);
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

		private void CanSetOnStateless<T, V>(
			T sb, Func<V, T> setter, Func<V> getter, Func<ISharedStatelessSessionBuilder> shared, V initialValue,
			params V[] values) where T : IStatelessSessionBuilder
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

		[Test]
		public void ThrowWhenUsingSessionFromDisposedFactoryAsync()
		{
			using (var session = Sfi.OpenSession())
			{
				try
				{
					Sfi.Dispose();
					Assert.That(() => session.GetAsync<Entity>(Guid.Empty), Throws.InstanceOf(typeof(ObjectDisposedException)));
				}
				finally
				{
					RebuildSessionFactory();
				}
			}
		}
	}
}
