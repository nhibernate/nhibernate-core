using System;
using Iesi.Collections.Generic;
using NHibernate.Intercept;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.ByteCode.Castle.Tests
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
			typeof(LazyFieldInterceptor).Should().Have.Attribute<SerializableAttribute>();
		}

		[Test]
		public void LazyFieldInterceptorIsBinarySerializable()
		{
			var pf = new ProxyFactory();
			var propertyInfo = typeof(MyClass).GetProperty("Id");
			pf.PostInstantiate("MyClass", typeof(MyClass), new HashedSet<System.Type>(), propertyInfo.GetGetMethod(), propertyInfo.GetSetMethod(), null);
			var fieldInterceptionProxy = (IFieldInterceptorAccessor)pf.GetFieldInterceptionProxy();
			fieldInterceptionProxy.FieldInterceptor = new DefaultFieldInterceptor(null, null, null, "MyClass", typeof(MyClass));

			fieldInterceptionProxy.Should().Be.BinarySerializable();
		}
	}
}