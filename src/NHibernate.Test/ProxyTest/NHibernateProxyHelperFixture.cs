using System;
using System.Collections;
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
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"ProxyTest.AProxy.hbm.xml"}; }
		}

		[Test]
		public void GetClassOfProxy()
		{
			ISession s = null;
			AProxy a = new AProxy();
			try
			{
				s = OpenSession();
				a.Name = "a proxy";
				s.Save(a);
				s.Flush();
			}
			finally
			{
				if (s != null)
				{
					s.Close();
				}
			}

			try
			{
				s = OpenSession();
				System.Type type = NHibernateProxyHelper.GetClass(a);
				Assert.AreEqual(typeof(AProxy), type, "Should have returned 'A' for a non-proxy");

				AProxy aProxied = (AProxy) s.Load(typeof(AProxy), a.Id);
				Assert.IsFalse(NHibernateUtil.IsInitialized(aProxied), "should be a proxy");

				type = NHibernateProxyHelper.GetClass(aProxied);
				Assert.AreEqual(typeof(AProxy), type, "even though aProxied was a Proxy it should have returned the correct type.");
				s.Delete(aProxied);
				s.Flush();
			}
			finally
			{
				if (s != null)
				{
					s.Close();
				}
			}
		}
	}
}