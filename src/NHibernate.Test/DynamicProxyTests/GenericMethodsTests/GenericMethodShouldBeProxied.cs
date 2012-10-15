using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.DynamicProxyTests.GenericMethodsTests
{
	[TestFixture]
	public class GenericMethodShouldBeProxied
	{
		[Test]
		public void CanProxyBasicGenericMethod()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			c.BasicGenericMethod<int>().Should().Be(5);
			c.BasicGenericMethod<string>().Should().Be("blha");
		}

		[Test]
		public void CanProxyMethodWithGenericBaseClassConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			c.MethodWithGenericBaseClassConstraint<MyGenericClass<int>, int>().Should().Be(typeof(MyGenericClass<int>));
		}

		[Test]
		public void CanProxySelfCastingMethod()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			c.As<MyClass>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxyMethodWithDefaultConstructorConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			c.MethodWithConstructorConstraint<MyClass>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxyGenericMethodWithInterfaceConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			c.MethodWithInterfaceConstraint<IMyInterface>(new MyDerivedClass()).Should().Be(typeof(IMyInterface));
		}

		[Test]
		public void CanProxyGenericMethodWithReferenceTypeAndInterfaceConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyDerivedClass()), null);
			c.MethodWithReferenceTypeAndInterfaceConstraint<MyDerivedClass>().Should().Not.Be.Null();
		}

		class MyDerivedClass : MyClass, IMyInterface
		{ }
	}
}