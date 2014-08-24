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

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.As<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsBase<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterface<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterfaceBase<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint2()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.As2<MyGenericClass<object>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase2()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsBase2<MyGenericClass<object>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint2()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterface2<MyGenericClass<object>>().Should().Not.Be.Null();
		}
		
		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint2()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterfaceBase2<MyGenericClass<object>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint3()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.As3<MyGenericClass<object>, object>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase3()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsBase3<MyGenericClassBase<object, object>, object>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint3()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterface3<MyGenericClass<object>, object>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint3()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterfaceBase3<MyGenericClass<object>, object>().Should().Not.Be.Null();
		}
		
		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint4()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.As4<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase4()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsBase4<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint4()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterface4<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint4()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterfaceBase4<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint5()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.As5<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase5()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsBase5<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint5()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterface5<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint5()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterfaceBase5<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void GenericTypeConstraint()
		{
			var type = typeof(MyGenericClass<int>);
			var method = type.GetMethod("As");

			var genericArgument = method.GetGenericArguments()[0]; // TRequestedType : class, IMyGenericInterface<TId>
			var typeConstraint = genericArgument.GetGenericParameterConstraints()[0]; // MyGenericClass<TId>

			Assert.That(typeConstraint, Is.EqualTo(typeof(MyGenericClass<>)));
			Assert.That(typeConstraint.GetGenericTypeDefinition(), Is.EqualTo(typeof(MyGenericClass<>)));
		}

		[Test]
		public void GenericInterfaceConstraint()
		{
			var type = typeof(MyGenericClass<int>);
			var method = type.GetMethod("AsInterface");

			var genericArgument = method.GetGenericArguments()[0]; // TRequestedType : class, IMyGenericInterface<TId>
			var typeConstraint = genericArgument.GetGenericParameterConstraints()[0]; // IMyGenericInterface<TId>
			var typeConstraintGenericArgument = typeConstraint.GetGenericArguments()[0]; // TId

			Assert.That(typeConstraint.GetGenericTypeDefinition(), Is.EqualTo(typeof(IMyGenericInterface<>)));
		}

		class MyDerivedClass : MyClass, IMyInterface
		{ }
	}
}