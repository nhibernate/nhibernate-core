using System.Collections;
using System.Data;
using Iesi.Collections;
using NHibernate.Connection;

namespace NHibernate.ByteCode.Spring.Tests
{
	public class DebugConnectionProvider : DriverConnectionProvider
	{
		private readonly ISet connections = new ListSet();

		public override IDbConnection GetConnection()
		{
			IDbConnection connection = base.GetConnection();
			connections.Add(connection);
			return connection;
		}

		public override void CloseConnection(IDbConnection conn)
		{
			base.CloseConnection(conn);
			connections.Remove(conn);
		}

		public bool HasOpenConnections
		{
			get
			{
				// check to see if all connections that were at one point opened
				// have been closed through the CloseConnection
				// method
				if (connections.IsEmpty)
				{
					// there are no connections, either none were opened or
					// all of the closings went through CloseConnection.
					return false;
				}
				else
				{
					// Disposing of an ISession does not call CloseConnection (should it???)
					// so a Diposed of ISession will leave an IDbConnection in the list but
					// the IDbConnection will be closed (atleast with MsSql it works this way).
					foreach (IDbConnection conn in connections)
					{
						if (conn.State != ConnectionState.Closed)
						{
							return true;
						}
					}

					// all of the connections have been Disposed and were closed that way
					// or they were Closed through the CloseConnection method.
					return false;
				}
			}
		}

		public void CloseAllConnections()
		{
			while (!connections.IsEmpty)
			{
				IEnumerator en = connections.GetEnumerator();
				en.MoveNext();
				CloseConnection(en.Current as IDbConnection);
			}
		}
	}
}