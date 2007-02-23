using System;
using System.Data;
using log4net;

namespace NHibernate.Connection
{
	/// <summary>
	/// A ConnectionProvider that uses an IDriver to create connections.
	/// </summary>
	public class DriverConnectionProvider : ConnectionProvider
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DriverConnectionProvider));

		/// <summary>
		/// Initializes a new instance of the <see cref="DriverConnectionProvider"/> class.
		/// </summary>
		public DriverConnectionProvider()
		{
		}

		/// <summary>
		/// Closes and Disposes of the <see cref="IDbConnection"/>.
		/// </summary>
		/// <param name="conn">The <see cref="IDbConnection"/> to clean up.</param>
		public override void CloseConnection(IDbConnection conn)
		{
			base.CloseConnection(conn);
			conn.Dispose();
		}

		/// <summary>
		/// Gets a new open <see cref="IDbConnection"/> through 
		/// the <see cref="NHibernate.Driver.IDriver"/>.
		/// </summary>
		/// <returns>
		/// An Open <see cref="IDbConnection"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// If there is any problem creating or opening the <see cref="IDbConnection"/>.
		/// </exception>
		public override IDbConnection GetConnection()
		{
			log.Debug("Obtaining IDbConnection from Driver");
			IDbConnection conn = Driver.CreateConnection();
			conn.ConnectionString = ConnectionString;
			conn.Open();
			return conn;
		}
	}
}