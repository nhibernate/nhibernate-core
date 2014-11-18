using System.Collections.Generic;
using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

namespace NHibernate.Test.DynamicProxyTests.ProxiedMembers
{
	public class MetodWithRefDictionaryTest
	{
		public class MyClass
		{
			public virtual void Method(ref Dictionary<string ,string> adictionary)
			{
				adictionary = new Dictionary<string, string>();
			}
		}

		[Test]
		public void Proxy()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			var dictionary = new Dictionary<string, string>();
			var myParam = dictionary;
			c.Method(ref myParam);
			Assert.That(myParam, Is.Not.SameAs(dictionary));
		}
	}
}