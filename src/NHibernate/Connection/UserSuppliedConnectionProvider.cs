using System;
using System.Data;
using System.Collections;

using NHibernate.Driver;

namespace NHibernate.Connection 
{
	/// <summary>
	/// An implementation of the <c>IConnectionProvider</c> that simply throws an exception when
	/// a connection is requested.
	/// </summary>
	/// <remarks>
	/// This implementation indicates that the user is expected to supply an ADO.NET connection
	/// </remarks>
	public class UserSuppliedConnectionProvider : ConnectionProvider 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserSuppliedConnectionProvider));
		
		public override void CloseConnection(IDbConnection conn) 
		{
			throw new InvalidOperationException("The User is responsible for closing ADO.NET connection - not NHibernate.");
		}

		public override IDbConnection GetConnection() 
		{
			throw new InvalidOperationException("The user must provide an ADO.NET connection - NHibernate is not creating it.");
		}

		/// <summary>
		/// Configures the ConnectionProvider with only the Driver class.
		/// </summary>
		/// <param name="settings"></param>
		/// <remarks>
		/// All other settings of the Connection are the responsibility of the User since they configured
		/// NHibernate to use a Connection supplied by the User.
		/// </remarks>
		public override void Configure(IDictionary settings) 
		{
			log.Warn("No connection properties specified - the user must supply an ADO.NET connection");
			
			ConfigureDriver(settings);
		}

		public override bool IsStatementCache 
		{
			get { return false; }
		}

		public override void Close()
		{
			// do nothing - don't need to throw an error because this is something
			// that NHibernate will call.
		}


	}
}
