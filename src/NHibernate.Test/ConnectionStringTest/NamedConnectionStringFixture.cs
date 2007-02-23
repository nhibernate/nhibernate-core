#if NET_2_0
using System;
using System.Collections;
using System.Data;
using NHibernate.Connection;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.ConnectionStringTest
{
	[TestFixture]
	public class NamedConnectionStringFixture
	{
		[Test]
		[ExpectedException(typeof(HibernateException), "Could not find named connection string MyConStr")]
		public void InvalidNamedConnectedStringThrows()
		{
			Hashtable settings = new Hashtable();
			settings.Add(Environment.ConnectionStringName, "MyConStr");
			ConnectionProvider cp = new MockConnectionProvider();
			cp.Configure(settings);
		}

		[Test]
		public void ConnectionStringInSettingsOverrideNamedConnectionSTring()
		{
			Hashtable settings = new Hashtable();
			string conStr = "Test Connection String";
			settings.Add(Environment.ConnectionString, conStr);
			settings.Add(Environment.ConnectionStringName, "MyConStr");
			MockConnectionProvider cp = new MockConnectionProvider();
			cp.Configure(settings);

			Assert.AreEqual(conStr, cp.PublicConnectionString);
		}

		[Test]
		public void CanGetNamedConnectionStringFromConfiguration()
		{
			Hashtable settings = new Hashtable();
			settings.Add(Environment.ConnectionStringName, "TestConnectionString");
			MockConnectionProvider cp = new MockConnectionProvider();
			cp.Configure(settings);

			Assert.AreEqual("TestConnectionString-TestConnectionString", cp.PublicConnectionString);
		}
	}
	
	public class MockConnectionProvider : ConnectionProvider
	{
		
		public string PublicConnectionString
		{
			get
			{
				return base.ConnectionString;
			}
		}
		
		/// <summary>
		/// Get an open <see cref="IDbConnection"/>.
		/// </summary>
		/// <returns>An open <see cref="IDbConnection"/>.</returns>
		public override IDbConnection GetConnection()
		{
			throw new NotImplementedException();
		}

		protected override void ConfigureDriver(IDictionary settings)
		{
			return;
		}
	}
	
}
#endif