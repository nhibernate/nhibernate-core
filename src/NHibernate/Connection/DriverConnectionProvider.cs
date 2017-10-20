using System;
using System.Data.Common;

namespace NHibernate.Connection
{
	/// <summary>
	/// A ConnectionProvider that uses an IDriver to create connections.
	/// </summary>
	public partial class DriverConnectionProvider : ConnectionProvider
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DriverConnectionProvider));

		/// <summary>
		/// Closes and Disposes of the <see cref="DbConnection"/>.
		/// </summary>
		/// <param name="conn">The <see cref="DbConnection"/> to clean up.</param>
		public override void CloseConnection(DbConnection conn)
		{
			base.CloseConnection(conn);
			conn.Dispose();
		}

		/// <summary>
		/// Gets a new open <see cref="DbConnection"/> through 
		/// the <see cref="NHibernate.Driver.IDriver"/>.
		/// </summary>
		/// <returns>
		/// An Open <see cref="DbConnection"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// If there is any problem creating or opening the <see cref="DbConnection"/>.
		/// </exception>
		public override DbConnection GetConnection()
		{
			log.Debug("Obtaining DbConnection from Driver");
			var conn = Driver.CreateConnection();
			try
			{
				conn.ConnectionString = ConnectionString;
				conn.Open();
			}
			catch (Exception)
			{
				conn.Dispose();
				throw;
			}
			
			return conn;
		}
	}
}