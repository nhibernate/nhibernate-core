using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using NHibernate.Connection;

namespace NHibernate.Test
{
	/// <summary>
	/// This connection provider keeps a list of all open connections,
	/// it is used when testing to check that tests clean up after themselves.
	/// </summary>
	public class DebugConnectionProvider : DriverConnectionProvider
	{
		private ConcurrentDictionary<IDbConnection, byte> connections = new ConcurrentDictionary<IDbConnection, byte>();

		public override IDbConnection GetConnection()
		{
			try
			{
				IDbConnection connection = base.GetConnection();
				connections.TryAdd(connection, 0);
				return connection;
			}
			catch (Exception e)
			{
				throw new HibernateException("Could not open connection to: " + ConnectionString, e);
			}
		}

		public override void CloseConnection(IDbConnection conn)
		{
			base.CloseConnection(conn);
			byte _;
			connections.TryRemove(conn, out _);
		}

		public bool HasOpenConnections
		{
			get
			{
				// Disposing of an ISession does not call CloseConnection (should it???)
				// so a Diposed of ISession will leave an IDbConnection in the list but
				// the IDbConnection will be closed (atleast with MsSql it works this way).
				return connections.Keys.Any(conn => conn.State != ConnectionState.Closed);
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
