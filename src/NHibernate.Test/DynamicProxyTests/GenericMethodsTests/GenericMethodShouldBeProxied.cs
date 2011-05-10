using System.Collections.Generic;
using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.DynamicProxyTests.GenericMethodsTests
{
	public class GenericMethodShouldBeProxied
	{
		public class MyClass
		{
			public virtual object Method<T>()
			{
				if(typeof(T) == typeof(int))
				{
					return 5;
				}
				if (typeof(T) == typeof(string))
				{
					return "blha";
				}
				return default(T);
			}

			public virtual TRequestedType As<TRequestedType>() where TRequestedType : MyClass
			{
				return this as TRequestedType;
			}
		}

		[Test]
		public void ProxyOfAGenericMethod()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			c.Method<int>().Should().Be(5);
			c.Method<string>().Should().Be("blha");
		}

		[Test]
		public void ProxyOfSelfCastingMethod()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			c.As<MyClass>().Should().Not.Be.Null();
		}
	}
}