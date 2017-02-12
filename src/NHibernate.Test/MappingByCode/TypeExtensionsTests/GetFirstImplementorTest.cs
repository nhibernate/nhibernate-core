using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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
			Assert.That(() => ((System.Type) null).GetFirstImplementorOf(typeof(IInterfaceNoImpl)), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => typeof(IInterfaceNoImpl).GetFirstImplementorOf(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenIsInterfaceThenNoImplementor()
		{
			Assert.That(typeof(IInterfaceNoImpl).GetFirstImplementorOf(typeof(IInterfaceNoImpl)), Is.Null);
		}

		[Test]
		public void WhenImplAsNoInterfaceThenNoImplementor()
		{
			Assert.That(typeof(MyClassNoInterface).GetFirstImplementorOf(typeof(IInterfaceNoImpl)), Is.Null);
		}

		[Test]
		public void WhenImplIsAtSameLevelThenReturnImplementor()
		{
			Assert.That(typeof(MyClass1).GetFirstImplementorOf(typeof(IInterface1)), Is.EqualTo(typeof(MyClass1)));
			Assert.That(typeof(MyClass2).GetFirstImplementorOf(typeof(IInterface2)), Is.EqualTo(typeof(MyClass2)));
			Assert.That(typeof(MyClass3).GetFirstImplementorOf(typeof(IInterface3)), Is.EqualTo(typeof(MyClass3)));
		}

		[Test]
		public void WhenImplIsAtDifferentLevelThenReturnImplementor()
		{
			Assert.That(typeof(MyClass2).GetFirstImplementorOf(typeof(IInterface1)), Is.EqualTo(typeof(MyClass1)));
			Assert.That(typeof(MyClass3).GetFirstImplementorOf(typeof(IInterface2)), Is.EqualTo(typeof(MyClass2)));
			Assert.That(typeof(MyClass3).GetFirstImplementorOf(typeof(IInterface1)), Is.EqualTo(typeof(MyClass1)));
		}
	}
}