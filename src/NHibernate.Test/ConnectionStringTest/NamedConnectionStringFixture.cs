using System;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Connection;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.ConnectionStringTest
{
	[TestFixture]
	public class NamedConnectionStringFixture
	{
		[Test]
		public void InvalidNamedConnectedStringThrows()
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings.Add(Environment.ConnectionStringName, "MyConStr");
			ConnectionProvider cp = new MockConnectionProvider();
			Assert.Throws<HibernateException>(()=>cp.Configure(settings), "Could not find named connection string MyConStr");
		}

		[Test]
		public void ConnectionStringInSettingsOverrideNamedConnectionSTring()
		{
			Dictionary<string, string> settings = new Dictionary<string, string>();
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
			Dictionary<string, string> settings = new Dictionary<string, string>();
			settings.Add(Environment.ConnectionStringName, "DummyConnectionString");
			MockConnectionProvider cp = new MockConnectionProvider();
			cp.Configure(settings);

			Assert.AreEqual("TestConnectionString-TestConnectionString", cp.PublicConnectionString);
		}
	}
	
	public partial class MockConnectionProvider : ConnectionProvider
	{
		
		public string PublicConnectionString
		{
			get
			{
				return base.ConnectionString;
			}
		}
		
		/// <summary>
		/// Get an open <see cref="DbConnection"/>.
		/// </summary>
		/// <returns>An open <see cref="DbConnection"/>.</returns>
		public override DbConnection GetConnection()
		{
			throw new NotImplementedException();
		}

		protected override void ConfigureDriver(IDictionary<string, string> settings)
		{
			return;
		}
	}
	
}
