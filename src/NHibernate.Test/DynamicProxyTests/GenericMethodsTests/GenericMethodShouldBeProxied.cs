using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;

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
			Assert.That(c.BasicGenericMethod<int>(), Is.EqualTo(5));
			Assert.That(c.BasicGenericMethod<string>(), Is.EqualTo("blha"));
		}

		[Test]
		public void CanProxyMethodWithGenericBaseClassConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			Assert.That(c.MethodWithGenericBaseClassConstraint<MyGenericClass<int>, int>(), Is.EqualTo(typeof(MyGenericClass<int>)));
		}

		[Test]
		public void CanProxySelfCastingMethod()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			Assert.That(c.As<MyClass>(), Is.Not.Null);
		}

		[Test]
		public void CanProxyMethodWithDefaultConstructorConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			Assert.That(c.MethodWithConstructorConstraint<MyClass>(), Is.Not.Null);
		}

		[Test]
		public void CanProxyGenericMethodWithInterfaceConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			Assert.That(c.MethodWithInterfaceConstraint<IMyInterface>(new MyDerivedClass()), Is.EqualTo(typeof(IMyInterface)));
		}
		
		[Test]
		public void CanProxyGenericMethodWithReferenceTypeAndInterfaceConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyDerivedClass()), null);
			Assert.That(c.MethodWithReferenceTypeAndInterfaceConstraint<MyDerivedClass>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.As<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsBase<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterface<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterfaceBase<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint2()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.As2<MyGenericClass<object>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase2()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsBase2<MyGenericClass<object>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint2()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterface2<MyGenericClass<object>>(), Is.Not.Null);
		}
		
		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint2()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterfaceBase2<MyGenericClass<object>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint3()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.As3<MyGenericClass<object>, object>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase3()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsBase3<MyGenericClassBase<object, object>, object>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint3()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterface3<MyGenericClass<object>, object>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint3()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>) factory.CreateProxy(typeof (MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterfaceBase3<MyGenericClass<object>, object>(), Is.Not.Null);
		}
		
		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint4()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.As4<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase4()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsBase4<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint4()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterface4<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint4()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterfaceBase4<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaint5()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.As5<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericConstaintBase5()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsBase5<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceConstaint5()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterface5<MyGenericClass<int>>(), Is.Not.Null);
		}

		[Test]
		public void CanProxySelfCastingMethodWithGenericInterfaceBaseConstaint5()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			Assert.That(c.AsInterfaceBase5<MyGenericClass<int>>(), Is.Not.Null);
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