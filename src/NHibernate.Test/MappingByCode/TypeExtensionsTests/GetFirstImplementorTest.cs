using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.TypeExtensionsTests
{
	public class GetFirstImplementorTest
	{
		private interface IInterfaceNoImpl
		{

		}
		private interface IInterface1
		{
			
		}
		private interface IInterface2
		{

		}
		private interface IInterface3
		{

		}
		private class MyClassNoInterface
		{

		}
		private class MyClass1: IInterface1
		{
			
		}
		private class MyClass2: MyClass1, IInterface2
		{

		}
		private class MyClass3 : MyClass2, IInterface3
		{

		}

		[Test]
		public void WhenInvalidThenThrows()
		{
			Executing.This(()=>((System.Type) null).GetFirstImplementorOf(typeof(IInterfaceNoImpl))).Should().Throw<ArgumentNullException>();
			Executing.This(() => typeof(IInterfaceNoImpl).GetFirstImplementorOf(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenIsInterfaceThenNoImplementor()
		{
			typeof(IInterfaceNoImpl).GetFirstImplementorOf(typeof(IInterfaceNoImpl)).Should().Be.Null();
		}

		[Test]
		public void WhenImplAsNoInterfaceThenNoImplementor()
		{
			typeof(MyClassNoInterface).GetFirstImplementorOf(typeof(IInterfaceNoImpl)).Should().Be.Null();
		}

		[Test]
		public void WhenImplIsAtSameLevelThenReturnImplementor()
		{
			typeof(MyClass1).GetFirstImplementorOf(typeof(IInterface1)).Should().Be(typeof(MyClass1));
			typeof(MyClass2).GetFirstImplementorOf(typeof(IInterface2)).Should().Be(typeof(MyClass2));
			typeof(MyClass3).GetFirstImplementorOf(typeof(IInterface3)).Should().Be(typeof(MyClass3));
		}

		[Test]
		public void WhenImplIsAtDifferentLevelThenReturnImplementor()
		{
			typeof(MyClass2).GetFirstImplementorOf(typeof(IInterface1)).Should().Be(typeof(MyClass1));
			typeof(MyClass3).GetFirstImplementorOf(typeof(IInterface2)).Should().Be(typeof(MyClass2));
			typeof(MyClass3).GetFirstImplementorOf(typeof(IInterface1)).Should().Be(typeof(MyClass1));
		}
	}
}