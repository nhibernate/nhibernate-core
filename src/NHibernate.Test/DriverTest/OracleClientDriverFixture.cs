using System;
using System.Data;

using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	/// <summary>
	/// Summary description for OracleClientDriverFixture.
	/// </summary>
	[TestFixture]
	public class OracleClientDriverFixture
	{
		
		/// <summary>
		/// Verify that the correct Connection Class is being loaded.
		/// </summary>
		[Test]
		public void ConnectionClassName() 
		{
			Driver.IDriver driver = new Driver.OracleClientDriver();
			IDbConnection conn = driver.CreateConnection();

			Assert.AreEqual( "System.Data.OracleClient.OracleConnection", conn.GetType().FullName);
		}

		/// <summary>
		/// Verify that the correct Command class is being loaded.
		/// </summary>
		[Test]
		public void CommandClassName() 
		{
			OracleClientDriver driver = new OracleClientDriver();
			IDbCommand cmd = driver.CreateCommand();

			Assert.AreEqual( "System.Data.OracleClient.OracleCommand", cmd.GetType().FullName );

		}
	}
}
