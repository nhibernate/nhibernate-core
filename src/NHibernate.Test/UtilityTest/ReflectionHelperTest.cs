using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	public class ReflectionHelperTest
	{
		private class MyClass
		{
			public void NoGenericMethod() {}
			public void GenericMethod<T>() { }
			public string BaseProperty { get; set; }
			public bool BaseBool { get; set; }
		}

		[Test]
		public void WhenGetMethodForNullThenThrows()
		{
			Assert.That(() => ReflectionHelper.GetMethodDefinition((Expression<System.Action>) null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenGenericGetMethodForNullThenThrows()
		{
			Assert.That(() => ReflectionHelper.GetMethodDefinition<object>((Expression<System.Action<object>>)null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenGetPropertyForNullThenThrows()
		{
			Assert.That(() => ReflectionHelper.GetProperty<object, object>(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenGenericMethodOfClassThenReturnGenericDefinition()
		{
			Assert.That(ReflectionHelper.GetMethodDefinition<MyClass>(mc => mc.GenericMethod<int>()), Is.EqualTo(typeof (MyClass).GetMethod("GenericMethod").GetGenericMethodDefinition()));
		}

		[Test]
		public void WhenNoGenericMethodOfClassThenReturnDefinition()
		{
			Assert.That(ReflectionHelper.GetMethodDefinition<MyClass>(mc => mc.NoGenericMethod()), Is.EqualTo(typeof(MyClass).GetMethod("NoGenericMethod")));
		}

		[Test]
		public void WhenStaticGenericMethodThenReturnGenericDefinition()
		{
			Assert.That(ReflectionHelper.GetMethodDefinition(() => Enumerable.All<int>(null, null)), Is.EqualTo(typeof(Enumerable).GetMethod("All").GetGenericMethodDefinition()));
		}

		[Test]
		public void WhenStaticNoGenericMethodThenReturnDefinition()
		{
			Assert.That(ReflectionHelper.GetMethodDefinition(() => string.Join(null, null)), Is.EqualTo(typeof(string).GetMethod("Join", new []{typeof(string), typeof(string[])})));
		}

		[Test]
		public void WhenGetPropertyThenReturnPropertyInfo()
		{
			Assert.That(ReflectionHelper.GetProperty<MyClass, string>(mc => mc.BaseProperty), Is.EqualTo(typeof(MyClass).GetProperty("BaseProperty")));
		}

		[Test]
		public void WhenGetPropertyForBoolThenReturnPropertyInfo()
		{
			Assert.That(ReflectionHelper.GetProperty<MyClass, bool>(mc => mc.BaseBool), Is.EqualTo(typeof(MyClass).GetProperty("BaseBool")));
		}
	}
}