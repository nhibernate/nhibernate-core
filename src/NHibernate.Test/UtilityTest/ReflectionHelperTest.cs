using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;

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
			Executing.This(() => ReflectionHelper.GetMethodDefinition((Expression<System.Action>) null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenGenericGetMethodForNullThenThrows()
		{
			Executing.This(() => ReflectionHelper.GetMethodDefinition<object>((Expression<System.Action<object>>)null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenGetPropertyForNullThenThrows()
		{
			Executing.This(() => ReflectionHelper.GetProperty<object, object>(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenGenericMethodOfClassThenReturnGenericDefinition()
		{
			ReflectionHelper.GetMethodDefinition<MyClass>(mc => mc.GenericMethod<int>()).Should().Be(typeof (MyClass).GetMethod("GenericMethod").GetGenericMethodDefinition());
		}

		[Test]
		public void WhenNoGenericMethodOfClassThenReturnDefinition()
		{
			ReflectionHelper.GetMethodDefinition<MyClass>(mc => mc.NoGenericMethod()).Should().Be(typeof(MyClass).GetMethod("NoGenericMethod"));
		}

		[Test]
		public void WhenStaticGenericMethodThenReturnGenericDefinition()
		{
			ReflectionHelper.GetMethodDefinition(() => Enumerable.All<int>(null, null)).Should().Be(typeof(Enumerable).GetMethod("All").GetGenericMethodDefinition());
		}

		[Test]
		public void WhenStaticNoGenericMethodThenReturnDefinition()
		{
			ReflectionHelper.GetMethodDefinition(() => string.Join(null, null)).Should().Be(typeof(string).GetMethod("Join", new []{typeof(string), typeof(string[])}));
		}

		[Test]
		public void WhenGetPropertyThenReturnPropertyInfo()
		{
			ReflectionHelper.GetProperty<MyClass, string>(mc => mc.BaseProperty).Should().Be(typeof(MyClass).GetProperty("BaseProperty"));
		}

		[Test]
		public void WhenGetPropertyForBoolThenReturnPropertyInfo()
		{
			ReflectionHelper.GetProperty<MyClass, bool>(mc => mc.BaseBool).Should().Be(typeof(MyClass).GetProperty("BaseBool"));
		}
	}
}