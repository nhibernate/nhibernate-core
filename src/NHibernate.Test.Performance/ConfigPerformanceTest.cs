using System;
using System.Collections;
using System.Reflection;

using NUnit.Framework;

using NHibernate.Cfg;

namespace NHibernate.Test.Performance
{
	/// <summary>
	/// Summary description for ConfigPerformanceTest.
	/// </summary>
	[TestFixture]
	public class ConfigPerformanceTest
	{
		private Configuration cfg;
		private ISessionFactory sessions;
		private ISession session;

		private IList files = new string[] {   
												  "FooBar.hbm.xml",
												  "Baz.hbm.xml",
												  "Qux.hbm.xml",
												  "Glarch.hbm.xml",
												  "Fum.hbm.xml",
												  "Fumm.hbm.xml",
												  "Fo.hbm.xml",
												  "One.hbm.xml",
												  "Many.hbm.xml",
												  "Immutable.hbm.xml" ,
												  "Fee.hbm.xml",
												  "Vetoer.hbm.xml",
												  "Holder.hbm.xml",
												  "Location.hbm.xml",
												  "Stuff.hbm.xml",
												  "Container.hbm.xml",
												  "Simple.hbm.xml",
												  "XY.hbm.xml"
											  };
		private string assemblyName = "NHibernate.DomainModel";

		[TearDown]
		public void TearDown()
		{
			if( session != null )
			{
				session.Close();
				session = null;
			}

			if( sessions != null )
			{
				sessions.Close();
				sessions = null;
			}
		}

		private void CreateConfig()
		{
			cfg = new Configuration();

			for (int i=0; i<files.Count; i++) 
			{
				cfg.AddResource( assemblyName + "." + files[i].ToString(), Assembly.Load( assemblyName ) );
			}
			sessions = cfg.BuildSessionFactory( );
		}

		private void CreateSession()
		{
			session = sessions.OpenSession();
		}

		[Test]
		public void CreateConfig100()
		{
			for ( int i = 0; i < 100; i++ )
			{
				CreateConfig();
				CreateSession();
			}
		}

		[Test]
		public void CreateSession100()
		{
			CreateConfig();
			for ( int i = 0; i < 100; i++ )
			{
				CreateSession();
			}
		}
	}
}
