using System;
using System.Collections.Generic;
using NHibernate.Proxy;
using NUnit.Framework;
using NHibernate.Intercept;

namespace NHibernate.Test.DynamicProxyTests
{
	public class LazyFieldInterceptorSerializable
	{
		[Serializable]
		public class MyClass
		{
			public virtual int Id { get; set; }
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

			Assert.That(fieldInterceptionProxy, Is.BinarySerializable);
		}
	}
}