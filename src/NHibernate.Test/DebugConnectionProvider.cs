using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.Connection;

namespace NHibernate.Test
{
	/// <summary>
	/// This connection provider keeps a list of all open connections,
	/// it is used when testing to check that tests clean up after themselves.
	/// </summary>
	public partial class DebugConnectionProvider : DriverConnectionProvider
	{
		private ConcurrentDictionary<DbConnection, byte> connections = new ConcurrentDictionary<DbConnection, byte>();

		public override DbConnection GetConnection()
		{
			try
			{
				var connection = base.GetConnection();
				connections.TryAdd(connection, 0);
				return connection;
			}
			catch (Exception e)
			{
				throw new HibernateException("Could not open connection to: " + ConnectionString, e);
			}
		}

		public override void CloseConnection(DbConnection conn)
		{
			base.CloseConnection(conn);
			byte _;
			connections.TryRemove(conn, out _);
		}

		public bool HasOpenConnections
			=> connections.Keys.Any(IsNotClosed);

		private static bool IsNotClosed(DbConnection conn)
		{
			try
			{
				return conn.State != ConnectionState.Closed;
			}
			catch (ObjectDisposedException)
			{
				return false;
			}
		}

		public void CloseAllConnections()
		{
			while (connections.Count != 0)
			{
				foreach (var conn in connections.Keys.ToArray())
				{
					CloseConnection(conn);
				}
			}
		}
	}
}
