using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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
			Assert.That(typeof(MyClass1).GetFirstImplementorOf(typeof(MyClass1)), Is.EqualTo(typeof(MyClass1)));
			Assert.That(typeof(MyClass2).GetFirstImplementorOf(typeof(MyClass2)), Is.EqualTo(typeof(MyClass2)));
			Assert.That(typeof(MyClass3).GetFirstImplementorOf(typeof(MyClass3)), Is.EqualTo(typeof(MyClass3)));
			Assert.That(typeof(MyClass4).GetFirstImplementorOf(typeof(MyClass4)), Is.EqualTo(typeof(MyClass4)));
		}

		[Test]
		public void WhenImplIsAtDifferentLevelThenReturnImplementor()
		{
			Assert.That(typeof(MyClass2).GetFirstImplementorOf(typeof(MyClass1)), Is.EqualTo(typeof(MyClass2)));
			Assert.That(typeof(MyClass3).GetFirstImplementorOf(typeof(MyClass1)), Is.EqualTo(typeof(MyClass2)));
			Assert.That(typeof(MyClass3).GetFirstImplementorOf(typeof(MyClass2)), Is.EqualTo(typeof(MyClass3)));
			Assert.That(typeof(MyClass4).GetFirstImplementorOf(typeof(MyClass1)), Is.EqualTo(typeof(MyClass2)));
			Assert.That(typeof(MyClass4).GetFirstImplementorOf(typeof(MyClass2)), Is.EqualTo(typeof(MyClass3)));
			Assert.That(typeof(MyClass4).GetFirstImplementorOf(typeof(MyClass3)), Is.EqualTo(typeof(MyClass4)));
		}

		[Test]
		public void WhenImplIsAtUpLevelThenReturnNull()
		{
			Assert.That(typeof(MyClass2).GetFirstImplementorOf(typeof(MyClass3)), Is.Null);
			Assert.That(typeof(MyClass3).GetFirstImplementorOf(typeof(MyClass4)), Is.Null);
		}
	}
}