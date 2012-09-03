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
				if (typeof(T) == typeof(int))
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

			public virtual TRequestedType Create<TRequestedType>() where TRequestedType : new()
			{
				return new TRequestedType();
			}
		}

		public interface MyInterface
		{
		}

		public class MyInterfaceImplementation : MyInterface
		{
			public virtual bool IsInterface<TRequestedType>() where TRequestedType : MyInterface
			{
				return this is TRequestedType;
			}

			public virtual TRequestedType AsInterface<TRequestedType>() where TRequestedType : class, MyInterface
			{
				return this as TRequestedType;
			}
		}

		public interface MyGenericInterface<TId>
		{
		}

		public class MyGenericClass<TId> : MyGenericInterface<TId>
		{
			public virtual TRequestedType As<TRequestedType>() where TRequestedType : MyGenericClass<TId>
			{
				return this as TRequestedType;
			}

			public virtual TRequestedType AsInterface<TRequestedType>() where TRequestedType : class, MyGenericInterface<TId>
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

		[Test]
		public void ProxyOfAGenericMethodWithDefaultConstructorConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyClass)factory.CreateProxy(typeof(MyClass), new PassThroughInterceptor(new MyClass()), null);
			c.Create<MyClass>().Should().Not.Be.Null();
		}

		[Test]
		public void ProxyOfAGenericMethodWithInterfaceConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyInterfaceImplementation)factory.CreateProxy(typeof(MyInterfaceImplementation), new PassThroughInterceptor(new MyInterfaceImplementation()), null);
			c.IsInterface<MyInterface>().Should().Be.True();
		}

		[Test]
		public void ProxyOfAGenericMethodWithReferenceTypeAndInterfaceConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyInterfaceImplementation)factory.CreateProxy(typeof(MyInterfaceImplementation), new PassThroughInterceptor(new MyInterfaceImplementation()), null);
			c.AsInterface<MyInterface>().Should().Not.Be.Null();
		}

		[Test]
		public void ProxyOfSelfCastingMethodInGenericClass()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.As<MyGenericClass<int>>().Should().Not.Be.Null();
		}

		[Test]
		public void ProxyOfSelfCastingMethodInGenericClassWithGenericInterfaceConstraint()
		{
			var factory = new ProxyFactory();
			var c = (MyGenericClass<int>)factory.CreateProxy(typeof(MyGenericClass<int>), new PassThroughInterceptor(new MyGenericClass<int>()), null);
			c.AsInterface<MyGenericClass<int>>().Should().Not.Be.Null();
		}
	}
}