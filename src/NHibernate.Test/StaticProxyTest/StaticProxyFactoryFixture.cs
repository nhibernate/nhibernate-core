using System.Collections.Generic;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.StaticProxyTest
{
	public class StaticProxyFactoryFixture
	{
		internal interface ISomething
		{
			int Id { get; }
		}

		public class TestClass : ISomething
		{
			public virtual int Id { get; set; }
		}

		[Test]
		public void CanCreateProxyForClassWithInternalInterface()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(typeof(TestClass).FullName, typeof(TestClass), new HashSet<System.Type> {typeof(INHibernateProxy)}, null, null, null);
			var proxy = factory.GetProxy(1, null);
			Assert.That(proxy, Is.Not.Null);
		}
	}
}
