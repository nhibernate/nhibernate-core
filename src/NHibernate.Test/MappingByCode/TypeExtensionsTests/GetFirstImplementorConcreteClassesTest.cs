using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.TypeExtensionsTests
{
	public class GetFirstImplementorConcreteClassesTest
	{
		private class MyClass1
		{

		}
		private class MyClass2 : MyClass1
		{

		}
		private class MyClass3 : MyClass2
		{

		}
		private class MyClass4 : MyClass3
		{

		}

		[Test]
		public void WhenImplIsAtSameLevelThenReturnImplementor()
		{
			typeof(MyClass1).GetFirstImplementorOf(typeof(MyClass1)).Should().Be(typeof(MyClass1));
			typeof(MyClass2).GetFirstImplementorOf(typeof(MyClass2)).Should().Be(typeof(MyClass2));
			typeof(MyClass3).GetFirstImplementorOf(typeof(MyClass3)).Should().Be(typeof(MyClass3));
			typeof(MyClass4).GetFirstImplementorOf(typeof(MyClass4)).Should().Be(typeof(MyClass4));
		}

		[Test]
		public void WhenImplIsAtDifferentLevelThenReturnImplementor()
		{
			typeof(MyClass2).GetFirstImplementorOf(typeof(MyClass1)).Should().Be(typeof(MyClass2));
			typeof(MyClass3).GetFirstImplementorOf(typeof(MyClass1)).Should().Be(typeof(MyClass2));
			typeof(MyClass3).GetFirstImplementorOf(typeof(MyClass2)).Should().Be(typeof(MyClass3));
			typeof(MyClass4).GetFirstImplementorOf(typeof(MyClass1)).Should().Be(typeof(MyClass2));
			typeof(MyClass4).GetFirstImplementorOf(typeof(MyClass2)).Should().Be(typeof(MyClass3));
			typeof(MyClass4).GetFirstImplementorOf(typeof(MyClass3)).Should().Be(typeof(MyClass4));
		}

		[Test]
		public void WhenImplIsAtUpLevelThenReturnNull()
		{
			typeof(MyClass2).GetFirstImplementorOf(typeof(MyClass3)).Should().Be.Null();
			typeof(MyClass3).GetFirstImplementorOf(typeof(MyClass4)).Should().Be.Null();
		}
	}
}