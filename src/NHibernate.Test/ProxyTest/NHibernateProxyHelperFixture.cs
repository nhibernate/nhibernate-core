using System;

using NHibernate.Proxy;

using NUnit.Framework;

namespace NHibernate.Test.ProxyTest
{
	/// <summary>
	/// Summary description for NHibernateProxyHelperFixture.
	/// </summary>
	[TestFixture]
	public class NHibernateProxyHelperFixture : TestCase
	{
		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			ExportSchema( new string[] { "ProxyTest.AProxy.hbm.xml"}, true, "NHibernate.Test" );
		}

		[SetUp]
		public void SetUp() 
		{
			// there are test in here where we don't need to resetup the 
			// tables - so only set the tables up once
		}

		[TearDown]
		public override void TearDown() 
		{
			// do nothing except not let the base TearDown get called
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown() 
		{
			base.TearDown();
		}

		#endregion

		[Test]
		public void GetClassOfProxy()
		{
			ISession s = null;
			AProxy a = new AProxy();
			try 
			{
				s = sessions.OpenSession();
				a.Name = "a proxy";
				s.Save( a );
				s.Flush();
			}
			finally
			{
				if( s!=null )
				{
					s.Close();
				}
			}

			try
			{
				s = sessions.OpenSession();
				System.Type type = NHibernateProxyHelper.GetClass( a );
				Assert.AreEqual( typeof(AProxy), type, "Should have returned 'A' for a non-proxy" );
				
				AProxy aProxied = (AProxy)s.Load( typeof(AProxy), a.Id );
				Assert.IsFalse( NHibernateUtil.IsInitialized( aProxied ), "should be a proxy" );

				type = NHibernateProxyHelper.GetClass( aProxied );
				Assert.AreEqual( typeof(AProxy), type, "even though aProxied was a Proxy it should have returned the correct type." );
				s.Delete( aProxied );
				s.Flush();
			}
			finally
			{
				if( s!=null )
				{
					s.Close();
				}
			}
		}
		
	}
}
