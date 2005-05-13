using System;
using System.Collections;
using System.Data;

using NHibernate.Connection;

using NUnit.Framework;
using Iesi.Collections;

namespace NHibernate.Test
{
	/// <summary>
	/// This connection provider keeps a list of all open connections,
	/// it is used when testing to check that tests clean up after themselves.
	/// </summary>
	public class DebugConnectionProvider : DriverConnectionProvider
	{
		ISet connections = new ListSet();

		public override IDbConnection GetConnection()
		{
			IDbConnection connection = base.GetConnection();
			connections.Add( connection );
			return connection;
		}

		public override void CloseConnection( IDbConnection conn )
		{
			base.CloseConnection( conn );
			connections.Remove( conn );
		}

		public bool HasOpenConnections
		{
			get
			{
				return !connections.IsEmpty;
			}
		}

		public void CloseAllConnections()
		{
			while( !connections.IsEmpty )
			{
				IEnumerator en = connections.GetEnumerator();
				en.MoveNext();
				CloseConnection( en.Current as IDbConnection );
			}
		}
	}
}
