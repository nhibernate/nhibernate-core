using System.Collections.Generic;
using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

namespace NHibernate.Test.DynamicProxyTests
{
	public class InterfaceWithEqualsGethashcodeTests
	{
		public interface IMyBaseObject
		{
			bool Equals(object that);
			int GetHashCode();
		}
		public interface IHasSomething : IMyBaseObject
		{
			string Something { get; set; }
		}
		public class InterceptedMethodsExposer : Proxy.DynamicProxy.IInterceptor
		{
			private readonly List<string> interceptedMethods = new List<string>();
			public object Intercept(InvocationInfo info)
			{
				interceptedMethods.Add(info.TargetMethod.Name);
				return true;
			}

			public IEnumerable<string> InterceptedMethods
			{
				get { return interceptedMethods; }
			}
		}

		[Test]
		public void WhenProxyAnInterfaceShouldInterceptEquals()
		{
			var proxyFactory = new ProxyFactory();
			var interceptor = new InterceptedMethodsExposer();
			var proxy = proxyFactory.CreateProxy(typeof(IHasSomething), interceptor, null);
			proxy.Equals(null);
			Assert.That(interceptor.InterceptedMethods, Contains.Item("Equals"));
		}
	}
}