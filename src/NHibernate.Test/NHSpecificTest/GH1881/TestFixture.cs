using System;
using System.Linq;
using System.Reflection;
using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1881
{
	[TestFixture]
	[Obsolete]
	public class TestFixture
	{
		[Test]
		public void InterfacesShouldBeImplementedExplicitlyOnProxies()
		{
			var proxy = new ProxyFactory().CreateProxy(typeof(PersistentGenericBag<object>), null, typeof(ILazyInitializedCollection));
			Assert.That(proxy, Is.Not.Null);

			foreach (var method in proxy.GetType().GetMethods().Where(m => m.DeclaringType == typeof(ILazyInitializedCollection)))
			{
				// These attributes are what .NET uses for explicitly implemented interface methods
				Assert.That(method.Attributes, Is.EqualTo(MethodAttributes.Private |
				                                          MethodAttributes.Final |
				                                          MethodAttributes.Virtual |
				                                          MethodAttributes.HideBySig |
				                                          MethodAttributes.NewSlot |
				                                          MethodAttributes.SpecialName));
			}
		}
	}
}
