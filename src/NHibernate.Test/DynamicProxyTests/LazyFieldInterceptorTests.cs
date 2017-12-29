using System;
using System.Collections.Generic;
using NHibernate.Proxy;
using NUnit.Framework;
using NHibernate.Intercept;

namespace NHibernate.Test.DynamicProxyTests
{
	public class LazyFieldInterceptorTests
	{
		[Serializable]
		public class MyClass
		{
			public virtual int Id { get; set; }

			public virtual void ThrowError()
			{
				// Use some specific exception type to avoid using the base class.
				throw new FormatException("test");
			}
		}

		[Test]
		public void LazyFieldInterceptorMarkedAsSerializable()
		{
			Assert.That(typeof(DefaultDynamicLazyFieldInterceptor), Has.Attribute<SerializableAttribute>());
		}

		[Test]
		public void LazyFieldInterceptorIsBinarySerializable()
		{
			var pf = new DefaultProxyFactory();
			var propertyInfo = typeof(MyClass).GetProperty("Id");
			pf.PostInstantiate("MyClass", typeof(MyClass), new HashSet<System.Type>(), propertyInfo.GetGetMethod(), propertyInfo.GetSetMethod(), null);
			var fieldInterceptionProxy = (IFieldInterceptorAccessor)pf.GetFieldInterceptionProxy(new MyClass());
			fieldInterceptionProxy.FieldInterceptor = new DefaultFieldInterceptor(null, null, null, "MyClass", typeof(MyClass));

			NHAssert.IsSerializable(fieldInterceptionProxy);
		}


		[Test]
		public void DefaultDynamicLazyFieldInterceptorUnWrapsTIEExceptions()
		{
			var pf = new DefaultProxyFactory();
			var propertyInfo = typeof(MyClass).GetProperty("Id");
			pf.PostInstantiate("MyClass", typeof(MyClass), new HashSet<System.Type>(), propertyInfo.GetGetMethod(), propertyInfo.GetSetMethod(), null);
			var myClassProxied = (MyClass)pf.GetFieldInterceptionProxy(new MyClass());
			Assert.Throws<FormatException>(() => myClassProxied.ThrowError(), "test");
		}
	}
}
